#!/usr/bin/env bash
# Export and hardening gates for a Linux or macOS artifact.
#
# The export gate exists because a library can build, link, and export nothing at all. That
# already happened here: -fvisibility=hidden plus LTO and --gc-sections produced a 14 KB
# libada.so with no ada_* symbols in it, because Ada does not annotate its C API for visibility.
# Without this check that artifact would have shipped and failed at the first P/Invoke.
set -euo pipefail

DIR="${1:-}"
[ -n "$DIR" ] || { echo "usage: verify-unix.sh <artifact-dir> [rid]" >&2; exit 2; }

# Which architecture the artifact is supposed to be. Defaults to the last segment of the
# artifact directory, which is the RID at every call site, so no caller has to repeat itself and
# none of them can forget to ask.
RID="${2:-$(basename "$DIR")}"

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

# Here strings, not pipes. grep -q exits at the first match, which closes the pipe, which kills
# printf with SIGPIPE, which pipefail turns into a failed pipeline. That made this report a
# symbol that was present as missing, intermittently, depending on where in the output the match
# happened to land. It passed for days and then failed on osx-arm64.
MISSING=""
for sym in $REQUIRED; do
  # Mach-O prefixes C symbols with an underscore.
  if ! grep -qE "(^| )_?${sym}$" <<< "$SYMBOLS"; then
    MISSING="$MISSING $sym"
  fi
done

if [ -n "$MISSING" ]; then
  echo "FAIL: missing exported symbols:$MISSING" >&2
  echo "If every ada_* symbol is missing, look for -fvisibility=hidden in the build flags." >&2
  echo "Ada does not annotate its C API, so hiding by default hides all of it." >&2
  exit 1
fi

TOTAL="$(grep -cE '(^| )_?ada_' <<< "$SYMBOLS" || true)"
echo "PASS: exports present, $TOTAL ada_* symbols total"

# Architecture. The musl RIDs are built inside a container and the arm64 legs are cross cutting
# enough that "the runner was arm64, so the output is arm64" is an assumption rather than a fact:
# a docker image pinned to a single platform digest, or a QEMU binfmt handler quietly doing its
# job, both produce a working .so for the wrong architecture. It would pass every other check
# here and then fail on exactly the machines the RID exists for. macOS is covered by lipo in
# native.yml, so this is the ELF half.
if [ "${LIB##*.}" = "so" ]; then
  case "$RID" in
    linux-x64|linux-musl-x64)     WANT="Advanced Micro Devices X86-64" ;;
    linux-arm64|linux-musl-arm64) WANT="AArch64" ;;
    *) echo "FAIL: no expected machine type for rid '$RID'" >&2; exit 1 ;;
  esac

  GOT="$(readelf -hW "$LIB" | sed -n 's/^  Machine:  *//p')"
  if [ "$GOT" != "$WANT" ]; then
    echo "FAIL: $RID expects machine '$WANT', this binary is '$GOT'" >&2
    exit 1
  fi
  echo "PASS: machine is '$GOT', which matches $RID"
fi

# Hardening. readelf is present on the Ubuntu runners, so no checksec dependency.
if [ "${LIB##*.}" = "so" ]; then
  HEADERS="$(readelf -lW "$LIB")"
  DYNAMIC="$(readelf -dW "$LIB")"
  fail=0

  grep -q 'GNU_RELRO' <<< "$HEADERS" \
    || { echo "FAIL: no GNU_RELRO segment" >&2; fail=1; }
  grep -qE 'BIND_NOW|FLAGS.*NOW' <<< "$DYNAMIC" \
    || { echo "FAIL: not full RELRO, BIND_NOW is absent" >&2; fail=1; }
  # A GNU_STACK marked RWE means an executable stack.
  STACK_LINE="$(grep 'GNU_STACK' <<< "$HEADERS" || true)"
  grep -q 'RWE' <<< "$STACK_LINE" \
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
