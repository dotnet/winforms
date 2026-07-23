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

    /// <summary>
    ///  Draws a two-pixel line in the parent background color immediately outside the rounded
    ///  border described by <paramref name="borderBounds"/> and <paramref name="cornerRadius"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The modern rounded chrome masks its corners with a <see cref="Region"/>, which cannot be
    ///   anti-aliased. That leaves jagged pixels just outside the anti-aliased border where the
    ///   region-clipped parent fill meets the control body. Tracing a short, anti-aliased line in
    ///   the parent's background color over that band blends the artifact into the parent.
    ///  </para>
    /// </remarks>
    internal static void PaintRoundedBorderRegionMitigation(
        Graphics graphics,
        Rectangle borderBounds,
        Size cornerRadius,
        int borderThickness,
        Color parentBackColor)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        if (borderBounds.Width <= 0 || borderBounds.Height <= 0)
        {
            return;
        }

        const int MitigationThickness = 2;

        // Offset the outline so the two-pixel stroke sits just outside the border's outer edge.
        int outset = (borderThickness / 2) + 1;

        Rectangle outerBounds = borderBounds;
        outerBounds.Inflate(outset, outset);

        Size outerRadius = new(
            cornerRadius.Width + outset,
            cornerRadius.Height + outset);

        using GraphicsStateScope state = new(graphics);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using GraphicsPath outline = new();
        outline.AddRoundedRectangle(outerBounds, outerRadius);

        using var pen = parentBackColor.GetCachedPenScope(MitigationThickness);
        graphics.DrawPath(pen, outline);
    }
}
