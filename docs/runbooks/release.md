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

## One time setup on nuget.org

Publishing uses trusted publishing, so there is no long lived API key anywhere. nuget.org
validates a short lived GitHub OIDC token against a policy and hands back a temporary key that
lives for one hour.

Register the policy once, at nuget.org under your username, Trusted Publishing:

| Field | Value |
| --- | --- |
| Repository Owner | `sanamhub` |
| Repository | `ada-csharp` |
| Workflow File | `release.yml` (file name only, no path) |
| Environment | `production` |

Then add one repository secret, `NUGET_USER`, holding the nuget.org **profile name**, not the
email address. That is all the workflow needs.

Two things to know about the policy. It applies to every package owned by the chosen owner, so
pick between your user and an organisation deliberately. And on a private repository a new policy
is only temporarily active for seven days until the first successful publish records the
repository and owner IDs, which is what stops someone deleting a repository, recreating it under
the same name, and publishing as if nothing changed. This repository is public, so that does not
apply here.

If trusted publishing is not yet visible on the account, it is being rolled out gradually. The
fallback is a scoped API key in a `NUGET_API_KEY` secret, which then has to be rotated and is
worth replacing with trusted publishing when it appears.

## What is not automated yet

Code signing is gated behind the `CODE_SIGNING_ENABLED` repository variable and needs a
certificate in the `production` environment. Until one exists the step is skipped rather than
faked. macOS notarisation has the same status, gated on `MACOS_SIGNING_IDENTITY`.
