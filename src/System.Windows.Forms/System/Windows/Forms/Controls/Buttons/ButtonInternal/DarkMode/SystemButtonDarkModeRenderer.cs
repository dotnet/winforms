// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.DarkModeButtonColors;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods for rendering a button with System FlatStyle in dark mode.
/// </summary>
internal class SystemButtonDarkModeRenderer : ButtonDarkModeRendererBase
{
    // UI constants
    private const int CornerRadius = 8;
    private const int FocusIndicatorCornerRadius = 6;

    private const int DefaultButtonBorderThickness = 0;
    private const int NonDefaultButtonBorderThickness = 0;
    private const int FocusedButtonBorderThickness = 3;
    private const int DarkBorderGapThickness = 2;
    private const int SystemStylePadding = FocusedButtonBorderThickness + DarkBorderGapThickness;

    private const int DefaultBackgroundColorOffset = 20;

    private protected override Padding PaddingCore { get; } = new Padding(SystemStylePadding);

    /// <summary>
    ///  Draws button background with system styling (larger rounded corners).
    /// </summary>
    public override Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Shrink for DarkBorderGap and FocusBorderThickness
        Rectangle fillBounds = Rectangle.Inflate(bounds, -SystemStylePadding, -SystemStylePadding);

        using GraphicsPath fillPath = CreateRoundedRectanglePath(fillBounds, CornerRadius - DarkBorderGapThickness);

        // Get appropriate background color based on state
        Color backColor = GetBackgroundColor(state, isDefault);

        // Fill the background using cached brush
        using var brush = backColor.GetCachedSolidBrushScope();
        graphics.FillPath(brush, fillPath);

        // Return content bounds (area inside the button for text/image)
        return fillBounds;
    }

    /// <summary>
    ///  Draws a focus indicator using a white thicker border.
    /// </summary>
    public override void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault)
    {
        // We need the bottom and the right border one pixel inside the button
        Rectangle focusRect = new(
            x: contentBounds.X,
            y: contentBounds.Y,
            width: contentBounds.Width - 1,
            height: contentBounds.Height - 1);

        // Create path for the focus outline
        using GraphicsPath focusPath = CreateRoundedRectanglePath(focusRect, FocusIndicatorCornerRadius);

        // System style uses a solid white border instead of dotted lines
        using var focusPen = Color.White.GetCachedPenScope(FocusedButtonBorderThickness);

        graphics.DrawPath(focusPen, focusPath);
    }

    /// <summary>
    ///  Gets the text color appropriate for the button state and type.
    /// </summary>
    public override Color GetTextColor(PushButtonState state, bool isDefault) =>
        state == PushButtonState.Disabled
            ? DefaultColors.DisabledTextColor
            : isDefault
                ? DefaultColors.StandardBackColor
                : DefaultColors.AcceptButtonTextColor;

    /// <summary>
    ///  Gets the background color appropriate for the button state and type.
    /// </summary>
    private static Color GetBackgroundColor(PushButtonState state, bool isDefault) =>
        // For default button in System style, use a darker version of the background color
        isDefault
            ? state switch
            {
                PushButtonState.Normal => Color.FromArgb(
                    DefaultColors.StandardBackColor.R - DefaultBackgroundColorOffset,
                    DefaultColors.StandardBackColor.G - DefaultBackgroundColorOffset,
                    DefaultColors.StandardBackColor.B - DefaultBackgroundColorOffset),
                PushButtonState.Hot => Color.FromArgb(
                    DefaultColors.HoverBackColor.R - DefaultBackgroundColorOffset,
                    DefaultColors.HoverBackColor.G - DefaultBackgroundColorOffset,
                    DefaultColors.HoverBackColor.B - DefaultBackgroundColorOffset),
                PushButtonState.Pressed => Color.FromArgb(
                    DefaultColors.PressedBackColor.R - DefaultBackgroundColorOffset,
                    DefaultColors.PressedBackColor.G - DefaultBackgroundColorOffset,
                    DefaultColors.PressedBackColor.B - DefaultBackgroundColorOffset),
                PushButtonState.Disabled => DefaultColors.DisabledBackColor,
                _ => DefaultColors.StandardBackColor
            }
            : state switch
            {
                PushButtonState.Normal => DefaultColors.StandardBackColor,
                PushButtonState.Hot => DefaultColors.HoverBackColor,
                PushButtonState.Pressed => DefaultColors.PressedBackColor,
                PushButtonState.Disabled => DefaultColors.DisabledBackColor,
                _ => DefaultColors.StandardBackColor
            };

    /// <summary>
    ///  Draws the button border based on the current state, using anti-aliasing and an additional inner border.
    /// </summary>
    public static void DrawButtonBorder(
        Graphics graphics,
        Rectangle bounds,
        PushButtonState state,
        bool isDefault,
        bool isFocused)
    {
        // Skip border drawing for disabled state
        if (state == PushButtonState.Disabled)
        {
            return;
        }

        // Don't draw regular border if focus border is already drawn
        if (isFocused)
        {
            return;
        }

        // Outer border path
        Rectangle borderRect = Rectangle.Inflate(bounds, -SystemStylePadding, -SystemStylePadding);

        using GraphicsPath borderPath = CreateRoundedRectanglePath(borderRect, CornerRadius);

        // We need to implement a subtle 3d effect around the already
        // painted filling. We do this by drawing a border with a 1px pen,
        // which is - with brighter colors - top and right a bit darker than
        // the fill color, and bottom and left yet another bit darker.
        //
        // For darker fill colors, we use a slightly lighter color for the top and right sides,
        // and a yet bit lighter color for the bottom and left sides.
        // We never change the color for the borders, we just adjust the brightness.

        // Get base color for the border based on button state and type
        Color backColor = GetBackgroundColor(state, isDefault);

        // For normal colors, make borders darker
        // For dark colors, make borders lighter
        bool isDarkColor = backColor.GetBrightness() < 0.5;

        // Top-left border (slightly lighter/darker)
        Color topLeftColor = isDarkColor
            ? ControlPaint.Light(backColor, 0.2f)
            : ControlPaint.Dark(backColor, 0.1f);

        // Bottom-right border (more pronounced light/dark)
        Color bottomRightColor = isDarkColor
            ? ControlPaint.Light(backColor, 0.4f)
            : ControlPaint.Dark(backColor, 0.2f);

        // Determine border thickness
        int borderThickness = isDefault ? DefaultButtonBorderThickness : NonDefaultButtonBorderThickness;

        // Save graphics state to restore anti-aliasing settings later
        GraphicsState? graphicState = null;

        try
        {
            graphicState = graphics.Save();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw top-left border segment
            using var topLeftPen = topLeftColor.GetCachedPenScope(borderThickness);
            graphics.DrawPath(topLeftPen, GetTopLeftSegmentPath(borderRect, CornerRadius));

            // Draw bottom-right border segment
            using var bottomRightPen = bottomRightColor.GetCachedPenScope(borderThickness);
            graphics.DrawPath(bottomRightPen, GetBottomRightSegmentPath(borderRect, CornerRadius));
        }
        finally
        {
            // Restore graphics state
            if (graphicState is not null)
            {
                graphics.Restore(graphicState);
            }
        }
    }

    /// <summary>
    ///  Creates a path for the top and left segments of a rounded rectangle.
    /// </summary>
    private static GraphicsPath GetTopLeftSegmentPath(Rectangle bounds, int radius)
    {
        GraphicsPath path = new();

        int diameter = radius * 2;

        // Top left corner arc
        Rectangle arcRect = new(bounds.Location, new Size(diameter, diameter));
        path.AddArc(arcRect, 180, 90);

        // Top line
        path.AddLine(bounds.Left + radius, bounds.Top, bounds.Right - radius, bounds.Top);

        // Top right corner arc (just the top portion)
        arcRect.X = bounds.Right - diameter;
        path.AddArc(arcRect, 270, 45);

        // Path back to middle of right side
        path.AddLine(
            bounds.Right - (int)(radius * Math.Sin(Math.PI / 4)),
            bounds.Top + (int)(radius * (1 - Math.Cos(Math.PI / 4))),
            bounds.Right,
            bounds.Top + bounds.Height / 2);

        // Path to middle of bottom
        path.AddLine(bounds.Right, bounds.Top + bounds.Height / 2, bounds.Left + bounds.Width / 2, bounds.Bottom);

        // Path to bottom left corner
        path.AddLine(bounds.Left + bounds.Width / 2, bounds.Bottom, bounds.Left, bounds.Bottom - bounds.Height / 2);

        // Path back to start
        path.AddLine(bounds.Left, bounds.Bottom - bounds.Height / 2, bounds.Left, bounds.Top + radius);

        return path;
    }

    /// <summary>
    ///  Creates a path for the bottom and right segments of a rounded rectangle.
    /// </summary>
    private static GraphicsPath GetBottomRightSegmentPath(Rectangle bounds, int radius)
    {
        GraphicsPath path = new();

        int diameter = radius * 2;

        // Start from middle of top edge
        path.AddLine(bounds.Left + bounds.Width / 2, bounds.Top, bounds.Right, bounds.Top + bounds.Height / 2);

        // Right line
        path.AddLine(bounds.Right, bounds.Top + bounds.Height / 2, bounds.Right, bounds.Bottom - radius);

        // Bottom right corner arc
        Rectangle arcRect = new(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter);
        path.AddArc(arcRect, 0, 90);

        // Bottom line
        path.AddLine(bounds.Right - radius, bounds.Bottom, bounds.Left + radius, bounds.Bottom);

        // Bottom left corner arc
        arcRect.X = bounds.Left;
        path.AddArc(arcRect, 90, 45);

        // Path back to middle of left side
        path.AddLine(
            bounds.Left + (int)(radius * (1 - Math.Cos(Math.PI / 4))),
            bounds.Bottom - (int)(radius * Math.Sin(Math.PI / 4)),
            bounds.Left,
            bounds.Top + bounds.Height / 2);

        // Close the path back to start
        path.AddLine(bounds.Left, bounds.Top + bounds.Height / 2, bounds.Left + bounds.Width / 2, bounds.Top);

        return path;
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
