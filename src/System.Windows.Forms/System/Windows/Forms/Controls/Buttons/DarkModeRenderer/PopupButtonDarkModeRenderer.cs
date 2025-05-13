// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods for rendering a button with Popup FlatStyle in dark mode.
/// </summary>
internal class PopupButtonDarkModeRenderer : IButtonDarkModeRenderer
{
    // Make this per-thread, so that different threads can safely use these methods
    [ThreadStatic]
    private static GraphicsPath? s_cornerPath;

    /// <summary>
    ///  Draws button background with popup styling, including subtle 3D effect.
    /// </summary>
    public Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Save original smoothing mode and set to anti-alias for smooth corners
        SmoothingMode originalMode = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Popup style uses slightly rounded corners like Standard
        const int cornerRadius = 5;

        // Create path for rounded corners
        GraphicsPath path = GetRoundedRectanglePath(bounds, cornerRadius);

        // Get appropriate background color based on state
        Color backColor = GetBackgroundColor(state, isDefault);

        // Fill the background
        using var brush = backColor.GetCachedSolidBrushScope();
        graphics.FillPath(brush, path);

        // Draw 3D effect borders
        DrawButtonBorder(graphics, bounds, state, isDefault);

        // Restore original smoothing mode
        graphics.SmoothingMode = originalMode;

        // Return content bounds (area inside the button for text/image)
        return Rectangle.Inflate(bounds, -6, -6);
    }

    /// <summary>
    ///  Draws a focus rectangle with dotted lines inside the button.
    /// </summary>
    public void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault)
    {
        // Save original smoothing mode
        SmoothingMode originalMode = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a slightly smaller rectangle for the focus indicator (2px inside)
        Rectangle focusRect = Rectangle.Inflate(contentBounds, -2, -2);

        // Create dotted pen with appropriate color
        Color focusColor = isDefault
            ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultFocusIndicatorColor
            : ButtonDarkModeRenderer.DarkModeButtonColors.FocusIndicatorColor;

        using var focusPen = new Pen(focusColor)
        {
            DashStyle = DashStyle.Dot
        };
        focusPen.DashStyle = DashStyle.Dot;

        // Draw the focus rectangle with rounded corners
        GraphicsPath focusPath = GetRoundedRectanglePath(focusRect, 3);
        graphics.DrawPath(focusPen, focusPath);

        // Restore original smoothing mode
        graphics.SmoothingMode = originalMode;
    }

    /// <summary>
    /// Gets the text color appropriate for the button state and type.
    /// </summary>
    public Color GetTextColor(PushButtonState state, bool isDefault)
    {
        return state == PushButtonState.Disabled
            ? ButtonDarkModeRenderer.DarkModeButtonColors.DisabledTextColor
            : isDefault
                ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultTextColor
                : ButtonDarkModeRenderer.DarkModeButtonColors.NormalTextColor;
    }

    /// <summary>
    /// Gets the background color appropriate for the button state and type.
    /// </summary>
    private static Color GetBackgroundColor(PushButtonState state, bool isDefault) =>
        isDefault
            ? state switch
            {
                PushButtonState.Normal => ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor,
                PushButtonState.Hot => ButtonDarkModeRenderer.DarkModeButtonColors.DefaultHoverBackgroundColor,
                PushButtonState.Pressed => ButtonDarkModeRenderer.DarkModeButtonColors.DefaultPressedBackgroundColor,
                PushButtonState.Disabled => ButtonDarkModeRenderer.DarkModeButtonColors.DefaultDisabledBackgroundColor,
                _ => ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor
            }
            : state switch
            {
                PushButtonState.Normal => ButtonDarkModeRenderer.DarkModeButtonColors.NormalBackgroundColor,
                PushButtonState.Hot => ButtonDarkModeRenderer.DarkModeButtonColors.HoverBackgroundColor,
                PushButtonState.Pressed => ButtonDarkModeRenderer.DarkModeButtonColors.PressedBackgroundColor,
                PushButtonState.Disabled => ButtonDarkModeRenderer.DarkModeButtonColors.DisabledBackgroundColor,
                _ => ButtonDarkModeRenderer.DarkModeButtonColors.NormalBackgroundColor
            };

    // Refactored DrawButtonBorder for a more visible 3D effect in dark mode.
    private static void DrawButtonBorder(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Popup style has a 3D effect border
        Rectangle borderRect = bounds;

        // Use slightly more contrasting border colors for dark mode 3D effect
        Color topLeftOuter, bottomRightOuter, topLeftInner, bottomRightInner;

        if (state == PushButtonState.Pressed)
        {
            // In pressed state, invert the 3D effect: highlight bottom/right, shadow top/left
            topLeftOuter = Color.FromArgb(60, 60, 60); // shadow
            bottomRightOuter = Color.FromArgb(110, 110, 110); // highlight
            topLeftInner = Color.FromArgb(40, 40, 40); // deeper shadow
            bottomRightInner = Color.FromArgb(130, 130, 130); // brighter highlight
        }
        else if (state == PushButtonState.Disabled)
        {
            // Disabled: subtle, low-contrast border
            topLeftOuter = Color.FromArgb(55, 55, 55);
            bottomRightOuter = Color.FromArgb(45, 45, 45);
            topLeftInner = Color.FromArgb(50, 50, 50);
            bottomRightInner = Color.FromArgb(50, 50, 50);
        }
        else
        {
            // Normal/hot: highlight top/left, shadow bottom/right
            topLeftOuter = Color.FromArgb(110, 110, 110); // highlight
            bottomRightOuter = Color.FromArgb(60, 60, 60); // shadow
            topLeftInner = Color.FromArgb(130, 130, 130); // brighter highlight
            bottomRightInner = Color.FromArgb(40, 40, 40); // deeper shadow
        }

        // Outer border
        using (var topLeftPen = topLeftOuter.GetCachedPenScope())
        using (var bottomRightPen = bottomRightOuter.GetCachedPenScope())
        {
            // Top
            graphics.DrawLine(topLeftPen, borderRect.Left, borderRect.Top, borderRect.Right - 2, borderRect.Top);
            // Left
            graphics.DrawLine(topLeftPen, borderRect.Left, borderRect.Top, borderRect.Left, borderRect.Bottom - 2);
            // Bottom
            graphics.DrawLine(bottomRightPen, borderRect.Left + 1, borderRect.Bottom - 1, borderRect.Right - 1, borderRect.Bottom - 1);
            // Right
            graphics.DrawLine(bottomRightPen, borderRect.Right - 1, borderRect.Top + 1, borderRect.Right - 1, borderRect.Bottom - 1);
        }

        // Inner border for more depth
        borderRect.Inflate(-1, -1);
        using (var topLeftPen = topLeftInner.GetCachedPenScope())
        using (var bottomRightPen = bottomRightInner.GetCachedPenScope())
        {
            // Top
            graphics.DrawLine(topLeftPen, borderRect.Left, borderRect.Top, borderRect.Right - 2, borderRect.Top);
            // Left
            graphics.DrawLine(topLeftPen, borderRect.Left, borderRect.Top, borderRect.Left, borderRect.Bottom - 2);
            // Bottom
            graphics.DrawLine(bottomRightPen, borderRect.Left + 1, borderRect.Bottom - 1, borderRect.Right - 1, borderRect.Bottom - 1);
            // Right
            graphics.DrawLine(bottomRightPen, borderRect.Right - 1, borderRect.Top + 1, borderRect.Right - 1, borderRect.Bottom - 1);
        }

        // For default buttons, add an additional inner border
        if (isDefault && state != PushButtonState.Disabled)
        {
            borderRect.Inflate(-1, -1);
            Color innerBorderColor = Color.FromArgb(
                Math.Max(0, ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.R - 30),
                Math.Max(0, ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.G - 20),
                Math.Max(0, ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.B - 40));
            using var innerBorderPen = innerBorderColor.GetCachedPenScope();
            graphics.DrawRectangle(innerBorderPen, borderRect);
        }
    }

    /// <summary>
    /// Creates a GraphicsPath for a rounded rectangle.
    /// </summary>
    private static GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
    {
        if (s_cornerPath is null)
        {
            s_cornerPath = new GraphicsPath();
        }
        else
        {
            s_cornerPath.Reset();
        }

        int diameter = radius * 2;
        Rectangle arcRect = new(bounds.Location, new Size(diameter, diameter));

        // Top left corner
        s_cornerPath.AddArc(arcRect, 180, 90);

        // Top right corner
        arcRect.X = bounds.Right - diameter;
        s_cornerPath.AddArc(arcRect, 270, 90);

        // Bottom right corner
        arcRect.Y = bounds.Bottom - diameter;
        s_cornerPath.AddArc(arcRect, 0, 90);

        // Bottom left corner
        arcRect.X = bounds.Left;
        s_cornerPath.AddArc(arcRect, 90, 90);

        s_cornerPath.CloseFigure();
        return s_cornerPath;
    }
}
