// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

internal static class ParentBackgroundRenderer
{
    internal static void Paint(
        Control control,
        Graphics graphics,
        Rectangle bounds,
        Color fallbackColor)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(graphics);

        using GraphicsStateScope state = new(graphics);
        using Region paintRegion = new(bounds);

        // Keep an existing clip (for example, the native TextBox client area) in effect while the
        // parent background is painted beneath the complete antialiased control body.
        using Region currentClip = graphics.Clip;
        paintRegion.Intersect(currentClip);

        Control? parent = control.ParentInternal;
        if (parent is null || parent.IsDisposed)
        {
            using var fallbackBrush = fallbackColor.GetCachedSolidBrushScope();
            graphics.FillRegion(fallbackBrush, paintRegion);
            return;
        }

        using PaintEventArgs paintEventArgs = new(graphics, bounds);
        control.PaintTransparentBackground(paintEventArgs, bounds, paintRegion);
    }
}
