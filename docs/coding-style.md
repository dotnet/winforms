# Coding Style

Our coding style is informed by the [.NET Runtime coding style](https://github.com/dotnet/runtime/blob/master/docs/coding-guidelines/coding-style.md) and the latest ["Framework Design Guidelines"](https://www.informit.com/store/framework-design-guidelines-conventions-idioms-and-9780135896464) (currently third edition).

When we establish guidelines we consider how they impact the ability to comprehend the code. The easier the code is to understand, the easier it is to maintain. There are a few core guiding principles:

- Code should be clear and unambiguous.
- Code should be efficient and concise, using the minimum amount of code necessary to achieve the desired functionality without sacrificing readability or maintainability (and as such we lean into new C# language features).
- Guidelines are mutable where they inhibit our guiding principles.

There are a few key things we strive for to attain our clarity goals:

- Minimal code block nesting.
- Minimal scrolling for code blocks (almost no horizontal and limited vertical).
- Minimal code visibility (via access modifiers, type nesting, and local functions).
- All context captured in source (via code or comments).

Windows Forms dates back to the mid 1990s (originally [Windows Foundation Classes](https://web.archive.org/web/20050915003238/http://msdn.microsoft.com/library/en-us/vjcore98/html/vjconintroductiontowfcprogramming.asp)). It has seen over 25 years of language and coding evolution. We believe that updating the code to modern style makes the code easier to maintain and update in the longer term. While we do not update everything as the style guidelines evolve, we do make continual investments in cleaning the codebase as an engineering best practice.

A number of the following detailed guidelines are captured in our `.editorconfig` and as such you'll get the appropriate feedback in Visual Studio. We're iterating to enable more automated analyzers in the future.

### Spacing

Enabling viewing white space will make conforming to these rules easier. In Visual Studio: "View White Space (Ctrl+R, Ctrl+W)" or "Edit -> Advanced -> View White Space".

1. Use four spaces of indentation (never use tabs). XML blocks should get a single space indent (this includes XML comments).
1. Lines should not have trailing white space or more than one space between code elements (`=` can be aligned when there is notable value in doing so, such as bit flag values).
1. Avoid more than one empty line at any time. For example, do not have two blank lines between members of a type.
1. Closing brackets (`}`) on a line by themself should get a blank line after them unless followed by another line with just a closing bracket.

### Line Breaks

1. Lines should be less than 120 characters long to limit horizontal scrolling in development tools. If it improves clarity, they can be slightly over, but they never should be longer than 150. Statements should not be broken into multiple lines when they fall under the 120 character limit.
1. When using expression body definitions (`=>`) they should be on the same line if they fit, otherwise they should be broken after the `=>` and indented once on the next line.
1. When breaking arguments to a method, all arguments should be indented on individual lines at one indent in.
1. When breaking logical statements, all logical operators (`&&`, `||`, etc.) at a given parenthetical scope should be indented on individual lines at one indent in.
1. When breaking ternary operators (`?:`), they should be broken into two singly indented lines, starting with `?` and `:`.

<details>
<summary>Code Samples</summary>

More complicated line breaks:

``` C#
if (_trackColumn != previousColumnIndex
    && !(previousColumnIndex == -1 && hti._col == _trackColumn)
    && (dataGridViewColumnNext is null || _trackColumn != dataGridViewColumnNext.Index))
```

``` C#
if (Focused
    && !IsCurrentCellInEditMode
    && (EditMode == DataGridViewEditMode.EditOnEnter
        || (EditMode != DataGridViewEditMode.EditProgrammatically && CurrentCellInternal.EditType is null)))
```

Why `=>` isn't at the beginning of a line:

``` C#
// Further breaks logically align
internal bool SingleVerticalBorderAdded =>
    !_layout.RowHeadersVisible
    && (AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single
        || CellBorderStyle == DataGridViewCellBorderStyle.SingleVertical);

// Same example, broken alignment
internal bool SingleVerticalBorderAdded
    => !_layout.RowHeadersVisible
    && (AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single
        || CellBorderStyle == DataGridViewCellBorderStyle.SingleVertical);

```
</details>

### Code Blocks

1. Use [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style) braces, where each brace begins on a new line.
1. `if` statements must use code blocks with the exception of single line parameter validation at the beginning of a method.
1. `using` statements must use code blocks. [Simple using declarations](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/ide0063) (which don't use code blocks) are preferred over using statements when possible. In the rare case that there are multiple `using` statements in a row, they should share a code block to reduce nesting.
1. `fixed` statements must use code blocks. When there are multiple `using` statements in a row, they should share a code block to reduce nesting.
1. `unsafe` should generally be applied to either whole methods or classes (when many methods are unsafe). When unsafe blocks are strictly necessary (say in an async or yield method) they must use code blocks.

### Comments

1. Code comments should be on their own line and precede the code they refer to. In condition blocks, comments should be inside the blocks (e.g., never before the `else`). Switch expression conditions have no blocks and should have the comment before the relevant condition line.
1. Avoid comments labeling the end of a block (never `if {} else {} // else`).
1. Methods and properties should have XML comments, not `\\` comments. `public` API comments should align with the documentation on https://learn.microsoft.com.
1. XML comment blocks that provide no further clarification are not required, with the exception of public API. (You don't have to document the return value if it is obvious, for example.)
1. Do not replicate comments from base methods or interfaces. Use `\\\ <inheritdoc cref='interfacemethod'\>` when docs aren't automatically inherited or you need to modify only a part of the documentation.
1. Avoid `\* *\` comments.

### General Naming

1. Avoid abbreviations and type names for variable or property names (e.g. `Point firstPoint` not `pt1`).
1. Names should describe their use, not their type (e.g. `Rectangle bounds` over `Rectangle rectangle`).
1. Use ```nameof(...)``` instead of ```"..."``` whenever possible and relevant.

### General Visibility

1. Always specify the visibility, even if it's the default (e.g. `private string _foo` not `string _foo`). Visibility should be the first modifier (e.g. `public abstract` not `abstract public`).

### Namespace Usings

1. Namespace `using`s should be specified at the top of the file, before `namespace` declarations, and should be sorted alphabetically, with the exception of `System.*` namespaces, which are to be placed on top of all others.
1. Global usings go should in a file called `globalusings.cs`.

### Namespaces

1. Use [file-scoped namespaces](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/namespace).
1. Files should be in sub folders that match the namespaces. Additional organizational folders can be created within namespace folders when necessary for clarity.

### Types

1. All types should be in their own files. This includes nested types.
1. All internal and private classes should be sealed when they are not derived from.
1. Use Pascal Casing to name all types. The only exception is for interop types, which should match the native casing.
1. Consider [primary constructors](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/instance-constructors#primary-constructors) for "simple" structs and classes where there is only one constructor with no logic other than field assignment.

### Fields

1. Type constants, statics, and fields should be specified at the top within the type declaration.
1. Use [`_camelCase`](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/identifier-names#camel-case) for internal and private fields and use `readonly` where possible. Prefix instance fields with `_`, static fields with `s_` and thread static fields with `t_`.
1. Use [`PascalCasing`](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/identifier-names#pascal-case) to name all constants. The only exception is for interop code where the constant value should match the naming in the native code.
1. `public` or `internal` fields should not be used (use properties).
1. `protected` properties should be preferred over fields.

### Properties

1. Use [`PascalCasing`](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/identifier-names#pascal-case) to name all properties.
1. Use [auto-implemented properties](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties) when possible.
1. Prefer methods over properties for expensive code or code with significant side effects.
1. Use [expression body definitions](https://learn.microsoft.com/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members) (`=>`) for single line getters and setters.

### Methods

1. Use [`PascalCasing`](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/identifier-names#pascal-case) to name all methods.
1. Constructors should precede all other methods and properties.
1. Use [expression body definitions](https://learn.microsoft.com/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members) (`=>`) for single line methods.
1. Avoid methods with a single calling method. Use [local functions](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/local-functions). Don't use a method at all in these cases if the logic is simple and not replicated.
1. Use static methods when instance fields are not accessed, and consider passing fields in to make methods static to clarify what a method modifies.
1. Prefer methods where top level blocks fit in a single editor screen (around 25 lines of code). Local functions can be used to break the logic into digestible chunks.
1. Keep block nesting to a minimum (preferably no more than 3 indents, 5 at the maximum). Use early outs and inverted conditionals to help manage nesting.
1. Exit (return) early from methods to reduce nesting.

### Variables

1. We never use `var` for [built-in types](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/built-in-types), e.g. don't use `var count = 2`, instead use `int count = 2`. `var` should only be used when the type name inhibits code readability.
1. [Target-typed `new()`](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/new-operator#target-typed-new) should be used to remove redundancy, not var. For example: `FileStream stream = new(...);`, not `var stream = new FileStream(...);`. Using target-typed `new()` for succinctness is allowed where the type is reasonably obvious, e.g. `Point[] points = [new(1, 2), new(5, 6)];`
1. [Collection expressions](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/collection-expressions) are preferred, e.g. `Point[] points = [new(1, 2)];` over `Point[] points = new Point[] { new(1, 2) }`;

### General

1. Use language keywords instead of BCL types (e.g. `int, string, float` instead of `Int32, String, Single`, etc.) for both type references as well as method calls (e.g. `int.Parse` instead of `Int32.Parse`).
1. Always use [interpolated strings](https://learn.microsoft.com/dotnet/csharp/language-reference/tokens/interpolated) when composing / formatting strings.
1. Use [raw string literals](https://learn.microsoft.com/dotnet/csharp/language-reference/tokens/raw-string) for multi-line strings.

### Conditions

1. Prefer [ternary operators](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/conditional-operator) (`?:`) over `if .. else` when there is only one line in each clause.
1. Prefer [pattern matching](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns) in conditions.
1. Prefer [null-coalescing](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/null-coalescing-operator) operators (`??` and `??=`).
1. Prefer [switch expressions](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/switch-expression) over switch statements where possible.

### Nullability

1. Prefer [pattern matching](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns) to check variable type and null state (e.g. `if (parameter is Form form)`, `if (manager.GetService<IContainer>() is { } container)`, etc.).
1. Using the postfix `!` [null-forgiving operator](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/null-forgiving) to override null analysis must always get a descriptive comment. Prefer to refactor code to avoid this where possible. If we do not control the code in question prefer to throw `ArgumentException` when an associated argument is known or `InvalidOperationException` otherwise. Do not let code fall into `NullReferenceException`.

### Visual Basic

_Forthcoming_