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

> ## 🛑 TENET — Build the solution ONLY with `build.cmd`
>
> **Never** build, validate, or declare the WinForms solution "clean" with a plain
> `dotnet build` / `dotnet msbuild` of `Winforms.sln`. Only **`build.cmd`** (Arcade) applies the
> repository's CI configuration — the **PublicAPI analyzer (RS0016/RS0017)**, the code-style and
> documentation analyzers, and **`-warnAsError`**. A plain `dotnet build` silently downgrades or
> skips these, so **"0 warnings" there does NOT mean CI is green** — the very same change can fail
> the official build with errors.
>
> * **Full / release / package / "is it clean?" verification → always `build.cmd`** (see §2).
> * A single-project `dotnet build` (see §3) is an **inner-loop convenience only**. It is fine while
>   iterating, but you **must re-verify with `build.cmd` before claiming a change builds cleanly**.
> * If `build.cmd` cannot run in your environment, the closest fallback is
>   `dotnet build <project> /p:ContinuousIntegrationBuild=true /p:TreatWarningsAsErrors=true` — and
>   you must say so explicitly rather than implying a `build.cmd` result.

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

> **Always use `build.cmd` (Arcade) for full, release, and package builds.** Do **not** use a plain
> `dotnet build` of the solution for these — only `build.cmd` guarantees the Arcade-supported build
> options and the download of the correct base SDK (`global.json`) needed to compile. Plain
> `dotnet build` is reserved for the fast single-project inner loop (see Section 3), and even then
> only after at least one successful `build.cmd` / `Restore.cmd`.

```
.\build.cmd
```

This restores **and** builds `Winforms.sln` in `Debug|Any CPU` by default.

Under the hood this runs:

```powershell
eng\common\Build.ps1 -NativeToolsOnMachine -restore -build -bl
```

### 2.1  Full (clean) test build — required workflow

For a full, clean build of the whole solution, **clean the artifacts first, then build**:

```powershell
# 1. Clean the artifacts folder.
.\build -clean

# 2. Build the full solution.
.\build
```

**Reporting requirement:** a full build is long-running, so while it runs **report progress back to
the user in the console to bridge the wait and give early orientation.** As assemblies complete,
report which assemblies have been **built successfully** and which **failed and with how many
errors**. Prefer running the build with a binary log (the default `-bl`) and/or stream the console
output so per-project results can be surfaced as they happen rather than only at the end.

### 2.2  Release build

```powershell
.\build -configuration release
```

### 2.3  Creating packages

```powershell
# Debug packages
.\build -pack

# Release packages
.\build -configuration release -pack
```

### 2.4  Full `Build.ps1` parameter list

`build.cmd` forwards every extra argument to `eng\common\Build.ps1`. The full surface is:

```
Build.ps1 [-configuration <string>] [-platform <string>] [-projects <string>]
          [-verbosity <string>] [-msbuildEngine <string>] [-warnAsError <bool>]
          [-warnNotAsError <string>] [-nodeReuse <bool>] [-buildCheck] [-restore]
          [-deployDeps] [-build] [-rebuild] [-deploy] [-test] [-integrationTest]
          [-performanceTest] [-sign] [-pack] [-publish] [-clean] [-productBuild]
          [-fromVMR] [-binaryLog] [-binaryLogName <string>] [-excludeCIBinarylog]
          [-ci] [-prepareMachine] [-runtimeSourceFeed <string>]
          [-runtimeSourceFeedKey <string>] [-excludePrereleaseVS]
          [-nativeToolsOnMachine] [-help] [-properties <string[]>] [<CommonParameters>]
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

> **Inner-loop only.** Use plain `dotnet build` of a single project **only** for quick iteration on
> one project, and **only after** at least one successful `.\build.cmd` / `.\Restore.cmd`. It does
> **not** guarantee the Arcade-supported build options or the download of the correct base SDK, so it
> must **never** be used for a full solution build, a release build, packaging, or any build whose
> result you intend to report as authoritative. For those, always use `build.cmd` (Section 2).

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
