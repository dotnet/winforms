// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods used to render a button control in dark mode.
/// </summary>
internal static partial class ButtonDarkModeRenderer
{
    // Singleton instances for each renderer type
    internal static IButtonDarkModeRenderer StandardRenderer { get; } = new StandardButtonDarkModeRenderer();

    internal static IButtonDarkModeRenderer FlatRenderer { get; } = new FlatButtonDarkModeRenderer();

    internal static IButtonDarkModeRenderer PopupRenderer { get; } = new PopupButtonDarkModeRenderer();

    internal static IButtonDarkModeRenderer SystemRenderer { get; } = new SystemButtonDarkModeRenderer();

    // Define padding values for each renderer type
    internal static Padding StandardPaddingCore { get; } = new(0);

    internal static Padding FlatPaddingCore { get; } = new(0);

    internal static Padding PopupPaddingCore { get; } = new(0);

    internal static Padding SystemPaddingCore { get; } = new(0);

    /// <summary>
    ///  Returns the appropriate padding for the specified flat style.
    /// </summary>
    internal static Padding GetPaddingCore(FlatStyle flatStyle) =>
        flatStyle switch
        {
            FlatStyle.Flat => FlatPaddingCore,
            FlatStyle.Popup => PopupPaddingCore,
            FlatStyle.System => SystemPaddingCore,
            _ => StandardPaddingCore
        };

    /// <summary>
    ///  Draws a button border using a specified path and border properties,
    ///  with the pen aligned inward to maintain consistent dimensions.
    /// </summary>
    public static void DrawButtonBorder(Graphics graphics, GraphicsPath path, Color borderColor, int borderWidth)
    {
        using var borderPen = new Pen(borderColor, borderWidth)
        {
            Alignment = PenAlignment.Inset
        };

        graphics.DrawPath(borderPen, path);
    }

    /// <summary>
    ///  Clears the background with the parent's background color or the control's background color if no parent is available.
    /// </summary>
    /// <param name="graphics">Graphics context to draw on</param>
    private static void ClearBackground(Graphics graphics, Color parentBackgroundColor)
    {
        graphics.Clear(parentBackgroundColor);
    }

    /// <summary>
    ///  Renders a button with the specified properties and delegates for painting image and field.
    /// </summary>
    internal static void RenderButton(
        Graphics graphics,
        Rectangle bounds,
        FlatStyle flatStyle,
        PushButtonState state,
        bool isDefault,
        bool focused,
        bool showFocusCues,
        Color parentBackgroundColor,
        Action<Rectangle> paintImage,
        Action<Rectangle, Color, bool> paintField)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(paintImage);
        ArgumentNullException.ThrowIfNull(paintField);

        IButtonDarkModeRenderer renderer = flatStyle switch
        {
            FlatStyle.Flat => FlatRenderer,
            FlatStyle.Popup => PopupRenderer,
            FlatStyle.System => SystemRenderer,
            _ => StandardRenderer
        };

        // Clear the background over the whole button area.
        ClearBackground(graphics, parentBackgroundColor);

        // Use padding from ButtonDarkModeRenderer
        Padding padding = GetPaddingCore(flatStyle);

        Rectangle paddedBounds = new(
            x: bounds.X + padding.Left,
            y: bounds.Y + padding.Top,
            width: bounds.Width - padding.Horizontal,
            height: bounds.Height - padding.Vertical);

        // Draw button background and get content bounds
        Rectangle contentBounds = renderer.DrawButtonBackground(graphics, paddedBounds, state, isDefault);

        // Offset content bounds for Popup style when button is pressed
        if (flatStyle == FlatStyle.Popup && state == PushButtonState.Pressed)
        {
            contentBounds.Offset(1, 1);
        }

        // Paint image and field using the provided delegates
        paintImage(contentBounds);

        paintField(
            contentBounds,
            renderer.GetTextColor(state, isDefault),
            false);

        if (focused && flatStyle == FlatStyle.System)
        {
            // Focus with the system renderer is completely surrounded by a border.
            renderer.DrawFocusIndicator(graphics, bounds, isDefault);

            return;
        }

        if (focused && showFocusCues)
        {
            // Draw focus indicator for other styles
            renderer.DrawFocusIndicator(graphics, contentBounds, isDefault);
        }
    }
}
