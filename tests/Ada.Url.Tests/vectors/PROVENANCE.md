# Test vector provenance

These files come from [web-platform-tests](https://github.com/web-platform-tests/wpt), under
`url/resources/`, and are the reference corpus for the WHATWG URL Standard.

| Field | Value |
| --- | --- |
| Upstream | `web-platform-tests/wpt` |
| Path | `url/resources/` |
| Pinned commit | `509e6fa4bc34de46802aae59ff103b87caa426a0` |
| Commit date | 2026-07-14 |
| Retrieved | 2026-08-26 |
| Upstream license | See the wpt repository. The corpus is published for exactly this use. |

## Files

| File | Size | Covers |
| --- | --: | --- |
| `urltestdata.json` | 228 KB | The main parse corpus. Input and base pairs with expected components, or an expected failure. |
| `setters_tests.json` | 82 KB | Per setter behaviour, including which assignments are silently ignored. |
| `IdnaTestV2.json` | 314 KB | UTS-46 and IDNA conformance. |
| `toascii.json` | 9 KB | Legacy domain to ASCII cases. |
| `percent-encoding.json` | 1 KB | Per component percent encode sets. |

## Why these are vendored rather than fetched

A test that downloads its own input is a test that fails when the network does, and a corpus
that updates silently turns a regression into a mystery. Pinning means a corpus change arrives
as a deliberate commit that can be reviewed alongside whatever behaviour it changes.

`urltestdata-javascript-only.json` is deliberately excluded. It covers JavaScript specific
behaviour that a .NET wrapper cannot exercise. Recording that here rather than leaving it out
silently, since an unexplained omission looks identical to a coverage gap.

## Keeping this current

The `wpt-drift` job diffs these files against upstream weekly and opens an issue when they move.
Update by re-running the fetch at a new commit, updating this file, and reviewing whatever the
suite then reports.
