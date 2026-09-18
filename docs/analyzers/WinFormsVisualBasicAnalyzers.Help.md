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
when it contains `InitializeComponent` for a partial type derived from `Control` and that type also has
a declaration in a non-Designer file.

### [WFO2002](https://aka.ms/winforms-warnings/wfo2002): Avoid unsupported code in `InitializeComponent`.

Move `For`, `For Each`, `While`, `Do`, `If`, `Select`, `GoTo`, `Try`, and `SyncLock` constructs to the
user code file.

### [WFO2003](https://aka.ms/winforms-warnings/wfo2003): Keep custom members out of Designer files.

Move properties, nested types, and custom methods out of the generated partial declaration.
Constructors, `InitializeComponent`, the standard `Dispose(Boolean)` override, and explicit interface
method implementations are allowed.

### [WFO2004](https://aka.ms/winforms-warnings/wfo2004): Keep generated fields at the end of the Designer file.

Fields referenced by `InitializeComponent` belong at the end of the Designer partial declaration.
The analyzer uses field-symbol identity, so similarly named locals, properties, and methods do not
trigger this diagnostic.

### [WFO2005](https://aka.ms/winforms-warnings/wfo2005): Keep event and delegate declarations out of Designer files.

Move event and delegate declarations to the user code file.

WFO2006 is C#-only because Visual Basic has no collection-expression syntax.

### [WFO2007](https://aka.ms/winforms-warnings/wfo2007): Avoid `NameOf` expressions in `InitializeComponent`.

Use the serialized string value instead of `NameOf`.

### [WFO2008](https://aka.ms/winforms-warnings/wfo2008): Avoid conditional expressions in `InitializeComponent`.

Move the three-argument `If` condition to the user code file and serialize one deterministic value.

### [WFO2009](https://aka.ms/winforms-warnings/wfo2009): Avoid null-coalescing expressions in `InitializeComponent`.

Resolve the two-argument `If` fallback outside generated Designer code.

### [WFO2010](https://aka.ms/winforms-warnings/wfo2010): Avoid null-conditional expressions in `InitializeComponent`.

Move null-dependent access to the user code file.

### [WFO2011](https://aka.ms/winforms-warnings/wfo2011): Avoid interpolated strings in `InitializeComponent`.

Serialize the resulting string value instead of an interpolated expression.

### [WFO2012](https://aka.ms/winforms-warnings/wfo2012): Avoid anonymous functions in `InitializeComponent`.

Use a named event handler in the user code file instead of a lambda.

WFO2007–WFO2012 identify constructs that CodeDOM cannot represent. Their separate IDs allow tools and
agents to apply construct-specific guidance.

| Item      | Value             |
|-----------|-------------------|
| Category  | WinForms Designer |
| Enabled   | True              |
| Severity  | Warning           |
| CodeFix   | False             |
| Added in  | NET11.0           |

---

## `PropertyAllocatesNewInstanceAnalyzer`

### [WFO2013](https://aka.ms/winforms-warnings/wfo2013): Avoid allocating a new object on every property access.

A property getter that directly returns `New` creates a fresh object every time the property is read.
Cache the instance when the property represents stable state, or replace the property with a method
when creating a fresh value is intentional.

| Item      | Value          |
|-----------|----------------|
| Category  | WinForms Usage |
| Enabled   | True           |
| Severity  | Warning        |
| CodeFix   | False          |
| Added in  | NET11.0        |

---
