# ADR-0001: Single target net10.0, UTF-8 first API

- **Status:** accepted
- **Date:** 2026-08-25
- **Approved by:** Sanam

## Context

Ada exposes a byte oriented C ABI: `const char*` plus `size_t`, UTF-8. The goal is a wrapper
with no measurable overhead over the native library.

The original brief asked us to evaluate multi targeting from modern .NET down to
`netstandard2.0`. `net8.0` was also considered as an LTS bridge.

`net10.0` gives us `[LibraryImport]` source generated marshalling, `[SuppressGCTransition]`,
`Utf8.FromUtf16`, `[SkipLocalsInit]`, `Span<T>` with real `ref` fields, and a clean
NativeAOT and trimming story. None of that exists on `netstandard2.0`.

## Decision

1. Ship one target framework: `net10.0`. No `netstandard2.0`, so no conditional compilation
   anywhere in the repository. CI enforces this with a grep.
2. The public API is UTF-8 first. `ReadOnlySpan<byte>` is the primary parameter and result
   shape. `string` and `ReadOnlySpan<char>` overloads exist for convenience and carry a
   documented transcode cost.
3. License the wrapper MIT. Ada is dual licensed Apache-2.0 or MIT, so MIT keeps the package
   to a single short license file with no compatibility question.

## Consequences

One code path. No `#if` divergence to audit, no `System.Memory` polyfill, no MSBuild file to
rescue native resolution on .NET Framework, no second CI lane. Allocation can be asserted
exactly with `GC.GetAllocatedBytesForCurrentThread()`, which is what turns the zero allocation
claim into a test instead of a benchmark.

No .NET Framework, Mono, or Unity support. Accepted because no such consumer exists for this
library.

The UTF-8 first choice has a limit worth stating plainly. End to end zero allocation only holds
when the caller passes UTF-8 and reads results as spans. Any `System.String` result allocates by
construction. Published benchmarks are therefore split into tiers, and the zero byte figure is
only ever quoted for the span in, span out tier.

## Alternatives considered

**net10.0 plus netstandard2.0.** Rejected. The full analysis stays in `ADA_WRAPPER_PLAN.md`
section 1 and doubles as the contingency plan if a .NET Framework consumer appears. It would
land behind the modern path, never in front of it, or its constraints leak into the primary API
design.

**net10.0 plus net8.0.** Rejected as superseded.

**A string based API using `StringMarshalling.Utf8`.** Correct, but it allocates and copies
through `Marshal.StringToCoTaskMemUTF8` on every call.
