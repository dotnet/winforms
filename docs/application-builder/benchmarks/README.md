# WinForms Application Builder lifecycle benchmarks

This standalone STA-aware harness compares three complete UI-thread lifecycles:

- Conventional `Application.Run(Form)`.
- `WinFormsApplicationBuilder` without a host.
- `WinFormsApplicationBuilder` with an empty Generic Host.

Each run uses a small minimized, off-screen form that closes itself after it is
shown. The harness runs each scenario in a separate child process for cold
launch measurements and for independent warm/resource sessions. This prevents
static initialization from one scenario from contaminating the others and
keeps all WinForms work on an STA thread. The benchmark project references the
WinForms implementation in this checkout.

## Run

From the repository root in PowerShell:

```powershell
$env:DOTNET_ROOT = "$PWD\.dotnet"
$env:PATH = "$PWD\.dotnet;$env:PATH"
dotnet run --configuration Release --project docs\application-builder\benchmarks\WinFormsApplicationBuilder.Benchmarks.csproj
```

Defaults are five cold process launches, five warmups plus 30 timed warm
cycles, and five warmups plus 100 resource-stability cycles per scenario.
Iteration counts can be changed with `--cold-iterations`, `--warm-iterations`,
and `--resource-iterations`, respectively. For example:

```powershell
dotnet run --configuration Release --project docs\application-builder\benchmarks\WinFormsApplicationBuilder.Benchmarks.csproj -- --cold-iterations 10 --warm-iterations 100 --resource-iterations 500
```

## Measurements and interpretation

- **Cold process launch-to-exit** is parent-measured and includes process and
  CLR startup, WinForms initialization, form display, and shutdown. It is not
  an isolated `Run` call measurement.
- **Warm scenario-to-form-shown** measures scenario construction through the
  form's `Shown` event after in-process warmups.
- **Warm form-closed-to-dispose** measures from `FormClosed` through message
  loop exit and builder/host disposal.
- **UI-thread allocated bytes** uses
  `GC.GetAllocatedBytesForCurrentThread`. It deliberately does not claim to
  include allocations on Generic Host worker threads.
- **Resource deltas** report changes in process handle count, thread count,
  private bytes, and post-full-GC managed live bytes relative to the preceding
  checkpoint, with four checkpoints during the default repeated-cycle run.
  One-time runtime, JIT, WinForms, and host caches may account for bounded
  initial growth; inspect whether counters continue increasing across later
  checkpoints and repeat runs before labeling growth a leak.

Results are comparative diagnostics, not pass/fail thresholds. No acceptable
overhead budget has been set by #14946. Record the commit, SDK/runtime, OS
build, architecture, power mode, iteration counts, and complete output when
comparing runs. Cold timings and private bytes are particularly sensitive to
machine load and should not be compared across different hardware as if they
were controlled.

## Decisions and limits

- A dedicated harness is used instead of BenchmarkDotNet because the measured
  unit of work owns the calling STA thread and runs a real WinForms message
  loop; each cold-start trial also needs a fresh process.
- The baseline uses the same form and automatic close behavior, while the two
  builder scenarios isolate the builder's own cost from adding Generic Host.
- The harness does not register hosted services, open modal dialogs, or
  measure application-specific UI construction.
- Resource snapshots are coarse process counters. They can reveal monotonic
  growth but do not identify an allocation site or replace a profiler.
- CI execution is intentionally not configured: timing and process-resource
  numbers are environment-sensitive and are not stable correctness assertions.
