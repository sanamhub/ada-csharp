#!/usr/bin/env bash
# Emits "<sha256>  <rid>/<file>" for every shipped binary in an artifact directory.
#
# The manifest is committed. The pack step verifies against it and fails on mismatch, so a
# swapped native binary breaks the build instead of shipping.
set -euo pipefail

DIR="${1:-}"
[ -n "$DIR" ] || { echo "usage: checksum.sh <artifact-dir>" >&2; exit 2; }

RID="$(basename "$DIR")"

# .unstripped and .pdb are debugging artifacts and are not packaged, so they stay out.
find "$DIR" -maxdepth 1 -type f \
     ! -name '*.unstripped' ! -name '*.pdb' \
     -print0 \
  | sort -z \
  | while IFS= read -r -d '' file; do
      # macOS ships shasum rather than sha256sum.
      if command -v sha256sum >/dev/null 2>&1; then
        sum="$(sha256sum "$file" | cut -d' ' -f1)"
      else
        sum="$(shasum -a 256 "$file" | cut -d' ' -f1)"
      fi
      echo "$sum  $RID/$(basename "$file")"
    done
