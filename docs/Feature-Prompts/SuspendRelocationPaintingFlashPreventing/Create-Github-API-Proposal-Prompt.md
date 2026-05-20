# Copilot Prompt 1 — Author the WinForms API Suggestion (GitHub issue)

## Your task

Write a complete API suggestion for the **dotnet/winforms** repository, ready to be filed
as a GitHub issue with the `api-suggestion` label. The issue covers one cohesive feature
area — *flicker-free UI mutation in WinForms* — composed of **three severable sub-features**.

You are not transcribing a settled spec. You are an experienced WinForms/.NET API designer
collaborating on this. The sections below give you **settled facts** and **current thinking
with reasoning**. Treat them differently (see "How to treat this briefing").

## How to treat this briefing

- **Settled — do not change:** the three sub-features and their scope; the public API
  *names* and *enum values* listed under "Settled API surface" below. These were
  argued through already.
- **Current thinking — challenge freely:** every *mechanism*, *risk framing*,
  *implementation strategy*, and *open question* below is our current lean with our
  reasoning attached. If you find a stronger argument, pivot — and say why. Pitch
  approaches as approaches, not gospel. We expect the API review board (and likely
  Stephen Toub) to pressure-test the mechanism choices; pre-empt that.
- **Actively look for what we missed.** Compatibility hazards, interaction with existing
  WinForms subsystems (data binding, `BindingSource`, `TableLayoutPanel`, MDI, DPI
  changes, `Control.RecreateHandle`, accessibility/UIA, designer surface), threading,
  trimming/AOT. If something here is wrong or naive, the most useful thing you can do
  is say so.

## Deliverable

A single Markdown document structured as a fileable `api-suggestion` issue, using exactly
these sections (this is the dotnet/winforms house format):

- `## Rationale`
- `## API Proposal` (C# signatures in fenced blocks, `namespace` declared)
- `## API Usage`
- `## Alternative Designs`
- `## Risks`
- `## Will this feature affect UI controls?`
- `### Status Checklist` (the standard api-suggestion checklist)

If during drafting you conclude the three sub-features should be filed as separate issues
rather than one, say so explicitly at the top and structure accordingly — that is a
legitimate pivot.

---

## Sub-feature A — `ISupportSuspendPainting` / `ISupportSuspendRelocation`

### Settled API surface
- Two free-standing public interfaces, `System.Windows.Forms` namespace:
  `ISupportSuspendPainting` with `BeginSuspendPainting()` / `EndSuspendPainting()`;
  `ISupportSuspendRelocation` with `BeginSuspendRelocation()` / `EndSuspendRelocation()`.
- `Control` implements both.
- `ListView`, `ListBox`, `ComboBox`, `TreeView`, `RichTextBox` override the *painting*
  methods to forward to their existing public `BeginUpdate` / `EndUpdate` (which remain
  unchanged in shape and behavior — source and binary compat).
- User-facing scope objects + extension methods (`SuspendPainting()`,
  `SuspendRelocation()`).

### Current thinking — challenge freely
- **Default `Control` painting suspension** via `WM_SETREDRAW`, refcounted; resume edge
  calls `Invalidate(true)`. Layout suspension forwards to existing
  `SuspendLayout` / `ResumeLayout`.
- **Refcount state** lives lazily on `Control` via the existing property-store slot
  pattern (zero cost until used). We considered default interface methods to avoid
  touching `Control`; rejected because `WM_SETREDRAW` is not reentrant and DIMs cannot
  hold per-instance state without a `ConditionalWeakTable` indirection that is strictly
  worse. Re-test this conclusion.
- **Not tied to `IArrangedElement`** — deliberately. `IArrangedElement` is internal, and
  `ToolStripItem` (an implementer) has no meaningful painting-suspension story. Future
  HWND-less "visuals" should implement these interfaces directly with their own
  mechanism. Evaluate whether the *relocation* interface specifically has a better home.
- **Scope type — our lean, expect pushback:** make the scopes `readonly ref struct`
  (pattern-based `Dispose`, works with `using`) rather than `class : IDisposable`.
  Reasoning: `ref struct` makes "forgot the `using`" / leaked-scope a *compile error*,
  which is what lets us honestly downgrade the unbalanced-refcount risk. Tradeoff: no
  `async`/iterator/lambda-capture/field storage, and you lose polymorphic `IDisposable`
  return. We think that tradeoff is fine for synchronous "mutate now" code paths.
  **This is a recommendation we expect to be pressure-tested in review — present both
  options with the tradeoff and recommend, do not assert.**
- **Refcount risk framing:** even with `ref struct` scopes, the interface methods stay
  `public` (designer-generated `InitializeComponent` must call them, and that code lives
  in the user's assembly). So a developer *can* call them directly. The honest claim is
  "the ergonomic path makes imbalance hard to hit accidentally; the refcount remains the
  correctness backstop" — not "the risk is eliminated." Nested scopes are supported by
  design, so the counter is necessary regardless.

## Sub-feature B — `DeferLocationChange` + `DeferWindowPos` batching

### Settled API surface
- A recommended user-facing entry point `DeferLocationChange()` returning a disposable
  scope, with multi-arg overloads to opt out of individual bundled behaviors
  (`suppressRender`, `suspendLayout`).

### Current thinking — challenge freely
- The scope bundles three things for a "I'm about to move many children" code path:
  Win32 `BeginDeferWindowPos` / `DeferWindowPos` / `EndDeferWindowPos` batching;
  `SuspendLayout` / `ResumeLayout`; and paint suppression (compose this from
  sub-feature A rather than duplicating `WM_SETREDRAW` logic).
- **Perf claim — be precise, do not overclaim.** `DeferWindowPos` improves *throughput*:
  one synchronized native move pass instead of N `SetWindowPos` calls, each with its own
  `WM_WINDOWPOSCHANGED`/`WM_SIZE`/invalidation/intermediate repaint. The
  `SuspendLayout` bundling separately improves *computation*: N `PerformLayout`
  invocations collapse to one. Neither speeds up the `LayoutEngine` algorithm itself.
  The proposal must keep these two wins distinct and must NOT claim "the layout engine
  got faster."
- **`HDWP` lifetime is the sharpest mechanical edge.** `BeginDeferWindowPos` allocates;
  each `DeferWindowPos` *returns a new HDWP* (must be captured/threaded); on failure it
  returns `NULL` and the *entire batch is lost*. The scope's `Dispose` must handle a
  `NULL` HDWP coherently (fall back to individual `SetWindowPos`, or abort cleanly —
  never `EndDeferWindowPos` on `NULL`) and must not leak a half-built HDWP if an
  exception unwinds through the `using` body. This deserves its own risk bullet.
- Same `ref struct` recommendation as A applies to this scope. Note: if the scope is
  `ref struct` it cannot be returned as `IDisposable` — evaluate whether the
  multi-overload story still works (it should; `using` is pattern-based).

## Sub-feature C — `Application.SetFormAppearanceMode` (deferred form display)

### Settled API surface
- `Application.SetFormAppearanceMode(FormAppearanceMode mode)` — process-wide
  configuration API, called early (before the first form), consistent in pattern and
  lifecycle with `Application.SetColorMode` and `Application.SetHighDpiMode`.
- `enum FormAppearanceMode { Classic = 0, Deferred = 1 }`.
- `Classic` = pre-.NET 11 behavior (opt-out). `Deferred` = .NET 11 default.
- Note the deliberate split: `Classic` is the enum's *zero value* (conservative
  `default`), while `Deferred` is the *runtime default* applied when the API is never
  called. Call this out so review does not read it as a contradiction.

### Current thinking — challenge freely
- Mechanism: cloak top-level forms via DWM (`DWMWA_CLOAK`) at handle creation, uncloak
  once the background has been painted, so the form is revealed in one step instead of
  flashing a default (white) background — most visible in dark mode.
- **Uncloak timing is genuinely open — this is the part most likely to need a better
  idea.** Our naive lean is "uncloak after the first `WM_PAINT` that paints the form
  background." Uncloak too early → still flashes; too late → window appears slow to
  open. Unlike Edge, WinForms has no single universal "first real frame ready" signal —
  it depends on double-buffering, custom `OnPaintBackground`, late-painting child
  controls. Evaluate alternatives and recommend; flag remaining uncertainty honestly.
- **Honesty caveat that must survive into the docs:** deferral applies to the *form
  background*. A deep tree of late-painting child controls can still produce visible
  updates after reveal. The XML doc / proposal must state this so a late-child blink is
  not later mis-filed as a regression. (The flash-elimination benefit belongs in the
  XML `<summary>`; the caveat in `<remarks>`.)
- Evaluate interaction with: MDI child forms, `Form.Show` vs `ShowDialog`, splash
  screens / forms that *want* to appear instantly, owned/tool windows, per-monitor DPI
  changes during creation, and `Form.Opacity` / layered windows.

## Filing instruction

If your final assessment is that the design is sound, produce the issue body ready to
file with the `api-suggestion` label. If you found a reason to pivot on anything outside
the settled API surface, lead with a short "Deviations from the briefing" note
explaining what you changed and why, then give the proposal. Either way, the proposal
itself is the deliverable.
