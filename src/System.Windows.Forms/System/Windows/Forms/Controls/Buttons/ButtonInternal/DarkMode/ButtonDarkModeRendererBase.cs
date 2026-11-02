// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods used to render a button control in dark mode.
/// </summary>
internal abstract partial class ButtonDarkModeRendererBase : IButtonRenderer
{
    // Define padding values for each renderer type
    private protected abstract Padding PaddingCore { get; }

    private protected virtual bool UseModernStateDefaults => false;

    /// <summary>
    ///  Gets the padding that insets the button body from the control bounds for the given focus state.
    /// </summary>
    /// <param name="focusRingVisible">
    ///  <see langword="true"/> when the focus ring will be drawn for this paint; otherwise <see langword="false"/>.
    /// </param>
    /// <remarks>
    ///  <para>
    ///   By default this returns <see cref="PaddingCore"/> regardless of focus. Renderers that reserve room for
    ///   an outer focus ring can override this to return a smaller padding when the ring is not drawn, letting
    ///   the button body grow into the space the ring and its gap would otherwise occupy.
    ///  </para>
    /// </remarks>
    private protected virtual Padding GetContentPadding(bool focusRingVisible) => PaddingCore;

    internal virtual Padding GetPreferredSizePadding() => PaddingCore;

    /// <summary>
    ///  The device DPI of the control being rendered. Set by the adapter before each paint so renderers
    ///  can DPI-scale their logical (96-DPI) constants. Defaults to 96 (100%).
    /// </summary>
    internal int DeviceDpi { get; set; } = 96;

    /// <summary>
    ///  Gets or sets the appearance settings for the button being rendered.
    /// </summary>
    internal FlatButtonAppearance? FlatAppearance { get; set; }

    /// <summary>
    ///  Scales a logical (96-DPI) value to the current <see cref="DeviceDpi"/>.
    /// </summary>
    private protected int Scale(int logicalValue) => (int)Math.Round(logicalValue * (DeviceDpi / 96.0));

    private protected int ScaleBorderThickness(int scaledThickness)
    {
        int borderSize = FlatAppearance?.BorderSize ?? 1;
        if (borderSize == 0)
        {
            return 0;
        }

        float dpiScale = Math.Max(1f, DeviceDpi / 96f);
        float factor = 1f + ((borderSize - 1) / dpiScale);

        return Math.Max(1, (int)Math.Round(scaledThickness * factor));
    }

    private protected Color ResolveBorderColor(Color designColor)
        => FlatAppearance is { BorderColor.IsEmpty: false } appearance
            ? appearance.BorderColor
            : designColor;

    private protected static Color DeriveHoverColor(Color baseColor)
        => baseColor.GetBrightness() < 0.5f
            ? ControlPaint.Light(baseColor, 0.15f)
            : ControlPaint.Dark(baseColor, 0.05f);

    private protected static Color DerivePressedColor(Color baseColor)
        => baseColor.GetBrightness() < 0.5f
            ? ControlPaint.Light(baseColor, 0.30f)
            : ControlPaint.Dark(baseColor, 0.12f);

    private protected static Color DeriveDisabledColor(Color baseColor)
    {
        int average = (baseColor.R + baseColor.G + baseColor.B) / 3;

        return Color.FromArgb(
            ((average * 6) + (baseColor.R * 4)) / 10,
            ((average * 6) + (baseColor.G * 4)) / 10,
            ((average * 6) + (baseColor.B * 4)) / 10);
    }

    /// <summary>
    ///  Clears the background with the parent's background color or the control's background color if no parent is available.
    /// </summary>
    /// <param name="graphics">Graphics context to draw on</param>
    private static void ClearBackground(Graphics graphics, Color parentBackgroundColor)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        graphics.Clear(parentBackgroundColor);
    }

    /// <summary>
    ///  Renders a button with the specified properties and a delegate for painting its content.
    /// </summary>
    public void RenderButton(
        Graphics graphics,
        Control control,
        Rectangle bounds,
        FlatStyle flatStyle,
        PushButtonState state,
        bool isDefault,
        bool focused,
        bool showFocusCues,
        Color parentBackgroundColor,
        Color backColor,
        Action<Rectangle> paintContent)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(paintContent);

        // Scope the graphics state so all changes are reverted after rendering
        using (new GraphicsStateScope(graphics))
        {
            // Use padding from the renderer. When the focus ring is not drawn, renderers may return a smaller
            // padding so the button body expands into the space the ring and its gap would otherwise occupy.
            Padding padding = GetContentPadding(focused && showFocusCues);

            Rectangle paddedBounds = new(
                x: bounds.X + padding.Left,
                y: bounds.Y + padding.Top,
                width: bounds.Width - padding.Horizontal,
                height: bounds.Height - padding.Vertical);

            if (PaintParentBackground && paddedBounds.Width > 0 && paddedBounds.Height > 0)
            {
                ParentBackgroundRenderer.Paint(control, graphics, bounds, parentBackgroundColor);
            }
            else
            {
                // Rectangular renderers still need a complete background before painting their body.
                ClearBackground(graphics, parentBackgroundColor);
            }

            // Draw button background and get content bounds
            Rectangle contentBounds = DrawButtonBackground(graphics, paddedBounds, state, isDefault, focused, backColor);

            paintContent(contentBounds);

            if (focused && showFocusCues)
            {
                // Draw focus indicator for other styles
                DrawFocusIndicator(graphics, bounds, isDefault);
            }
        }
    }

    private protected virtual bool PaintParentBackground => false;

    public abstract Rectangle DrawButtonBackground(
        Graphics graphics,
        Rectangle bounds,
        PushButtonState state,
        bool isDefault,
        bool focused,
        Color backColor);

    public abstract void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault);

    public abstract Color GetTextColor(PushButtonState state, bool isDefault, Color backColor);

    public Color GetBackgroundColor(PushButtonState state, bool isDefault, Color customBaseColor)
    {
        if (state == PushButtonState.Hot
            && FlatAppearance is { MouseOverBackColorCore.IsEmpty: false } hoverAppearance)
        {
            return hoverAppearance.MouseOverBackColorCore;
        }

        if (state == PushButtonState.Pressed
            && FlatAppearance is { MouseDownBackColorCore.IsEmpty: false } pressedAppearance)
        {
            return pressedAppearance.MouseDownBackColorCore;
        }

        if (UseModernStateDefaults
            && FlatAppearance is not null
            && state is PushButtonState.Hot or PushButtonState.Pressed)
        {
            return ModernButtonColorMath.GetStateColor(this, state, isDefault, customBaseColor);
        }

        if (!customBaseColor.IsEmpty)
        {
            return state switch
            {
                PushButtonState.Disabled => DeriveDisabledColor(customBaseColor),
                PushButtonState.Hot => DeriveHoverColor(customBaseColor),
                PushButtonState.Pressed => DerivePressedColor(customBaseColor),
                _ => customBaseColor
            };
        }

        return GetBackgroundColor(state, isDefault);
    }

    public abstract Color GetBackgroundColor(PushButtonState state, bool isDefault);
}
