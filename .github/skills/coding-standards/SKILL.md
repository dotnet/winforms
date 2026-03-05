---
name: coding-standards
description: >-
  C# and .NET coding standards for generating new code files. Use when creating
  new classes, methods, controls, tests, or any new C# / VB source files in this
  repository. Covers C# 14 / .NET 10 patterns, naming, formatting, XML docs,
  WinForms conventions, and performance idioms.
metadata:
  author: dotnet-winforms
  version: "1.0"
---

# Coding Standards — New Code Generation

These rules apply whenever you **generate new** C# or VB.NET source files in
this repository. For modernizing or refactoring *existing* files, see the
`code-modernization` skill instead.

## Target Framework

* Default target is **.NET 10.0** (`net10.0`).
* Some files target .NET Framework or are multi-targeted — check the project
  file before generating.
* Do not edit files under `eng/common/`.
* Follow all rules defined in `.editorconfig`.

## Language Version

Use **C# 14** features and patterns. Key features to prefer:

| Feature | Example |
|---|---|
| Extension members (extension blocks) | `extension(string s) { public bool IsBlank() => ... }` |
| `field` keyword in properties | `set => field = value ?? throw ...;` |
| Null-conditional assignment | `customer?.Order = GetOrder();` |
| `nameof` on unbound generics | `nameof(List<>)` |
| Lambda modifiers without types | `(text, out result) => int.TryParse(text, out result)` |
| Partial constructors / events | `partial MyClass() { }` |
| First-class `Span<T>` conversions | Implicit `T[]` → `Span<T>` / `ReadOnlySpan<T>` |

## General Assumptions

* **Namespace imports** are defined in a global usings file — omit standard
  `using` directives unless the import is non-obvious.
* **Nullable Reference Types** are enabled — annotate all new code accordingly.
* Use **file-scoped namespace** declarations.

## Type Usage and `var` Policy

Prefer explicit type names. Use `var` only when the type is either:

1. **Obviously redundant** with the right-hand side (e.g., factory methods
   whose return type is clear from the variable name), or

2. **Overly complex** — deeply nested generics, generic tuples, or similarly
   unwieldy type expressions.

```csharp
// REQUIRED — explicit types for primitives and clear declarations
int count = 5;
string name = "Button";
bool isVisible = true;
Button okButton = new();            // target-typed new — preferred

// ACCEPTABLE — var when type is obvious or complex
var items = GetComplexGenericResult(); // long generic return type
var (key, value) = dictionary.First(); // tuple deconstruction
```

**Never** use `var` for primitive types (`int`, `string`, `bool`, `double`,
`float`, `decimal`, `char`, `byte`, `long`, etc.).

When combining declaration and instantiation, prefer **target-typed `new()`**:

```csharp
Dictionary<string, List<int>> map = new();   // preferred
List<string> items = [];                      // collection expression — also preferred
```

## Null Checking

* **Use `is null` / `is not null`** — never `== null` or `!= null`.
* Use coalesce: `value ?? "default"`, `value ??= GetDefault()`.

## Naming and Field Conventions

| Element | Convention |
|---|---|
| `private` / `internal` instance fields | `_camelCase` |
| `private static` fields | `s_camelCase` |
| `[ThreadStatic]` fields | `t_camelCase` |
| Constants | `PascalCase` |
| All other members | `PascalCase` |

* **Never** qualify with `this.` unless strictly necessary to resolve ambiguity.
* Declare accessibility modifiers **explicitly** on every type and member.
* Use the **narrowest possible scope** — prefer `private` over `internal` over
  `public`.
* Mark members `static` when they do not access instance state.

## Formatting and Line Length

### Expression-bodied members

Use expression bodies (`=>`) for single-expression methods and read-only
properties. When the **total line length would exceed 60 characters**, wrap by
placing the `=>` on the next line, indented:

```csharp
// Short — fits on one line
internal int BorderWidth => _borderWidth;

// Long — wrap the arrow to the next line
private bool IsValidSize(Size size)
    => size.Width > 0 && size.Height > 0;

internal string QualifiedName
    => $"{Namespace}.{TypeName}";
```

> **Important semantic distinction:** `public Foo Bar => new Foo();` creates a
> new instance on every access. `public Foo Bar { get; } = new Foo();` creates
> one instance at construction. Never convert between these forms unless the
> original semantics were provably wrong.

### Braces and blocks

* **Allman style** — opening brace on its own line.
* Insert an **empty line** after closing braces of control-flow blocks and
  **before `return` statements**.
* If a comment precedes a line that needs an empty line above it, the empty line
  goes **before the comment**.

### Ternary operator

Put each branch on its own line unless the whole expression is very short:

```csharp
Color textColor = e.Item.Enabled
    ? GetDarkModeColor(e.TextColor)
    : GetDarkModeColor(SystemColors.GrayText);
```

## Pattern Matching and Switch Expressions

Prefer `switch` expressions over `switch` statements over `if`-chains:

```csharp
string result = value switch
{
    > 0 => "Positive",
    < 0 => "Negative",
    _ => "Zero"
};
```

Use `and`, `or`, relational, property, tuple, type, and list patterns where they
improve clarity. When converting `if`-chains that return or assign, a switch
expression is almost always clearer.

## Error Handling and Argument Validation

Use **throw-helper methods** — never hand-roll null / range checks:

```csharp
ArgumentNullException.ThrowIfNull(parameter);
ArgumentOutOfRangeException.ThrowIfNegative(value);
ObjectDisposedException.ThrowIf(_disposed, this);
```

Always use `nameof()` for parameter and member names in exceptions and asserts.

## Performance Idioms

* Single-`char` overloads: `text.Contains('c')`, `sb.Append('x')`.
* `ReadOnlySpan<char>` over `Substring` when possible.
* `items.Any()` over `Count() > 0`.
* `TryGetValue` over `ContainsKey` + indexer.
* `string.IsNullOrEmpty` over `?.Length == 0`.
* `Array.Empty<T>()` over `new T[0]`.
* Prefer `using` declarations (`using var ...`) over `using` blocks. Use the
  block form only when a tighter scope is required.

```csharp
using Pen focusPen = new(focusColor)
{
    Width = 1.0f,
    DashStyle = DashStyle.Dot
};
```

## Magic Numbers

Replace hard-coded literals with named constants or enums or avoid then for new code when possible. If a literal is truly self-explanatory and unlikely to change, it may be acceptable to leave it as-is.

```csharp

## XML Documentation

Every class, struct, record, interface, and enum — **regardless of access
modifier, including `private` nested types** — must have an XML doc header:

* **`<summary>`** — always present; a concise statement of the type's purpose.
* **`<remarks>`** — add for non-trivial types. Use `<para>` for multiple
  paragraphs.
* Use `<inheritdoc/>` on overridden members.
* Do **not** XML-comment local functions; use a regular `//` comment if
  the purpose is not self-evident.
* Use **Unicode characters** in docs — never HTML entities.
* Indent XML structure with **1 space** per nesting level:

```csharp
/// <summary>
///  Gets or sets the border width of the control.
/// </summary>
/// <remarks>
///  <para>
///   The border width affects the visual appearance and layout
///   calculations of the control.
///  </para>
///  <para>
///   Values must be non-negative. A value of 0 means no border.
///  </para>
/// </remarks>
public int BorderWidth { get; set; }
```

## File Structure

* File-scoped namespaces.
* UTF-8 with BOM, CRLF line endings.
* 4-space indentation, no tabs.
* No trailing whitespace; final newline at end of file.

## WinForms-Specific

### Events

```csharp
public event EventHandler<EventArgs>? Click;

protected virtual void OnClick(EventArgs e)
{
    Click?.Invoke(this, e);
}

// NRT-aware handler signature
private void OnSomeEvent(object? sender, EventArgs e) { }
```

Use `EventArgs.Empty` for parameterless event raises.

### Designer integration

* Mark designer-generated controls as nullable:
  `private Button? _okButton;`
* Dispose components with `is not null`:

```csharp
protected override void Dispose(bool disposing)
{
    if (disposing && components is not null)
    {
        components.Dispose();
    }

    base.Dispose(disposing);
}
```

### Platform attributes

```csharp
[SupportedOSPlatform("windows")]
private void WindowsSpecificMethod() { }
```
