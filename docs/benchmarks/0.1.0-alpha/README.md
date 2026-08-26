# Benchmark results, 0.1.0-alpha

Run on a GitHub hosted `ubuntu-24.04` runner. Commit `eee1cd6`, Ada v4.0.0, `x86-64-v2` baseline
build.

```
BenchmarkDotNet v0.15.2
Runtime .NET 10.0.11 (10.0.1126.37416), X64 RyuJIT AVX2
GC Concurrent Workstation
HardwareIntrinsics AVX2, AES, BMI1, BMI2, FMA, LZCNT, PCLMUL, POPCNT, VectorSize=256
```

## Read this first

**The ratios are trustworthy. The absolute nanosecond figures are indicative.** A shared CI
runner has noisy neighbours and no frequency guarantees. Both parsers ran on the same machine in
the same process, so comparing them to each other is fair. Comparing these numbers to a run on
different hardware is not.

**The two parsers do not implement the same specification.** Ada follows WHATWG. `System.Uri`
follows RFC 3986 and 3987 plus a decade of .NET specific behaviour. They disagree on real inputs,
so speed is only half the picture.

**The tiers matter.** T1 is UTF-8 in and span out, which is where zero allocation is possible. T2
returns a `string`, which allocates by construction. T3 takes a `string` and returns one, which is
the only like for like comparison against `System.Uri`. The zero byte figure only ever refers to
T1.

## W1, a plain URL

`https://example.com/path`

| Method | Mean | Ratio | Allocated | Alloc ratio |
| --- | ---: | ---: | ---: | ---: |
| `Ada.CanParse` | 47.5 ns | 0.26 | **0 B** | 0.00 |
| `Ada` T1, span in, span out | 92.4 ns | 0.50 | **0 B** | 0.00 |
| `Ada` T1, read all ten components | 100.7 ns | 0.55 | **0 B** | 0.00 |
| `Ada` T2, span in, string out | 109.2 ns | 0.59 | 72 B | 0.25 |
| `Ada` T3, string in, string out | 122.0 ns | 0.66 | 72 B | 0.25 |
| `System.Uri` | 184.6 ns | 1.00 | 288 B | 1.00 |

## W2, a complex URL

Credentials, a non default port, dot segments, percent encoding, an internationalised host, and a
heavy query.

| Method | Mean | Ratio | Allocated | Alloc ratio |
| --- | ---: | ---: | ---: | ---: |
| `Ada` T1, normalise into a caller buffer | 1,134 ns | 0.67 | **0 B** | 0.00 |
| `Ada` T1, span in, span out | 1,139 ns | 0.68 | **0 B** | 0.00 |
| `Ada` T3, string in, string out | 1,374 ns | 0.82 | 392 B | 0.18 |
| `System.Uri` | 1,684 ns | 1.00 | 2,160 B | 1.00 |

## What the numbers say

**Zero allocation holds.** Every T1 row reports 0 B and no Gen0 collections. That is the headline
claim, and it is also asserted deterministically in the test suite rather than left to a
benchmark, because a shared runner cannot gate anything.

**Roughly 2x on the simple case, 1.5x on the complex one.** On W1, span in and span out is half
the cost of `System.Uri` with none of the garbage. Validation alone is about a quarter.

**Allocation is where the gap is widest.** `System.Uri` allocates 2,160 bytes parsing the complex
URL. The span path allocates nothing, and even the string returning path allocates 392 bytes, a
fifth as much. In a request pipeline parsing thousands of URLs a second, that is the difference
that shows up as GC pressure.

**The complex URL costs about 1.1 microseconds in both parsers.** That is IDNA plus percent
decoding, and it dwarfs the wrapper. Do not expect interop tuning to move it.

## An answer to an open question

The plan left one decision to measurement: whether to build a component slicing fast path that
reads the eight `ada_url_components` offsets and slices the href, instead of calling ten separate
getters.

Reading **all ten components costs 100.7 ns against 92.4 ns for three**. Eight extra native calls
cost roughly 8 ns in total, under a nanosecond each, because they are `[SuppressGCTransition]`
leaves. A slicing layer could therefore save at most about 9% of an already fast path, and it
would introduce real correctness risk: every one of those eight fields can carry the
`ada_url_omitted` sentinel, and an unchecked cast to `int` produces `-1` and an out of range
slice.

**Decision: do not build it.** The getters are effectively free. This closes AC-5.6.

## Reproducing

```bash
dotnet run -c Release --project benchmarks/Ada.Url.Benchmarks -- --filter '*'
```

Needs the native library. In CI the `bench` workflow downloads it; locally it has to be built
from `native/`. For numbers worth quoting in absolute terms, run on a machine you control rather
than a shared runner.
