// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.DarkModeButtonColors;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods for rendering a button with Popup FlatStyle in dark mode.
/// </summary>
internal class PopupButtonDarkModeRenderer : ButtonDarkModeRendererBase
{
    // UI constants
    private const int ButtonCornerRadius = 5;
    private const int FocusCornerRadius = 3;
    private const int FocusPadding = 2;
    private const int BorderThickness = 2;
    private const int ContentOffset = 1; // Offset for content when pressed

    private protected override Padding PaddingCore { get; } = new(0);

    // Default border color adjustment constants
    private const int DefaultBorderROffset = 30;
    private const int DefaultBorderGOffset = 20;
    private const int DefaultBorderBOffset = 40;

    /// <summary>
    ///  Draws button background with popup styling, including subtle 3D effect.
    /// </summary>
    public override Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Use padding from ButtonDarkModeRenderer
        Padding padding = PaddingCore;
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
    public override void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault)
    {
        // Create a slightly smaller rectangle for the focus indicator
        Rectangle focusRect = Rectangle.Inflate(contentBounds, -FocusPadding, -FocusPadding);

        // Create dotted pen with appropriate color
        Color focusColor = isDefault
            ? DefaultColors.AcceptFocusIndicatorBackColor
            : DefaultColors.FocusIndicatorBackColor;

        // See GDI+ best practices: pens with custom DashStyle must not use cached pens.
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
    public override Color GetTextColor(PushButtonState state, bool isDefault) =>
        state == PushButtonState.Disabled
            ? DefaultColors.DisabledTextColor
            : isDefault
                ? DefaultColors.AcceptButtonTextColor
                : DefaultColors.NormalTextColor;

    /// <summary>
    ///  Gets the background color appropriate for the button state and type.
    /// </summary>
    private static Color GetBackgroundColor(PushButtonState state, bool isDefault) =>
        isDefault
            ? state switch
            {
                PushButtonState.Normal => DefaultColors.StandardBackColor,
                PushButtonState.Hot => DefaultColors.HoverBackColor,
                PushButtonState.Pressed => DefaultColors.PressedBackColor,
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
                topLeftOuter = DefaultColors.ShadowColor;       // shadow
                bottomRightOuter = DefaultColors.HighlightColor; // highlight
                topLeftInner = DefaultColors.ShadowDarkColor;   // deeper shadow
                bottomRightInner = DefaultColors.HighlightBrightColor; // brighter highlight
            }
            else if (state == PushButtonState.Disabled)
            {
                // Disabled: subtle, low-contrast border
                topLeftOuter = DefaultColors.DisabledBorderLightColor;
                bottomRightOuter = DefaultColors.DisabledBorderDarkColor;
                topLeftInner = DefaultColors.DisabledBorderMidColor;
                bottomRightInner = DefaultColors.DisabledBorderMidColor;
            }
            else
            {
                // Normal/hot: highlight top/left, shadow bottom/right
                topLeftOuter = DefaultColors.HighlightColor;     // highlight
                bottomRightOuter = DefaultColors.ShadowColor;     // shadow
                topLeftInner = DefaultColors.HighlightBrightColor; // brighter highlight
                bottomRightInner = DefaultColors.ShadowDarkColor;  // deeper shadow
            }

            // Custom pen needed for PenAlignment.Inset - can't use cached version
            // See GDI+ best practices: pens with custom alignment must not use cached pens.
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
                    Math.Max(0, DefaultColors.StandardBackColor.R - DefaultBorderROffset),
                    Math.Max(0, DefaultColors.StandardBackColor.G - DefaultBorderGOffset),
                    Math.Max(0, DefaultColors.StandardBackColor.B - DefaultBorderBOffset));

                // Custom pen needed for PenAlignment.Inset - can't use cached version
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
