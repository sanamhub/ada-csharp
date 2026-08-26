#!/usr/bin/env bash
# Export and hardening gates for a Linux or macOS artifact.
#
# The export gate exists because a library can build, link, and export nothing at all. That
# already happened here: -fvisibility=hidden plus LTO and --gc-sections produced a 14 KB
# libada.so with no ada_* symbols in it, because Ada does not annotate its C API for visibility.
# Without this check that artifact would have shipped and failed at the first P/Invoke.
set -euo pipefail

DIR="${1:-}"
[ -n "$DIR" ] || { echo "usage: verify-unix.sh <artifact-dir>" >&2; exit 2; }

LIB=""
for candidate in "$DIR/libada.so" "$DIR/libada.dylib"; do
  [ -f "$candidate" ] && LIB="$candidate" && break
done
[ -n "$LIB" ] || { echo "no libada.so or libada.dylib in $DIR" >&2; exit 1; }

echo "verifying $LIB"

# Any one of these missing means the wrapper cannot function at all.
REQUIRED="ada_parse ada_free ada_get_href ada_free_owned_string"

case "$LIB" in
  *.so)    SYMBOLS="$(nm -gD --defined-only "$LIB" 2>/dev/null || nm -gD "$LIB")" ;;
  *.dylib) SYMBOLS="$(nm -gU "$LIB")" ;;
esac

MISSING=""
for sym in $REQUIRED; do
  # Mach-O prefixes C symbols with an underscore.
  if ! printf '%s\n' "$SYMBOLS" | grep -qE "(^| )_?${sym}$"; then
    MISSING="$MISSING $sym"
  fi
done

if [ -n "$MISSING" ]; then
  echo "FAIL: missing exported symbols:$MISSING" >&2
  echo "If every ada_* symbol is missing, look for -fvisibility=hidden in the build flags." >&2
  echo "Ada does not annotate its C API, so hiding by default hides all of it." >&2
  exit 1
fi

TOTAL="$(printf '%s\n' "$SYMBOLS" | grep -cE '(^| )_?ada_' || true)"
echo "PASS: exports present, $TOTAL ada_* symbols total"

# Hardening. readelf is present on the Ubuntu runners, so no checksec dependency.
if [ "${LIB##*.}" = "so" ]; then
  HEADERS="$(readelf -lW "$LIB")"
  DYNAMIC="$(readelf -dW "$LIB")"
  fail=0

  printf '%s\n' "$HEADERS" | grep -q 'GNU_RELRO' \
    || { echo "FAIL: no GNU_RELRO segment" >&2; fail=1; }
  printf '%s\n' "$DYNAMIC" | grep -qE 'BIND_NOW|FLAGS.*NOW' \
    || { echo "FAIL: not full RELRO, BIND_NOW is absent" >&2; fail=1; }
  # A GNU_STACK marked RWE means an executable stack.
  printf '%s\n' "$HEADERS" | grep 'GNU_STACK' | grep -q 'RWE' \
    && { echo "FAIL: executable stack" >&2; fail=1; }

  [ "$fail" -eq 0 ] || exit 1
  echo "PASS: RELRO, BIND_NOW, non-executable stack"
else
  # Mach-O is PIE by default. Report signing state rather than gating on it, since a local run
  # has no signing identity.
  if codesign -dv "$LIB" >/dev/null 2>&1; then
    echo "PASS: dylib is signed"
  else
    echo "NOTE: dylib is unsigned, expected outside a release build"
  fi
fi
