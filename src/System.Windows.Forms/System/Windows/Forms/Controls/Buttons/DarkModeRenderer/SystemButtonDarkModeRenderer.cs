﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods for rendering a button with System FlatStyle in dark mode.
/// </summary>
internal class SystemButtonDarkModeRenderer : IButtonDarkModeRenderer
{
    // Make this per-thread, so that different threads can safely use these methods
    [ThreadStatic]
    private static GraphicsPath? s_cornerPath;

    /// <summary>
    ///  Draws button background with system styling (larger rounded corners).
    /// </summary>
    public Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Save original smoothing mode and set to anti-alias for smooth corners
        SmoothingMode originalMode = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // System style uses larger rounded corners
        const int cornerRadius = 8;

        // Add 2px margin for the System style
        Rectangle drawBounds = Rectangle.Inflate(bounds, -2, -2);

        // Create path for rounded corners
        GraphicsPath path = GetRoundedRectanglePath(drawBounds, cornerRadius);

        // Get appropriate background color based on state
        Color backColor = GetBackgroundColor(state, isDefault);

        // Fill the background
        using (SolidBrush brush = new(backColor))
        {
            graphics.FillPath(brush, path);
        }

        // Draw border
        DrawButtonBorder(graphics, path, state, isDefault);

        // Restore original smoothing mode
        graphics.SmoothingMode = originalMode;

        // Return content bounds (area inside the button for text/image)
        // System style has more padding than other styles
        return Rectangle.Inflate(drawBounds, -8, -8);
    }

    /// <summary>
    ///  Draws a focus indicator using a white thicker border.
    /// </summary>
    public void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault)
    {
        // System style uses a white border instead of dotted lines
        SmoothingMode originalMode = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create focus rectangle slightly larger than content
        Rectangle focusRect = Rectangle.Inflate(contentBounds, 4, 4);

        // Create path for the focus outline
        GraphicsPath focusPath = GetRoundedRectanglePath(focusRect, 6);

        // System style uses a solid white border instead of dotted lines
        using var focusPen = new Pen(Color.White, 2f);
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
    private static Color GetBackgroundColor(PushButtonState state, bool isDefault)
    {
        // For default button in System style, use a darker version of the background color
        if (isDefault)
        {
            return state switch
            {
                PushButtonState.Normal => Color.FromArgb(
                    ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.R - 20,
                    ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.G - 20,
                    ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.B - 20),
                PushButtonState.Hot => Color.FromArgb(
                    ButtonDarkModeRenderer.DarkModeButtonColors.DefaultHoverBackgroundColor.R - 20,
                    ButtonDarkModeRenderer.DarkModeButtonColors.DefaultHoverBackgroundColor.G - 20,
                    ButtonDarkModeRenderer.DarkModeButtonColors.DefaultHoverBackgroundColor.B - 20),
                PushButtonState.Pressed => Color.FromArgb(
                    ButtonDarkModeRenderer.DarkModeButtonColors.DefaultPressedBackgroundColor.R - 20,
                    ButtonDarkModeRenderer.DarkModeButtonColors.DefaultPressedBackgroundColor.G - 20,
                    ButtonDarkModeRenderer.DarkModeButtonColors.DefaultPressedBackgroundColor.B - 20),
                PushButtonState.Disabled => ButtonDarkModeRenderer.DarkModeButtonColors.DefaultDisabledBackgroundColor,
                _ => ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor
            };
        }

        return state switch
        {
            PushButtonState.Normal => ButtonDarkModeRenderer.DarkModeButtonColors.NormalBackgroundColor,
            PushButtonState.Hot => ButtonDarkModeRenderer.DarkModeButtonColors.HoverBackgroundColor,
            PushButtonState.Pressed => ButtonDarkModeRenderer.DarkModeButtonColors.PressedBackgroundColor,
            PushButtonState.Disabled => ButtonDarkModeRenderer.DarkModeButtonColors.DisabledBackgroundColor,
            _ => ButtonDarkModeRenderer.DarkModeButtonColors.NormalBackgroundColor
        };
    }

    /// <summary>
    /// Draws the button border based on the current state.
    /// </summary>
    private static void DrawButtonBorder(Graphics graphics, GraphicsPath path, PushButtonState state, bool isDefault)
    {
        // For System style, we use a thicker border for default button
        if (isDefault)
        {
            // Default button gets a thicker border
            using var borderPen = new Pen(Color.White, 2f);
            graphics.DrawPath(borderPen, path);
        }
        else if (state != PushButtonState.Disabled)
        {
            // Non-default buttons get a thin border
            using var borderPen = new Pen(ButtonDarkModeRenderer.DarkModeButtonColors.SingleBorderColor);
            graphics.DrawPath(borderPen, path);
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
