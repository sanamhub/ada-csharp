# Changelog

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versioning follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- `win-arm64`. Native binaries for it ship in the package, the conformance suite runs on a
  `windows-11-arm` runner, and the reproducibility check and the checksum manifest cover it. The
  binary cross compiles from the x64 runner, so `native/verify-windows.ps1` now reads the PE
  machine field and fails unless it matches the RID: an x64 binary shipped under `win-arm64`
  would pass every other gate and then fail on exactly the machines the RID exists for.
  `/CETCOMPAT` is not passed there, because CET is x86 and x64 only and the linker takes the flag
  and emits nothing. ADR-0008.

- Benchmark `W4` gained `ReuseHandleAndReadHostname`, which re-parses into one `AdaUrl` through
  `TrySetHref` instead of allocating a URL object per URL. It is the only lever the binding has
  over the native allocation and it is worth the most on Windows.
- `native/bench/alloc-probe.cpp` and a dispatch only workflow that runs it on Windows and Linux.
  It replaces global `operator new` to count and size what `ada_parse` allocates, then times
  `ada_can_parse`, `ada_parse` with `ada_free`, a replay of just the recorded allocations, and
  `ada_set_href` on a kept handle, reporting each against the allocation free control because the
  two runners are different machines.

### Changed

- README says why Windows is slower instead of listing hypotheses, and documents handle reuse
  with the case where it does not help. The Windows gap is the heap: `ada_parse` allocates twice
  per URL and replaying those two allocations alone costs 87 ns on Windows against 22 ns on
  Linux, which is 76% of everything the Windows parse spends beyond validating. Reproduced in two
  runs, decided in ADR-0007, measured in #20.
- The claim that the native allocation is "an upstream limit rather than something this package
  can route around" was half wrong and is corrected. `ada_set_href` re-parses into an existing
  handle, so a loop can avoid one of the two allocations. Worth 27% on Windows and 8% on Linux
  on a plain URL, and nothing at all on a hard one.

### Removed

- The clang-cl toolset option in `native/build-windows.ps1`, `native/cmake/clang-cl-tweaks.cmake`,
  the toolset probe workflow, and the two clang-cl variants in the performance experiment. It
  existed to answer #19, whether a different code generator closes the Windows gap. ADR-0007
  answered that from the other direction: the gap is the heap, and both compilers link against
  the same one. The path also carried a build flag that never worked, `/p:UseLldLink=false`,
  which is worse to leave in than to remove.

### Fixed

- The ADR-0001 gate stopped reading generated code. It greps `src` and `tests` for `#if` and runs
  after the build, so it was walking `obj` too. xunit.v3 4.0.1 added an
  `#if XUNIT_GENERATED_DISABLE_WARNINGS` guard to its generated test entry point and failed a gate
  about source on a dependency bump that touched none.

## [0.1.0-beta.2] - 2026-09-13

Still beta. No public API changed in this release: it is a faster and much smaller Windows
binary, a hardening gate that was not actually gating, and a performance section that now says
what the measurements support rather than what was assumed.

### Added

- Benchmark `W4`, which measures the gap between validating a URL and parsing one. About two
  thirds of a parse is `ada_parse` allocating a URL object and `ada_free` releasing it, so the
  gap is now tracked rather than rediscovered.

### Changed

- README performance section reports sustained throughput and per platform speedups alongside
  the existing per call figures. The headline said "roughly twice as fast as `System.Uri`", which
  came from a single URL microbenchmark with a hot cache on one platform. A full parse is 1.9x
  on Linux x64 and level on Windows x64, and allocation is zero everywhere.
- README performance section is a third shorter, down from eight subsections to five, and the
  comparison column now reads as a speedup, 1.9x, rather than a ratio below one, 0.52x, which is
  easy to read backwards. The per platform table no longer has a column whose every cell said
  "see below".
- `docs/benchmarks/0.1.0-beta.1/` holds results for all four platforms. The alpha results are
  kept and marked superseded.
- The win-x64 native library is built with whole program optimisation, behind an export list
  generated from upstream's `include/ada_c.h`. `ada.dll` drops from 487,936 bytes to 279,552,
  43% smaller, and a hard URL parses about 7% faster. Both changed at once, so that 7% belongs to
  the pair and not to either half. A plain URL does not move measurably. The
  exported surface narrows from every mangled C++ symbol in the library to the 79 functions the
  C API declares, which is a breaking change only for something linking against Ada's internals
  directly: this package imports 79 entry points and the header declares exactly 79. Measured
  over three rotated rounds in #18, decided in ADR-0006.
- The test suite runs on Microsoft.Testing.Platform. xunit.v3 4.0.0 ships on MTP v2, which no
  longer bridges to VSTest on the .NET 10 SDK, so `global.json` selects the MTP runner and the
  VSTest host, adapter and collector are gone. Coverage now comes from
  Microsoft.Testing.Extensions.CodeCoverage and the TRX report from
  Microsoft.Testing.Extensions.TrxReport. Every workflow filters with
  `--filter-trait` rather than `--filter`. All 1,253 tests pass unchanged.

### Fixed

- Benchmark `W1` compared `CanParse` against `new Uri()` plus reading three components, which is
  not the same work, and the README repeated the result as "about 4x faster to validate". Against
  the cheapest equivalent, `Uri.TryCreate` discarded, validation is about 1.3x faster on a plain
  URL and slightly slower on a corpus heavy in internationalised hosts. `CanParse` now has its
  own category, `W0 validate`, with a like for like baseline, and the README says what the
  measurements support.
- The benchmark collation produced a summary with no comparison in it. Every ratio printed as
  `n/a` and every benchmark was grouped under a heading called "other", because the script read
  BenchmarkDotNet's JSON export, which carries neither a `Categories` field nor a baseline
  marker. It now reads the markdown export, which has both, and fails loudly if no row ends up
  with a ratio.
- The nuget.org page had no project website link. `PackageProjectUrl` was never set.

### Security

- The win-x64 hardening gate checks CET instead of assuming it. `/CETCOMPAT` is recorded in the
  PE debug directory rather than in `DllCharacteristics`, so `native/verify-windows.ps1` read
  four bits and took the fifth on trust. A toolchain that accepts the flag and emits no record
  would have shipped a binary with no shadow stack support, and nothing would have said so. The
  check is skipped on machines that are not x64, where `/CETCOMPAT` has no equivalent.

### Known

- Windows is level with `System.Uri` where Linux is 1.9x ahead, and the cause is now narrowed to
  allocation. Whole program optimisation was the standing answer and it is the wrong one: the
  generated export list and `/GL` together are worth about 7% on a hard URL, not the factor of
  two this file used to claim without measuring, and the two changed together so neither can take
  the credit alone.

  What the benchmarks actually say is that Ada's non-allocating work is not slow on Windows at
  all. `CanParse` costs 40.9 ns there against 47.0 ns on Linux. The whole divergence is in the
  step from validating to parsing, which is where `ada_parse` allocates a result object and the
  `std::string` buffer inside it: 95.3 ns on Windows against 43.2 ns on Linux, 2.2x, and 2.8x
  once normalised against the same machine's managed baseline.

  That points at the allocator rather than the code generator, so #20 is the live question and
  #19 is much less promising than it looked. Tracked in #20.

## [0.1.0-beta.1] - 2026-08-26

First published release. Beta rather than alpha because the WHATWG conformance suite passes in
full on four platforms and the package is verified by installing it into a clean project, but
the public API has had no outside use yet and may still move.


### Added

- Engineering plan (`docs/ADA_PLAN.md`) covering framework targeting, native build,
  P/Invoke architecture, conformance testing, benchmarks, and CI.
- Repository scaffold: solution, library and test projects on `net10.0`, central package
  management, analyzer and lint gates, `ci.yml`, and ADR-0001 through ADR-0005.
- Native build for all six RIDs: build scripts for Windows, Linux, Alpine, and macOS, plus
  `native.yml` with export, hardening, and checksum gates.
- Interop layer: the full `ada_c.h` surface bound with `LibraryImport`, blittable ABI structs,
  a native resolver for single file and development layouts, and allocation free UTF-16
  transcoding.
- Public API: `AdaUrl` for multi property work, one shot statics for validation and
  normalisation, `AdaUrlComponents`, and `AdaLibrary`.
- Tests: ABI conformance, parsing behaviour, and allocation assertions.
- WHATWG conformance suite over the vendored web-platform-tests corpus, pinned at a known
  commit: 874 parse cases and 278 setter cases.
- `AdaSearchParams` with an allocation free struct enumerator, and `AdaIdna` for UTS-46
  conversion.

### Notes

- Conformance is verified on Linux x64, Linux arm64, macOS arm64, and Windows x64.
