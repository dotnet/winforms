// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods used to render a button control in dark mode.
/// </summary>
internal abstract partial class ButtonDarkModeRendererBase : IButtonRenderer
{
    // Define padding values for each renderer type
    private protected abstract Padding PaddingCore { get; }

    /// <summary>
    ///  Clears the background with the parent's background color or the control's background color if no parent is available.
    /// </summary>
    /// <param name="graphics">Graphics context to draw on</param>
    private static void ClearBackground(Graphics graphics, Color parentBackgroundColor)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        graphics.Clear(parentBackgroundColor);
    }

    /// <summary>
    ///  Renders a button with the specified properties and delegates for painting image and field.
    /// </summary>
    public void RenderButton(
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

        // Clear the background over the whole button area.
        ClearBackground(graphics, parentBackgroundColor);

        // Use padding from ButtonDarkModeRenderer
        Padding padding = PaddingCore;

        Rectangle paddedBounds = new(
            x: bounds.X + padding.Left,
            y: bounds.Y + padding.Top,
            width: bounds.Width - padding.Horizontal,
            height: bounds.Height - padding.Vertical);

        // Draw button background and get content bounds
        Rectangle contentBounds = DrawButtonBackground(graphics, paddedBounds, state, isDefault);

        // Offset content bounds for Popup style when button is pressed
        // if (flatStyle == FlatStyle.Popup && state == PushButtonState.Pressed)
        // {
        //    contentBounds.Offset(1, 1);
        // }

        // Paint image and field using the provided delegates
        paintImage(contentBounds);

        paintField(
            contentBounds,
            GetTextColor(state, isDefault),
            false);

        if (focused && showFocusCues)
        {
            // Draw focus indicator for other styles
            DrawFocusIndicator(graphics, bounds, isDefault);
        }
    }

    public abstract Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault);

    public abstract void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault);

    public abstract Color GetTextColor(PushButtonState state, bool isDefault);
}
