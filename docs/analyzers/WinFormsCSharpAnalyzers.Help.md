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
when it contains `InitializeComponent` for a partial type derived from `Control` and that type also has
a declaration in a non-Designer file.

### [WFO2002](https://aka.ms/winforms-warnings/wfo2002): Avoid control flow in `InitializeComponent`.

Move loops, conditionals, switch constructs, local functions, `goto`, exception handling, and locking
to the user code file. `nameof`, interpolation, lambdas, null operators, and conditional expressions
are not prohibited by this rule.

### [WFO2003](https://aka.ms/winforms-warnings/wfo2003): Keep custom members out of Designer files.

Move properties, nested types, and custom methods out of the generated partial declaration.
Constructors, `InitializeComponent`, the standard `Dispose(bool)` override, and explicit interface
implementations are allowed.

### [WFO2004](https://aka.ms/winforms-warnings/wfo2004): Keep generated fields at the end of the Designer file.

Fields referenced by `InitializeComponent` belong at the end of the Designer partial declaration.
The analyzer uses field-symbol identity, so similarly named locals, properties, and methods do not
trigger this diagnostic.

### [WFO2005](https://aka.ms/winforms-warnings/wfo2005): Keep event and delegate declarations out of Designer files.

Move event and delegate declarations to the user code file.

### [WFO2006](https://aka.ms/winforms-warnings/wfo2006): Avoid collection expressions in Designer files.

Use syntax supported by Designer serialization instead of C# collection expressions in generated
Designer code.

| Item      | Value             |
|-----------|-------------------|
| Category  | WinForms Designer |
| Enabled   | True              |
| Severity  | Warning           |
| CodeFix   | False             |
| Added in  | NET11.0           |

---
