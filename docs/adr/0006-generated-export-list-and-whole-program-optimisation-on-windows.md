# ADR-0006: Generated export list and whole program optimisation on Windows

- **Status:** accepted
- **Date:** 2026-09-12
- **Approved by:** Sanam
- **Amends:** ADR-0003, section "Why Windows has no whole program optimisation"

## Context

ADR-0003 recorded that the Windows build ships without `/GL` and `/LTCG`. Ada has no
`__declspec(dllexport)`, so the DLL depends on upstream's `WINDOWS_EXPORT_ALL_SYMBOLS`, which
makes CMake run `cmake -E __create_def` across the compiled objects to generate an export file.
Under `/GL` those objects hold IL rather than COFF symbols and that step dies with `0xC0000005`.
The choice at the time was whole program optimisation with no exports, or a DLL that works.

ADR-0003 also said to quantify the cost before deciding whether it mattered. That did not happen
for a year. In the meantime `CHANGELOG.md` acquired a claim that the missing optimisation "costs
roughly a factor of two on the parse path", which was never measured, and the README repeated it
in gentler words as the explanation for Windows being level with `System.Uri` while Linux is 1.9x
ahead.

Issue #18 measured it. Two variants, three rounds each, rotated order, one runner, compared as a
ratio against `System.Uri` measured in the same process, because the runner drifts more between
rounds than the effect being measured is worth.

| Category | Benchmark | No `/GL` | `.def` + `/GL` | Change |
| --- | --- | ---: | ---: | ---: |
| W1 | span in, span out | 0.940, spread 0.080 | 0.950, spread 0.040 | -1.1% |
| W1 | read all ten | 1.060, spread 0.050 | 1.140, spread 0.070 | -7.0% |
| W1 | span in, string out | 1.060, spread 0.110 | 1.060, spread 0.110 | 0.0% |
| W1 | string in, string out | 1.230, spread 0.070 | 1.190, spread 0.140 | +3.4% |
| W2 | normalize | 1.070, spread 0.030 | 1.010, spread 0.030 | +5.9% |
| W2 | span in, span out | 1.090, spread 0.030 | 1.010, spread 0.040 | +7.9% |
| W2 | string in, string out | 1.190, spread 0.040 | 1.110, spread 0.020 | +7.2% |

W1 is noise: every change is the size of its own spread and the signs disagree. W2 is not: all
three rows move the same way by 6 to 8 percent with spreads of 0.02 to 0.04.

`ada.dll` also drops from 487,936 bytes to 279,552, which is 43 percent.

The factor of two was wrong. So was "it does not matter".

## Decision

1. **The Windows export list is generated from upstream's `include/ada_c.h` at build time**,
   written to a `.def` file, with `WINDOWS_EXPORT_ALL_SYMBOLS` turned off. `/GL` and `/LTCG` come
   back, because `cmake -E __create_def` is no longer in the build and so cannot crash on IL
   objects.

2. **The list is generated, never hand maintained.** A hand written list is a second copy of
   upstream's API, and it goes stale the next time the pinned tag moves with nothing failing to
   say so. The generator parses the header in the pinned clone on every build and throws if it
   finds fewer than 60 functions, because a parser that quietly matched almost nothing would
   produce a DLL that links, ships, and throws `EntryPointNotFoundException` on the consumer's
   first call.

3. **Upstream's tree is not patched.** `WINDOWS_EXPORT_ALL_SYMBOLS` is set on the `ada` target
   with no option guarding it, so it is turned off through a deferred call injected with
   `CMAKE_PROJECT_TOP_LEVEL_INCLUDES`. ADR-0003 point 1 pins upstream by release tag, and the
   checksum manifest, the SBOM and the provenance story all rest on the artifact being exactly
   that tag. A patched clone would end that.

4. **The exported surface narrows from everything to the documented C API**, 79 functions on
   v4.0.0. This is the part that changes what consumers see.

## Consequences

The Windows DLL is 43 percent smaller and about 7 percent faster on URLs heavy enough to do real
work. On a plain URL the difference is not measurable on a shared runner.

Anyone linking against a mangled C++ symbol in `ada.dll` loses it. Nobody can be doing that
through this package: the wrapper imports 79 entry points and `include/ada_c.h` declares exactly
79. A direct consumer of the native binary could be, and for them this is a breaking change to
the artifact, which is why it is written down here rather than slipped in.

`native/CHECKSUMS.txt` changes for `win-x64`, and the reproducibility check has to confirm that
`/LTCG` still produces the same bytes twice. If it does not, this decision is reverted rather
than the check weakened: a manifest that cannot tell a rebuild from a substitution is the only
thing it exists to detect.

The `all-symbols` path stays in `native/build-windows.ps1` so the comparison can be rerun when
the pinned tag moves. It is one parameter, not a second build.

## What this does not explain

Windows is still level with `System.Uri` where Linux is 1.9x ahead, and 7 percent on the hard
path does not close that. The remaining hypotheses are the MSVC code generator (#19) and the
Windows heap serving the two allocations every parse makes (#20). This ADR closes the question
ADR-0003 left open and no more.

## Alternatives considered

**Leave it as it was.** Rejected. The measurement says there is a real improvement on the heavy
path and a 43 percent size reduction, and the reason for not taking them was a crash that the
`.def` removes.

**Hand write the export list.** Rejected, see decision point 2.

**Patch upstream's `CMakeLists.txt` in the clone.** Rejected, see decision point 3.

**Wait for upstream to annotate its C API.** That work is #21 and is worth doing anyway, since it
fixes this for every binding rather than only ours. It is not a reason to wait: it is not merged,
not released, and not ours to schedule. When it lands in a tagged release the generated `.def`
goes away and upstream's annotations take over.
