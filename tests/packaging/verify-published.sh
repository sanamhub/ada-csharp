#!/usr/bin/env bash
# Installs Ada.Url from nuget.org into a throwaway project and runs it.
#
# This is different from verify-package.sh, which tests the artifact a build just produced. This
# tests what nuget.org actually serves, which is the only thing a user ever sees. A package can
# pass every local check and still be wrong on the feed: the wrong files uploaded, indexing
# incomplete, or a version that resolves to something unexpected.
#
# Run it after publishing, and again a few minutes later, since nuget.org indexing is not instant.
set -euo pipefail

VERSION=""
KEEP="false"
WAIT_MINUTES="0"

while [ $# -gt 0 ]; do
  case "$1" in
    --version)       VERSION="$2";      shift 2 ;;
    --keep)          KEEP="true";       shift 1 ;;
    --wait-minutes)  WAIT_MINUTES="$2"; shift 2 ;;
    *) echo "unknown argument: $1" >&2; exit 2 ;;
  esac
done

[ -n "$VERSION" ] || { echo "usage: verify-published.sh --version <version> [--wait-minutes N] [--keep]" >&2; exit 2; }

# nuget.org validates and indexes a new package before anything can restore it, and documents that
# as taking up to an hour. An earlier version of this allowed three minutes, then declared a
# perfectly good release broken. Waiting is the normal case here, not the error case.
echo "checking nuget.org for Ada.Url $VERSION"
[ "$WAIT_MINUTES" != "0" ] && echo "  will wait up to $WAIT_MINUTES minutes for validation and indexing"

deadline=$(( $(date +%s) + WAIT_MINUTES * 60 ))
attempt=0
while :; do
  attempt=$(( attempt + 1 ))

  # Ask the flat container directly. This is the same index restore uses, so if the version is not
  # here yet, no amount of retrying dotnet restore will help.
  INDEX="$(curl -sS --fail "https://api.nuget.org/v3-flatcontainer/ada.url/index.json" 2>/dev/null || true)"

  if [ -n "$INDEX" ] && grep -q "\"$VERSION\"" <<< "$INDEX"; then
    echo "  version is indexed (attempt $attempt)"
    break
  fi

  if [ "$(date +%s)" -ge "$deadline" ]; then
    if [ -z "$INDEX" ]; then
      echo "nuget.org still has no Ada.Url package after $WAIT_MINUTES minutes." >&2
    else
      echo "Ada.Url is on nuget.org but $VERSION never appeared after $WAIT_MINUTES minutes." >&2
      echo "Versions currently indexed:" >&2
      echo "$INDEX" >&2
    fi
    exit 1
  fi

  remaining=$(( (deadline - $(date +%s)) / 60 ))
  echo "  not indexed yet, ${remaining}m left before giving up"
  sleep 60
done

WORK="$(mktemp -d)"
if [ "$KEEP" = "true" ]; then
  echo "working directory: $WORK (kept)"
else
  trap 'rm -rf "$WORK"' EXIT
fi
cd "$WORK"

# nuget.org only. No local feed, no fallback, so this cannot accidentally resolve a package the
# build produced instead of the one that was published.
cat > NuGet.Config <<'XML'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
XML

cat > consumer.csproj <<XML
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>Consumer</RootNamespace>
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

// Exercises a native call, a borrowed span, an owned native string, IDNA, the enumerator and the
// storable handle. A package that is only partly correct cannot get through all of these.
int failures = 0;

void Check(bool ok, string what)
{
    Console.WriteLine($"  {(ok ? "ok  " : "FAIL")}  {what}");
    if (!ok) { failures++; }
}

Console.WriteLine($"native version: {AdaLibrary.NativeVersion}");
Check(AdaLibrary.NativeVersion == AdaLibrary.PinnedVersion, "native version matches the pinned one");

if (!AdaUrl.TryParse("https://user:pass@Bücher.example:8443/a/../b?q=1#f"u8, out AdaUrl url))
{
    Console.Error.WriteLine("FAIL: parse returned false");
    return 1;
}

using (url)
{
    Check(Encoding.UTF8.GetString(url.Hostname) == "xn--bcher-kva.example", "IDNA host normalised");
    Check(Encoding.UTF8.GetString(url.Pathname) == "/b", "dot segments removed");
    Check(Encoding.UTF8.GetString(url.Port) == "8443", "port read");
    Check(url.HasCredentials, "credentials detected");

    Span<byte> origin = stackalloc byte[64];
    Check(url.TryGetOrigin(origin, out int written) && written > 0, "origin copied out");
}

using (var parameters = AdaSearchParams.Parse("a=1&b=2&c=3"u8))
{
    int count = 0;
    foreach (AdaSearchParams.Entry entry in parameters) { count++; }
    Check(count == 3, "search params enumerated");
}

Check(AdaIdna.ToAscii("Bücher.example") == "xn--bcher-kva.example", "IDNA to ascii");
Check(AdaIdna.ToUnicode("xn--bcher-kva.example") == "bücher.example", "IDNA to unicode");

using (AdaUrlHandle handle = AdaUrlHandle.Parse("https://example.com/x"u8))
{
    Check(Encoding.UTF8.GetString(handle.GetHostname()) == "example.com", "storable handle works");
}

// The standard this library exists to implement. If a published package got these wrong, nothing
// else about it matters.
Check(AdaUrl.CanParse("https://example.com/"u8), "valid URL accepted");
Check(!AdaUrl.CanParse("http://f:b/c"u8), "malformed URL rejected");

Console.WriteLine(failures == 0 ? "PASS" : $"{failures} check(s) failed");
return failures == 0 ? 0 : 1;
CS

echo "restoring from nuget.org"
dotnet restore --verbosity quiet

echo "running"
dotnet run -c Release --verbosity quiet

echo
echo "published package verified: Ada.Url $VERSION works when installed from nuget.org"
