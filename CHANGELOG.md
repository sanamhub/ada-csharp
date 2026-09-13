# Changelog

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versioning follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
  43% smaller, and a hard URL parses about 7% faster. A plain URL does not move measurably. The
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

- Why Windows is level with `System.Uri` while Linux is 1.9x ahead is not explained. Whole
  program optimisation was the standing answer and it is not the right one: turning it on is
  worth about 7%, not the factor of two this file used to claim without measuring. The
  hypotheses still standing are the MSVC code generator and the Windows heap serving the two
  allocations every parse makes. See #19 and #20.

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
