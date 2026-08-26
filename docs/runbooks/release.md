# Release runbook

## Before tagging

1. `CHANGELOG.md` has an entry for the version, moved out of Unreleased. The GitHub release
   notes are built from that section, so it is the text people will actually read.
2. `src/Ada.Url/Ada.Url.csproj` has the matching `<Version>`.
3. `native/CHECKSUMS.txt` matches the natives the release will build. It is produced by the
   `manifest` job in `native.yml`. If upstream Ada moved, this file has to be regenerated and
   committed, or the release fails its verification step, which is the intended behaviour.
4. `PublicAPI.Unshipped.txt` entries are moved to `PublicAPI.Shipped.txt` for a stable release.
5. CI is green on `main`.

The first three are checked by the `preflight` job before anything is built, so getting one
wrong costs seconds rather than a whole release.

## Releasing

```bash
git tag v0.1.0
git push origin v0.1.0
```

That triggers `release.yml`:

| Job | What it does |
| --- | --- |
| `preflight` | Checks the tag, the project version and the changelog agree. Seconds. |
| `natives` | Builds all six native libraries from the pinned upstream Ada tag. |
| `verify` | Packs with the completeness gate active and consumes the package from a clean project on five platforms, Alpine included. |
| `publish` | Waits on the `production` environment approval, then verifies checksums, packs, builds the SBOM, pushes to nuget.org, reinstalls from nuget.org, and creates the GitHub release. |

Approve the deployment at the run's page, under `publish`, `Review deployments`.

Everything after the approval is automatic. **The GitHub release is created by the workflow**,
with notes taken from the `CHANGELOG.md` section for the tag, an install snippet, and the
`.nupkg`, `.snupkg` and SBOM attached. A tag containing a `-`, so any beta or rc, is marked as a
prerelease. Do not create one by hand at `/releases/new`; that only produces a duplicate.

Use the `workflow_dispatch` trigger with `dry-run: true` to exercise everything except the push
to nuget.org and the release creation.

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

The workflow already reinstalls the package from nuget.org and creates the release, so what is
left is short.

1. Read the release page. Confirm the notes match the changelog and that the nupkg, snupkg and
   SBOM are attached.
2. Run `tests/packaging/verify-published.sh --version <version>` on a second operating system.
   The workflow runs it on Windows only, and this is the one test that uses what nuget.org
   actually served rather than a local artifact.
3. Open the next `Unreleased` section in `CHANGELOG.md` and bump `<Version>` for the next cycle.
   This is the one manual changelog step; nothing writes it for you.

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
