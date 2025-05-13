// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Interface for dark mode button renderers with various FlatStyle implementations.
/// </summary>
internal interface IButtonDarkModeRenderer
{
    /// <summary>
    ///  Draws button background with appropriate styling.
    /// </summary>
    /// <param name="graphics">Graphics context to draw on</param>
    /// <param name="bounds">Bounds of the button</param>
    /// <param name="state">State of the button (normal, hot, pressed, disabled)</param>
    /// <param name="isDefault">True if button is the default button</param>
    /// <returns>The content bounds (area inside the button for text/image)</returns>
    Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, VisualStyles.PushButtonState state, bool isDefault);

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
    Color GetTextColor(VisualStyles.PushButtonState state, bool isDefault);
}
