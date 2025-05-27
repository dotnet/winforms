// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods for rendering a button with Flat FlatStyle in dark mode.
/// </summary>
internal class FlatButtonDarkModeRenderer : ButtonDarkModeRendererBase
{
    // UI constants
    private const int FocusIndicatorInflate = -2;
    private const int BorderThicknessDefault = 2;
    private const int BorderThicknessNormal = 1;
    private const int FlatEdgeRoundingAngle = 8;

    private static readonly Size s_flatEdgeRoundingAngleSize =
        new(FlatEdgeRoundingAngle, FlatEdgeRoundingAngle);

    private protected override Padding PaddingCore { get; } = new Padding(0);

    /// <summary>
    ///  Draws button background with flat styling (no rounded corners).
    /// </summary>
    public override Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
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
    public override void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault)
    {
        // Create a slightly smaller rectangle for the focus indicator
        Rectangle focusRect = Rectangle.Inflate(contentBounds, FocusIndicatorInflate, FocusIndicatorInflate);

        // Create dotted pen with appropriate color
        Color focusColor = isDefault
            ? IButtonRenderer.DarkModeButtonColors.DefaultFocusIndicatorColor
            : IButtonRenderer.DarkModeButtonColors.FocusIndicatorColor;

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
    public override Color GetTextColor(PushButtonState state, bool isDefault) =>
        state == PushButtonState.Disabled
            ? IButtonRenderer.DarkModeButtonColors.DisabledTextColor
            : isDefault
                ? IButtonRenderer.DarkModeButtonColors.DefaultTextColor
                : IButtonRenderer.DarkModeButtonColors.NormalTextColor;

    /// <summary>
    ///  Gets the background color appropriate for the button state and type.
    /// </summary>
    private static Color GetBackgroundColor(PushButtonState state, bool isDefault) =>
        isDefault
            ? state switch
            {
                PushButtonState.Normal => IButtonRenderer.DarkModeButtonColors.DefaultBackgroundColor,
                PushButtonState.Hot => IButtonRenderer.DarkModeButtonColors.DefaultHoverBackgroundColor,
                PushButtonState.Pressed => IButtonRenderer.DarkModeButtonColors.DefaultPressedBackgroundColor,
                PushButtonState.Disabled => IButtonRenderer.DarkModeButtonColors.DefaultDisabledBackgroundColor,
                _ => IButtonRenderer.DarkModeButtonColors.DefaultBackgroundColor
            }
            : state switch
            {
                PushButtonState.Normal => IButtonRenderer.DarkModeButtonColors.NormalBackgroundColor,
                PushButtonState.Hot => IButtonRenderer.DarkModeButtonColors.HoverBackgroundColor,
                PushButtonState.Pressed => IButtonRenderer.DarkModeButtonColors.PressedBackgroundColor,
                PushButtonState.Disabled => IButtonRenderer.DarkModeButtonColors.DisabledBackgroundColor,
                _ => IButtonRenderer.DarkModeButtonColors.NormalBackgroundColor
            };

    /// <summary>
    ///  Draws the button border based on the current state.
    /// </summary>
    private static void DrawButtonBorder(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // For flat style, we need to create a GraphicsPath for the rectangle
        using GraphicsPath path = new();
        path.AddRoundedRectangle(bounds, s_flatEdgeRoundingAngleSize);

        // Skip border drawing for disabled state
        if (state == PushButtonState.Disabled)
        {
            IButtonRenderer.DrawButtonBorder(
                graphics,
                path,
                IButtonRenderer.DarkModeButtonColors.DisabledBorderLightColor,
                1);

            return;
        }

        Color borderColor;

        int thickness = isDefault
            ? BorderThicknessDefault
            : BorderThicknessNormal;

        // For pressed state, draw a darker border
        if (state == PushButtonState.Pressed)
        {
            borderColor = isDefault
                ? IButtonRenderer.DarkModeButtonColors.PressedSingleBorderColor
                : IButtonRenderer.DarkModeButtonColors.DefaultFocusIndicatorColor;

            IButtonRenderer.DrawButtonBorder(graphics, path, borderColor, thickness);

            return;
        }

        // For other states, draw a border with appropriate thickness
        // For pressed state, draw a darker border
        borderColor = isDefault
            ? IButtonRenderer.DarkModeButtonColors.DefaultSingleBorderColor
            : IButtonRenderer.DarkModeButtonColors.SingleBorderColor;

        IButtonRenderer.DrawButtonBorder(graphics, path, borderColor, thickness);

        return;
    }
}
