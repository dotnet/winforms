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

    private protected override Padding PaddingCore { get; } = new(0);

    public override Rectangle DrawButtonBackground(
        Graphics graphics, Rectangle bounds, int borderSize, PushButtonState state, bool isDefault, Color backColor)
    {
        // fill background
        using var back = backColor.GetCachedSolidBrushScope();

        Rectangle rectangle = bounds;
        rectangle.Inflate(-1 - borderSize, -1 - borderSize);

        graphics.FillRectangle(back, rectangle);
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

    public override Color GetTextColor(PushButtonState state, bool isDefault) =>
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

    public override void DrawButtonBorder(Graphics g, Rectangle bounds, int borderSize, PushButtonState state, bool isDefault)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Win32 draws its stroke fully *inside* the control → inset by 1 px
        Rectangle outer = Rectangle.Inflate(bounds, -1, -1);

        DrawSingleBorder(g, outer, borderSize, GetBorderColor(state));

        // Default button gets a second 1‑px border one pixel further inside
        if (isDefault)
        {
            Rectangle inner = Rectangle.Inflate(outer, -1, -1);
            DrawSingleBorder(g, inner, borderSize, DefaultColors.AcceptFocusIndicatorBackColor);
        }
    }

    private static void DrawSingleBorder(Graphics g, Rectangle rect, int borderSize, Color color)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var pen = new Pen(color, borderSize) { Alignment = PenAlignment.Inset };
        g.DrawRectangle(pen, rect);
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
