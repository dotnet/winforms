// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods used to render a button control in dark mode.
/// </summary>
internal static partial class ButtonDarkModeRenderer
{
    // Singleton instances for each renderer type
    internal static IButtonDarkModeRenderer StandardRenderer { get; } = new StandardButtonDarkModeRenderer();
    internal static IButtonDarkModeRenderer FlatRenderer { get; } = new FlatButtonDarkModeRenderer();
    internal static IButtonDarkModeRenderer PopupRenderer { get; } = new PopupButtonDarkModeRenderer();
    internal static IButtonDarkModeRenderer SystemRenderer { get; } = new SystemButtonDarkModeRenderer();

    /// <summary>
    ///  Gets or sets the amount by which the button bounds are deflated before rendering the button.
    ///  This value is applied to all sides of the button.
    /// </summary>
    public static Padding ButtonDeflation { get; set; } = new(2);

    /// <summary>
    ///  Draws the background of a control's parent in the specified area.
    ///  Simply fills with the parent's BackColor using the original bounds (no deflation).
    /// </summary>
    public static void DrawParentBackground(
        Graphics g,
        Rectangle bounds,
        Control childControl)
    {
        if (g is null || childControl?.Parent is null)
        {
            return;
        }

        // Use original bounds for parent background (no deflation)
        Color parentBackColor = childControl.Parent.BackColor;

        using var brush = parentBackColor.GetCachedSolidBrushScope();
        g.FillRectangle(brush, bounds);
    }

    /// <summary>
    ///  Draws the button background with a frame indicating its default state.
    /// </summary>
    public static void DrawButtonBackground(Graphics graphics, Rectangle bounds, Color frameColor, bool isDefault)
    {
        int frameThickness = isDefault ? 2 : 1;
        using var framePen = frameColor.GetCachedPenScope(frameThickness);
        graphics.DrawRectangle(framePen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
    }

    /// <summary>
    ///  Draws a button border using a specified path and border properties,
    ///  with the pen aligned inward to maintain consistent dimensions.
    /// </summary>
    public static void DrawButtonBorder(Graphics graphics, GraphicsPath path, Color borderColor, int borderWidth)
    {
        using var borderPen = borderColor.GetCachedPenScope(borderWidth);
        graphics.DrawPath(borderPen, path);
    }
}
