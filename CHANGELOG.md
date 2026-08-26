# Changelog

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versioning follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Benchmark `W4`, which measures the gap between validating a URL and parsing one. About two
  thirds of a parse is `ada_parse` allocating a URL object and `ada_free` releasing it, so the
  gap is now tracked rather than rediscovered.

### Changed

- README performance section reports sustained throughput and per platform ratios alongside the
  existing per call figures. The headline said "roughly twice as fast as `System.Uri`", which
  came from a single URL microbenchmark with a hot cache on one platform. Validation is 3 to 4x
  on every platform, a full parse is 1.9x on Linux x64 and level on Windows x64, and allocation
  is zero everywhere.
- `docs/benchmarks/0.1.0-beta.1/` holds results for all four platforms. The alpha results are
  kept and marked superseded.

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

### Known

- The Windows native library is built without whole program optimisation, which costs roughly a
  factor of two on the parse path. `/GL` breaks `cmake -E __create_def`, which the build depends
  on because Ada has no `__declspec(dllexport)`. Writing the export list explicitly would allow
  `/GL` and `/LTCG` to come back. Validation and allocation are unaffected. See ADR-0003.

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
