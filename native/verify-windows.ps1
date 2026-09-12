#Requires -Version 7
<#
.SYNOPSIS
    Export and hardening gates for a win-x64 artifact.
.DESCRIPTION
    The export check uses dumpbin, located through vswhere so the script does not depend on a
    developer shell being active.

    The hardening check reads the PE DllCharacteristics field directly instead of parsing
    dumpbin text output. It is a handful of byte offsets, it needs no toolchain, and it cannot
    drift when dumpbin changes its wording.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory, Position = 0)][string]$ArtifactDir
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$dll = Join-Path $ArtifactDir 'ada.dll'
if (-not (Test-Path $dll)) { throw "no ada.dll in $ArtifactDir" }

Write-Output "verifying $dll"

# --- Exports -----------------------------------------------------------------------------
# Any one of these missing means the wrapper cannot function at all.
$required = @('ada_parse', 'ada_free', 'ada_get_href', 'ada_free_owned_string')

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$dumpbin = $null
if (Test-Path $vswhere) {
    $vsRoot = & $vswhere -latest -products '*' -property installationPath
    if ($vsRoot) {
        $dumpbin = Get-ChildItem -Path (Join-Path $vsRoot 'VC/Tools/MSVC') `
                                 -Filter 'dumpbin.exe' -Recurse -File -ErrorAction SilentlyContinue |
                   Where-Object { $_.FullName -match 'Hostx64\\x64' } |
                   Select-Object -First 1 -ExpandProperty FullName
    }
}
if (-not $dumpbin) { $dumpbin = (Get-Command dumpbin.exe -ErrorAction SilentlyContinue)?.Source }
if (-not $dumpbin) { throw 'dumpbin.exe not found. Install the MSVC toolset or run inside a developer shell.' }

$exports = & $dumpbin /nologo /exports $dll
if ($LASTEXITCODE -ne 0) { throw "dumpbin failed with exit code $LASTEXITCODE" }

# -match and -notmatch against an ARRAY return the matching or non-matching elements, not a
# boolean. "$exports -notmatch $sym" is therefore a non-empty array, which is truthy, for every
# symbol, so this reported everything missing from a DLL that exported all of it. Select-String
# -Quiet returns an actual boolean.
$missing = @()
foreach ($sym in $required) {
    if (-not ($exports | Select-String -SimpleMatch $sym -Quiet)) {
        $missing += $sym
    }
}

if ($missing.Count -gt 0) {
    Write-Output 'ada_* symbols dumpbin actually reported:'
    $found = $exports | Select-String -Pattern 'ada_\w+' | Select-Object -First 20
    if ($found) { $found | ForEach-Object { "  $_" } } else { '  (none at all)' }
    Write-Error "FAIL: missing exported symbols: $($missing -join ', ')"
    exit 1
}

$total = ($exports | Select-String -Pattern '\bada_\w+' -AllMatches).Matches.Count
Write-Output "PASS: exports present, $total ada_* symbols total"

# --- Hardening ---------------------------------------------------------------------------
$bytes = [System.IO.File]::ReadAllBytes($dll)

if ([System.BitConverter]::ToUInt16($bytes, 0) -ne 0x5A4D) { throw 'not a PE file, no MZ header' }
$peOffset = [System.BitConverter]::ToInt32($bytes, 0x3C)
if ([System.BitConverter]::ToUInt32($bytes, $peOffset) -ne 0x00004550) { throw 'not a PE file, no PE signature' }

# COFF header is 20 bytes, so the optional header starts 24 bytes past the signature.
# DllCharacteristics sits at offset 0x46 in the optional header for both PE32 and PE32+.
$optionalHeader = $peOffset + 24
$characteristics = [System.BitConverter]::ToUInt16($bytes, $optionalHeader + 0x46)

$flags = [ordered]@{
    'High entropy VA' = 0x0020
    'Dynamic base'    = 0x0040
    'NX compatible'   = 0x0100
    'Control Flow Guard' = 0x4000
}

$failed = @()
foreach ($name in $flags.Keys) {
    $set = ($characteristics -band $flags[$name]) -ne 0
    Write-Output ("  {0,-20} {1}" -f $name, $(if ($set) { 'yes' } else { 'NO' }))
    if (-not $set) { $failed += $name }
}

if ($failed) {
    Write-Error "FAIL: hardening flags missing: $($failed -join ', ')"
    exit 1
}

Write-Output 'PASS: ASLR, high entropy VA, DEP and CFG all set'

# --- CET ---------------------------------------------------------------------------------
# CET shadow stack compatibility is not in DllCharacteristics. The linker records it in the debug
# directory, as an IMAGE_DEBUG_TYPE_EX_DLLCHARACTERISTICS entry whose payload has bit 0x0001 set.
#
# This used to go ungated, on the reasoning that passing /CETCOMPAT was proof enough. It is not.
# A different toolset can take the flag and emit no record, and the artifact then ships with no
# shadow stack support and nothing anywhere says so. Read the record.

$machine = [System.BitConverter]::ToUInt16($bytes, $peOffset + 4)

# /CETCOMPAT is x64 only. There is no arm64 equivalent to look for, so this is skipped rather
# than failed for win-arm64.
if ($machine -ne 0x8664) {
    Write-Output ('SKIP: CET is x64 only, and this is machine 0x{0:X4}' -f $machine)
    exit 0
}

$magic          = [System.BitConverter]::ToUInt16($bytes, $optionalHeader)
$dataDirectories = $optionalHeader + $(if ($magic -eq 0x20B) { 0x70 } else { 0x60 })

# Data directory 6 is the debug directory.
$debugRva  = [System.BitConverter]::ToUInt32($bytes, $dataDirectories + 6 * 8)
$debugSize = [System.BitConverter]::ToUInt32($bytes, $dataDirectories + 6 * 8 + 4)
if ($debugRva -eq 0 -or $debugSize -eq 0) { Write-Error 'FAIL: no debug directory, so no CET record'; exit 1 }

# Section headers follow the optional header, 40 bytes each, and are what turns a virtual
# address back into a file offset.
$sectionCount = [System.BitConverter]::ToUInt16($bytes, $peOffset + 6)
$sectionStart = $optionalHeader + [System.BitConverter]::ToUInt16($bytes, $peOffset + 20)

function Resolve-Rva {
    param([Parameter(Mandatory)][uint32]$Rva)

    for ($i = 0; $i -lt $sectionCount; $i++) {
        $header      = $sectionStart + $i * 40
        $virtual     = [System.BitConverter]::ToUInt32($bytes, $header + 12)
        $rawSize     = [System.BitConverter]::ToUInt32($bytes, $header + 16)
        $rawPointer  = [System.BitConverter]::ToUInt32($bytes, $header + 20)
        if ($Rva -ge $virtual -and $Rva -lt ($virtual + $rawSize)) {
            return $rawPointer + ($Rva - $virtual)
        }
    }
    throw "RVA 0x$('{0:X}' -f $Rva) is in no section"
}

$cetCompatible = $false
$debugOffset = Resolve-Rva -Rva $debugRva

for ($entry = 0; $entry -lt [int]($debugSize / 28); $entry++) {
    $at = $debugOffset + $entry * 28
    if ([System.BitConverter]::ToUInt32($bytes, $at + 12) -ne 20) { continue }  # EX_DLLCHARACTERISTICS

    $payloadSize   = [System.BitConverter]::ToUInt32($bytes, $at + 16)
    $payloadOffset = [System.BitConverter]::ToUInt32($bytes, $at + 24)
    if ($payloadSize -lt 4) { continue }

    $extended = [System.BitConverter]::ToUInt32($bytes, $payloadOffset)
    $cetCompatible = ($extended -band 0x0001) -ne 0
    break
}

if (-not $cetCompatible) {
    Write-Error 'FAIL: no CET compatibility record. The linker took /CETCOMPAT and emitted nothing, or the flag was dropped.'
    exit 1
}

Write-Output 'PASS: CET compatible'
