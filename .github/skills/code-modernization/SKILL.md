---
name: code-modernization
description: >-
  Instructions for modernizing and refactoring existing C# / VB.NET code files.
  Use when asked to refactor, modernize, clean up, review, or improve existing
  source files in this repository. Covers upgrading to C# 14 / .NET 10 idioms,
  comment quality, spelling and grammar fixes, XML documentation, and
  readability improvements.
metadata:
  author: dotnet-winforms
  version: "1.0"
---

# Code Modernization — Refactoring Existing Code

These rules apply when **refactoring or modernizing existing** C# or VB.NET
source files. For generating *new* files from scratch, see the
`coding-standards` skill instead.

> **Golden rule:** Make only high-confidence changes. If a transformation could
> alter runtime semantics and you are not 100 % certain it is safe, leave the
> code as-is and add a `// TODO:` comment explaining the potential improvement.

## Scope and Safety

* Default target is **.NET 10.0** — but verify the project's actual target
  before applying C# 14 syntax.
* Multi-targeted and .NET Framework files exist — do not blindly apply modern
  syntax that would break older targets.
* Do not edit files under `eng/common/`.
* Respect `.editorconfig` settings.

## Modernizing Language Constructs to C# 14

Apply the following transformations when safe:

### Namespace and usings

* Convert block-scoped namespaces → **file-scoped** namespaces.
* Remove `using` directives that are covered by the project's global usings.

### Nullable Reference Types

* Add NRT annotations where missing. Mark `object? sender` in event handlers.
* Replace `== null` / `!= null` with `is null` / `is not null`.
* Introduce `??` and `??=` for null-coalescing where it simplifies the code.
* Apply null-conditional assignment where appropriate:

```csharp
// Before
if (customer is not null)
{
    customer.Order = GetCurrentOrder();
}

// After (C# 14)
customer?.Order = GetCurrentOrder();
```

### Type usage and `var` policy

* Replace `var` with **explicit type names** for primitive types and any
  declaration where the type is not immediately obvious.

* **Keep `var`** only when the declared type would be overly complex (deeply
  nested generics, generic tuples) or when it is obviously redundant with the
  variable name — e.g., pooled list or dictionary factory methods whose return
  type is self-evident from context.

* Convert `new TypeName()` on the right-hand side to **target-typed `new()`**
  when the left-hand side already declares the type:

```csharp
// Before
Dictionary<string, List<int>> map = new Dictionary<string, List<int>>();

// After
Dictionary<string, List<int>> map = new();
```

* Modernize collection initializers:

```csharp
// Before
List<string> items = new List<string>();

// After
List<string> items = [];
```

### The `field` keyword (C# 14)

Where a property has a manually declared backing field solely for simple
validation or transformation, consider converting to the `field` keyword:

```csharp
// Before
private string _message;
public string Message
{
    get => _message;
    set => _message = value ?? throw new ArgumentNullException(nameof(value));
}

// After
public string Message
{
    get;
    set => field = value ?? throw new ArgumentNullException(nameof(value));
}
```

Only apply when the backing field is not accessed from anywhere else in the
class.

### Extension members

When refactoring extension method classes, consider whether the new **extension
block** syntax improves clarity:

```csharp
// Before
public static class StringExtensions
{
    public static bool IsBlank(this string value)
        => string.IsNullOrWhiteSpace(value);
}

// After
public static class StringExtensions
{
    extension(string value)
    {
        public bool IsBlank()
            => string.IsNullOrWhiteSpace(value);
    }
}
```

### Pattern matching and switch expressions

* Convert `if`-`else if` chains that compare the same variable
  → **switch expressions**.

* Replace `is` + cast → pattern variable: `if (obj is Control c && c.Visible)`.

* Use `and`, `or`, relational, property, tuple, type, and list patterns
  where they eliminate temporary variables or nested conditions.

### Expression-bodied members

Convert single-expression methods and read-only properties to expression bodies.
When the **total line length would exceed 60 characters**, place the `=>`
on the next line:

```csharp
// Before
internal int BorderWidth
{
    get { return _borderWidth; }
}

// After — short
internal int BorderWidth => _borderWidth;

// After — long (arrow wraps)
private bool IsValidSize(Size size)
    => size.Width > 0 && size.Height > 0;
```

> **Semantic hazard:** `public Foo Bar => new Foo();` creates a new instance on
> every access, while `public Foo Bar { get; } = new Foo();` creates one
> instance at construction time. **Never** convert between these forms unless
> the original semantics were provably incorrect. When you encounter an
> expression-bodied property that returns a new instance, add a comment
> documenting that the per-access instantiation is intentional.

### Ternary operator

Refactor verbose `if` / `else` assignment blocks to ternary, with each branch
on its own line:

```csharp
Color textColor = e.Item.Enabled
    ? GetDarkModeColor(e.TextColor)
    : GetDarkModeColor(SystemColors.GrayText);
```

### Error handling

Replace hand-rolled null / range checks with throw helpers:

```csharp
// Before
if (parameter is null) throw new ArgumentNullException(nameof(parameter));

// After
ArgumentNullException.ThrowIfNull(parameter);
```

Also: `ArgumentOutOfRangeException.ThrowIfNegative`,
`ObjectDisposedException.ThrowIf`.

### Performance micro-upgrades

* `text.Contains("x")` → `text.Contains('x')` (single-char overload).
* `sb.Append("x")` → `sb.Append('x')`.
* `Substring` → `AsSpan` / `ReadOnlySpan<char>` where the substring is
  consumed without allocation.
* `Count() > 0` → `Any()`.
* `ContainsKey` + indexer → `TryGetValue`.
* `new T[0]` → `Array.Empty<T>()`.
* Traditional `using` blocks → `using` declarations unless a tighter scope is
  genuinely needed.

### Accessibility and modifiers

* Add missing explicit access modifiers.
* Narrow scope where possible (`internal` → `private`, etc.).
* Add `static` to members that do not use instance state.
* Remove unnecessary `this.` qualifications.

## Comment Quality and Cleanup

### Spelling and grammar

* Check **every existing comment** for correct spelling and grammar.
* Fix typographical errors, incorrect punctuation, and awkward phrasing.
* **Remove double spaces after punctuation** — use a single space after periods,
  colons, semicolons, question marks, and exclamation marks.
* Remove or rephrase inappropriate comments or microaggressions.

### Preserving and improving comments

* **Never delete comments** that carry necessary information or genuinely help
  the reader — refactor them to be more precise and clear instead.
* For long, complex code blocks, **insert concise, helpful comments** at
  strategic points (before non-obvious logic, at phase boundaries, before tricky
  calculations) so that developers unfamiliar with the codebase can follow the
  flow.
* Keep comments factual and professional. Avoid humor that ages poorly.

## XML Documentation

### Class-level documentation

**Every class, struct, record, interface, and enum** — regardless of access
modifier, including `private` and `private protected` nested types — must have
an XML doc header:

* **Short classes** (< ~50 lines): a `<summary>` that explains the type's
  purpose is sufficient.

* **Longer / complex classes**: add a `<remarks>` section with `<para>` blocks
  describing design rationale, usage patterns, threading considerations, or
  important invariants.

```csharp
/// <summary>
///  Manages the lifetime and caching of GDI+ brush objects
///  used for dark-mode rendering.
/// </summary>
/// <remarks>
///  <para>
///   Brushes are pooled per-thread to avoid contention on the
///   GDI+ shared state. Call <see cref="Return"/> to release a
///   brush back to the pool.
///  </para>
///  <para>
///   This class is not thread-safe across threads; each thread
///   maintains its own pool via <c>[ThreadStatic]</c> storage.
///  </para>
/// </remarks>
internal class DarkModeBrushCache
{
    // ...
}
```

### Member documentation

* Use `<inheritdoc/>` on overridden or interface-implemented members.
* Do **not** XML-comment local functions — use a regular `//` comment if
  needed.
* Use **Unicode characters** in XML docs, never HTML entities.
* Indent XML structure with **1 space** per nesting level.

## Formatting Cleanup

* Convert block-scoped namespaces → file-scoped.
* Ensure **Allman-style** braces.
* Insert empty lines after closing braces of control-flow blocks and before
  `return` statements. If a comment precedes a line requiring spacing, the
  empty line goes before the comment.
* 4-space indentation, no tabs.
* Remove trailing whitespace; ensure final newline.
* UTF-8 with BOM, CRLF line endings.

## WinForms-Specific Modernization

* Nullable event handlers: `public event EventHandler<EventArgs>? Click;`
* NRT-aware handler signatures: `private void OnFoo(object? sender, EventArgs e)`
* Use `EventArgs.Empty` for parameterless raises.
* Designer control fields as nullable: `private Button? _okButton;`
* Dispose with `is not null`:

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

* Platform-specific methods: `[SupportedOSPlatform("windows")]`.
* Prefer `using` declarations for GDI+ cached scopes:

```csharp
using var brush = backColor.GetCachedSolidBrushScope();
```

## Refactoring Checklist

Before submitting a modernized file, verify:

- [ ] No `var` on primitive types
- [ ] No `== null` / `!= null` — only `is null` / `is not null`
- [ ] No missing access modifiers
- [ ] No `this.` unless required for disambiguation
- [ ] No hand-rolled null/range checks — throw helpers used
- [ ] Comments: spelling, grammar, single space after punctuation
- [ ] All classes have XML doc headers (including private nested types)
- [ ] Expression-bodied `=>` wraps to next line when total exceeds 60 chars
- [ ] Empty line before `return`, after closing braces of blocks
- [ ] No magic numbers without named constants
