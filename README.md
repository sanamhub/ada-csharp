# Ada.Url

WHATWG compliant URL parsing for .NET, built on [Ada](https://github.com/ada-url/ada).

Ada is the C++ URL parser behind Node.js, and is also used by Cloudflare Workers, Telegram,
Datadog, Kong and Redpanda. This package brings the same parser, and the same results, to .NET.

Roughly twice as fast as `System.Uri` on a plain URL, and allocation free on the UTF-8 path.

```csharp
using var url = AdaUrl.Parse("https://example.org/path/../file.txt"u8);
Encoding.UTF8.GetString(url.Href);      // https://example.org/file.txt
Encoding.UTF8.GetString(url.Hostname);  // example.org
```

> **0.1.0-beta.1.** The conformance suite passes in full on four platforms and the package is
> verified by installing it into a clean project. The public API has had no outside use yet, so
> it may still move before 1.0.

## Why not `System.Uri`

`System.Uri` implements RFC 3986 and 3987 plus a decade of .NET specific behaviour. It is not
WHATWG compliant, so it disagrees with browsers, and with the Node, Go and Python parsers, on a
long list of real inputs. It also allocates on almost every operation.

How far apart are they? Across 538 absolute URLs from the WHATWG test corpus, the two parsers
produce a different outcome on **186 of them**:

| | Count |
| --- | ---: |
| Same result | 352 |
| Accepted by Ada, rejected by `System.Uri` | 64 |
| **Rejected by Ada, accepted by `System.Uri`** | **32** |
| Both parsed, different serialisation | 90 |

The bolded row is the security one. Each of those 32 is an input `System.Uri` accepts that
browsers, Node, Go and Python all refuse. Code that validates a URL with one parser and then
fetches it with another has an exploitable gap exactly there.

Every case is listed in `docs/system-uri-differences.md`, generated from the corpus by a test
rather than written by hand.

## Install

```bash
dotnet add package Ada.Url
```

Native binaries for `win-x64`, `linux-x64`, `linux-arm64`, `linux-musl-x64`, `osx-x64` and
`osx-arm64` ship inside the package. Nothing to install separately.

## Using it

**One shot checks.** Most code only needs an answer, not an object. These parse, answer and free
inside the call, so nothing escapes and nothing allocates.

```csharp
AdaUrl.CanParse("https://example.com/"u8);                        // true

Span<byte> buffer = stackalloc byte[256];
AdaUrl.TryNormalize(input, buffer, out int written);
AdaUrl.TryGetHostname(input, buffer, out written);                // for an allow list
```

**Several properties from one URL.** `AdaUrl` is a `ref struct`, so the compiler keeps it on the
stack and its lifetime cannot outrun the block.

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

## Performance

### A plain URL

`https://example.com/path`

| What you are doing | Ada.Url | `System.Uri` | | Allocated |
| --- | ---: | ---: | --- | --- |
| Check it is a valid URL | **47 ns** | 185 ns | 3.9x faster | 0 B against 288 B |
| Parse, read host, path and query | **92 ns** | 185 ns | 2.0x faster | 0 B against 288 B |
| Parse, read all ten components | **101 ns** | 185 ns | 1.8x faster | 0 B against 288 B |
| Parse, return the URL as a `string` | **122 ns** | 185 ns | 1.5x faster | 72 B against 288 B |

The first three rows allocate **nothing at all**. Not less, none.

### A hard URL

Credentials, a non default port, an internationalised host, dot segments and a heavy percent
encoded query:

| What you are doing | Ada.Url | `System.Uri` | | Allocated |
| --- | ---: | ---: | --- | --- |
| Normalise into your own buffer | **1,134 ns** | 1,684 ns | 1.5x faster | 0 B against 2,160 B |
| Parse and read four components | **1,139 ns** | 1,684 ns | 1.5x faster | 0 B against 2,160 B |
| Parse and return a `string` | **1,374 ns** | 1,684 ns | 1.2x faster | 392 B against 2,160 B |

Both parsers slow down here, because IDNA and percent decoding are genuinely expensive. The gap
that widens is allocation: **2,160 bytes against nothing**.

### Which number matters

Speed is the headline; allocation is usually the one that changes a capacity plan. A service
parsing 50,000 URLs a second allocates about 100 MB a second through `System.Uri` and nothing at
all through the span path, and that difference is GC pauses rather than nanoseconds.

To get zero allocation you have to pass UTF-8 and read spans. Hand it a `string` and ask for a
`string` back and you still win, by less. Both paths are measured above rather than one being
quoted and the other implied.

### Caveats

Measured on a GitHub hosted `ubuntu-24.04` runner, x64. Ratios are trustworthy, since both
parsers ran in the same process on the same machine. **Absolute nanoseconds are indicative
only**, because a shared CI runner has noisy neighbours and no frequency guarantee.

Results for Linux arm64, Windows x64 and macOS arm64, plus the thousand URL batch workload and
the UTF-16 transcode cost by input length, are in
[`docs/benchmarks/`](docs/benchmarks/). Reproduce any of it with
`dotnet run -c Release --project benchmarks/Ada.Url.Benchmarks`.

## How it works

Ada's C API is byte oriented UTF-8, so this API is UTF-8 first. `ReadOnlySpan<byte>` is the
primary shape and `string` overloads carry a documented transcode cost.

Interop signatures use only blittable types (`byte*`, `nuint`, `nint`) and pin with `fixed`,
which skips string marshalling entirely. Lifetime comes in three sizes: handle free statics for
the common case, a stack bound `ref struct` for work touching several properties, and a
`SafeHandle` for a URL that has to live in a field.

## Limits

**Zero allocation means span in, span out.** Any `System.String` result allocates by
construction. Benchmarks are published in tiers and the zero byte figure only ever refers to the
span tier.

**A borrowed span is invalidated by any setter.** Documented and tested, not enforced. Enforcing
it would cost the property this library exists to provide. Copy anything you need to keep across
a mutation. See [ADR-0004](docs/adr/0004-unmanaged-lifetime-model.md).

**`net10.0` only.** No .NET Framework, Mono or Unity. See
[ADR-0001](docs/adr/0001-single-target-net10-and-utf8-first-api.md).

**Handles are not thread safe.** One per thread, or synchronise externally. Concurrent reads of
an instance nobody mutates are fine, but one concurrent setter makes every outstanding span a
use after free.

## Security

A URL parser is usually deciding whether a URL is safe to fetch. Two rules:

1. Compare the parsed `hostname`, never a prefix of the raw input. Prefix checks against raw
   input are the classic SSRF bypass.
2. Compare against post IDNA ASCII. Visually confusable Unicode domains normalise to different
   ASCII, so a Unicode level comparison can be fooled.

This library never logs a URL, and neither should you at any level you would ship. URLs
routinely carry credentials in `username` and `password`.

## Conformance

874 WHATWG parse cases and 278 setter cases from the
[web-platform-tests](https://github.com/web-platform-tests/wpt) corpus, pinned at a known commit,
passing on Linux x64, Linux arm64, macOS arm64 and Windows x64. That includes the 267 inputs the
standard says must be **rejected**.

## Build

```bash
dotnet build -c Release
dotnet test  -c Release
```

The tests need the native library. CI downloads it; locally, build it from `native/`, which needs
a C++ toolchain and CMake.

## Documentation

| File | Contents |
| --- | --- |
| [`ADA_WRAPPER_PLAN.md`](ADA_WRAPPER_PLAN.md) | Framework targeting, native build, interop architecture, test strategy, benchmarks, CI |
| [`docs/adr/`](docs/adr) | Architecture decision records |
| [`docs/system-uri-differences.md`](docs/system-uri-differences.md) | Every disagreement with `System.Uri` |
| [`docs/runbooks/release.md`](docs/runbooks/release.md) | Release and rollback |
| [`CHANGELOG.md`](CHANGELOG.md) | Release history |

## License

MIT, see [`LICENSE`](LICENSE). Ada is dual licensed Apache-2.0 or MIT and is redistributed here
under the MIT option. See [`THIRD-PARTY-NOTICES.txt`](THIRD-PARTY-NOTICES.txt).

Unofficial, and not affiliated with the Ada project.
