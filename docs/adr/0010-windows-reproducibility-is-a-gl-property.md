# ADR-0010: Windows reproducibility is a property of /GL

- **Status:** accepted
- **Date:** 2026-09-18
- **Approved by:** Sanam
- **Amends:** ADR-0008, the reproducibility paragraph
- **Follows:** ADR-0006, ADR-0008

## Context

ADR-0008 said the `win-arm64` hash in `native/CHECKSUMS.txt` was "confirmed to repeat rather than
a hash from one build", and cited three builds. All three ran on runner image `20260907.297`.
Three builds on one image is one observation about images, not three. #45 opened when a fourth
build, on a newer image, produced different bytes.

The first reading of that was wrong too, and in a way worth recording: `win-arm64` looked like the
only affected RID because the two RIDs had landed on different images in the same run, and one
observation of each was read as a comparison.

`win-drift.yml` settled it. Four samples, both images, each building both RIDs with whole program
optimisation on and off in a single job so the image cannot vary within a sample.

| rid | `/GL` | on `20260907.297.1` | on `20260913.307.1` |
| --- | --- | --- | --- |
| `win-x64` | on | `c426ca6a` | `2e3e93c5` |
| `win-x64` | off | `31f854b0` | `31f854b0` |
| `win-arm64` | on | `e73ed3ed` | `b13434aa` |
| `win-arm64` | off | `7a85fbee` | `7a85fbee` |

With `/GL` and `/LTCG` off, both Windows RIDs are byte identical across both images. With them on,
both track the image. It is not per machine: `20260913.307.1` was sampled in three separate runs on
three separate runners and produced the same two hashes every time.

The toolchain is identical between the images, which is why nothing caught this. `cl /Bv` names
every compiler pass separately and they all match.

| | `20260907.297.1` | `20260913.307.1` |
| --- | --- | --- |
| MSVC toolset directory | `14.44.35207` | `14.44.35207` |
| `cl.exe`, `c1.dll`, `c1xx.dll`, `c2.dll` | `19.44.35228.0` | `19.44.35228.0` |
| `link.exe`, `mspdb140.dll` | `14.44.35228.0` | `14.44.35228.0` |
| Windows SDK | `10.0.26100.0` | `10.0.26100.0` |
| CMake | `3.31.6` | `3.31.6` |
| Visual Studio installation | `17.14.37614.0` | `17.14.37628.2` |

Every code generating component reports the same version. The only difference the toolchain admits
to is the Visual Studio installation version, which is the shell and the installer rather than the
compiler. So a version pin would not have caught this and a version comparison would have called
the two images identical.

`reproducible.yml` could not catch it either, by construction: it builds twice inside one job on
one machine, so it tests for embedded timestamps, which is what it was written for, and it cannot
see across images. Both arm64 samples passed it while disagreeing with each other.

And v0.1.0 passed its checksum gate because the `natives` job restored both Windows binaries from
cache and never rebuilt them. The gate compared the cached bytes with themselves.

## Decision

1. **The reproducibility claim in ADR-0008 is withdrawn.** `e73ed3ed` was a hash from one image,
   not a hash confirmed to repeat. The same correction applies to `win-x64` and `c426ca6a`, which
   ADR-0006 inherited without stating a claim about it.

2. **`native/CHECKSUMS.txt` does not currently gate either Windows binary.** While a runner image
   rollout is in progress each Windows RID has two possible outputs, so a release has roughly a
   coin flip per RID of a checksum failure that is indistinguishable from the substitution the
   manifest exists to catch. Anyone hitting it would regenerate the manifest, which is the habit
   the control exists to prevent.

3. **The cause is recorded, and it is `/GL` and `/LTCG`, not the compiler version.** MSVC's link
   time code generation produces different output on the two images from identical inputs and
   identical component versions. Any future decision about the Windows build starts from that
   rather than from the four options #45 opened with.

4. **`native/build-windows.ps1` takes `-Ipo auto|on|off`.** `auto` is the previous behaviour and
   ships unchanged: `def` exports imply whole program optimisation and `all-symbols` cannot have
   it. The override exists because the question above could not otherwise be asked, and because
   whichever mitigation is chosen will need it. `all-symbols` with `/GL` is refused up front,
   since that combination dies in `cmake -E __create_def` on IL objects. See ADR-0006.

5. **`win-perf-experiment.yml` gains variant `c` and a RID input.** Variant `a` differs from `b`
   in two ways at once, exports and `/GL`, so #18's 7 percent covers the pair and not `/GL` alone.
   Variant `c` holds the export list fixed and moves only `/GL`. The RID input exists because the
   case for turning `/GL` off on arm64 rested on nothing having measured it there, which is an
   absence of evidence rather than a zero. arm64 builds on an x64 host and measures on
   `windows-11-arm`, so conformance moved to the measuring job: a cross compiled arm64 DLL cannot
   be loaded on the machine that produced it.

## Consequences

The mitigation is not decided here. Turning `/GL` off makes both RIDs reproducible and costs 6 to
8 percent on W2 on `win-x64` by ADR-0006's controlled A/B, about 10 percent by the 0.1.0 published
results, and an unknown amount on arm64 until variant `c` runs there. That trade belongs to whoever
owns the release, and this ADR exists so the choice is made against measurements rather than
against the four guesses #45 started with.

Until it is made, a Windows release cut during an image rollout can fail its checksum step for an
innocent reason. The correct response to that failure is to check which image built the binary
before touching `native/CHECKSUMS.txt`.

`docs/benchmarks/0.1.0/` carries the same caveat, because those numbers describe binaries that
came from cache and therefore from one particular image.
