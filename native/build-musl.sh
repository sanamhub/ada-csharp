#!/usr/bin/env bash
# Builds the linux-musl-x64 native inside Alpine.
#
# This RID is not optional. A glibc .so will not load on Alpine, and containers are a first
# class target for this package.
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
ALPINE="alpine:3.20@sha256:216266c86fc4dcef5619930bd394245824c2af52fd21ba7c6fa0e618657d4c3b"

docker run --rm -v "$ROOT:/w" -w /w "$ALPINE" sh -c "
  set -eu
  # bash is not in the Alpine base image and build-linux.sh needs it.
  apk add --no-cache bash build-base clang lld binutils cmake ninja git
  ./native/build-linux.sh --ada-tag '$ADA_TAG' --rid '$RID'
"
