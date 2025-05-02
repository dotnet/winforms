// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods used to render a button control in dark mode.
/// </summary>
internal static partial class ButtonDarkModeRenderer
{
    // Make this per-thread, so that different threads can safely use these methods
    [ThreadStatic]
    private static GraphicsPath? s_cornerPath;

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
        Graphics? graphics = deviceContext.TryGetGraphics(create: true);

        if (graphics is null)
        {
            return;
        }

        DrawButtonBackground(graphics, bounds, state, isDefault: false);
    }

    internal static void DrawButtonForHandle(
        IDeviceContext deviceContext,
        Rectangle bounds,
        bool focused,
        PushButtonState state)
    {
        Graphics? graphics = deviceContext.TryGetGraphics(create: true);
        if (graphics is null)
        {
            return;
        }

        // Calculate the content bounds (inner area of button)
        Rectangle contentBounds = DrawButtonBackground(graphics, bounds, state, isDefault: false);

        // Draw focus rectangle if needed
        if (focused)
        {
            DrawFocusRectangle(graphics, contentBounds, isDefault: false);
        }
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState)"/>
    public static void DrawButton(Graphics g, Rectangle bounds, bool focused, PushButtonState state) =>
        DrawButtonForHandle(g, bounds, focused, state);

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
        // Draw button background first
        Rectangle contentBounds = DrawButtonBackground(g, bounds, state, isDefault: false);

        // Get appropriate text color
        Color textColor = GetTextColor(state, isDefault: false);

        // Draw the text
        TextRenderer.DrawText(g, buttonText, font, contentBounds, textColor, flags);

        // Draw focus indicator if needed
        if (focused)
        {
            DrawFocusRectangle(g, contentBounds, isDefault: false);
        }
    }

    /// <inheritdoc cref="DrawButton(Graphics, Rectangle, string?, Font?, TextFormatFlags, Image, Rectangle, bool, PushButtonState)"/>
    public static void DrawButton(Graphics g, Rectangle bounds, Image image, Rectangle imageBounds, bool focused, PushButtonState state)
    {
        // Draw button background
        Rectangle contentBounds = DrawButtonBackground(g, bounds, state, isDefault: false);

        // Draw the image
        g.DrawImage(image, imageBounds);

        // Draw focus indicator if needed
        if (focused)
        {
            DrawFocusRectangle(g, contentBounds, isDefault: false);
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
        Graphics? graphics = deviceContext.TryGetGraphics(create: true);
        if (graphics is null)
        {
            return;
        }

        // Determine if this is a default button
        bool isDefault = false; // In a real implementation, we'd determine this from the button's properties

        // Draw button background and get content bounds
        Rectangle contentBounds = DrawButtonBackground(graphics, bounds, state, isDefault);

        // Draw the image
        graphics.DrawImage(image, imageBounds);

        // Get appropriate text color based on state
        Color textColor = GetTextColor(state, isDefault);

        // Draw the text
        TextRenderer.DrawText(deviceContext, buttonText, font, contentBounds, textColor, flags);

        // Draw focus indicator if needed
        if (focused)
        {
            DrawFocusRectangle(graphics, contentBounds, isDefault);
        }
    }

    /// <summary>
    /// Draws the button background with proper dark mode styling including rounded corners.
    /// </summary>
    private static Rectangle DrawButtonBackground(Graphics graphics, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        // Save original smoothing mode and set to anti-alias for smooth corners
        SmoothingMode originalMode = graphics.SmoothingMode;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create path for rounded corners
        GraphicsPath path = GetRoundedRectanglePath(bounds, 5);

        // Get appropriate background color based on state
        Color backColor = GetBackgroundColor(state, isDefault);

        // Fill the background
        using (SolidBrush brush = new(backColor))
        {
            graphics.FillPath(brush, path);
        }

        // Draw border if needed
        DrawButtonBorder(graphics, path, state, isDefault);

        // Restore original smoothing mode
        graphics.SmoothingMode = originalMode;

        // Return content bounds (area inside the button for text/image)
        return Rectangle.Inflate(bounds, -6, -6);
    }

    /// <summary>
    /// Gets the text color appropriate for the button state and type.
    /// </summary>
    private static Color GetTextColor(PushButtonState state, bool isDefault)
    {
        if (state == PushButtonState.Disabled)
        {
            return DarkModeButtonColors.DisabledTextColor;
        }

        return isDefault ? DarkModeButtonColors.DefaultTextColor : DarkModeButtonColors.NormalTextColor;
    }

    /// <summary>
    /// Gets the background color appropriate for the button state and type.
    /// </summary>
    private static Color GetBackgroundColor(PushButtonState state, bool isDefault) =>
        isDefault
            ? state switch
            {
                PushButtonState.Normal => DarkModeButtonColors.DefaultBackgroundColor,
                PushButtonState.Hot => DarkModeButtonColors.DefaultHoverBackgroundColor,
                PushButtonState.Pressed => DarkModeButtonColors.DefaultPressedBackgroundColor,
                PushButtonState.Disabled => DarkModeButtonColors.DefaultDisabledBackgroundColor,
                _ => DarkModeButtonColors.DefaultBackgroundColor
            }
            : state switch
            {
                PushButtonState.Normal => DarkModeButtonColors.NormalBackgroundColor,
                PushButtonState.Hot => DarkModeButtonColors.HoverBackgroundColor,
                PushButtonState.Pressed => DarkModeButtonColors.PressedBackgroundColor,
                PushButtonState.Disabled => DarkModeButtonColors.DisabledBackgroundColor,
                _ => DarkModeButtonColors.NormalBackgroundColor
            };

    /// <summary>
    /// Draws the button border based on the current state.
    /// </summary>
    private static void DrawButtonBorder(Graphics graphics, GraphicsPath path, PushButtonState state, bool isDefault)
    {
        // For pressed state, draw a darker inner border
        if (state == PushButtonState.Pressed)
        {
            using Pen borderPen = new(isDefault
                ? Color.FromArgb(80, 0, 0, 0)
                : DarkModeButtonColors.BottomRightBorderColor);

            graphics.DrawPath(borderPen, path);
        }

        // For other states, draw a single-pixel border
        else if (state != PushButtonState.Disabled)
        {
            Color borderColor = isDefault
                ? Color.FromArgb(
                    red: DarkModeButtonColors.DefaultBackgroundColor.R - 20,
                    green: DarkModeButtonColors.DefaultBackgroundColor.G - 10,
                    blue: DarkModeButtonColors.DefaultBackgroundColor.B - 30)
                : DarkModeButtonColors.SingleBorderColor;

            using Pen borderPen = new(borderColor);
            graphics.DrawPath(borderPen, path);
        }
    }

    /// <summary>
    /// Draws a focus rectangle with dotted lines inside the button.
    /// </summary>
    private static void DrawFocusRectangle(Graphics graphics, Rectangle contentBounds, bool isDefault)
    {
        // Create a slightly smaller rectangle for the focus indicator (2px inside)
        Rectangle focusRect = Rectangle.Inflate(contentBounds, -2, -2);

        // Create dotted pen with appropriate color
        Color focusColor = isDefault
            ? DarkModeButtonColors.DefaultFocusIndicatorColor
            : DarkModeButtonColors.FocusIndicatorColor;

        using Pen focusPen = new(focusColor)
        {
            DashStyle = DashStyle.Dot
        };

        // Draw the focus rectangle with rounded corners
        GraphicsPath focusPath = GetRoundedRectanglePath(focusRect, 3);
        graphics.DrawPath(focusPen, focusPath);
    }

    /// <summary>
    /// Creates a GraphicsPath for a rounded rectangle.
    /// </summary>
    private static GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
    {
        if (s_cornerPath is null)
        {
            s_cornerPath = new GraphicsPath();
        }
        else
        {
            s_cornerPath.Reset();
        }

        int diameter = radius * 2;
        Rectangle arcRect = new(bounds.Location, new Size(diameter, diameter));

        // Top left corner
        s_cornerPath.AddArc(arcRect, 180, 90);

        // Top right corner
        arcRect.X = bounds.Right - diameter;
        s_cornerPath.AddArc(arcRect, 270, 90);

        // Bottom right corner
        arcRect.Y = bounds.Bottom - diameter;
        s_cornerPath.AddArc(arcRect, 0, 90);

        // Bottom left corner
        arcRect.X = bounds.Left;
        s_cornerPath.AddArc(arcRect, 90, 90);

        s_cornerPath.CloseFigure();
        return s_cornerPath;
    }

    internal static ButtonState ConvertToButtonState(PushButtonState state) => state switch
    {
        PushButtonState.Pressed => ButtonState.Pushed,
        PushButtonState.Disabled => ButtonState.Inactive,
        _ => ButtonState.Normal,
    };
}
