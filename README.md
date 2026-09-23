# Ada.Url

[![NuGet](https://img.shields.io/nuget/v/Ada.Url?logo=nuget)](https://www.nuget.org/packages/Ada.Url)
[![Downloads](https://img.shields.io/nuget/dt/Ada.Url?logo=nuget)](https://www.nuget.org/packages/Ada.Url)
[![CI](https://github.com/sanamhub/ada-csharp/actions/workflows/ci.yml/badge.svg)](https://github.com/sanamhub/ada-csharp/actions/workflows/ci.yml)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

WHATWG compliant URL parsing for .NET, built on [Ada](https://github.com/ada-url/ada).

Ada is the C++ URL parser behind Node.js, and is also used by Cloudflare Workers, Telegram,
Datadog, Kong and Redpanda. This package brings the same parser, and the same results, to .NET.

Zero allocation on the UTF-8 path, where `System.Uri` costs about 370 bytes per URL. 1.9x faster
on Linux x64, 1.4x on macOS arm64, about level on Windows x64. Why Windows is the odd one out is
still open, and the [performance](#performance) section says what has been ruled out so far.

```csharp
using var url = AdaUrl.Parse("https://example.org/path/../file.txt"u8);
Encoding.UTF8.GetString(url.Href);      // https://example.org/file.txt
Encoding.UTF8.GetString(url.Hostname);  // example.org
```

> **0.1.0.** The conformance suite passes in full on five platforms and the package is verified
> by installing it into a clean project on each. The public API is considered done and has not
> moved since the first beta. This is still `0.x`, so under SemVer a minor bump may break it.

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

Native binaries for `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `linux-musl-x64`,
`linux-musl-arm64`, `osx-x64` and `osx-arm64` ship inside the package. Nothing to install
separately.

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

Two results, and they do not behave the same way. Allocation drops to zero on the span path on
every platform. Speed depends on the platform, and on Windows there is none.

| Platform | Parse and read three properties | Allocated |
| --- | ---: | --- |
| Linux x64 | **1.9x faster** | **0 B** against 288 B |
| Linux arm64 | **1.9x faster** | **0 B** against 288 B |
| macOS arm64 | **1.4x faster** | **0 B** against 288 B |
| Windows x64 | about level | **0 B** against 288 B |

Why Windows is the odd one out is answered: it is the allocator, and whole program optimisation
is not the answer. This README used to say it was.

It used to be off, because Ada has no `__declspec(dllexport)`, so the export list came from
`cmake -E __create_def`, which crashes on the IL objects `/GL` produces. Generating the export
list from Ada's own `ada_c.h` takes that step out of the build, which made `/GL` and `/LTCG`
possible, and 0.1.0 shipped with them. The export list stays: it makes the DLL 43% smaller.
From 0.1.1 `/GL` is off again. With it on, MSVC produces different bytes on different runner
images from identical sources, so the committed checksum cannot tell a new image from a swapped
binary. That costs 4 to 13 percent on `CanParse` and 3 to 5 percent on a hard URL. The numbers
are in [#45](https://github.com/sanamhub/ada-csharp/issues/45), the decision in ADR-0011.

Upstream's `src/ada.cpp` includes every other `.cpp`, so the library is a single translation unit
and there was never much for whole program optimisation to inline across.

What costs the rest is the Windows heap. `ada_parse` allocates twice per URL, 96 and 32 bytes,
and replaying just those two allocations with no parsing in the loop costs 87 ns on Windows
against 22 ns on Linux. That is 76% of everything the Windows parse spends beyond validating,
reproduced in two runs. The measurement is in
[#20](https://github.com/sanamhub/ada-csharp/issues/20), the decision in ADR-0007, and the probe
that produced it is `native/bench/alloc-probe.cpp`.

No allocator is bundled to hide it. That would make this `ada.dll` differ from upstream's at the
one place the checksum manifest and the SBOM exist to say it does not, and it would override the
allocator of a process we do not own. There is a way to get some of it back, below.

### Numbers, Linux x64

A plain URL, `https://example.com/path`:

| Call | Ada.Url | `System.Uri` | Speedup | Allocated |
| --- | ---: | ---: | ---: | --- |
| `TryParse` then 3 spans | **90 ns** | 175 ns | **1.9x** | **0 B** against 288 B |
| `TryParse` then all 10 | **103 ns** | 175 ns | **1.7x** | **0 B** against 288 B |
| `TryParse` then `GetString` | 112 ns | 175 ns | 1.6x | 72 B against 288 B |
| `string` in, `string` out | 122 ns | 175 ns | 1.4x | 72 B against 288 B |

A hard URL, with credentials, a non default port, an internationalised host, dot segments and a
heavy percent encoded query:

| Call | Ada.Url | `System.Uri` | Speedup | Allocated |
| --- | ---: | ---: | ---: | --- |
| `TryNormalize(utf8, buffer, out n)` | **1,141 ns** | 1,705 ns | **1.5x** | **0 B** against 2,160 B |
| `TryParse` then 4 spans | **1,150 ns** | 1,705 ns | **1.5x** | **0 B** against 2,160 B |
| `string` in, `string` out | 1,341 ns | 1,705 ns | 1.3x | 392 B against 2,160 B |

Both parsers slow down on the hard URL, because IDNA and percent decoding are genuinely
expensive. The gap that widens is allocation: 2,160 bytes against nothing.

### Validating without parsing

`CanParse` answers "is this a valid URL" without building anything. Against the cheapest
equivalent, `Uri.TryCreate` with the result discarded, it is **1.3x faster on a plain URL** and
**slightly slower on a corpus heavy in internationalised hosts**, where full UTS-46 costs more
than what `System.Uri` does.

Use it because it allocates nothing and is three times cheaper than parsing and throwing the
result away, not because it beats `System.Uri` by a wide margin.

### Parsing many URLs on Windows

Two thirds of a parse is `ada_parse` allocating a URL object and `ada_free` releasing it, and on
Windows that allocation costs about 4x what it costs on Linux. A loop does not have to pay it
per URL. `TrySetHref` re-parses into a handle you already have:

```csharp
// One result object for the whole loop instead of one per URL.
using var url = AdaUrl.Parse(urls[0]);

foreach (byte[] next in urls)
{
    if (!url.TrySetHref(next)) continue;
    Consume(url.Hostname);
}
```

| Plain URL, per call | Linux x64 | Windows x64 |
| --- | ---: | ---: |
| parse, then free | 69 ns | 162 ns |
| `TrySetHref` on a kept handle | **64 ns** | **118 ns** |

27% on Windows, 8% on Linux. Unlike every other table on this page those two rows are native
measurements from `native/bench/alloc-probe.cpp`, not the managed benchmark, so do not line them
up against the nanoseconds above. Benchmark `W4` measures the same thing through the binding:
on Windows x64 a reused handle costs 2.48x to 2.78x a validation where a fresh parse costs
2.88x to 3.11x, in [`docs/benchmarks/0.1.0/`](docs/benchmarks/0.1.0/). It is worth nothing on a hard URL, 1,741 ns against 1,771, because
`set_href` still allocates four times there and the IDNA work dominates either way. So this is
for high volume loops over ordinary URLs, not a blanket recommendation.

The handle is reused, so anything you read out of it is invalidated by the next `TrySetHref`.
Copy what you need before the next iteration, the same rule that already applies to every setter.

### Sustained throughput

Ten million parses over a corpus of a million distinct URLs, one thread of a 16 core x64 desktop
on Windows, using the published package. Windows, so read this as the pessimistic case.

| Work | Rate | Per URL | Allocated |
| --- | ---: | ---: | --- |
| `AdaUrl.CanParse(utf8)` | **6.1 M/s** | 164 ns | **0 B**, zero gen0 collections |
| `AdaUrl.TryParse` then `Hostname` | **1.97 M/s** | 507 ns | **0 B**, zero gen0 collections |
| `Uri.TryCreate`, discarded | 2.69 M/s | 371 ns | 247 B, 26 gen0 per million |
| `Uri.TryCreate` then `.Host` | 1.56 M/s | 643 ns | 374 B, 39 gen0 per million |

Throughput held between 1.8 and 2.0 M/s whether the working set was 6 KiB or 60 MiB, so this is
not a cache effect. Across 16 threads it reaches 8.7 to 9.4 M/s, and two threads scale at 1.99x,
so nothing serialises at the interop boundary. Beyond that it is memory bandwidth bound.

A parse costs 338 ns more than a validation, 502 ns against 164 ns. That gap is neither parsing
nor P/Invoke, which costs a couple of nanoseconds a call. `ada_parse` heap allocates a URL object
and `ada_free` releases it, and that pair is about two thirds of what a parse costs. `ada_c.h`
has no way to parse into caller supplied storage, so half of it is an upstream limit. The other
half is the result object, and keeping one handle avoids it: see above. Benchmark `W4` measures
the gap on every run.

### Reading these numbers

Allocation is the result that holds everywhere. A service parsing 50,000 URLs a second allocates
about 100 MB a second through `System.Uri` and nothing at all through the span path, and that
difference is GC pauses rather than nanoseconds. Getting it needs UTF-8 in and spans out. Hand it
a `string` and ask for a `string` back and you still win, by less.

Speedups are trustworthy, since both parsers ran in the same process on the same machine.
**Absolute nanoseconds and rates are indicative only**: a shared CI runner has noisy neighbours
and no frequency guarantee, and your hardware is not this hardware.

The two parsers do not implement the same specification, so speed is only half of the comparison.
If speed is what you are here for and you deploy on Windows, benchmark your own traffic before
switching. The honest summary for Windows x64 in 0.1.0: about 12 percent ahead of `System.Uri`
on a plain URL through the span path, level on a hard one, no garbage either way, different
specification. Generating the export list and compiling with `/GL` is worth about 10 percent of
that on a hard URL. The plain URL figure improved against 0.1.0-beta.1 as well, but Linux
improved more on the same row with no build change at all, so read that one as the runner.

Full results for all five benchmarked platforms, the thousand URL batch workload and the UTF-16
transcode cost by input length are in
[`docs/benchmarks/0.1.0/`](docs/benchmarks/0.1.0/). Those pages print raw
BenchmarkDotNet output, where the `Ratio` column is the inverse of the speedups here and lower is
faster: `0.52x` there is `1.9x` on this page. Reproduce any of it with
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

Verified on Linux x64, Linux arm64, macOS arm64, Windows x64 and Windows arm64, every commit.

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
