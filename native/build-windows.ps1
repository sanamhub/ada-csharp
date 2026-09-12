#Requires -Version 7
<#
.SYNOPSIS
    Builds Ada as a shared library for win-x64.
.DESCRIPTION
    Baseline is x86-64-v2, not a static AVX2 build, so the artifact runs on any CPU from 2009
    onward instead of raising the floor to Haswell.

    ADA_USE_SIMDUTF is OFF. With BUILD_SHARED_LIBS=ON it propagates to simdutf, and building
    simdutf as a DLL crashes cmake -E __create_def while generating exports.def. See ADR-0003.

    MultiThreadedDLL matches the CRT that .NET processes already load. A static CRT inside a DLL
    sitting next to .NET is a heap mismatch waiting to happen.

    -Toolset and -Exports exist to measure issues #18 and #19. Both default to what this script
    has always done, so a normal build is unchanged. Whichever variant does not win gets deleted
    rather than left here as dead configuration.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$AdaTag,
    [string]$Rid = 'win-x64',

    # msvc is cl. clang-cl is the LLVM toolset that ships inside Visual Studio 2022. Same
    # source, different optimiser, which is the whole question in #19.
    [ValidateSet('msvc', 'clang-cl')][string]$Toolset = 'msvc',

    # all-symbols leans on upstream's WINDOWS_EXPORT_ALL_SYMBOLS and exports every mangled C++
    # symbol in the library. def generates an export list from upstream's include/ada_c.h and
    # exports only the ada_* C API, which also lets /GL back in. See #18.
    [ValidateSet('all-symbols', 'def')][string]$Exports = 'all-symbols'
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
# /guard:cf, /DYNAMICBASE, /HIGHENTROPYVA and /CETCOMPAT are required hardening.
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
$linkFlags = '/OPT:REF /OPT:ICF /INCREMENTAL:NO /DEBUG /GUARD:CF /DYNAMICBASE /HIGHENTROPYVA /CETCOMPAT /Brepro /PDBALTPATH:%_PDB%'

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
#
# Off for clang-cl in every case. That MSBuild property drives cl's /GL and does nothing useful
# for the LLVM toolset, whose LTO wants lld-link rather than link.exe. Thin LTO under clang-cl is
# a separate experiment, not this one.
$ipo = if ($Toolset -eq 'msvc' -and $Exports -eq 'def') { 'ON' } else { 'OFF' }

if ($Toolset -eq 'clang-cl') { $injects += Join-Path $PSScriptRoot 'cmake/clang-cl-tweaks.cmake' }

if (Test-Path $build) { Remove-Item -Recurse -Force $build }

$cmakeArgs = @(
    '-S', $src, '-B', $build, '-G', 'Visual Studio 17 2022', '-A', 'x64',
    '-DCMAKE_BUILD_TYPE=Release',
    '-DBUILD_SHARED_LIBS=ON',
    '-DADA_TESTING=OFF', '-DADA_BENCHMARKS=OFF', '-DADA_TOOLS=OFF',
    '-DADA_USE_SIMDUTF=OFF',
    "-DCMAKE_INTERPROCEDURAL_OPTIMIZATION=$ipo",
    '-DCMAKE_CXX_STANDARD=20', '-DCMAKE_CXX_STANDARD_REQUIRED=ON',
    '-DCMAKE_MSVC_RUNTIME_LIBRARY=MultiThreadedDLL',
    "-DCMAKE_CXX_FLAGS_RELEASE=$cxxFlags",
    "-DCMAKE_SHARED_LINKER_FLAGS_RELEASE=$linkFlags"
)

if ($Toolset -eq 'clang-cl') { $cmakeArgs += @('-T', 'ClangCL') }

# CMAKE_PROJECT_TOP_LEVEL_INCLUDES runs our scripts straight after upstream's project() call,
# which is how both variants change the ada target without a patch landing in the clone. A
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
        throw "cmake $cmakeVersion is too old. -Toolset clang-cl and -Exports def both need CMAKE_PROJECT_TOP_LEVEL_INCLUDES, which is CMake 3.24 or later."
    }

    $list = ($injects | ForEach-Object { $_.Replace('\', '/') }) -join ';'
    $cmakeArgs += "-DCMAKE_PROJECT_TOP_LEVEL_INCLUDES=$list"
}

Write-Output "configuring $Rid with toolset=$Toolset exports=$Exports ipo=$ipo"

cmake @cmakeArgs
if ($LASTEXITCODE -ne 0) { throw "cmake configure failed with exit code $LASTEXITCODE" }

cmake --build $build --config Release --parallel
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
