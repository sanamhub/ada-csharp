#!/usr/bin/env python3
"""Turn BenchmarkDotNet output from several platforms into one summary plus detail pages.

Raw BenchmarkDotNet tables are hard to read. They carry twelve columns, most of which only
matter when a number looks wrong, and there is one table per class per platform. Someone
deciding whether to adopt this library wants two facts: how it compares to System.Uri, and
whether it allocates.

So this writes two shapes.

  README.md      the summary. One row per benchmark, ratio and allocation, all platforms side
                 by side. Ratios are comparable across platforms because each is measured
                 against System.Uri on that same machine.

  <rid>.md       the detail. Full BenchmarkDotNet output, every column, per platform. This is
                 where you go when a summary number looks implausible.

Absolute nanoseconds are deliberately kept out of the summary. The runners are different
hardware, so putting 47 ns next to 61 ns invites a comparison that is not valid.
"""

from __future__ import annotations

import pathlib
import shutil
import sys

# Order matters: this is the order platforms appear in the summary columns.
PLATFORMS = ["linux-x64", "linux-arm64", "win-x64", "osx-arm64"]

PLATFORM_LABEL = {
    "linux-x64": "Linux x64",
    "linux-arm64": "Linux arm64",
    "win-x64": "Windows x64",
    "osx-arm64": "macOS arm64",
}

# Columns that describe the run rather than a benchmark parameter.
FIXED_COLUMNS = {
    "Type", "Method", "Categories", "Job", "Mean", "Error", "StdDev", "StdErr", "Median",
    "Min", "Max", "Op/s", "Ratio", "RatioSD", "MedianRatio", "Gen0", "Gen1", "Gen2",
    "Allocated", "Alloc Ratio", "Baseline", "Rank",
}


def parse_markdown(path: pathlib.Path) -> list[dict]:
    """Read the benchmark rows out of BenchmarkDotNet's markdown export.

    The JSON export is the obvious source and it is the wrong one. It carries no Categories
    field and no baseline marker, so grouping fell back to a single bucket called "other" and
    every ratio printed as "n/a". The markdown has both, plus the ratio BenchmarkDotNet already
    computed against the right baseline, which is better than recomputing it here.

    The rows have no leading pipe, so a parser that splits on a leading "|" sees nothing.
    """
    rows: list[dict] = []
    header: list[str] | None = None

    for line in path.read_text(encoding="utf-8").splitlines():
        if "|" not in line:
            continue

        cells = [c.strip() for c in line.split("|")]
        if cells and cells[-1] == "":
            cells.pop()

        # The dashes under the header, and the blank spacer rows between groups.
        if all(set(c) <= set("-: ") for c in cells if c):
            continue
        if not any(cells):
            continue

        if cells[0] == "Type" or (header is None and "Method" in cells):
            header = cells
            continue

        if header is None or len(cells) != len(header):
            continue

        row = dict(zip(header, cells, strict=False))
        if not row.get("Method"):
            continue
        rows.append(row)

    return rows


def load_reports(root: pathlib.Path) -> dict[str, list[dict]]:
    """Read every platform's markdown report, keyed by RID."""
    reports: dict[str, list[dict]] = {}

    for rid in PLATFORMS:
        src = next(root.rglob(f"benchmark-{rid}/**/{rid}.md"), None)
        if src is None:
            continue
        rows = parse_markdown(src)
        if rows:
            reports[rid] = rows

    return reports


def method_key(row: dict, header_params: list[str]) -> str:
    """A stable name for a benchmark across platforms, parameters included."""
    name = row.get("Method", "?")
    parts = [f"{p}={row[p]}" for p in header_params if row.get(p) not in (None, "", "?")]
    return f"{name} [{', '.join(parts)}]" if parts else name


def param_columns(rows: list[dict]) -> list[str]:
    """Whatever columns are neither fixed nor empty, in the order they appear."""
    seen: list[str] = []
    for row in rows:
        for key in row:
            if key not in FIXED_COLUMNS and key not in seen and key:
                seen.append(key)
    return seen


def ratio_and_bytes(row: dict) -> tuple[str, str]:
    raw_ratio = (row.get("Ratio") or "").strip()
    if raw_ratio in ("", "?", "NA"):
        ratio = "n/a"
    elif raw_ratio in ("1.00", "1.0", "1"):
        # BenchmarkDotNet prints the baseline as exactly 1.00.
        ratio = "baseline"
    else:
        ratio = f"{raw_ratio}x"

    raw_alloc = (row.get("Allocated") or "").strip()
    if raw_alloc in ("", "?", "NA"):
        alloc = "n/a"
    elif raw_alloc == "-":
        alloc = "**0 B**"
    else:
        alloc = raw_alloc

    return ratio, alloc


def write_summary(reports: dict[str, list[dict]], out_dir: pathlib.Path) -> None:
    present = [rid for rid in PLATFORMS if rid in reports]

    lines: list[str] = []
    lines.append("# Benchmark summary")
    lines.append("")
    lines.append("Every figure compares against `System.Uri` measured in the same process on the")
    lines.append("same machine, so the ratios are meaningful on every platform and comparable")
    lines.append("between them. Lower is faster.")
    lines.append("")
    lines.append("Absolute nanoseconds are deliberately not here. The runners are different")
    lines.append("hardware, so putting one platform's timing beside another's invites a comparison")
    lines.append("that is not valid. Per platform detail, with every column, is linked at the end.")
    lines.append("")

    if not present:
        lines.append("No results were produced.")
        (out_dir / "README.md").write_text("\n".join(lines) + "\n", encoding="utf-8")
        return

    params = param_columns([r for rid in present for r in reports[rid]])

    # Group by category so W1, W2, W3 and W4 read as separate questions rather than one list.
    by_category: dict[str, list[str]] = {}
    for rid in present:
        for row in reports[rid]:
            category = row.get("Categories") or row.get("Type") or "other"
            key = method_key(row, params)
            bucket = by_category.setdefault(category, [])
            if key not in bucket:
                bucket.append(key)

    index = {
        rid: {method_key(row, params): row for row in reports[rid]}
        for rid in present
    }

    for category in sorted(by_category):
        lines.append(f"## {category}")
        lines.append("")
        header = "| Benchmark | " + " | ".join(
            f"{PLATFORM_LABEL[r]} ratio | {PLATFORM_LABEL[r]} alloc" for r in present
        ) + " |"
        divider = "| --- | " + " | ".join(["---:", "---:"] * len(present)) + " |"
        lines.append(header)
        lines.append(divider)

        for key in by_category[category]:
            cells: list[str] = []
            for rid in present:
                row = index[rid].get(key)
                if row is None:
                    cells.extend(["n/a", "n/a"])
                else:
                    ratio, alloc = ratio_and_bytes(row)
                    cells.extend([ratio, alloc])
            lines.append(f"| `{key}` | " + " | ".join(cells) + " |")

        lines.append("")

    lines.append("## Detail")
    lines.append("")
    lines.append("Full BenchmarkDotNet output, every column, one file per platform.")
    lines.append("")
    for rid in present:
        lines.append(f"- [{PLATFORM_LABEL[rid]}]({rid}.md)")
    lines.append("")
    lines.append("## Reading these")
    lines.append("")
    lines.append("A ratio of `0.50x` means half the time of `System.Uri`, so twice as fast.")
    lines.append("`baseline` marks the row each group is measured against.")
    lines.append("")
    lines.append("Allocation is the column that usually matters more. A parser that allocates")
    lines.append("nothing does not add GC pressure no matter how many URLs go through it.")

    (out_dir / "README.md").write_text("\n".join(lines) + "\n", encoding="utf-8")


def main() -> int:
    if len(sys.argv) != 3:
        print("usage: collate-benchmarks.py <results-dir> <output-dir>", file=sys.stderr)
        return 2

    root = pathlib.Path(sys.argv[1])
    out_dir = pathlib.Path(sys.argv[2])
    out_dir.mkdir(parents=True, exist_ok=True)

    reports = load_reports(root)

    # Detail pages, copied through as produced.
    for rid in PLATFORMS:
        src = next(root.rglob(f"benchmark-{rid}/**/{rid}.md"), None)
        if src is not None:
            shutil.copyfile(src, out_dir / f"{rid}.md")

    write_summary(reports, out_dir)

    if not reports:
        print("no benchmark results were parsed", file=sys.stderr)
        return 1

    # A summary where nothing has a ratio is the failure this script shipped with for its first
    # release, and it looks like a successful run. Fail loudly instead.
    params = param_columns([r for rows in reports.values() for r in rows])
    rated = sum(
        1
        for rows in reports.values()
        for row in rows
        if ratio_and_bytes(row)[0] != "n/a"
    )
    total = sum(len(rows) for rows in reports.values())

    print(f"collated platforms: {', '.join(sorted(reports))}")
    for rid, rows in sorted(reports.items()):
        print(f"  {rid}: {len(rows)} benchmarks")
    print(f"  rows with a ratio: {rated} of {total}")

    if rated == 0:
        print("::error::every ratio came out n/a, so the summary carries no comparison", file=sys.stderr)
        return 1

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
