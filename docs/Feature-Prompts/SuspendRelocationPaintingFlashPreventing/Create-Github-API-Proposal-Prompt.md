# Copilot Prompt 1 — Author the WinForms API Suggestion

## Your task

Write a complete API suggestion for the `dotnet/winforms` repository, ready to be filed
as a GitHub issue with the `api-suggestion` label. The issue covers two related features
for reducing visible intermediate states while WinForms applications update their UI:

1. Painting suspension with optional layout suspension across a control tree.
2. Deferred top-level form reveal.

Use these issue sections:

- `## Rationale`
- `## API Proposal`
- `## API Usage`
- `## Alternative Designs`
- `## Risks`
- `## Will this feature affect UI controls?`
- `### Status Checklist`

## Sub-feature A — painting and layout suspension

### Settled API surface

```csharp
namespace System.Windows.Forms;

public interface ISupportSuspendPainting
{
    void BeginSuspendPainting();
    void EndSuspendPainting();
}

public enum LayoutSuspendTraversal
{
    None = 0,
    TopLevelOnly = 1,
    Traverse = 2,
}

public sealed class SuspendPaintingScope : IDisposable
{
    public SuspendPaintingScope(ISupportSuspendPainting? target);
    public void Dispose();
}

public static class ControlMutationExtensions
{
    public static SuspendPaintingScope SuspendPainting(
        this ISupportSuspendPainting target);

    public static SuspendPaintingScope SuspendPainting(
        this ISupportSuspendPainting target,
        LayoutSuspendTraversal layoutSuspendTraversal);

    public static SuspendPaintingScope SuspendPainting(
        this ISupportSuspendPainting target,
        Func<Control, bool> suspendLayoutContainerFilter);
}
```

- `Control` implements `ISupportSuspendPainting` explicitly and exposes protected virtual
  `BeginSuspendPaintingCore` / `EndSuspendPaintingCore` hooks.
- `ListView`, `ListBox`, `ComboBox`, `TreeView`, and `RichTextBox` route painting
  suspension through their existing `BeginUpdate` / `EndUpdate` paths.
- `None` suspends painting only.
- `TopLevelOnly` suspends layout on the target `Control`.
- `Traverse` suspends layout on the target and all existing descendants.
- The predicate is evaluated for the target and every descendant. A `false` result skips
  that node but does not prune traversal.
- Selected nodes are suspended even when they currently have no children.
- Layout-aware overloads require the target to derive from `Control`.
- The scope is a sealed class so it can span `await`.

### Design considerations

- Snapshot the selected controls when the scope starts.
- Suspend layout root-to-leaf and resume it deepest-first.
- Resume layout before ending painting so recursive invalidation occurs after layout.
- Keep disposal idempotent and preserve nesting through the existing ref counts.
- Explain that controls added after the snapshot are covered by their parent's suspended
  layout but do not receive an independently balanced suspension.
- Discuss traversal cost for large control trees and exceptions thrown by predicates.
- Include invalid and non-`Control` target behavior in the proposal.

## Sub-feature B — deferred form reveal

Use the current `FormRevealMode` design from the tracked API proposal:

```csharp
namespace System.Windows.Forms;

public enum FormRevealMode
{
    Inherit = -1,
    Classic = 0,
    Deferred = 1,
}

public partial class Form
{
    public virtual FormRevealMode FormRevealMode { get; set; }
}

public partial class Application
{
    public static FormRevealMode DefaultFormRevealMode { get; }
    public static void SetDefaultFormRevealMode(FormRevealMode mode);
    public static bool IsFormRevealDeferred { get; }
}
```

Describe DWM cloaking, dark-mode-aware default resolution, designer serialization,
top-level-window limitations, and conservative fallback behavior on unsupported systems.

## Filing instruction

Produce a complete proposal rather than an implementation plan. Clearly distinguish fixed
API shape from implementation choices that remain open for review.
