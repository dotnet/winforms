// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.DarkModeButtonColors;

namespace System.Windows.Forms;

/// <summary>
///  Flat‑style button renderer that, for the moment, mimics the Win32/Dark‑mode
///  renderer bit‑for‑bit so that we can switch implementations without any
///  visual delta.  Once the design team decides on a new look we can diverge
///  again.
/// </summary>
internal sealed class FlatButtonDarkModeRenderer : ButtonDarkModeRendererBase
{
    private const int FocusIndicatorInflate = -3;
    private const int CornerRadius = 6;
    private static readonly Size s_corner = new(CornerRadius, CornerRadius);

    private protected override Padding PaddingCore { get; } = new(0);

    public override Rectangle DrawButtonBackground(
        Graphics graphics,
        Rectangle bounds,
        PushButtonState state,
        bool isDefault,
        bool focused,
        Color backColor)
    {
        // fill background
        using var back = backColor.GetCachedSolidBrushScope();
        graphics.FillRectangle(back, bounds);

        // draw border identical to Win32
        DrawButtonBorder(graphics, bounds, state, isDefault);

        // return inner content area (border + 1 px system padding)
        return Rectangle.Inflate(bounds, -3, -3);
    }

    public override void DrawFocusIndicator(Graphics g, Rectangle contentBounds, bool isDefault)
    {
        Rectangle focus = Rectangle.Inflate(contentBounds, FocusIndicatorInflate, FocusIndicatorInflate);

        Color focusBackColor = isDefault
            ? DefaultColors.AcceptFocusIndicatorBackColor
            : DefaultColors.FocusIndicatorBackColor;

        ControlPaint.DrawFocusRectangle(
            g,
            focus,
            DefaultColors.FocusBorderColor,
            focusBackColor);
    }

    public override Color GetTextColor(PushButtonState state, bool isDefault, Color backColor) =>
        state == PushButtonState.Disabled
            ? DefaultColors.DisabledTextColor
            : isDefault
                ? DefaultColors.AcceptButtonTextColor
                : DefaultColors.NormalTextColor;

    public override Color GetBackgroundColor(PushButtonState state, bool isDefault) =>
        isDefault
            ? state switch
            {
                PushButtonState.Normal => DefaultColors.StandardBackColor,
                PushButtonState.Hot => DefaultColors.HoverBackColor,
                PushButtonState.Pressed => DefaultColors.PressedBackColor,
                PushButtonState.Disabled => DefaultColors.DisabledBackColor,
                _ => DefaultColors.StandardBackColor
            }
            : state switch
            {
                PushButtonState.Normal => DefaultColors.StandardBackColor,
                PushButtonState.Hot => DefaultColors.HoverBackColor,
                PushButtonState.Pressed => DefaultColors.PressedBackColor,
                PushButtonState.Disabled => DefaultColors.DisabledBackColor,
                _ => DefaultColors.StandardBackColor
            };

    private void DrawButtonBorder(Graphics g, Rectangle bounds, PushButtonState state, bool isDefault)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        int thickness = ScaleBorderThickness(1);
        if (thickness == 0)
        {
            return;
        }

        // Win32 draws its stroke fully inside the control.
        Rectangle outer = Rectangle.Inflate(bounds, -1, -1);

        DrawSingleBorder(g, outer, ResolveBorderColor(GetBorderColor(state)), thickness);

        // Default button gets a second border one pixel further inside.
        if (isDefault)
        {
            Rectangle inner = Rectangle.Inflate(outer, -1, -1);
            DrawSingleBorder(g, inner, DefaultColors.AcceptFocusIndicatorBackColor, thickness);
        }
    }

    private static void DrawSingleBorder(Graphics g, Rectangle rect, Color color, int thickness)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var path = new GraphicsPath();
        path.AddRoundedRectangle(rect, s_corner);

        using var pen = new Pen(color, thickness) { Alignment = PenAlignment.Inset };
        g.DrawPath(pen, path);
    }

    private static Color GetBorderColor(PushButtonState state) =>
        state switch
        {
            PushButtonState.Pressed => DefaultColors.PressedSingleBorderColor,
            PushButtonState.Hot => DefaultColors.SingleBorderColor,
            PushButtonState.Disabled => DefaultColors.DisabledBorderLightColor,
            _ => DefaultColors.SingleBorderColor,
        };
}
