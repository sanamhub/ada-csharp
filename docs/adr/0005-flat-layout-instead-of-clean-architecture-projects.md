# ADR-0005: Flat layout instead of four Clean Architecture projects

- **Status:** accepted
- **Date:** 2026-08-25
- **Approved by:** Sanam

## Context

The project standard is Clean Architecture by default: Domain, then Application, then
Infrastructure, then Presentation. Any deviation has to be written down and approved by name.

This library is a P/Invoke wrapper. The whole public surface is about five types (`AdaUrl`,
`AdaUrlHandle`, `AdaSearchParams`, `AdaIdna`, `AdaUrlComponents`) over one native dependency.
There is no domain logic, no persistence, no transport, and no dependency inversion to express.
The infrastructure is one static class of `[LibraryImport]` declarations. The domain is a URL
string that Ada itself owns.

## Decision

One project, flat layout:

```
src/Ada.Url/
  Interop/     AdaNative.cs, AdaNativeStructs.cs, Transcode.cs, NativeResolver.cs
  AdaUrl.cs  AdaUrlHandle.cs  AdaSearchParams.cs  AdaIdna.cs  AdaUrlComponents.cs
```

`Interop/` is the only place P/Invoke and `unsafe` appear. That is the entire layering rule, and
it holds through review and `.editorconfig` scoping rather than a project boundary.

An earlier draft proposed four folders plus a NetArchTest test enforcing dependency direction.
Both are dropped. With two conceptual layers, a test proving the direction is theatre.

## Consequences

Fewer projects to build, and a package with no artificial internal seams. Follows YAGNI.

If this library ever grows real domain logic, the split has to be introduced later. That is a
cheap refactor for a five type library, and an expensive tax to pay up front on a guess.

## Alternatives considered

**Four projects** (`Ada.Url.Domain`, `.Application`, `.Infrastructure`, `.Presentation`).
Rejected. Four assemblies and three project references to express one static interop class.

**One project, four folders, plus an architecture test.** Rejected. The folders would be named
after layers that do not exist here, and the test adds a dependency to prove a rule that two
files already make obvious.
