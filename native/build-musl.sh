#!/usr/bin/env bash
# Builds a musl native inside Alpine, for linux-musl-x64 or linux-musl-arm64.
#
# These RIDs are not optional. A glibc .so will not load on Alpine, and containers are a first
# class target for this package. arm64 containers are no longer unusual, so the same argument
# that bought linux-musl-x64 buys this one.
set -euo pipefail

ADA_TAG=""
RID="linux-musl-x64"

while [ $# -gt 0 ]; do
  case "$1" in
    --ada-tag) ADA_TAG="$2"; shift 2 ;;
    --rid)     RID="$2";     shift 2 ;;
    *) echo "unknown argument: $1" >&2; exit 2 ;;
  esac
done

[ -n "$ADA_TAG" ] || { echo "--ada-tag is required" >&2; exit 2; }

ROOT="$(cd "$(dirname "$0")/.." && pwd)"

# Pinned by digest, not by tag, so the toolchain cannot change under us between runs.
#
# This is the digest of the 3.20.0 multi-architecture index, not of one image inside it. The
# previous pin was the amd64 image directly, which is a single platform manifest: on an arm64
# host docker would refuse it, or silently run it under emulation and produce an x86-64 .so
# sitting in the linux-musl-arm64 directory. The index resolves to the right image per host and
# both come from the same Alpine release, so the two musl RIDs cannot drift apart by a patch
# level either.
#
# The amd64 image inside this index is sha256:216266c8, which is exactly what the old pin named,
# so linux-musl-x64 is byte for byte unchanged by this.
ALPINE="alpine:3.20@sha256:77726ef6b57ddf65bb551896826ec38bc3e53f75cdde31354fbffb4f25238ebd"

# The container runs as root, so everything it writes into the bind mount is root owned and the
# runner user cannot delete it afterwards. That is not theoretical: the reproducibility check
# removes native/build, native/ada-src and artifacts/native between its two builds, and it failed
# with "Permission denied" on every file the first build had produced.
#
# apk needs root, so the fix is to hand ownership back on the way out rather than to drop
# privileges on the way in. $(id -u) expands on the host, which is the whole point.
docker run --rm -v "$ROOT:/w" -w /w "$ALPINE" sh -c "
  set -eu
  # bash is not in the Alpine base image and build-linux.sh needs it.
  apk add --no-cache bash build-base clang lld binutils cmake ninja git python3
  ./native/build-linux.sh --ada-tag '$ADA_TAG' --rid '$RID'
  chown -R $(id -u):$(id -g) /w/native /w/artifacts
"
