// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods for rendering a button with Standard FlatStyle in dark mode.
/// </summary>
internal class StandardButtonDarkModeRenderer : IButtonDarkModeRenderer
{
    // UI constants
    private const int CornerRadius = 5;
    private const int FocusIndicatorCornerRadius = 4;
    private const int FocusIndicatorInset = 3;
    private const int PressedInnerBorderAlpha = 200;
    private const int PressedInnerBorderR = 180;
    private const int PressedInnerBorderG = 180;
    private const int PressedInnerBorderB = 180;
    private const int DefaultBorderRedOffset = -20;
    private const int DefaultBorderGreenOffset = -10;
    private const int DefaultBorderBlueOffset = -30;
    private const int DefaultBorderThickness = 3;
    private const int SingleBorderThickness = 2;

    /// <summary>
    ///  Draws button background with standard styling (slightly rounded corners).
    /// </summary>
    public Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Standard style uses slightly rounded corners
        using GraphicsPath path = CreateRoundedRectanglePath(bounds, CornerRadius);

        // Get appropriate background color based on state
        Color backColor = GetBackgroundColor(state, isDefault);

        // Fill the background using cached brush
        using var brush = backColor.GetCachedSolidBrushScope();
        graphics.FillPath(brush, path);

        // Draw border if needed
        DrawButtonBorder(graphics, path, state, isDefault);

        // Return content bounds (area inside the button for text/image)
        return bounds;
    }

    /// <summary>
    ///  Draws a focus rectangle with dotted lines inside the button.
    /// </summary>
    public void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault)
    {
        // Create a slightly smaller rectangle for the focus indicator
        Rectangle focusRect = Rectangle.Inflate(contentBounds, -FocusIndicatorInset, -FocusIndicatorInset);

        // Create dotted pen with appropriate color
        Color focusColor = isDefault
            ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultFocusIndicatorColor
            : ButtonDarkModeRenderer.DarkModeButtonColors.FocusIndicatorColor;

        // Custom pen needed for DashStyle - can't use cached version
        using var focusPen = new Pen(focusColor)
        {
            DashStyle = DashStyle.Dot
        };

        // Draw the focus rectangle with rounded corners
        using GraphicsPath focusPath = CreateRoundedRectanglePath(focusRect, FocusIndicatorCornerRadius);
        graphics.DrawPath(focusPen, focusPath);
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
    private static void DrawButtonBorder(Graphics graphics, GraphicsPath path, PushButtonState state, bool isDefault)
    {
        // Skip border drawing for disabled state
        if (state == PushButtonState.Disabled)
        {
            return;
        }

        // For pressed state, draw a darker inner border
        if (state == PushButtonState.Pressed)
        {
            Color borderColor = isDefault
                ? Color.FromArgb(PressedInnerBorderAlpha, PressedInnerBorderR, PressedInnerBorderG, PressedInnerBorderB)
                : ButtonDarkModeRenderer.DarkModeButtonColors.BottomRightBorderColor;

            // Use the helper with inset alignment
            ButtonDarkModeRenderer.DrawButtonBorder(graphics, path, borderColor, SingleBorderThickness);

            return;
        }

        // For normal/hot states, draw a single-pixel border
        Color normalBorderColor = isDefault
            ? Color.FromArgb(
                red: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.R + DefaultBorderRedOffset,
                green: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.G + DefaultBorderGreenOffset,
                blue: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.B + DefaultBorderBlueOffset)
            : ButtonDarkModeRenderer.DarkModeButtonColors.SingleBorderColor;

        int thickness = isDefault ? DefaultBorderThickness : SingleBorderThickness;
        ButtonDarkModeRenderer.DrawButtonBorder(graphics, path, normalBorderColor, thickness);
    }

    /// <summary>
    ///  Creates a GraphicsPath for a rounded rectangle.
    /// </summary>
    private static GraphicsPath CreateRoundedRectanglePath(Rectangle bounds, int radius)
    {
        GraphicsPath path = new();
        path.AddRoundedRectangle(bounds, new Size(radius, radius));

        return path;
    }
}
