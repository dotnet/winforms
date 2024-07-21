// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods used to render a check box control with or without visual styles.
/// </summary>
public static class CheckBoxRenderer
{
    // Make this per-thread, so that different threads can safely use these methods.
    [ThreadStatic]
    private static VisualStyleRenderer? t_visualStyleRenderer;
    private static readonly VisualStyleElement s_checkBoxElement = VisualStyleElement.Button.CheckBox.UncheckedNormal;

    /// <inheritdoc cref="ButtonRenderer.RenderMatchingApplicationState"/>
    public static bool RenderMatchingApplicationState { get; set; } = true;

    private static bool RenderWithVisualStyles => !RenderMatchingApplicationState || Application.RenderWithVisualStyles;

    /// <inheritdoc cref="ButtonRenderer.IsBackgroundPartiallyTransparent(PushButtonState)"/>
    public static bool IsBackgroundPartiallyTransparent(CheckBoxState state)
    {
        if (RenderWithVisualStyles)
        {
            InitializeRenderer((int)state);
            return t_visualStyleRenderer.IsBackgroundPartiallyTransparent();
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc cref="ButtonRenderer.DrawParentBackground(Graphics, Rectangle, Control)"/>
    public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl)
    {
        if (RenderWithVisualStyles)
        {
            InitializeRenderer(0);

            t_visualStyleRenderer.DrawParentBackground(g, bounds, childControl);
        }
    }

    /// <inheritdoc cref="DrawCheckBox(Graphics, Point, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, CheckBoxState)"/>
    public static void DrawCheckBox(Graphics g, Point glyphLocation, CheckBoxState state)
    {
        if (RenderWithVisualStyles)
        {
            DrawCheckBoxWithVisualStyles(g, glyphLocation, state);
        }
        else
        {
            Rectangle glyphBounds = new(glyphLocation, GetGlyphSize(g, state));
            if (IsMixed(state))
            {
                ControlPaint.DrawMixedCheckBox(g, glyphBounds, ConvertToButtonState(state));
            }
            else
            {
                ControlPaint.DrawCheckBox(g, glyphBounds, ConvertToButtonState(state));
            }
        }
    }

    internal static void DrawCheckBoxWithVisualStyles(
        IDeviceContext deviceContext,
        Point glyphLocation,
        CheckBoxState state,
        HWND hwnd = default)
    {
        InitializeRenderer((int)state);

        using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();
        Rectangle glyphBounds = new(glyphLocation, GetGlyphSize(hdc, state, hwnd));
        t_visualStyleRenderer.DrawBackground(hdc, glyphBounds, hwnd);
    }

    /// <inheritdoc cref="DrawCheckBox(Graphics, Point, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, CheckBoxState)"/>
    public static void DrawCheckBox(
        Graphics g,
        Point glyphLocation,
        Rectangle textBounds,
        string? checkBoxText,
        Font? font,
        bool focused,
        CheckBoxState state) => DrawCheckBox(
            g,
            glyphLocation,
            textBounds,
            checkBoxText,
            font,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
            focused,
            state);

    /// <inheritdoc cref="DrawCheckBox(Graphics, Point, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, CheckBoxState)"/>
    public static void DrawCheckBox(
        Graphics g,
        Point glyphLocation,
        Rectangle textBounds,
        string? checkBoxText,
        Font? font,
        TextFormatFlags flags,
        bool focused,
        CheckBoxState state) => DrawCheckBox(g, glyphLocation, textBounds, checkBoxText, font, flags, focused, state, HWND.Null);

    internal static void DrawCheckBox(
        Graphics g,
        Point glyphLocation,
        Rectangle textBounds,
        string? checkBoxText,
        Font? font,
        TextFormatFlags flags,
        bool focused,
        CheckBoxState state,
        HWND hwnd)
    {
        Rectangle glyphBounds = new(glyphLocation, GetGlyphSize(g, state, hwnd));
        Color textColor;

        if (RenderWithVisualStyles)
        {
            InitializeRenderer((int)state);

            t_visualStyleRenderer.DrawBackground(g, glyphBounds);
            textColor = t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
        }
        else
        {
            if (IsMixed(state))
            {
                ControlPaint.DrawMixedCheckBox(g, glyphBounds, ConvertToButtonState(state));
            }
            else
            {
                ControlPaint.DrawCheckBox(g, glyphBounds, ConvertToButtonState(state));
            }

            textColor = SystemColors.ControlText;
        }

        TextRenderer.DrawText(g, checkBoxText, font, textBounds, textColor, flags);

        if (focused)
        {
            ControlPaint.DrawFocusRectangle(g, textBounds);
        }
    }

    /// <inheritdoc cref="DrawCheckBox(Graphics, Point, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, CheckBoxState)"/>
    public static void DrawCheckBox(
        Graphics g,
        Point glyphLocation,
        Rectangle textBounds,
        string? checkBoxText,
        Font? font,
        Image image,
        Rectangle imageBounds,
        bool focused,
        CheckBoxState state) => DrawCheckBox(
            g,
            glyphLocation,
            textBounds,
            checkBoxText,
            font,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
            image,
            imageBounds,
            focused,
            state);

    /// <summary>
    ///  Renders a CheckBox control.
    /// </summary>
    public static void DrawCheckBox(
        Graphics g,
        Point glyphLocation,
        Rectangle textBounds,
        string? checkBoxText,
        Font? font,
        TextFormatFlags flags,
        Image image,
        Rectangle imageBounds,
        bool focused,
        CheckBoxState state)
    {
        Rectangle glyphBounds = new(glyphLocation, GetGlyphSize(g, state));
        Color textColor;

        if (RenderWithVisualStyles)
        {
            InitializeRenderer((int)state);

            // Keep this drawing order! It matches default drawing order.
            t_visualStyleRenderer.DrawImage(g, imageBounds, image);
            t_visualStyleRenderer.DrawBackground(g, glyphBounds);
            textColor = t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
        }
        else
        {
            g.DrawImage(image, imageBounds);
            if (IsMixed(state))
            {
                ControlPaint.DrawMixedCheckBox(g, glyphBounds, ConvertToButtonState(state));
            }
            else
            {
                ControlPaint.DrawCheckBox(g, glyphBounds, ConvertToButtonState(state));
            }

            textColor = SystemColors.ControlText;
        }

        TextRenderer.DrawText(g, checkBoxText, font, textBounds, textColor, flags);

        if (focused)
        {
            ControlPaint.DrawFocusRectangle(g, textBounds);
        }
    }

    /// <summary>
    ///  Returns the size of the CheckBox glyph.
    /// </summary>
    public static Size GetGlyphSize(Graphics g, CheckBoxState state) => GetGlyphSize((IDeviceContext)g, state);

    internal static Size GetGlyphSize(IDeviceContext deviceContext, CheckBoxState state, HWND hwnd = default)
    {
        if (!RenderWithVisualStyles)
        {
            return new Size(13, 13);
        }

        using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();
        return GetGlyphSize(hdc, state, hwnd);
    }

    internal static Size GetGlyphSize(HDC hdc, CheckBoxState state, HWND hwnd)
    {
        if (RenderWithVisualStyles)
        {
            InitializeRenderer((int)state);

            return t_visualStyleRenderer.GetPartSize(hdc, ThemeSizeType.Draw, hwnd);
        }

        return new Size(13, 13);
    }

    internal static ButtonState ConvertToButtonState(CheckBoxState state) => state switch
    {
        CheckBoxState.CheckedNormal or CheckBoxState.CheckedHot => ButtonState.Checked,
        CheckBoxState.CheckedPressed => (ButtonState.Checked | ButtonState.Pushed),
        CheckBoxState.CheckedDisabled => (ButtonState.Checked | ButtonState.Inactive),
        CheckBoxState.UncheckedPressed => ButtonState.Pushed,
        CheckBoxState.UncheckedDisabled => ButtonState.Inactive,
        // Downlevel mixed drawing works only if ButtonState.Checked is set
        CheckBoxState.MixedNormal or CheckBoxState.MixedHot => ButtonState.Checked,
        CheckBoxState.MixedPressed => (ButtonState.Checked | ButtonState.Pushed),
        CheckBoxState.MixedDisabled => (ButtonState.Checked | ButtonState.Inactive),
        _ => ButtonState.Normal,
    };

    internal static CheckBoxState ConvertFromButtonState(ButtonState state, bool isMixed, bool isHot)
    {
        if (isMixed)
        {
            if ((state & ButtonState.Pushed) == ButtonState.Pushed)
            {
                return CheckBoxState.MixedPressed;
            }
            else if ((state & ButtonState.Inactive) == ButtonState.Inactive)
            {
                return CheckBoxState.MixedDisabled;
            }
            else if (isHot)
            {
                return CheckBoxState.MixedHot;
            }

            return CheckBoxState.MixedNormal;
        }
        else if ((state & ButtonState.Checked) == ButtonState.Checked)
        {
            if ((state & ButtonState.Pushed) == ButtonState.Pushed)
            {
                return CheckBoxState.CheckedPressed;
            }
            else if ((state & ButtonState.Inactive) == ButtonState.Inactive)
            {
                return CheckBoxState.CheckedDisabled;
            }
            else if (isHot)
            {
                return CheckBoxState.CheckedHot;
            }

            return CheckBoxState.CheckedNormal;
        }
        else
        {
            // Unchecked
            if ((state & ButtonState.Pushed) == ButtonState.Pushed)
            {
                return CheckBoxState.UncheckedPressed;
            }
            else if ((state & ButtonState.Inactive) == ButtonState.Inactive)
            {
                return CheckBoxState.UncheckedDisabled;
            }
            else if (isHot)
            {
                return CheckBoxState.UncheckedHot;
            }

            return CheckBoxState.UncheckedNormal;
        }
    }

    private static bool IsMixed(CheckBoxState state) => state switch
    {
        CheckBoxState.MixedNormal
            or CheckBoxState.MixedHot
            or CheckBoxState.MixedPressed
            or CheckBoxState.MixedDisabled => true,
        _ => false,
    };

    private static bool IsDisabled(CheckBoxState state) => state switch
    {
        CheckBoxState.CheckedDisabled or CheckBoxState.UncheckedDisabled or CheckBoxState.MixedDisabled => true,
        _ => false,
    };

    [MemberNotNull(nameof(t_visualStyleRenderer))]
    private static void InitializeRenderer(int state)
    {
        int part = s_checkBoxElement.Part;
        if (SystemInformation.HighContrast
            && IsDisabled((CheckBoxState)state)
            && VisualStyleRenderer.IsCombinationDefined(
                s_checkBoxElement.ClassName,
                VisualStyleElement.Button.CheckBox.HighContrastDisabledPart))
        {
            part = VisualStyleElement.Button.CheckBox.HighContrastDisabledPart;
        }

        if (t_visualStyleRenderer is null)
        {
            t_visualStyleRenderer = new VisualStyleRenderer(s_checkBoxElement.ClassName, part, state);
        }
        else
        {
            t_visualStyleRenderer.SetParameters(s_checkBoxElement.ClassName, part, state);
        }
    }
}
