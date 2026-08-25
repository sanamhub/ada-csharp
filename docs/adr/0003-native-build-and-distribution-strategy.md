# ADR-0003: Native build and distribution strategy

- **Status:** proposed. Item 2 is blocked on the P1 measurement.
- **Date:** 2026-08-25
- **Approved by:** Sanam (items 1, 3, 4, 5, 6)

## Context

Ada gets part of its speed from SIMD, and the brief asked for AVX2 and BMI2 build flags to keep
it. Compiling the whole library with `/arch:AVX2` or `-mavx2 -mbmi2` does keep it, and at the
same time raises the minimum CPU to Intel Haswell or AMD Excavator. Below that the process dies
with `SIGILL`. That is not a fallback, it is a crash, possibly on a customer VM whose host
advertises an older feature mask.

## Decision

1. **Pin upstream by release tag**, currently `v4.0.0`, never `main`. Moving the tag is a
   deliberate change gated by the conformance suite, not a Dependabot PR.
2. **Ship an `x86-64-v2` baseline build with `ADA_USE_SIMDUTF=ON`.** simdutf picks AVX-512,
   AVX2, or SSE kernels at runtime, so we get SIMD speed on whatever CPU is actually running
   without setting a floor. `ADA_USE_SIMDUTF` is off by default upstream, so this item stays
   proposed until the P1 measurement compares simdutf on, simdutf off, and an AVX2 pinned
   reference build. No AVX2 pinned artifact ships either way.
3. **Six RIDs:** `win-x64`, `linux-x64`, `linux-arm64`, `linux-musl-x64`, `osx-x64`,
   `osx-arm64`. `win-arm64` deferred. No 32 bit native.
4. **Two macOS RIDs rather than a `lipo` universal binary.** The RID graph already picks the
   right asset, and a universal binary doubles the size of every deployment for nothing. The
   `lipo` recipe stays in the build script for single file bundle consumers.
5. **One package containing all natives**, not a metapackage plus per RID runtime packages.
6. **Natives are CI artifacts and are never committed.** Built only in CI from the pinned tag,
   with a SHA-256 manifest verified at pack time, code signing, macOS notarisation, and an SBOM.

## Consequences

The baseline build runs on any x86-64-v2 CPU, which means 2009 hardware onward, while still
using AVX-512 where it exists.

Six RIDs cost six CI matrix legs and six ABI conformance lanes.

`linux-musl-x64` is not optional. A glibc `.so` will not load on Alpine, and containers are a
first class target.

The hardening flags are required, not optional: CFG, ASLR, high entropy VA, and CET
on Windows, RELRO, NX, and stack protector on Linux. A native library without them will not pass
security review.

One trap is gated in CI. On ELF and Mach-O, `-fvisibility=hidden` exports only symbols with an
explicit visibility attribute. If upstream does not annotate its C API, we would ship a library
that links and exports nothing. The export symbol gate catches it. The fix is to drop the flag,
not to patch upstream.

## Alternatives considered

**AVX2 pinned as the default build.** Rejected. A hidden CPU floor and a hard crash, for a few
percent.

**A separate opt in AVX2 package.** Rejected. Runtime dispatch already gets the win, and a second
package is a second thing to support.

**A `lipo` universal macOS dylib as the shipped asset.** Rejected. Doubles size for no gain.

**Per RID runtime packages via `runtime.json`.** Rejected for now. Revisit if the native grows.
