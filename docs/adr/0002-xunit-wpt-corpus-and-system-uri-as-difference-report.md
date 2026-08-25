# ADR-0002: xUnit v3, pinned WPT corpus, System.Uri as a difference report

- **Status:** accepted
- **Date:** 2026-08-25
- **Approved by:** Sanam

## Context

This library's value is WHATWG compliance, so the test suite is the real specification. Three
choices had to be settled before writing any tests.

## Decision

**xUnit v3 as the test framework.** It supports `net10.0` directly, and its `[Theory]` and
`MemberData` handling is what makes a corpus of tens of thousands of vectors workable.

**Vendor the web-platform-tests URL corpus at a pinned commit** under `tests/vectors/`, with the
commit hash recorded in `PROVENANCE.md`. A weekly CI job diffs against upstream and opens an
issue when it drifts. The corpus is never fetched at test time.

**Treat `System.Uri` as a difference report, not a test oracle.** It implements RFC 3986 and
3987 plus a decade of .NET specific behaviour, and it is not WHATWG compliant.

## Consequences

A JSON to `[Theory]` adapter turns each vector into a separately named test, so a failure names
the input that broke rather than reporting that the conformance suite failed.

Pinning the corpus keeps tests off the network. Network dependent tests flake, and a corpus that
updates silently turns a regression into a mystery.

Divergences from `System.Uri` are generated into `docs/system-uri-differences.md` and committed.
A new row is a signal to investigate, not automatically a failure. Only a small curated set where
both parsers must agree is asserted. That file is a deliverable on its own. It answers the
question every prospective user asks, which is what changes if I switch.

## Alternatives considered

**NUnit.** Would work. xUnit chosen for the theory handling.

**Fetching the corpus at test time.** Rejected. Network dependent tests are flaky tests.

**Using `System.Uri` as an oracle.** Rejected. It would encode non compliant behaviour as the
expected result.
