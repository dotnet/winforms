---
name: gdi-rendering-tests
description: >-
  Instructions for writing unit tests for Graphics, Bitmap, GraphicsPath, and
  Font rendering APIs in System.Drawing. Covers bitmap-based verification,
  integer/float overload testing, version-guarded test blocks, and
  FluentAssertions patterns for path-point validation.
metadata:
  author: dotnet-winforms
  version: "1.0"
---

# Writing Tests for GDI+ Rendering APIs

These rules apply when **writing tests for existing or new drawing APIs** on
`Graphics`, `GraphicsPath`, `Bitmap`, `Font`, and related `System.Drawing`
types. For WinForms control API tests, see the `control-api-tests` skill. For
the drawing API implementation patterns, see the `using-and-extending-gdi-plus`
skill.

> **Golden rule:** Every rendering API needs tests for both integer and float
> overloads, and must verify that drawing actually produces pixels — not just
> that no exception is thrown.

---

## 1. Test Project Structure

### 1.1 Test locations

| API area | Test file location |
|---|---|
| `Graphics` methods | `src\System.Drawing.Common\tests\GraphicsTests.cs` |
| `GraphicsPath` methods | `src\System.Drawing.Common\tests\Drawing2D\GraphicsPathTests.cs` |
| `Bitmap` methods | `src\System.Drawing.Common\tests\BitmapTests.cs` |
| `Font` / text rendering | `src\System.Drawing.Common\tests\FontTests.cs` |

Add new tests to the **existing test file** for the class under test. If the
file is very large, use a partial file named `{Class}Tests.{Feature}.cs`.

### 1.2 Test framework

The project uses **xUnit** with **FluentAssertions**. Graphics tests do not
require STA threads, so use standard `[Fact]` and `[Theory]` attributes (not
`[WinFormsFact]`/`[WinFormsTheory]`).

---

## 2. .NET Version Guards in Tests

Tests for version-guarded APIs **must** use the same preprocessor directive as
the API itself. Currently, new APIs target at least **.NET 11**:

```csharp
#if NET11_0_OR_GREATER
    [Fact]
    public void Graphics_DrawRoundedRectangle_Integer()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.DrawRoundedRectangle(Pens.Red, new(0, 0, 10, 10), new(2, 2));
        VerifyBitmapNotEmpty(bitmap);
    }
#endif
```

> **Match the guard exactly.** If the API is `#if NET11_0_OR_GREATER`, the tests
> must also be `#if NET11_0_OR_GREATER`. This ensures tests compile and run only
> when the API is available.

---

## 3. Bitmap-Based Rendering Verification

### 3.1 The verification pattern

The standard approach for testing drawing APIs is:

1. Create a small `Bitmap` (e.g., 10×10).
2. Obtain a `Graphics` from the bitmap.
3. Call the drawing API.
4. Verify the bitmap is **not empty** (contains non-zero pixels).

```csharp
[Fact]
public void Graphics_DrawMyShape_Integer()
{
    using Bitmap bitmap = new(10, 10);
    using Graphics graphics = Graphics.FromImage(bitmap);
    graphics.DrawMyShape(Pens.Red, new(0, 0, 10, 10));
    VerifyBitmapNotEmpty(bitmap);
}
```

### 3.2 VerifyBitmapNotEmpty / VerifyBitmapEmpty helpers

Use the existing unsafe helper methods in `GraphicsTests.cs` that lock bitmap
bits and scan for non-zero pixels:

```csharp
internal unsafe static void VerifyBitmapNotEmpty(Bitmap bitmap)
{
    BitmapData data = bitmap.LockBits(
        default, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    try
    {
        ReadOnlySpan<byte> bytes = new(
            (byte*)data.Scan0, data.Stride * data.Height);
        bytes.IndexOfAnyExcept((byte)0).Should().NotBe(-1);
    }
    finally
    {
        bitmap.UnlockBits(data);
    }
}

internal unsafe static void VerifyBitmapEmpty(Bitmap bitmap)
{
    BitmapData data = bitmap.LockBits(
        default, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    try
    {
        ReadOnlySpan<byte> bytes = new(
            (byte*)data.Scan0, data.Stride * data.Height);
        bytes.IndexOfAnyExcept((byte)0).Should().Be(-1);
    }
    finally
    {
        bitmap.UnlockBits(data);
    }
}
```

**When to use each:**

* `VerifyBitmapNotEmpty` — after a draw/fill call, to confirm something was
  actually rendered.
* `VerifyBitmapEmpty` — to confirm an initial bitmap has no content, or to
  verify a clear operation.

### 3.3 Use `using` declarations for all disposables

Always dispose `Bitmap`, `Graphics`, `Pen`, `Brush`, `GraphicsPath`, and `Font`
objects with `using` declarations:

```csharp
using Bitmap bitmap = new(10, 10);
using Graphics graphics = Graphics.FromImage(bitmap);
using Pen pen = new(Color.Blue, 2);
```

---

## 4. Testing Integer and Float Overloads

Every drawing primitive provides both integer (`Rectangle`, `Point`, `Size`) and
float (`RectangleF`, `PointF`, `SizeF`) overloads. **Write separate tests for
each:**

```csharp
#if NET11_0_OR_GREATER
    [Fact]
    public void Graphics_DrawRoundedRectangle_Integer()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.DrawRoundedRectangle(Pens.Red, new(0, 0, 10, 10), new(2, 2));
        VerifyBitmapNotEmpty(bitmap);
    }

    [Fact]
    public void Graphics_DrawRoundedRectangle_Float()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.DrawRoundedRectangle(
            Pens.Red, new RectangleF(0, 0, 10, 10), new(2, 2));
        VerifyBitmapNotEmpty(bitmap);
    }
#endif
```

### 4.1 Test naming convention

Follow the pattern:

```
{ClassName}_{MethodName}_{Integer|Float}
```

For additional scenario tests, extend with a scenario suffix:

```
Graphics_DrawRoundedRectangle_Integer
Graphics_DrawRoundedRectangle_Float
Graphics_FillRoundedRectangle_Integer
Graphics_FillRoundedRectangle_Float
GraphicsPath_AddRoundedRectangle_Integer
GraphicsPath_AddRoundedRectangle_Float
```

---

## 5. Testing GraphicsPath Operations

### 5.1 Path point verification

For `GraphicsPath.Add[Shape]` methods, verify the resulting path points using
FluentAssertions' approximate comparison:

```csharp
#if NET11_0_OR_GREATER
    [Fact]
    public void GraphicsPath_AddRoundedRectangle_Integer()
    {
        using GraphicsPath path = new();
        path.AddRoundedRectangle(
            new Rectangle(10, 10, 20, 20), new(5, 5));
        path.PathPoints.Should().BeApproximatelyEquivalentTo(
            new PointF[]
            {
                new(27.499994f, 10),
                new(28.880707f, 9.999997f),
                // … remaining expected points …
            },
            precision: 0.000001f);
    }
#endif
```

### 5.2 How to obtain expected path points

1. Write the test with the API call but without assertions.
2. Run the test under a debugger or add a temporary output.
3. Capture the actual `PathPoints` array.
4. Use those values as the expected data with an appropriate floating-point
   tolerance (typically `0.000001f`).

### 5.3 Test both overloads separately

Even though the integer overload delegates to the float overload, test both
independently — the cast from `Rectangle` → `RectangleF` must be verified:

```csharp
[Fact]
public void GraphicsPath_AddRoundedRectangle_Integer() { /* … */ }

[Fact]
public void GraphicsPath_AddRoundedRectangle_Float() { /* … */ }
```

---

## 6. Testing Draw and Fill Pairs

When a new shape has both `Draw` and `Fill` methods, write tests for all four
combinations (Draw×Integer, Draw×Float, Fill×Integer, Fill×Float):

```csharp
#if NET11_0_OR_GREATER
    [Fact]
    public void Graphics_DrawRoundedRectangle_Integer()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.DrawRoundedRectangle(Pens.Red, new(0, 0, 10, 10), new(2, 2));
        VerifyBitmapNotEmpty(bitmap);
    }

    [Fact]
    public void Graphics_FillRoundedRectangle_Integer()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.FillRoundedRectangle(
            Brushes.Green, new(0, 0, 10, 10), new(2, 2));
        VerifyBitmapNotEmpty(bitmap);
    }

    // … and Float variants for each …
#endif
```

---

## 7. Testing Pen and Brush Variations

For more thorough coverage, consider additional tests with:

* Different pen widths.
* Different brush types (`SolidBrush`, `LinearGradientBrush`).
* Anti-aliased vs. default smoothing modes.
* Edge cases: zero-size rectangles, very large corner radii, negative
  coordinates.

```csharp
[Fact]
public void Graphics_DrawRoundedRectangle_ThickPen()
{
    using Bitmap bitmap = new(20, 20);
    using Graphics graphics = Graphics.FromImage(bitmap);
    using Pen pen = new(Color.Blue, 3);
    graphics.DrawRoundedRectangle(pen, new RectangleF(2, 2, 16, 16), new(4, 4));
    VerifyBitmapNotEmpty(bitmap);
}
```

---

## 8. Testing Font and Text Rendering

For font and text rendering APIs, use the same bitmap-based approach:

```csharp
[Fact]
public void Graphics_DrawString_RendersText()
{
    using Bitmap bitmap = new(100, 30);
    using Graphics graphics = Graphics.FromImage(bitmap);
    using Font font = new("Arial", 12);
    graphics.DrawString("Test", font, Brushes.Black, 0, 0);
    VerifyBitmapNotEmpty(bitmap);
}
```

For font metric APIs, verify returned values are within expected ranges rather
than exact pixel matching, since font rendering can vary across environments.

---

## 9. Checklist for GDI+ Rendering Tests

* [ ] Tests guarded with matching `#if NET11_0_OR_GREATER` (or appropriate
      version)
* [ ] Separate tests for integer and float overloads
* [ ] Tests for both Draw and Fill variants (if applicable)
* [ ] GraphicsPath tests verify path points with approximate comparison
* [ ] Bitmap-based verification using `VerifyBitmapNotEmpty`
* [ ] All disposables use `using` declarations
* [ ] Test naming follows `{Class}_{Method}_{Overload}` convention
* [ ] Tests added to existing test files (not new files, unless partials)
