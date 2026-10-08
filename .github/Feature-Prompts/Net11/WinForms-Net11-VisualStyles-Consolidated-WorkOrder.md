# Consolidated Work Order: WinForms .NET 11 — Remaining VisualStylesMode Items

**Branch:** `Net11/Integration-3` (KlausLoeffelmann/winforms)

**Already implemented on this branch — consume, do not build:** the VisualStylesMode change-impact
model (`VisualStylesModeChangeImpact`, `protected virtual GetVisualStylesModeChangeImpact(old, new)`,
base `OnVisualStylesModeChanged` dispatcher with effective-equality early-out and coalesced
`LayoutTransaction` cascade, `protected EffectiveVisualStylesMode`), the `SystemVisualSettings`
surface (snapshot, `SystemVisualSettingsCategories`, unified `Application` event, leak-free
`Control.OnSystemVisualSettingsChanged` cascade), and the `HighPrecisionTimer` rework. Read their
current source before starting — do not re-derive their shapes from this document.

## Commit discipline

One coherent commit (or small series) per phase, in phase order. Bug fixes and docs/chores get
their own commits. No cross-phase mixing, no squashing phases together.

## Global constraints (every phase)

- **No new settable style/theming properties.** If implementation pressure suggests one
  (`CaptionFont`, `CornerRadius`, `HeaderColor`, …), stop and flag. All colors derive from mode
  palette, dark mode, system accent, High Contrast.
- No shadows/blur/elevation (per-paint GDI+ cost).
- High Contrast needs no per-feature handling: the `EffectiveVisualStylesMode` clamp resolves HC to
  `Classic` everywhere.
- Shared renderer constants (border color family, radii, stroke widths) live in **one** internal
  location — duplication is a review blocker.
- Follow repo conventions, analyzers, and the branch's XML-doc voice.

---

# Phase 1 — GroupBox Net11 FlatStyles

`FlatStyle` becomes the mode-gated renderer selector (precedent: the Net11 Button treatment).

| FlatStyle | Classic / HC / Disabled | Net11+ |
|---|---|---|
| `Standard` | classic etched frame | **Card**: caption above frame; rounded filled card, subtly elevated surface (Win11 settings-card archetype) |
| `Flat` | classic flat | **Outline**: rounded stroke, transparent body, caption inline on the border line |
| `Popup` | classic popup | **Header band**: filled accent-tinted title bar, body below, firm border (vendor "TitledPanel" archetype for POS migration) |
| `System` | native `BS_GROUPBOX` | native — classic regardless of mode; document explicitly |

### 1.1 Derived caption font — keystone, no API
Caption rendered with a paint-time-derived font: ambient `Font` × ~1.15, `SemiBold` (named internal
constant; tune against the Win11 reference). `Font` is **never modified** — children inherit
nothing; the ambient-font problem is structurally avoided. Multiplicative derivation ⇒ system
text-scale changes move the caption proportionally; wire the `TextScale` category via
`OnSystemVisualSettingsChanged` ⇒ `Metrics` impact. Cache the derived font (recreate on
`Font`/text-scale/DPI change); no per-paint allocation. Migration doc note: manually-bolded
GroupBox fonts double up under Net11 — guidance, not code (see phase 4).

### 1.2 `DisplayRectangle` — explicit compat decision
Card style's caption-above-frame moves the content origin down and shrinks the inner area vs
classic. Accepted: metric changes are what opt-in means, and correct AutoSize layouts re-measure
via the impact model. Compute `DisplayRectangle` per style/mode from actual renderer metrics — no
constants duplicated between paint and layout code. Record in the high-risk review doc: this is the
biggest compat lever in the change.

### 1.3 Impact model integration
Override `GetVisualStylesModeChangeImpact`: `Classic`/`Disabled` ↔ `>= Net11` ⇒ `Metrics`; within
`>= Net11` decide from actual renderer differences and document in the override's remarks. Runtime
`FlatStyle` switches under Net11 with differing metrics must route layout equivalent to `Metrics` —
verify the `FlatStyle` setter path requests layout, not just repaint.

### 1.4 Rendering + A11y
`RightToLeft` respected in all three renderers. Popup band: accent-derived fill (never settable),
luminance-based readable foreground, reacts to the `AccentColor` category (`Repaint`). Disabled
state desaturated per mode conventions. Set `UIA_HeadingLevelPropertyId` (fixed level, e.g.
`HeadingLevel2` — **no nesting-depth heuristic**) under Net11 styles; decide/document/test HC
behavior (recommendation: retain heading semantics; verify no double-announcement with the
grouping role).

### 1.5 Tests
`DisplayRectangle` goldens per (`FlatStyle` × mode × DPI × text scale); live text-scale change ⇒
caption grows, `Metrics` fires, hosting AutoSize `TableLayoutPanel` row re-measures without control
recreation; `Font` never mutated (assert pre/post paint and post text-scale); runtime FlatStyle
switch ⇒ layout, not just repaint; UIA heading assertions; dark/accent snapshots; `System` renders
identically pre/post mode switch.

---

# Phase 2 — ComboBox Net11 FlatStyles

**Architectural starting point:** `FlatStyle.Flat`/`Popup` have routed through the WinForms-internal
`FlatComboAdapter` since 1.x — field, border, drop-button fully WinForms-painted, native theming
bypassed. The Net11 renderer is a **new adapter next to `FlatComboAdapter`**; extend the existing
FlatStyle-based adapter selection to be mode-gated.

| FlatStyle | Classic / HC / Disabled | Net11+ |
|---|---|---|
| `Standard` | native themed | **Rounded field** matching `TextBoxBase` Net11 (newly routes `Standard` through the adapter under Net11 — record in high-risk doc) |
| `Flat` | classic flat adapter | **Underline**: borderless field, bottom accent underline on focus |
| `Popup` | flat-until-hover | borderless at rest, rounded border on hover/focus |
| `System` | native | native — classic regardless of mode; document |

### 2.1 Field metrics — TextBoxBase parity is a requirement
Share border constants with the `TextBoxBase` Net11 renderer (extract to a common internal location
if not already shared). **Height parity:** single-line Net11 `TextBox` and Net11 `ComboBox` (all
`DropDownStyle`s except `Simple`) land at the same height for the same `Font`/DPI — golden test.
If `ItemHeight`-driven sizing conflicts with parity padding, **resolve in favor of parity** and
document. Drop-button: scalable drawn chevron per DPI (no bitmap, no Marlett); hover/pressed states
from the mode palette. `RightToLeft`: mirrored chevron and text alignment.

### 2.2 Dropdown list (popup HWND)
Rounded corners via `DWMWA_WINDOW_CORNER_PREFERENCE` (`DWMWCP_ROUNDSMALL`) on the list HWND
(`GetComboBoxInfo` → `hwndList`); apply on dropdown creation; gate on Win11 build 22000+, silent
no-op below. Interior colors: verify the existing dark-mode item-draw path keys off the Net11
palette — no second mechanism. `Simple`: embedded child, corner preference N/A, adapter draws the
composite border. **Verify the full 9-cell `DropDownStyle` × modern-`FlatStyle` matrix** —
`Simple` is where combo adapters historically rot.

### 2.3 EDIT child (`DropDownStyle.DropDown`)
Existing dark-mode EDIT-child handling composes with Net11 field padding so text baseline and left
inset match `DropDownList` (adapter-painted) and `TextBoxBase` — baseline-alignment golden across
the three. No NC treatment on the child; the adapter owns the frame.

### 2.4 Impact model integration
`Classic`/`Disabled` ↔ `>= Net11` ⇒ `Metrics` if the parity work changes preferred size across the
boundary (expected yes — verify, document). Runtime FlatStyle switches with metric differences
route layout. `AccentColor` category ⇒ `Repaint` (focus underline, chevron accent states).

### 2.5 Tests
Adapter-selection matrix (`FlatStyle` × `EffectiveVisualStylesMode`); `System` provably untouched
pre/post mode switch; height/baseline goldens per `DropDownStyle` × DPI × text scale; 9-cell
snapshots light + dark; corner preference Win11+/no-op-below; live mode switch in an AutoSize TLP
row re-measures without recreation; RTL snapshot for `Standard`.

---

# Phase 3 — Bug fixes

### 3.1 Issue #14754 — `FlatStyle.System` + `Appearance.ToggleSwitch` + Net11 throws
An existing commit on this branch fixes the exception — locate it, include it, reference the issue
in the commit message. Beyond the crash, **define the behavior**: `System` (native rendering)
cannot draw a toggle switch. Decision (default unless the existing commit already decided
otherwise): **`Appearance` wins** — `ToggleSwitch` requires owner rendering by definition, so the
control renders via the WinForms toggle renderer as if `Standard`; document in both properties'
XML remarks. (Rejected alternative: `System` wins and silently renders a classic checkbox — makes
a set property a silent no-op, worse than the exception for debuggability.) Add a matrix smoke
test: **no** `FlatStyle × Appearance × VisualStylesMode` combination may throw.

### 3.2 `VisualStylesMode.Inherit` serialization
Bug: the CodeDom serializer reads the public getter, which resolves the ambient value — `Inherit`
is never observable there, so a designer reset momentarily shows `Inherit` and then serializes the
*resolved* value. Fix with the classic ambient pattern operating on the **raw stored** value, never
the resolved getter:

- `private bool ShouldSerializeVisualStylesMode()` ⇒ raw value set and ≠ `Inherit`.
- `private void ResetVisualStylesMode()` ⇒ raw value := `Inherit` (single assignment; no transient
  resolve-and-store anywhere in the reset path).
- Ensure no code path writes a resolved value back into raw storage during reset or designer
  refresh.
- Tests: CodeDom serialization emits nothing for `Inherit`; emits the explicit value otherwise;
  reset in a designer-host test returns the property to ambient with no serialized line; designer
  round-trip preserves `Inherit`-ness.

---

# Phase 4 — Documentation

### 4.1 Placement architecture
- **XML remarks carry the contract; one conceptual doc carries the guidance.** Create
  `docs/net11-visualstyles-layout-guidance.md` (name/location per repo docs conventions); every
  affected `FlatStyle`/`VisualStylesMode`/`Appearance` remark links to it. Do **not** duplicate the
  guidance essay across the affected controls' properties.
- **Mechanism for long docs:** **C# 13 partial properties** — defining declarations carrying the
  extensive XML docs in dedicated `*.Docs.cs` partial files (e.g.
  `Control.VisualStylesMode.Docs.cs`), implementations in the main files. Class-level docs likewise
  on a dedicated partial declaration. Docs on the defining declaration only.

### 4.2 Contract content for the XML remarks (per affected control, kept tight)
- Certain `VisualStylesMode` settings **will** change adornment real-estate requirements without
  compromising the client area — **the control gets bigger** when the switch flips. Therefore
  `TableLayoutPanel`/`FlowLayoutPanel` are the recommended containers for controls in these modes.
  This is deliberate: A11Y-driven sizing (an 8×8 check glyph is not reasonable in the 4K/200% era),
  and size reaction to system text-scale changes is unavoidable anyway — the same containers
  handle both.
- **Exception — fixed-bounds controls:** `RichTextBox` and multiline `TextBox` have no intrinsic
  `IntegralHeight`/AutoSize behavior. Switching to a mode needing more adornment real estate does
  **not** resize them; bounds stay, which means the usable client area shrinks slightly at
  constant bounds. They must be programmatically aligned based on context — **and that is
  intended**. Controls with intrinsic AutoSize (`Button`, single-line `TextBox`, `Label`) or with
  `AutoSize` set re-layout and trigger re-layout automatically.
- Where repo docs conventions allow forward references: note that Copilot/agent tooling assists
  the migration, and reference the (planned) agent skills for HighDPI layouting, A11Y,
  text-scale-adaptive layouts, and pixel-perfect→fluent refactoring.

### 4.3 Conceptual doc content (the guidance essay)
- **Anchor vs. Dock in cascading layouts** — the teachable rule: **anchor when consuming/aligning
  size** (stretch within a cell whose size others determine), **dock when pushing size** (the
  child's content drives the parent). Worked examples inside `TableLayoutPanel` and in
  constituent-layout situations.
- **The equal-size OK/Cancel pattern** as the flagship example — guaranteed today with the right
  `TableLayoutPanel` approach: AutoSize/GrowAndShrink panel, two 50% columns, AutoSize buttons
  anchored `Left | Right` — both buttons always get the width of the wider preferred size. Include
  the sample below (**corrected from the draft**: `_btnCancel.DialogResult` was `DialogResult.OK`,
  fixed to `Cancel`; `CellBorderStyle = Inset` removed as a cell-visualization leftover — neither
  belongs in a canonical sample that agents will replicate at scale):

```csharp
_btnOK = new Button();
_btnCancel = new Button();
_tlpDialogResultButtons = new TableLayoutPanel();
_tlpDialogResultButtons.SuspendLayout();
SuspendLayout();
//
// _btnOK
//
_btnOK.Anchor = AnchorStyles.Left | AnchorStyles.Right;
_btnOK.AutoSize = true;
_btnOK.AutoSizeMode = AutoSizeMode.GrowAndShrink;
_btnOK.DialogResult = DialogResult.OK;
_btnOK.Name = "_btnOK";
_btnOK.Padding = new Padding(14, 0, 14, 0);
_btnOK.TabIndex = 0;
_btnOK.Text = "OK";
_btnOK.UseVisualStyleBackColor = true;
//
// _btnCancel
//
_btnCancel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
_btnCancel.AutoSize = true;
_btnCancel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
_btnCancel.DialogResult = DialogResult.Cancel;
_btnCancel.Name = "_btnCancel";
_btnCancel.Padding = new Padding(14, 0, 14, 0);
_btnCancel.TabIndex = 1;
_btnCancel.Text = "Cancel";
_btnCancel.UseVisualStyleBackColor = true;
//
// _tlpDialogResultButtons
//
_tlpDialogResultButtons.AutoSize = true;
_tlpDialogResultButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
_tlpDialogResultButtons.ColumnCount = 2;
_tlpDialogResultButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
_tlpDialogResultButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
_tlpDialogResultButtons.Controls.Add(_btnCancel, 1, 0);
_tlpDialogResultButtons.Controls.Add(_btnOK, 0, 0);
_tlpDialogResultButtons.Name = "_tlpDialogResultButtons";
_tlpDialogResultButtons.RowCount = 1;
_tlpDialogResultButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
_tlpDialogResultButtons.TabIndex = 3;
```

- Treat the OK/Cancel sample as **exemplary, not exhaustive**: identify comparable
  customer-information-deficit spots (mixed TextBox/ComboBox row alignment, GroupBox card
  `DisplayRectangle` implications, text-scale-driven growth in TLP AutoSize rows) and add
  equivalent worked samples where they plausibly prevent support traffic.

---

# Phase 5 — GitHub chores

Run each item only when the corresponding implementation stands. Ask the user for concrete issue
numbers/URLs — do not guess. Amendments are additions clearly marked as such; match each issue's
formatting. Expected-Questions blocks are decisions with rationale — include **as written**, do not
soften into open questions.

### 5.1 GroupBox + ComboBox issue notes
Add sections (or a shared note, per existing issue structure) documenting the mode-gated FlatStyle
renderings as behavioral changes under opt-in mode (not new API): the taxonomy tables, the
`DisplayRectangle` decision (GroupBox), the `Standard`-through-adapter note (ComboBox), the
`System`-stays-native statements, and the #14754 behavior decision (`Appearance` wins over
`FlatStyle.System` for `ToggleSwitch`; never throw).

EQ block:
- *Isn't this theming?* No new settable surface exists; all three looks are framework-defined
  renderings of an existing enum under an opt-in mode; colors/metrics derive from system state
  (accent, dark mode, HC, text scale). Theming remains the business of control vendor partners.
- *Why does `System` not modernize?* It means native rendering by contract; native
  `BS_GROUPBOX`/native combo cannot be restyled. Documented, not overlooked.
- *Why does the GroupBox card change `DisplayRectangle`?* Metric changes are what `VisualStylesMode`
  opt-in means; correct AutoSize layouts re-measure through the change-impact model; recorded as
  the change's biggest compat lever in the high-risk doc.

### 5.2 "What's new in WinForms .NET 11" issue
Create an issue whose **description is the markdown itself** — the single source later lifted into
blog post, docs, and readme. Sections: `VisualStylesMode` / `EffectiveVisualStylesMode` /
`SetDefaultVisualStylesMode` (opt-in story, HC guarantee); the change-impact model for control
authors; `SystemVisualSettings` (snapshot, unified event, leak-free control path, vendor-plumbing
pitch); modernized control renderings per FlatStyle (Button, TextBoxBase, GroupBox incl. UIA
heading levels, ComboBox incl. rounded dropdown); layout guidance pointer (phase 4 conceptual doc);
notable fixes (#14754, `Inherit` serialization). Tone: factual, links to issues/PRs per entry; no
marketing prose — the blog post adds that layer later.
