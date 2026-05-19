---
name: running-tests
description: >-
  Instructions for running unit tests, integration tests, and individual tests
  in the WinForms repository. Use this when asked how to run tests, filter
  them, use Visual Studio workflows, or troubleshoot test failures.
metadata:
  author: dotnet-winforms
  version: "2.0"
---

# Running Tests in the WinForms Repository

The repository uses **xUnit v3** with **Microsoft.Testing.Platform** as its
test framework. Tests are organized into **unit tests** and
**integration / functional tests**.

> **Important:** The test projects build as self-contained executables using
> Microsoft.Testing.Platform (not the legacy VSTest adapter). The `dotnet test`
> `--filter` flag and `vstest.console.exe` do **not** work with these projects.
> See sections 4 and 4b for the correct filtering syntax.

---

## 1  Running All Unit Tests

```bash
.\build.cmd -test
```

This builds the solution **and** executes every unit test project. Results
appear in `artifacts\TestResults\`.

### Release configuration

```bash
.\build.cmd -test -configuration Release
```

---

## 2  Running All Integration / Functional Tests

```bash
.\build.cmd -integrationTest
```

Functional tests open and close windows automatically — do not interact with the
desktop while they run.

---

## 3  Running Tests for a Single Project

After a full build (`.\build.cmd`), navigate to the test project directory and
use `dotnet test`:

```bash
# Unit tests for System.Windows.Forms
pushd src\test\unit\System.Windows.Forms
dotnet test

# Unit tests for System.Drawing.Common
pushd src\System.Drawing.Common\tests
dotnet test

# Unit tests for System.Windows.Forms.Primitives
pushd src\System.Windows.Forms.Primitives\tests\UnitTests
dotnet test
```

> **Tip:** The repo-local SDK must be on your `PATH`.
> Run `.\Restore.cmd` or launch via `.\start-code.cmd` / `.\start-vs.cmd`.

### Key test project paths

| Test suite | Project directory |
|------------|-------------------|
| System.Windows.Forms.Tests | `src\test\unit\System.Windows.Forms` |
| System.Drawing.Common.Tests | `src\System.Drawing.Common\tests` |
| System.Windows.Forms.Primitives.Tests | `src\System.Windows.Forms.Primitives\tests\UnitTests` |
| System.Windows.Forms.Design.Tests | `src\System.Windows.Forms.Design\tests\UnitTests` |
| System.Windows.Forms.Analyzers.Tests | `src\System.Windows.Forms.Analyzers\tests\UnitTests` |

---

## 4  Running a Single Test or Filtered Tests (via executable)

The preferred way to run individual tests is to invoke the compiled test
executable directly from `artifacts\bin\`. The executables use xUnit v3's
built-in filter options — **not** the `--filter` flag.

### Prerequisites

The test executables target a .NET preview runtime. The correct runtime version
must be installed system-wide, **or** you must set `DOTNET_ROOT` to the
repo-local `.dotnet` folder:

```powershell
# From the repository root:
$env:DOTNET_ROOT = "$PWD\.dotnet"
$env:PATH = "$PWD\.dotnet;$env:PATH"
```

If the required .NET runtime is not installed, use the `download-sdk` skill to
install it.

> **Note:** The TFM in executable paths (e.g. `net11.0`) changes with each
> major .NET version. Check `artifacts\bin\<ProjectName>\Debug\` for the
> actual TFM directory name.

### Filter by method name (fully qualified)

```powershell
& "artifacts\bin\System.Windows.Forms.Tests\Debug\net11.0-windows7.0\System.Windows.Forms.Tests.exe" `
  --filter-method "System.Windows.Forms.Tests.ButtonTests.Button_AutoSizeModeGetSet"
```

### Filter by class

```powershell
& "artifacts\bin\System.Windows.Forms.Tests\Debug\net11.0-windows7.0\System.Windows.Forms.Tests.exe" `
  --filter-class "System.Windows.Forms.Tests.ButtonTests"
```

### Filter by namespace

```powershell
& "artifacts\bin\System.Drawing.Common.Tests\Debug\net11.0\System.Drawing.Common.Tests.exe" `
  --filter-namespace "System.Drawing.Tests"
```

### Wildcard matching

All filter options support `*` wildcards at the beginning and/or end:

```powershell
# All test methods containing "AutoSize"
--filter-method "*AutoSize*"

# All classes ending with "ButtonTests"
--filter-class "*ButtonTests"
```

### Filter by trait

```powershell
--filter-trait "Category=Accessibility"
```

### Exclude tests (negated filters)

```powershell
--filter-not-method "*SlowTest*"
--filter-not-class "System.Windows.Forms.Tests.ClipboardTests"
--filter-not-trait "Category=Interactive"
```

### Query filter language

For complex filtering, use `--filter-query` with xUnit's
[query filter language](https://xunit.net/docs/query-filter-language):

```powershell
--filter-query "/*/*/ButtonTests/*"
```

### Useful options

| Option | Description |
|--------|-------------|
| `--list-tests` | List all tests without running them |
| `--stop-on-fail` `on` | Stop on first failure |
| `--show-live-output` `on` | Show `ITestOutputHelper` output live |
| `--output` `Detailed` | Verbose output |
| `--parallel` `none` | Disable parallel execution |
| `--report-trx` | Generate a TRX report |
| `--report-xunit-html` | Generate an HTML report |

---

## 4b  Running a Single Test via `dotnet test` (alternative)

`dotnet test` can also be used from the test project directory, but note that
the `--filter` flag **does not work** with the Microsoft.Testing.Platform
runner. To pass xUnit v3 filter options through `dotnet test`, use `--`:

```bash
pushd src\test\unit\System.Windows.Forms
dotnet test -- --filter-method "System.Windows.Forms.Tests.ButtonTests.Button_AutoSizeModeGetSet"
```

> **Note:** If `dotnet test --filter` reports "Zero tests ran" with exit
> code 5, this is expected — switch to the executable approach in section 4
> or use the `--` separator.

---

## 5  Running Tests from Visual Studio

1. Launch via `.\start-vs.cmd` (ensures repo-local SDK is on `PATH`).
2. Open **Test Explorer** (<kbd>Ctrl+E, T</kbd>).
3. Run / debug tests as usual.

For common VS-specific issues see `docs\testing-in-vs.md`.

---

## 6  Test Project Layout

Each WinForms library has its own test projects:

```
src\
  test\
    unit\
      System.Windows.Forms\       ← System.Windows.Forms.Tests.csproj
  System.Drawing.Common\
    tests\                        ← System.Drawing.Common.Tests.csproj
  System.Windows.Forms.Primitives\
    tests\UnitTests\              ← ...Primitives.Tests.csproj
  System.Windows.Forms.Design\
    tests\UnitTests\              ← ...Design.Tests.csproj
  System.Windows.Forms.Analyzers\
    tests\UnitTests\              ← ...Analyzers.Tests.csproj
  ...
```

Test output (compiled executables) goes to:

```
artifacts\bin\<ProjectName>\Debug\<tfm>\<ProjectName>.exe
```

New test source files are auto-included (SDK-style project) — no `.csproj`
edits needed.

---

## 7  Test Attributes & Categories

| Attribute | When to use |
|-----------|-------------|
| `[WinFormsFact]` / `[WinFormsTheory]` | Tests involving UI controls or requiring a synchronization context |
| `[StaFact]` / `[StaTheory]` | Tests requiring an STA thread but not the full WinForms context |
| `[Fact]` / `[Theory]` | Pure logic tests with no UI or threading requirements |
| `[Collection("Sequential")]` | Tests that must **not** run in parallel (e.g. clipboard, drag-and-drop, global state) |

### Theory data

Use `[InlineData]` or `[MemberData]` for parameterized tests. Avoid creating
UI controls inside member-data methods — create them inside the test body
instead.

---

## 8  Test Results & Troubleshooting

| Artifact | Location |
|----------|----------|
| TRX results | `artifacts\TestResults\Debug\<Project>_<tfm>_<arch>.trx` |
| HTML results | `artifacts\TestResults\Debug\<Project>_<tfm>_<arch>.html` |
| XML results | `artifacts\TestResults\Debug\<Project>_<tfm>_<arch>.xml` |
| Log summary | `artifacts\log\` |
| Binary log | `artifacts\log\Debug\Build.binlog` |

### Parsing test results programmatically

TRX files are XML with namespace `http://microsoft.com/schemas/VisualStudio/TeamTest/2010`.
To find failed tests:

```powershell
Get-ChildItem "artifacts\TestResults\Debug\*.trx" | ForEach-Object {
    [xml]$trx = Get-Content $_.FullName
    $ns = @{t='http://microsoft.com/schemas/VisualStudio/TeamTest/2010'}
    $counters = Select-Xml -Xml $trx -XPath '//t:ResultSummary/t:Counters' -Namespace $ns |
        Select-Object -ExpandProperty Node
    if ([int]$counters.failed -gt 0) {
        Write-Host "=== $($_.BaseName) ($($counters.failed) failed) ==="
        Select-Xml -Xml $trx -XPath '//t:UnitTestResult[@outcome="Failed"]' -Namespace $ns |
            ForEach-Object { Write-Host "  FAIL: $($_.Node.testName)" }
    }
}
```

### Common issues

* **"Zero tests ran" with `dotnet test --filter`** — the repo uses
  Microsoft.Testing.Platform, which does not support the `--filter` flag.
  Use the test executable directly with `--filter-method` /
  `--filter-class` (see section 4), or pass filters after `--` separator.
* **"Could not find testhost" / "testhost.deps.json not found"** —
  `vstest.console.exe` and `dotnet vstest` cannot run these test assemblies.
  Use the test executable directly (section 4).
* **"framework not found" when running test .exe** — the required .NET
  preview runtime is not installed. Set `DOTNET_ROOT` to the repo-local
  `.dotnet` folder, or install the runtime using the `download-sdk` skill.
* **Test runner crash** — configure automatic memory-dump collection
  ([Collecting User-Mode Dumps](https://learn.microsoft.com/windows/win32/wer/collecting-user-mode-dumps)),
  then reproduce and inspect the dump in WinDbg.
* **Flaky clipboard / drag-and-drop tests** — ensure they are in the
  `[Collection("Sequential")]` collection so they don't run in parallel.
* **VS test discovery fails** — see `docs\testing-in-vs.md`.

---

## Quick-Reference Command Cheat Sheet

```bash
# Full build + all unit tests
.\build.cmd -test

# Full build + all integration tests
.\build.cmd -integrationTest

# Single project tests (after initial build)
pushd src\test\unit\System.Windows.Forms
dotnet test

# --- Running individual tests via executable (preferred) ---

# Set up repo-local runtime (from repo root, if system runtime is not installed)
$env:DOTNET_ROOT = "$PWD\.dotnet"
$env:PATH = "$PWD\.dotnet;$env:PATH"

# Single test by fully-qualified method name
& "artifacts\bin\System.Windows.Forms.Tests\Debug\net11.0-windows7.0\System.Windows.Forms.Tests.exe" `
  --filter-method "System.Windows.Forms.Tests.ButtonTests.Button_AutoSizeModeGetSet"

# All tests in a class
& "artifacts\bin\System.Windows.Forms.Tests\Debug\net11.0-windows7.0\System.Windows.Forms.Tests.exe" `
  --filter-class "System.Windows.Forms.Tests.ButtonTests"

# Wildcard match on method name
& "artifacts\bin\System.Windows.Forms.Tests\Debug\net11.0-windows7.0\System.Windows.Forms.Tests.exe" `
  --filter-method "*AutoSize*"

# List all tests without running
& "artifacts\bin\System.Windows.Forms.Tests\Debug\net11.0-windows7.0\System.Windows.Forms.Tests.exe" `
  --list-tests
```
