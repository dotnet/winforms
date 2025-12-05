# General

* In the main branch, write code that runs on .NET 10.0, but keep in mind that we have a couple of.NET Framework and multi-targeted code files.
* Make only high-confidence suggestions when reviewing code changes.
* Do not edit files in `eng/common/`.
* Apply guidelines defined in `.editorconfig`.

## 1. Code Style and Quality Guidelines

### 1.1 C# Language Features and Modern Patterns

- Use C# 13 features and patterns throughout the codebase. In particular, focus on these general issues:

  * **Namespace-Imports**: Assume, the standard using/imports for importing namespaces for the respective areas are defined in a global using file.
  * **Null-references types**: Assume, for complete new code files you generated, that Null-Reference-Types are enabled.
  * **Namespace definition for code-files**: Make sure, to generate or to refactor to file-scoped namespace definitions.

- Apply Nullable Reference Types (NRT) consistently. Note, that this also counts for the `sender` (object?) parameter in the event signatures, like this one:

  ```csharp
  // Update event handler signatures
  public event EventHandler<EventArgs>? SomeEvent;
  private void OnSomeEvent(object? sender, EventArgs e) { ... }
  ```

- **Always insert empty lines after structure blocks and before `return` statements**

- Use pattern matching, `is`, `and`, `or`, and switch expressions where applicable

  ```csharp
  if (obj is Control control && control.Visible)
  {
      // Use 'control' variable
  }
  ```

- Refactor or use the ternary operator where possible, but make sure that you put each branch on its own line, except if the expression is very short:

    ```csharp
    // Original code
    textColor = e.Item.Enabled ? GetDarkModeColor(e.TextColor) : GetDarkModeColor(SystemColors.GrayText);

    // or:
    if (e.Item.Enabled)
    {
        textColor = GetDarkModeColor(e.TextColor);
    }
    else
    {
        textColor = GetDarkModeColor(SystemColors.GrayText);
    }
    ```

    becomes:

    ```csharp
    // Use ternary operator with line breaks for clarity
    textColor = e.Item.Enabled 
        ? GetDarkModeColor(e.TextColor) 
        : GetDarkModeColor(SystemColors.GrayText);
    ```

- - Prefer `switch` expression over switch statements over chains of `if` statements, where if makes sense:

  ```csharp
  // Prefer switch expressions over switch statements
  string result = value switch
  {
      > 0 => "Positive",
      < 0 => "Negative", 
      _ => "Zero"
  };
  ```

  Chain of If-statements can often be converted like so:

    ```csharp
    // Original code
    private static Color GetDarkModeColor(Color color)
    {
        if (color == SystemColors.Control)
            return Color.FromArgb(45, 45, 45);
        if (color == SystemColors.ControlLight)
            return Color.FromArgb(60, 60, 60);
        if (color == SystemColors.ControlDark)
            return Color.FromArgb(30, 30, 30);
        if (color == SystemColors.ControlText)
            return Color.FromArgb(240, 240, 240);
        // For any other colors, darken them if they're bright
        if (color.GetBrightness() > 0.5)
        {
            // Create a darker version for light colors
            return ControlPaint.Dark(color, 0.2f);
        }

        return color;
    }
    ```

    becomes:

    ```csharp
    // Use switch expression for color handling
    private static Color GetDarkModeColor(Color color) =>
        color switch
        {
            Color c when c == SystemColors.Control => Color.FromArgb(45, 45, 45),
            Color c when c == SystemColors.ControlLight => Color.FromArgb(60, 60, 60),
            Color c when c == SystemColors.ControlDark => Color.FromArgb(30, 30, 30),
            _ when color.GetBrightness() > 0.5 => ControlPaint.Dark(color, 0.2f),
            _ => color
        };
    ```

    Other examples for switch expressions are:
    // Multiple conditions with 'and'/'or' patterns
    string GetFileCategory(string extension) => extension.ToLower() switch
    {
        ".jpg" or ".png" or ".gif" or ".bmp" => "Image",
        ".mp4" or ".avi" or ".mkv" or ".mov" => "Video", 
        ".txt" or ".md" or ".csv" => "Text",
        string ext when ext.StartsWith(".doc") => "Document",
        _ => "Unknown"
    };

    // Tuple patterns for multiple parameters
    string GetQuadrant(int x, int y) => (x, y) switch
    {
        (> 0, > 0) => "First",
        (< 0, > 0) => "Second", 
        (< 0, < 0) => "Third",
        (> 0, < 0) => "Fourth",
        _ => "Origin or Axis"
    };

    // Property patterns with nested conditions
    decimal CalculateShipping(Order order) => order switch
    {
        { Weight: > 50, IsPriority: true } => 25.00m,
        { Weight: > 50 } => 15.00m,
        { IsPriority: true, Total: > 100 } => 5.00m,
        { Total: > 100 } => 0m,
        _ => 7.50m
    };

    // Type patterns with casting
    string ProcessValue(object value) => value switch
    {
        int i when i > 0 => $"Positive: {i}",
        int i when i < 0 => $"Negative: {i}",
        string s when !string.IsNullOrEmpty(s) => $"Text: {s}",
        bool b => b ? "True" : "False",
        null => "Null value",
        _ => "Unknown type"
    };

    // Range patterns (C# 11+)
    string GetAgeGroup(int age) => age switch
    {
        < 13 => "Child",
        >= 13 and < 20 => "Teen", 
        >= 20 and < 65 => "Adult",
        >= 65 => "Senior",
        _ => "Invalid"
    };

    // List patterns (C# 11+) - great for method parameter validation
    string AnalyzeList<T>(List<T> items) => items switch
    {
        [] => "Empty",
        [var single] => $"Single item: {single}",
        [var first, var second] => $"Two items: {first}, {second}",
        [var first, .., var last] => $"Multiple items from {first} to {last}",
    };

    // Enum with flags - often overlooked refactoring opportunity
    string GetPermissionDescription(FilePermissions permissions) => permissions switch
    {
        FilePermissions.Read => "Read only",
        FilePermissions.Write => "Write only", 
        FilePermissions.Read | FilePermissions.Write => "Read and Write",
        FilePermissions.Read | FilePermissions.Execute => "Read and Execute",
        FilePermissions.All => "Full access",
        _ => "No permissions"
    };

    // Refactoring complex validation chains
    ValidationResult ValidateUser(User user) => user switch
    {
        null => ValidationResult.Error("User cannot be null"),
        { Email: null or "" } => ValidationResult.Error("Email required"),
        { Email: string email } when !email.Contains('@') => ValidationResult.Error("Invalid email"),
        { Age: < 13 } => ValidationResult.Error("Must be 13 or older"),
        { Name.Length: < 2 } => ValidationResult.Error("Name too short"),
        _ => ValidationResult.Success()
    };

- **Prefer modern collection initializers**: Use `[]` syntax for collections

  ```csharp
  List<string> items = [];           // Preferred
  List<string> items = new();        // Also acceptable
  List<string> items = new List<string>(); // Avoid
  ```

### 1.2 Type Usage and Null Checking

- **Use explicit type names for primitive types** (enforced as error)
  ```csharp
  int count = 5;           // Required
  string name = "Button";  // Required
  bool isVisible = true;   // Required
  var index = 0;          // Error - use 'int index = 0;'
  ```

- **Use `var` only for complex types when type is apparent from context**
  ```csharp
  var control = new Button();              // OK - type is obvious, but better is:
  Button control = new();                  // Preferred
  
  // OK - long type name
  var items = GetComplexCollectionWithRidiculousLongTypeName();
  int primitiveValue = 42;                 // Required for primitives
  ```

- **Prefer `is null` and `is not null` over reference equality** (enforced as error)
  ```csharp
  if (value is null) { ... }               // Required
  if (value is not null) { ... }          // Required
  if (value == null) { ... }              // Error
  ```

- **Use coalesce expressions for null handling**
  ```csharp
  string result = input ?? "default";     // Preferred
  value ??= GetDefaultValue();            // For null assignment
  ```

### 1.3 Naming and Field Conventions

- **Prefix static fields with `s_`** (suggestion level)
  ```csharp
  private static readonly int s_defaultBorderWidth = 1;
  ```
An exception to this rule is when the field is attributed with `[ThreadStatic]`, in which case the prefix should be `t_`:
  ```csharp
  [ThreadStatic]
  private static int t_threadLocalValue;
  ```

- **Prefix private/internal instance fields with `_`** (suggestion level)
  ```csharp
  private int _borderWidth;
  private string _controlName;
  ```

- **Use PascalCase for constants** (suggestion level)
  ```csharp
  public const int DefaultWidth = 100;
  private const string DefaultText = "Button";
  ```

- **Avoid `this.` qualification unless absolutely necessary** (enforced as error)
  ```csharp
  _fieldName = value;      // Correct
  this._fieldName = value; // Error
  PropertyName = value;    // Correct  
  this.PropertyName = value; // Error
  ```

### 1.4 Visibility and Method Organization

- **Use the narrowest possible scope for classes, methods, and fields**
  - Prefer `private` over `internal` over `public`
  - Consider `private protected` for overridable members in internal classes

- **Declare accessibility modifiers explicitly** (enforced as error)
  ```csharp
  internal class MyControl { ... }         // Required
  private void HandleClick() { ... }      // Required
  class MyControl { ... }                 // Error - missing 'internal'
  void HandleClick() { ... }              // Error - missing 'private'
  ```

- **Mark members as `static` when they don't access instance state** (warning level)
  ```csharp
  private static bool IsValidInput(string input) => !string.IsNullOrEmpty(input);
  ```

- **Use expression bodies for simple properties and single-line methods**
  ```csharp
  internal int BorderWidth => _borderWidth;
  
  private bool IsVisible() => Visible && Parent?.Visible == true;
  ```

For longer expressions, use line breaks with proper alignment:
  ```csharp
  internal int SomeFooIntegerProperty =>
      _someFooIntegerProperty;
      
  private bool IsValidSize(Size size) =>
      size.Width > 0 
      && size.Height > 0;
  ```
**IMPORTANT**

In C#, `public Foo FooInstance => new Foo();` and `public Foo FooInstance { get; } = new Foo();` are NOT interchangeable.

* The first is an expression-bodied property that returns a new value each time it is accessed.
* The second is a read-only auto-property with initializer that returns the same value every time, created once.

When editing, generating, or suggesting C# properties, always maintain the original intent:

* Use an expression-bodied property (`=>`) for computed or dynamic values.
* Use an auto-property with initializer (`{ get; } = ...`) for stored, initialized-once values.

Never "correct" or convert one style to the other unless you are certain the semantic behavior is intended to change because it was wrong with 100% confidence to begin with. If you encounter a case, where you have a expression-bodied property that returns a new instance, and you encounter this in a code review, you should add a comment explaining why this is intended, and that it is not a mistake.

### 1.5 Error Handling and Argument Validation

- **Use argument validation throw helpers** (enforced as error)
  ```csharp
  ArgumentNullException.ThrowIfNull(parameter);           // Required
  ArgumentOutOfRangeException.ThrowIfNegative(value);     // Required
  ObjectDisposedException.ThrowIf(_disposed, this);       // Required
  
  // Instead of traditional patterns:
  if (parameter is null) throw new ArgumentNullException(nameof(parameter)); // Avoid
  ```

- **Use `nameof` instead of string literals** (enforced as error)
  ```csharp
  throw new ArgumentException($"Invalid {nameof(parameter)}", nameof(parameter));
  Debug.Assert(value > 0, $"{nameof(value)} must be positive");
  ```

### 1.6 Performance Optimizations

- **Prefer performance-optimized string operations** (enforced as error)
  ```csharp
  text.Contains('c');                    // Required for single chars
  text.Contains("char");                 // Error - use char overload
  
  StringBuilder.Append('x');             // Required for single chars  
  StringBuilder.Append("x");             // Error - use char overload
  
  ReadOnlySpan<char> span = text.AsSpan(0, 10); // Preferred over Substring
  ```

- **Use LINQ efficiently**
  ```csharp
  items.Any();                          // Preferred over Count() > 0
  items.FirstOrDefault();               // Preferred over Where().First()
  collection.TryGetValue(key, out var value); // Preferred over ContainsKey + indexer
  ```

- **Avoid unnecessary allocations**
  ```csharp
  string.IsNullOrEmpty(text);          // Preferred over text?.Length == 0
  Array.Empty<string>();               // Preferred over new string[0]
  ```

### 1.7 Control-Specific Patterns

- **Handle platform-specific code appropriately**
  ```csharp
  [SupportedOSPlatform("windows")]
  private void WindowsSpecificMethod() 
  { 
      ... 
  }
  ```

- **Use proper disposal patterns**
Prefer the modern C# "using declaration" syntax (using var ...) over the traditional using statement block.
Use:

```csharp
using var brush = backColor.GetCachedSolidBrushScope();
```

instead of:

```csharp
using (SolidBrushCache.Scope brush = backColor.GetCachedSolidBrushScope())
{
    // ...
}
```

This makes code more concise, reduces nesting, and improves readability. Only use the older style if you need to limit the scope of the variable to a smaller section within a larger method, or when the using var pattern is not supported. **Note**, that if you use `using` with the type name over `var` for new instances that you can omit the type name after the `new` like:

```csharp
using Pen focusPen = new(focusColor) // Use 'using' with type name and omit type after 'new'
{
    Width = 1.0f,
    DashStyle = DashStyle.Dot
};
```

### 1.8 XML Documentation Standards

- **Format XML comments with single-space indentation and proper structure**
  ```csharp
  /// <summary>
  ///  Gets or sets the border width of the control.
  /// </summary>
  /// <remarks>
  ///  <para>
  ///   The border width affects the visual appearance of the control.
  ///  </para>
  ///  <para>
  ///   Values must be non-negative. A value of 0 means no border.
  ///  </para>
  /// </remarks>
  /// <param name="value">The border width in pixels.</param>
  /// <returns>The current border width.</returns>
  public int BorderWidth { get; set; }
  ```

- **Use Unicode characters instead of HTML entities in documentation** (enforced as error)

- **Don't XML-comment local functions**, as this is not supported. Instead, be more verbose in a comment if the local function's purpose is not immediately obvious.

- **Use `<inheritdoc/>` for inherited members** to avoid duplication and ensure consistency in documentation.
  ```csharp
  /// <inheritdoc/>
  public override void OnClick(EventArgs e)
  {
      base.OnClick(e);
      // Additional logic here
  }
  ```

### 1.9 File Structure and Formatting

- **Use file-scoped namespaces**
- **Files must have UTF-8 BOM encoding and CRLF line endings**
- **Remove trailing whitespace** (enforced as warning)
- **Insert final newline at end of files**
- **Use 4-space indentation consistently**

- **Place opening braces on new lines** (Allman style)
  ```csharp
  if (condition)
  {
      // Code here
  }
  ```

### 1.10 Magic Numbers

- **Avoid magic numbers**: Use named constants or enums instead of hard-coded values. This improves readability and maintainability.

  ```csharp
  // Avoid magic numbers
  const int MaxItems = 100;
  const double Pi = 3.14159;
  
  // Use named constants or enums
  public enum ColorCode
  {
      Red = 1,
      Green = 2,
      Blue = 3
  }
  
  // Example usage
  ColorCode color = ColorCode.Red;
  ```

### 1.11 Using of empty lines for clarity

- **Use empty lines to separate logical sections of code**: This improves readability and helps to understand the structure of the code. Use them judiciously to avoid cluttering the code.

  ```csharp
  // Example of using empty lines for clarity
  public int SomeMethod()
  {
      if (condition)
      {
          // Do something
      }
      else
      {
         // Do something else.
         // Leave a line after the closing brace for clarity.
      }

      // Initialize variables
      int x = 0;
      int y = 0;
  
      // Perform calculations
      x = CalculateX();
  
      // Update UI, and the return in a separate line.
      UpdateUI(x, y);

      return x;
  }
  ```

  Note: If a code line for which these empty-line rules apply has a comment on top of it, the empty line should be placed before the comment, if there isn't one. And of course, NEVER comment that an empty lines need to be inserted at a spot, which would defeat the purpose of the empty line.
  

### 1.12 Refactoring of existing comments

- **Refactor existing comments**: When refactoring code, also consider updating or removing outdated comments. Comments should accurately reflect the current state of the code and its purpose. Avoid/rephrase inappropriate comments or microaggressions.

- Do not delete comments, though, when they carry either necessary information or are helpful for the reader. Instead, refactor them to be more precise and clear.

## 2. WinForms-Specific Guidelines

### 2.1 Event Handling

- **Use nullable event handlers consistently**
  ```csharp
  public event EventHandler<EventArgs>? Click;
  
  protected virtual void OnClick(EventArgs e)
  {
      Click?.Invoke(this, e);
  }
  ```

- **Use `EventArgs.Empty` for empty event arguments**
  ```csharp
  protected virtual void OnPaint(PaintEventArgs e)
  {
      Paint?.Invoke(this, EventArgs.Empty);
  }
  ```

- Take into account, that the typical Event handler signature has been modified for NRT:
    ```csharp
    private void OnClick(object? sender, EventArgs e) { ... }
    ```


### 2.2 Designer Integration

- **Mark designer-generated fields appropriately**
  ```csharp
  private Button? _okButton;  // For designer-generated controls
  ```

- **Use proper component disposal in designer code**
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
  