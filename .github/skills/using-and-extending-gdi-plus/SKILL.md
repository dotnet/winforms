---
name: using-and-extending-gdi-plus
description: >-
  Instructions for using GDI, GDI+ for drawing primitives, shapes and
  processing/rendering bitmaps and rendering Fonts. Also, tenets for adding new
  APIs to System.Drawing for modernization and improving drawing, imaging, and
  font features.
metadata:
  author: dotnet-winforms
  version: "2.0"
---

# Using and Extending GDI+ in WinForms

These rules apply when **using GDI+ for drawing** in the WinForms runtime and
when **adding new drawing APIs** to `System.Drawing`. For control-level API
additions (properties, events, etc.), see the `new-control-api` skill. For
writing tests for rendering APIs, see the `gdi-rendering-tests` skill.

> **Golden rule:** Cache what you can, dispose what you must, and always restore
> graphics state after modifying it.

---

## 1. Object Caching for Performance

### 1.1 Cached Pen and SolidBrush objects

WinForms provides a ref-counted caching mechanism for `Pen` and `SolidBrush`
objects (`RefCountedCache<TObject, TCacheEntryData, TKey>`). Cached objects have
implicit conversions that let them behave like their GDI+ counterparts while
reducing allocation overhead.

### 1.2 Prefer SystemPens and SystemBrushes

`SystemPens` and `SystemBrushes` already provide cached objects. Always
prioritize their usage (unless you need a non-default pen width). Objects
obtained from these APIs **must not** be disposed:

```csharp
// Good — already cached via SystemPens.
e.Graphics.DrawLine(SystemPens.ButtonHighlight, 0, bounds.Bottom - 1, bounds.Width, bounds.Bottom - 1);
```

### 1.3 Using scoped cached objects

When `SystemPens`/`SystemBrushes` do not have the color you need, prefer cached
scopes over direct instantiation:

```csharp
// INCORRECT — direct instantiation.
using (var pen = new Pen(Color.Red))
{
    g.DrawLine(pen, p1, p2);
}

// CORRECT — cached scope.
using var pen = Color.Red.GetCachedPenScope();
g.DrawLine(pen, p1, p2);
```

Available cached scopes:

```csharp
// Default-width pen (width = 1)
using var pen = color.GetCachedPenScope();

// Custom-width pen (integer only)
using var thickPen = color.GetCachedPenScope(2);

// SolidBrush
using var brush = color.GetCachedSolidBrushScope();
```

Always use `var` for the scope type and always apply `using`.

**When caching is not possible:** If the pen needs additional configuration
during its lifetime (e.g., `DashStyle`, `CustomStartCap`, `Inset`), you must
use a non-cached pen/brush and dispose it explicitly.

### 1.4 Refactoring helpers to return scoped objects

When refactoring methods that return `Pen` or `SolidBrush`:

* For **private** helpers — change the return type to the scope type directly.
* For **public/internal** helpers — add a new scope-returning method and
  preserve the original.

```csharp
// Before
private Pen GetHighlightPen() => new Pen(SystemColors.Highlight);

// After
private PenCache.Scope GetHighlightPenScope()
    => SystemColors.Highlight.GetCachedPenScope();
```

---

## 2. GraphicsInternal Usage

### 2.1 When to use GraphicsInternal

In WinForms control painting, prefer `e.GraphicsInternal` over `e.Graphics` for
performance. `GraphicsInternal` avoids unnecessary state saves:

```csharp
void Paint(PaintEventArgs e)
{
    e.GraphicsInternal.DrawRectangle(pen, rect);
}
```

**Caveat:** Do not pass `GraphicsInternal` to other methods — callees cannot
distinguish it from a regular `Graphics` instance.

### 2.2 State management with GraphicsInternal

If you must modify the clip or transform, save and restore state:

```csharp
GraphicsState? previousState = null;
try
{
    previousState = graphicsInternal.Save();
    graphicsInternal.TranslateTransform(x, y);
    graphicsInternal.SetClip(rect);
    // … draw …
}
finally
{
    if (previousState is not null)
    {
        graphicsInternal.Restore(previousState);
    }
}
```

---

## 3. Graphics Quality Settings

When changing quality settings, **always** restore the original value:

```csharp
SmoothingMode originalMode = g.SmoothingMode;
try
{
    g.SmoothingMode = SmoothingMode.AntiAlias;
    // … draw …
}
finally
{
    g.SmoothingMode = originalMode;
}
```

Settings that must be preserved and restored:

* `SmoothingMode`
* `TextRenderingHint`
* `InterpolationMode`
* `CompositingQuality`
* `PixelOffsetMode`

---

## 4. Resource Management

### 4.1 Disposable objects

Always dispose GDI+ objects that cannot be cached:

```csharp
using var customPen = new Pen(Color.Red) { DashStyle = DashStyle.Dash };
g.DrawLine(customPen, p1, p2);
```

### 4.2 GraphicsPath objects

Never cache `GraphicsPath` objects — always create, use, and dispose locally:

```csharp
using GraphicsPath path = new();
path.AddEllipse(rect);
g.FillPath(brush, path);
```

### 4.3 Avoid premature disposal

Ensure objects remain valid throughout their usage. Do not pass a scoped object
to something that may use it after the scope ends:

```csharp
// INCORRECT — brush may be used after scope ends.
using (var brush = color.GetCachedSolidBrushScope())
{
    someObject.SomeFutureOperation(brush);
}

// CORRECT — immediate use within scope.
using var brush = color.GetCachedSolidBrushScope();
g.FillRectangle(brush, rect);
```

---

## 5. Adding New Drawing Primitives to Graphics

When adding new drawing APIs (e.g., `DrawXxx` / `FillXxx` methods) to the
`Graphics` class or new path operations to `GraphicsPath`, follow the
established pattern from the `RoundedRectangle` API addition.

### 5.1 .NET version guard — mandatory

All new public APIs in `System.Drawing` **must** be guarded with a preprocessor
directive for the target .NET version. Currently, new APIs target at least
**.NET 11**:

```csharp
#if NET11_0_OR_GREATER
    /// <summary>
    ///  Draws the outline of the specified rounded rectangle.
    /// </summary>
    public void DrawRoundedRectangle(Pen pen, RectangleF rect, SizeF corner)
    {
        using GraphicsPath path = new();
        path.AddRoundedRectangle(rect, corner);
        DrawPath(pen, path);
    }
#endif
```

> **Why?** The `System.Drawing.Common` package ships as part of the shared
> framework. Version guards ensure new APIs are only available on the .NET
> version they were approved for, preventing accidental use on older runtimes,
> specifically, when we need to service parts of main at a later point in
> time back into an earlier version, or if we're including for a new version,
> whose branch has not snapped, yet.

### 5.2 Provide both integer and float overloads

Every new drawing primitive must have two public overloads:

1. An **integer overload** (`Rectangle`, `Point`, `Size`) that delegates to the
   float overload.
2. A **float overload** (`RectangleF`, `PointF`, `SizeF`) with the actual
   implementation.

The integer overload uses `<inheritdoc cref="..."/>` to inherit documentation
from the float overload:

```csharp
#if NET11_0_OR_GREATER
    /// <inheritdoc cref="DrawRoundedRectangle(Pen, RectangleF, SizeF)"/>
    public void DrawRoundedRectangle(Pen pen, Rectangle rect, Size corner) =>
        DrawRoundedRectangle(pen, (RectangleF)rect, corner);

    /// <summary>
    ///  Draws the outline of the specified rounded rectangle.
    /// </summary>
    /// <param name="pen">The <see cref="Pen"/> to draw the outline with.</param>
    /// <param name="rect">The bounds of the rounded rectangle.</param>
    /// <param name="corner">
    ///  The size of the ellipse used to round the corners of the rectangle.
    /// </param>
    public void DrawRoundedRectangle(Pen pen, RectangleF rect, SizeF corner)
    {
        using GraphicsPath path = new();
        path.AddRoundedRectangle(rect, corner);
        DrawPath(pen, path);
    }
#endif
```

### 5.3 Provide Draw and Fill pairs

When adding a new shape primitive, provide both `Draw` (outline) and `Fill`
(interior) methods. The `Fill` variant takes a `Brush` instead of a `Pen`:

```csharp
#if NET11_0_OR_GREATER
    public void FillRoundedRectangle(Brush brush, RectangleF rect, SizeF corner)
    {
        using GraphicsPath path = new();
        path.AddRoundedRectangle(rect, corner);
        FillPath(brush, path);
    }
#endif
```

### 5.4 Add corresponding GraphicsPath method

If the new primitive is path-based, add the `Add[Shape]` method to
`GraphicsPath` as well, using the same version guard and integer/float overload
pattern:

```csharp
#if NET11_0_OR_GREATER
    /// <inheritdoc cref="AddRoundedRectangle(RectangleF, SizeF)"/>
    public void AddRoundedRectangle(Rectangle rect, Size corner) =>
        AddRoundedRectangle((RectangleF)rect, corner);

    /// <summary>
    ///  Adds a rounded rectangle to this path.
    /// </summary>
    /// <param name="rect">The bounds of the rectangle to add.</param>
    /// <param name="corner">
    ///  The size of the ellipse used to round the corners of the rectangle.
    /// </param>
    public void AddRoundedRectangle(RectangleF rect, SizeF corner)
    {
        StartFigure();
        // … arc calls for each corner …
        CloseFigure();
    }
#endif
```

### 5.5 XML documentation

* The **float overload** carries the full `<summary>`, `<param>`, and optional
  `<remarks>` documentation.
* The **integer overload** uses `<inheritdoc cref="FloatOverload"/>` to avoid
  duplicating docs.
* Use `<see cref="..."/>` for cross-references to `Pen`, `Brush`,
  `GraphicsPath`, etc.

### 5.6 Placement in source files

Place new methods **adjacent to existing related methods**:

* `DrawRoundedRectangle` next to `DrawRectangle`.
* `FillRoundedRectangle` next to `FillRectangle`.
* `AddRoundedRectangle` next to `AddRectangle` / `AddRectangles`.

---

## 6. Complete Examples

### Basic rendering with cached resources

```csharp
private void PaintControl(PaintEventArgs e)
{
    var g = e.GraphicsInternal;

    using var borderPen = SystemColors.ActiveBorder.GetCachedPenScope();
    using var fillBrush = SystemColors.Control.GetCachedSolidBrushScope();

    g.FillRectangle(fillBrush, ClientRectangle);
    g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
}
```

### Complex rendering with state management

```csharp
private void DrawComplexControl(PaintEventArgs e)
{
    GraphicsState? previousState = null;
    SmoothingMode originalMode = SmoothingMode.Default;

    try
    {
        var g = e.GraphicsInternal;
        previousState = g.Save();
        originalMode = g.SmoothingMode;

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TranslateTransform(10, 10);

        using var pen = Color.Blue.GetCachedPenScope(2);
        using var brush = Color.LightBlue.GetCachedSolidBrushScope();

        using GraphicsPath path = new();
        path.AddEllipse(0, 0, 100, 50);
        g.FillPath(brush, path);
        g.DrawPath(pen, path);
    }
    finally
    {
        if (previousState is not null)
        {
            e.GraphicsInternal.Restore(previousState);
        }

        e.GraphicsInternal.SmoothingMode = originalMode;
    }
}
```

---

## 7. Checklist Before Submitting

* [ ] New APIs guarded with `#if NET11_0_OR_GREATER` (or the appropriate
      target version)
* [ ] Both integer and float overloads provided
* [ ] Draw and Fill pairs provided for shape primitives
* [ ] Corresponding `GraphicsPath.Add[Shape]` method added if path-based
* [ ] XML documentation on all new public members
* [ ] Integer overload uses `<inheritdoc cref="..."/>`
* [ ] Cached pens/brushes used where possible; disposed when not cacheable
* [ ] `GraphicsPath` objects created and disposed locally (never cached)
* [ ] Graphics state (smoothing, clip, transform) saved and restored
* [ ] New methods placed adjacent to related existing methods
* [ ] Tests written for both integer and float overloads (see
      `gdi-rendering-tests` skill)
