# Anchor Layout High-DPI Regression: .NET Framework vs .NET 8+

Controls anchored to `Bottom` or `Right` can appear displaced or invisible in .NET 8+ on any
monitor scaled above 100% DPI, while the same code is correct under .NET Framework 4.8.

The bug lives entirely in the **V1 anchor path** (`UpdateAnchorInfo` plus the compat repair in
`ComputeAnchoredBounds`) and does **not** affect `AnchorLayoutV2`.

Two separate V1 compat failures exist in the .NET 10 path:

- A **stale positive anchor capture** for controls anchored only to trailing edges (`Bottom`
  without `Top`, or `Right` without `Left`).
- A **stretch-anchor recovery double-refresh** where a stretch-anchored parent is recovered from
  the wrong baseline and then immediately re-qualifies for the stale-positive repair on the same
  axis, collapsing the recovered height.

`AnchorLayoutV2` eliminates the root cause (see [Solution 1](#solution-1--enable-anchorlayoutv2-recommended)).
The **fork compat repair** resolves both failures at the framework level without requiring an application
change (see [Solution 3](#solution-3--additive-layout-side-repair-current-fork-implementation)).

---

## Problem Description

### Symptoms

On any machine where at least one monitor is scaled above 100% DPI (e.g. 200%), controls with
`Anchor` involving `Bottom` or `Right` inside deeply nested containers (e.g. `GroupBox` →
`TabPage` → `TabControl`) are positioned far outside their parent's client area when the form
becomes visible — or are completely invisible.

Characteristic signs:

| Observation | Detail |
|---|---|
| Control appears below visible area | `Bounds.Bottom` greatly exceeds `ClientRectangle.Height` |
| Problem is DPI-dependent | Invisible at 100% DPI, severe at 200%+ |
| Problem is already present before `Show()` | The bad position is set during form construction |
| The stale-anchor bug is .NET 8+ specific | The bad positive anchor capture does not occur under .NET Framework 4.8 |

### Reproduction

The displacement is proportional to the DPI scale factor. At 200% DPI, the child control's
coordinates are already DPI-scaled at the time of anchor capture, but the parent's
`DisplayRectangle` is still at a transient construction-time size. For example:

```
parent ClientRectangle.Height = 100   ← transient height during construction
control bounds (DPI-scaled)   = {Y=284, Height=176}
bottomOffset = (284 + 176) − 100 = +360   ← positive: stale capture detected
→ final Y    = finalParentHeight + 360 − 176   ← control far below visible area
```

The transient parent height occurs when a child control is added to a container **after** that
container has been inserted into a deeper hierarchy, but before any top-level form is attached.
At that moment the layout engine assigns a temporary, too-small height to the container, and the
V1 path captures the anchor offset against that height.

---

## Root Cause

### The Stable Behavior in .NET Framework 4.8

In .NET Framework, the `EnableAnchorLayoutHighDpiImprovements` switch defaults to `false`. The
legacy `ComputeAnchoredBounds` path always stores `Right` and `Bottom` as **negative distances**
from the parent's edges:

```
anchorInfo.Right  = elementBounds.Right  - parentWidth    // e.g. −20 (20 px from right edge)
anchorInfo.Bottom = elementBounds.Bottom - parentHeight   // e.g. −12 (12 px from bottom edge)
```

These negative offsets are stable regardless of when they are captured and are simply added to
the live `displayRect` dimensions at layout time, placing the control correctly.

### The Broken Path in .NET 8+

`EnableAnchorLayoutHighDpiImprovements` no longer exists. The DPI-aware branch is now gated on
`ScaleHelper.IsScalingRequirementMet`, which returns `true` whenever any monitor is above 96 dpi
(100%) or the process is per-monitor-aware:

```csharp
// ScaleHelper.cs
internal static bool IsScalingRequirementMet => IsScalingRequired || s_processPerMonitorAware;
internal static bool IsScalingRequired        => InitialSystemDpi != OneHundredPercentLogicalDpi;
```

When `IsScalingRequirementMet` is `true`, `UpdateAnchorInfo` (V1) takes a special branch intended
to rescue controls that were pushed off-screen by a DPI-scale event:

```csharp
if (IsAnchored(anchor, AnchorStyles.Right))
{
    if (ScaleHelper.IsScalingRequirementMet
        && (anchorInfo.Right - parentWidth > 0)  // control appears beyond right edge
        && (oldAnchorInfo.Right < 0))             // but had a valid negative anchor before
    {
        // Preserve old anchor to avoid losing the control off the right edge.
        anchorInfo.Right = oldAnchorInfo.Right;
    }
    else
    {
        anchorInfo.Right -= parentWidth;          // standard negative-distance calculation
    }
}
```

**The guard fires incorrectly during initial form construction.** When a child control is added
while its parent already has a transient (too-small) `DisplayRectangle`, these conditions hold:

1. The parent height is transiently small (e.g. `100`).
2. The control's `Y` coordinate is already DPI-scaled (e.g. `284` at 200%).
3. `anchorInfo.Bottom - parentHeight = (284 + 176) − 100 = +360 > 0` — the guard fires.
4. `oldAnchorInfo` is also invalid, so the preserved offset is wrong.

On the next layout pass, this wrong positive offset is applied against the final (larger) parent
height, displacing the control far outside the visible area.

### What the Compat Repair Fixes

The compat repair in `ComputeAnchoredBounds` detects a stale positive trailing anchor and
recomputes it against a stable baseline. After the repair, the stale positive `Bottom` (or
`Right`) value is converted back into a stable negative margin. The control is then placed
correctly relative to the parent's final size.

### The Stretch-Anchor Double-Refresh

A second failure can occur on the same layout pass when a container is itself stretch-anchored
(`Top | Bottom` or `Left | Right`):

1. The stretch anchor is recovered against the growth in the parent display rectangle.
2. After recovery, the `Bottom` (or `Right`) value is positive by design for a stretch-anchored
   control — satisfying the stale-positive check.
3. A second refresh would then overwrite the recovered stretch anchors, shrinking the container
   back down.

This is not a separate layout system defect. It is the result of the compat repair firing twice
with different semantics on the same axis. The fix evaluates the stretch check first and
short-circuits the positive check when the stretch path fires (see Solution 3).

### Why High DPI Amplifies the Error

At 200% DPI all coordinates are approximately doubled. A moderate anchor offset error (e.g. +180)
becomes a large one (+360), pushing the control well outside any visible area. At 100% DPI the
same error is small enough to be nearly invisible.

### Contributing Factors Summary

| Factor | Role |
|---|---|
| `ScaleHelper.IsScalingRequirementMet` | Activates the DPI-aware branch for any non-100% DPI monitor |
| `UpdateAnchorInfo` V1 guard condition | Fires incorrectly during construction against a transient parent size |
| `ControlDpiScalingHelper` coordinate scaling | Multiplies all coordinates by the DPI factor, amplifying the captured error |
| Stretch-anchor follow-up refresh | Reapplies stale-positive logic on an axis already recovered as a stretch anchor, shrinking the parent again |

---

## Solutions

Three independent approaches exist, ordered from most to least recommended.

### Decision Guide

```
Are you on .NET 8+ and can change the app configuration?
  └─ Yes → Solution 1 (AnchorLayoutV2) — eliminates the root cause, zero code change
Are you unable to change configuration (e.g. third-party host)?
  └─ Solution 3 (fork additive repair) — no app change required, smallest source delta
Do you need a workaround with no source changes at all?
  └─ Solution 2 (DpiUnaware) — loses DPI sharpness on scaled monitors
```

---

### Solution 1 — Enable `AnchorLayoutV2` ✅ Recommended

`AnchorLayoutV2` (introduced in .NET 8, opt-in) replaces the over-eager V1 capture with a
**deferred model**: anchor offsets are not committed until the parent's layout is resumed and its
`DisplayRectangle` is stable. This eliminates the transient-capture problem at the source.

Enable it in `runtimeconfig.template.json` (or `runtimeconfig.json`):

```json
{
  "configProperties": {
    "System.Windows.Forms.AnchorLayoutV2": true
  }
}
```

| Attribute | Value |
|---|---|
| Risk | Low — opt-in, no source change required |
| Scope | Per-application configuration |
| Verified | Yes — eliminates the transient-capture path entirely |
| Limitation | Requires the application to control its own `runtimeconfig` |

---

### Solution 2 — Use `DpiUnaware` or `DpiUnawareGdiScaled`

Setting the DPI mode to `DpiUnaware` (or `DpiUnawareGdiScaled`) makes
`IsScalingRequirementMet` return `false`, so .NET 8 follows the same stable anchor path as
.NET Framework.

```csharp
Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
// or
Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
```

| Attribute | Value |
|---|---|
| Risk | Low — well-understood mode |
| Scope | Per-application startup |
| Verified | Yes — sidesteps the broken branch entirely |
| Limitation | UI appears blurry on scaled monitors (GDI bitmap stretching) |

---

### Solution 3 — Additive Layout-Side Repair ⭐ Current Fork Implementation

This is the **current implementation** in the WiseTech Global WinForms fork. It preserves the
original `UpdateAnchorInfo` behavior entirely and **adds a compat repair step** inside
`ComputeAnchoredBounds` that handles both stale positive trailing anchors and stale stretch-anchor
recovery when the parent's `DisplayRectangle` has grown to a stable size.

**Key design principle:** additive and non-breaking — the existing V1 behavior is unchanged.

The single call site in `DefaultLayout.cs`, inside `ComputeAnchoredBounds`, is:

```csharp
// DefaultLayout.cs — ComputeAnchoredBounds
AnchorInfo layout = GetAnchorInfo(element)!;

// ... read left/top/right/bottom from layout ...

AnchorStyles anchor = TryRefreshAnchorInfoForDisplayRectangleGrowth(element, layout, displayRect);
```

All compat logic lives in `DefaultLayout.AnchorLayoutCompat.cs` (a partial class file) to keep the
change isolated from the main layout file:

```csharp
// DefaultLayout.AnchorLayoutCompat.cs
private static AnchorStyles TryRefreshAnchorInfoForDisplayRectangleGrowth(
    IArrangedElement element, AnchorInfo anchorInfo, Rectangle displayRect)
{
    AnchorStyles anchor = GetAnchor(element);
    Rectangle bounds = GetCachedBounds(element);

    // Stretch check must run first. If it triggers, the positive check is skipped so that
    // the recovered stretch anchors are not immediately overwritten by a second refresh.
    bool shouldRefreshStretchAnchors =
        ShouldRefreshAnchorInfoForStaleStretchAnchors(element, anchorInfo, bounds, displayRect, anchor);

    if (!shouldRefreshStretchAnchors
        && !ShouldRefreshAnchorInfoForStalePositiveAnchors(anchorInfo, bounds, displayRect, anchor))
    {
        return anchor;
    }

    RefreshAnchorInfoForDisplayRectangleGrowth(element, anchorInfo, displayRect, anchor, shouldRefreshStretchAnchors);

    return anchor;
}
```

**How the repair works:**

- `ShouldRefreshAnchorInfoForStalePositiveAnchors(AnchorInfo, Rectangle bounds, Rectangle displayRect, AnchorStyles)`
  targets controls anchored only to trailing edges (`Right` without `Left`, `Bottom` without
  `Top`). A positive trailing offset on such an axis — combined with evidence that the parent's
  `displayRect` has grown past the recorded `anchorInfo.DisplayRectangle` — is the signature of a
  stale transient capture.
- `ShouldRefreshAnchorInfoForStaleStretchAnchors(IArrangedElement, AnchorInfo, Rectangle bounds, Rectangle displayRect, AnchorStyles)`
  covers the complementary case: a stretch-anchored control (`Left | Right` or `Top | Bottom`)
  whose actual size is smaller than the size predicted by the specified bounds and the growth in
  the parent display rectangle. It uses `GetDisplayRectangleForSpecifiedContainerBounds` to
  reconstruct a stable reference rectangle from the container's `SpecifiedBounds`.
- The **stretch check runs before the positive check** (short-circuit `&&`). This is the key
  guard against the double-refresh: once the stretch path fires for an axis, the positive path
  cannot fire for the same axis in the same layout pass.
- `RefreshAnchorInfoForDisplayRectangleGrowth` delegates to `ResetAnchorInfo`, passing either the
  original captured `DisplayRectangle` (stretch recovery) or a rectangle derived from the
  container's `SpecifiedBounds` (trailing-edge recovery), then recomputes all four anchor values
  so the trailing offsets become stable negative distances again.

| Attribute | Value |
|---|---|
| Risk | Low — additive only, no existing behavior changed |
| Scope | Framework source change (no app change required) |
| Verified | Stale-anchor and stretch-anchor regression tests pass on `net10.0-windows` at 200% DPI |
| Limitation | Only targets the transient-capture class of failure; does not address other DPI layout edge cases |

---

## Relationship Between Paths

```
.NET Framework 4.8
  └─ EnableAnchorLayoutHighDpiImprovements = false (disabled by default)
       └─ Always uses legacy ComputeAnchoredBounds — stable, no DPI-aware guard

.NET 8+ (default, DPI > 100%)
  └─ IsScalingRequirementMet = true
       └─ V1 UpdateAnchorInfo with DPI-aware guard — fires spuriously during construction
            ├─ Solution 2: make IsScalingRequirementMet false (DpiUnaware)
            └─ Solution 3: additive layout-side repair in ComputeAnchoredBounds  ← current fork

.NET 8+ with AnchorLayoutV2 = true
  └─ UpdateAnchorInfoV2 — defers capture until parent layout is stable
       └─ Solution 1: root-cause fix, no source change needed
```

---

## Verification

### Manual Testing

Requires a machine with at least one monitor set to 200% DPI:

1. Launch the application and open a form with bottom/right-anchored controls inside nested containers.
2. **Without any fix:** the control is far below its parent or invisible.
3. **With Solution 1** (`AnchorLayoutV2`): control is correctly positioned inside the parent's client area.
4. **With Solution 2** (`DpiUnaware`): control is correctly positioned, but UI may appear blurry.
5. **With Solution 3** (fork repair): control is correctly positioned; test host requires no config change.

### Automated Test Criteria

For a bottom/right-anchored control inside a nested container at 200% DPI:

- The control's `Bounds` are fully contained within the parent's `ClientRectangle` after `Show()`.
- The control does not overlap sibling controls that were correctly sized.
- For stretch-anchored containers: the container's height matches the size implied by the
  specified bounds and the display-rectangle growth — it is not collapsed by a second refresh.

### Fork Regression Tests

| Test | Status |
|---|---|
| `...V1Path_ShouldRemainWithinGroupBoxAfterFormIsShown` | ✅ trailing-edge stale capture (Solution 3) |
| `...WithAnchorLayoutV2_RemainsWithinGroupBoxAfterFormIsShown` | ✅ V2 path unaffected |
| `...StretchAnchoredGroupBox_RecoveredStretchAnchor_ShouldNotTriggerFollowUpPositiveRefresh` | ✅ stretch double-refresh guard |

No changes to `Control.cs` are required — the fix is entirely within `DefaultLayout.cs` and
`DefaultLayout.AnchorLayoutCompat.cs`.

---

## References

- [Anchor layout changes in .NET 8.0](https://github.com/dotnet/winforms/blob/main/docs/design/anchor-layout-changes-in-net80.md)
- `src/System.Windows.Forms/System/Windows/Forms/Layout/DefaultLayout.cs` — `UpdateAnchorInfo`, `ComputeAnchoredBounds`, `UpdateAnchorInfoV2`
- `src/System.Windows.Forms/System/Windows/Forms/Layout/DefaultLayout.AnchorLayoutCompat.cs` — `TryRefreshAnchorInfoForDisplayRectangleGrowth`, `ShouldRefreshAnchorInfoForStalePositiveAnchors`, `ShouldRefreshAnchorInfoForStaleStretchAnchors`, `RefreshAnchorInfoForDisplayRectangleGrowth`, `ResetAnchorInfo`
- `src/System.Windows.Forms.Primitives/src/System/Windows/Forms/Internals/ScaleHelper.cs` — `IsScalingRequirementMet`
