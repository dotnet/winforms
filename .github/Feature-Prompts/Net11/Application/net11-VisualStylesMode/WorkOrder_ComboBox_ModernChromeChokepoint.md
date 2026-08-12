# Work-Order: Centralize ComboBox Modern Chrome State into an Idempotent Chokepoint

**Area:** WinForms Runtime — `VisualStylesMode` / .NET 11 visual modernization
**Component:** `System.Windows.Forms.ComboBox` (modern `FlatStyle` rendering)
**Branch baseline:** `KlausLoeffelmann/winforms` @ `d6ca49640903fa4f15be20b427f253bb940855d4` (`Net11/VisualStylesMode-Fixes`, rebased on `Net11/Integration-3`, upstream draft PR dotnet/winforms#14768)
**Type:** Rearchitecture (bug-class elimination), supersedes point-fix follow-ups to the "1-pixel internal padding" work-order.

---

## 1. Problem Statement

The modern ComboBox chrome (`UsesModernComboAdapter == true`) mutates **three independent pieces of native state**:

1. `CB_SETITEMHEIGHT(-1)` — the native selection-field height,
2. `EM_SETMARGINS` on the child edit HWND,
3. `SetWindowPos` on the child edit (and, for `ComboBoxStyle.Simple`, the child list) HWND.

Each mutation has its own capture/restore bookkeeping (`_modernEditBaseBounds`, `_modernEditBaseClientSize`, `_modernSimpleListBaseBounds`, `_modernFieldHeightApplied`, `_modernHandleInitialized`), and the mutations are applied from **many triggers** (`OnVisualStylesModeChanged`, `OnHandleCreated`, `OnFontChanged`, `OnPaddingChanged`, `OnSystemVisualSettingsChanged`, `RescaleConstantsForDpi`, `WM_SETFONT` on the child, `WM_WINDOWPOSCHANGED` for Simple, `UpdateItemHeight`, the `FlatStyle` setter) in trigger-specific orderings that depend on what `base.*` does in between.

This produces an entire **class** of state-desynchronization bugs. Four confirmed instances against the pinned baseline:

### Defect A — Vertical/horizontal inset asymmetry (edit overshoots the rounded border)
`UpdateModernEditMargins` applies `Fixed3DBorderPadding (2) + InternalChromeInset (2) + ComboBoxStyleInset (1)` = 5 logical px **horizontally** via `EM_SETMARGINS`, but `UpdateModernEditBounds` insets the edit **vertically** by only `ComboBoxStyleInset (1) + Padding.Top/Bottom`. The rounded border ring consumes ~3 px vertically (Fixed3D zone + AA stroke), so with `Padding.Empty` the edit client overlaps the border curve. `Padding = 2` masks it, which confirms the diagnosis.

### Defect B — `EM_SETMARGINS` never reset on modern → classic
In `ComboBox.OnVisualStylesModeChanged`, `RestoreAndResetModernEditBounds()` empties `_modernEditBaseBounds` **before** the trailing `UpdateModernEditMargins()` call; that method's guard `(!UsesModernComboAdapter && _modernEditBaseBounds.IsEmpty)` then early-returns, so the classic edit child keeps the 5 px modern margins.

### Defect C — Restore targets `COMBOBOXINFO.rcItem`, not the edit's native bounds
The DropDown path seeds `_modernEditBaseBounds` from `rcItem` (the combo's selection-field rect) but restores the **edit HWND** to that rect. The edit natively sits *inside* `rcItem` with a theme inset. The Simple path already does this correctly via `GetChildBounds(comboBoxInfo.hwndItem)`; the DropDown path does not.

### Defect D — Delta-based `CB_SETITEMHEIGHT` is order-fragile and non-idempotent
`ApplyPreferredFieldHeight` computes `heightDelta = preferredHeight - Height` and applies it **relative** to the current item height. Before this runs, `Control.OnVisualStylesModeChanged` (`Metrics` impact) has already executed `RequestLayout`, `UpdateStyles`, and `RefreshVisualStylesModeNonClientArea` (SetWindowPos w/ `SWP_FRAMECHANGED`). Whenever any of these reconciles `Height` first — anchored controls, `TableLayoutPanel` cells, designer behavior service, or **deserialization**, where the mode transition occurs before handle creation and `Height` is already set to the target — the delta collapses to `0` and the native selection-field height silently keeps the previous mode's value. Because the math is relative, each round-trip can accumulate error. This is the primary suspect for the classic missing drop-down button and the misplaced button/frame remnants after classic → modern round-trips, and it fully explains why every deserialization pass re-triggers the corruption.

---

## 2. Target Architecture

Replace the trigger-specific mutation choreography with **one idempotent chokepoint**, mirroring the `EffectiveVisualStylesMode` chokepoint pattern already established for the .NET 11 work: many inputs, one authoritative computation, one applier.

### 2.1 Core principle

> The chokepoint computes the complete **target native state absolutely** from current inputs and applies it. It never reads back its own previous output to decide the next one, and calling it N times in any order after any trigger yields the same native state as calling it once.

There is no separate "restore" path: applying the chokepoint while `UsesModernComboAdapter == false` *is* the restore, because the target state for classic mode is the captured native baseline.

### 2.2 New shape (in `ComboBox.Modern.cs`)

```csharp
/// <summary>
///  Captures the native, unmodified child layout of the ComboBox once per handle
///  lifetime, before any modern chrome mutation is applied.
/// </summary>
private readonly struct NativeComboBaseline
{
    public int SelectionFieldItemHeight { get; init; }
    public Rectangle EditBounds { get; init; }
    public Rectangle SimpleListBounds { get; init; }
    public Size ClientSize { get; init; }

    public bool IsCaptured
        => !EditBounds.IsEmpty || SelectionFieldItemHeight > 0;
}

/// <summary>
///  The complete target state for the ComboBox's native children under the
///  current effective visual styles mode.
/// </summary>
private readonly struct ModernComboTargetState
{
    public int SelectionFieldItemHeight { get; init; }
    public Padding EditMargins { get; init; }
    public Rectangle EditBounds { get; init; }
    public Rectangle SimpleListBounds { get; init; }
}

/// <summary>
///  Single chokepoint. Computes the absolute target state for item height,
///  edit margins, and child bounds from the current inputs and applies it.
///  Idempotent; safe to call from any trigger in any order.
/// </summary>
private void ApplyModernComboLayout();
```

### 2.3 Rules

1. **Baseline capture happens exactly once per handle lifetime**, in `OnHandleCreated`, *before* the first mutation: native default `CB_GETITEMHEIGHT(-1)`, `GetChildBounds(hwndItem)` (fixes Defect C — use child bounds, never `rcItem`), `GetChildBounds(hwndList)` for Simple, and the `ClientSize` at capture time. Cleared in `OnHandleDestroyed`.
2. **Item height is computed absolutely** (fixes Defect D):
   - Modern: derive the target selection-field height from `ModernPreferredHeight` minus the native frame, measured against the **HWND** (`GetWindowRect`), never against `Control.Height` bookkeeping.
   - Classic: target = `NativeComboBaseline.SelectionFieldItemHeight`.
   - Apply only when the current native value differs (read-compare-write, so repaint/relayout storms are avoided while idempotency is preserved).
3. **Edit margins are computed absolutely**: modern = the existing `Fixed3DBorderPadding + InternalChromeInset + ComboBoxStyleInset` formula plus `Padding.Left/Right`; classic = `0` (fixes Defect B — restore is just "apply classic target").
4. **Vertical edit insets use the same chrome formula as the horizontal margins** (fixes Defect A): `topInset`/`bottomInset` = scaled `Fixed3DBorderPadding + ComboBoxStyleInset` (+ `Padding.Top/Bottom`), sourced from a single `GetModernChromeInsets()` helper so the border geometry (`CreateFieldPath`, `ClearNativeFrame`) and the child layout can never diverge again. If the "≥ 1 px internal padding" requirement from the previous work-order needs adjusting, it is adjusted **here, in one place**.
5. **All triggers call only the chokepoint.** `OnVisualStylesModeChanged`, `OnHandleCreated`, `OnFontChanged`, `OnPaddingChanged`, `OnSystemVisualSettingsChanged`, `RescaleConstantsForDpi`, child `WM_SETFONT`, `WM_WINDOWPOSCHANGED` (Simple), `UpdateItemHeight`, and the `FlatStyle` setter each reduce to: invalidate caches as needed → `ApplyModernComboLayout()`. Ordering relative to `base.*` calls no longer matters, because the chokepoint reads only current inputs.
6. **Reentrancy guard lives inside the chokepoint** (`_adjustingModernChildBounds` moves in; callers never manage it).
7. **Delete** `RestoreModernEditBounds`, `RestoreAndResetModernEditBounds`, `ResetModernEditBounds`, `ResizeModernSimpleBaseBounds`, `UpdateModernEditMargins`, `UpdateModernEditBounds`, `UpdateModernSimpleEditBounds`, `ApplyPreferredFieldHeight`'s delta logic, and the fields `_modernEditAppliedBounds`, `_modernEditBaseBounds`, `_modernEditBaseClientSize`, `_modernFieldHeightApplied`, `_modernSimpleListBaseBounds`. Replaced by `NativeComboBaseline` + the chokepoint.
8. `WinFormsApplicationBuilder`/host concerns are out of scope; this is runtime-internal. Public API surface is unchanged (`FlatStyle`, `Padding`, `PreferredHeight` semantics stay as shipped in Integration-3).

### 2.4 Explicit non-goals

- No changes to `ModernComboAdapter` painting itself except consuming `GetModernChromeInsets()` for its geometry constants.
- No changes to the `EffectiveVisualStylesMode` propagation in `Control` — the chokepoint must work with the existing `Metrics`/`Repaint` impact model as-is.
- No behavioral change for `FlatStyle.System` (remains fully native) or for classic `Flat`/`Popup` (existing `FlatComboAdapter`).

---

## 3. Tasks

| # | Task | Notes |
|---|------|-------|
| 1 | Introduce `NativeComboBaseline` capture in `OnHandleCreated`; clear in `OnHandleDestroyed`. | Capture **before** dark-mode theming and before any `CB_SETITEMHEIGHT`. |
| 2 | Introduce `GetModernChromeInsets()` shared by adapter geometry and child layout. | Single source of truth for Defect A. |
| 3 | Implement `ComputeModernComboTargetState()` (pure, absolute). | Inputs: `EffectiveVisualStylesMode`, `FlatStyle`, `DropDownStyle`, `Font`/`FontHeight`, `Padding`, `DeviceDpiInternal`, `SystemVisualSettings`, `RightToLeft`, baseline. |
| 4 | Implement `ApplyModernComboLayout()` applier with read-compare-write per state item and internal reentrancy guard. | Item height via HWND-measured frame, not `Control.Height`. |
| 5 | Reroute all triggers listed in Rule 5 through the chokepoint; delete legacy methods/fields per Rule 7. | `OnVisualStylesModeChanged` body shrinks to cache invalidation + `base` + chokepoint + `RefreshModernDropDownCornerPreference`. |
| 6 | Verify `DrawMode.OwnerDrawFixed/Variable` interplay: `UpdateItemHeight`'s explicit `CB_SETITEMHEIGHT` calls must not fight the chokepoint. | Owner-draw item heights own indices ≥ 0; chokepoint owns index −1 in Normal mode; define precedence for owner-draw −1. |
| 7 | Add a debug assertion helper that dumps `CB_GETITEMHEIGHT(-1)`, `COMBOBOXINFO.rcItem/rcButton`, and edit HWND bounds — used for the acceptance runs below. | Also resolves the open question whether the missing classic arrow is a Defect D consequence (degenerate `rcButton`) or an overlap. |
| 8 | Consider a `WFCC` `ComponentChange` entry if any observable classic-mode behavior shifts (e.g., corrected edit inset after round-trip). | Only if acceptance runs surface an observable delta vs. .NET 10 classic. |

---

## 4. Acceptance Criteria

1. **Round-trip invariance:** `Classic → Net11 → Classic` repeated 10× (runtime *and* designer-hosted) yields native state byte-identical to a freshly created classic ComboBox: item height, edit HWND bounds, `EM_GETMARGINS`, `rcButton` non-degenerate, drop-down arrow visible.
2. **Order independence:** Deserialization sequences that set `FlatStyle`/`Padding`/`Font`/mode in any order, before or after handle creation, converge to the same state. Specifically: mode transition before handle creation must not zero out the item-height adjustment (Defect D repro).
3. **Layout-container safety:** Combo inside `TableLayoutPanel` and with `Anchor = Top|Bottom` survives mode round-trips without frame remnants or misplaced button (Screenshot 4/5 repro).
4. **No overshoot at `Padding.Empty`:** the edit client rect is fully contained within the rounded field path at 100 %, 150 %, 200 % DPI (Screenshot 1 repro). `Padding` values add on top and never subtract.
5. **Simple style:** resize + mode round-trips keep edit/list stacked correctly; no drift of `_modernSimpleListBaseBounds`-equivalent state.
6. **RTL:** all of the above with `RightToLeft.Yes`.
7. **Idempotency probe:** calling `ApplyModernComboLayout()` 5× consecutively produces zero additional native messages after the first call (read-compare-write verified via message trace).
8. Existing modern paint tests (adapter geometry, chevron, `DropDownList` text) pass unchanged.

---

## 5. Risks / Open Questions

- **Native height authority:** for non-Simple styles the native control owns window height derived from item height; the chokepoint must treat `CB_SETITEMHEIGHT(-1)` as the *only* height lever and let `Control.Height` bookkeeping follow via `WM_WINDOWSPOSCHANGED`, never the reverse. Any remaining `Height = preferredHeight` assignments should be audited for redundancy.
- **Owner-draw precedence** (Task 6) needs an explicit decision; proposal: in `DrawMode != Normal`, the chokepoint does not touch index −1 and modern metrics derive the field height from the owner-draw value instead.
- **Classic missing-arrow root cause** is expected to be Defect D but must be confirmed via the Task 7 dump before closing; if it turns out to be an overlap issue, Task 1's child-bounds baseline already covers it.

---

## 6. References (pinned)

- `ComboBox.cs`, `ComboBox.Modern.cs`, `ComboBox.ModernComboAdapter.cs`, `ComboBox.FlatComboAdapter.cs`, `Rendering/ModernControlVisualStyles.cs` — all at
  `https://github.com/KlausLoeffelmann/winforms/blob/d6ca49640903fa4f15be20b427f253bb940855d4/src/System.Windows.Forms/System/Windows/Forms/...`
- `Control.OnVisualStylesModeChanged` / `VisualStylesModeChangeImpact.Metrics` handling — `Control.cs`, same SHA (~lines 7060–7110).
- Upstream draft PR: dotnet/winforms#14768 (`Net11/Integration-3`).
