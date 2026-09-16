# ADR-0008: Ship win-arm64

- **Status:** accepted
- **Date:** 2026-09-16
- **Approved by:** Sanam
- **Amends:** ADR-0003, point 3, the deferred RID list

## Context

ADR-0003 shipped six RIDs and deferred `win-arm64`. The reason was cost, not a blocker: a
seventh native to build, verify, checksum, reproduce and consume, for a platform with no runner
to test it on. Both halves of that have moved.

`windows-11-arm` is generally available and free for public repositories, so there is somewhere
to run the conformance suite and the consumption test. And the binary cross compiles from the
existing x64 runner with `-A ARM64`, so no separate build host is needed. Ada already defines
`ADA_NEON` for `_M_ARM64`, so the SIMD path is not lost on the way over.

## Decision

1. **`win-arm64` ships**, built on the x64 runner, executed on `windows-11-arm`.

2. **`/CETCOMPAT` comes off for this RID and nothing else changes.** CET shadow stacks are x86
   and x64 only. `link.exe` accepts the flag for an arm64 target and emits no record, which is
   exactly the failure `native/verify-windows.ps1` was taught to catch in 0.1.0-beta.2. Passing a
   flag whose only possible outcome here is a gate that has to be weakened to let it through
   would be worse than not passing it, so the build does not pass it and the verifier skips the
   check on any machine that is not x64.

   `/guard:cf`, `/DYNAMICBASE`, `/HIGHENTROPYVA`, `/Brepro` and `/PDBALTPATH` all still apply and
   stay required.

3. **The machine type of the output is checked, not trusted.** `native/verify-windows.ps1` reads
   the COFF machine field and fails unless it matches the RID: `0xAA64` for `win-arm64`, `0x8664`
   for `win-x64`. The RID defaults to the last segment of the artifact directory, so every
   existing call site gets the check without being changed and none of them can forget to ask
   for it.

4. **The x86-64-v2 baseline argument does not carry over.** ADR-0003 picked that floor so the x64
   artifact runs on any CPU from 2009. arm64 has no equivalent decision to make, so this RID
   takes the default and the absence of a baseline flag is deliberate rather than an oversight.

## Consequences

The package grows by one native library. Every gate that covered `win-x64` now covers
`win-arm64`: exports, the imported entry point set, hardening flags, the reproducibility check,
the checksum manifest and the consumption test from a clean project.

Cross compiling means the build machine never runs what it produced. That is why point 3 exists
and why the conformance suite runs on `windows-11-arm` rather than being skipped as "same source,
same compiler". ADR-0003 makes the same argument for `osx-x64`, where `lipo` answers it; this is
the PE equivalent, and unlike `osx-x64` there is a runner to execute on.

Pull request CI grows a fifth native and a fifth test leg. The native build is cached on the
upstream tag and the hash of `native/`, so a pull request that does not touch the build scripts
still pays for neither.

`native/CHECKSUMS.txt` gains a line. Until the reproducibility check has run for this RID, that
line is a hash from one build rather than a hash confirmed to repeat, which is the same position
`win-x64` was in before `/Brepro` landed.

Signing does not cover it, because signing does not cover anything yet: `scripts/sign.ps1` is
gated on a certificate that does not exist in the `production` environment. When one does, it
covers the whole `out` directory and picks this up with no change.

## Alternatives considered

**Build natively on `windows-11-arm`.** Rejected for now. It would remove the cross compile and
with it the need for point 3, but it adds a build host and a second toolchain to keep matching,
and the reproducibility check would then be comparing builds from a machine class the other six
RIDs do not use. Cross compiling with a machine check is the smaller change. If the ARM64 toolset
ever stops shipping alongside MSVC on the x64 image, this is the fallback.

**Keep deferring it.** Rejected. The two reasons ADR-0003 gave have both gone away, and Windows
on arm is no longer a rounding error in the install base.
