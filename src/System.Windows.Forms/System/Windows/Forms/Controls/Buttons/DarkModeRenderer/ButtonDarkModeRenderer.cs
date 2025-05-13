// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods used to render a button control in dark mode.
/// </summary>
internal static partial class ButtonDarkModeRenderer
{
    private const FlatStyle FlatSyleStandard = FlatStyle.Standard;

    // Thread-static instances of each renderer type
    [ThreadStatic]
    private static IButtonDarkModeRenderer? s_standardRenderer;

    [ThreadStatic]
    private static IButtonDarkModeRenderer? s_flatRenderer;

    [ThreadStatic]
    private static IButtonDarkModeRenderer? s_popupRenderer;

    [ThreadStatic]
    private static IButtonDarkModeRenderer? s_systemRenderer;

    // Default FlatStyle if not specified
    private const FlatStyle DefaultFlatStyle = FlatSyleStandard;

    /// <summary>
    /// Gets the standard style renderer instance.
    /// </summary>
    internal static IButtonDarkModeRenderer StandardRenderer => s_standardRenderer ??= new StandardButtonDarkModeRenderer();

    /// <summary>
    /// Gets the flat style renderer instance.
    /// </summary>
    internal static IButtonDarkModeRenderer FlatRenderer => s_flatRenderer ??= new FlatButtonDarkModeRenderer();

    /// <summary>
    /// Gets the popup style renderer instance.
    /// </summary>
    internal static IButtonDarkModeRenderer PopupRenderer => s_popupRenderer ??= new PopupButtonDarkModeRenderer();

    /// <summary>
    /// Gets the system style renderer instance.
    /// </summary>
    internal static IButtonDarkModeRenderer SystemRenderer => s_systemRenderer ??= new SystemButtonDarkModeRenderer();

    /// <summary>
    ///  Gets or sets a value indicating whether the renderer uses the application state to determine rendering style.
    /// </summary>
    public static bool RenderMatchingApplicationState { get; set; } = true;

    /// <summary>
    ///  Indicates whether the background has semitransparent or alpha-blended pieces.
    /// </summary>
    public static bool IsBackgroundPartiallyTransparent()
    {
        // Dark mode always has partially transparent background due to rounded corners
        return true;
    }

    /// <summary>
    ///  Draws the background of a control's parent in the specified area.
    /// </summary>
    public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl)
        => DrawParentBackground((IDeviceContext)g, bounds, childControl);

    internal static void DrawParentBackground(IDeviceContext dc, Rectangle bounds, Control childControl)
    {
        Graphics? graphics = dc.TryGetGraphics(create: true);
        if (graphics is null)
        {
            return;
        }

        Control? parent = childControl.Parent;
        if (parent is null)
        {
            return;
        }

        // Create a temporary graphics context with the proper transformation
        using Bitmap tempBitmap = new(bounds.Width, bounds.Height);
        using Graphics tempGraphics = Graphics.FromImage(tempBitmap);

        // Calculate the portion of parent we need to render
        Point pt = parent.PointToClient(childControl.PointToScreen(Point.Empty));
        tempGraphics.TranslateTransform(-pt.X, -pt.Y);

        // Call internal PaintBackground directly with the rectangle
        Rectangle parentRect = new(pt, bounds.Size);
        using PaintEventArgs e = new(tempGraphics, parentRect);

        // This is the important part - we need to pass the rectangle
        parent.PaintBackground(e, parentRect);

        // Now draw the captured background to our target graphics
        graphics.DrawImage(tempBitmap, bounds);
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState)"/>
    public static void DrawButton(Graphics g, Rectangle bounds, PushButtonState state) =>
        DrawButton((IDeviceContext)g, bounds, state);

    internal static void DrawButton(IDeviceContext deviceContext, Rectangle bounds, PushButtonState state)
    {
        DrawButton(deviceContext, bounds, state, DefaultFlatStyle);
    }

    internal static void DrawButton(IDeviceContext deviceContext, Rectangle bounds, PushButtonState state, FlatStyle flatStyle)
    {
        Graphics? graphics = deviceContext.TryGetGraphics(create: true);

        if (graphics is null)
        {
            return;
        }

        IButtonDarkModeRenderer renderer = ButtonDarkModeRendererFactory.GetRenderer(flatStyle);
        renderer.DrawButtonBackground(graphics, bounds, state, isDefault: false);
    }

    internal static void DrawButtonForHandle(
        IDeviceContext deviceContext,
        Rectangle bounds,
        bool focused,
        PushButtonState state)
    {
        DrawButtonForHandle(deviceContext, bounds, focused, state, DefaultFlatStyle);
    }

    internal static void DrawButtonForHandle(
        IDeviceContext deviceContext,
        Rectangle bounds,
        bool focused,
        PushButtonState state,
        FlatStyle flatStyle)
    {
        Graphics? graphics = deviceContext.TryGetGraphics(create: true);
        if (graphics is null)
        {
            return;
        }

        // Get the appropriate renderer
        IButtonDarkModeRenderer renderer = ButtonDarkModeRendererFactory.GetRenderer(flatStyle);

        // Calculate the content bounds (inner area of button)
        Rectangle contentBounds = renderer.DrawButtonBackground(graphics, bounds, state, isDefault: false);

        // Draw focus indicator if needed
        if (focused)
        {
            renderer.DrawFocusIndicator(graphics, contentBounds, isDefault: false);
        }
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState)"/>
    public static void DrawButton(Graphics g, Rectangle bounds, bool focused, PushButtonState state) =>
        DrawButtonForHandle(g, bounds, focused, state);

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState, FlatStyle)"/>
    public static void DrawButton(Graphics g, Rectangle bounds, bool focused, PushButtonState state, FlatStyle flatStyle) =>
        DrawButtonForHandle(g, bounds, focused, state, flatStyle);

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

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState, FlatStyle)"/>
    public static void DrawButton(
        Graphics g,
        Rectangle bounds,
        string? buttonText,
        Font? font,
        bool focused,
        PushButtonState state,
        FlatStyle flatStyle)
    {
        DrawButton(
            g,
            bounds,
            buttonText,
            font,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
            focused,
            state,
            flatStyle);
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState)"/>
    public static void DrawButton(Graphics g, Rectangle bounds, string? buttonText, Font? font, TextFormatFlags flags, bool focused, PushButtonState state)
    {
        DrawButton(g, bounds, buttonText, font, flags, focused, state, DefaultFlatStyle);
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState, FlatStyle)"/>
    public static void DrawButton(
        Graphics g,
        Rectangle bounds,
        string? buttonText,
        Font? font,
        TextFormatFlags flags,
        bool focused,
        PushButtonState state,
        FlatStyle flatStyle)
    {
        // Get the appropriate renderer
        IButtonDarkModeRenderer renderer = ButtonDarkModeRendererFactory.GetRenderer(flatStyle);

        // Draw button background first
        Rectangle contentBounds = renderer.DrawButtonBackground(g, bounds, state, isDefault: false);

        // Get appropriate text color
        Color textColor = renderer.GetTextColor(state, isDefault: false);

        // Draw the text
        TextRenderer.DrawText(g, buttonText, font, contentBounds, textColor, flags);

        // Draw focus indicator if needed
        if (focused)
        {
            renderer.DrawFocusIndicator(g, contentBounds, isDefault: false);
        }
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState)"/>
    public static void DrawButton(Graphics g, Rectangle bounds, Image image, Rectangle imageBounds, bool focused, PushButtonState state)
    {
        DrawButton(g, bounds, image, imageBounds, focused, state, DefaultFlatStyle);
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState, FlatStyle)"/>
    public static void DrawButton(
        Graphics g,
        Rectangle bounds,
        Image image,
        Rectangle imageBounds,
        bool focused,
        PushButtonState state,
        FlatStyle flatStyle)
    {
        // Get the appropriate renderer
        IButtonDarkModeRenderer renderer = ButtonDarkModeRendererFactory.GetRenderer(flatStyle);

        // Draw button background
        Rectangle contentBounds = renderer.DrawButtonBackground(g, bounds, state, isDefault: false);

        // Draw the image
        g.DrawImage(image, imageBounds);

        // Draw focus indicator if needed
        if (focused)
        {
            renderer.DrawFocusIndicator(g, contentBounds, isDefault: false);
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

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState, FlatStyle)"/>
    public static void DrawButton(
        Graphics g,
        Rectangle bounds,
        string? buttonText,
        Font? font,
        Image image,
        Rectangle imageBounds,
        bool focused,
        PushButtonState state,
        FlatStyle flatStyle) => DrawButton(
            g,
            bounds,
            buttonText,
            font,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
            image,
            imageBounds,
            focused,
            state,
            flatStyle);

    /// <summary>
    ///  Draws a button control in dark mode style.
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
        => DrawButton((IDeviceContext)g, bounds, buttonText, font, flags, image, imageBounds, focused, state, DefaultFlatStyle);

    /// <summary>
    ///  Draws a button control in dark mode style with the specified FlatStyle.
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
        PushButtonState state,
        FlatStyle flatStyle)
        => DrawButton((IDeviceContext)g, bounds, buttonText, font, flags, image, imageBounds, focused, state, flatStyle);

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
        DrawButton(deviceContext, bounds, buttonText, font, flags, image, imageBounds, focused, state, DefaultFlatStyle);
    }

    internal static void DrawButton(
        IDeviceContext deviceContext,
        Rectangle bounds,
        string? buttonText,
        Font? font,
        TextFormatFlags flags,
        Image image,
        Rectangle imageBounds,
        bool focused,
        PushButtonState state,
        FlatStyle flatStyle)
    {
        Graphics? graphics = deviceContext.TryGetGraphics(create: true);
        if (graphics is null)
        {
            return;
        }

        // Determine if this is a default button
        bool isDefault = false; // In a real implementation, we'd determine this from the button's properties

        // Get the appropriate renderer
        IButtonDarkModeRenderer renderer = ButtonDarkModeRendererFactory.GetRenderer(flatStyle);

        // Draw button background and get content bounds
        Rectangle contentBounds = renderer.DrawButtonBackground(graphics, bounds, state, isDefault);

        // Draw the image
        graphics.DrawImage(image, imageBounds);

        // Get appropriate text color based on state
        Color textColor = renderer.GetTextColor(state, isDefault);

        // Draw the text
        TextRenderer.DrawText(deviceContext, buttonText, font, contentBounds, textColor, flags);

        // Draw focus indicator if needed
        if (focused)
        {
            renderer.DrawFocusIndicator(graphics, contentBounds, isDefault);
        }
    }

    internal static ButtonState ConvertToButtonState(PushButtonState state) => state switch
    {
        PushButtonState.Pressed => ButtonState.Pushed,
        PushButtonState.Disabled => ButtonState.Inactive,
        _ => ButtonState.Normal,
    };
}
