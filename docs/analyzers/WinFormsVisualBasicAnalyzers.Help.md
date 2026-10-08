# How to use System.Windows.Forms.Analyzers

System.Windows.Forms.Analyzers analyzers and source generators are shipped inbox with Windows Desktop .NET SDK, and
are automatically referenced for Window Forms .NET applications.

## `MissingPropertySerializationConfiguration`

`MissingPropertySerializationConfiguration` checks for missing `DesignerSerializationVisibilityAttribute` on properties of classes which are 
derived from `Control` and could potentially serialize design-time data by the designer without the user being aware of it.

### [WFO1000](https://aka.ms/winforms-warnings/wfo1000): Missing property serialization configuration.

Properties of classes derived from `Control` should have `DesignerSerializationVisibilityAttribute` 
set to `DesignerSerializationVisibility.Content` or `DesignerSerializationVisibility.Visible`.

| Item      | Value            |
|-----------|------------------|
| Category  | WinForms Security|
| Enabled   | True             |
| Severity  | Error            |
| CodeFix   | False            |
| Added in  | NET9.0           |

---

## `AvoidPassingFuncReturningTaskWithoutCancellationToken`

`AvoidPassingFuncReturningTaskWithoutCancellationToken` checks parameters passed to `Control.InvokeAsync`. It suggests to use a cancellation token when passing a task to these methods.

### [WFO2001](https://aka.ms/winforms-warnings/wfo2001): Task is being passed to InvokeAsync without a cancellation token.

Avoid passing a `Func<T>` to `InvokeAsync` where `T` is a `Task` or `ValueTask`, unless your intention is for the delegate to simply be kicked off as an unsupervised task. Instead, use `Func<CancellationToken, ValueTask>` or `Func<CancellationToken, ValueTask<T>>`, so that the delegate passed to `InvokeAsync` can be awaited, allowing exceptions to be properly handled. 

| Item      | Value            |
|-----------|------------------|
| Category  | WinForms Usage   |
| Enabled   | True             |
| Severity  | Warning          |
| CodeFix   | False            |
| Added in  | NET9.0           |

---

## `ImplementITypedDataObject`

`ImplementITypedDataObject` checks custom implementations of the managed `IDataObject` interface and suggests to also implement the `ITypedDataObject` interface.

### [WFO1001](https://aka.ms/winforms-warnings/wfo1001): `IDataObject` type does not implement `ITypedDataObject`.

Types should implement `ITypedDataObject` to support best practices when interacting with data. Types will not work with typed APIs in Clipboard and other data exchange scenarios if they only implement `IDataObject`.

| Item      | Value            |
|-----------|------------------|
| Category  | WinForms Security|
| Enabled   | True             |
| Severity  | Warning          |
| CodeFix   | False            |
| Added in  | NET10.0          |

---

## WinForms Designer guardrails

These rules protect the generated partial declaration in `.Designer.vb` files. A file is analyzed only
when it contains an instance, non-generic, parameterless `Sub InitializeComponent` for a partial type
derived from `System.Windows.Forms.Control` or `System.ComponentModel.Component`, and that same type
also has a declaration in a non-Designer file. Framework symbols are resolved semantically: unrelated
types named `Control` or `Component` do not qualify. Identifier matching respects VB casing rules and
optional parameter-list parentheses.

The boundary is design-time round-tripping, not runtime equivalence. CodeDOM cannot express many
modern constructs, and the WinForms Designer interprets only a subset of CodeDOM statements.
Keep initialization unrolled and move runtime-only behavior to the user partial after
`InitializeComponent`; do not hide designer initialization behind helper calls.

### [WFO3000](https://aka.ms/winforms-warnings/wfo3000): Avoid unsupported code in `InitializeComponent`.

Move `For`, `For Each`, `While`, `Do`, `If`, `Select`, `GoTo`, `Try`, `SyncLock`, `Using`, and `Await`
out of generated initialization. The standard generated `Dispose` override is intentionally exempt:
its `Try`/`If`/`Finally` structure is valid infrastructure.

### [WFO3001](https://aka.ms/winforms-warnings/wfo3001): Keep custom members out of Designer files.

Move properties, nested types, and custom methods out of the generated partial declaration.
Constructors, `InitializeComponent`, the standard `Dispose(Boolean)` override, and explicit interface
method implementations are allowed.

### [WFO3002](https://aka.ms/winforms-warnings/wfo3002): Keep generated fields at the end of the Designer file.

Keep fields and `WithEvents` declarations at the end of the Designer partial, except the conventional
`IContainer components` infrastructure. This preserves the historical generated layout rather than
enforcing a modern application-code style. Component members constructed in `InitializeComponent`
belong in that partial; merely reading a user field does not make it designer-owned. Inherited and
unrelated-type members must not be moved.

VB `WithEvents` declarations bind as property symbols. They are explicitly handled without treating
ordinary properties as generated fields.

### [WFO3003](https://aka.ms/winforms-warnings/wfo3003): Keep event and delegate declarations out of Designer files.

Move event and delegate declarations to the user code file.

WFO3004 is C#-only because Visual Basic has no collection-expression syntax.

### [WFO3005](https://aka.ms/winforms-warnings/wfo3005): Avoid `NameOf` expressions in `InitializeComponent`.

Use the serialized string value instead of `NameOf`.

### [WFO3006](https://aka.ms/winforms-warnings/wfo3006): Avoid conditional expressions in `InitializeComponent`.

Move the three-argument `If` condition to the user code file and serialize one deterministic value.

### [WFO3007](https://aka.ms/winforms-warnings/wfo3007): Avoid null-coalescing expressions in `InitializeComponent`.

Resolve the two-argument `If` fallback outside generated Designer code.

### [WFO3008](https://aka.ms/winforms-warnings/wfo3008): Avoid null-conditional expressions in `InitializeComponent`.

Move null-dependent access to the user code file.

### [WFO3009](https://aka.ms/winforms-warnings/wfo3009): Avoid interpolated strings in `InitializeComponent`.

Serialize the resulting string value instead of an interpolated expression.

### [WFO3010](https://aka.ms/winforms-warnings/wfo3010): Avoid anonymous functions in `InitializeComponent`.

Use a named event handler in the user partial instead of a lambda. Use `Handles` for designer member
events and `AddressOf` for supported local-component event hookups.

WFO3005–WFO3010 identify constructs that CodeDOM cannot represent. Their separate IDs allow tools and
agents to apply construct-specific guidance.

### [WFO3011](https://aka.ms/winforms-warnings/wfo3011): Declare Designer component members `WithEvents`.

A component member constructed by `InitializeComponent` must use the VB Designer's event model.
For example, keep `Friend WithEvents Button1 As Button` at the end of the Designer partial and put
`Private Sub Button1_Click(...) Handles Button1.Click` in the user partial. The rule does not apply to
the components container, scalar/helper fields, or local variables.

### [WFO3012](https://aka.ms/winforms-warnings/wfo3012): Use `Handles` for Designer member events.

Replace `AddHandler Button1.Click, AddressOf Button1_Click` in `InitializeComponent` with
`Handles Button1.Click` on the named handler in the user partial, without retaining both hookups.
Root events preserve the receiver with `Handles Me.EventName`, `Handles MyBase.EventName`, or
`Handles MyClass.EventName`; inherited `WithEvents` members also support `Handles` without
redeclaring them.

This is not a blanket ban on `AddHandler`. The designer's field-generation setting is named
`GenerateMember`: when it is `False`, components can be locals, and supported
`AddHandler local.Click, AddressOf Handler` wiring remains valid. Arbitrary properties, inherited
plain fields, unrelated receivers, and runtime event wiring outside `InitializeComponent` are not
rewritten into the member model. CodeDOM itself represents event attachment; the restriction protects
VB Designer ownership and event round-tripping.

WFO3011 and WFO3012 are VB-only. No automatic fix is offered because changing declarations and
subscriptions requires preserving captures, accessibility, and existing `Handles` clauses.

| Item      | Value             |
|-----------|-------------------|
| Category  | WinForms Designer |
| Enabled   | True              |
| Severity  | Error (WFO3000, WFO3001, WFO3003, WFO3006–WFO3008, WFO3010–WFO3012); Warning (WFO3002, WFO3005, WFO3009) |
| CodeFix   | False             |
| Added in  | NET11.0           |

---

## `PropertyAllocatesNewInstanceAnalyzer`

### [WFO2000](https://aka.ms/winforms-warnings/wfo2000): Avoid constructing fresh reference instances in property getters.

This remains a general usage warning, not a Designer-only restriction. It identifies getter return
paths that directly construct reference instances, not guaranteed allocation on every access.
This includes directly returned arrays and anonymous objects.
Value types, cached and initializer-backed properties, indexers, generated code, nested lambda
returns, and user-defined conversions are excluded.

Cache only when stable identity is intended. Otherwise consider a factory method when API
compatibility permits, or document a narrow suppression. No automatic fix changes ownership,
disposal, or threading semantics. Content-serialized properties returning fresh instances are a
particular design-time hazard because property-grid edits can be applied to discarded instances.

| Item      | Value          |
|-----------|----------------|
| Category  | WinForms Usage |
| Enabled   | True           |
| Severity  | Warning        |
| CodeFix   | False          |
| Added in  | NET11.0        |

---
