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

import json
import pathlib
import re
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


def load_reports(root: pathlib.Path) -> dict[str, list[dict]]:
    """Read every BenchmarkDotNet JSON report, keyed by RID."""
    reports: dict[str, list[dict]] = {}

    for rid in PLATFORMS:
        entries: list[dict] = []
        for path in sorted(root.rglob(f"benchmark-{rid}/**/*.json")):
            if "report" not in path.name.lower():
                continue
            try:
                data = json.loads(path.read_text(encoding="utf-8"))
            except (json.JSONDecodeError, OSError):
                continue
            entries.extend(data.get("Benchmarks", []))
        if entries:
            reports[rid] = entries

    return reports


def method_key(bench: dict) -> str:
    """A stable name for a benchmark across platforms, parameters included."""
    name = bench.get("Method", "?")
    params = bench.get("Parameters", "")
    return f"{name} [{params}]" if params else name


def ratio_and_bytes(bench: dict, baselines: dict[str, float]) -> tuple[str, str]:
    stats = bench.get("Statistics") or {}
    mean = stats.get("Mean")
    allocated = (bench.get("Memory") or {}).get("BytesAllocatedPerOperation")

    group = bench.get("Categories") or []
    baseline = baselines.get(group[0] if group else "", None)

    if mean is None:
        ratio = "n/a"
    elif baseline:
        ratio = f"{mean / baseline:.2f}x"
    else:
        ratio = "baseline" if bench.get("IsBaseline") else "n/a"

    if allocated is None:
        alloc = "n/a"
    elif allocated == 0:
        alloc = "**0 B**"
    elif allocated < 1024:
        alloc = f"{int(allocated)} B"
    else:
        alloc = f"{allocated / 1024:.1f} KB"

    return ratio, alloc


def baselines_for(entries: list[dict]) -> dict[str, float]:
    """Mean of the baseline benchmark in each category."""
    out: dict[str, float] = {}
    for b in entries:
        if not b.get("IsBaseline"):
            continue
        cats = b.get("Categories") or [""]
        mean = (b.get("Statistics") or {}).get("Mean")
        if mean:
            out[cats[0]] = mean
    return out


def write_summary(reports: dict[str, dict], out_dir: pathlib.Path) -> None:
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

    # Group by category so W1, W2 and W3 read as separate questions rather than one long list.
    by_category: dict[str, list[str]] = {}
    for rid in present:
        for bench in reports[rid]["entries"]:
            cats = bench.get("Categories") or ["other"]
            by_category.setdefault(cats[0], [])
            key = method_key(bench)
            if key not in by_category[cats[0]]:
                by_category[cats[0]].append(key)

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
                entries = reports[rid]["entries"]
                baselines = reports[rid]["baselines"]
                match = next((b for b in entries if method_key(b) == key), None)
                if match is None:
                    cells.extend(["n/a", "n/a"])
                else:
                    ratio, alloc = ratio_and_bytes(match, baselines)
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
    lines.append("`baseline` marks the `System.Uri` row each group is measured against.")
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

    raw = load_reports(root)
    reports = {rid: {"entries": e, "baselines": baselines_for(e)} for rid, e in raw.items()}

    # Detail pages, copied through as produced.
    for rid in PLATFORMS:
        src = next(root.rglob(f"benchmark-{rid}/**/{rid}.md"), None)
        if src is not None:
            shutil.copyfile(src, out_dir / f"{rid}.md")

    write_summary(reports, out_dir)

    found = ", ".join(sorted(reports)) or "none"
    print(f"collated platforms: {found}")
    for rid, data in sorted(reports.items()):
        print(f"  {rid}: {len(data['entries'])} benchmarks")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
