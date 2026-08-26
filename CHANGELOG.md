# Changelog

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versioning follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Engineering plan (`ADA_WRAPPER_PLAN.md`) covering framework targeting, native build,
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

### Notes

- Search params and IDNA are bound at the interop layer but not yet wrapped. They arrive in P3
  along with the WHATWG conformance corpus.
