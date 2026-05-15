# Releasing a NuGet Package

The NuGet package is built and published via the **Create Release** GitHub Actions workflow (`.github/workflows/create-release.yml`).

There are two ways to trigger it.

---

## Option 1 — GitHub CLI

Use this when you want to release from a specific branch without opening the browser.

```powershell
# 1. Set the default repo (only needed once per machine / clone)
gh repo set-default WiseTechGlobal/winforms

# 2. Trigger the workflow
gh workflow run create-release.yml `
    --ref <branch-name> `
    --field version=<version>
```

**Example:**

```powershell
gh repo set-default WiseTechGlobal/winforms
gh workflow run create-release.yml `
    --ref SPG/WI01075819/WM_INITMENUPOPUP_Missing `
    --field version=0.1.2-pr.17.1
```

The `version` field must be a valid [SemVer 2.0.0](https://semver.org/) string, for example:
- `1.2.3` for a stable release
- `1.2.3-preview.1` for a pre-release

---

## Option 2 — GitHub Actions Web UI

Use this when you prefer a point-and-click experience.

1. Open the repository on GitHub and go to **Actions**.
2. Select the **Create Release** workflow from the left-hand list.
3. Click **Run workflow** (top-right of the run list).
4. In the pop-up:
   - Choose the **branch** you want to release from.
   - Enter the **package version** (e.g. `1.2.3` or `1.2.3-preview.1`).
5. Click the green **Run workflow** button.

---

## Notes

- The workflow **pushes to NuGet.org only for manual (`workflow_dispatch`) runs**. Pull-request–triggered runs build and validate the package but do not publish it.
- The package project is located at `src/System.Windows.Forms.Legacy/System.Windows.Forms.Package/System.Windows.Forms.Package.csproj`.
- Produced `.nupkg` files are written to `artifacts/packages/`.
