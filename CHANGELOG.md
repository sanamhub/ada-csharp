# Changelog

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versioning follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed

- The nuget.org page had no project website link. `PackageProjectUrl` was never set.

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
