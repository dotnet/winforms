# WinForms GDI+ Best Practices: Performance, Quality, and Resource Management

This document provides comprehensive guidelines for optimizing GDI+ usage in the WinForms runtime. Following these practices ensures consistent performance, proper resource management, and high-quality rendering across the codebase.

## 1. Object Caching for Performance

### 1.1 Cached Pen and SolidBrush Objects

WinForms runtime provides a caching mechanism for `Pen` and `SolidBrush` objects to improve performance and reduce memory pressure. The underlying system uses:

```csharp
internal abstract partial class RefCountedCache<TObject, TCacheEntryData, TKey>
```

These cached objects have implicit conversions that make them behave like actual GDI+ types while significantly reducing allocation overhead.

### APIs, which already provide cached Pens or Brushes.

The APIs `SystemPens` and `SystemBrushes` already provide cached Pen/Brush objects. 
- Make sure you prioritize their usage (except you need pen objects with a different width).
- If you use this API, these Pen or Brush object MUST NOT be disposed!

For example:
```CSharp
    // Nothing to explicitly cache, since `SystemPens` already provides cached pen objects.
    if (ToolStripManager.VisualStylesEnabled)
    {
        e.Graphics.DrawLine(SystemPens.ButtonHighlight, 0, bounds.Bottom - 1, bounds.Width, bounds.Bottom - 1);
        e.Graphics.DrawLine(SystemPens.InactiveBorder, 0, bounds.Bottom - 2, bounds.Width, bounds.Bottom - 2);
    }
```


### 1.2 Using Cached Objects

Always prefer cached objects over direct instantiation:

```csharp
// INCORRECT: Direct instantiation
using (var pen = new Pen(Color.Red))
{
    g.DrawLine(pen, p1, p2);
}

// CORRECT: Using cached object
using var pen = Color.Red.GetCachedPenScope();
g.DrawLine(pen, p1, p2);
```

Available cached objects:

- **Pens:**
  ```csharp
  // Default width (1)
  using var pen = color.GetCachedPenScope();
  
  // Custom width (integer only)
  using var thickPen = color.GetCachedPenScope(2);
  ```

- **SolidBrushes:**
  ```csharp
  using var brush = color.GetCachedSolidBrushScope();
  ```

Always use `var` for brevity and always apply `using` to ensure proper disposal.

IMPORTANT: You cannot use pen- or brush-scoped, when the pen needs additional features, for example, when during its lifetime, the pen needs to be modified (e.g., `Inset`, `DashStyle`, `CustomStartCap`, etc.). In these cases, you need to use the non-cached version of the pen or brush.

### 1.3 Transforming Helper Methods

When refactoring existing code:

- Identify methods that return `Pen` or `SolidBrush`
- For private helpers, change the return type to the corresponding cache-scope type
- For public/internal helpers, create new methods returning scope types while preserving the original methods

```csharp
// BEFORE
private Pen GetHighlightPen()
{
    return new Pen(SystemColors.Highlight);
}

// AFTER
private PenCache.Scope GetHighlightPenScope()
    => SystemColors.Highlight.GetCachedPenScope();
```

## 2. GraphicsInternal Usage

### 2.1 When to Use GraphicsInternal

Always prefer `GraphicsInternal` over `Graphics` for performance improvements.

```csharp
// INCORRECT: Using Graphics directly
void Paint(PaintEventArgs e)
{
    e.Graphics.DrawRectangle(pen, rect);
}

// CORRECT: Using GraphicsInternal
void Paint(PaintEventArgs e)
{
    e.GraphicsInternal.DrawRectangle(pen, rect);
}
```

From the `PaintEventArgs` class:

```csharp
/// <summary>
///  For internal use to improve performance. DO NOT use this method if you modify the Graphics Clip or Transform.
/// </summary>
internal Graphics GraphicsInternal => _event.GetOrCreateGraphicsInternal(SaveStateIfNeeded);
```

IMPORTANT: While using `GraphicsInternal` should be used directly for performance improvements, avoid passing it around, since the callee would not be able to recognize the `GraphicsInternal` type.

### 2.2 State Management

If you must modify the clip or transform with `GraphicsInternal`, always implement proper state management:

```csharp
GraphicsState? previousState = null;
try
{
    // Save the current state before modifying
    previousState = graphicsInternal.Save();
    
    // Now safe to modify the clip or transform
    graphicsInternal.TranslateTransform(x, y);
    graphicsInternal.SetClip(rect);
    
    // Perform drawing operations
    // ...
}
finally
{
    // CRITICAL: Always restore the previous state
    if (previousState is not null)
    {
        graphicsInternal.Restore(previousState);
    }
}
```

## 3. Graphics Quality Settings

When changing graphics quality settings, always restore the previous setting:

```csharp
// Store original smoothing mode
SmoothingMode originalMode = g.SmoothingMode;

try
{
    // Set temporary mode for specific drawing
    g.SmoothingMode = SmoothingMode.AntiAlias;
    
    // Perform drawing operations
    // ...
}
finally
{
    // Restore original mode when done
    g.SmoothingMode = originalMode;
}
```

Always preserve and restore these settings:
- TextRenderingHint
- InterpolationMode
- CompositingQuality
- PixelOffsetMode
- SmoothingMode

## 4. Resource Management

### 4.1 Disposable Objects

Always properly dispose GDI+ objects that cannot be cached:

```csharp
// Objects with custom settings cannot be cached
using (var customPen = new Pen(Color.Red) { DashStyle = DashStyle.Dash })
{
    g.DrawLine(customPen, p1, p2);
}
```

### 4.2 GraphicsPath Objects

Never cache GraphicsPath objects - always create, use, and dispose them locally:

```csharp
// CORRECT: Local creation and disposal
using (var path = new GraphicsPath())
{
    path.AddEllipse(rect);
    g.FillPath(brush, path);
}

// INCORRECT: Never store as a field
// private GraphicsPath _cachedPath; // BAD PRACTICE!
```

### 4.3 Avoiding Premature Disposal

Ensure objects are valid throughout their entire usage lifecycle:

```csharp
// INCORRECT: Potentially disposing before use
using (var brush = color.GetCachedSolidBrushScope())
{
    someObject.SomeFutureOperation(brush); // brush might be disposed when used!
}

// CORRECT: Immediate usage within scope
using (var brush = color.GetCachedSolidBrushScope())
{
    g.FillRectangle(brush, rect);
}
```


## 5. Complete Examples

### Basic Rendering with Cached Resources

```csharp
private void PaintControl(PaintEventArgs e)
{
    // Use GraphicsInternal for better performance
    var g = e.GraphicsInternal;
    
    // Use cached pen and brush
    using var borderPen = SystemColors.ActiveBorder.GetCachedPenScope();
    using var fillBrush = SystemColors.Control.GetCachedSolidBrushScope();
    
    // Draw with cached resources
    g.FillRectangle(fillBrush, ClientRectangle);
    g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
}
```

### Complex Rendering with State Management

```csharp
private void DrawComplexControl(PaintEventArgs e)
{
    GraphicsState? previousState = null;
    SmoothingMode originalMode = SmoothingMode.Default;
    
    try
    {
        var g = e.GraphicsInternal;
        
        // Save state before modifications
        previousState = g.Save();
        originalMode = g.SmoothingMode;
        
        // Apply quality settings
        g.SmoothingMode = SmoothingMode.AntiAlias;
        
        // Apply transform
        g.TranslateTransform(10, 10);
        
        // Use cached resources
        using var pen = Color.Blue.GetCachedPenScope(2);
        using var brush = Color.LightBlue.GetCachedSolidBrushScope();
        
        // Create a path (never cache paths)
        using (var path = new GraphicsPath())
        {
            path.AddEllipse(0, 0, 100, 50);
            g.FillPath(brush, path);
            g.DrawPath(pen, path);
        }
    }
    finally
    {
        // Restore state
        if (previousState is not null)
        {
            e.GraphicsInternal.Restore(previousState);
        }
        
        // Restore quality settings
        e.GraphicsInternal.SmoothingMode = originalMode;
    }
}
```

### Helper Method Transformation

```csharp
// BEFORE
private Pen CreateBorderPen()
{
    return new Pen(SystemColors.ActiveBorder);
}

// AFTER
private PenCache.Scope GetBorderPenScope()
    => SystemColors.ActiveBorder.GetCachedPenScope();

// For public/internal APIs, create a new method and keep the original
internal Pen CreateHighlightPen()
{
    return new Pen(SystemColors.Highlight);
}

// Add a new method returning the cached version
internal PenCache.Scope GetHighlightPenScope()
    => SystemColors.Highlight.GetCachedPenScope();
```
