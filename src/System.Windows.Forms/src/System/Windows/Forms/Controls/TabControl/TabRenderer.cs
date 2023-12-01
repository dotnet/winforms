// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  This is a rendering class for the Tab control.
/// </summary>
public static class TabRenderer
{
    // Make this per-thread, so that different threads can safely use these methods.
    [ThreadStatic]
    private static VisualStyleRenderer? t_visualStyleRenderer;

    /// <summary>
    ///  Returns true if this class is supported for the current OS and user/application settings,
    ///  otherwise returns false.
    /// </summary>
    public static bool IsSupported => VisualStyleRenderer.IsSupported; // no downlevel support

    /// <summary>
    ///  Renders a Tab item.
    /// </summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, TabItemState state)
    {
        InitializeRenderer(VisualStyleElement.Tab.TabItem.Normal, (int)state);

        t_visualStyleRenderer.DrawBackground(g, bounds);
    }

    /// <summary>
    ///  Renders a Tab item.
    /// </summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, bool focused, TabItemState state)
    {
        InitializeRenderer(VisualStyleElement.Tab.TabItem.Normal, (int)state);

        t_visualStyleRenderer.DrawBackground(g, bounds);

        // GetBackgroundContentRectangle() returns same rectangle as bounds for this control!
        Rectangle contentBounds = Rectangle.Inflate(bounds, -3, -3);
        if (focused)
        {
            ControlPaint.DrawFocusRectangle(g, contentBounds);
        }
    }

    /// <summary>
    ///  Renders a Tab item.
    /// </summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, string? tabItemText, Font? font, TabItemState state)
    {
        DrawTabItem(g, bounds, tabItemText, font, false, state);
    }

    /// <summary>
    ///  Renders a Tab item.
    /// </summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, string? tabItemText, Font? font, bool focused, TabItemState state)
    {
        DrawTabItem(g, bounds, tabItemText, font,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
                    focused, state);
    }

    /// <summary>
    ///  Renders a Tab item.
    /// </summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, string? tabItemText, Font? font, TextFormatFlags flags, bool focused, TabItemState state)
    {
        InitializeRenderer(VisualStyleElement.Tab.TabItem.Normal, (int)state);
        t_visualStyleRenderer.DrawBackground(g, bounds);

        // GetBackgroundContentRectangle() returns same rectangle as bounds for this control!
        Rectangle contentBounds = Rectangle.Inflate(bounds, -3, -3);
        Color textColor = t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
        TextRenderer.DrawText(g, tabItemText, font, contentBounds, textColor, flags);

        if (focused)
        {
            ControlPaint.DrawFocusRectangle(g, contentBounds);
        }
    }

    /// <summary>
    ///  Renders a Tab item.
    /// </summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, Image image, Rectangle imageRectangle, bool focused, TabItemState state)
    {
        InitializeRenderer(VisualStyleElement.Tab.TabItem.Normal, (int)state);

        t_visualStyleRenderer.DrawBackground(g, bounds);

        // GetBackgroundContentRectangle() returns same rectangle as bounds for this control!
        Rectangle contentBounds = Rectangle.Inflate(bounds, -3, -3);

        t_visualStyleRenderer.DrawImage(g, imageRectangle, image);

        if (focused)
        {
            ControlPaint.DrawFocusRectangle(g, contentBounds);
        }
    }

    /// <summary>
    ///  Renders a Tab item.
    /// </summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, string? tabItemText, Font? font, Image image, Rectangle imageRectangle, bool focused, TabItemState state)
    {
        DrawTabItem(g, bounds, tabItemText, font,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
                    image, imageRectangle, focused, state);
    }

    /// <summary>
    ///  Renders a Tab item.
    /// </summary>
    public static void DrawTabItem(Graphics g, Rectangle bounds, string? tabItemText, Font? font, TextFormatFlags flags, Image image, Rectangle imageRectangle, bool focused, TabItemState state)
    {
        InitializeRenderer(VisualStyleElement.Tab.TabItem.Normal, (int)state);

        t_visualStyleRenderer.DrawBackground(g, bounds);

        // GetBackgroundContentRectangle() returns same rectangle as bounds for this control!
        Rectangle contentBounds = Rectangle.Inflate(bounds, -3, -3);
        t_visualStyleRenderer.DrawImage(g, imageRectangle, image);
        Color textColor = t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
        TextRenderer.DrawText(g, tabItemText, font, contentBounds, textColor, flags);

        if (focused)
        {
            ControlPaint.DrawFocusRectangle(g, contentBounds);
        }
    }

    /// <summary>
    ///  Renders a TabPage.
    /// </summary>
    public static void DrawTabPage(Graphics g, Rectangle bounds)
        => DrawTabPage((IDeviceContext)g, bounds);

    internal static void DrawTabPage(IDeviceContext deviceContext, Rectangle bounds)
    {
        InitializeRenderer(VisualStyleElement.Tab.Pane.Normal, 0);
        t_visualStyleRenderer.DrawBackground(deviceContext, bounds);
    }

    [MemberNotNull(nameof(t_visualStyleRenderer))]
    private static void InitializeRenderer(VisualStyleElement element, int state)
    {
        if (t_visualStyleRenderer is null)
        {
            t_visualStyleRenderer = new VisualStyleRenderer(element.ClassName, element.Part, state);
        }
        else
        {
            t_visualStyleRenderer.SetParameters(element.ClassName, element.Part, state);
        }
    }
}
