# Task: Create a new API — `Application` system-text-size awareness — and the respective API proposal

## What to do

Create a **new API proposal / API review issue in the upstream `dotnet/winforms` repo**
(`origin` = my fork, `upstream` = the Microsoft repo where this lands). Write it per the
WinForms repo conventions and the relevant skills for authoring new-API issues. Apply the
skills; don't ask me for boilerplate.

This proposal introduces **runtime awareness of the Windows Accessibility text-size
setting** at the `Application` level, plus a per-`Form` change notification. It is the
**foundation** proposal; a companion `TreeView.NodeLeading` proposal references this one.

## Before you implement anything

**Verify every premise below against current source before committing to the design.**
Verify, don't trust. If any premise is wrong, stop and tell me. Key files (VMR @
`96982699e0dd8c046f397541dc0eb235ea8a4958`):

- `src/winforms/src/System.Windows.Forms.Primitives/src/System/Windows/Forms/Internals/ScaleHelper.cs`
- `src/winforms/src/System.Windows.Forms/System/Windows/Forms/Application.cs`
- `src/winforms/src/System.Windows.Forms/System/Windows/Forms/Application.ThreadContext.cs`
- `src/winforms/src/System.Windows.Forms/System/Windows/Forms/Application.ParkingWindow.cs`
- `src/runtime/src/libraries/Microsoft.Win32.SystemEvents/...` (SystemEvents / UserPreferenceCategory)

## Rationale — surface and complete an existing partial implementation

This is **not** a net-new feature; it **finishes a half-built one**. WinForms already reads
the Accessibility text-size setting, but only once and only for the default font:

`ScaleHelper.ScaleToSystemTextSize(Font?)` reads
`HKEY_CURRENT_USER\Software\Microsoft\Accessibility` → value `TextScaleFactor`
(REG_DWORD, clamped 100–225), and returns the font scaled by `TextScaleFactor / 100`
(or `null` if 100, if the font `IsSystemFont`, or if the OS is < Windows 10 1507). It is
documented in-source as the **Settings → Display → Make Text Bigger** setting.

The gaps: (1) the value is **never surfaced** to developers; (2) it is read **once**, at
default-font construction, and the app **never reacts** when the user changes the setting at
runtime; (3) there is **no notification** mechanism. This proposal fills those three gaps.

## Three-knob disambiguation (MANDATORY callout — reviewers will conflate these)

Windows surfaces three *different* sizing mechanisms; the Settings UI even puts two on one
page (`System → Display → Custom scaling` shows a "Custom scaling 100–500%" box AND a
"Text size" link). The proposal MUST state plainly which one it targets:

1. **Display / Custom scaling (100–500%)** → this is **DPI**. Already handled by
   `HighDpiMode` and the DPI events (`WM_DPICHANGED`, `Control.DpiChanged*`). **NOT** this
   proposal.
2. **Accessibility → Text size (100–225%)** → registry `TextScaleFactor` under
   `HKCU\Software\Microsoft\Accessibility`; WinRT `UISettings.TextScaleFactor` /
   `TextScaleFactorChanged`. **THIS is the target.** Independent of DPI.
3. **Legacy pre-Win10 per-element text sizing** (title bars/menus) → **removed** in Windows
   10 1703; the Accessibility slider replaced it. Mentioned only to close the loop.

State explicitly that `Application.SystemTextSize` reflects **#2 only**, and is orthogonal to
DPI (#1).

## Proposed API

### `Application` (process-static)

- **`public static double SystemTextSize { get; }`** — the current Accessibility text-scale
  factor (1.0–2.25; i.e. `TextScaleFactor / 100`). **Live getter** — re-reads the value, does
  not cache, because it is a system setting that changes at runtime. Well-defined regardless
  of whether any `Form` exists or how many UI threads are running (it is process-global).
- **`public static SystemTextSizeAwareness SystemTextSizeAwareness { get; set; }`** — the
  mode. **Enum, not bool**, deliberately, to reserve room for a future `Automatic`:
  - `Unaware` (default) — no notification raised; fully back-compatible, nothing changes.
  - `Notify` — raise change notifications (see below); the app decides how to respond.
  - *(reserved, NOT implemented now: `Automatic` — framework re-flows for you. Reserving the
    enum slot now avoids a future breaking bool→enum change, the same lesson `HighDpiMode`
    learned.)*
- **`public static event EventHandler? SystemTextSizeChanged`** — fires once per process when
  the setting changes (only when awareness is `Notify`).

### `Form` (instance)

- **`public event EventHandler? SystemTextSizeChanged`** — instance event, raised on each
  top-level `Form` when the setting changes.
- **`protected virtual void OnSystemTextSizeChanged(EventArgs e)`** — overridable, fires the
  instance event via the `EventHandlerList` pattern (`Events[s_systemTextSizeChangedEvent]`).

## The trigger architecture (the leak-critical part — get this exactly right)

A naive design — a static `Application.SystemTextSizeChanged` that `Form`s/`Control`s
subscribe to — **leaks**: the static event strongly roots every subscriber, so no `Form`
that subscribes is ever collected. Avoid this by **mirroring the DPI architecture**, where
`Control`/`Form` learn of DPI changes from their **own `WndProc`** (`WM_DPICHANGED` →
`OnDpiChanged` → instance event via `EventHandlerList`), **not** from a static subscription.

Verified facts that constrain the design:

- **`WM_SETTINGCHANGE` is broadcast** (`HWND_BROADCAST`) and delivered directly to top-level
  windows' `WndProc`s — it bypasses the thread message queue, so **`IMessageFilter` does NOT
  see it.** Do not use a message filter.
- **The WinForms parking window is message-only** (`CreateParams.Parent = HWND_MESSAGE`).
  Message-only windows are **excluded from broadcasts**, so the parking window **cannot**
  receive `WM_SETTINGCHANGE`. Do not use it.
- **`Application` has no `MainForm`.** The main form lives on `ApplicationContext` (per-run,
  per-UI-thread), can be `null` (tray/loop-only apps), and is mutable (splash→main handoff).
  So the main form is **not** a reliable receiver. Do not anchor the app-level event to it.
- **`SystemEvents` already owns a hidden top-level broadcast-receiving window** (its
  `.NET-BroadcastEventWindow`), and exposes `UserPreferenceChanged` with a
  `UserPreferenceCategory` (the relevant value is `Accessibility`, which is **coarse** — it
  covers any accessibility change, so you must re-read `TextScaleFactor` and diff to confirm
  it was text-scale).

**Resulting design:**

- **App-level:** `Application` makes **one internal, process-lifetime** subscription to
  `SystemEvents.UserPreferenceChanged`, filters `Category == Accessibility`, re-reads
  `TextScaleFactor`, diffs against the cached value, and if changed raises the static
  `SystemTextSizeChanged`. This is a single framework-static→framework-static link — it does
  **not** root any user object, so it is **not** the leak hazard. Reuses the existing hidden
  broadcast window; **no new HWND** required.
- **Form-level:** each top-level `Form` handles `WM_SETTINGCHANGE` in its **own `WndProc`**
  (it is a broadcast — every top-level window receives it), re-reads + diffs, and raises its
  **instance** `SystemTextSizeChanged` via `OnSystemTextSizeChanged`. `Form`s do **not**
  subscribe to `Application` — no rooting, lifetime = the window.

Caveat to document: there is **no dedicated `WM_TEXTSCALECHANGED`** message. Both paths must
recognize a *relevant* change by re-reading `TextScaleFactor` and comparing, not by the
message alone.

## Aids vs. leave-it-to-the-user

**Notify-only. No automatic re-layout / font-rescaling aids in this proposal.** Reasons:
text scale interacts with `AutoScaleMode`, anchored/docked layout, and explicitly-set fonts
in app-specific ways; a generic "scale all fonts by the factor" helper breaks more than it
fixes. The reserved `Automatic` enum value is exactly where such behavior would live later.
`Notify` gives the developer the factor and the event; they decide. (Consistent with the
companion `NodeLeading` proposal's conservative-default philosophy.)

## Why this matters across controls (evidence — include the matrix)

Multiple text-measuring controls derive item/row/tile extents from a `Font` that today only
reacts to the text-size setting once at startup (via `ScaleToSystemTextSize` on the default
font) and never again. So **any single cached height scalar is wrong the moment text size
changes at runtime** — which is the core argument for runtime awareness:

- **ListBox / ComboBox** — have `MeasureItem` + `OwnerDrawVariable` (a real per-item measure
  hatch) and `ItemHeight`. Their gap is the legacy default base calc + the missing runtime
  text-scale reaction — i.e. exactly this proposal.
- **TreeView** — has neither `MeasureItem` nor a wrapped native height API; only a uniform
  native item height. Worst-positioned for a managed fix; addressed by the companion
  `TreeView.NodeLeading` proposal, which depends on this one.
- **ListView (Details)** — no `MeasureItem`, no native row-height message; row height is
  comctl-computed from `SmallImageList` + control font, while per-item/subitem fonts are
  honored via `NM_CUSTOMDRAW` (`CDRF_NEWFONT`). Userland workarounds (phantom `SmallImageList`;
  `LVS_OWNERDRAWFIXED` + one-shot reflected `WM_MEASUREITEM`; "inflate control font / shrink
  item fonts") each have holes (header leak, set-once, exhaustive per-item font setting,
  owner-draw-all). **No clean complete userland solution exists** — strengthening the case
  that text-size reaction belongs in the framework.

## XML doc requirements

- Document that `SystemTextSize` is the **Accessibility text-size** factor (Settings →
  Display → Make Text Bigger), **not** DPI/display scaling, and is process-global / live.
- Document the `Unaware`/`Notify` semantics and that `Automatic` is reserved for future use.
- Document the no-rooting design note on the static event (so consumers understand instance
  vs. static).

## Open questions for review

- Should `SystemTextSize` be `double` (1.0–2.25) or expose the raw int percent (100–225)?
- Behavior on OS < Windows 10 1507 (where `ScaleToSystemTextSize` no-ops): `SystemTextSize`
  returns 1.0 and no events fire?
- Whether to also expose the value/event on `Application` only, leaving `Form` consumers to
  use their own `WndProc` override — or provide the `Form` instance event as proposed
  (recommended, for parity with the DPI event model).

## Output

The upstream issue per the skills: summary; the "complete a partial implementation"
rationale; the mandatory three-knob disambiguation; the proposed API; the leak-safe trigger
architecture with the four verified constraints (broadcast vs. IMessageFilter, message-only
parking window, no MainForm on Application, SystemEvents reuse); Notify-only stance; the
cross-control matrix; XML-doc requirements; open questions. Flag anything the source
contradicts.
