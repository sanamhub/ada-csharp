# Benchmark results, 0.1.0

Run on GitHub hosted runners on 2026-09-17, Ada v4.0.0, five platforms.

## Read this first

**These are the binaries that shipped.** Every native in this run was a cache hit, and the
sha256 of all five matches [`native/CHECKSUMS.txt`](../../../native/CHECKSUMS.txt) as released.
Nothing here was measured against a rebuild.

**Ratios compare against `System.Uri` in the same process on the same machine**, so they are
fair on every platform and comparable between platforms. Lower is faster. A ratio of `0.50x`
means half the time, so twice as fast.

**The two parsers do not implement the same specification.** Ada follows WHATWG, `System.Uri`
follows RFC 3986 and 3987 plus a decade of .NET specific behaviour. They disagree on real
inputs, so speed is only half the comparison. See
[`docs/system-uri-differences.md`](../../system-uri-differences.md).

**Absolute nanoseconds are indicative only.** Shared runners have noisy neighbours and no
frequency guarantee. Per platform detail is linked at the end.

**Three shipped RIDs are missing.** `linux-musl-x64`, `linux-musl-arm64` and `osx-x64` have no
hosted runner this harness can use. The musl natives are built from the same source with the
same flags as their glibc counterparts, so the glibc rows are the closest available guide, not
a measurement.

## What changed since 0.1.0-beta.1

**Windows x64 got faster, and the size of it is larger than ADR-0006 predicted.** 0.1.0-beta.1
shipped without `/GL` and `/LTCG`; 0.1.0 generates the export list from `ada_c.h` and turns both
back on. W2 moved from `1.13x`, `1.14x` and `1.25x` to `1.00x`, `1.03x` and `1.07x`, a 10 to 14
percent reduction. [ADR-0006](../../adr/0006-generated-export-list-and-whole-program-optimisation-on-windows.md)
measured 6 to 8 percent on that category from a single runner.

W1 moved too, from `1.01x` and `1.10x` to `0.88x` and `1.00x`. ADR-0006 found W1 to be noise, so
do not attribute that to the optimisation. The runs are ten days apart on different runner
hardware, and the gap between the two explanations is not resolvable from these numbers.

**`W0 validate` is now populated.** beta.1 warned that its `CanParse` row was measured against
`new Uri()` plus three component reads, which is validation against parsing. The correct
baseline is `Uri.TryCreate` with the result discarded, and against that `CanParse` runs `0.54x`
to `0.79x`.

**Windows arm64 is measured for the first time.** Cross compiled on `windows-2022`, benchmarked
on `windows-11-arm`. It sits between Windows x64 and the Linux platforms on the parse path and
behind both on transcode.

**`ReuseHandleAndReadHostname` is new in W4.** It answers what a kept handle saves over parsing
into a fresh one, which is the mitigation ADR-0007 documents for the Windows allocator gap. On
Windows x64 it costs `2.48x` to `2.78x` a validation where `ParseAndReadHostname` costs `2.88x`
to `3.11x`.

## Two rows have no baseline marker on Windows x64

`Ada_Basic_T1_ReadEveryComponent` and `Ada_Complex_T1_Normalize` both print `1.00x` on Windows
x64, and so does the `System.Uri` row they are measured against. That is a real result:
158.90 ns against 158.15 ns in W1. It is not a formatting fault, and neither row is the baseline.

## A caveat on the Windows figures

[Issue #45](https://github.com/sanamhub/ada-csharp/issues/45) records that the Windows natives
are a function of the runner image: the same source and the same compiler version produce one of
two different binaries depending on which image the build lands on. These numbers describe the
released bytes, because the cache served them. A rebuild on a different image may not be the same
binary, and would need measuring again.

## TranscodeBenchmarks

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | Windows arm64 ratio | Windows arm64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Utf8_NoTranscode [Length=16]` | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** |
| `Utf16_NonAscii [Length=16]` | 1.23x | **0 B** | 1.40x | **0 B** | 1.13x | **0 B** | 1.54x | **0 B** | 1.53x | **0 B** |
| `Utf16_Ascii [Length=16]` | 1.25x | **0 B** | 1.45x | **0 B** | 1.16x | **0 B** | 1.57x | **0 B** | 1.49x | **0 B** |
| `Utf16_CallerTranscodes [Length=16]` | 1.46x | **0 B** | 1.59x | **0 B** | 1.28x | **0 B** | 1.48x | **0 B** | 1.47x | **0 B** |
| `Utf8_NoTranscode [Length=64]` | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** |
| `Utf16_Ascii [Length=64]` | 1.28x | **0 B** | 1.42x | **0 B** | 1.30x | **0 B** | 1.49x | **0 B** | 1.48x | **0 B** |
| `Utf16_CallerTranscodes [Length=64]` | 1.52x | **0 B** | 1.63x | **0 B** | 1.44x | **0 B** | 1.48x | **0 B** | 1.39x | **0 B** |
| `Utf16_NonAscii [Length=64]` | 1.57x | **0 B** | 1.67x | **0 B** | 1.42x | **0 B** | 1.87x | **0 B** | 1.79x | **0 B** |
| `Utf8_NoTranscode [Length=170]` | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** |
| `Utf16_Ascii [Length=170]` | 1.24x | **0 B** | 1.50x | **0 B** | 1.30x | **0 B** | 1.63x | **0 B** | 1.76x | **0 B** |
| `Utf16_CallerTranscodes [Length=170]` | 1.64x | **0 B** | 1.74x | **0 B** | 1.56x | **0 B** | 1.67x | **0 B** | 1.62x | **0 B** |
| `Utf16_NonAscii [Length=170]` | 2.27x | **0 B** | 2.38x | **0 B** | 2.09x | **0 B** | 2.87x | **0 B** | 2.81x | **0 B** |
| `Utf8_NoTranscode [Length=256]` | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** |
| `Utf16_CallerTranscodes [Length=256]` | 1.58x | **0 B** | 1.83x | **0 B** | 1.50x | **0 B** | 1.75x | **0 B** | 1.73x | **0 B** |
| `Utf16_Ascii [Length=256]` | 1.62x | **0 B** | 2.03x | **0 B** | 1.56x | **0 B** | 2.03x | **0 B** | 2.02x | **0 B** |
| `Utf16_NonAscii [Length=256]` | 3.11x | **0 B** | 2.94x | **0 B** | 3.18x | **0 B** | 3.65x | **0 B** | 3.59x | **0 B** |
| `Utf8_NoTranscode [Length=1024]` | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** |
| `Utf16_CallerTranscodes [Length=1024]` | 1.81x | **0 B** | 2.59x | **0 B** | 1.74x | **0 B** | 2.85x | **0 B** | 3.11x | **0 B** |
| `Utf16_Ascii [Length=1024]` | 1.93x | **0 B** | 2.88x | **0 B** | 1.84x | **0 B** | 3.09x | **0 B** | 3.19x | **0 B** |
| `Utf16_NonAscii [Length=1024]` | 9.15x | **0 B** | 8.84x | **0 B** | 10.32x | **0 B** | 11.36x | **0 B** | 11.29x | **0 B** |
| `Utf8_NoTranscode [Length=4096]` | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** |
| `Utf16_CallerTranscodes [Length=4096]` | 3.77x | **0 B** | 5.68x | **0 B** | 3.18x | **0 B** | 7.10x | **0 B** | 7.08x | **0 B** |
| `Utf16_Ascii [Length=4096]` | 3.84x | **0 B** | 5.92x | **0 B** | 3.03x | **0 B** | 7.37x | **0 B** | 7.26x | **0 B** |
| `Utf16_NonAscii [Length=4096]` | 33.32x | **0 B** | 30.41x | **0 B** | 36.61x | **0 B** | 40.77x | **0 B** | 35.63x | **0 B** |

## W0 validate

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | Windows arm64 ratio | Windows arm64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Ada_Basic_CanParse` | 0.57x | **0 B** | 0.69x | **0 B** | 0.79x | **0 B** | 0.54x | **0 B** | 0.57x | **0 B** |
| `SystemUri_Basic_Validate` | baseline | 56 B | baseline | 56 B | baseline | 56 B | baseline | 56 B | baseline | 56 B |

## W1

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | Windows arm64 ratio | Windows arm64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Ada_Basic_T1_SpanIn_SpanOut` | 0.36x | **0 B** | 0.51x | **0 B** | 0.88x | **0 B** | 0.72x | **0 B** | 0.70x | **0 B** |
| `Ada_Basic_T1_ReadEveryComponent` | 0.45x | **0 B** | 0.59x | **0 B** | 1.00x | **0 B** | 0.81x | **0 B** | 0.74x | **0 B** |
| `Ada_Basic_T2_SpanIn_StringOut` | 0.50x | 72 B | 0.62x | 72 B | 1.04x | 72 B | 0.83x | 72 B | 0.76x | 72 B |
| `Ada_Basic_T3_StringIn_StringOut` | 0.57x | 72 B | 0.73x | 72 B | 1.12x | 72 B | 0.95x | 72 B | 0.92x | 72 B |
| `SystemUri_Basic` | baseline | 288 B | baseline | 288 B | 1.00x | 288 B | baseline | 288 B | baseline | 288 B |

## W2

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | Windows arm64 ratio | Windows arm64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Ada_Complex_T1_SpanIn_SpanOut` | 0.66x | **0 B** | 0.64x | **0 B** | 1.03x | **0 B** | 0.88x | **0 B** | 0.87x | **0 B** |
| `Ada_Complex_T1_Normalize` | 0.68x | **0 B** | 0.63x | **0 B** | 1.00x | **0 B** | 0.88x | **0 B** | 0.89x | **0 B** |
| `Ada_Complex_T3_StringIn_StringOut` | 0.77x | 392 B | 0.72x | 392 B | 1.07x | 392 B | 0.97x | 392 B | 0.95x | 392 B |
| `SystemUri_Complex` | baseline | 2160 B | baseline | 2160 B | 1.00x | 2160 B | baseline | 2160 B | baseline | 2160 B |

## W3 full read

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | Windows arm64 ratio | Windows arm64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Ada_ReadAll` | 0.60x | **0 B** | 0.66x | **0 B** | 1.09x | **0 B** | 0.93x | **0 B** | 0.88x | **0 B** |
| `SystemUri_ReadAll` | baseline | 371 B | baseline | 371 B | baseline | 371 B | baseline | 371 B | baseline | 371 B |

## W3 hostname

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | Windows arm64 ratio | Windows arm64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Ada_ExtractHostname` | 0.73x | **0 B** | 0.79x | **0 B** | 1.34x | **0 B** | 1.10x | **0 B** | 1.02x | **0 B** |
| `SystemUri_ExtractHostname` | baseline | 218 B | baseline | 218 B | baseline | 218 B | baseline | 218 B | baseline | 218 B |

## W3 validate

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | Windows arm64 ratio | Windows arm64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `SystemUri_Validate` | baseline | 91 B | baseline | 91 B | baseline | 91 B | baseline | 91 B | baseline | 91 B |
| `Ada_Validate` | 1.06x | **0 B** | 1.14x | **0 B** | 1.83x | **0 B** | 1.38x | **0 B** | 1.12x | **0 B** |

## W4

| Benchmark | Linux x64 ratio | Linux x64 alloc | Linux arm64 ratio | Linux arm64 alloc | Windows x64 ratio | Windows x64 alloc | Windows arm64 ratio | Windows arm64 alloc | macOS arm64 ratio | macOS arm64 alloc |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `CanParse [WorkingSet=100]` | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** |
| `ReuseHandleAndReadHostname [WorkingSet=100]` | 2.17x | **0 B** | 2.06x | **0 B** | 2.48x | **0 B** | 2.85x | **0 B** | 2.55x | **0 B** |
| `ParseAndDispose [WorkingSet=100]` | 2.32x | **0 B** | 2.24x | **0 B** | 3.02x | **0 B** | 3.13x | **0 B** | 2.88x | **0 B** |
| `ParseAndReadHostname [WorkingSet=100]` | 2.34x | **0 B** | 2.28x | **0 B** | 3.11x | **0 B** | 3.16x | **0 B** | 2.97x | **0 B** |
| `ParseAndReadFive [WorkingSet=100]` | 2.50x | **0 B** | 2.35x | **0 B** | 3.07x | **0 B** | 3.27x | **0 B** | 2.99x | **0 B** |
| `CanParse [WorkingSet=10000]` | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** |
| `ReuseHandleAndReadHostname [WorkingSet=10000]` | 2.33x | **0 B** | 2.15x | **0 B** | 2.78x | **0 B** | 3.01x | **0 B** | 2.77x | **0 B** |
| `ParseAndDispose [WorkingSet=10000]` | 2.42x | **0 B** | 2.25x | **0 B** | 3.05x | **0 B** | 3.27x | **0 B** | 2.87x | **0 B** |
| `ParseAndReadHostname [WorkingSet=10000]` | 2.45x | **0 B** | 2.28x | **0 B** | 3.07x | **0 B** | 3.32x | **0 B** | 2.93x | **0 B** |
| `ParseAndReadFive [WorkingSet=10000]` | 2.57x | **0 B** | 2.36x | **0 B** | 3.27x | **0 B** | 3.42x | **0 B** | 2.88x | **0 B** |
| `CanParse [WorkingSet=200000]` | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** | baseline | **0 B** |
| `ReuseHandleAndReadHostname [WorkingSet=200000]` | 2.33x | **0 B** | 2.16x | **0 B** | 2.60x | **0 B** | 3.02x | **0 B** | 3.10x | **0 B** |
| `ParseAndDispose [WorkingSet=200000]` | 2.42x | **0 B** | 2.27x | **0 B** | 2.94x | **0 B** | 3.29x | **0 B** | 3.05x | **0 B** |
| `ParseAndReadHostname [WorkingSet=200000]` | 2.46x | **0 B** | 2.30x | **0 B** | 2.88x | **0 B** | 3.32x | **0 B** | 3.01x | **0 B** |
| `ParseAndReadFive [WorkingSet=200000]` | 2.55x | **0 B** | 2.38x | **0 B** | 3.08x | **0 B** | 3.44x | **0 B** | 3.25x | **0 B** |

## Detail

Full BenchmarkDotNet output, every column, one file per platform.

- [Linux x64](linux-x64.md)
- [Linux arm64](linux-arm64.md)
- [Windows x64](win-x64.md)
- [Windows arm64](win-arm64.md)
- [macOS arm64](osx-arm64.md)

## Reading these

A ratio of `0.50x` means half the time of `System.Uri`, so twice as fast.
`baseline` marks the row each group is measured against.

Allocation is the column that usually matters more. A parser that allocates
nothing does not add GC pressure no matter how many URLs go through it.
