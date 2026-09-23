# Contributing

Bug reports, fixes and documentation improvements are welcome. For a new feature or an API change,
open an issue first so the design can be agreed before you write code.

This project follows the [Code of Conduct](CODE_OF_CONDUCT.md). Report security problems as
described in [SECURITY.md](SECURITY.md), not in a public issue.

## Scope

Ada.Url is a thin binding over [Ada](https://github.com/ada-url/ada). URL parsing results come from
Ada. If Ada.Url gives a wrong result and Ada gives the same one, report it
[upstream](https://github.com/ada-url/ada/issues).

## Setup

You need the .NET SDK version in [`global.json`](global.json), CMake, and a C++ toolchain:
Visual Studio 2022 with the C++ workload on Windows, GCC or Clang on Linux, Xcode on macOS.

Build the native library for your platform once. It goes to `artifacts/native/<rid>/`, where the
build and tests find it.

```bash
./native/build-linux.sh --ada-tag v4.0.0 --rid linux-x64
./native/build-macos.sh --ada-tag v4.0.0 --rid osx-arm64
pwsh ./native/build-windows.ps1 -AdaTag v4.0.0 -Rid win-x64
```

The Ada tag must match `AdaUrlUpstreamTag` in [`Directory.Build.props`](Directory.Build.props).

Then:

```bash
dotnet build -c Release
dotnet test -c Release
```

## Pull requests

- Keep a pull request to one change. Small is easier to review.
- Add or update tests for any behaviour change. The WHATWG conformance suite must keep passing.
- Public API changes go in `src/Ada.Url/PublicAPI.Unshipped.txt`. The build fails if you forget.
- Add a line to the `Unreleased` section of [`CHANGELOG.md`](CHANGELOG.md) for anything a user of
  the package would notice.
- A significant design decision gets an ADR in [`docs/adr/`](docs/adr). ADRs are not edited after
  they are accepted; a later ADR supersedes an earlier one.
- CI must pass. It builds and tests on Windows, Linux and macOS, x64 and arm64.

## Commit messages

[Conventional Commits](https://www.conventionalcommits.org/): `type(scope): summary`, in the
imperative, lower case, no trailing period. Types: `feat`, `fix`, `docs`, `chore`, `refactor`,
`test`, `build`, `ci`, `perf`. The body says why, not what.

```
fix(interop): copy the href before calling a setter

Ada returns a borrowed pointer that dangles after any setter.
```

## Code style

The rules in [`.editorconfig`](.editorconfig) are enforced by the build. Public members need XML
documentation that says what the member does, what it returns and what breaks it.
