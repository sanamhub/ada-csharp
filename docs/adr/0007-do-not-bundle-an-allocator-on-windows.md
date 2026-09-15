# ADR-0007: Do not bundle an allocator on Windows

- **Status:** accepted
- **Date:** 2026-09-15
- **Approved by:** Sanam
- **Relates to:** ADR-0003, ADR-0006

## Context

Windows sits level with `System.Uri` where Linux is 1.9x ahead. Two explanations were on the
table. ADR-0006 closed the first: whole program optimisation is worth about 7% on a hard URL for
a pair of changes, not the factor of two this repository claimed for a year without measuring.
The second was the allocator, and issue #20 asked whether to bundle one inside `ada.dll`.

The benchmarks could only reach it by subtraction. `CanParse` costs about the same on both
platforms, a full parse costs 2.2x more on Windows, so the difference is in the step that
allocates. That difference also holds the aggregator filling its `std::string` and computing
eight component offsets, and a subtraction cannot separate them. Deciding on one would have
repeated the mistake ADR-0006 had just finished correcting.

So `native/bench/alloc-probe.cpp` measures it directly. It replaces global `operator new` to
count and size what a parse actually allocates, then times three loops in one process:
`ada_can_parse` as an allocation free control, `ada_parse` with `ada_free`, and a replay loop
that performs exactly the recorded allocations and nothing else. Two runs on windows-2022 against
ubuntu-24.04, ada v4.0.0, median of 9 rounds of 100,000.

`ada_parse` allocates twice on a plain URL: the `ada::result<ada::url_aggregator>` and the
`std::string` inside it. 88 and 31 bytes on Linux, 96 and 32 on Windows. `ada_can_parse`
allocates nothing there, because v4.0.0 has a fast path that never builds an aggregator, so the
control is clean.

| Plain URL | linux-x64 | win-x64 | win / linux |
| --- | ---: | ---: | ---: |
| `can_parse`, control | 40.1 / 40.3 ns | 40.3 / 46.9 ns | |
| `parse` + `free` | 72.4 / 69.0 ns | 153.0 / 161.9 ns | 2.11 / 2.35 |
| the two allocations alone | 22.0 / 21.9 ns | 86.2 / 87.3 ns | **3.92 / 3.99** |

Replaying the two allocations with no parsing in the loop costs about 4x more on Windows, and
that is 76% of everything the Windows parse spends beyond validating, in both runs.

## Decision

1. **No allocator is bundled.** `ada.dll` keeps the default CRT heap.

2. **Handle reuse is documented instead.** `ada_set_href` re-parses into an existing handle, and
   `AdaUrl.TrySetHref` already ships, so a caller with a loop can allocate the result object once
   rather than once per URL. Measured at 118.0 ns against 161.9 ns on Windows and 63.6 against
   69.0 on Linux, with allocations going from two to one.

3. **The allocation count goes upstream** with the export annotation work in #21. `ada_c.h` has
   no way to parse into caller-supplied storage, so every C consumer pays two allocations per URL
   with no opt out. That is upstream's to fix or to decline.

4. **#19 is closed.** clang-cl and MSVC link against the same CRT heap, so a different compiler
   cannot touch the thing that costs 76% of the gap. The A/B is not worth the runner time.

5. **The probe stays.** ADR-0006 kept the `all-symbols` build path so its comparison could be
   rerun when the pinned tag moves, and this is the same argument. The file header said to delete
   it when #20 closed; that was written before it had produced anything worth rechecking.

## Consequences

The Windows number does not improve. A caller who parses in a loop can have 27% of it by keeping
one `AdaUrl` and calling `TrySetHref`, which is a documentation change, not a code one.

Bundling mimalloc or snmalloc would have been the fastest way to close the gap and is rejected
on three grounds. It makes our `ada.dll` differ from upstream's at exactly the point where the
checksum manifest, the SBOM and ADR-0003's pinned tag all exist to say it does not. It hides an
upstream problem instead of reporting it. And it overrides the allocator of a process we do not
own: a consumer who has already chosen one would get ours inside this DLL and never be asked.

The saving from handle reuse is only half of the two allocations, because
`url_aggregator::set_href` parses into a fresh aggregator and copy assigns it, so the temporary
still allocates the buffer. The kept result object is what is saved. This is worth knowing before
anyone reports the remaining half as a second bug.

Handle reuse does nothing for a hard URL, 1,741 ns against 1,771 on Windows, because `set_href`
still allocates four times there and the IDNA work dominates either way. The README says so
rather than quoting the plain URL figure alone.

## What this does not say

The probe times `operator new` and `operator delete` on the default heap on both platforms,
single threaded. It does not explain why the Windows heap is 4x slower for 32 and 96 byte blocks,
and it does not measure a contended heap, which is the case a server actually runs and where the
gap could go either way. Neither question changes this decision, because both land upstream or in
the consumer's process rather than here.

The two runs disagree by 16% on the Windows control, which is runner drift and the reason every
number here is also given as a ratio. Anyone rerunning the probe should compare ratios, not
nanoseconds.

## Alternatives considered

**Bundle mimalloc.** Rejected, see consequences.

**Add an API that parses into caller-supplied storage.** Not available. `ada_c.h` has no such
entry point, and adding one to our side would mean reaching past the C API into ada's C++ types,
which is a fork in everything but name.

**Set the Windows segment heap for the process.** Not ours to set. It is an application manifest
setting and this is a library.

**Leave #20 open and keep looking.** Rejected. The question was which of two causes it is, and
the answer is 76% one of them in two runs. Reproducing it a third time would not change what gets
built.
