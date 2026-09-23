# ADR-0011: Ship Windows natives without /GL

- **Status:** accepted
- **Date:** 2026-09-23
- **Approved by:** Sanam
- **Decides:** the mitigation ADR-0010 left open
- **Supersedes:** ADR-0006, the part that turns `/GL` and `/LTCG` on

## Context

ADR-0010 established that `/GL` and `/LTCG` make MSVC produce different bytes on different runner
images from identical sources and identical component versions. With them off, both Windows RIDs
are byte identical across the two images sampled. It left the choice open because it has a cost on
both sides.

Measured in #45 with the export list held fixed, so only `/GL` moves. Positive is slower without it.

| | `win-x64` | `win-arm64` |
| --- | ---: | ---: |
| `CanParse`, working set 100 to 200,000 | +3.7 to +8.5% | +11.2 to +12.7% |
| W2 normalize, hard URL | +4.9% | +2.8% |
| W1 span in, span out, plain URL | +1.7% | -0.7% |

## Decision

Windows natives ship with `/GL` and `/LTCG` off. `-Ipo` on `native/build-windows.ps1` defaults to
`off`. `on` stays for `win-perf-experiment.yml` and `win-drift.yml`.

The generated export list from ADR-0006 stays. It halves the DLL on its own merits.

The committed hashes become `31f854b0` for `win-x64` and `7a85fbee` for `win-arm64`, the `ipo=off`
outputs recorded on both images in #45.

## Why

This library is a thin wrapper that ships prebuilt native code. The checksum manifest is how a
consumer and the release job know the bytes came from the pinned sources. With `/GL` on it cannot
gate either Windows binary, and a mismatch during an image rollout teaches whoever is on the release
to regenerate the manifest, which is worse than having no check.

The cost falls on validation. `CanParse` ran at 0.79x of `Uri.TryCreate` on Windows x64 and 0.54x on
arm64 in `docs/benchmarks/0.1.0/`. A 13 percent regression leaves it well ahead on both.

## Consequences

Windows is a few percent slower on validation and on hard URLs from 0.1.1. `docs/benchmarks/0.1.0/`
describes the `/GL` build and stays as it is.

Pinning the runner image was rejected in #45: it trades a reproducibility problem for one about
security updates.

Two images is a sample, not proof. If a future image changes the `ipo=off` output, the release
checksum step will say so, and the right first move is `win-drift.yml`, not a regenerated manifest.
