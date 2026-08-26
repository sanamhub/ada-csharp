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
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$AdaTag,
    [string]$Rid = 'win-x64'
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

# No /GL and no /LTCG.
#
# Ada has no __declspec(dllexport), so the DLL depends on upstream's WINDOWS_EXPORT_ALL_SYMBOLS,
# which makes CMake run `cmake -E __create_def` to build an exports file by reading the compiled
# objects. With /GL those objects hold IL rather than COFF symbols, and __create_def dies with
# 0xC0000005 reading them. It crashed on simdutf first, then on ada.vcxproj once simdutf was
# turned off, so it is /GL and not the dependency.
#
# The choice is whole program optimisation with no exports, or a working DLL. /OPT:REF and
# /OPT:ICF still run at link time, so this is not the whole of LTO gone.
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

if (Test-Path $build) { Remove-Item -Recurse -Force $build }

cmake -S $src -B $build -G 'Visual Studio 17 2022' -A x64 `
    -DCMAKE_BUILD_TYPE=Release `
    -DBUILD_SHARED_LIBS=ON `
    -DADA_TESTING=OFF -DADA_BENCHMARKS=OFF -DADA_TOOLS=OFF `
    -DADA_USE_SIMDUTF=OFF `
    -DCMAKE_INTERPROCEDURAL_OPTIMIZATION=OFF `
    -DCMAKE_CXX_STANDARD=20 -DCMAKE_CXX_STANDARD_REQUIRED=ON `
    -DCMAKE_MSVC_RUNTIME_LIBRARY=MultiThreadedDLL `
    "-DCMAKE_CXX_FLAGS_RELEASE=$cxxFlags" `
    "-DCMAKE_SHARED_LINKER_FLAGS_RELEASE=$linkFlags"
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
