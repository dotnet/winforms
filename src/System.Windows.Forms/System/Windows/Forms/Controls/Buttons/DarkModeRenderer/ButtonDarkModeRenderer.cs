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
    // Singleton instances for each renderer type
    internal static IButtonDarkModeRenderer StandardRenderer { get; } = new StandardButtonDarkModeRenderer();
    internal static IButtonDarkModeRenderer FlatRenderer { get; } = new FlatButtonDarkModeRenderer();
    internal static IButtonDarkModeRenderer PopupRenderer { get; } = new PopupButtonDarkModeRenderer();
    internal static IButtonDarkModeRenderer SystemRenderer { get; } = new SystemButtonDarkModeRenderer();

    /// <summary>
    ///  Gets or sets the amount by which the button bounds are deflated before rendering the button.
    ///  This value is applied to all sides of the button.
    /// </summary>
    public static Padding ButtonDeflation { get; set; } = new(2);

    /// <summary>
    ///  Returns the bounds to use for the button background, after applying deflation.
    /// </summary>
    private static Rectangle GetDeflatedBounds(Rectangle bounds)
    {
        return Rectangle.FromLTRB(
            bounds.Left + ButtonDeflation.Left,
            bounds.Top + ButtonDeflation.Top,
            bounds.Right - ButtonDeflation.Right,
            bounds.Bottom - ButtonDeflation.Bottom);
    }

    /// <summary>
    ///  Draws the background of a control's parent in the specified area, adjusted for button deflation.
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

        // Adjust bounds for deflation
        Rectangle deflatedBounds = GetDeflatedBounds(bounds);

        // Create a temporary graphics context with the proper transformation
        using Bitmap tempBitmap = new(deflatedBounds.Width, deflatedBounds.Height);
        using Graphics tempGraphics = Graphics.FromImage(tempBitmap);

        // Calculate the portion of parent we need to render
        Point pt = parent.PointToClient(childControl.PointToScreen(Point.Empty));
        tempGraphics.TranslateTransform(-pt.X - ButtonDeflation.Left, -pt.Y - ButtonDeflation.Top);

        // Call internal PaintBackground directly with the rectangle
        Rectangle parentRect = new(pt.X + ButtonDeflation.Left, pt.Y + ButtonDeflation.Top, deflatedBounds.Width, deflatedBounds.Height);
        using PaintEventArgs e = new(tempGraphics, parentRect);

        parent.PaintBackground(e, parentRect);

        // Now draw the captured background to our target graphics
        graphics.DrawImage(tempBitmap, deflatedBounds);
    }

    // Update all DrawButton methods to use GetDeflatedBounds for the button area
    internal static void DrawButton(IDeviceContext deviceContext, Rectangle bounds, PushButtonState state, FlatStyle flatStyle)
    {
        Graphics? graphics = deviceContext.TryGetGraphics(create: true);

        if (graphics is null)
        {
            return;
        }

        Rectangle deflatedBounds = GetDeflatedBounds(bounds);

        IButtonDarkModeRenderer renderer = ButtonDarkModeRendererFactory.GetRenderer(flatStyle);
        renderer.DrawButtonBackground(graphics, deflatedBounds, state, isDefault: false);
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

        Rectangle deflatedBounds = GetDeflatedBounds(bounds);

        // Get the appropriate renderer
        IButtonDarkModeRenderer renderer = ButtonDarkModeRendererFactory.GetRenderer(flatStyle);

        // Calculate the content bounds (inner area of button)
        Rectangle contentBounds = renderer.DrawButtonBackground(graphics, deflatedBounds, state, isDefault: false);

        // Draw focus indicator if needed
        if (focused)
        {
            renderer.DrawFocusIndicator(graphics, contentBounds, isDefault: false);
        }
    }

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

        Rectangle deflatedBounds = GetDeflatedBounds(bounds);

        // Draw button background first
        Rectangle contentBounds = renderer.DrawButtonBackground(g, deflatedBounds, state, isDefault: false);

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

        Rectangle deflatedBounds = GetDeflatedBounds(bounds);

        // Draw button background
        Rectangle contentBounds = renderer.DrawButtonBackground(g, deflatedBounds, state, isDefault: false);

        // Draw the image
        g.DrawImage(image, imageBounds);

        // Draw focus indicator if needed
        if (focused)
        {
            renderer.DrawFocusIndicator(g, contentBounds, isDefault: false);
        }
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

        Rectangle deflatedBounds = GetDeflatedBounds(bounds);

        // Determine if this is a default button
        bool isDefault = false; // In a real implementation, we'd determine this from the button's properties

        // Get the appropriate renderer
        IButtonDarkModeRenderer renderer = ButtonDarkModeRendererFactory.GetRenderer(flatStyle);

        // Draw button background and get content bounds
        Rectangle contentBounds = renderer.DrawButtonBackground(graphics, deflatedBounds, state, isDefault);

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
}
