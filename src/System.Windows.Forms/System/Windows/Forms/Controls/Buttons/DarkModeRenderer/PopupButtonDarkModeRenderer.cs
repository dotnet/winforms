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
    // UI constants
    private const int ButtonCornerRadius = 5;
    private const int FocusCornerRadius = 3;
    private const int ContentPadding = 6;
    private const int FocusPadding = 2;
    private const int BorderThickness = 1;

    // Border color constants
    private static Color ShadowDarkColor { get; } = Color.FromArgb(40, 40, 40);         // Deeper shadow
    private static Color ShadowColor { get; } = Color.FromArgb(60, 60, 60);             // Standard shadow
    private static Color HighlightColor { get; } = Color.FromArgb(110, 110, 110);       // Standard highlight
    private static Color HighlightBrightColor { get; } = Color.FromArgb(130, 130, 130); // Brighter highlight
    private static Color DisabledBorderDarkColor { get; } = Color.FromArgb(45, 45, 45);
    private static Color DisabledBorderLightColor { get; } = Color.FromArgb(55, 55, 55);
    private static Color DisabledBorderMidColor { get; } = Color.FromArgb(50, 50, 50);

    // Default border color adjustment constants
    private const int DefaultBorderROffset = 30;
    private const int DefaultBorderGOffset = 20;
    private const int DefaultBorderBOffset = 40;

    // Make this per-thread, so that different threads can safely use these methods
    [ThreadStatic]
    private static GraphicsPath? s_cornerPath;

    // Cached pens for border drawing, also per-thread
    [ThreadStatic]
    private static Pen? s_topLeftOuterPen;

    [ThreadStatic]
    private static Pen? s_bottomRightOuterPen;

    [ThreadStatic]
    private static Pen? s_topLeftInnerPen;

    [ThreadStatic]
    private static Pen? s_bottomRightInnerPen;

    [ThreadStatic]
    private static Pen? s_defaultInnerBorderPen;

    /// <summary>
    ///  Draws button background with popup styling, including subtle 3D effect.
    /// </summary>
    public Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Save original smoothing mode and set to anti-alias for smooth corners
        SmoothingMode originalMode = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create path for rounded corners
        GraphicsPath path = GetRoundedRectanglePath(bounds, ButtonCornerRadius);

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
        return Rectangle.Inflate(bounds, -ContentPadding, -ContentPadding);
    }

    /// <summary>
    ///  Draws a focus rectangle with dotted lines inside the button.
    /// </summary>
    public void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault)
    {
        // Save original smoothing mode
        SmoothingMode originalMode = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a slightly smaller rectangle for the focus indicator
        Rectangle focusRect = Rectangle.Inflate(contentBounds, -FocusPadding, -FocusPadding);

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
        GraphicsPath focusPath = GetRoundedRectanglePath(focusRect, FocusCornerRadius);
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

    // Refactored DrawButtonBorder to use cached pens
    private static void DrawButtonBorder(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Create a GraphicsPath for the border to ensure consistent alignment
        // The path needs to match exactly the same dimensions as the filled background
        using GraphicsPath path = GetRoundedRectanglePath(bounds, ButtonCornerRadius);

        // Popup style has a 3D effect border
        Rectangle borderRect = bounds;

        // Use slightly more contrasting border colors for dark mode 3D effect
        Color topLeftOuter, bottomRightOuter, topLeftInner, bottomRightInner;

        if (state == PushButtonState.Pressed)
        {
            // In pressed state, invert the 3D effect: highlight bottom/right, shadow top/left
            topLeftOuter = ShadowColor;       // shadow
            bottomRightOuter = HighlightColor; // highlight
            topLeftInner = ShadowDarkColor;   // deeper shadow
            bottomRightInner = HighlightBrightColor; // brighter highlight
        }
        else if (state == PushButtonState.Disabled)
        {
            // Disabled: subtle, low-contrast border
            topLeftOuter = DisabledBorderLightColor;
            bottomRightOuter = DisabledBorderDarkColor;
            topLeftInner = DisabledBorderMidColor;
            bottomRightInner = DisabledBorderMidColor;
        }
        else
        {
            // Normal/hot: highlight top/left, shadow bottom/right
            topLeftOuter = HighlightColor;     // highlight
            bottomRightOuter = ShadowColor;     // shadow
            topLeftInner = HighlightBrightColor; // brighter highlight
            bottomRightInner = ShadowDarkColor;  // deeper shadow
        }

        // Initialize or update cached outer pens
        s_topLeftOuterPen = InitializeOrUpdatePen(s_topLeftOuterPen, topLeftOuter);
        s_bottomRightOuterPen = InitializeOrUpdatePen(s_bottomRightOuterPen, bottomRightOuter);

        // Draw the outer 3D border lines

        // Top
        graphics.DrawLine(s_topLeftOuterPen, borderRect.Left, borderRect.Top, borderRect.Right - 1, borderRect.Top);
        // Left
        graphics.DrawLine(s_topLeftOuterPen, borderRect.Left, borderRect.Top, borderRect.Left, borderRect.Bottom - 1);
        // Bottom
        graphics.DrawLine(s_bottomRightOuterPen, borderRect.Left, borderRect.Bottom - 1, borderRect.Right - 1, borderRect.Bottom - 1);
        // Right
        graphics.DrawLine(s_bottomRightOuterPen, borderRect.Right - 1, borderRect.Top, borderRect.Right - 1, borderRect.Bottom - 1);

        // Inner border for more depth
        borderRect.Inflate(-BorderThickness, -BorderThickness);

        // Initialize or update cached inner pens
        s_topLeftInnerPen ??= InitializeOrUpdatePen(s_topLeftInnerPen, topLeftInner);
        s_bottomRightInnerPen ??= InitializeOrUpdatePen(s_bottomRightInnerPen, bottomRightInner);

        // Draw the inner 3D border lines
        // Top
        graphics.DrawLine(s_topLeftInnerPen, borderRect.Left, borderRect.Top, borderRect.Right - 1, borderRect.Top);
        // Left
        graphics.DrawLine(s_topLeftInnerPen, borderRect.Left, borderRect.Top, borderRect.Left, borderRect.Bottom - 1);
        // Bottom
        graphics.DrawLine(s_bottomRightInnerPen, borderRect.Left, borderRect.Bottom - 1, borderRect.Right - 1, borderRect.Bottom - 1);
        // Right
        graphics.DrawLine(s_bottomRightInnerPen, borderRect.Right - 1, borderRect.Top, borderRect.Right - 1, borderRect.Bottom - 1);

        // For default buttons, add an additional inner border
        if (isDefault && state != PushButtonState.Disabled)
        {
            borderRect.Inflate(-BorderThickness, -BorderThickness);
            Color innerBorderColor = Color.FromArgb(
                Math.Max(0, ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.R - DefaultBorderROffset),
                Math.Max(0, ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.G - DefaultBorderGOffset),
                Math.Max(0, ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.B - DefaultBorderBOffset));

            // Initialize or update default inner border pen
            s_defaultInnerBorderPen ??= InitializeOrUpdatePen(s_defaultInnerBorderPen, innerBorderColor);
            graphics.DrawRectangle(s_defaultInnerBorderPen, borderRect);
        }
    }

    /// <summary>
    /// Initializes a new pen if null or updates an existing pen's color.
    /// </summary>
    private static Pen InitializeOrUpdatePen(Pen? pen, Color color)
    {
        if (pen is null)
        {
            pen = new Pen(color) { Alignment = PenAlignment.Inset };
        }
        else if (pen.Color != color)
        {
            pen.Color = color;
        }

        return pen;
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
