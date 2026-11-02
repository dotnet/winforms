# Work Order: VisualStylesMode — Change-Impact API (Implementation)

**Branch:** `Net11/Integration-2` (KlausLoeffelmann/winforms)
**Scope:** Code changes only. GitHub issue updates are handled by a separate work order
(`ApiReview-IssueUpdates.md`, Section 1) and run only after this implementation stands.
**Out of scope:** `HighPrecisionTimer` (separate work order), `SystemVisualSettings` (separate work order —
but see the composition note in item 4, the two cascades share a pattern).

---

## Background

The `VisualStylesMode` API is implemented on this branch. Two gaps motivate this change:

1. **Correctness/ordering:** Base `Control.OnVisualStylesModeChanged` only invalidates, raises the
   event, and cascades — no preferred-size cache invalidation, no layout transaction (unlike
   `OnFontChanged`). Every metric-changing control reimplements a four-step protocol; `TextBoxBase`
   currently runs `LayoutTransaction.DoLayoutIf(...)` **before** `UpdateStyles()`, so containers
   measure against pre-transition metrics — a live mode switch clips/overlaps until controls are
   recreated.
2. **Performance:** A top-level switch triggers one full parent layout per metric-affected control,
   each against partially-transitioned state; repaint-only controls pay costs they don't need.

Fix: a declarative impact model — derived controls state **what** changes, the base class owns
**how and in what order** to react, with one coalesced layout pass per container.

---

## Changes

### 1. `Control.cs` — visibility promotion

`EffectiveVisualStylesMode`: `private protected` → `protected`. **Non-virtual** (deliberate — it is
the accessibility policy enforcement point; the customization hook is the already-virtual
`DefaultVisualStylesMode`). Extend XML docs: this is the value controls must honor for rendering;
reflects High Contrast (⇒ `Classic`) and `Disabled`; cross-reference `DefaultVisualStylesMode`.

### 2. `Control.cs` — impact enum + virtual

```csharp
protected enum VisualStylesModeChangeImpact
{
    None,            // no rendering difference; skip all work
    Repaint,         // client-area rendering only; metrics identical
    NonClientUpdate, // NC frame changes; style/frame update, no size change
    Metrics          // preferred size / layout metrics change; full invalidation + layout
}

protected virtual VisualStylesModeChangeImpact GetVisualStylesModeChangeImpact(
    VisualStylesMode oldMode,
    VisualStylesMode newMode)
    => VisualStylesModeChangeImpact.Repaint;
```

Parameters are **effective** modes. Document: implementations may consult instance state
(`Multiline`, `BorderStyle`, …) but the value must be stable for the duration of the change handling.

### 3. `Control.cs` — setter early-out on effective equality

In the `VisualStylesMode` setter and `OnParentVisualStylesModeChanged`: capture effective mode before
the change; if unchanged after, skip `OnVisualStylesModeChanged` entirely (no invalidate, no cascade
into that subtree). Preserve the existing shadowing behavior (child with explicit local value already
stops the ambient cascade) and add the effective-equality check on top. High Contrast active ⇒ raw
`Net11 → Latest` switch is a complete no-op.

### 4. `Control.cs` — `OnVisualStylesModeChanged` becomes the dispatcher

Preserve the disposal guard and public event raise. Then:

- `impact = GetVisualStylesModeChangeImpact(oldEffective, newEffective)` — plumb old/new effective
  values from the setter/cascade (least-invasive mechanism consistent with how `OnParentFontChanged`
  plumbs state).
- `None` → raise event, skip rendering/layout work, still cascade (children's impact may differ).
- `Repaint` → `Invalidate()`.
- `NonClientUpdate` → `UpdateStyles()` + NC frame refresh (`SetWindowPos` + `FRAMECHANGED`, per the
  branch's existing pattern), then `Invalidate()`.
- `Metrics` → in order: `CommonProperties.xClearPreferredSizeCache(this)` → `UpdateStyles()` / frame
  refresh → layout deferred to the coalesced step:

```csharp
if (ChildControls is { } children)
{
    using (new LayoutTransaction(this, this, PropertyNames.VisualStylesMode, resumeLayout: false))
    {
        for (int i = 0; i < children.Count; i++)
        {
            children[i].OnParentVisualStylesModeChanged(e);
        }
    }
}

LayoutTransaction.DoLayout(this, this, PropertyNames.VisualStylesMode);
```

Add `PropertyNames.VisualStylesMode` if missing.

**Composition note:** the `SystemVisualSettings` work order introduces a structurally identical
parent→child cascade (`OnSystemVisualSettingsChanged`, modeled on `OnSystemColorsChanged`). Keep the
two cascades pattern-identical; a High Contrast toggle arriving via that path resolves here as an
effective-mode change and must take the same early-out/dispatch route — no duplicate handling.

### 5. `TextBoxBase.cs` — adopt the model, delete the hand-rolled protocol

- Override `GetVisualStylesModeChangeImpact`: crossing `Classic`/`Disabled` ↔ `>= Net11` ⇒ `Metrics`
  (`PreferredHeight` selection and NC padding both change). Within `>= Net11` (`Net11 → Latest`) ⇒
  `Repaint` **unless** the branch's renderer shows metric differences between those modes — verify
  against `PreferredHeightCore` and the NC padding tables; document the decision in XML remarks.
- Slim `OnVisualStylesModeChanged`: remove manual `xClearPreferredSizeCache` / `DoLayoutIf` /
  `AdjustHeight` sequencing — base owns it. Keep control-specific work
  (`_focusIndicatorRenderer?.Synchronize(...)`, `_triggerNewClientSizeRequest = false`), with the
  ordering contract: latch reset **before** `base.OnVisualStylesModeChanged(e)` (which runs
  `UpdateStyles()`); any residual `AdjustHeight` need goes **after** `base` — verify with the
  live-switch repro (TableLayoutPanel, AutoSize rows, anchored single-line TextBoxes; runtime mode
  switch; rows must resize without reopening the form).
- This closes the `DoLayoutIf(AutoSize: false)` hole: base-owned coalesced layout means
  `AutoSize == false` TextBoxes no longer skip parent re-measurement.

### 6. Documentation & tests

- `docs/Net11Api_05_VisualStylesMode.HighRiskReview.md`: add the behavioral change (base
  `OnVisualStylesModeChanged` now clears caches, updates styles, requests layout; overriders calling
  `base` inherit this). If the branch's `WFCC`/`ComponentChange` infrastructure is present, annotate;
  otherwise the high-risk doc entry suffices.
- `ControlTests.VisualStylesMode.cs`:
  - effective-equality early-out (HC active ⇒ no event storm, no layout),
  - default impact is `Repaint`,
  - `Metrics` path clears preferred-size cache; exactly one layout per container for a multi-control
    subtree switch (layout-count instrumentation or `LayoutEventArgs` assertion),
  - `TextBoxBase` live-switch regression test: preferred-height change reflected in an AutoSize
    `TableLayoutPanel` row without control recreation.

## Constraints

- No binary breaking changes (`protected` promotion + new `protected` members on unsealed public
  class are additive).
- Do not change the `EffectiveVisualStylesMode` clamping logic.
- Match repo analyzers/style and the branch's XML-doc voice.
