// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms;

public static unsafe partial class ControlPaint
{
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    /// <summary>
    ///  Draws a modern up/down button suitable for both light and dark modes.
    /// </summary>
    internal static void DrawModernControlButton(
        Graphics graphics,
        Rectangle bounds,
        ModernControlButton button,
        bool isPressed,
        bool isDisabled,
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
            backgroundColor =
                isPressed
                    ? s_darkModeBackgroundPressed
                    : isDisabled
                        ? s_darkModeBackgroundDisabled
                        : s_darkModeBackgroundNormal;

            borderColor =
                isPressed
                    ? s_darkModeBorderPressed
                    : isDisabled
                        ? s_darkModeBorderDisabled
                        : s_darkModeBorderNormal;

            arrowColor = isDisabled
                ? s_darkModeArrowDisabled
                : s_darkModeArrowNormal;
        }
        else
        {
            // Light mode colors
            backgroundColor =
                isPressed
                    ? s_lightModeBackgroundPressed
                    : isDisabled
                        ? s_lightModeBackgroundDisabled
                        : s_lightModeBackgroundNormal;

            borderColor =
                isPressed
                    ? s_lightModeBorderPressed
                    : isDisabled
                        ? s_lightModeBorderDisabled
                        : s_lightModeBorderNormal;

            arrowColor = isDisabled
                ? s_lightModeArrowDisabled
                : s_lightModeArrowNormal;
        }

        // Enable anti-aliasing for smooth rendering
        SmoothingMode oldMode = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        try
        {
            // Extract border flags
            bool hasBorder = (button & ModernControlButton.SingleBorder) != 0 || (button & ModernControlButton.RoundedBorder) != 0;
            bool hasRoundedBorder = (button & ModernControlButton.RoundedBorder) != 0;

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

            // Draw the content
            ModernControlButton contentType = button & ~(ModernControlButton.SingleBorder | ModernControlButton.RoundedBorder);
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
        ModernControlButton buttonType,
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
            case ModernControlButton.Empty:
                // Nothing to draw
                break;

            case ModernControlButton.Up:
                DrawUpArrow(graphics, contentBrush, centerX, centerY, CalculateArrowSize(bounds));
                break;

            case ModernControlButton.Down:
                DrawDownArrow(graphics, contentBrush, centerX, centerY, CalculateArrowSize(bounds));
                break;

            case ModernControlButton.UpDown:
                DrawUpDownArrows(graphics, contentBrush, centerX, centerY, bounds);
                break;

            case ModernControlButton.Left:
                DrawLeftArrow(graphics, contentBrush, centerX, centerY, CalculateArrowSize(bounds));
                break;

            case ModernControlButton.Right:
                DrawRightArrow(graphics, contentBrush, centerX, centerY, CalculateArrowSize(bounds));
                break;

            case ModernControlButton.LeftRight:
                DrawLeftRightArrows(graphics, contentBrush, centerX, centerY, bounds);
                break;

            case ModernControlButton.Ellipse:
                DrawEllipse(graphics, contentBrush, centerX, centerY, bounds);
                break;
        }
    }

    /// <summary>
    ///  Calculates the arrow size based on button bounds.
    /// </summary>
    private static int CalculateArrowSize(Rectangle bounds)
    {
        int arrowSize = Math.Min(bounds.Width, bounds.Height) / 2;

        return Math.Max(3, Math.Min(arrowSize, 7)); // Clamp between 3 and 7 pixels
    }

    /// <summary>
    ///  Draws combined up/down arrows with kerning.
    /// </summary>
    private static void DrawUpDownArrows(Graphics graphics, Brush brush, int centerX, int centerY, Rectangle bounds)
    {
        int arrowSize = CalculateArrowSize(bounds) - 1; // Slightly smaller for combined
        int spacing = 2; // Minimal spacing between arrows

        // Draw up arrow (top half)
        int upCenterY = centerY - spacing;
        DrawUpArrow(graphics, brush, upCenterY - arrowSize / 2, centerX, arrowSize);

        // Draw down arrow (bottom half)
        int downCenterY = centerY + spacing;
        DrawDownArrow(graphics, brush, downCenterY + arrowSize / 2, centerX, arrowSize);
    }

    /// <summary>
    ///  Draws combined left/right arrows with kerning.
    /// </summary>
    private static void DrawLeftRightArrows(Graphics graphics, Brush brush, int centerX, int centerY, Rectangle bounds)
    {
        int arrowSize = CalculateArrowSize(bounds) - 1; // Slightly smaller for combined
        int spacing = 2; // Minimal spacing between arrows

        // Draw left arrow
        int leftCenterX = centerX - spacing;
        DrawLeftArrow(graphics, brush, leftCenterX - arrowSize / 2, centerY, arrowSize);

        // Draw right arrow
        int rightCenterX = centerX + spacing;
        DrawRightArrow(graphics, brush, rightCenterX + arrowSize / 2, centerY, arrowSize);
    }

    /// <summary>
    ///  Draws an ellipse symbol (...).
    /// </summary>
    private static void DrawEllipse(Graphics graphics, Brush brush, int centerX, int centerY, Rectangle bounds)
    {
        int dotSize = Math.Max(2, Math.Min(bounds.Height / 8, 3));
        int spacing = dotSize + 1;
        int totalWidth = (dotSize * 3) + (spacing * 2);

        // Center the dots
        int startX = centerX - totalWidth / 2 + dotSize / 2;

        for (int i = 0; i < 3; i++)
        {
            int x = startX + (i * (dotSize + spacing));
            graphics.FillEllipse(brush, x - dotSize / 2, centerY - dotSize / 2, dotSize, dotSize);
        }
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
