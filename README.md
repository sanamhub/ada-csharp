# Ada.Url

[![NuGet](https://img.shields.io/nuget/v/Ada.Url?logo=nuget)](https://www.nuget.org/packages/Ada.Url)
[![Downloads](https://img.shields.io/nuget/dt/Ada.Url?logo=nuget)](https://www.nuget.org/packages/Ada.Url)
[![CI](https://github.com/sanamhub/ada-csharp/actions/workflows/ci.yml/badge.svg)](https://github.com/sanamhub/ada-csharp/actions/workflows/ci.yml)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

WHATWG compliant URL parsing for .NET, built on [Ada](https://github.com/ada-url/ada).

Ada is the C++ URL parser behind Node.js, and is also used by Cloudflare Workers, Telegram,
Datadog, Kong and Redpanda. This package brings the same parser, and the same results, to .NET.

About 3 to 4x faster to validate a URL, and allocation free on the UTF-8 path where
`System.Uri` costs around 370 bytes each. A full parse is about 1.9x faster on Linux x64 and
level on Windows x64, for a reason worth knowing before you adopt it. See performance.

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

All 186 are listed in
[`docs/system-uri-differences.md`](docs/system-uri-differences.md), generated from the corpus by
a test rather than written by hand, so it cannot drift from what the parsers actually do.

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

| Call | Ada.Url | `System.Uri` | Speed | Allocated |
| --- | ---: | ---: | --- | --- |
| `AdaUrl.CanParse(utf8)` | **47 ns** | 185 ns | 3.9x faster | **0 B** against 288 B |
| `AdaUrl.TryParse(utf8, out url)` then 3 spans | **92 ns** | 185 ns | 2.0x faster | **0 B** against 288 B |
| `AdaUrl.TryParse(utf8, out url)` then all 10 | **101 ns** | 185 ns | 1.8x faster | **0 B** against 288 B |
| `AdaUrl.TryParse(utf8, out url)` then `GetString` | **122 ns** | 185 ns | 1.5x faster | 72 B against 288 B |

The first three rows allocate **nothing at all**. Not less, none.

These are single URL microbenchmarks with a hot cache. Under sustained load the parse
advantage narrows to about 1.2x, while the validation advantage and the allocation
difference hold. See sustained throughput below.

### A hard URL

Credentials, a non default port, an internationalised host, dot segments and a heavy percent
encoded query:

| Call | Ada.Url | `System.Uri` | Speed | Allocated |
| --- | ---: | ---: | --- | --- |
| `AdaUrl.TryNormalize(utf8, buffer, out n)` | **1,134 ns** | 1,684 ns | 1.5x faster | **0 B** against 2,160 B |
| `AdaUrl.TryParse(utf8, out url)` then 4 spans | **1,139 ns** | 1,684 ns | 1.5x faster | **0 B** against 2,160 B |
| `AdaUrl.TryParse(utf8, out url)` then `GetString` | **1,374 ns** | 1,684 ns | 1.2x faster | 392 B against 2,160 B |

Both parsers slow down here, because IDNA and percent decoding are genuinely expensive. The gap
that widens is allocation: **2,160 bytes against nothing**.

### Sustained throughput

The tables above time one URL at a time with a hot cache, which is what a microbenchmark measures.
This is ten million parses over a corpus of a million distinct URLs, on one thread of a 16 core
x64 desktop, using the published package.

| Work | Rate | Per URL | Allocated |
| --- | ---: | ---: | --- |
| `AdaUrl.CanParse(utf8)` | **6.1 M/s** | 165 ns | **0 B**, zero gen0 collections |
| `AdaUrl.TryParse` then `Hostname` | **1.95 M/s** | 513 ns | **0 B**, zero gen0 collections |
| `AdaUrl.TryParse` then 5 components | 1.9 M/s | 522 ns | **0 B**, zero gen0 collections |
| `Uri.IsWellFormedUriString` | 1.55 M/s | 644 ns | 309 B, 32 gen0 per million |
| `Uri.TryCreate` then `.Host` | 1.61 M/s | 619 ns | 374 B, 39 gen0 per million |

So **3.9x on validation, 1.2x on a full parse**, and no allocation against 374 bytes per URL.

That parse figure is a Windows number, and Windows is the slowest platform for this. Measured on
CI against `System.Uri` on the same machine:

| Platform | Validate | Parse, span in and span out |
| --- | ---: | ---: |
| Linux x64 | **3.7x faster** | **1.9x faster** |
| Linux arm64 | **4.2x faster** | **1.9x faster** |
| macOS arm64 | **4.2x faster** | **1.4x faster** |
| Windows x64 | **3.3x faster** | level |

Windows is slower because the native library is built without whole program optimisation there.
Ada has no `__declspec(dllexport)`, so the build relies on CMake's `WINDOWS_EXPORT_ALL_SYMBOLS`,
which runs `cmake -E __create_def` across the compiled objects to generate the export list. With
`/GL` those objects hold IL rather than COFF symbols and that step crashes, so `/GL` and `/LTCG`
are off. Linux and macOS build with `-O3 -flto=thin`. This is a real limitation, recorded in
ADR-0003, and it is fixable by writing the export list explicitly instead of generating it.

Validation and allocation are unaffected. Both are wins on every platform.

Throughput held between 1.8 and 2.0 M/s whether the working set was 6 KiB or 60 MiB, so this is
not a cache effect. Across 16 threads it reaches 8.7 to 9.4 M/s. Two threads scale at 1.99x, so
nothing serialises at the interop boundary; beyond that it is memory bandwidth bound.

### Why a parse costs more than a validation

| | Per URL |
| --- | ---: |
| `CanParse`, nothing kept | 165 ns |
| `TryParse` then `Dispose` | 505 ns |
| difference | **340 ns** |

That gap is neither parsing nor P/Invoke, which costs a couple of nanoseconds a call. `ada_parse`
heap allocates a URL object and `ada_free` releases it, and that pair is about two thirds of what
a parse costs. `ada_c.h` has no way to parse into caller supplied storage, so it is an upstream
limit rather than something this package can route around.

The practical consequence: if you only need to know whether a string is a URL, call `CanParse`.
It is three times faster than parsing and discarding the result. Benchmark `W4` measures this
gap on every run so a future improvement, or regression, shows up as a number.

### Which number matters

Speed is the headline; allocation is usually the one that changes a capacity plan. A service
parsing 50,000 URLs a second allocates about 100 MB a second through `System.Uri` and nothing at
all through the span path, and that difference is GC pauses rather than nanoseconds.

To get zero allocation you have to pass UTF-8 and read spans. Hand it a `string` and ask for a
`string` back and you still win, by less. Both paths are measured above rather than one being
quoted and the other implied.

### Caveats

The per call tables were measured on a GitHub hosted `ubuntu-24.04` runner, x64. The sustained
throughput figures were measured on a 16 core x64 desktop, Windows, workstation GC, against the
published package restored from nuget.org.

Ratios are trustworthy, since both parsers ran in the same process on the same machine.
**Absolute nanoseconds and rates are indicative only**: a shared CI runner has noisy neighbours
and no frequency guarantee, and your hardware is not this hardware.

The two parsers do not implement the same specification, so speed is only half of the comparison.
`System.Uri` follows RFC 3986 and 3987; this follows WHATWG. They disagree on real inputs, which
is what [`docs/system-uri-differences.md`](docs/system-uri-differences.md) is for.

Results for Linux arm64, Windows x64 and macOS arm64, plus the thousand URL batch workload and
the UTF-16 transcode cost by input length, are in
[`docs/benchmarks/0.1.0-beta.1/`](docs/benchmarks/0.1.0-beta.1/). Reproduce any of it with
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

Tested against the [web-platform-tests](https://github.com/web-platform-tests/wpt) URL corpus,
the same suite browsers are held to, pinned at a known commit.

| | Cases | Result |
| --- | ---: | --- |
| URLs that must parse | 607 | all pass |
| URLs that must be **rejected** | 267 | all rejected |
| Setter behaviour | 278 | all pass |
| **Total** | **1,152** | **all pass** |

Verified on Linux x64, Linux arm64, macOS arm64 and Windows x64, every commit.

The rejection row is the one worth noticing. Accepting a malformed URL is the failure mode that
turns into a security bug, and it is the half of a specification that is easy to skip.

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
| [`docs/ADA_PLAN.md`](docs/ADA_PLAN.md) | Framework targeting, native build, interop architecture, test strategy, benchmarks, CI |
| [`docs/adr/`](docs/adr) | Architecture decision records |
| [`docs/system-uri-differences.md`](docs/system-uri-differences.md) | Every disagreement with `System.Uri` |
| [`docs/runbooks/release.md`](docs/runbooks/release.md) | Release and rollback |
| [`CHANGELOG.md`](CHANGELOG.md) | Release history |

## License

MIT, see [`LICENSE`](LICENSE). Ada is dual licensed Apache-2.0 or MIT and is redistributed here
under the MIT option. See [`THIRD-PARTY-NOTICES.txt`](THIRD-PARTY-NOTICES.txt).

Unofficial, and not affiliated with the Ada project.
