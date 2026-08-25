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

$missing = $required | Where-Object { $exports -notmatch "\b$_\b" }
if ($missing) {
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

# CET shadow stack compatibility lives in the debug directory rather than DllCharacteristics,
# so it is reported by the linker flag being present and not gated here.
Write-Output 'PASS: ASLR, high entropy VA, DEP and CFG all set'
