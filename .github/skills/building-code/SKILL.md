---
name: building-code
description: >-
  Instructions for restoring and building the WinForms repository.
  Use when asked how to restore NuGet packages, build the full solution,
  build a single project, create packages, or troubleshoot build errors.
metadata:
  author: dotnet-winforms
  version: "1.0"
---

# Building the WinForms Repository

## Prerequisites

* Windows is required for WinForms runtime scenarios, test execution, and Visual
  Studio workflows.
* Linux is supported for command-line restore/build only; use `build.sh`
  instead of `build.cmd` / `Restore.cmd`.
* Visual Studio 2022 (for IDE builds) — see `WinForms.vsconfig` for required workloads.
* The repo-local .NET SDK (specified in `global.json`) is used automatically by
  `build.cmd` and `Restore.cmd`. You do **not** need a machine-wide SDK install
  for command-line builds.

---

## 1  Restore

Restoring downloads the repo-local SDK and all NuGet packages.

```
.\Restore.cmd
```

Under the hood this runs:

```powershell
eng\common\Build.ps1 -NativeToolsOnMachine -restore
```

You can pass any extra `Build.ps1` flags after `Restore.cmd`, e.g.
`.\Restore.cmd -configuration Release`.

---

## 2  Full Solution Build (preferred)

```
.\build.cmd
```

This restores **and** builds `Winforms.sln` in `Debug|Any CPU` by default.

Under the hood this runs:

```powershell
eng\common\Build.ps1 -NativeToolsOnMachine -restore -build -bl
```

### Common flags

| Flag | Short | Description |
|------|-------|-------------|
| `-configuration <Debug\|Release>` | `-c` | Build configuration (default: `Debug`) |
| `-platform <x86\|x64\|Any CPU>` | | Platform (default: `Any CPU`) |
| `-restore` | `-r` | Restore only |
| `-build` | `-b` | Build only (skip restore if already done) |
| `-rebuild` | | Clean + build |
| `-clean` | | Delete build artifacts |
| `-pack` | | Create NuGet packages (`Microsoft.Private.Winforms`) |
| `-bl` / `-binaryLog` | | Emit `artifacts\log\Debug\Build.binlog` |
| `-ci` | | CI mode (stricter warnings, signing, etc.) |
| `-test` | `-t` | Build **and** run unit tests |
| `-integrationTest` | | Build **and** run integration / functional tests |

### Examples

```bash
# Release build
.\build.cmd -configuration Release

# Build and run unit tests
.\build.cmd -test

# Create NuGet package
.\build.cmd -pack
```

---

## 3  Optimized Building a Single Project (fast inner-loop)

Prefer rebuilding just the project(s) with recent changes by using the
standard `dotnet build` command, **after** at least one initial successful
full restore (via `.\Restore.cmd` or `.\build.cmd`).

This is **much** faster than building the whole solution.

```bash
# Build a single src project
dotnet build src\System.Windows.Forms\System.Windows.Forms.csproj

# Build a single test project
dotnet build src\test\unit\System.Windows.Forms\System.Windows.Forms.Tests.csproj

# Release configuration
dotnet build src\System.Windows.Forms\System.Windows.Forms.csproj -c Release
```

> **Tip:** The repo-local SDK must be on your `PATH`. Running `.\start-code.cmd`
> or `.\start-vs.cmd` prepends it automatically. From a plain terminal you can
> also run `.\Restore.cmd` first (it sets up the SDK).

---

## 4  Building from Visual Studio

1. Run `.\Restore.cmd` (one-time, or after SDK/package changes).
2. Run `.\start-vs.cmd` — opens `Winforms.sln` with the repo-local SDK on `PATH`.
3. Build normally (<kbd>Ctrl+Shift+B</kbd>).

## 5  Building from Visual Studio Code

1. (Optional) `.\Restore.cmd`
2. `.\start-code.cmd` — opens the workspace with the repo-local SDK on `PATH`.
3. Build from the integrated terminal: `.\build.cmd` or `dotnet build <project>`.

---

## Build Outputs

| Artifact | Location |
|----------|----------|
| Binaries | `artifacts\bin\<Project>\Debug\<tfm>\` |
| Logs | `artifacts\log\` |
| Binary log | `artifacts\log\Debug\Build.binlog` |
| Test results | `artifacts\TestResults\` |
| NuGet packages | `artifacts\packages\` |

Use the [MSBuild Structured Log Viewer](https://msbuildlog.com/) to inspect
`.binlog` files when troubleshooting build errors.

---

## Troubleshooting

* **Most errors are compile errors** — fix them as usual.
* **MSBuild task errors** — inspect `artifacts\log\Debug\Build.binlog`.
* **SDK version mismatch** — the repo pins its SDK in `global.json`;
  run `.\Restore.cmd` to ensure the correct SDK is available.
* **VS preview features** — if using a non-Preview VS, enable
  *Tools → Options → Environment → Preview Features →
  Use previews of the .NET SDK*.
