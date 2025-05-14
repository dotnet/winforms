// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods for rendering a button with Flat FlatStyle in dark mode.
/// </summary>
internal class FlatButtonDarkModeRenderer : IButtonDarkModeRenderer
{
    /// <summary>
    ///  Draws button background with flat styling (no rounded corners).
    /// </summary>
    public Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Use padding from ButtonDarkModeRenderer
        Padding padding = ButtonDarkModeRenderer.GetPaddingCore(FlatStyle.Flat);
        Rectangle paddedBounds = Rectangle.Inflate(bounds, -padding.Left, -padding.Top);

        Color backColor = GetBackgroundColor(state, isDefault);
        using var brush = backColor.GetCachedSolidBrushScope();
        graphics.FillRectangle(brush, paddedBounds);

        DrawButtonBorder(graphics, paddedBounds, state, isDefault);

        return Rectangle.Inflate(paddedBounds, -padding.Left, -padding.Top);
    }

    /// <summary>
    ///  Draws a focus rectangle with dotted lines inside the button.
    /// </summary>
    public void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault)
    {
        // Create a slightly smaller rectangle for the focus indicator
        Rectangle focusRect = Rectangle.Inflate(contentBounds, -2, -2);

        // Create dotted pen with appropriate color
        Color focusColor = isDefault
            ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultFocusIndicatorColor
            : ButtonDarkModeRenderer.DarkModeButtonColors.FocusIndicatorColor;

        using var focusPen = new Pen(focusColor)
        {
            DashStyle = DashStyle.Dot
        };

        // Draw the focus rectangle (no rounded corners)
        graphics.DrawRectangle(focusPen, focusRect);
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

    /// <summary>
    /// Draws the button border based on the current state.
    /// </summary>
    private static void DrawButtonBorder(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // For flat style, we need to create a GraphicsPath for the rectangle
        using GraphicsPath path = new();
        path.AddRectangle(bounds);

        // For pressed state, draw a darker border
        if (state == PushButtonState.Pressed)
        {
            Color borderColor = isDefault
                ? Color.FromArgb(80, 0, 0, 0)
                : ButtonDarkModeRenderer.DarkModeButtonColors.BottomRightBorderColor;

            ButtonDarkModeRenderer.DrawButtonBorder(graphics, path, borderColor, 1);
        }

        // For other states, draw a single-pixel border
        else if (state != PushButtonState.Disabled)
        {
            Color borderColor = isDefault
                ? Color.FromArgb(
                    red: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.R - 20,
                    green: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.G - 10,
                    blue: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.B - 30)
                : ButtonDarkModeRenderer.DarkModeButtonColors.SingleBorderColor;

            int thickness = isDefault ? 2 : 1;
            ButtonDarkModeRenderer.DrawButtonBorder(graphics, path, borderColor, thickness);
        }
    }
}
