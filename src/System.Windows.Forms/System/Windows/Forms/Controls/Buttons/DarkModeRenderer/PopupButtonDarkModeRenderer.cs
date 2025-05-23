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
    private const int FocusPadding = 2;
    private const int BorderThickness = 2;
    private const int ContentOffset = 1; // Offset for content when pressed

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

    /// <summary>
    ///  Draws button background with popup styling, including subtle 3D effect.
    /// </summary>
    public Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Use padding from ButtonDarkModeRenderer
        Padding padding = ButtonDarkModeRenderer.GetPaddingCore(FlatStyle.Popup);
        Rectangle paddedBounds = Rectangle.Inflate(bounds, -padding.Left, -padding.Top);

        // Content rect will be used to position text and images
        Rectangle contentBounds = Rectangle.Inflate(paddedBounds, -padding.Left, -padding.Top);

        // Adjust content position when pressed to enhance 3D effect
        if (state == PushButtonState.Pressed)
        {
            contentBounds.Offset(ContentOffset, ContentOffset);
        }

        // Get appropriate background color based on state
        Color backColor = GetBackgroundColor(state, isDefault);

        // Create path for rounded corners
        using GraphicsPath path = CreateRoundedRectanglePath(paddedBounds, ButtonCornerRadius);

        // Fill the background using cached brush
        using var brush = backColor.GetCachedSolidBrushScope();
        graphics.FillPath(brush, path);

        // Draw 3D effect borders
        DrawButtonBorder(graphics, paddedBounds, state, isDefault);

        // Return content bounds (area inside the button for text/image)
        return contentBounds;
    }

    /// <summary>
    ///  Draws a focus rectangle with dotted lines inside the button.
    ///  Adjusts for the 3D effect based on the button's state.
    /// </summary>
    public void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault)
    {
        // Create a slightly smaller rectangle for the focus indicator
        Rectangle focusRect = Rectangle.Inflate(contentBounds, -FocusPadding, -FocusPadding);

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
        using GraphicsPath focusPath = CreateRoundedRectanglePath(focusRect, FocusCornerRadius);
        graphics.DrawPath(focusPen, focusPath);
    }

    /// <summary>
    ///  Gets the text color appropriate for the button state and type.
    ///  Adjusts color for 3D effect when needed.
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
    ///  Draws the 3D effect border for the button.
    /// </summary>
    private static void DrawButtonBorder(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Save original smoothing mode to restore later
        SmoothingMode originalMode = graphics.SmoothingMode;

        try
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Create a GraphicsPath for the border to ensure consistent alignment
            // The path needs to match exactly the same dimensions as the filled background
            using GraphicsPath path = CreateRoundedRectanglePath(bounds, ButtonCornerRadius);

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

            // Create and use outer pens with proper disposal
            using (var topLeftOuterPen = new Pen(topLeftOuter) { Alignment = PenAlignment.Inset })
            using (var bottomRightOuterPen = new Pen(bottomRightOuter) { Alignment = PenAlignment.Inset })
            {
                // Draw the outer 3D border lines
                // Top
                graphics.DrawLine(topLeftOuterPen, borderRect.Left, borderRect.Top, borderRect.Right - 1, borderRect.Top);
                // Left
                graphics.DrawLine(topLeftOuterPen, borderRect.Left, borderRect.Top, borderRect.Left, borderRect.Bottom - 1);
                // Bottom
                graphics.DrawLine(bottomRightOuterPen, borderRect.Left, borderRect.Bottom - 1, borderRect.Right - 1, borderRect.Bottom - 1);
                // Right
                graphics.DrawLine(bottomRightOuterPen, borderRect.Right - 1, borderRect.Top, borderRect.Right - 1, borderRect.Bottom - 1);
            }

            // Inner border for more depth
            borderRect.Inflate(-BorderThickness, -BorderThickness);

            // Create and use inner pens with proper disposal
            using (var topLeftInnerPen = new Pen(topLeftInner) { Alignment = PenAlignment.Inset })
            using (var bottomRightInnerPen = new Pen(bottomRightInner) { Alignment = PenAlignment.Inset })
            {
                // Draw the inner 3D border lines
                // Top
                graphics.DrawLine(topLeftInnerPen, borderRect.Left, borderRect.Top, borderRect.Right - 1, borderRect.Top);
                // Left
                graphics.DrawLine(topLeftInnerPen, borderRect.Left, borderRect.Top, borderRect.Left, borderRect.Bottom - 1);
                // Bottom
                graphics.DrawLine(bottomRightInnerPen, borderRect.Left, borderRect.Bottom - 1, borderRect.Right - 1, borderRect.Bottom - 1);
                // Right
                graphics.DrawLine(bottomRightInnerPen, borderRect.Right - 1, borderRect.Top, borderRect.Right - 1, borderRect.Bottom - 1);
            }

            // For default buttons, add an additional inner border
            if (isDefault && state != PushButtonState.Disabled)
            {
                borderRect.Inflate(-BorderThickness, -BorderThickness);
                Color innerBorderColor = Color.FromArgb(
                    Math.Max(0, ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.R - DefaultBorderROffset),
                    Math.Max(0, ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.G - DefaultBorderGOffset),
                    Math.Max(0, ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.B - DefaultBorderBOffset));

                // Create and use default inner border pen with proper disposal
                using var defaultInnerBorderPen = new Pen(innerBorderColor) { Alignment = PenAlignment.Inset };
                graphics.DrawRectangle(defaultInnerBorderPen, borderRect);
            }
        }
        finally
        {
            // Restore original smoothing mode
            graphics.SmoothingMode = originalMode;
        }
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
