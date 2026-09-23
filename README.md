# Ada.Url

[![NuGet](https://img.shields.io/nuget/v/Ada.Url?logo=nuget)](https://www.nuget.org/packages/Ada.Url)
[![Downloads](https://img.shields.io/nuget/dt/Ada.Url?logo=nuget)](https://www.nuget.org/packages/Ada.Url)
[![CI](https://github.com/sanamhub/ada-csharp/actions/workflows/ci.yml/badge.svg)](https://github.com/sanamhub/ada-csharp/actions/workflows/ci.yml)
[![License](https://img.shields.io/badge/license-MIT-blue)](https://github.com/sanamhub/ada-csharp/blob/main/LICENSE)

WHATWG compliant URL parsing for .NET, built on [Ada](https://github.com/ada-url/ada), the C++ URL
parser behind Node.js. It parses URLs the way browsers do, allocates nothing on the UTF-8 path, and
is up to 1.9x faster than `System.Uri`.

```csharp
using var url = AdaUrl.Parse("https://example.org/path/../file.txt"u8);
Encoding.UTF8.GetString(url.Href);      // https://example.org/file.txt
Encoding.UTF8.GetString(url.Hostname);  // example.org
```

> **Status:** 0.x. The conformance suite passes in full on every supported platform and the public
> API has not changed since the first beta, but under SemVer a minor version may still break it.

## Install

```bash
dotnet add package Ada.Url
```

Requires .NET 10. Native binaries for `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`,
`linux-musl-x64`, `linux-musl-arm64`, `osx-x64` and `osx-arm64` ship inside the package.

## Usage

**One shot checks.** Parse, answer and free inside the call. Nothing allocates.

```csharp
AdaUrl.CanParse("https://example.com/"u8);                        // true

Span<byte> buffer = stackalloc byte[256];
AdaUrl.TryNormalize(input, buffer, out int written);
AdaUrl.TryGetHostname(input, buffer, out written);                // for an allow list
```

**Several properties from one URL.** `AdaUrl` is a `ref struct`, so it stays on the stack and
cannot outlive the block.

```csharp
using var url = AdaUrl.Parse("https://user:pass@example.com:8443/a/b?q=1#frag"u8);

url.Hostname;        // example.com
url.Port;            // 8443
url.Search;          // ?q=1
url.HasCredentials;  // true
```

**Query strings**, enumerated without allocating:

```csharp
using var parameters = AdaSearchParams.Parse("key1=value1&key2=value2"u8);
foreach (AdaSearchParams.Entry entry in parameters)
{
    // entry.Key and entry.Value are borrowed spans
}
```

**Internationalised domains:**

```csharp
AdaIdna.ToAscii("Bücher.example");            // xn--bcher-kva.example
AdaIdna.ToUnicode("xn--bcher-kva.example");   // bücher.example
```

`ReadOnlySpan<byte>` (UTF-8) is the primary API. `string` overloads exist and pay a transcode.

## Why not `System.Uri`

`System.Uri` implements RFC 3986 and 3987 plus .NET specific behaviour. It is not WHATWG
compliant, so it disagrees with browsers and with the Node, Go and Python parsers. Across 538
absolute URLs from the WHATWG test corpus:

| | Count |
| --- | ---: |
| Same result | 352 |
| Accepted by Ada, rejected by `System.Uri` | 64 |
| **Rejected by Ada, accepted by `System.Uri`** | **32** |
| Both parsed, different serialisation | 90 |

The bold row matters for security. Each of those 32 is an input that browsers, Node, Go and Python
all refuse and `System.Uri` accepts. Code that validates a URL with one parser and fetches it with
another has a gap exactly there. All 186 differences are listed in
[`docs/system-uri-differences.md`](https://github.com/sanamhub/ada-csharp/blob/main/docs/system-uri-differences.md),
generated from the corpus by a test.

## Performance

| Platform | Parse and read three properties | Allocated |
| --- | ---: | --- |
| Linux x64 | **1.9x faster** | **0 B** against 288 B |
| Linux arm64 | **1.9x faster** | **0 B** against 288 B |
| macOS arm64 | **1.4x faster** | **0 B** against 288 B |
| Windows x64 | about level | **0 B** against 288 B |

Zero allocation holds on every platform. Speed depends on it: Windows is level because its heap
makes Ada's two internal allocations per parse about 4x more expensive than on Linux. Measured on
0.1.0. Details, per-call numbers and a pattern for high volume loops on Windows are in
[`docs/performance.md`](https://github.com/sanamhub/ada-csharp/blob/main/docs/performance.md).

## Limits

- **Zero allocation means span in, span out.** Any `string` result allocates.
- **A borrowed span is invalidated by any setter.** Documented and tested, not enforced. Copy what
  you need before mutating. See
  [ADR-0004](https://github.com/sanamhub/ada-csharp/blob/main/docs/adr/0004-unmanaged-lifetime-model.md).
- **`net10.0` only.** No .NET Framework, Mono or Unity. See
  [ADR-0001](https://github.com/sanamhub/ada-csharp/blob/main/docs/adr/0001-single-target-net10-and-utf8-first-api.md).
- **Handles are not thread safe.** Concurrent reads are fine; a concurrent setter makes every
  outstanding span a use after free.

## Security

A URL parser usually decides whether a URL is safe to fetch.

1. Compare the parsed `Hostname`, never a prefix of the raw input. Prefix checks are the classic
   SSRF bypass.
2. Compare against the post IDNA ASCII form. Confusable Unicode domains normalise to different
   ASCII.

The library never logs a URL. Neither should you: URLs routinely carry credentials.

Report vulnerabilities as described in
[`SECURITY.md`](https://github.com/sanamhub/ada-csharp/blob/main/SECURITY.md).

## Conformance

Tested on every commit against the [web-platform-tests](https://github.com/web-platform-tests/wpt)
URL corpus, the suite browsers are held to, on Linux x64 and arm64, macOS arm64, and Windows x64
and arm64.

| | Cases | Result |
| --- | ---: | --- |
| URLs that must parse | 607 | all pass |
| URLs that must be **rejected** | 267 | all rejected |
| Setter behaviour | 278 | all pass |
| **Total** | **1,152** | **all pass** |

## Building from source

```bash
dotnet build -c Release
dotnet test  -c Release
```

The tests need the native library. CI builds it; locally, build it with the scripts in `native/`,
which need a C++ toolchain and CMake.

## Documentation

| Link | Contents |
| --- | --- |
| [`docs/performance.md`](https://github.com/sanamhub/ada-csharp/blob/main/docs/performance.md) | Benchmarks and what they mean |
| [`docs/system-uri-differences.md`](https://github.com/sanamhub/ada-csharp/blob/main/docs/system-uri-differences.md) | Every disagreement with `System.Uri` |
| [`docs/adr/`](https://github.com/sanamhub/ada-csharp/tree/main/docs/adr) | Architecture decision records |
| [`docs/ADA_PLAN.md`](https://github.com/sanamhub/ada-csharp/blob/main/docs/ADA_PLAN.md) | Original design plan |
| [`docs/runbooks/release.md`](https://github.com/sanamhub/ada-csharp/blob/main/docs/runbooks/release.md) | Releasing and rolling back |
| [`CHANGELOG.md`](https://github.com/sanamhub/ada-csharp/blob/main/CHANGELOG.md) | Release history |

## License

MIT, see [`LICENSE`](https://github.com/sanamhub/ada-csharp/blob/main/LICENSE). Ada is dual
licensed Apache-2.0 or MIT and is redistributed under the MIT option. See
[`THIRD-PARTY-NOTICES.txt`](https://github.com/sanamhub/ada-csharp/blob/main/THIRD-PARTY-NOTICES.txt).

Unofficial, not affiliated with the Ada project.
