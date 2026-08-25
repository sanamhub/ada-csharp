# Changelog

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versioning follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Engineering plan (`ADA_WRAPPER_PLAN.md`) covering framework targeting, native build,
  P/Invoke architecture, conformance testing, benchmarks, and CI.
- Repository scaffold: solution, library and test projects on `net10.0`, central package
  management, analyzer and lint gates, `ci.yml`, and ADR-0001 through ADR-0005.

### Notes

- No functional API yet. `AdaUrlInfo.UpstreamAdaVersion` is a placeholder. The URL API lands in
  P2 and P3 of the roadmap.
