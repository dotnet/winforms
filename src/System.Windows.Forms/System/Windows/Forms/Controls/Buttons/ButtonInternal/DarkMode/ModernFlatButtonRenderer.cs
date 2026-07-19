// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Renders a modern <see cref="FlatStyle.Flat"/> button with a rectangular body and a single border.
/// </summary>
internal sealed class ModernFlatButtonRenderer : ButtonDarkModeRendererBase
{
    private const int BorderThicknessLogical = 1;
    private const int ContentInsetLogical = 4;
    private const int FocusInsetLogical = 3;

    private static readonly Color s_lightBorder = Color.FromArgb(0xD0, 0xD0, 0xD0);
    private static readonly Color s_darkBorder = Color.FromArgb(0x55, 0x55, 0x55);
    private static readonly Color s_lightNormal = Color.FromArgb(0xFB, 0xFB, 0xFB);
    private static readonly Color s_lightDisabled = Color.FromArgb(0xFA, 0xFA, 0xFA);
    private static readonly Color s_darkNormal = Color.FromArgb(0x2D, 0x2D, 0x2D);
    private static readonly Color s_darkDisabled = Color.FromArgb(0x25, 0x25, 0x25);

    private static bool IsDark => Application.IsDarkModeEnabled;

    private int BorderThickness
        => ScaleBorderThickness(Math.Max(1, Scale(BorderThicknessLogical)));

    private protected override Padding PaddingCore => Padding.Empty;

    private protected override bool UseModernStateDefaults => true;

    internal override Padding GetPreferredSizePadding()
        => new(Math.Max(BorderThickness, Scale(ContentInsetLogical)));

    public override Rectangle DrawButtonBackground(
        Graphics graphics,
        Rectangle bounds,
        PushButtonState state,
        bool isDefault,
        bool focused,
        Color backColor)
    {
        GraphicsState? saved = graphics.Save();
        try
        {
            graphics.SmoothingMode = SmoothingMode.Default;

            using (var brush = backColor.GetCachedSolidBrushScope())
            {
                graphics.FillRectangle(brush, bounds);
            }

            int thickness = BorderThickness;
            if (thickness > 0 && bounds.Width > 0 && bounds.Height > 0)
            {
                Color designBorderColor = isDefault
                    ? (IsDark ? Color.White : SystemColors.Highlight)
                    : (IsDark ? s_darkBorder : s_lightBorder);

                using var pen = new Pen(ResolveBorderColor(designBorderColor), thickness)
                {
                    Alignment = PenAlignment.Inset
                };

                graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
        }
        finally
        {
            if (saved is not null)
            {
                graphics.Restore(saved);
            }
        }

        int inset = Math.Max(BorderThickness, Scale(ContentInsetLogical));
        return Rectangle.Inflate(bounds, -inset, -inset);
    }

    public override void DrawFocusIndicator(Graphics graphics, Rectangle contentBounds, bool isDefault)
    {
        int inset = Scale(FocusInsetLogical);
        Rectangle focusRectangle = Rectangle.Inflate(contentBounds, -inset, -inset);
        if (focusRectangle.Width <= 0 || focusRectangle.Height <= 0)
        {
            return;
        }

        Color focusColor = ResolveBorderColor(IsDark ? Color.White : SystemColors.WindowText);
        using var focusPen = new Pen(focusColor) { DashStyle = DashStyle.Dot };
        graphics.DrawRectangle(focusPen, focusRectangle);
    }

    public override Color GetTextColor(PushButtonState state, bool isDefault, Color backColor)
    {
        if (state == PushButtonState.Disabled)
        {
            Color preferredForeColor = IsDark
                ? Color.FromArgb(0x88, 0x88, 0x88)
                : Color.FromArgb(0xA0, 0xA0, 0xA0);

            return ModernControlColorMath.GetDisabledTextColor(
                preferredForeColor,
                backColor);
        }

        return ModernButtonColorMath.GetReadableForeColor(backColor);
    }

    public override Color GetBackgroundColor(PushButtonState state, bool isDefault)
    {
        if (state == PushButtonState.Disabled)
        {
            return IsDark ? s_darkDisabled : s_lightDisabled;
        }

        if (isDefault)
        {
            return ModernButtonColorMath.GetDefaultButtonColor(state);
        }

        Color baseColor = IsDark ? s_darkNormal : s_lightNormal;

        return state switch
        {
            PushButtonState.Hot => DeriveHoverColor(baseColor),
            PushButtonState.Pressed => DerivePressedColor(baseColor),
            _ => baseColor
        };
    }
}
