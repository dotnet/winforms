# Copilot Prompt 2 — Implement the flicker-free UI mutation APIs

## Prerequisite

Read the current upstream API suggestion before implementation. Treat its latest
`API Proposal` section as the public contract and report any conflict instead of silently
choosing a different shape.

## Scope

### A — painting suspension with optional layout traversal

- Implement `ISupportSuspendPainting` on `Control` with ref-counted
  `BeginSuspendPaintingCore` / `EndSuspendPaintingCore` hooks.
- Route `ListView`, `ListBox`, `ComboBox`, `TreeView`, and `RichTextBox` through their
  existing `BeginUpdate` / `EndUpdate` mechanisms.
- Keep `SuspendPaintingScope` as a sealed, idempotent `IDisposable` class so it can span
  `await`.
- Provide these extension methods:

  ```csharp
  public static SuspendPaintingScope SuspendPainting(
      this ISupportSuspendPainting target);

  public static SuspendPaintingScope SuspendPainting(
      this ISupportSuspendPainting target,
      LayoutSuspendTraversal layoutSuspendTraversal);

  public static SuspendPaintingScope SuspendPainting(
      this ISupportSuspendPainting target,
      Func<Control, bool> suspendLayoutContainerFilter);
  ```

- `LayoutSuspendTraversal` has `None`, `TopLevelOnly`, and `Traverse`.
- Snapshot selected controls before suspension. Suspend root-to-leaf and resume
  deepest-first.
- The predicate is evaluated for the target and every descendant. A `false` result skips
  suspension for that node without pruning its descendants.
- Suspend selected controls even when they currently have no children.
- Validate layout-aware calls before beginning painting. A non-`Control` target is invalid.
- Resume all selected layout scopes before ending painting so the recursive invalidation
  occurs after layout.

### B — deferred form reveal

- Implement the approved `FormRevealMode`, `Form.FormRevealMode`, and
  `Application` configuration APIs.
- Preserve classic behavior when deferred reveal is not active.
- Cloak eligible top-level form handles and uncloak according to the timing strategy in the
  proposal.
- Keep unsupported DWM and window configurations inert and safe.

## Engineering requirements

- Match the current repository language version, nullable annotations, code style, and
  interop conventions.
- Add XML documentation for every public or protected API.
- Update `PublicAPI.Unshipped.txt`.
- Keep state lazy where existing `Control` property-store patterns apply.
- Do not change existing public `BeginUpdate` / `EndUpdate` behavior.

## Tests

- Painting ref-count balance, nesting, idempotent disposal, handle creation, and handle
  recreation.
- `None`, `TopLevelOnly`, and `Traverse` layout selection.
- Predicate selection, continued traversal after a rejected node, and empty containers.
- Deepest-first layout resume and recursive invalidation after layout.
- Null, invalid-enum, and non-`Control` target failures without partial suspension.
- Existing native update paths for the selected built-in controls.
- Classic/deferred form reveal behavior and unsupported-OS fallback.

## Deliverable

Provide production code, focused tests, updated API tracking, and an updated API proposal.
Call out any behavior that the implementation proves unsafe or unnecessarily costly.
