# Work Order: SystemVisualSettings (Implementation)

**Branch:** `Net11/Integration-2` (KlausLoeffelmann/winforms)
**Scope:** Code changes only. The GitHub proposal issue is handled by a separate work order
(`ApiReview-IssueUpdates.md`, Section 3) and run only after this implementation stands.
**Supersedes:** `Application.GetWindowsAccentColor` and the standalone accessibility text-size change
event currently on the branch (see item 6).

---

## Background

Windows delivers visual/accessibility setting changes through four channels — `WM_SETTINGCHANGE`,
`WM_DWMCOLORIZATIONCOLORCHANGED`, `WM_THEMECHANGED`, `WM_SYSCOLORCHANGE` — and today each consumer
normalizes that zoo individually. This work order introduces one typed, read-only snapshot plus one
unified change notification, with a **leak-free consumption path for controls** (virtual cascade, no
static-event subscription — the `SystemEvents.UserPreferenceChanged` leak class must be structurally
impossible for the default path).

Deliberate non-goals, enforced by the type system: **no settable properties.** Perceptual adjustments
(contrast, color filtering, text scale, focus prominence, motion) are user-owned via Windows
accessibility settings; app-side theming is the business of control vendor partners. Renderers derive
output from this snapshot combined with `EffectiveVisualStylesMode`.

---

## 1. New types (`System.Windows.Forms`)

```csharp
public sealed class SystemVisualSettings
{
    public Color AccentColor { get; }
    public float TextScaleFactor { get; }                 // 1.0–2.25, Windows a11y text scale
    public bool HighContrastEnabled { get; }
    public bool ClientAreaAnimationEnabled { get; }       // SPI_GETCLIENTAREAANIMATION
    public bool KeyboardCuesVisible { get; }              // SPI_GETKEYBOARDCUES (system default)
    public Size FocusBorderMetrics { get; }               // SPI_GETFOCUSBORDERWIDTH/HEIGHT, pixels
}

[Flags]
public enum SystemVisualSettingsCategories
{
    None = 0,
    AccentColor = 1 << 0,
    TextScale = 1 << 1,
    HighContrast = 1 << 2,
    Animations = 1 << 3,
    KeyboardCues = 1 << 4,
    FocusMetrics = 1 << 5
}

public class SystemVisualSettingsChangedEventArgs : EventArgs
{
    public SystemVisualSettings OldSettings { get; }
    public SystemVisualSettings NewSettings { get; }
    public SystemVisualSettingsCategories Changed { get; }
}
```

Immutable snapshot semantics — values must not shift under an event handler comparing old vs new.
XML remarks per the design notes: `HighContrastEnabled` cross-references the effective-mode clamp;
`KeyboardCuesVisible` documents the system-default vs per-window (`WM_UPDATEUISTATE`) distinction;
`FocusBorderMetrics` is documented as the baseline input for border/focus prominence in renderers
(scaled for DPI and `TextScaleFactor`), replacing fixed constants; the class remarks state the
no-app-side-overrides principle explicitly.

## 2. `Application` surface

```csharp
public static SystemVisualSettings SystemVisualSettings { get; }
public static event EventHandler<SystemVisualSettingsChangedEventArgs>? SystemVisualSettingsChanged;
```

Event XML remarks must state: (a) raised once per settings transition, normalized across the four
underlying messages; (b) handlers should early-out via `e.Changed`; (c) **audience is app-lifetime
consumers** (theming engines, services); components with shorter lifetime than the application must
unsubscribe; **controls and forms should use the `Control`-level virtual/instance event instead,
which requires no unsubscription** (see item 3). This positioning is the leak fix — make the
leak-free path the documented default.

## 3. `Control`-level consumption (the leak-free path)

```csharp
protected virtual void OnSystemVisualSettingsChanged(SystemVisualSettingsChangedEventArgs e);
public event EventHandler<SystemVisualSettingsChangedEventArgs>? SystemVisualSettingsChanged;
```

- Model the cascade on the existing `OnSystemColorsChanged` pattern in `Control.cs`: virtual
  dispatch parent→children, instance event raised from within the virtual. No subscription to any
  static event exists anywhere in this path — lifetime coupling is structural.
- Keep the cascade pattern-identical to `OnVisualStylesModeChanged` / `OnParentVisualStylesModeChanged`
  (see the VisualStylesMode impact work order). A `HighContrast` category change resolves as an
  effective-visual-styles-mode change for affected controls and must route through that machinery's
  early-out/dispatch — no duplicate HC handling in this cascade.
- **Remove/replace the existing Form-level replicated text-size event**: it generalizes into this
  cascade and disappears as a special case. Migrate in-box usages.
- Staleness rule (document in XML remarks, mirroring `OnSystemColorsChanged` folklore — this time
  written down): the cascade only reaches parented controls; a control created but not yet parented
  misses transitions and must re-query `Application.SystemVisualSettings` on handle creation /
  `OnParentChanged`.

## 4. Message plumbing and normalization

Central internal tracker (e.g. `SystemVisualSettingsTracker`) holding the current snapshot:

- Every **top-level** window already receives the four raw messages; handle them in the existing
  top-level `WndProc` paths.
- On receipt: the window asks the tracker to re-query. The **first** arriver computes the diff
  against the current snapshot, atomically swaps it (`Interlocked` reference swap), raises the static
  `Application` event **once**, and cascades into its own tree. Subsequent top-levels re-query, see
  no diff, raise nothing at Application level, but **still cascade into their own trees** using the
  already-computed args.
- Threading falls out for free: each tree is notified on the thread owning its top-level — no
  marshaling, correct for multi-message-loop applications. Do not centralize onto one thread.
- Coalesce message storms: a single user action can produce several of the four messages; debounce
  within a message-pump iteration (re-query once per burst per top-level, not per message).

## 5. In-box consumption cleanup

- Audit in-box `Microsoft.Win32.SystemEvents` subscriptions (`UserPreferenceChanged` et al.) in
  controls/renderers; migrate those covered by the new categories to the cascade. Document any that
  must remain (categories outside this surface) — do not expand the snapshot to chase them in this
  work order.
- `TextBoxBase` Net11 border rendering: consume `FocusBorderMetrics` + `TextScaleFactor` as the
  border-prominence input where fixed constants are currently used (coordinate with the animation /
  focus-indicator renderer as applicable).
- Animated renderers (`AnimatedControlRenderer` / `AnimationManager`): honor
  `ClientAreaAnimationEnabled == false` by rendering final state immediately and suppressing
  transitions; react to the `Animations` category change at runtime.

## 6. Supersede the piecemeal APIs

- `Application.GetWindowsAccentColor` → `Application.SystemVisualSettings.AccentColor`. The method
  has not shipped stable: **remove it** on this branch (preferred) rather than obsoleting, to avoid
  two sources of truth. If removal is blocked by preview-compat policy, `[Obsolete]` with pointer.
- The standalone text-size change event (Application- and/or Form-level) → `Changed.HasFlag(TextScale)`
  on the unified event / cascade. Same removal-vs-obsolete decision, same preference.
- Migrate all in-box call sites.

## 7. Tests

- Snapshot immutability and correct SPI mapping per property (mock/native-shim as the repo's test
  infra allows).
- Normalization: N top-level windows + one settings transition ⇒ exactly one Application-level raise;
  every window's tree cascaded exactly once; delivery on each tree's own thread.
- Flags correctness per category, including multi-category transitions (HC toggle typically changes
  colors + HC + metrics in one burst — must coalesce to one event with combined flags).
- Leak test: create/dispose forms subscribing to the **control-level** event in a loop; assert
  collectability (`WeakReference`), proving no static rooting. Counter-test documenting that the
  static event does root (expected, documented behavior).
- HC toggle end-to-end: cascade triggers effective-mode change path once, no duplicate layout.

## Constraints

- All new public surface XML-documented in the branch's voice; internal tracker fully internal.
- No settable members anywhere on the new types — if implementation pressure suggests one, stop and
  flag rather than adding it.
- Windows-only P/Invoke via `LibraryImport`, gated per existing repo practice.
