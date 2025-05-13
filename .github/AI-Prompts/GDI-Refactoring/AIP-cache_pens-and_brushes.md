# C# Refactor: Pen & Brush to Cached Scope API

You are an expert C# code transformer.

**Goal:** Rewrite every creation and usage of `Pen` and `SolidBrush` in the input file to use our `RefCountedCache` (or `PenCache`/`SolidBrushCache`) scoped API instead of direct `new` calls or helper methods returning non-cached objects.

---

## Transformation Rules

1. **Direct Instantiations**  
   - **Pens**  
     - Replace `new Pen(color)` with:
       ```csharp
       using var pen = color.GetCachedPenScope();
       ```
     - Replace `new Pen(color, width)` with:
       ```csharp
       using var pen = color.GetCachedPenScope(width);
       ```
     - Preserve original variable names, e.g.:
       ```csharp
       using var borderPen = color.GetCachedPenScope();
       ```

   - **SolidBrushes**  
     - Replace `new SolidBrush(color)` with:
       ```csharp
       using var brush = color.GetCachedSolidBrushScope();
       ```
     - Preserve original variable names.

2. **Helper Methods**  
   - **Detection:**  
     - Identify any methods in the class that return `Pen` or `SolidBrush`.
     - If a helper returns a raw `Pen` or `SolidBrush`, transform it to return the corresponding cache‐scope type (`PenCache.Scope` or `SolidBrushCache.Scope`).

   - **Private Helpers:**  
     - Change the return type to the scope type.
     - Update the method body to call the cache API directly (e.g., `color.GetCachedPenScope()`).

   - **Public/Internal Helpers:**  
     - If the helper is externally visible, create a new class‐scoped helper method (e.g., `GetDarkModePenScope`) returning `PenCache.Scope` or `SolidBrushCache.Scope`, based on the original logic, and leave the original helper untouched.

3. **Usages in Drawing Calls**  
   - All `g.Draw...` or `g.Fill...` invocations that previously took a `Pen` or `Brush` should work unchanged with the new scoped objects via the implicit converter.

4. **Preserve Formatting & Logic**  
   - Do not alter any logic that is unrelated to Pen/Brush instantiation.
   - Maintain existing code style and formatting.

---

## Example

**Before:**
```csharp
using (var p = new Pen(Color.Red, 2))
{
    g.DrawEllipse(p, bounds);
}

private Pen GetCustomBorderPen(Color color)
{
    return new Pen(color);
}
```

**After:**
```csharp
using var p = Color.Red.GetCachedPenScope(2);
g.DrawEllipse(p, bounds);

private PenCache.Scope GetCustomBorderPenScope(Color color)
    => color.GetCachedPenScope();
```

---

**Task:** Rewrite the entire input file accordingly and output the modified code.
