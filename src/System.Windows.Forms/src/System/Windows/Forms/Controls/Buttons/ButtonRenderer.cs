// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods used to render a button control with or without visual styles.
/// </summary>
public static class ButtonRenderer
{
    // Make this per-thread, so that different threads can safely use these methods.
    [ThreadStatic]
    private static VisualStyleRenderer? t_visualStyleRenderer;
    private static readonly VisualStyleElement s_buttonElement = VisualStyleElement.Button.PushButton.Normal;

    /// <summary>
    ///  Gets or sets a value indicating whether the renderer uses the application state to determine rendering style.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   If the <see cref="RenderMatchingApplicationState"/> property is <see langword="true"/>, the renderer
    ///   uses the setting from the <see cref="Application.RenderWithVisualStyles"/> property to determine the
    ///   rendering style. If <see cref="RenderMatchingApplicationState"/> property is <see langword="false"/>,
    ///   the renderer will always render using visual styles.
    ///  </para>
    /// </remarks>
    public static bool RenderMatchingApplicationState { get; set; } = true;

    private static bool RenderWithVisualStyles => !RenderMatchingApplicationState || Application.RenderWithVisualStyles;

    /// <summary>
    ///  Indicates whether the background has semitransparent or alpha-blended pieces.
    /// </summary>
    public static bool IsBackgroundPartiallyTransparent(PushButtonState state)
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
    ///  Draws the background of a control's parent in the specified area.
    /// </summary>
    public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl)
        => DrawParentBackground((IDeviceContext)g, bounds, childControl);

    internal static void DrawParentBackground(IDeviceContext dc, Rectangle bounds, Control childControl)
    {
        if (RenderWithVisualStyles)
        {
            InitializeRenderer(0);
            t_visualStyleRenderer.DrawParentBackground(dc, bounds, childControl);
        }
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState)"/>
    public static void DrawButton(Graphics g, Rectangle bounds, PushButtonState state) =>
        DrawButton((IDeviceContext)g, bounds, state);

    internal static void DrawButton(IDeviceContext deviceContext, Rectangle bounds, PushButtonState state)
    {
        if (RenderWithVisualStyles)
        {
            InitializeRenderer((int)state);
            t_visualStyleRenderer.DrawBackground(deviceContext, bounds);
        }
        else
        {
            Graphics? graphics = deviceContext.TryGetGraphics(create: true);
            if (graphics is not null)
            {
                ControlPaint.DrawButton(graphics, bounds, ConvertToButtonState(state));
            }
        }
    }

    internal static void DrawButtonForHandle(
        IDeviceContext deviceContext,
        Rectangle bounds,
        bool focused,
        PushButtonState state,
        HWND hwnd)
    {
        Rectangle contentBounds;

        if (RenderWithVisualStyles)
        {
            InitializeRenderer((int)state);

            using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();
            t_visualStyleRenderer.DrawBackground(hdc, bounds, hwnd);
            contentBounds = t_visualStyleRenderer.GetBackgroundContentRectangle(hdc, bounds);
        }
        else
        {
            Graphics? graphics = deviceContext.TryGetGraphics(create: true);
            if (graphics is not null)
            {
                ControlPaint.DrawButton(graphics, bounds, ConvertToButtonState(state));
            }

            contentBounds = Rectangle.Inflate(bounds, -3, -3);
        }

        if (focused)
        {
            Graphics? graphics = deviceContext.TryGetGraphics(create: true);
            if (graphics is not null)
            {
                ControlPaint.DrawFocusRectangle(graphics, contentBounds);
            }
        }
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState)"/>
    public static void DrawButton(Graphics g, Rectangle bounds, bool focused, PushButtonState state) =>
        DrawButtonForHandle(g, bounds, focused, state, HWND.Null);

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState)"/>
    public static void DrawButton(Graphics g, Rectangle bounds, string? buttonText, Font? font, bool focused, PushButtonState state)
    {
        DrawButton(
            g,
            bounds,
            buttonText,
            font,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
            focused,
            state);
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState)"/>
    public static void DrawButton(Graphics g, Rectangle bounds, string? buttonText, Font? font, TextFormatFlags flags, bool focused, PushButtonState state)
    {
        Rectangle contentBounds;
        Color textColor;

        if (RenderWithVisualStyles)
        {
            InitializeRenderer((int)state);

            t_visualStyleRenderer.DrawBackground(g, bounds);
            contentBounds = t_visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
            textColor = t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
        }
        else
        {
            ControlPaint.DrawButton(g, bounds, ConvertToButtonState(state));
            contentBounds = Rectangle.Inflate(bounds, -3, -3);
            textColor = Application.ApplicationColors.ControlText;
        }

        TextRenderer.DrawText(g, buttonText, font, contentBounds, textColor, flags);

        if (focused)
        {
            ControlPaint.DrawFocusRectangle(g, contentBounds);
        }
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState)"/>

    public static void DrawButton(Graphics g, Rectangle bounds, Image image, Rectangle imageBounds, bool focused, PushButtonState state)
    {
        Rectangle contentBounds;

        if (RenderWithVisualStyles)
        {
            InitializeRenderer((int)state);

            t_visualStyleRenderer.DrawBackground(g, bounds);
            t_visualStyleRenderer.DrawImage(g, imageBounds, image);
            contentBounds = t_visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
        }
        else
        {
            ControlPaint.DrawButton(g, bounds, ConvertToButtonState(state));
            g.DrawImage(image, imageBounds);
            contentBounds = Rectangle.Inflate(bounds, -3, -3);
        }

        if (focused)
        {
            ControlPaint.DrawFocusRectangle(g, contentBounds);
        }
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState)"/>
    public static void DrawButton(
        Graphics g,
        Rectangle bounds,
        string? buttonText,
        Font? font,
        Image image,
        Rectangle imageBounds,
        bool focused,
        PushButtonState state) => DrawButton(
            g,
            bounds,
            buttonText,
            font,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
            image,
            imageBounds,
            focused,
            state);

    /// <summary>
    ///  Draws a button control.
    /// </summary>
    public static void DrawButton(
        Graphics g,
        Rectangle bounds,
        string? buttonText,
        Font? font,
        TextFormatFlags flags,
        Image image,
        Rectangle imageBounds,
        bool focused,
        PushButtonState state)
        => DrawButton((IDeviceContext)g, bounds, buttonText, font, flags, image, imageBounds, focused, state);

    internal static void DrawButton(
        IDeviceContext deviceContext,
        Rectangle bounds,
        string? buttonText,
        Font? font,
        TextFormatFlags flags,
        Image image,
        Rectangle imageBounds,
        bool focused,
        PushButtonState state)
    {
        Rectangle contentBounds;
        Color textColor;

        Graphics? graphics = deviceContext.TryGetGraphics(create: true);

        if (RenderWithVisualStyles)
        {
            InitializeRenderer((int)state);

            t_visualStyleRenderer.DrawBackground(deviceContext, bounds);
            if (graphics is not null)
            {
                t_visualStyleRenderer.DrawImage(graphics, imageBounds, image);
            }

            contentBounds = t_visualStyleRenderer.GetBackgroundContentRectangle(deviceContext, bounds);
            textColor = t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
        }
        else
        {
            if (graphics is not null)
            {
                ControlPaint.DrawButton(graphics, bounds, ConvertToButtonState(state));
                graphics.DrawImage(image, imageBounds);
            }

            contentBounds = Rectangle.Inflate(bounds, -3, -3);
            textColor = Application.ApplicationColors.ControlText;
        }

        TextRenderer.DrawText(deviceContext, buttonText, font, contentBounds, textColor, flags);

        if (focused && graphics is not null)
        {
            ControlPaint.DrawFocusRectangle(graphics, contentBounds);
        }
    }

    internal static ButtonState ConvertToButtonState(PushButtonState state) => state switch
    {
        PushButtonState.Pressed => ButtonState.Pushed,
        PushButtonState.Disabled => ButtonState.Inactive,
        _ => ButtonState.Normal,
    };

    [MemberNotNull(nameof(t_visualStyleRenderer))]
    private static void InitializeRenderer(int state)
    {
        if (t_visualStyleRenderer is null)
        {
            t_visualStyleRenderer = new VisualStyleRenderer(s_buttonElement.ClassName, s_buttonElement.Part, state);
        }
        else
        {
            t_visualStyleRenderer.SetParameters(s_buttonElement.ClassName, s_buttonElement.Part, state);
        }
    }
}
