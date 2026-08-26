# ADR-0003: Native build and distribution strategy

- **Status:** accepted
- **Date:** 2026-08-25
- **Approved by:** Sanam

## Context

Ada gets part of its speed from SIMD, and the brief asked for AVX2 and BMI2 build flags to keep
it. Compiling the whole library with `/arch:AVX2` or `-mavx2 -mbmi2` does keep it, and at the
same time raises the minimum CPU to Intel Haswell or AMD Excavator. Below that the process dies
with `SIGILL`. That is not a fallback, it is a crash, possibly on a customer VM whose host
advertises an older feature mask.

## Decision

1. **Pin upstream by release tag**, currently `v4.0.0`, never `main`. Moving the tag is a
   deliberate change gated by the conformance suite, not a Dependabot PR.
2. **Ship an `x86-64-v2` baseline build with `ADA_USE_SIMDUTF=OFF`.** No AVX2 pinned artifact
   ships. The baseline keeps the CPU floor at 2009 hardware while Ada retains its own SIMD paths.
   simdutf was tried and rejected, see below.
3. **Six RIDs:** `win-x64`, `linux-x64`, `linux-arm64`, `linux-musl-x64`, `osx-x64`,
   `osx-arm64`. `win-arm64` deferred. No 32 bit native.

   Which of them get built is per workflow rather than fixed. A pull request builds the four its
   test matrix runs on, the benchmark builds the four it measures on, and a release builds all
   six.

   `osx-x64` is a special case. The binary is cross compiled on Apple Silicon through
   `CMAKE_OSX_ARCHITECTURES`, so producing it is fast and needs no Intel host, and the build
   checks the result with `lipo` so an arm64 binary cannot ship under that RID. Only *running*
   an x86_64 binary needs `macos-13`, a retiring Intel image that queued 20 to 40 minutes on
   every attempt and twice never started. Because `needs` is job level in GitHub Actions, that
   one leg blocks everything behind it, so its consumption test runs nightly rather than in the
   release path. A release that cannot start for 40 minutes is a release nobody cuts.
4. **Two macOS RIDs rather than a `lipo` universal binary.** The RID graph already picks the
   right asset, and a universal binary doubles the size of every deployment for nothing. The
   `lipo` recipe stays in the build script for single file bundle consumers.
5. **One package containing all natives**, not a metapackage plus per RID runtime packages.
6. **Natives are CI artifacts and are never committed.** Built only in CI from the pinned tag,
   with a SHA-256 manifest verified at pack time, code signing, macOS notarisation, and an SBOM.

## Why simdutf is off

The first attempt enabled `ADA_USE_SIMDUTF=ON`, on the reasoning that simdutf dispatches its
kernel at runtime and would therefore give SIMD speed without a static CPU floor. The first CI
run showed why that does not work here.

Ada pulls simdutf through CPM, and `BUILD_SHARED_LIBS=ON` propagates into it. simdutf is then
built as a **second shared library** rather than being linked in.

On Windows that build fails outright. `cmake -E __create_def`, which generates `exports.def` from
the object list, crashes with `0xC0000005` while processing the simdutf objects.

On Linux and macOS it succeeds but produces `libsimdutf.so.25.0.0` next to `libada.so`, so
`libada.so` gains a runtime dependency. Shipping it would mean a second binary per RID plus rpath
handling, and the package layout in this ADR assumes one file per RID.

Linking simdutf statically into a shared Ada is not reachable from the command line, because
`BUILD_SHARED_LIBS` is a single global setting that both projects read.

So the choice is one shared object per RID without simdutf, or two with it and no Windows
support. The first is obviously better. Ada is SIMD accelerated in its own right, and simdutf
only accelerates transcoding.

Worth revisiting if upstream gains an option to link simdutf statically, or if measurement shows
the transcoding path actually matters for our workload.

## Why symbol visibility stays at the default

`-fvisibility=hidden` looked like free hygiene. It is not, here.

Ada does not annotate its C API with visibility attributes, so hiding by default hides every
`ada_*` symbol. Combined with LTO and `--gc-sections`, the linker then has no roots to keep, and
CI produced a 14 KB `libada.so` that exported nothing at all. It built, it linked, and it would
have failed at the consumer's first P/Invoke.

The export gate caught it, which is the whole reason that gate exists. The flag is gone. Do not
add it back without patching upstream, which is not our call.

## Why Windows has no whole program optimisation

Ada has no `__declspec(dllexport)`, so the Windows DLL relies on upstream's
`WINDOWS_EXPORT_ALL_SYMBOLS`. That makes CMake run `cmake -E __create_def`, which reads the
compiled objects and writes an exports file.

With `/GL`, those objects hold IL rather than COFF symbols, and `__create_def` dies with
`0xC0000005` reading them. It crashed on simdutf first and then on `ada.vcxproj` once simdutf was
off, which is what identifies `/GL` rather than the dependency as the cause.

So the Windows build drops `/GL`, `/LTCG`, and `CMAKE_INTERPROCEDURAL_OPTIMIZATION`. The choice
was whole program optimisation with no exports, or a DLL that works. `/OPT:REF` and `/OPT:ICF`
still run, so link time optimisation is reduced rather than absent. Linux and macOS keep
`-flto=thin`, so this costs one platform, not all three.

Worth revisiting if upstream adds export annotations, which would remove the dependency on
`__create_def` entirely.

## Consequences

The baseline build runs on any x86-64-v2 CPU, which means 2009 hardware onward.

Windows binaries are optimised slightly less than Linux and macOS ones. Quantify it in the
benchmark run before deciding whether it matters.

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

**`ADA_USE_SIMDUTF=ON`.** Tried and rejected. See the section above.

**A separate opt in AVX2 package.** Rejected. A second package is a second thing to build, sign,
version, and support, and its whole value is a few percent on a workload nobody has measured yet.
Revisit only with numbers.

**A `lipo` universal macOS dylib as the shipped asset.** Rejected. Doubles size for no gain.

**Per RID runtime packages via `runtime.json`.** Rejected for now. Revisit if the native grows.
