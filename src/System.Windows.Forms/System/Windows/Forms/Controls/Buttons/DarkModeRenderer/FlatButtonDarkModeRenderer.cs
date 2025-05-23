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
    // UI constants
    private const int FocusIndicatorInflate = -2;
    private const int BorderThicknessDefault = 2;
    private const int BorderThicknessNormal = 1;

    // Color constants
    private static Color PressedDefaultBorderColor => Color.FromArgb(200, 0, 0, 0);
    private const int DefaultBorderColorRedOffset = -20;
    private const int DefaultBorderColorGreenOffset = -10;
    private const int DefaultBorderColorBlueOffset = -30;

    /// <summary>
    ///  Draws button background with flat styling (no rounded corners).
    /// </summary>
    public Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Get appropriate background color based on state
        Color backColor = GetBackgroundColor(state, isDefault);

        // Fill the background using cached brush
        using var brush = backColor.GetCachedSolidBrushScope();
        graphics.FillRectangle(brush, bounds);

        // Draw border if needed
        DrawButtonBorder(graphics, bounds, state, isDefault);

        // Return content bounds (area inside the button for text/image)
        return bounds;
    }

    /// <summary>
    ///  Draws a focus rectangle with dotted lines inside the button.
    /// </summary>
    public void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault)
    {
        // Create a slightly smaller rectangle for the focus indicator
        Rectangle focusRect = Rectangle.Inflate(contentBounds, FocusIndicatorInflate, FocusIndicatorInflate);

        // Create dotted pen with appropriate color
        Color focusColor = isDefault
            ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultFocusIndicatorColor
            : ButtonDarkModeRenderer.DarkModeButtonColors.FocusIndicatorColor;

        // Custom pen needed for DashStyle - can't use cached version
        using var focusPen = new Pen(focusColor)
        {
            DashStyle = DashStyle.Dot
        };

        // Draw the focus rectangle (no rounded corners)
        graphics.DrawRectangle(focusPen, focusRect);
    }

    /// <summary>
    ///  Gets the text color appropriate for the button state and type.
    /// </summary>
    public Color GetTextColor(PushButtonState state, bool isDefault) =>
        state == PushButtonState.Disabled
            ? ButtonDarkModeRenderer.DarkModeButtonColors.DisabledTextColor
            : isDefault
                ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultTextColor
                : ButtonDarkModeRenderer.DarkModeButtonColors.NormalTextColor;

    /// <summary>
    ///  Gets the background color appropriate for the button state and type.
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
    ///  Draws the button border based on the current state.
    /// </summary>
    private static void DrawButtonBorder(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // For flat style, we need to create a GraphicsPath for the rectangle
        using GraphicsPath path = new();
        path.AddRectangle(bounds);

        // Skip border drawing for disabled state
        if (state == PushButtonState.Disabled)
        {
            return;
        }

        // For pressed state, draw a darker border
        if (state == PushButtonState.Pressed)
        {
            Color borderColor = isDefault
                ? PressedDefaultBorderColor
                : ButtonDarkModeRenderer.DarkModeButtonColors.BottomRightBorderColor;

            ButtonDarkModeRenderer.DrawButtonBorder(graphics, path, borderColor, BorderThicknessNormal);

            return;
        }

        // For other states, draw a border with appropriate thickness
        Color normalBorderColor = isDefault
            ? Color.FromArgb(
                red: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.R + DefaultBorderColorRedOffset,
                green: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.G + DefaultBorderColorGreenOffset,
                blue: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.B + DefaultBorderColorBlueOffset)
            : ButtonDarkModeRenderer.DarkModeButtonColors.SingleBorderColor;

        int thickness = isDefault ? BorderThicknessDefault : BorderThicknessNormal;
        ButtonDarkModeRenderer.DrawButtonBorder(graphics, path, normalBorderColor, thickness);
    }
}
