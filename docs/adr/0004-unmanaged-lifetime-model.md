# ADR-0004: Unmanaged lifetime model, and why use after free is not enforced

- **Status:** accepted
- **Date:** 2026-08-25
- **Approved by:** Sanam

## Context

Ada's C ABI has two hazards that managed code has to handle.

`ada_string` and `ada_owned_string` are structurally identical. Both are a `const char*` and a
`size_t`, and the difference lives only in the header's prose. Four functions return owned memory
that leaks if it is not freed.

Every borrowed pointer dangles the moment the URL is mutated or freed.

## Decision

Three pieces, in order of preference.

**Handle free statics are the primary API.** `CanParse`, `TryNormalize`, `TryGetOrigin`,
`TryGetHostname`. Parse, use, and free all happen inside one call, and results are written into a
caller supplied `Span<byte>`. Nothing escapes and nothing allocates.

**`AdaUrl`, a stack bound `ref struct`,** for work that touches several properties. No object
header, no finalizer, no GC tracking. The `ref struct` rules stop the handle reaching a field, a
lambda capture, an async state machine, or the heap.

**`AdaUrlHandle : SafeHandle`** for fields, caches, async flows, and cross thread ownership. One
allocation and a finalizer, which is the visible price of being able to store a URL.

Owned strings are wrapped in a disposable `AdaOwnedString` ref struct so the free cannot be
skipped. Public APIs prefer the `Try...(..., Span<byte> destination, out int written)` shape, so
most callers never see native memory at all.

**Use after free of a borrowed span is handled by contract, tests, and ASAN, not by enforcement.**
An earlier design wrapped every getter result in a struct carrying a generation counter. That
wrapper would have appeared in the signature of every getter, taxing the exact path that has to
stay free.

## Consequences

The overhead target is met, and the common case, which is validating or normalising a URL in a
request pipeline, has no lifetime exposure at all.

The residual risk is real, so it stays open in the risk register instead of being marked
mitigated. A caller who holds a span across a setter reads freed memory. What we do about it:
document the invalidation on every getter and setter, make `CopyTo(Span<byte>)` the documented
rule for anything held across a mutation, cover it in test category 15, and run Linux ASAN and
LSAN over the whole conformance suite.

A forgotten `Dispose` on `AdaUrl` leaks native memory. Handled by making the handle free API the
documented default, and by handle counters that only exist in `ADA_DIAGNOSTICS` builds and are
asserted in test teardown.

## Alternatives considered

**Generation counter span wrapper.** Rejected. An API tax on the hot path.

**A Roslyn analyzer that errors on an `AdaUrl` outside a `using`.** Rejected for v1. An analyzer
project plus its test project, to catch one mistake. Revisit if leaks actually show up.

**`SafeHandle` only.** Rejected. An allocation and a finalizer per URL defeats the point.

**Always on `EventCounter` handle tracking.** Rejected. An `Interlocked.Increment` per parse is
measurable against a 50 ns operation.
