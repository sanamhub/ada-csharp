# Benchmark results, 0.1.0-beta.1

Run on GitHub hosted runners across four platforms, Ada v4.0.0.

## Read this first

**Ratios compare against `System.Uri` in the same process on the same machine**, so they are
fair on every platform and comparable between platforms. Lower is faster. A ratio of `0.50x`
means half the time, so twice as fast.

**The two parsers do not implement the same specification.** Ada follows WHATWG, `System.Uri`
follows RFC 3986 and 3987 plus a decade of .NET specific behaviour. They disagree on real
inputs, so speed is only half the comparison. See
[`docs/system-uri-differences.md`](../../system-uri-differences.md).

**Windows is the slow platform, and the reason is known.** Parsing on Windows is roughly level
with `System.Uri` where Linux x64 is about twice as fast. Linux and macOS build the native with
`-O3 -flto=thin` and interprocedural optimisation on. Windows builds with `/O2` and no `/GL` or
`/LTCG`, because Ada has no `__declspec(dllexport)` and relies on CMake's
`WINDOWS_EXPORT_ALL_SYMBOLS`, which runs `cmake -E __create_def` over the compiled objects.
With `/GL` those objects hold IL rather than COFF symbols and that step crashes. See ADR-0003.
Validation is still three to four times faster on Windows, and allocation is zero everywhere.

**Absolute nanoseconds are indicative only.** Shared runners have noisy neighbours and no
frequency guarantee. Per platform detail is linked at the end.

## What the numbers say

**Zero allocation holds on every platform.** Every span in, span out row reports 0 B. That is
also asserted deterministically in the test suite, because a shared runner cannot gate anything.

**The `CanParse` ratio in W1 below is not a like for like comparison.** Its baseline is
`new Uri()` followed by reading three components, so it measures validation against parsing and
reading. Against the cheapest equivalent, `Uri.TryCreate` with the result discarded, `CanParse`
is about 1.3x faster on a plain URL and slightly slower on the W3 corpus, which is heavy in
internationalised hosts. The benchmark now carries a `W0 validate` category with the correct
baseline; these results predate it.

**A full parse is 1.9x on Linux x64, 1.9x on Linux arm64, 1.4x on macOS arm64, and about level
on Windows x64.** Those rows are like for like: both sides parse and read three components.

**W4 shows where parse time goes.** Parsing and disposing costs 2.2x to 3.1x what validating
costs, and the gap is flat from a 100 URL working set to 200,000, so it is not cache behaviour.
`ada_parse` heap allocates a URL object that `ada_free` releases, and `ada_c.h` offers no way to
parse into caller supplied storage. If you only need to know whether a string is a URL, call
`CanParse`.

## W1

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Ada_Basic_CanParse` | 0.27x | **0 B** | 0.24x | **0 B** | 0.30x | **0 B** | 0.24x | **0 B** |
| `Ada_Basic_T1_SpanIn_SpanOut` | 0.52x | **0 B** | 0.52x | **0 B** | 1.01x | **0 B** | 0.70x | **0 B** |
| `Ada_Basic_T1_ReadEveryComponent` | 0.59x | **0 B** | 0.60x | **0 B** | 1.10x | **0 B** | 0.77x | **0 B** |
| `Ada_Basic_T2_SpanIn_StringOut` | 0.64x | 72 B | 0.62x | 72 B | 1.17x | 72 B | 0.82x | 72 B |
| `Ada_Basic_T3_StringIn_StringOut` | 0.70x | 72 B | 0.74x | 72 B | 1.27x | 72 B | 1.07x | 72 B |
| `SystemUri_Basic` | baseline | 288 B | baseline | 288 B | baseline | 288 B | baseline | 288 B |

## W2

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Ada_Complex_T1_Normalize` | 0.67x | **0 B** | 0.63x | **0 B** | 1.13x | **0 B** | 0.88x | **0 B** |
| `Ada_Complex_T1_SpanIn_SpanOut` | 0.67x | **0 B** | 0.64x | **0 B** | 1.14x | **0 B** | 0.87x | **0 B** |
| `Ada_Complex_T3_StringIn_StringOut` | 0.79x | 392 B | 0.73x | 392 B | 1.25x | 392 B | 0.93x | 392 B |
| `SystemUri_Complex` | baseline | 2160 B | baseline | 2160 B | baseline | 2160 B | baseline | 2160 B |

## W3 full read

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Ada_ReadAll` | 0.67x | **0 B** | 0.66x | **0 B** | 1.20x | **0 B** | 0.83x | **0 B** |
| `SystemUri_ReadAll` | baseline | 371 B | baseline | 371 B | baseline | 371 B | 1.02x | 371 B |

## W3 hostname

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Ada_ExtractHostname` | 0.80x | **0 B** | 0.79x | **0 B** | 1.39x | **0 B** | 0.92x | **0 B** |
| `SystemUri_ExtractHostname` | baseline | 218 B | baseline | 218 B | baseline | 218 B | baseline | 218 B |

## W3 validate

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `SystemUri_Validate` | baseline | 91 B | baseline | 91 B | baseline | 91 B | baseline | 91 B |
| `Ada_Validate` | 1.18x | **0 B** | 1.15x | **0 B** | 1.99x | **0 B** | 1.20x | **0 B** |

## W4

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `CanParse [WorkingSet=100]` | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** |
| `ParseAndReadHostname [WorkingSet=100]` | 2.18x | **0 B** | 2.27x | **0 B** | 2.98x | **0 B** | 3.06x | **0 B** |
| `ParseAndDispose [WorkingSet=100]` | 2.22x | **0 B** | 2.24x | **0 B** | 2.92x | **0 B** | 3.01x | **0 B** |
| `ParseAndReadFive [WorkingSet=100]` | 2.33x | **0 B** | 2.37x | **0 B** | 3.10x | **0 B** | 3.13x | **0 B** |
| `CanParse [WorkingSet=10000]` | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** |
| `ParseAndReadHostname [WorkingSet=10000]` | 2.32x | **0 B** | 2.28x | **0 B** | 3.04x | **0 B** | 3.08x | **0 B** |
| `ParseAndDispose [WorkingSet=10000]` | 2.35x | **0 B** | 2.26x | **0 B** | 3.08x | **0 B** | 3.10x | **0 B** |
| `ParseAndReadFive [WorkingSet=10000]` | 2.43x | **0 B** | 2.37x | **0 B** | 3.15x | **0 B** | 3.18x | **0 B** |
| `CanParse [WorkingSet=200000]` | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** |
| `ParseAndDispose [WorkingSet=200000]` | 2.32x | **0 B** | 2.27x | **0 B** | 3.01x | **0 B** | 3.11x | **0 B** |
| `ParseAndReadHostname [WorkingSet=200000]` | 2.35x | **0 B** | 2.29x | **0 B** | 3.15x | **0 B** | 3.13x | **0 B** |
| `ParseAndReadFive [WorkingSet=200000]` | 2.44x | **0 B** | 2.38x | **0 B** | 3.17x | **0 B** | 3.19x | **0 B** |

## Detail

Full BenchmarkDotNet output, every column, one file per platform.

- [Linux x64](linux-x64.md)
- [Linux arm64](linux-arm64.md)
- [Windows x64](win-x64.md)
- [macOS arm64](osx-arm64.md)

## Reading these

A ratio of `0.50x` means half the time of `System.Uri`, so twice as fast.
`baseline` marks the row each group is measured against.

Allocation is the column that usually matters more. A parser that allocates
nothing does not add GC pressure no matter how many URLs go through it.
