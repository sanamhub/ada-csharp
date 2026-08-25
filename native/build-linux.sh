#!/usr/bin/env bash
# Builds Ada as a shared library for a Linux RID.
#
# Baseline is x86-64-v2 with ADA_USE_SIMDUTF=ON, not a static AVX2 build. simdutf picks its
# kernel at runtime, so we keep the SIMD speed without raising the minimum CPU. See ADR-0003.
set -euo pipefail

ADA_TAG=""
RID=""
SANITIZE=""

while [ $# -gt 0 ]; do
  case "$1" in
    --ada-tag)  ADA_TAG="$2"; shift 2 ;;
    --rid)      RID="$2";     shift 2 ;;
    --sanitize) SANITIZE="$2"; shift 2 ;;
    *) echo "unknown argument: $1" >&2; exit 2 ;;
  esac
done

[ -n "$ADA_TAG" ] || { echo "--ada-tag is required" >&2; exit 2; }
[ -n "$RID" ]     || { echo "--rid is required" >&2; exit 2; }

case "$RID" in
  linux-x64|linux-musl-x64) ARCH_FLAGS="-march=x86-64-v2 -mtune=generic" ;;
  linux-arm64)              ARCH_FLAGS="-march=armv8-a+crc+crypto -mtune=neoverse-n1" ;;
  *) echo "unsupported rid for this script: $RID" >&2; exit 2 ;;
esac

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
SRC="$ROOT/native/ada-src"
BUILD="$ROOT/native/build/$RID"
OUT="$ROOT/artifacts/native/$RID"

if [ ! -d "$SRC/.git" ]; then
  git clone --depth 1 --branch "$ADA_TAG" https://github.com/ada-url/ada.git "$SRC"
fi

CXX_FLAGS="-O3 -DNDEBUG $ARCH_FLAGS -flto=thin -fno-plt -fno-semantic-interposition"
CXX_FLAGS="$CXX_FLAGS -fvisibility=hidden -ffunction-sections -fdata-sections"
CXX_FLAGS="$CXX_FLAGS -fstack-protector-strong -fcf-protection=full"
LINK_FLAGS="-flto=thin -Wl,--gc-sections -Wl,-O2 -Wl,--as-needed"
LINK_FLAGS="$LINK_FLAGS -Wl,-z,relro,-z,now -Wl,-z,noexecstack"

# musl's _FORTIFY_SOURCE coverage differs from glibc and warns on some headers.
if [ "$RID" != "linux-musl-x64" ]; then
  CXX_FLAGS="$CXX_FLAGS -D_FORTIFY_SOURCE=2"
fi

# The sanitizer build is for the nightly leak lane, never for a shipped artifact.
LTO="ON"
if [ -n "$SANITIZE" ]; then
  CXX_FLAGS="-O1 -g -fno-omit-frame-pointer -fsanitize=$SANITIZE $ARCH_FLAGS"
  LINK_FLAGS="-fsanitize=$SANITIZE"
  LTO="OFF"
fi

rm -rf "$BUILD"
cmake -S "$SRC" -B "$BUILD" -G Ninja \
  -DCMAKE_BUILD_TYPE=Release \
  -DBUILD_SHARED_LIBS=ON \
  -DADA_TESTING=OFF -DADA_BENCHMARKS=OFF -DADA_TOOLS=OFF \
  -DADA_USE_SIMDUTF=ON \
  -DCMAKE_INTERPROCEDURAL_OPTIMIZATION="$LTO" \
  -DCMAKE_CXX_STANDARD=20 -DCMAKE_CXX_STANDARD_REQUIRED=ON \
  -DCMAKE_C_COMPILER=clang -DCMAKE_CXX_COMPILER=clang++ \
  -DCMAKE_CXX_FLAGS_RELEASE="$CXX_FLAGS" \
  -DCMAKE_SHARED_LINKER_FLAGS_RELEASE="$LINK_FLAGS"

cmake --build "$BUILD" --parallel

# NuGet does not preserve symlinks, so resolve libada.so -> libada.so.4.0.0 and copy the real
# file under the plain name. The managed side asks the loader for "ada", which becomes libada.so.
mkdir -p "$OUT"
REAL="$(readlink -f "$BUILD/libada.so")"
[ -f "$REAL" ] || { echo "build produced no libada.so" >&2; exit 1; }
cp "$REAL" "$OUT/libada.so"

if [ -z "$SANITIZE" ]; then
  cp "$OUT/libada.so" "$OUT/libada.so.unstripped"
  llvm-strip --strip-unneeded "$OUT/libada.so" 2>/dev/null || strip --strip-unneeded "$OUT/libada.so"
fi

echo "built $RID from $ADA_TAG:"
ls -l "$OUT"
