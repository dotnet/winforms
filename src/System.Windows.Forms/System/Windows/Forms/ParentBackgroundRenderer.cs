// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms;

internal static class ParentBackgroundRenderer
{
    internal static void Paint(
        Control control,
        Graphics graphics,
        Rectangle bounds,
        GraphicsPath opaquePath,
        Color fallbackColor)
    {
        ArgumentNullException.ThrowIfNull(control);
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(opaquePath);

        using GraphicsStateScope state = new(graphics);
        using Region exposedRegion = new(bounds);
        exposedRegion.Exclude(opaquePath);

        // Keep an existing clip (for example, the native TextBox client area) in effect while
        // PaintTransparentBackground establishes the parent-coordinate clip.
        using Region currentClip = graphics.Clip;
        exposedRegion.Intersect(currentClip);

        Control? parent = control.ParentInternal;
        if (parent is null || parent.IsDisposed)
        {
            using SolidBrush fallbackBrush = new(fallbackColor);
            graphics.FillRegion(fallbackBrush, exposedRegion);
            return;
        }

        using PaintEventArgs paintEventArgs = new(graphics, bounds);
        control.PaintTransparentBackground(paintEventArgs, bounds, exposedRegion);
    }
}
