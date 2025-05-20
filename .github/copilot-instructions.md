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
  
  // Prefer switch expressions over switch statements
  string result = value switch
  {
      > 0 => "Positive",
      < 0 => "Negative", 
      _ => "Zero"
  };
  ```

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
  var control = new Button();              // OK - type is obvious
  var items = GetComplexCollection();      // OK - long type name
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

- For longer expressions, use line breaks with proper alignment:
  ```csharp
  internal int SomeFooIntegerProperty =>
      _someFooIntegerProperty;
      
  private bool IsValidSize(Size size) =>
      size.Width > 0 
      && size.Height > 0;
  ```

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
  private void WindowsSpecificMethod() { ... }
  ```

- **Use proper disposal patterns**
  ```csharp
  using var brush = new SolidBrush(Color.Red);  // Simple using for single resources
  
  // Traditional using for complex scenarios
  using (Graphics g = CreateGraphics())
  {
      // Drawing operations
  }
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