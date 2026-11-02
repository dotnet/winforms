// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Interface for dark mode button renderers with various FlatStyle implementations.
/// </summary>
internal partial interface IButtonRenderer
{
    /// <summary>
    ///  Draws a button border using a specified path and border properties,
    ///  with the pen aligned inward to maintain consistent dimensions.
    /// </summary>
    static void DrawButtonBorder(Graphics graphics, GraphicsPath path, Color borderColor, int borderWidth)
    {
        using var borderPen = new Pen(borderColor, borderWidth)
        {
            Alignment = PenAlignment.Inset
        };

        graphics.DrawPath(borderPen, path);
    }

    /// <summary>
    ///  Renders the button with the specified style, state, and content.
    /// </summary>
    /// <param name="graphics">The graphics context to draw on.</param>
    /// <param name="control">The button control whose parent surface is used for exposed regions.</param>
    /// <param name="bounds">The bounds of the button.</param>
    /// <param name="flatStyle">The flat style of the button.</param>
    /// <param name="state">The visual state of the button (normal, hot, pressed, disabled, default).</param>
    /// <param name="isDefault">True if the button is the default button; otherwise, false.</param>
    /// <param name="focused">True if the button is focused; otherwise, false.</param>
    /// <param name="showFocusCues">True to show focus cues; otherwise, false.</param>
    /// <param name="parentBackgroundColor">The background color of the parent control.</param>
    /// <param name="paintContent">An action to lay out and paint the image and text within the content rectangle.</param>
    void RenderButton(
        Graphics graphics,
        Control control,
        Rectangle bounds,
        FlatStyle flatStyle,
        PushButtonState state,
        bool isDefault,
        bool focused,
        bool showFocusCues,
        Color parentBackgroundColor,
        Color backColor,
        Action<Rectangle> paintContent);

    /// <summary>
    ///  Draws button background with appropriate styling.
    /// </summary>
    /// <param name="graphics">Graphics context to draw on</param>
    /// <param name="bounds">Bounds of the button</param>
    /// <param name="state">State of the button (normal, hot, pressed, disabled)</param>
    /// <param name="isDefault">True if button is the default button</param>
    /// <param name="focused">True if the button is focused</param>
    /// <returns>The content bounds (area inside the button for text/image)</returns>
    Rectangle DrawButtonBackground(
        Graphics graphics,
        Rectangle bounds,
        PushButtonState state,
        bool isDefault,
        bool focused,
        Color backColor);

    /// <summary>
    ///  Draws focus indicator appropriate for this style.
    /// </summary>
    /// <param name="graphics">Graphics context to draw on</param>
    /// <param name="contentBounds">Content bounds of the button</param>
    /// <param name="isDefault">True if button is the default button</param>
    void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault);

    /// <summary>
    ///  Gets the text color appropriate for the button state and type.
    /// </summary>
    Color GetTextColor(PushButtonState state, bool isDefault, Color backColor);
}
