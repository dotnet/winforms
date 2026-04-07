# PaintEventArgs.ResetGraphics – FailFast Crash

## Problem

Applications running on .NET 10 can terminate with a `FailFast` crash during painting:

```
Message: Called ResetGraphics more than once?
   at System.Windows.Forms.PaintEventArgs.ResetGraphics()
   at System.Windows.Forms.Control.WmPaint(Message& m)
```

The crash occurs when a `Form` (or any `Control`) is shown via `ShowDialog()` and its background is painted through the non-double-buffered code path. The process is terminated because `Debug.Fail` escalates to `Environment.FailFast` in .NET 10.

## Root Cause

`PaintEventArgs` has two internal ways to create the underlying `Graphics` object:

| Property | Method called on `DrawingEventArgs` | Saves graphics state? |
|---|---|---|
| `Graphics` (public) | `GetOrCreateGraphicsInternal()` — no callback | **No** |
| `GraphicsInternal` (internal) | `GetOrCreateGraphicsInternal(SaveStateIfNeeded)` — passes callback | **Yes** |

When `WmPaint` runs without double-buffering and `AllPaintingInWmPaint` is set, it creates a `PaintEventArgs` with an `HDC` and `DrawingEventFlags.SaveState`:

```csharp
pevent = new PaintEventArgs(dc, clip, paintBackground ? DrawingEventFlags.SaveState : default);
```

The expected call sequence is:

1. `PaintWithErrorHandling(pevent, PaintLayerBackground)` → `OnPaintBackground` → internal code accesses `GraphicsInternal` → `SaveStateIfNeeded` callback fires → `_savedGraphicsState` is set.
2. `pevent.ResetGraphics()` → finds `_savedGraphicsState != null` → restores state → sets it to `null`.

However, if user code in `OnPaintBackground` (or any downstream virtual) accesses `e.Graphics` (the public property) **before** any internal WinForms code has accessed `e.GraphicsInternal`, the `Graphics` object is created via the overload **without** the `SaveStateIfNeeded` callback. `_savedGraphicsState` remains `null`.

When `WmPaint` subsequently calls `pevent.ResetGraphics()`, the guard:

```csharp
if (_event.Flags.HasFlag(DrawingEventFlags.SaveState) && graphics is not null)
{
    if (_savedGraphicsState is not null)   // ← null because state was never saved
    {
        ...
    }
    else
    {
        Debug.Fail("Called ResetGraphics more than once?");  // ← fires → FailFast
    }
}
```

…detects `_savedGraphicsState == null` and calls `Debug.Fail`, which in .NET 10 terminates the process via `FailFast`.

The same class of bug also affects `PrintPaintEventArgs` (used in `WmPrintClient`), which is constructed with `DrawingEventFlags.SaveState` and calls `e.ResetGraphics()` from `OnPrint`.

## Alternative Approaches Considered

### Option A — Comment out `Debug.Fail` (rejected)

A prior workaround (WI00857973) commented out the `Debug.Fail` line:

```csharp
// commented out. Getting thrown a lot when we run net8.0-windows\CargoWiseOneAnyCpu.exe in debug mode.
// see: WI00857973 - Comment out Debug.Fail in PaintEventArgs in forked WinForms
//Debug.Fail("Called ResetGraphics more than once?");
```

This only silences the crash. The underlying state is still broken:

- `_savedGraphicsState` is still `null`, so `graphics.Restore(...)` is **silently skipped**.
- Any clip region or transform applied during `OnPaintBackground` **bleeds into `OnPaint`**, causing subtle rendering artifacts (wrong clipping, misaligned drawing).
- The `Debug.Fail` guard, which correctly detects real double-call bugs, is permanently disabled.

| | Comment out `Debug.Fail` | Our fix (Option B) |
|---|---|---|
| Crash fixed | ✓ | ✓ |
| Graphics state correctly restored | ✗ — silently skipped | ✓ |
| Root cause addressed | ✗ | ✓ |
| `Debug.Fail` still guards real double-call bugs | ✗ — removed | ✓ |
| Risk of rendering artifacts | Yes (clip/transform leaks) | None |

> Note: the `Debug.Fail` message "Called ResetGraphics more than once?" is misleading — the real bug is that it was called *before* state had been saved, not that it was called twice. Commenting it out treats the symptom while allowing incorrect rendering to proceed silently.

### A note on test design

`Debug.Fail` is decorated with `[Conditional("DEBUG")]` and is therefore a compile-time no-op in release/test builds. A test that simply asserts "no exception was thrown" when showing and refreshing a control will pass even **without** the fix, because the assertion never executes.

The correct approach is to assert the **observable behavioural invariant** that the fix preserves: a clip region applied in `OnPaintBackground` via the public `Graphics` property must not be visible in `OnPaint`. This is always detectable via `Graphics.IsVisible(centre)` regardless of build configuration.

### Option B — Save state in the `Graphics` getter (chosen)

## Solution

The fix is in [`PaintEventArgs.Graphics`](System.Windows.Forms/System/Windows/Forms/Rendering/PaintEventArgs.cs):

The public `Graphics` property is changed from an expression-bodied passthrough to a full property that detects first-time `Graphics` creation and calls `SaveStateIfNeeded` in that case — matching the behaviour already present in `GraphicsInternal`:

```csharp
public Graphics Graphics
{
    get
    {
        // When PaintEventArgs is created with an HDC and DrawingEventFlags.SaveState,
        // SaveStateIfNeeded is normally called lazily via GraphicsInternal on first access.
        // If user code accesses this public Graphics property before any GraphicsInternal
        // call, the Graphics object gets created without SaveStateIfNeeded being invoked,
        // leaving _savedGraphicsState as null. ResetGraphics() would then incorrectly
        // trigger a Debug.Fail. We detect first-time creation here and save the state.
        bool willBeCreated = _event.GetGraphics(create: false) is null;
        Graphics g = _event.Graphics;

        if (willBeCreated)
        {
            SaveStateIfNeeded(g);
        }

        return g;
    }
}
```

### Why this is safe

| Scenario | `willBeCreated` | Effect |
|---|---|---|
| `Graphics`-based constructor (double-buffered path) | `false` — graphics already exists from ctor | No-op; state was already saved in ctor via `SaveStateIfNeeded(graphics)`. |
| HDC path, `GraphicsInternal` accessed first (normal path) | `false` — callback already ran | No-op; `_savedGraphicsState` is already set. |
| HDC path, public `Graphics` accessed first (the bug) | `true` — graphics is about to be created | State saved immediately after creation. `ResetGraphics()` succeeds. ✓ |
| `SaveState` flag not set | No save attempted regardless | `SaveStateIfNeeded` is a no-op when `SaveState` is not in `Flags`. |

No double-save can occur because `willBeCreated` is only `true` on the very first call that creates the object; all subsequent calls return `false`.
