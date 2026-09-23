# Changelog

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versioning follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Published benchmark results for 0.1.0, in `docs/benchmarks/0.1.0/`, covering the five platforms
  with a hosted runner. Every native in the run was a cache hit whose sha256 matches
  `native/CHECKSUMS.txt`, so the numbers describe the released bytes rather than a rebuild.
  Windows arm64 is measured for the first time. The export list and `/GL` change from ADR-0006
  shows up as 10 percent on W2, flat on Linux over the same pair of runs, which is the control
  that makes it attributable. W1 improved on Windows too, but improved further on Linux with no
  build change, so the page credits that to the runner rather than to the optimisation.
- `linux-musl-arm64`. Alpine on arm64 got a `DllNotFoundException` for a reason that had nothing
  to do with the caller's code. Built by the existing `build-musl.sh` on an arm64 runner, and the
  consumption test runs in an Alpine container on arm64, so the library is loaded on the platform
  it targets rather than assumed to work because it compiled. ADR-0009.

### Changed

- Windows natives build without `/GL` and `/LTCG`, so both Windows binaries are byte identical
  across runner images and `native/CHECKSUMS.txt` gates them again. With them on, MSVC produced
  different bytes on different images from identical sources and component versions, and a
  release had roughly a coin flip per Windows RID of failing its checksum step for an innocent
  reason. Costs 4 to 13 percent on `CanParse` and 3 to 5 percent on a hard URL. `CanParse` stays
  well ahead of `Uri.TryCreate`. The generated export list stays. ADR-0011, #45.
- `native/build-windows.ps1` takes `-Ipo on|off`, default `off`. `on` exists so the comparison can
  be rerun. `all-symbols` with `/GL` is refused up front, since it dies in `cmake -E __create_def`
  on IL objects.
- `win-perf-experiment.yml` gains variant `c`, a pure `/GL` comparison, and a RID input so
  `win-arm64` can be measured on `windows-11-arm`. Variant `a` moves exports and `/GL` together,
  so #18's 7 percent covers the pair rather than `/GL` alone.

### Fixed

- `scripts/collate-benchmarks.py` no longer drops a platform without saying so. Its platform list
  is hand maintained, `win-arm64` was added to `bench.yml` and not to it, and the result was a
  green run and a summary that simply had no Windows arm64 column. It now fails when a
  `benchmark-<rid>` artifact arrives for a RID it does not know.
- `scripts/collate-benchmarks.py` no longer labels a measured row as the baseline of its group.
  It read the baseline off `Ratio` being `1.00`, which a row that lands within half a percent of
  the baseline also prints. Two real Windows x64 rows in this run did exactly that, 158.90 ns
  against 158.15 ns. `Program.cs` now asks BenchmarkDotNet for its `Baseline` column, and the
  script prefers it, falling back to `Ratio` only where a single row in the group claims `1.00`.
- The Alpine image was pinned to an amd64 image digest rather than to the multi-architecture
  index. On an arm64 host docker either refuses that or runs it under emulation and produces an
  x86-64 `.so` in the `linux-musl-arm64` directory. The pin is now the 3.20.0 index, which
  resolves per host and keeps both musl RIDs on one Alpine release. The amd64 image inside it is
  the one the old pin named, so `linux-musl-x64` is byte for byte unchanged.
- `native/build-musl.sh` hands ownership of what it built back to the invoking user. The
  container runs as root, so everything it wrote into the bind mount was root owned and the
  runner user could not delete it. The reproducibility check does exactly that between its two
  builds and failed with `Permission denied` on every file the first build produced. `apk` needs
  root, so ownership is handed back on the way out rather than dropped on the way in.
- `native/verify-unix.sh` checks the ELF machine type against the RID instead of trusting the
  runner. A container built for one platform, or a QEMU binfmt handler doing its job quietly,
  produces a library for the wrong architecture that passes exports, hardening and checksums, and
  then fails on exactly the machines the RID exists for. ADR-0008 added the PE half of this for
  `win-arm64`.

## [0.1.0] - 2026-09-17

Out of beta. Nothing in the API changed to earn that: the surface has been stable since
0.1.0-beta.1 and moves to `PublicAPI.Shipped.txt` unchanged. What changed is that the questions
the beta was holding open have answers.

`0.1.0-beta.1` said beta because the API "may still move". It has not, through two betas. The
Windows performance gap was the other open question and it is measured now, in ADR-0006 and
ADR-0007, rather than explained by a guess this file used to print as fact.

This is still `0.x`. Under SemVer that means a minor bump may break the API, so the promise here
is that the surface is considered done, not that it is frozen.

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
