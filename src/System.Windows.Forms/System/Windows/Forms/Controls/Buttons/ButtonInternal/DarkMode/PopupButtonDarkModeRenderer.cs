// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.DarkModeButtonColors;

namespace System.Windows.Forms;

/// <summary>
///  Renders a Classic <see cref="FlatStyle.Popup"/> button in dark mode.
/// </summary>
internal sealed class PopupButtonDarkModeRenderer : ButtonDarkModeRendererBase
{
    private const int CornerRadiusLogical = 5;
    private const int ContentInsetLogical = 3;
    private const int FocusInsetLogical = 2;
    private const int PressOffsetLogical = 1;

    private protected override Padding PaddingCore => Padding.Empty;

    private protected override bool PaintParentBackground => true;

    public override Rectangle DrawButtonBackground(
        Graphics graphics,
        Rectangle bounds,
        PushButtonState state,
        bool isDefault,
        bool focused,
        Color backColor)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return Rectangle.Empty;
        }

        Rectangle bodyBounds = bounds;
        bodyBounds.Width--;
        bodyBounds.Height--;

        using GraphicsPath bodyPath = CreateRoundedPath(bodyBounds);
        using (var brush = backColor.GetCachedSolidBrushScope())
        {
            graphics.FillPath(brush, bodyPath);
        }

        DrawButtonBorder(
            graphics,
            bodyBounds,
            state,
            isDefault);

        int contentInset = Math.Max(1, Scale(ContentInsetLogical));
        Rectangle contentBounds = Rectangle.Inflate(
            bodyBounds,
            -contentInset,
            -contentInset);

        if (state == PushButtonState.Pressed)
        {
            int pressOffset = Math.Max(1, Scale(PressOffsetLogical));
            contentBounds.Offset(pressOffset, pressOffset);
        }

        return contentBounds;
    }

    public override void DrawFocusIndicator(
        Graphics graphics,
        Rectangle contentBounds,
        bool isDefault)
    {
        int focusInset = Math.Max(1, Scale(FocusInsetLogical));
        Rectangle focusBounds = Rectangle.Inflate(
            contentBounds,
            -focusInset,
            -focusInset);
        if (focusBounds.Width <= 0 || focusBounds.Height <= 0)
        {
            return;
        }

        Color focusBackColor = isDefault
            ? DefaultColors.AcceptFocusIndicatorBackColor
            : DefaultColors.FocusIndicatorBackColor;
        ControlPaint.DrawFocusRectangle(
            graphics,
            focusBounds,
            DefaultColors.FocusBorderColor,
            focusBackColor);
    }

    public override Color GetTextColor(
        PushButtonState state,
        bool isDefault,
        Color backColor)
        => state == PushButtonState.Disabled
            ? DefaultColors.DisabledTextColor
            : DefaultColors.NormalTextColor;

    public override Color GetBackgroundColor(
        PushButtonState state,
        bool isDefault)
        => state switch
        {
            PushButtonState.Hot => DefaultColors.HoverBackColor,
            PushButtonState.Pressed => DefaultColors.PressedBackColor,
            PushButtonState.Disabled => DefaultColors.DisabledBackColor,
            _ => DefaultColors.StandardBackColor
        };

    private void DrawButtonBorder(
        Graphics graphics,
        Rectangle bounds,
        PushButtonState state,
        bool isDefault)
    {
        int thickness = ScaleBorderThickness(
            Math.Max(1, Scale(1)));
        if (thickness == 0)
        {
            return;
        }

        using GraphicsStateScope graphicsStateScope = new(graphics);
        using GraphicsPath borderClip = CreateRoundedPath(bounds);
        graphics.SetClip(borderClip);

        Color topLeftColor;
        Color bottomRightColor;

        if (FlatAppearance is { BorderColor.IsEmpty: false } appearance)
        {
            topLeftColor = appearance.BorderColor;
            bottomRightColor = appearance.BorderColor;
        }
        else
        {
            (topLeftColor, bottomRightColor) = state switch
            {
                PushButtonState.Pressed => (
                    DefaultColors.ShadowDarkColor,
                    DefaultColors.HighlightBrightColor),
                PushButtonState.Disabled => (
                    DefaultColors.DisabledBorderLightColor,
                    DefaultColors.DisabledBorderDarkColor),
                _ => (
                    DefaultColors.HighlightColor,
                    DefaultColors.ShadowDarkColor)
            };
        }

        using var topLeftPen = topLeftColor.GetCachedPenScope(
            thickness);
        using var bottomRightPen = bottomRightColor.GetCachedPenScope(
            thickness);
        graphics.DrawLine(
            topLeftPen,
            bounds.Left,
            bounds.Bottom - 1,
            bounds.Left,
            bounds.Top);
        graphics.DrawLine(
            topLeftPen,
            bounds.Left,
            bounds.Top,
            bounds.Right - 1,
            bounds.Top);
        graphics.DrawLine(
            bottomRightPen,
            bounds.Right - 1,
            bounds.Top,
            bounds.Right - 1,
            bounds.Bottom - 1);
        graphics.DrawLine(
            bottomRightPen,
            bounds.Right - 1,
            bounds.Bottom - 1,
            bounds.Left,
            bounds.Bottom - 1);

        if (isDefault && state != PushButtonState.Disabled)
        {
            Rectangle defaultBounds = Rectangle.Inflate(
                bounds,
                -thickness,
                -thickness);
            if (defaultBounds.Width > 0 && defaultBounds.Height > 0)
            {
                using var defaultPen =
                    DefaultColors.AcceptFocusIndicatorBackColor
                        .GetCachedPenScope(thickness);
                graphics.DrawRectangle(defaultPen, defaultBounds);
            }
        }
    }

    private GraphicsPath CreateRoundedPath(Rectangle bounds)
    {
        GraphicsPath path = new();
        int radius = Math.Clamp(
            Scale(CornerRadiusLogical),
            1,
            Math.Max(1, Math.Min(bounds.Width, bounds.Height)));
        path.AddRoundedRectangle(
            bounds,
            new Size(radius, radius));

        return path;
    }
}
