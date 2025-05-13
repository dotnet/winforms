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
        using (SolidBrush brush = new(backColor))
        {
            graphics.FillPath(brush, path);
        }

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
        if (state == PushButtonState.Disabled)
        {
            return ButtonDarkModeRenderer.DarkModeButtonColors.DisabledTextColor;
        }

        return isDefault
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

    /// <summary>
    /// Draws the button border based on the current state to create 3D effect.
    /// </summary>
    private static void DrawButtonBorder(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Popup style has a 3D effect border
        Rectangle borderRect = bounds;

        if (state == PushButtonState.Pressed)
        {
            // For pressed state, invert the 3D effect
            using var topLeftPen = new Pen(ButtonDarkModeRenderer.DarkModeButtonColors.BottomRightBorderColor);
            using var bottomRightPen = new Pen(ButtonDarkModeRenderer.DarkModeButtonColors.TopLeftBorderColor);

            // Draw top and left borders (darker in pressed state)
            graphics.DrawLine(topLeftPen, borderRect.Left, borderRect.Top, borderRect.Right - 1, borderRect.Top);
            graphics.DrawLine(topLeftPen, borderRect.Left, borderRect.Top, borderRect.Left, borderRect.Bottom - 1);

            // Draw bottom and right borders (lighter in pressed state)
            graphics.DrawLine(bottomRightPen, borderRect.Left, borderRect.Bottom - 1, borderRect.Right - 1, borderRect.Bottom - 1);
            graphics.DrawLine(bottomRightPen, borderRect.Right - 1, borderRect.Top, borderRect.Right - 1, borderRect.Bottom - 1);
        }
        else if (state != PushButtonState.Disabled)
        {
            // For normal and hot states
            using var topLeftPen = new Pen(ButtonDarkModeRenderer.DarkModeButtonColors.TopLeftBorderColor);
            using var bottomRightPen = new Pen(ButtonDarkModeRenderer.DarkModeButtonColors.BottomRightBorderColor);

            // Draw top and left borders (lighter)
            graphics.DrawLine(topLeftPen, borderRect.Left, borderRect.Top, borderRect.Right - 1, borderRect.Top);
            graphics.DrawLine(topLeftPen, borderRect.Left, borderRect.Top, borderRect.Left, borderRect.Bottom - 1);

            // Draw bottom and right borders (darker)
            graphics.DrawLine(bottomRightPen, borderRect.Left, borderRect.Bottom - 1, borderRect.Right - 1, borderRect.Bottom - 1);
            graphics.DrawLine(bottomRightPen, borderRect.Right - 1, borderRect.Top, borderRect.Right - 1, borderRect.Bottom - 1);

            // For default buttons, add an additional inner border
            if (isDefault)
            {
                borderRect.Inflate(-1, -1);

                using var innerBorderPen = new Pen(Color.FromArgb(
                    red: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.R - 20,
                    green: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.G - 10,
                    blue: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.B - 30));

                graphics.DrawRectangle(innerBorderPen, borderRect);
            }
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
