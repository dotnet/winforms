// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  This is a rendering class for the ComboBox control.
/// </summary>
public static class ComboBoxRenderer
{
    // Make this per-thread, so that different threads can safely use these methods.
    [ThreadStatic]
    private static VisualStyleRenderer? t_visualStyleRenderer;
    private static readonly VisualStyleElement s_comboBoxElement = VisualStyleElement.ComboBox.DropDownButton.Normal;
    private static readonly VisualStyleElement s_textBoxElement = VisualStyleElement.TextBox.TextEdit.Normal;

    /// <summary>
    ///  Returns true if this class is supported for the current OS and user/application settings,
    ///  otherwise returns false.
    /// </summary>
    public static bool IsSupported => VisualStyleRenderer.IsSupported; // no downlevel support

    private static void DrawBackground(Graphics g, Rectangle bounds, ComboBoxState state)
    {
        t_visualStyleRenderer!.DrawBackground(g, bounds);

        // for disabled comboboxes, comctl does not use the window backcolor, so
        // we don't refill here in that case.
        if (state != ComboBoxState.Disabled)
        {
            Color windowColor = t_visualStyleRenderer.GetColor(ColorProperty.FillColor);
            if (windowColor != Application.SystemColors.Window)
            {
                Rectangle fillRect = t_visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
                fillRect.Inflate(-2, -2);
                // then we need to re-fill the background.
                g.FillRectangle(SystemBrushes.Window, fillRect);
            }
        }
    }

    /// <summary>
    ///  Renders the textbox part of a ComboBox control.
    /// </summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, ComboBoxState state)
    {
        InitializeRenderer(s_textBoxElement, (int)state);

        DrawBackground(g, bounds, state);
    }

    /// <summary>
    ///  Renders the textbox part of a ComboBox control.
    /// </summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, string? comboBoxText, Font? font, ComboBoxState state)
    {
        DrawTextBox(g, bounds, comboBoxText, font, TextFormatFlags.TextBoxControl, state);
    }

    /// <summary>
    ///  Renders the textbox part of a ComboBox control.
    /// </summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, string? comboBoxText, Font? font, Rectangle textBounds, ComboBoxState state)
    {
        DrawTextBox(g, bounds, comboBoxText, font, textBounds, TextFormatFlags.TextBoxControl, state);
    }

    /// <summary>
    ///  Renders the textbox part of a ComboBox control.
    /// </summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, string? comboBoxText, Font? font, TextFormatFlags flags, ComboBoxState state)
    {
        InitializeRenderer(s_textBoxElement, (int)state);

        Rectangle textBounds = t_visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
        textBounds.Inflate(-2, -2);
        DrawTextBox(g, bounds, comboBoxText, font, textBounds, flags, state);
    }

    /// <summary>
    ///  Renders the textbox part of a ComboBox control.
    /// </summary>
    public static void DrawTextBox(Graphics g, Rectangle bounds, string? comboBoxText, Font? font, Rectangle textBounds, TextFormatFlags flags, ComboBoxState state)
    {
        InitializeRenderer(s_textBoxElement, (int)state);

        DrawBackground(g, bounds, state);
        Color textColor = t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
        TextRenderer.DrawText(g, comboBoxText, font, textBounds, textColor, flags);
    }

    /// <summary>
    ///  Renders a ComboBox drop-down button.
    /// </summary>
    public static void DrawDropDownButton(Graphics g, Rectangle bounds, ComboBoxState state)
    {
        using DeviceContextHdcScope hdc = new(g);
        DrawDropDownButtonForHandle(hdc, bounds, state, HWND.Null);
    }

    /// <summary>
    ///  Renders a ComboBox drop-down button in per-monitor scenario.
    /// </summary>
    /// <param name="hdc">Device context.</param>
    /// <param name="bounds">Dropdown button bounds.</param>
    /// <param name="state">State.</param>
    /// <param name="hwnd">Handle of the control.</param>
    internal static void DrawDropDownButtonForHandle(HDC hdc, Rectangle bounds, ComboBoxState state, HWND hwnd)
    {
        InitializeRenderer(s_comboBoxElement, (int)state);
        t_visualStyleRenderer.DrawBackground(hdc, bounds, hwnd);
    }

    [MemberNotNull(nameof(t_visualStyleRenderer))]
    private static void InitializeRenderer(VisualStyleElement visualStyleElement, int state)
    {
        if (t_visualStyleRenderer is null)
        {
            t_visualStyleRenderer = new VisualStyleRenderer(visualStyleElement.ClassName, visualStyleElement.Part, state);
        }
        else
        {
            t_visualStyleRenderer.SetParameters(visualStyleElement.ClassName, visualStyleElement.Part, state);
        }
    }
}
