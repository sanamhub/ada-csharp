# Performance

Measured against `System.Uri` in the same process on the same machine, with BenchmarkDotNet.
Speedups are reliable. Absolute nanoseconds are indicative only: a shared CI runner has noisy
neighbours and no frequency guarantee, and your hardware is not this hardware.

Raw results for every platform are in [`benchmarks/0.1.0/`](benchmarks/0.1.0/). Reproduce with:

```bash
dotnet run -c Release --project benchmarks/Ada.Url.Benchmarks
```

Those pages print BenchmarkDotNet's `Ratio` column, which is the inverse of the speedups here:
`0.52x` there is `1.9x` on this page.

## Summary

| Platform | Parse and read three properties | Allocated |
| --- | ---: | --- |
| Linux x64 | **1.9x faster** | **0 B** against 288 B |
| Linux arm64 | **1.9x faster** | **0 B** against 288 B |
| macOS arm64 | **1.4x faster** | **0 B** against 288 B |
| Windows x64 | about level | **0 B** against 288 B |

Allocation is the result that holds everywhere. A service parsing 50,000 URLs a second allocates
about 100 MB a second through `System.Uri` and nothing through the span path, and that difference
shows up as GC pauses rather than nanoseconds. Getting it needs UTF-8 in and spans out. `string` in
and `string` out still wins, by less.

These numbers are from 0.1.0. From 0.1.1 Windows builds without `/GL`, which costs 4 to 13 percent
on `CanParse` and 3 to 5 percent on a hard URL. See [ADR-0011](adr/0011-ship-windows-natives-without-gl.md).

## Linux x64

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

Both parsers slow down on the hard URL, because IDNA and percent decoding are expensive. The gap
that widens is allocation.

## Validating without parsing

`CanParse` answers "is this a valid URL" without building anything. Against `Uri.TryCreate` with
the result discarded it is 1.3x faster on a plain URL and slightly slower on a corpus heavy in
internationalised hosts, where full UTS-46 costs more than what `System.Uri` does.

Use it because it allocates nothing and is three times cheaper than parsing and discarding the
result, not because it beats `System.Uri` by a wide margin.

## Why Windows is slower

The Windows heap. `ada_parse` allocates twice per URL, 96 and 32 bytes. Replaying only those two
allocations, with no parsing, costs 87 ns on Windows against 22 ns on Linux. That is 76% of what
the Windows parse spends beyond validating. The measurement is in
[#20](https://github.com/sanamhub/ada-csharp/issues/20), the probe is
`native/bench/alloc-probe.cpp`, and the decision is [ADR-0007](adr/0007-do-not-bundle-an-allocator-on-windows.md).

No allocator is bundled to hide it. That would make this `ada.dll` differ from upstream's in the
place the checksum manifest and SBOM exist to vouch for, and it would override the allocator of a
process this library does not own.

## Parsing many URLs on Windows

A loop does not have to pay the allocation per URL. `TrySetHref` re-parses into a handle you
already have:

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

27% on Windows, 8% on Linux. These two rows are native measurements from `alloc-probe.cpp`, not
the managed benchmark, so do not compare them with the other tables. It is worth nothing on a hard
URL, where `set_href` still allocates and IDNA dominates. Use it for high volume loops over
ordinary URLs.

The handle is reused, so anything read from it is invalidated by the next `TrySetHref`. Copy what
you need first, the same rule as for every setter.

## Sustained throughput

Ten million parses over a million distinct URLs, one thread of a 16 core x64 desktop on Windows,
using the published package. Windows, so read this as the pessimistic case.

| Work | Rate | Per URL | Allocated |
| --- | ---: | ---: | --- |
| `AdaUrl.CanParse(utf8)` | **6.1 M/s** | 164 ns | **0 B**, zero gen0 collections |
| `AdaUrl.TryParse` then `Hostname` | **1.97 M/s** | 507 ns | **0 B**, zero gen0 collections |
| `Uri.TryCreate`, discarded | 2.69 M/s | 371 ns | 247 B, 26 gen0 per million |
| `Uri.TryCreate` then `.Host` | 1.56 M/s | 643 ns | 374 B, 39 gen0 per million |

Throughput held between 1.8 and 2.0 M/s whether the working set was 6 KiB or 60 MiB, so this is
not a cache effect. Across 16 threads it reaches 8.7 to 9.4 M/s, and two threads scale at 1.99x,
so nothing serialises at the interop boundary.

## Before you switch for speed

The two parsers do not implement the same specification, so speed is only half the comparison. If
you deploy on Windows and speed is the reason, benchmark your own traffic first.
