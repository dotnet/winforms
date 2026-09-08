# Work Order: HighPrecisionTimer Rework (Animation Timing)

**Branch:** `Net11/Integration-2` (KlausLoeffelmann/winforms)
**Files:**
- `src/System.Windows.Forms.Primitives/src/System/Windows/Forms/Animation/HighPrecisionTimer.cs`
- `src/System.Windows.Forms.Primitives/src/System/Windows/Forms/Animation/HighPrecisionTimerTick.cs`
- `src/System.Windows.Forms.Primitives/tests/UnitTests/System/Windows/Forms/Animation/HighPrecisionTimerTests.cs`
- Consumer: `src/System.Windows.Forms/System/Windows/Forms/Rendering/Animation/AnimationManager.cs`

This work order is self-contained; it results from a code review of the current implementation.
Read the current sources first, verify each finding against the code as it stands (the branch may have
moved), then implement.

**Keep as-is (explicitly not up for redesign):** the registration model — per-registration
`SynchronizationContext` capture, `InFlight` CAS-based frame coalescing with a `DroppedFrames` counter
surfaced in `HighPrecisionTimerTick`, and the id-based `TimerRegistration` disposable struct
(double-dispose safe, `default` safe). The *clock/pacing core* is what gets replaced.

---

## Finding 1 (critical): fixed-cadence pacer + spin causes a sawtooth burning ~50% of a core

Current design: `PeriodicTimer` at 14 ms (60 Hz path), then `SpinToTarget` spins to the 16.667 ms
frame target with `SpinOnce(sleep1Threshold: -1)` (never sleeps).

`PeriodicTimer` fires on its own **fixed** cadence (14, 28, 42, 56, …) and does not re-phase per
`WaitForNextTickAsync` call, while frame targets are 16.67, 33.33, 50, 66.67, … The phase slips
2.67 ms per frame, so spin duration grows every frame — wake 28 → spin 5.3 ms; wake 42 → spin 8 ms;
wake 56 → spin 10.7 ms — until phases wrap and the sawtooth restarts. Average spin ≈ half a frame
≈ 8 ms of every 16.67 ms ⇒ ~50% of one core, continuously, for as long as **any** animation is
registered. Infinite cycles (e.g. a pulsing focus indicator) make this permanent. The 30 Hz path
(30 ms tick vs 33.33 ms frame) has the identical slip.

**Required fix (architecture, not constant-tuning):** absolute schedule. Due times are
`epoch + frameIndex * period` against a single clock; wait on a mechanism that can hit them
(Finding 3), with at most a sub-millisecond residual spin. Overshoot must be amortized against the
absolute schedule (no `lastTick + period` relative scheduling — that accumulates and runs slow).

## Finding 2: integer-millisecond arithmetic throughout

`stopwatch.ElapsedMilliseconds` (truncated `long`) feeds `lastTickTimestamp`, `elapsed`, the spin
target, drift detection, and the `Timestamp`/`Elapsed` values delivered to consumers — ±1 ms
quantization (6% of a 16.667 ms budget) plus systematic truncation bias. Use `ElapsedTicks` /
`Elapsed.TotalMilliseconds` (double) end to end; `HighPrecisionTimerTick` fields stay `TimeSpan`,
constructed from ticks.

## Finding 3: replace `timeBeginPeriod(1)` with a high-resolution waitable timer

The code gates on `windows10.0.17134` — which `timeBeginPeriod` (ancient) does not need, but which is
exactly the build (1803) that introduced `CreateWaitableTimerExW` with
`CREATE_WAITABLE_TIMER_HIGH_RESOLUTION`. Use it:

- absolute due times (negative-relative or absolute FILETIME) with sub-ms accuracy,
- no process-wide timer-resolution raise (current code holds 1 ms resolution for the entire lifetime
  of any animation — a documented power/battery anti-pattern, and post-Win11 the effective resolution
  changes for occluded windows, silently shifting the timing floor),
- composes directly with Finding 1's absolute schedule and eliminates the spin loop almost entirely.

Fallback below 17134: keep a coarse path (30 Hz, plain waits, no `timeBeginPeriod`) — document that
sub-frame precision is not attempted there. Remove `TimeBeginPeriod`/`TimeEndPeriod` P/Invokes if no
longer referenced.

## Finding 4: `Register`/`Unregister` vs `StopTimer` race strands registrations

`Unregister` does `TryRemove` → `IsEmpty?` → `StopTimer()` without coordinating with `Register`.
Interleaving: A removes the last entry and observes empty; B adds a registration and `EnsureRunning`
sees `s_loopTask != null` (still running) and returns; A stops the timer. Result: live registration,
dead timer — animation frozen until an unrelated `Register` restarts the loop.
**Fix:** perform the emptiness check + stop decision under `s_lock` together with loop-state
transitions, or introduce a generation counter that `StopTimer` validates before actually stopping.

## Finding 5: per-frame, per-registration closure allocations on the hot path

`SyncContext.Post(_ => _ = InvokeCallbackAsync(registration, tick, cancellationToken), null)`
allocates closure + delegate per registration per frame (60 Hz × N renderers of steady GC pressure).
**Fix:** one cached `static SendOrPostCallback`; pass state via a per-registration state object —
`InFlight` guarantees exclusivity, so tick data can be written into a reusable per-registration slot
before posting. Target: zero allocations per frame in steady state.

## Finding 6: drift `Debug.Assert` is an assert storm

Drift >20% for 10 frames is normal under a debugger, breakpoints, or CI load. Once tripped,
`consecutiveDriftFrames` keeps incrementing, so the assert fires **every subsequent frame**.
**Fix:** replace with tracing/EventSource counters (drift, dropped frames, spin time). If any assert
remains, reset the counter after firing once.

## Finding 7: lifecycle edges

- `StopTimer` never observes `s_loopTask`; a stop/start pair can transiently run two loops, and a
  stopping loop can dispatch one final frame with a canceled token. Decide and document: either join
  the old loop (bounded) or make late dispatch provably benign.
- Post-unregister ticks can still be in flight toward a disposed consumer; `AnimationManager` /
  `AnimatedControlRenderer` must tolerate late callbacks — add a test.
- `Reset()` (test hook) mutates state without locking — document the serialization requirement or
  lock it.
- Dead code: `Registration.Id` is never read.
- `InvokeCallbackAsync` swallows non-OCE exceptions with `Debug.Fail` and keeps invoking the same
  callback forever; consider auto-unregistering a registration after N consecutive faults.

## Finding 8: single-SyncContext funnel in `AnimationManager` (design note)

`HighPrecisionTimer` correctly captures a `SynchronizationContext` **per registration**, but
`AnimationManager` is a process-wide singleton with a single registration, so every animation in a
multi-message-loop application marshals to the first UI thread — and dies with it. Minimum: document
the constraint. Better: make the manager per-UI-thread (e.g. `[ThreadStatic]` instance keyed off the
message-loop thread), preserving one timer-registration-per-UI-thread. Also note the duplicate
timeline: the manager keeps its own `Stopwatch` instead of deriving progress from
`HighPrecisionTimerTick.Timestamp/Elapsed`; animation progress should use the tick's timeline so frame
coalescing (`DroppedFrames`) is accounted for consistently.

## Finding 9: 60 Hz is a settled ceiling; the period is a pacer-owned runtime value for *downshift*

**Decision (do not relitigate):** the timer targets a hard 60 Hz ceiling (30 Hz fallback). Do not
add refresh-rate matching or raise the cadence. Rationale, for the record:

- Under DWM, windowed GDI/GDI+ apps do not tear (composition is tear-free from the redirection
  surface); the only artifact of an unsynced 60 Hz timer is judder, which is imperceptible for this
  content class (focus pulses, hover fades, toggle transitions — not motion/scrolling).
- GDI+ raster cost scales linearly with rate; driving N animated controls at 120–144 Hz multiplies
  compute for no perceptible gain (e.g. batched MVVM-driven updates across many controls).
- A process-wide timer cannot refresh-match on mixed-rate multi-monitor setups ("the" refresh rate
  is ill-defined), and `DwmFlush`-style vblank pacing blocks per frame, binds to one monitor, and
  misbehaves under RDP — unfit for a process-wide UI timer by construction.

**Required now (structural):** the frame period must be a runtime value owned by the pacer
(queryable/settable internally), not compile-time constants woven through the loop. The 30 Hz
fallback already makes the period variable; the forward-looking motivation is **downshifting**, not
matching: future power-driven reductions (30 Hz or full pause for occluded/minimized windows — where
Windows 11 timer coalescing already alters the effective cadence — and battery-saver scenarios)
must be addable without another rework.

---

## Acceptance criteria

1. Steady-state CPU of the timer loop with one registered infinite animation: **< 2% of one core**
   (measure; the current implementation is the ~50% baseline per Finding 1).
2. No `timeBeginPeriod` while animations run (verify via `powercfg /energy` or timer-resolution
   query on a Win10 1803+ box).
3. Frame delivery: mean interval within ±0.5 ms of target over a 10 s run on an idle machine at
   60 Hz; no monotonic slow drift (absolute-schedule check: 600th frame due time within one frame of
   `epoch + 600 × period`).
4. Zero per-frame heap allocations in steady state (verify with an allocation-tracking test or
   `GC.GetAllocatedBytesForCurrentThread` bracketing).
5. Race test for Finding 4: concurrent register/unregister stress leaves no registration without a
   running loop.
6. Existing `HighPrecisionTimerTests` updated/extended accordingly; late-callback tolerance test for
   consumers (Finding 7).
7. `HighPrecisionTimerTick` surface unchanged (internal consumers depend on it); all other churn is
   internal to the pacer.

## Constraints

- Everything stays `internal`; no public API review implications.
- Follow repo conventions (`LibraryImport`, nullable enabled, existing XML-doc voice).
- Windows-only paths gated with `[SupportedOSPlatform]` / `OperatingSystem.IsWindowsVersionAtLeast`
  as currently practiced in the file.
