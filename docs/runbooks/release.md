# Release runbook

## Before tagging

1. `CHANGELOG.md` has an entry for the version, moved out of Unreleased.
2. `native/CHECKSUMS.txt` matches the natives the release will build. It is produced by the
   `manifest` job in `native.yml`. If upstream Ada moved, this file has to be regenerated and
   committed, or the release fails its verification step, which is the intended behaviour.
3. `PublicAPI.Unshipped.txt` entries are moved to `PublicAPI.Shipped.txt` for a stable release.
4. CI is green on `main`.

## Releasing

```bash
git tag v0.1.0
git push origin v0.1.0
```

That triggers `release.yml`, which builds all six natives from the pinned upstream tag, packs
with the completeness gate active, and consumes the package from a clean project on every
platform including Alpine. The `publish` job then waits on the `production` environment
approval.

Use the `workflow_dispatch` trigger with `dry-run: true` to exercise everything except the push
to nuget.org.

## Rolling back

**A published NuGet package cannot be deleted.** Unlisting is the rollback, and it is the only
one available.

```bash
dotnet nuget delete Ada.Url <version> --source https://api.nuget.org/v3/index.json --non-interactive
```

Despite the command name that unlists rather than deletes. The package stays resolvable for
anyone who already depends on that exact version, and stops appearing in search and in version
ranges. Follow it with a patch release, because unlisting alone leaves existing consumers on the
bad version.

## After releasing

1. Install the published package from nuget.org in a clean project on Windows, Linux and macOS
   and run the smoke test. The `verify` job tests the local artifact, which is not the same as
   testing what nuget.org actually served.
2. Confirm the GitHub release carries the nupkg, snupkg and SBOM.
3. Open the next `Unreleased` section in `CHANGELOG.md`.

## What is not automated yet

Code signing is gated behind the `CODE_SIGNING_ENABLED` repository variable and needs a
certificate in the `production` environment. Until one exists the step is skipped rather than
faked. macOS notarisation has the same status, gated on `MACOS_SIGNING_IDENTITY`.
