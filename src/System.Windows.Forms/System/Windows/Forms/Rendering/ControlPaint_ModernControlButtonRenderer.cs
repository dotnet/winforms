// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms;

public static unsafe partial class ControlPaint
{
    /// <summary>
    ///  Scaling factor for button content (arrows, dots, etc.).
    ///  Values greater than 1.0 make the content larger relative to the button.
    /// </summary>
    private const double ContentScaleFactor = 1.5;

    /// <summary>
    ///  Draws a modern up/down button suitable for both light and dark modes.
    /// </summary>
    internal static void DrawModernControlButton(
        Graphics graphics,
        Rectangle bounds,
        ModernControlButtonStyle button,
        ModernControlButtonState state,
        bool isDarkMode)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        // Define colors for different states
        Color backgroundColor;
        Color borderColor;
        Color arrowColor;

        if (isDarkMode)
        {
            // Dark mode colors
            backgroundColor = state switch
            {
                ModernControlButtonState.Pressed => s_darkModeBackgroundPressed,
                ModernControlButtonState.Disabled => s_darkModeBackgroundDisabled,
                ModernControlButtonState.Hover => s_darkModeBackgroundHover,
                _ => s_darkModeBackgroundNormal
            };

            borderColor = state switch
            {
                ModernControlButtonState.Pressed => s_darkModeBorderPressed,
                ModernControlButtonState.Disabled => s_darkModeBorderDisabled,
                ModernControlButtonState.Hover => s_darkModeBorderHover,
                _ => s_darkModeBorderNormal
            };

            arrowColor = state == ModernControlButtonState.Disabled
                ? s_darkModeArrowDisabled
                : s_darkModeArrowNormal;
        }
        else
        {
            // Light mode colors
            backgroundColor = state switch
            {
                ModernControlButtonState.Pressed => s_lightModeBackgroundPressed,
                ModernControlButtonState.Disabled => s_lightModeBackgroundDisabled,
                ModernControlButtonState.Hover => s_lightModeBackgroundHover,
                _ => s_lightModeBackgroundNormal
            };

            borderColor = state switch
            {
                ModernControlButtonState.Pressed => s_lightModeBorderPressed,
                ModernControlButtonState.Disabled => s_lightModeBorderDisabled,
                ModernControlButtonState.Hover => s_lightModeBorderHover,
                _ => s_lightModeBorderNormal
            };

            arrowColor = state == ModernControlButtonState.Disabled
                ? s_lightModeArrowDisabled
                : s_lightModeArrowNormal;
        }

        // Enable anti-aliasing for smooth rendering
        SmoothingMode oldMode = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        try
        {
            // Extract border flags
            bool hasBorder = (button & ModernControlButtonStyle.SingleBorder) != 0
                || (button & ModernControlButtonStyle.RoundedBorder) != 0;

            bool hasRoundedBorder = (button & ModernControlButtonStyle.RoundedBorder) != 0;

            bool isFocused = state == ModernControlButtonState.Focused;

            // Draw background
            using (var backgroundBrush = backgroundColor.GetCachedSolidBrushScope())
            {
                if (hasRoundedBorder)
                {
                    Size radius = new(4, 4);
                    graphics.FillRoundedRectangle(backgroundBrush, bounds, radius);
                }
                else
                {
                    graphics.FillRectangle(backgroundBrush, bounds);
                }
            }

            // Draw border if needed
            if (hasBorder)
            {
                using var borderPen = borderColor.GetCachedPenScope();

                if (hasRoundedBorder)
                {
                    Size radius = new(4, 4);
                    graphics.DrawRoundedRectangle(borderPen, bounds, radius);
                }
                else
                {
                    graphics.DrawRectangle(borderPen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }
            }

            if (isFocused)
            {
                // Draw focus rectangle
                Rectangle focusRect = bounds;
                focusRect.Inflate(-1, -1); // Deflate to avoid drawing over the border
                DrawFocusRectangle(graphics, focusRect, Color.Empty, backgroundColor);
            }

            // Draw the content
            ModernControlButtonStyle contentType = button
                & ~(ModernControlButtonStyle.SingleBorder
                    | ModernControlButtonStyle.RoundedBorder);

            bool isPressed = state == ModernControlButtonState.Pressed;
            DrawButtonContent(graphics, bounds, contentType, arrowColor, isPressed);
        }
        finally
        {
            graphics.SmoothingMode = oldMode;
        }
    }

    /// <summary>
    ///  Draws the button content based on the button type.
    /// </summary>
    private static void DrawButtonContent(
        Graphics graphics,
        Rectangle bounds,
        ModernControlButtonStyle buttonType,
        Color contentColor,
        bool isPressed)
    {
        // Calculate center point
        int centerX = bounds.X + bounds.Width / 2;
        int centerY = bounds.Y + bounds.Height / 2;

        // Apply pressed offset
        if (isPressed)
        {
            centerX += 1;
            centerY += 1;
        }

        using var contentBrush = contentColor.GetCachedSolidBrushScope();

        switch (buttonType)
        {
            case ModernControlButtonStyle.Empty:
                // Nothing to draw
                break;

            case ModernControlButtonStyle.Up:
                DrawUpArrow(graphics, contentBrush, centerX, centerY, ScaleSymbolSize(bounds));
                break;

            case ModernControlButtonStyle.Down:
                DrawDownArrow(graphics, contentBrush, centerX, centerY, ScaleSymbolSize(bounds));
                break;

            case ModernControlButtonStyle.UpDown:
                DrawUpDownArrows(graphics, contentBrush, centerX, centerY, bounds);
                break;

            case ModernControlButtonStyle.Left:
                DrawLeftArrow(graphics, contentBrush, centerX, centerY, ScaleSymbolSize(bounds));
                break;

            case ModernControlButtonStyle.Right:
                DrawRightArrow(graphics, contentBrush, centerX, centerY, ScaleSymbolSize(bounds));
                break;

            case ModernControlButtonStyle.RightLeft:
                DrawLeftRightArrows(graphics, contentBrush, centerX, centerY, bounds);
                break;

            case ModernControlButtonStyle.Ellipse:
                DrawEllipseSymbol(graphics, contentBrush, centerX, centerY, bounds);
                break;

            case ModernControlButtonStyle.OpenDropDown:
                DrawOpenDropDownChevron(graphics, contentBrush, centerX, centerY, ScaleSymbolSize(bounds));
                break;
        }
    }

    /// <summary>
    ///  Calculates the arrow size based on button bounds and DPI scaling.
    /// </summary>
    private static int ScaleSymbolSize(Rectangle bounds)
    {
        // Base size is calculated as a fraction of the smaller dimension
        int minDimension = Math.Min(bounds.Width, bounds.Height);

        // Use a base ratio that provides good visual balance
        // This gives us roughly 40% of the button size for the symbol
        const double baseSymbolRatio = 0.4;

        // Calculate the symbol size with scaling factor applied
        int symbolSize = (int)(minDimension * baseSymbolRatio * ContentScaleFactor);

        // Ensure we always have at least a 1-pixel symbol
        return Math.Max(1, symbolSize);
    }

    /// <summary>
    ///  Draws combined up/down arrows with proportional spacing.
    /// </summary>
    private static void DrawUpDownArrows(Graphics graphics, Brush brush, int centerX, int centerY, Rectangle bounds)
    {
        // Get the base symbol size
        int baseArrowSize = ScaleSymbolSize(bounds);

        // For combined arrows, reduce size slightly to fit both with spacing
        int arrowSize = (int)(baseArrowSize * 0.7);

        // Calculate proportional spacing based on arrow size
        int spacing = Math.Max(1, arrowSize / 3);

        // Calculate total height needed
        int totalHeight = (arrowSize * 2) + spacing;

        // Adjust positions to center the combined arrows
        int topY = centerY - (totalHeight / 2) + (arrowSize / 2);
        int bottomY = centerY + (totalHeight / 2) - (arrowSize / 2);

        // Draw up arrow
        DrawUpArrow(graphics, brush, centerX, topY, arrowSize);

        // Draw down arrow
        DrawDownArrow(graphics, brush, centerX, bottomY, arrowSize);
    }

    /// <summary>
    ///  Draws combined left/right arrows with proportional spacing.
    /// </summary>
    private static void DrawLeftRightArrows(Graphics graphics, Brush brush, int centerX, int centerY, Rectangle bounds)
    {
        // Get the base symbol size
        int baseArrowSize = ScaleSymbolSize(bounds);

        // For combined arrows, reduce size slightly to fit both with spacing
        int arrowSize = (int)(baseArrowSize * 0.7);

        // Calculate proportional spacing based on arrow size
        int spacing = Math.Max(1, arrowSize / 3);

        // Calculate total width needed
        int totalWidth = (arrowSize * 2) + spacing;

        // Adjust positions to center the combined arrows
        int leftX = centerX - (totalWidth / 2) + (arrowSize / 2);
        int rightX = centerX + (totalWidth / 2) - (arrowSize / 2);

        // Draw left arrow
        DrawLeftArrow(graphics, brush, leftX, centerY, arrowSize);

        // Draw right arrow
        DrawRightArrow(graphics, brush, rightX, centerY, arrowSize);
    }

    /// <summary>
    ///  Draws an ellipse symbol (...) with DPI-aware sizing.
    /// </summary>
    private static void DrawEllipseSymbol(Graphics graphics, Brush brush, int centerX, int centerY, Rectangle bounds)
    {
        // Calculate dot size as a proportion of button height
        int minDimension = Math.Min(bounds.Width, bounds.Height);
        int dotSize = Math.Max(1, (int)(minDimension * 0.1 * ContentScaleFactor));

        // Calculate proportional spacing
        int spacing = Math.Max(1, dotSize / 2);

        // Calculate total width for centering
        int totalWidth = (dotSize * 3) + (spacing * 2);

        // Center the dots
        int startX = centerX - totalWidth / 2 + dotSize / 2;

        for (int i = 0; i < 3; i++)
        {
            int x = startX + (i * (dotSize + spacing));

            graphics.FillEllipse(
                brush: brush,
                x: x - dotSize / 2,
                y: centerY - dotSize / 2,
                width: dotSize,
                height: dotSize);
        }
    }

    /// <summary>
    ///  Draws a classic down-facing chevron for combo box dropdowns with DPI-aware sizing.
    /// </summary>
    private static void DrawOpenDropDownChevron(Graphics graphics, Brush brush, int centerX, int centerY, int size)
    {
        // Calculate chevron dimensions proportionally
        int chevronWidth = size;
        int chevronHeight = (size * 2) / 3;

        // Stroke thickness scales with size
        int strokeThickness = Math.Max(1, size / 4);

        // Define the 4 points of the chevron (two strokes forming a downward angle)
        Point[] leftStroke =
        [
            new Point(centerX - chevronWidth / 2, centerY - chevronHeight / 3),
            new Point(centerX - strokeThickness / 2, centerY + chevronHeight / 3),
            new Point(centerX + strokeThickness / 2, centerY + chevronHeight / 3),
            new Point(centerX - chevronWidth / 2 + strokeThickness, centerY - chevronHeight / 3),
        ];

        Point[] rightStroke =
        [
            new Point(centerX + chevronWidth / 2 - strokeThickness, centerY - chevronHeight / 3),
            new Point(centerX - strokeThickness / 2, centerY + chevronHeight / 3),
            new Point(centerX + strokeThickness / 2, centerY + chevronHeight / 3),
            new Point(centerX + chevronWidth / 2, centerY - chevronHeight / 3),
        ];

        // Draw both strokes to form the chevron
        graphics.FillPolygon(brush, leftStroke);
        graphics.FillPolygon(brush, rightStroke);
    }

    private static void DrawUpArrow(Graphics graphics, Brush brush, int centerX, int centerY, int size)
    {
        Point[] points =
        [
            new Point(centerX, centerY - size / 2),      // Top point
            new Point(centerX - size / 2, centerY + size / 2), // Bottom left
            new Point(centerX + size / 2, centerY + size / 2), // Bottom right
        ];

        graphics.FillPolygon(brush, points);
    }

    private static void DrawDownArrow(Graphics graphics, Brush brush, int centerX, int centerY, int size)
    {
        Point[] points =
        [
            new Point(centerX, centerY + size / 2),      // Bottom point
            new Point(centerX - size / 2, centerY - size / 2), // Top left
            new Point(centerX + size / 2, centerY - size / 2), // Top right
        ];

        graphics.FillPolygon(brush, points);
    }

    private static void DrawLeftArrow(Graphics graphics, Brush brush, int centerX, int centerY, int size)
    {
        Point[] points =
        [
            new Point(centerX - size / 2, centerY),      // Left point
            new Point(centerX + size / 2, centerY - size / 2), // Top right
            new Point(centerX + size / 2, centerY + size / 2), // Bottom right
        ];

        graphics.FillPolygon(brush, points);
    }

    private static void DrawRightArrow(Graphics graphics, Brush brush, int centerX, int centerY, int size)
    {
        Point[] points =
        [
            new Point(centerX + size / 2, centerY),      // Right point
            new Point(centerX - size / 2, centerY - size / 2), // Top left
            new Point(centerX - size / 2, centerY + size / 2), // Bottom left
        ];

        graphics.FillPolygon(brush, points);
    }
}
