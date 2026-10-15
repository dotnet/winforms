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

These rules protect the generated partial declaration in `.Designer.cs` files. A file is analyzed only
when it contains an instance, non-generic, parameterless `void InitializeComponent` for a partial type
derived from `System.Windows.Forms.Control` or `System.ComponentModel.Component`, and that same type
also has a declaration in a non-Designer file. Framework symbols are resolved semantically: unrelated
types named `Control` or `Component` do not qualify.

The boundary is design-time round-tripping, not runtime equivalence. CodeDOM cannot express many
modern language constructs, and the WinForms Designer interprets only a subset of the statements
CodeDOM can represent. Keep initialization unrolled: explicit construction, assignments, supported
calls, and named event hookups. Move runtime-only behavior to the user partial after
`InitializeComponent`; do not hide designer initialization in a helper call.

### [WFO3000](https://aka.ms/winforms-warnings/wfo3000): Avoid unsupported code in `InitializeComponent`.

Move loops (including deconstructing `foreach`), conditionals, switch constructs, local functions,
`goto`, exception handling, locking, `using`, and `await` out of generated initialization.
Use a block-bodied `InitializeComponent`, not an expression-bodied method. These restrictions do not
apply to the standard generated `Dispose` override.

### [WFO3001](https://aka.ms/winforms-warnings/wfo3001): Keep custom members out of Designer files.

Move properties, nested types, and custom methods out of the generated partial declaration.
Constructors, `InitializeComponent`, the standard `Dispose(bool)` override, and explicit interface
implementations are allowed.

### [WFO3002](https://aka.ms/winforms-warnings/wfo3002): Keep generated fields at the end of the Designer file.

Keep fields at the end of the Designer partial, except the conventional `IContainer components`
infrastructure. This ordering preserves the historical generated layout; it is not itself a CodeDOM
expression restriction. Component fields constructed in `InitializeComponent` must belong to that
partial. Merely reading a user field does not make it designer-owned, and inherited, static, or
unrelated-type fields must not be moved into it.

### [WFO3003](https://aka.ms/winforms-warnings/wfo3003): Keep event and delegate declarations out of Designer files.

Move event and delegate declarations to the user code file.

### [WFO3004](https://aka.ms/winforms-warnings/wfo3004): Avoid collection expressions in Designer files.

Use syntax supported by Designer serialization instead of C# collection expressions in generated
Designer code, for example `new Control[] { button1, button2 }` or unrolled `Add` calls.

### [WFO3005](https://aka.ms/winforms-warnings/wfo3005): Avoid `nameof` expressions in `InitializeComponent`.

Use the serialized string value instead of `nameof`.

### [WFO3006](https://aka.ms/winforms-warnings/wfo3006): Avoid conditional expressions in `InitializeComponent`.

Move the condition to the user code file and serialize one deterministic value.

### [WFO3007](https://aka.ms/winforms-warnings/wfo3007): Avoid null-coalescing expressions in `InitializeComponent`.

Resolve the fallback outside generated Designer code. This includes both `??` and `??=`.

### [WFO3008](https://aka.ms/winforms-warnings/wfo3008): Avoid null-conditional expressions in `InitializeComponent`.

Move null-dependent access to the user code file.

### [WFO3009](https://aka.ms/winforms-warnings/wfo3009): Avoid interpolated strings in `InitializeComponent`.

Serialize the resulting string value instead of an interpolated expression.

### [WFO3010](https://aka.ms/winforms-warnings/wfo3010): Avoid anonymous functions in `InitializeComponent`.

Use a named event handler in the user code file instead of a lambda or anonymous method.

WFO3005–WFO3010 identify constructs that CodeDOM cannot represent. Their separate IDs allow tools and
agents to apply construct-specific guidance.

| Item      | Value             |
|-----------|-------------------|
| Category  | WinForms Designer |
| Enabled   | True              |
| Severity  | Error (WFO3000, WFO3001, WFO3003, WFO3004, WFO3006–WFO3008, WFO3010); Warning (WFO3002, WFO3005, WFO3009) |
| CodeFix   | False             |
| Added in  | NET11.0           |

---

## `PropertyAllocatesNewInstanceAnalyzer`

### [WFO2000](https://aka.ms/winforms-warnings/wfo2000): Avoid constructing fresh reference instances in property getters.

This is a general usage warning, not a Designer-only restriction. It identifies getter return paths
that directly construct a reference instance, without claiming that every access takes that path.
This includes directly returned arrays and anonymous objects.
Value-type construction, cached and initializer-backed properties, indexers, generated code, and
returns inside nested lambdas/local functions are excluded. User-defined conversions are not treated
as transparent returns. The rule does not perform interprocedural factory analysis.

Cache the instance only when stable identity is intended. For deliberate creation, consider a factory
method if API compatibility permits, or document a narrow suppression. There is no automatic fix:
caching can change ownership, disposal, and threading behavior. In particular, fresh instances from
content-serialized properties can discard property-grid edits instead of preserving designer state.

| Item      | Value          |
|-----------|----------------|
| Category  | WinForms Usage |
| Enabled   | True           |
| Severity  | Warning        |
| CodeFix   | False          |
| Added in  | NET11.0        |

---
