// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  This is a rendering class for the TextBox control.
/// </summary>
public static class TextBoxRenderer
{
    // Make this per-thread, so that different threads can safely use these methods.
    [ThreadStatic]
    private static VisualStyleRenderer? t_visualStyleRenderer;
    private static readonly VisualStyleElement s_textBoxElement = VisualStyleElement.TextBox.TextEdit.Normal;

    /// <summary>
    ///  Returns true if this class is supported for the current OS and user/application settings,
    ///  otherwise returns false.
    /// </summary>
    public static bool IsSupported => VisualStyleRenderer.IsSupported; // no downlevel support

    private static void DrawBackground(Graphics g, Rectangle bounds, TextBoxState state)
    {
        t_visualStyleRenderer!.DrawBackground(g, bounds);
        if (state != TextBoxState.Disabled)
        {
            Color windowColor = t_visualStyleRenderer.GetColor(ColorProperty.FillColor);
            if (windowColor != SystemColors.Window)
            {
                Rectangle fillRect = t_visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
                g.FillRectangle(SystemBrushes.Window, fillRect);
            }
        }
    }

    /// <summary>
    ///  Renders a TextBox control.
    /// </summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, TextBoxState state)
    {
        InitializeRenderer((int)state);
        DrawBackground(g, bounds, state);
    }

    /// <summary>
    ///  Renders a TextBox control.
    /// </summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, string? textBoxText, Font? font, TextBoxState state)
    {
        DrawTextBox(g, bounds, textBoxText, font, TextFormatFlags.TextBoxControl, state);
    }

    /// <summary>
    ///  Renders a TextBox control.
    /// </summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, string? textBoxText, Font? font, Rectangle textBounds, TextBoxState state)
    {
        DrawTextBox(g, bounds, textBoxText, font, textBounds, TextFormatFlags.TextBoxControl, state);
    }

    /// <summary>
    ///  Renders a TextBox control.
    /// </summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, string? textBoxText, Font? font, TextFormatFlags flags, TextBoxState state)
    {
        InitializeRenderer((int)state);
        Rectangle textBounds = t_visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
        textBounds.Inflate(-2, -2);
        DrawTextBox(g, bounds, textBoxText, font, textBounds, flags, state);
    }

    /// <summary>
    ///  Renders a TextBox control.
    /// </summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, string? textBoxText, Font? font, Rectangle textBounds, TextFormatFlags flags, TextBoxState state)
    {
        InitializeRenderer((int)state);

        DrawBackground(g, bounds, state);
        Color textColor = t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
        TextRenderer.DrawText(g, textBoxText, font, textBounds, textColor, flags);
    }

    [MemberNotNull(nameof(t_visualStyleRenderer))]
    private static void InitializeRenderer(int state)
    {
        if (t_visualStyleRenderer is null)
        {
            t_visualStyleRenderer = new VisualStyleRenderer(s_textBoxElement.ClassName, s_textBoxElement.Part, state);
        }
        else
        {
            t_visualStyleRenderer.SetParameters(s_textBoxElement.ClassName, s_textBoxElement.Part, state);
        }
    }
}
