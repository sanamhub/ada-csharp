#!/usr/bin/env bash
# Consumes the built NuGet package the way a real user would, from a clean project with no
# reference to this repository's source.
#
# This is the check that would have caught the package shipping with no runtimes folder. The
# library built, the tests passed, and the package installed cleanly. It only broke at the
# consumer's first call, which is the worst place to find out.
set -euo pipefail

PACKAGE_DIR=""
EXPECT_RID=""
AOT="false"

while [ $# -gt 0 ]; do
  case "$1" in
    --package-dir) PACKAGE_DIR="$2"; shift 2 ;;
    --rid)         EXPECT_RID="$2";  shift 2 ;;
    --aot)         AOT="true";       shift 1 ;;
    *) echo "unknown argument: $1" >&2; exit 2 ;;
  esac
done

[ -n "$PACKAGE_DIR" ] || { echo "--package-dir is required" >&2; exit 2; }

PACKAGE_DIR="$(cd "$PACKAGE_DIR" && pwd)"
VERSION="$(ls "$PACKAGE_DIR"/Ada.Url.*.nupkg | head -1 | sed 's/.*Ada\.Url\.\(.*\)\.nupkg/\1/')"
[ -n "$VERSION" ] || { echo "no Ada.Url package found in $PACKAGE_DIR" >&2; exit 1; }

# Under Git Bash, pwd returns an MSYS path like /d/a/repo/out, and .NET reads that as
# C:\d\a\repo\out, which does not exist. Anything handed to a .NET tool or to Python needs the
# native form, while the shell keeps using the MSYS one.
NATIVE_PACKAGE_DIR="$PACKAGE_DIR"
if command -v cygpath >/dev/null 2>&1; then
  NATIVE_PACKAGE_DIR="$(cygpath -w "$PACKAGE_DIR")"
fi

echo "consuming Ada.Url $VERSION from $NATIVE_PACKAGE_DIR"

WORK="$(mktemp -d)"
trap 'rm -rf "$WORK"' EXIT
cd "$WORK"

# Ada.Url must come from the local build, or this test checks the wrong artifact. A local-only
# feed enforces that, but it also starves NativeAOT, which pulls the ILCompiler packages from
# nuget.org and fails with NU1101 when nothing else is configured.
#
# Package source mapping gives both: Ada.Url can only resolve from local, everything else only
# from nuget.org. Narrower than a local-only feed rather than looser, because it also pins where
# every other dependency is allowed to come from.
cat > NuGet.Config <<XML
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$NATIVE_PACKAGE_DIR" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="local">
      <package pattern="Ada.Url" />
    </packageSource>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>
  </packageSourceMapping>
</configuration>
XML

cat > consumer.csproj <<XML
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <!-- Set explicitly. This project lives in a temp directory on purpose, so it inherits none
         of the repository's Directory.Build.props and gets no defaults from it. -->
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>Consumer</RootNamespace>
    <PublishAot>$AOT</PublishAot>
    <InvariantGlobalization>true</InvariantGlobalization>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Ada.Url" Version="$VERSION" />
  </ItemGroup>
</Project>
XML

cat > Program.cs <<'CS'
using System.Text;
using Ada.Url;

// Deliberately exercises a native call, a borrowed span, an owned native string, and the
// enumerator, so a partly working deployment cannot pass this.
if (!AdaUrl.TryParse("https://user:pass@Bücher.example:8443/a/../b?q=1#f"u8, out AdaUrl url))
{
    Console.Error.WriteLine("FAIL: parse returned false");
    return 1;
}

using (url)
{
    string host = Encoding.UTF8.GetString(url.Hostname);
    string path = Encoding.UTF8.GetString(url.Pathname);

    if (host != "xn--bcher-kva.example") { Console.Error.WriteLine($"FAIL: hostname was '{host}'"); return 1; }
    if (path != "/b") { Console.Error.WriteLine($"FAIL: pathname was '{path}'"); return 1; }

    Span<byte> origin = stackalloc byte[64];
    if (!url.TryGetOrigin(origin, out int written)) { Console.Error.WriteLine("FAIL: origin"); return 1; }
    Console.WriteLine($"origin: {Encoding.UTF8.GetString(origin[..written])}");
}

using (var parameters = AdaSearchParams.Parse("a=1&b=2"u8))
{
    int count = 0;
    foreach (AdaSearchParams.Entry entry in parameters) { count += entry.Key.Length; }
    if (count != 2) { Console.Error.WriteLine($"FAIL: enumerated {count}"); return 1; }
}

Console.WriteLine($"native version: {AdaLibrary.NativeVersion}");
if (AdaLibrary.NativeVersion != AdaLibrary.PinnedVersion)
{
    Console.Error.WriteLine($"FAIL: loaded native {AdaLibrary.NativeVersion}, expected {AdaLibrary.PinnedVersion}");
    return 1;
}

Console.WriteLine("PASS");
return 0;
CS

dotnet restore --verbosity quiet

if [ "$AOT" = "true" ]; then
  echo "publishing with NativeAOT"
  dotnet publish -c Release -o out --verbosity quiet
  ./out/consumer
else
  dotnet run -c Release --verbosity quiet
fi

# The package must carry the native for the platform under test. A missing RID shows up above as
# a DllNotFoundException, but naming it here makes the cause obvious rather than something to
# work out from a stack trace.
if [ -n "$EXPECT_RID" ]; then
  echo "checking the package contains runtimes/$EXPECT_RID/native"
  # Windows runners expose python, not python3.
  PY_BIN="python3"
  command -v python3 >/dev/null 2>&1 || PY_BIN="python"
  "$PY_BIN" - "$NATIVE_PACKAGE_DIR/Ada.Url.$VERSION.nupkg" "$EXPECT_RID" <<'PY'
import sys, zipfile
names = zipfile.ZipFile(sys.argv[1]).namelist()
prefix = f"runtimes/{sys.argv[2]}/native/"
found = [n for n in names if n.startswith(prefix)]
if not found:
    rids = sorted({n.split('/')[1] for n in names if n.startswith('runtimes/')})
    print(f"FAIL: no {prefix} in the package. It contains: {rids}", file=sys.stderr)
    sys.exit(1)
print(f"  found {found}")
PY
fi

echo "package consumption OK"
