# Ada.Url

A .NET wrapper around [Ada](https://github.com/ada-url/ada), the WHATWG compliant URL parser
that Node.js uses.

> **Pre-alpha scaffold.** There is no working API yet. The engineering plan is done and the
> repository is set up. Implementation follows the roadmap in
> [`ADA_WRAPPER_PLAN.md`](ADA_WRAPPER_PLAN.md), section 7.4. Do not depend on this package.

## Why

`System.Uri` implements RFC 3986 and 3987 plus a decade of .NET specific behaviour. It is not
WHATWG compliant, so it disagrees with browsers, and with the Node, Go, and Python URL parsers,
on a long list of real inputs. It also allocates on almost every operation.

This wrapper aims to expose Ada to .NET with no measurable overhead. On the UTF-8 span in,
span out path the target is zero bytes allocated and zero GC collections, checked by a test in
CI rather than by a benchmark.

## Design

Ada's C ABI is byte oriented UTF-8, so the API is UTF-8 first. `ReadOnlySpan<byte>` is the
primary shape, and `string` overloads carry a documented transcode cost.

Interop signatures use only blittable types (`byte*`, `nuint`, `nint`) and pin with `fixed`,
which skips string marshalling entirely.

Lifetime comes in three sizes. Handle free statics for the common validate and normalise case.
A stack bound `ref struct` for work that touches several properties. A `SafeHandle` for when a
URL has to live in a field.

## Limits

**Zero allocation means span in, span out.** Any `System.String` result allocates by
construction. Benchmarks are published in tiers and the zero byte figure only ever refers to the
span tier.

**A borrowed span is invalidated by any setter.** This is documented and tested, not enforced.
Enforcing it would cost the thing this library exists to provide. See
[ADR-0004](docs/adr/0004-unmanaged-lifetime-model.md).

**`net10.0` only.** No .NET Framework, Mono, or Unity. See
[ADR-0001](docs/adr/0001-single-target-net10-and-utf8-first-api.md).

**Handles are not thread safe.** One handle per thread, or synchronise externally.

## Security

A URL parser is usually used to decide whether a URL is safe to fetch. Two rules matter:

1. Compare the parsed `hostname`, never the raw input string. Prefix checks against raw input
   are the classic SSRF bypass.
2. Match allow lists against post IDNA ASCII. Visually confusable Unicode domains normalise to
   different ASCII, so comparing at the Unicode level can be fooled.

This library never logs a URL, and neither should you at any level you would ship. URLs
routinely carry credentials in `username` and `password`.

## Build

```bash
dotnet build -c Release
dotnet test  -c Release
```

The native library is not part of the build yet. It arrives in P1 of the plan. Once it does,
natives are built only in CI from the pinned upstream tag and are never committed.

## Documentation

| File | Contents |
| --- | --- |
| [`ADA_WRAPPER_PLAN.md`](ADA_WRAPPER_PLAN.md) | Framework targeting, native build, P/Invoke architecture, test strategy, benchmarks, CI |
| [`docs/adr/`](docs/adr) | Architecture decision records |
| [`CHANGELOG.md`](CHANGELOG.md) | Release history |

## License

MIT, see [`LICENSE`](LICENSE). Ada is dual licensed Apache-2.0 or MIT and is redistributed here
under the MIT option. See [`THIRD-PARTY-NOTICES.txt`](THIRD-PARTY-NOTICES.txt).

This is an unofficial wrapper and is not affiliated with the Ada project.
