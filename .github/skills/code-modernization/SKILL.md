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
  version: "1.1"
---

# Code Modernization — Refactoring Existing Code

These rules apply when **refactoring or modernizing existing** C# or VB.NET
source files. For generating *new* files from scratch, see the
`coding-standards` skill instead.

> **Golden rule:** Make only high-confidence changes. If a transformation could
> alter runtime semantics and you are not 100 % certain it is safe, leave the
> code as-is and add a `// TODO:` comment explaining the potential improvement.

## Scope and Safety

* Default target is **.NET 10.0** — verify the project's actual target before
  applying C# 14 syntax.
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

* For Framework-targeted projects, only refactor to NRTs if the whole project
  is already set up to process them, or if the code file has an explicit
  `#nullable enable` at the top. Do not add `#nullable enable` to files that
  do not already have it.
* If neither condition is met, leave the file as-is — the risk of nullability
  bugs in a non-NRT-aware codebase outweighs incremental annotation.
* Add NRT annotations where missing. Mark `object? sender` in event handlers.

### Testing for null

* **Always replace `== null` / `!= null` with `is null` / `is not null`** —
  the pattern-matching forms cannot be overridden by custom `==` / `!=`
  operators and are preferred for consistency.
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

Apply the following rules **in priority order**:

1. **Never use `var` for primitive types.** Always spell out `int`, `string`,
   `bool`, `double`, `float`, `decimal`, `char`, `byte`, `long`, etc.

```csharp
// Before
var count = items.Length;
var name = component.Site.Name;
var isVisible = control.Visible;

// After
int count = items.Length;
string name = component.Site.Name;
bool isVisible = control.Visible;
```

2. **Keep (or introduce) `var` when the type is already visible or clearly
   implied on the same line** (repeating the type adds noise). This applies to:

   * **Casts:** `var foo = (IDesignerHostShim)designerHost;`
   * **`as` casts:** `var button = toolStripItem as ToolStripDropDownButton;`
   * **Generic methods with explicit type argument(s):** The `<T>` already
     tells the reader the type, even if the method signature technically
     returns a base type:
     `var host = this.GetService<IDesignerHost>();`
     `var session = provider.GetRequiredService<DesignerSession>();`
   * **`out var` in generic methods that name the type:**
     `site.TryGetService<INestedContainer>(out var container)` — the `<T>`
     already specifies the type.
   * **Methods whose name implies the return type:**
     `var componentType = component.GetType();`
     `var resourceStream = BitmapSelector.GetResourceStream(type, name);`
     `TryLoadBitmapFromStream(stream, out var resourceBitmap)`

```csharp
// Before (redundant repetition)
IDesignerHostShim designerHostShim = (IDesignerHostShim)designerHost;
IDesignerHost host = this.GetService<IDesignerHost>();
ViewModelClientFactoryManager manager = client.CompositionHost.GetExport<ViewModelClientFactoryManager>();
Type componentType = component.GetType();
Stream resourceStream = BitmapSelector.GetResourceStream(componentType, componentType.Name + ".bmp");

// After (var — type is visible or clearly implied on the line)
var designerHostShim = (IDesignerHostShim)designerHost;
var host = this.GetService<IDesignerHost>();
var manager = client.CompositionHost.GetExport<ViewModelClientFactoryManager>();
var componentType = component.GetType();
var resourceStream = BitmapSelector.GetResourceStream(componentType, componentType.Name + ".bmp");
```

3. **Use `var` for deeply nested or complex generic types** where the full type
   name is unwieldy and the variable name already communicates intent.

```csharp
// var improves readability for complex generics
using var pooledList = ListPool<IComponent>.GetPooledObject();
var result = pooledList.Object;
```

4. **Use explicit types when neither the variable name nor the surrounding
   context reveals what type is in play.** If a reader would have to navigate
   to a method signature to understand what a variable holds, spell it out.

```csharp
// Before (what is result? what does GetConfiguration return?)
var result = ProcessInput(data);
var config = serviceProvider.GetConfiguration();
var response = session.GetWinFormsEndpoints().DocumentOutline.CreateViewModel(session.Id);

// After
ValidationOutcome result = ProcessInput(data);
AppConfiguration config = serviceProvider.GetConfiguration();
CreateViewModelResponse response = session.GetWinFormsEndpoints().DocumentOutline.CreateViewModel(session.Id);
```

5. **Prefer target-typed `new()` over `var`** when the type is visible on the
   left — clean construction without redundancy:

```csharp
// Before
Dictionary<string, List<int>> map = new Dictionary<string, List<int>>();
var map = new Dictionary<string, List<int>>();

// After
Dictionary<string, List<int>> map = new();
Button saveButton = new();
```

**Do NOT use target-typed `new()` when the type isn't visible on the same line:**

```csharp
// DO — type is visible on the right, so var is fine:
var map = new Dictionary<string, List<int>>();

// DON'T — _map is a backing field declared elsewhere.
_map = new();
```

6. **`var` is always fine for tuple deconstruction:**

```csharp
var (nodes, images) = viewModel.UpdateTreeView(displayStyle);
var (key, value) = dictionary.First();
```

7. Modernize collection initializers:

```csharp
// Before
List<string> items = new List<string>();

// After
List<string> items = [];
```

* Consider collection initializers also for methods that return collections:

```csharp
// Before
Control[] controls = _view.Controls.Cast<Control>().ToArray();

// After
Control[] controls = [.. _view.Controls.Cast<Control>()];
```

* Avoid collection initializers when a constructable array type is required:

```csharp
// Will not compile — collection initializer syntax requires
// a constructable array type here.
Control CreateErrorControlForMessage(string message)
   => CreateErrorControl([new InvalidOperationException(message)]);

// Correct:
Control CreateErrorControlForMessage(string message)
   => CreateErrorControl(new[] { new InvalidOperationException(message) });
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

### Readability

1. When modernizing, **never sacrifice readability for brevity.**

Do not collapse multi-line logic into a single dense expression. If the original code is easier to follow across multiple statements, keep it that way.

Prefer extension method call syntax when the same operation is available as both a static call and an extension method — the extension form reads more naturally and reduces visual clutter:

```csharp
// Before (static call)
Size deviceSize = DpiHelper.LogicalToDeviceUnits(image.Size);

// After (extension method)
Size deviceSize = image.Size.LogicalToDeviceUnits();
```

2. **Prefer inline `#pragma` or `[SuppressMessage]`**

Prefer inline `#pragma` or `[SuppressMessage]` at the call site over global suppressions in `GlobalSuppressions.cs`, so justification is visible in context. Only use global suppressions for truly project-wide rules (e.g., legacy threading model decisions that apply everywhere).

3. **Named arguments**

Use named arguments when passing multiple literals or when the meaning of a parameter isn't clear from the argument expression itself:

```csharp
// GOOD — named arguments clarify meaning of literals
var errorControl = CreateErrorControlForMessage(
    message: "An unexpected error occurred. Please try again.",
    showRetryButton: true);
```

   When method calls take a lot of space due to a long argument list, consider
   wrapping individual arguments on separate lines. If using named arguments,
   use them for *every* argument for consistency:

```csharp
LongMethodWithManyNamedArguments(
    firstArgument: value1,
    secondArgument: value2,
    thirdArgument: value3,
    fourthArgument: value4);
```

4. **Wrap dot-chains with more than 2 member accesses** — each call goes on its own indented line:

```csharp
// Fine — 2 or fewer:
var names = items.Where(x => x.IsActive).ToList();

// Wrap — more than 2:
var results = collection
    .Where(x => x.IsActive)
    .OrderBy(x => x.Name)
    .Select(x => x.Id)
    .ToList();
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
> instance at construction time. Never convert between these forms unless the
> original semantics were provably incorrect — instead, add a comment confirming
> per-access instantiation is intentional.

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
  calculations).
* Keep comments factual and professional. Avoid humor that ages poorly.

* Never deconstruct class names into single words for comments.
  **Rule:** Assume a code fragment or a member name, if a term/an expression is formatted in Pascal Case.

```CSharp

// Original comment:

// Ensure API like Type.GetType(...) use the UserAssemblyLoadContext if runtime
// needs to load assemblies. See https://github.com/dotnet/coreclr/blob/master/Documentation/design-docs/AssemblyLoadContext.ContextualReflection.md
// for more information.

// DO NOT:
// Sets contextual reflection to the user assembly load context, when available.

// DO:
// Ensures APIs like Type.GetType(...) use UserAssemblyLoadContext if the runtime
// needs to load assemblies. See https://github.com/dotnet/coreclr/blob/master/Documentation/design-docs/AssemblyLoadContext.ContextualReflection.md

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
* Do **not** XML-comment local functions — use `//` comments instead.
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

* [ ] No `var` on primitive types
* [ ] `var` used when the type is visible or implied on the line (casts,
      generic method calls, `out var` with generic methods, methods whose
      name implies the return type)
* [ ] Target-typed `new()` used only when type is visible on the same line
* [ ] No redundant type repetition on both sides of an assignment
* [ ] Collection initializers applied where safe (not where a constructable
      array type is required)
* [ ] No `== null` / `!= null` — only `is null` / `is not null`
* [ ] `field` keyword applied where backing field is only used by one property
* [ ] No missing access modifiers
* [ ] No `this.` unless required for disambiguation
* [ ] No hand-rolled null/range checks — throw helpers used
* [ ] Extension method syntax preferred over static calls when available
* [ ] Inline `#pragma` / `[SuppressMessage]` at call site, not in
      GlobalSuppressions.cs (unless project-wide)
* [ ] Named arguments used for multiple literals or unclear parameters
* [ ] Dot-chains with more than 2 member accesses wrapped to separate lines
* [ ] Comments: spelling, grammar, single space after punctuation
* [ ] All classes have XML doc headers (including private nested types)
* [ ] Expression-bodied `=>` wraps to next line when total exceeds 60 chars
* [ ] Empty line before `return`, after closing braces of blocks
* [ ] No magic numbers without named constants
