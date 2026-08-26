#!/usr/bin/env bash
# Builds Ada as a shared library for a Linux RID.
#
# Baseline is x86-64-v2, not a static AVX2 build, so the artifact runs on any CPU from 2009
# onward instead of raising the floor to Haswell.
#
# ADA_USE_SIMDUTF is OFF. With BUILD_SHARED_LIBS=ON it propagates to simdutf and builds it as a
# second shared library, which breaks the Windows build and would add a runtime dependency to
# ship. Ada keeps its own SIMD paths either way. See ADR-0003.
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
# No -fvisibility=hidden. Ada does not annotate its C API for visibility, so hiding by default
# hides every ada_* symbol, and LTO plus --gc-sections then strips the library down to nothing.
# The first attempt produced a 14 KB libada.so that exported not one symbol. Do not add it back
# without also patching upstream, which is not our call to make.
CXX_FLAGS="$CXX_FLAGS -ffunction-sections -fdata-sections"
CXX_FLAGS="$CXX_FLAGS -fstack-protector-strong -fcf-protection=full"
LINK_FLAGS="-flto=thin -Wl,--gc-sections -Wl,-O2 -Wl,--as-needed"
LINK_FLAGS="$LINK_FLAGS -Wl,-z,relro,-z,now -Wl,-z,noexecstack"

if [ "$RID" = "linux-musl-x64" ]; then
  # musl's _FORTIFY_SOURCE coverage differs from glibc and warns on some headers. Also, clang
  # on Alpine defaults to GNU ld, which needs the gold plugin for ThinLTO. lld handles it.
  CXX_FLAGS="$CXX_FLAGS -fuse-ld=lld"
  LINK_FLAGS="$LINK_FLAGS -fuse-ld=lld"
else
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
  -DADA_USE_SIMDUTF=OFF \
  -DCMAKE_INTERPROCEDURAL_OPTIMIZATION="$LTO" \
  -DCMAKE_CXX_STANDARD=20 -DCMAKE_CXX_STANDARD_REQUIRED=ON \
  -DCMAKE_C_COMPILER=clang -DCMAKE_CXX_COMPILER=clang++ \
  -DCMAKE_CXX_FLAGS_RELEASE="$CXX_FLAGS" \
  -DCMAKE_SHARED_LINKER_FLAGS_RELEASE="$LINK_FLAGS"

cmake --build "$BUILD" --parallel

# CMake writes the target under src/, not the build root.
#
# NuGet does not preserve symlinks either, so resolve libada.so -> libada.so.4.0.0 and copy the
# real file under the plain name. The managed side asks the loader for "ada", which the platform
# turns into libada.so.
mkdir -p "$OUT"
# head closes the pipe early, which kills find with SIGPIPE and trips pipefail. Turn it off
# for exactly this line rather than losing the protection everywhere else.
set +o pipefail
BUILT="$(find "$BUILD" -name 'libada.so*' -not -name '*.unstripped' | head -1)"
set -o pipefail
if [ -z "$BUILT" ]; then
  echo "build produced no libada.so. What it did produce:" >&2
  find "$BUILD" -name '*.so*' >&2
  exit 1
fi
REAL="$(readlink -f "$BUILT")"
cp "$REAL" "$OUT/libada.so"

if [ -z "$SANITIZE" ]; then
  cp "$OUT/libada.so" "$OUT/libada.so.unstripped"
  llvm-strip --strip-unneeded "$OUT/libada.so" 2>/dev/null || strip --strip-unneeded "$OUT/libada.so"
fi

echo "built $RID from $ADA_TAG:"
ls -l "$OUT"
