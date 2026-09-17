# ADR-0009: Ship linux-musl-arm64

- **Status:** accepted
- **Date:** 2026-09-17
- **Approved by:** Sanam
- **Amends:** ADR-0003, point 3, the shipped RID list
- **Follows:** ADR-0008

## Context

ADR-0003 shipped `linux-musl-x64` because a glibc `.so` does not load on Alpine and containers
are a first class target for this package. It did not ship the arm64 equivalent.

The argument has not changed, only the hardware. Graviton and Ampere containers are ordinary now,
and an Alpine arm64 consumer today gets a `DllNotFoundException` for a reason that has nothing to
do with their code and that they cannot fix.

This is the cheapest RID left. `native/build-musl.sh` already exists, `native/build-linux.sh`
already knows how to target arm64, and `ubuntu-24.04-arm` is generally available and free for
public repositories.

## Decision

1. **`linux-musl-arm64` ships**, built by the existing `build-musl.sh` on `ubuntu-24.04-arm`,
   with `build-linux.sh` taking the same arm64 flags it already uses for `linux-arm64`:
   `-march=armv8-a+crc+crypto -mtune=neoverse-n1` and `-mbranch-protection=standard`.

2. **The Alpine pin moves from an image digest to an index digest.** It was
   `alpine:3.20@sha256:216266c8`, which is the amd64 image, a single platform manifest. On an
   arm64 host docker either refuses it or, with a binfmt handler installed, runs it under
   emulation and produces an x86-64 `.so` sitting in the `linux-musl-arm64` directory. The pin is
   now `sha256:77726ef6`, the 3.20.0 multi-architecture index, which resolves to the right image
   per host.

   This is still a digest pin, so the toolchain still cannot change under us, and both musl RIDs
   now come from one Alpine release rather than two independently chosen ones.

   The amd64 image inside that index is `sha256:216266c8`, exactly what the old pin named, so
   `linux-musl-x64` is byte for byte unchanged.

3. **The ELF machine type of every Linux artifact is checked, not trusted.**
   `native/verify-unix.sh` reads `readelf -h` and fails unless the machine matches the RID. It
   takes the RID from the last segment of the artifact directory, so every existing call site
   gets the check and none can forget to ask for it.

   This is not only about the pin above. Any container built for one platform, and any QEMU
   binfmt handler quietly doing its job, produces a working library for the wrong architecture
   that passes the export gate, the hardening gate and the checksum, and then fails on exactly
   the machines the RID exists for. ADR-0008 added the PE half of this for `win-arm64`.

## Consequences

Eight native libraries ship. The package grows by roughly the size of one `libada.so`.

`linux-musl-x64` keeps its checksum, because the resolved amd64 image is the same one. The
manifest gains one line for the new RID.

The consumption test runs in an Alpine container on an arm64 runner, so this RID is proved by
loading the library on the platform it targets rather than by the fact that it compiled. That is
the same standard `linux-musl-x64` is held to, and it is the check that would have caught the
single platform pin.

`native/verify-unix.sh` now takes an optional second argument. Every caller passes the RID
explicitly in `native.yml`; the default from the directory name exists so a local run does the
right thing without being told.

## Alternatives considered

**Pin a second, separate arm64 image digest.** Rejected. Two pins drift: nothing makes the next
person bump both, and two musl RIDs built from different Alpine patch levels is a difference
nobody would notice until it mattered. One index digest says the same thing once.

**Keep the old amd64 pin and add the index only for arm64.** Same objection, with the extra
problem that the reason for the asymmetry would live only in a comment.

**Build musl arm64 by cross compiling from x64.** Rejected. The runner exists and is free, and
cross compiling would need a musl sysroot for aarch64 to be assembled and kept current, which is
more moving parts than the thing it replaces.

**Keep deferring it.** Rejected. The cost is one matrix entry on scripts that already handle both
halves, and the current behaviour for an Alpine arm64 consumer is a crash at the first call.
