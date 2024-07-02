// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  This is a rendering class for the GroupBox control.
/// </summary>
public static class GroupBoxRenderer
{
    // Make this per-thread, so that different threads can safely use these methods.
    [ThreadStatic]
    private static VisualStyleRenderer? t_visualStyleRenderer;
    private static readonly VisualStyleElement s_groupBoxElement = VisualStyleElement.Button.GroupBox.Normal;
    private const int TextOffset = 8;
    private const int BoxHeaderWidth = 7;    // The groupbox frame shows 7 pixels before the caption.

    /// <summary>
    ///  If this property is true, then the renderer will use the setting from Application.RenderWithVisualStyles
    ///  to determine how to render.If this property is false, the renderer will always render with visualstyles.
    /// </summary>
    public static bool RenderMatchingApplicationState { get; set; } = true;

    private static bool RenderWithVisualStyles
        => (!RenderMatchingApplicationState || Application.RenderWithVisualStyles);

    /// <summary>
    ///  Returns true if the background corresponding to the given state is partially transparent, else false.
    /// </summary>
    public static bool IsBackgroundPartiallyTransparent(GroupBoxState state)
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

    /// <summary>
    ///  This is just a convenience wrapper for VisualStyleRenderer.DrawThemeParentBackground. For downlevel,
    ///  this isn't required and does nothing.
    /// </summary>
    public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl)
    {
        if (RenderWithVisualStyles)
        {
            InitializeRenderer(0);
            t_visualStyleRenderer.DrawParentBackground(g, bounds, childControl);
        }
    }

    /// <summary>
    ///  Renders a GroupBox control.
    /// </summary>
    public static void DrawGroupBox(Graphics g, Rectangle bounds, GroupBoxState state)
    {
        if (RenderWithVisualStyles)
        {
            DrawThemedGroupBoxNoText(g, bounds, state);
        }
        else
        {
            DrawUnthemedGroupBoxNoText(g, bounds);
        }
    }

    /// <summary>
    ///  Renders a GroupBox control. Uses the text color specified by the theme.
    /// </summary>
    public static void DrawGroupBox(Graphics g, Rectangle bounds, string? groupBoxText, Font? font, GroupBoxState state)
        => DrawGroupBox(g, bounds, groupBoxText, font, TextFormatFlags.Top | TextFormatFlags.Left, state);

    /// <summary>
    ///  Renders a GroupBox control.
    /// </summary>
    public static void DrawGroupBox(
        Graphics g,
        Rectangle bounds,
        string? groupBoxText,
        Font? font,
        Color textColor,
        GroupBoxState state)
        => DrawGroupBox(g, bounds, groupBoxText, font, textColor, TextFormatFlags.Top | TextFormatFlags.Left, state);

    /// <summary>
    ///  Renders a GroupBox control. Uses the text color specified by the theme.
    /// </summary>
    public static void DrawGroupBox(
        Graphics g,
        Rectangle bounds,
        string? groupBoxText,
        Font? font,
        TextFormatFlags flags,
        GroupBoxState state)
        => DrawGroupBox((IDeviceContext)g, bounds, groupBoxText, font, flags, state);

    internal static void DrawGroupBox(
        IDeviceContext deviceContext,
        Rectangle bounds,
        string? groupBoxText,
        Font? font,
        TextFormatFlags flags,
        GroupBoxState state)
    {
        if (RenderWithVisualStyles)
        {
            DrawThemedGroupBoxWithText(deviceContext, bounds, groupBoxText, font, DefaultTextColor(state), flags, state);
        }
        else
        {
            DrawUnthemedGroupBoxWithText(deviceContext, bounds, groupBoxText, font, DefaultTextColor(state), flags);
        }
    }

    /// <summary>
    ///  Renders a GroupBox control.
    /// </summary>
    public static void DrawGroupBox(
        Graphics g,
        Rectangle bounds,
        string? groupBoxText,
        Font? font,
        Color textColor,
        TextFormatFlags flags,
        GroupBoxState state)
        => DrawGroupBox((IDeviceContext)g, bounds, groupBoxText, font, textColor, flags, state);

    internal static void DrawGroupBox(
        IDeviceContext deviceContext,
        Rectangle bounds,
        string? groupBoxText,
        Font? font,
        Color textColor,
        TextFormatFlags flags,
        GroupBoxState state)
    {
        if (RenderWithVisualStyles)
        {
            DrawThemedGroupBoxWithText(deviceContext, bounds, groupBoxText, font, textColor, flags, state);
        }
        else
        {
            DrawUnthemedGroupBoxWithText(deviceContext, bounds, groupBoxText, font, textColor, flags);
        }
    }

    /// <summary>
    ///  Draws a themed GroupBox with no text label.
    /// </summary>
    private static void DrawThemedGroupBoxNoText(Graphics g, Rectangle bounds, GroupBoxState state)
    {
        InitializeRenderer((int)state);
        t_visualStyleRenderer.DrawBackground(g, bounds);
    }

    /// <summary>
    ///  Draws a themed GroupBox with a text label.
    /// </summary>
    private static void DrawThemedGroupBoxWithText(
        IDeviceContext deviceContext,
        Rectangle bounds,
        string? groupBoxText,
        Font? font,
        Color textColor,
        TextFormatFlags flags,
        GroupBoxState state)
    {
        InitializeRenderer((int)state);

        // Calculate text area, and render text inside it
        Rectangle textBounds = bounds;

        textBounds.Width -= 2 * BoxHeaderWidth;
        Size measuredBounds = TextRenderer.MeasureText(
            deviceContext,
            groupBoxText,
            font,
            new Size(textBounds.Width, textBounds.Height),
            flags);

        textBounds.Width = measuredBounds.Width;
        textBounds.Height = measuredBounds.Height;

        if ((flags & TextFormatFlags.Right) == TextFormatFlags.Right)
        {
            // +1 to account for the margin built in the MeasureText result
            textBounds.X = bounds.Right - textBounds.Width - BoxHeaderWidth + 1;
        }
        else
        {
            // -1 to account for the margin built in the MeasureText result
            textBounds.X += BoxHeaderWidth - 1;
        }

        TextRenderer.DrawText(deviceContext, groupBoxText, font, textBounds, textColor, flags);

        // Calculate area for background box
        Rectangle boxBounds = bounds;
        if (font is not null)
        {
            boxBounds.Y += font.Height / 2;
            boxBounds.Height -= font.Height / 2;
        }

        // Break box into three segments, that don't overlap the text area
        Rectangle clipLeft = boxBounds;
        Rectangle clipMiddle = boxBounds;
        Rectangle clipRight = boxBounds;

        clipLeft.Width = BoxHeaderWidth;
        clipMiddle.Width = Math.Max(0, textBounds.Width - 3);  // -3 to account for the margin built in the MeasureText result
        if ((flags & TextFormatFlags.Right) == TextFormatFlags.Right)
        {
            clipLeft.X = boxBounds.Right - BoxHeaderWidth;
            clipMiddle.X = clipLeft.Left - clipMiddle.Width;
            clipRight.Width = clipMiddle.X - boxBounds.X;
        }
        else
        {
            clipMiddle.X = clipLeft.Right;
            clipRight.X = clipMiddle.Right;
            clipRight.Width = boxBounds.Right - clipRight.X;
        }

        clipMiddle.Y = textBounds.Bottom;
        clipMiddle.Height -= (textBounds.Bottom - boxBounds.Top);

        Debug.Assert(textBounds.Y <= boxBounds.Y, "if text below box, need to render area of box above text");

        // Render clipped portion of background in each segment
        t_visualStyleRenderer.DrawBackground(deviceContext, boxBounds, clipLeft);
        t_visualStyleRenderer.DrawBackground(deviceContext, boxBounds, clipMiddle);
        t_visualStyleRenderer.DrawBackground(deviceContext, boxBounds, clipRight);
    }

    /// <summary>
    ///  Draws an un-themed GroupBox with no text label.
    /// </summary>
    private static void DrawUnthemedGroupBoxNoText(Graphics g, Rectangle bounds)
    {
        Color backColor = Application.ApplicationColors.Control;
        using var light = ControlPaint.Light(backColor, 1.0f).GetCachedPenScope();
        using var dark = ControlPaint.Dark(backColor, 0f).GetCachedPenScope();

        // left
        g.DrawLine(light, bounds.Left + 1, bounds.Top + 1, bounds.Left + 1, bounds.Height - 1);
        g.DrawLine(dark, bounds.Left, bounds.Top + 1, bounds.Left, bounds.Height - 2);

        // bottom
        g.DrawLine(light, bounds.Left, bounds.Height - 1, bounds.Width - 1, bounds.Height - 1);
        g.DrawLine(dark, bounds.Left, bounds.Height - 2, bounds.Width - 1, bounds.Height - 2);

        // top
        g.DrawLine(light, bounds.Left + 1, bounds.Top + 1, bounds.Width - 1, bounds.Top + 1);
        g.DrawLine(dark, bounds.Left, bounds.Top, bounds.Width - 2, bounds.Top);

        // right
        g.DrawLine(light, bounds.Width - 1, bounds.Top, bounds.Width - 1, bounds.Height - 1);
        g.DrawLine(dark, bounds.Width - 2, bounds.Top, bounds.Width - 2, bounds.Height - 2);
    }

    /// <summary>
    ///  Draws an un-themed GroupBox with a text label. Variation of the logic in GroupBox.DrawGroupBox().
    /// </summary>
    private static void DrawUnthemedGroupBoxWithText(
        IDeviceContext deviceContext,
        Rectangle bounds,
        string? groupBoxText,
        Font? font,
        Color textColor,
        TextFormatFlags flags)
    {
        // Calculate text area, and render text inside it
        Rectangle textBounds = bounds;

        textBounds.Width -= TextOffset;
        Size measuredBounds = TextRenderer.MeasureText(
            deviceContext,
            groupBoxText,
            font,
            new Size(textBounds.Width, textBounds.Height),
            flags);

        textBounds.Width = measuredBounds.Width;
        textBounds.Height = measuredBounds.Height;

        if ((flags & TextFormatFlags.Right) == TextFormatFlags.Right)
        {
            textBounds.X = bounds.Right - textBounds.Width - TextOffset;
        }
        else
        {
            textBounds.X += TextOffset;
        }

        TextRenderer.DrawText(deviceContext, groupBoxText, font, textBounds, textColor, flags);

        // Pad text area to stop background from touching text
        if (textBounds.Width > 0)
        {
            textBounds.Inflate(2, 0);
        }

        int boxTop = bounds.Top;
        if (font is not null)
        {
            boxTop += font.Height / 2;
        }

        using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();

        ReadOnlySpan<int> darkLines =
        [
            bounds.Left, boxTop - 1, bounds.Left, bounds.Height - 2,                            // Left
            bounds.Left, bounds.Height - 2, bounds.Width - 1, bounds.Height - 2,                // Right
            bounds.Left, boxTop - 1, textBounds.X - 3, boxTop - 1,                              // Top-left
            textBounds.X + textBounds.Width + 2, boxTop - 1, bounds.Width - 2, boxTop - 1,      // Top-right
            bounds.Width - 2, boxTop - 1, bounds.Width - 2, bounds.Height - 2                   // Right
        ];

        using CreatePenScope hpenDark = new(SystemColors.ControlDark);
        hdc.DrawLines(hpenDark, darkLines);

        ReadOnlySpan<int> lightLines =
        [
            bounds.Left + 1, boxTop, bounds.Left + 1, bounds.Height - 1,                        // Left
            bounds.Left, bounds.Height - 1, bounds.Width, bounds.Height - 1,                    // Right
            bounds.Left + 1, boxTop, textBounds.X - 2, boxTop,                                  // Top-left
            textBounds.X + textBounds.Width + 1, boxTop, bounds.Width - 1, boxTop,              // Top-right
            bounds.Width - 1, boxTop, bounds.Width - 1, bounds.Height - 1                       // Right
        ];

        using CreatePenScope hpenLight = new(SystemColors.ControlLight);
        hdc.DrawLines(hpenLight, lightLines);
    }

    private static Color DefaultTextColor(GroupBoxState state)
    {
        if (RenderWithVisualStyles)
        {
            InitializeRenderer((int)state);
            return t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
        }
        else
        {
            return Application.ApplicationColors.ControlText;
        }
    }

    [MemberNotNull(nameof(t_visualStyleRenderer))]
    private static void InitializeRenderer(int state)
    {
        int part = s_groupBoxElement.Part;
        if (SystemInformation.HighContrast
            && ((GroupBoxState)state == GroupBoxState.Disabled)
            && VisualStyleRenderer.IsCombinationDefined(
                s_groupBoxElement.ClassName,
                VisualStyleElement.Button.GroupBox.HighContrastDisabledPart))
        {
            part = VisualStyleElement.Button.GroupBox.HighContrastDisabledPart;
        }

        if (t_visualStyleRenderer is null)
        {
            t_visualStyleRenderer = new VisualStyleRenderer(s_groupBoxElement.ClassName, part, state);
        }
        else
        {
            t_visualStyleRenderer.SetParameters(s_groupBoxElement.ClassName, part, state);
        }
    }
}
