# Release runbook

## Before tagging

1. `CHANGELOG.md` has an entry for the version, moved out of Unreleased. The GitHub release
   notes are built from that section, so it is the text people will actually read.
2. `src/Ada.Url/Ada.Url.csproj` has the matching `<Version>`.
3. `native/CHECKSUMS.txt` matches the natives the release will build. It is produced by the
   `manifest` job in `native.yml`. If the upstream Ada tag or a build script changed, regenerate
   and commit it, or the release fails verification, which is the intended behaviour.

   To regenerate: run the `native` workflow, open the `manifest` job, and copy its combined
   output. Only change this file for a reason you can name. It exists so a binary that changed
   without one stops the release, and every builds-are-not-reproducible workaround ends with
   somebody regenerating it on autopilot, which is exactly the habit an attacker relies on.

   All six RIDs are reproducible: the same source and flags give byte identical output. The
   `reproducible build` workflow builds win-x64 and linux-x64 twice from scratch every week and
   fails if the two differ. Windows needs `/Brepro` and `/PDBALTPATH` for this, since MSVC
   otherwise stamps the build time and a fresh PDB signature into every binary.
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
| `publish` | Waits on the `production` environment approval, then verifies checksums, packs, builds the SBOM, pushes to nuget.org, and creates the GitHub release. |
| `verify published` | Called by `release.yml` after `publish`. Waits for nuget.org validation, then installs the package from the live feed on five platforms. |

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

1. Read the release page. Confirm the notes match the changelog and that the nupkg, snupkg and
   SBOM are attached.
2. Watch `verify published`. It runs after `publish` and installs the package from nuget.org on
   Linux x64, Linux arm64, Alpine, macOS arm64 and Windows.

   It is called by `release.yml`, not triggered by the release event. A release created by a
   workflow using `GITHUB_TOKEN` does not start new workflow runs, so an `on: release` trigger
   here looks correct and never fires.

   Expect it to sit waiting. nuget.org validates and indexes a new package before anything can
   restore it, and documents that as taking up to an hour, so a package that is live on the site
   is not yet installable. The workflow waits up to 75 minutes. The release itself is created
   straight after the push and does not wait for any of this.
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
