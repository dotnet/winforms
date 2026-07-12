# Copilot Prompt 2 — Implement the flicker-free UI mutation APIs

## Prerequisite

This prompt consumes the **approved API suggestion** produced by Prompt 1 (and refined
through API review). Before implementing, read that proposal in full and treat its final
`API Proposal` section as the contract. Where the approved proposal and this prompt
disagree, the **approved proposal wins** — note any such conflict explicitly rather than
silently picking one.

## Your task

Implement the three sub-features in **dotnet/winforms**, production quality, against the
approved API surface. You have engineering latitude on *internals* — the public surface
is fixed by the proposal, the implementation is yours to do well.

## Scope

### A — `ISupportSuspendPainting` / `ISupportSuspendRelocation`
- The two interfaces, `System.Windows.Forms` namespace.
- `Control` implementation: refcounted painting suspension; lazy refcount state via the
  existing property-store slot pattern (follow the established `Control` precedent — do
  not add an eager field). Layout-suspension methods forward to existing
  `SuspendLayout` / `ResumeLayout`.
- Overrides on `ListView`, `ListBox`, `ComboBox`, `TreeView`, `RichTextBox` forwarding
  the painting methods to their existing `BeginUpdate` / `EndUpdate`. The existing public
  `BeginUpdate` / `EndUpdate` signatures and behavior MUST NOT change.
- The user-facing scope type(s) and extension methods, exactly as the approved proposal
  specifies them (`ref struct` vs `class` per the proposal's final decision).
- Unbalanced `End*` must match `ResumeLayout` precedent (the proposal will have settled
  throw-vs-no-op; follow it).

### B — `DeferLocationChange` + `DeferWindowPos` batching
- The scope and its overloads per the approved proposal.
- Win32 batching via `BeginDeferWindowPos` / `DeferWindowPos` / `EndDeferWindowPos`.
  Capture and thread the returned `HDWP` correctly on every `DeferWindowPos` call.
- `Dispose` must handle a `NULL` HDWP coherently and must not leak on exception unwind.
- Compose paint suppression from sub-feature A; do not duplicate `WM_SETREDRAW` logic.

### C — `Application.SetFormAppearanceMode` + `FormAppearanceMode`
- The enum (`Classic = 0`, `Deferred = 1`) and the `Application` configuration API.
- `Deferred` is the runtime default when the API is never called; `Classic` restores
  pre-.NET 11 behavior.
- DWM cloaking at top-level form handle creation; uncloak per the timing strategy the
  approved proposal settled on.
- Must be inert / safe when the OS does not support the relevant DWM attributes.

## Engineering requirements

- Target the C# language version and runtime of the current dotnet/winforms `main`.
- NRTs enabled; assume the repo's global usings.
- Match dotnet/winforms code style, P/Invoke conventions (CsWin32-generated `PInvoke`
  surface), and the existing interop patterns — do not hand-roll `DllImport` if a
  generated entry point exists.
- All public API gets XML docs. For `FormAppearanceMode.Deferred`, the flash-elimination
  benefit goes in `<summary>`; the "background only, deep child trees may still update"
  caveat goes in `<remarks>`.
- Public API additions require matching entries in the `*.cs` reference-assembly /
  public-API-baseline files the repo uses.
- Thread affinity: all of this assumes the UI thread; add debug assertions where the
  repo already does, and do not let them affect release behavior.

## Tests

- Unit tests for refcount balance, including nesting and unbalanced-`End`.
- Tests that `ListView` et al. route through their native path and do not double-suspend.
- Tests for `DeferLocationChange` correctness including the `NULL`-HDWP fallback and
  exception-unwind path.
- For `FormAppearanceMode`, tests for `Classic` (no behavior change) and `Deferred`
  (cloak/uncloak lifecycle), plus the OS-unsupported fallback.

## Deliverable

A pull request (or a clear set of commits) implementing the above, with a PR description
that summarizes the change, links the API suggestion, and calls out any place the
implementation revealed a problem with the approved design that review should revisit.
