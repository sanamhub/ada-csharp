#!/usr/bin/env bash
# Builds Ada as a shared library for a macOS RID.
#
# Two single arch builds and two RIDs, not one lipo universal binary. The RID graph already
# picks the right asset, and a universal binary doubles the size of every deployment. See
# ADR-0003. The lipo path stays below for single file bundle consumers who need it.
set -euo pipefail

ADA_TAG=""
RID=""

while [ $# -gt 0 ]; do
  case "$1" in
    --ada-tag) ADA_TAG="$2"; shift 2 ;;
    --rid)     RID="$2";     shift 2 ;;
    *) echo "unknown argument: $1" >&2; exit 2 ;;
  esac
done

[ -n "$ADA_TAG" ] || { echo "--ada-tag is required" >&2; exit 2; }

case "$RID" in
  # -mcpu=apple-m1 is safe here only because osx-arm64 implies Apple Silicon. Do not copy this
  # to linux-arm64, where the hardware is unknown.
  osx-arm64) OSX_ARCH="arm64";  ARCH_FLAGS="-mcpu=apple-m1";   MIN="11.0"  ;;
  osx-x64)   OSX_ARCH="x86_64"; ARCH_FLAGS="-march=x86-64-v2"; MIN="10.15" ;;
  *) echo "unsupported rid for this script: $RID" >&2; exit 2 ;;
esac

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
SRC="$ROOT/native/ada-src"
BUILD="$ROOT/native/build/$RID"
OUT="$ROOT/artifacts/native/$RID"

if [ ! -d "$SRC/.git" ]; then
  git clone --depth 1 --branch "$ADA_TAG" https://github.com/ada-url/ada.git "$SRC"
fi

rm -rf "$BUILD"
cmake -S "$SRC" -B "$BUILD" -G Ninja \
  -DCMAKE_BUILD_TYPE=Release \
  -DBUILD_SHARED_LIBS=ON \
  -DADA_TESTING=OFF -DADA_BENCHMARKS=OFF -DADA_TOOLS=OFF \
  -DADA_USE_SIMDUTF=OFF \
  -DCMAKE_INTERPROCEDURAL_OPTIMIZATION=ON \
  -DCMAKE_CXX_STANDARD=20 -DCMAKE_CXX_STANDARD_REQUIRED=ON \
  -DCMAKE_OSX_ARCHITECTURES="$OSX_ARCH" \
  -DCMAKE_OSX_DEPLOYMENT_TARGET="$MIN" \
  -DCMAKE_CXX_FLAGS_RELEASE="-O3 -DNDEBUG -flto=thin -fvisibility=hidden -fstack-protector-strong $ARCH_FLAGS" \
  -DCMAKE_SHARED_LINKER_FLAGS_RELEASE="-flto=thin -Wl,-dead_strip"

cmake --build "$BUILD" --parallel

# CMake writes the target under src/, and names it libada.dylib or libada.4.dylib depending on
# how it handles SOVERSION.
mkdir -p "$OUT"
BUILT="$(find "$BUILD" -name 'libada*.dylib' | head -1)"
if [ -z "$BUILT" ]; then
  echo "build produced no dylib. What it did produce:" >&2
  find "$BUILD" -name '*.dylib' >&2
  exit 1
fi
REAL="$(python3 -c "import os,sys; print(os.path.realpath(sys.argv[1]))" "$BUILT")"
cp "$REAL" "$OUT/libada.dylib"

# Unsigned dylibs are increasingly blocked on recent macOS. The identity comes from the CI
# secret store, and is absent on a local run, which is fine.
if [ -n "${MACOS_SIGNING_IDENTITY:-}" ]; then
  codesign --force --timestamp --sign "$MACOS_SIGNING_IDENTITY" "$OUT/libada.dylib"
else
  echo "MACOS_SIGNING_IDENTITY not set, skipping codesign"
fi

echo "built $RID from $ADA_TAG:"
ls -l "$OUT"
