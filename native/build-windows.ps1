#Requires -Version 7
<#
.SYNOPSIS
    Builds Ada as a shared library for win-x64 or win-arm64.
.DESCRIPTION
    On x64 the baseline is x86-64-v2, not a static AVX2 build, so the artifact runs on any CPU
    from 2009 onward instead of raising the floor to Haswell. arm64 has no equivalent floor to
    pick, so there is nothing to say there.

    win-arm64 cross compiles from an x64 host with -A ARM64, so it needs no separate build
    machine, only the ARM64 toolset installed alongside MSVC. /CETCOMPAT comes off, because CET
    is x86 and x64 only and there is no arm64 equivalent to ask for. Everything else that
    hardens the binary stays. The machine type of the output is checked by
    native/verify-windows.ps1 rather than trusted, because a silently x64 binary shipped under
    this RID would fail on exactly the machines the RID exists for.

    ADA_USE_SIMDUTF is OFF. With BUILD_SHARED_LIBS=ON it propagates to simdutf, and building
    simdutf as a DLL crashes cmake -E __create_def while generating exports.def. See ADR-0003.

    MultiThreadedDLL matches the CRT that .NET processes already load. A static CRT inside a DLL
    sitting next to .NET is a heap mismatch waiting to happen.

    -Exports defaults to def: the export list is generated from upstream's include/ada_c.h and
    whole program optimisation is on. That is measured, not assumed. See ADR-0006 and #18.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$AdaTag,
    [ValidateSet('win-x64', 'win-arm64')][string]$Rid = 'win-x64',

    # def generates an export list from upstream's include/ada_c.h, exports only the ada_* C API,
    # and lets /GL back in. It is the default because it measured 7% faster on the hard URL path
    # and halves the DLL. See ADR-0006.
    #
    # all-symbols is upstream's WINDOWS_EXPORT_ALL_SYMBOLS, exporting every mangled C++ symbol in
    # the library with no whole program optimisation. Kept so the comparison can be rerun, which
    # #18 will want when the pinned tag moves.
    [ValidateSet('def', 'all-symbols')][string]$Exports = 'def',

    # auto ties whole program optimisation to -Exports, which is the shipping behaviour: def
    # implies it, all-symbols cannot have it. on and off override that, and the only reason they
    # exist is #45. Whether /GL is what makes the Windows output track the runner image needs
    # def exports with /GL off, and auto cannot express that combination.
    [ValidateSet('auto', 'on', 'off')][string]$Ipo = 'auto'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$root  = Split-Path -Parent $PSScriptRoot
$src   = Join-Path $root 'native/ada-src'
$build = Join-Path $root "native/build/$Rid"
$out   = Join-Path $root "artifacts/native/$Rid"

if (-not (Test-Path (Join-Path $src '.git'))) {
    git clone --depth 1 --branch $AdaTag https://github.com/ada-url/ada.git $src
    if ($LASTEXITCODE -ne 0) { throw "git clone failed with exit code $LASTEXITCODE" }
}

<#
.SYNOPSIS
    Writes a module definition file listing every ada_* function declared in include/ada_c.h.
.DESCRIPTION
    Generated from the pinned clone on every build, never hand maintained. A hand written list
    is a second copy of upstream's API, and that copy goes stale on the next tag bump without
    anything failing to say so.

    Split on semicolons rather than matched line by line, because several declarations wrap
    across two lines. Statements with no parentheses are types and macros, and typedefs are
    skipped outright, so what is left is the function declarations.
#>
function New-AdaExportsDef {
    param([Parameter(Mandatory)][string]$Header, [Parameter(Mandatory)][string]$OutFile)

    $text = Get-Content -Raw $Header
    $text = [regex]::Replace($text, '/\*.*?\*/', ' ', [Text.RegularExpressions.RegexOptions]::Singleline)
    $text = [regex]::Replace($text, '//[^\r\n]*', ' ')
    $text = (($text -split "`n") | Where-Object { $_.TrimStart() -notmatch '^#' }) -join "`n"

    $names = [System.Collections.Generic.List[string]]::new()
    foreach ($statement in ($text -split ';')) {
        if ($statement -notmatch '\(') { continue }
        if ($statement -match '(?s)^\s*typedef\b') { continue }
        $found = [regex]::Match($statement, '([A-Za-z_]\w*)\s*\(')
        if ($found.Success -and $found.Groups[1].Value.StartsWith('ada_')) {
            $names.Add($found.Groups[1].Value)
        }
    }

    $names = @($names | Sort-Object -Unique)

    # v4.0.0 declares 79. A parser that quietly matched almost nothing would produce a DLL that
    # exports almost nothing, which links, ships, and then throws EntryPointNotFoundException on
    # the consumer's first call. Fail here instead.
    if ($names.Count -lt 60) {
        throw "only $($names.Count) ada_* functions found in $Header, expected at least 60. The parser is wrong or the header moved."
    }

    $lines = @('EXPORTS') + ($names | ForEach-Object { "    $_" })
    Set-Content -Path $OutFile -Value $lines -Encoding ascii
    Write-Output "generated $OutFile with $($names.Count) exports"
}

# /GL only when the export list does not depend on cmake -E __create_def.
#
# Ada has no __declspec(dllexport), so the all-symbols build depends on upstream's
# WINDOWS_EXPORT_ALL_SYMBOLS, which makes CMake run `cmake -E __create_def` to build an exports
# file by reading the compiled objects. With /GL those objects hold IL rather than COFF symbols,
# and __create_def dies with 0xC0000005 reading them. It crashed on simdutf first, then on
# ada.vcxproj once simdutf was turned off, so it is /GL and not the dependency.
#
# A .def file takes __create_def out of the build, which takes the crash with it. Whether /GL
# then buys anything is the open question: upstream's src/ada.cpp includes every other .cpp, so
# the library is one translation unit and whole program optimisation has nothing to cross. The
# CRT is dynamic, so there is no CRT LTCG either. #18 measures it.
#
# /OPT:REF and /OPT:ICF run either way.
#
# /guard:cf, /DYNAMICBASE and /HIGHENTROPYVA are required hardening on both architectures.
# /CETCOMPAT is x64 only and is added below.
#
# /Brepro is what makes the checksum manifest mean anything. By default MSVC stamps the PE header
# with the build time and the debug directory with a fresh PDB signature, so two builds of
# identical source produce different bytes. That was not theoretical: 3971fb8a and 69163c67 are
# the same commit built twice. With no way to reproduce a binary, a committed hash cannot tell a
# rebuild apart from a substitution, which is the only thing it exists to detect.
#
# /PDBALTPATH:%_PDB% stores the PDB file name rather than its full path in the debug directory,
# so the binary does not depend on where the build happened.
#
# The five Unix RIDs already reproduce byte for byte with no extra flags.
$cxxFlags  = '/O2 /Ob3 /Oi /Gy /Gw /EHsc /DNDEBUG /Zi /guard:cf /Brepro'
$linkFlags = '/OPT:REF /OPT:ICF /INCREMENTAL:NO /DEBUG /GUARD:CF /DYNAMICBASE /HIGHENTROPYVA /Brepro /PDBALTPATH:%_PDB%'

# CET shadow stacks are an x86 and x64 feature. link.exe accepts /CETCOMPAT on an arm64 target
# and emits nothing, which is the failure verify-windows.ps1 was taught to catch, so do not pass
# a flag whose only possible outcome here is a gate that has to be weakened to let it through.
$arch = if ($Rid -eq 'win-arm64') { 'ARM64' } else { 'x64' }
if ($arch -eq 'x64') { $linkFlags += ' /CETCOMPAT' }

$injects = @()

if ($Exports -eq 'def') {
    $defFile = Join-Path $root "native/build/$Rid-exports.def"
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $defFile) | Out-Null
    New-AdaExportsDef -Header (Join-Path $src 'include/ada_c.h') -OutFile $defFile

    # Quoted, because the linker flags are one space delimited string by the time MSBuild sees
    # them. A checkout under a path with a space in it would otherwise split this option in half
    # and the linker would look for a definition file that does not exist.
    $linkFlags += " /DEF:`"$defFile`""
    $injects += Join-Path $PSScriptRoot 'cmake/no-export-all-symbols.cmake'
}

# Whole program optimisation goes through CMAKE_INTERPROCEDURAL_OPTIMIZATION rather than a raw
# /GL, so MSBuild sets WholeProgramOptimization on the compile and LinkTimeCodeGeneration on the
# link together. Setting one by hand and forgetting the other is LNK1257 at the end of a long
# build.
# Not $ipo: PowerShell variable names are case insensitive, so that would be the $Ipo parameter
# and this would overwrite the argument it is reading.
$ipoValue = switch ($Ipo) {
    'on'  { 'ON' }
    'off' { 'OFF' }
    default { if ($Exports -eq 'def') { 'ON' } else { 'OFF' } }
}

# all-symbols leaves WINDOWS_EXPORT_ALL_SYMBOLS on, so cmake -E __create_def reads the compiled
# objects, and under /GL those hold IL and it dies with 0xC0000005. Refuse the combination here
# rather than an hour into a build with an access violation and no explanation. ADR-0006.
if ($Exports -eq 'all-symbols' -and $ipoValue -eq 'ON') {
    throw "-Exports all-symbols cannot be combined with -Ipo on: cmake -E __create_def crashes on IL objects. See ADR-0006."
}

if (Test-Path $build) { Remove-Item -Recurse -Force $build }

$cmakeArgs = @(
    '-S', $src, '-B', $build, '-G', 'Visual Studio 17 2022', '-A', $arch,
    '-DCMAKE_BUILD_TYPE=Release',
    '-DBUILD_SHARED_LIBS=ON',
    '-DADA_TESTING=OFF', '-DADA_BENCHMARKS=OFF', '-DADA_TOOLS=OFF',
    '-DADA_USE_SIMDUTF=OFF',
    "-DCMAKE_INTERPROCEDURAL_OPTIMIZATION=$ipoValue",
    '-DCMAKE_CXX_STANDARD=20', '-DCMAKE_CXX_STANDARD_REQUIRED=ON',
    '-DCMAKE_MSVC_RUNTIME_LIBRARY=MultiThreadedDLL',
    "-DCMAKE_CXX_FLAGS_RELEASE=$cxxFlags",
    "-DCMAKE_SHARED_LINKER_FLAGS_RELEASE=$linkFlags"
)

# CMAKE_PROJECT_TOP_LEVEL_INCLUDES runs our script straight after upstream's project() call,
# which is how def mode turns WINDOWS_EXPORT_ALL_SYMBOLS off without a patch landing in the clone. A
# patched clone would mean the artifact is no longer the pinned upstream tag, and the checksum
# manifest and the SBOM both rest on it being exactly that.
#
# It arrived in CMake 3.24, and an older CMake drops an unknown -D without a word. def mode would
# then keep WINDOWS_EXPORT_ALL_SYMBOLS on, run __create_def across IL objects, and die with the
# 0xC0000005 this whole parameter exists to avoid. Say which version is missing instead.
if ($injects.Count -gt 0) {
    $reported = @(cmake --version)[0]
    if ($reported -notmatch '(\d+)\.(\d+)') { throw "could not read a version out of `"$reported`"" }
    $cmakeVersion = [version]"$($Matches[1]).$($Matches[2])"
    if ($cmakeVersion -lt [version]'3.24') {
        throw "cmake $cmakeVersion is too old. -Exports def needs CMAKE_PROJECT_TOP_LEVEL_INCLUDES, which is CMake 3.24 or later."
    }

    $list = ($injects | ForEach-Object { $_.Replace('\', '/') }) -join ';'
    $cmakeArgs += "-DCMAKE_PROJECT_TOP_LEVEL_INCLUDES=$list"
}

Write-Output "configuring $Rid with arch=$arch exports=$Exports ipo=$ipoValue"

cmake @cmakeArgs
if ($LASTEXITCODE -ne 0) { throw "cmake configure failed with exit code $LASTEXITCODE" }

$buildArgs = @('--build', $build, '--config', 'Release', '--parallel')

cmake @buildArgs
if ($LASTEXITCODE -ne 0) { throw "cmake build failed with exit code $LASTEXITCODE" }

New-Item -ItemType Directory -Force -Path $out | Out-Null

$dll = Get-ChildItem -Path $build -Filter 'ada.dll' -Recurse -File | Select-Object -First 1
if (-not $dll) {
    Write-Output 'build produced no ada.dll. What it did produce:'
    Get-ChildItem -Path $build -Filter '*.dll' -Recurse -File | ForEach-Object { $_.FullName }
    throw 'build produced no ada.dll'
}
Copy-Item $dll.FullName (Join-Path $out 'ada.dll') -Force

# The PDB goes to a symbol server, not into the package. Keep it as a separate CI artifact so
# future crash dumps are readable.
$pdb = Get-ChildItem -Path $build -Filter 'ada.pdb' -Recurse -File | Select-Object -First 1
if ($pdb) { Copy-Item $pdb.FullName (Join-Path $out 'ada.pdb') -Force }

Write-Output "built $Rid from ${AdaTag}:"
Get-ChildItem $out | Format-Table Name, Length
