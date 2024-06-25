// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms.ButtonInternal;

internal class ButtonPopupAdapter : ButtonBaseAdapter
{
    internal ButtonPopupAdapter(ButtonBase control) : base(control) { }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        ColorData colors = PaintPopupRender(e).Calculate();
        LayoutData layout = PaintPopupLayout(state == CheckState.Unchecked, 1).Layout();

        Rectangle r = Control.ClientRectangle;

        if (state == CheckState.Indeterminate)
        {
            using Brush backgroundBrush = CreateDitherBrush(colors.Highlight, colors.ButtonFace);
            PaintButtonBackground(e, r, backgroundBrush);
        }
        else
        {
            Control.PaintBackground(e, r, IsHighContrastHighlighted() ? Application.ApplicationColors.Highlight : Control.BackColor, r.Location);
        }

        if (Control.IsDefault)
        {
            r.Inflate(-1, -1);
        }

        PaintImage(e, layout);
        PaintField(
            e,
            layout,
            colors,
            state != CheckState.Indeterminate && IsHighContrastHighlighted() ? Application.ApplicationColors.HighlightText : colors.WindowText,
            drawFocus: true);

        Color borderColor = colors.Options.HighContrast
            ? colors.WindowText
            : GetContrastingBorderColor(colors.ButtonShadow);

        DrawDefaultBorder(e, r, borderColor, Control.IsDefault);

        if (state == CheckState.Unchecked)
        {
            ControlPaint.DrawBorderSimple(e, r, borderColor);
        }
        else
        {
            Draw3DLiteBorder(e, r, colors, up: false);
        }
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        ColorData colors = PaintPopupRender(e).Calculate();
        LayoutData layout = PaintPopupLayout(state == CheckState.Unchecked, SystemInformation.HighContrast ? 2 : 1).Layout();

        Rectangle r = Control.ClientRectangle;

        if (state == CheckState.Indeterminate)
        {
            using Brush backgroundBrush = CreateDitherBrush(colors.Highlight, colors.ButtonFace);
            PaintButtonBackground(e, r, backgroundBrush);
        }
        else
        {
            Control.PaintBackground(e, r, IsHighContrastHighlighted() ? Application.ApplicationColors.Highlight : Control.BackColor, r.Location);
        }

        if (Control.IsDefault)
        {
            r.Inflate(-1, -1);
        }

        PaintImage(e, layout);
        PaintField(e, layout, colors, IsHighContrastHighlighted() ? Application.ApplicationColors.HighlightText : colors.WindowText, drawFocus: true);

        DrawDefaultBorder(e, r, colors.Options.HighContrast ? colors.WindowText : colors.ButtonShadow, Control.IsDefault);

        if (SystemInformation.HighContrast)
        {
            Graphics g = e.GraphicsInternal;
            using var windowFrame = colors.WindowFrame.GetCachedPenScope();
            using var highlight = colors.Highlight.GetCachedPenScope();
            using var buttonShadow = colors.ButtonShadow.GetCachedPenScope();

            // top, left white
            g.DrawLine(windowFrame, r.Left + 1, r.Top + 1, r.Right - 2, r.Top + 1);
            g.DrawLine(windowFrame, r.Left + 1, r.Top + 1, r.Left + 1, r.Bottom - 2);

            // bottom, right white
            g.DrawLine(windowFrame, r.Left, r.Bottom - 1, r.Right, r.Bottom - 1);
            g.DrawLine(windowFrame, r.Right - 1, r.Top, r.Right - 1, r.Bottom);

            // top, left gray
            g.DrawLine(highlight, r.Left, r.Top, r.Right, r.Top);
            g.DrawLine(highlight, r.Left, r.Top, r.Left, r.Bottom);

            // bottom, right gray
            g.DrawLine(buttonShadow, r.Left + 1, r.Bottom - 2, r.Right - 2, r.Bottom - 2);
            g.DrawLine(buttonShadow, r.Right - 2, r.Top + 1, r.Right - 2, r.Bottom - 2);

            r.Inflate(-2, -2);
        }
        else
        {
            Draw3DLiteBorder(e, r, colors, true);
        }
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        ColorData colors = PaintPopupRender(e).Calculate();
        LayoutData layout = PaintPopupLayout(up: false, SystemInformation.HighContrast ? 2 : 1).Layout();

        Rectangle r = Control.ClientRectangle;
        PaintButtonBackground(e, r, background: null);
        if (Control.IsDefault)
        {
            r.Inflate(-1, -1);
        }

        r.Inflate(-1, -1);

        PaintImage(e, layout);
        PaintField(e, layout, colors, colors.WindowText, drawFocus: true);

        r.Inflate(1, 1);
        DrawDefaultBorder(e, r, colors.Options.HighContrast ? colors.WindowText : colors.WindowFrame, Control.IsDefault);
        ControlPaint.DrawBorderSimple(e, r, colors.Options.HighContrast ? colors.WindowText : GetContrastingBorderColor(colors.ButtonShadow));
    }

    protected override LayoutOptions Layout(PaintEventArgs e)
    {
        LayoutOptions layout = PaintPopupLayout(up: false, 0);
        Debug.Assert(
            layout.GetPreferredSizeCore(LayoutUtils.s_maxSize) == PaintPopupLayout(up: true, 2).GetPreferredSizeCore(LayoutUtils.s_maxSize),
            "The state of up should not effect PreferredSize");
        return layout;
    }

    internal static LayoutOptions PaintPopupLayout(
        bool up,
        int paintedBorder,
        Rectangle clientRectangle,
        Padding padding,
        bool isDefault,
        Font font,
        string text,
        bool enabled,
        ContentAlignment textAlign,
        RightToLeft rtl)
    {
        LayoutOptions layout = CommonLayout(clientRectangle, padding, isDefault, font, text, enabled, textAlign, rtl);
        layout.BorderSize = paintedBorder;
        layout.PaddingSize = 2 - paintedBorder;
        layout.HintTextUp = false;
        layout.TextOffset = !up;
        layout.ShadowedText = SystemInformation.HighContrast;

        Debug.Assert(
            layout.BorderSize + layout.PaddingSize == 2,
            "It is assumed borderSize + paddingSize will always be 2. Bad value for paintedBorder?");

        return layout;
    }

    private LayoutOptions PaintPopupLayout(bool up, int paintedBorder)
    {
        LayoutOptions layout = CommonLayout();
        layout.BorderSize = paintedBorder;
        layout.PaddingSize = 2 - paintedBorder; // 3 - paintedBorder - (Control.IsDefault ? 1 : 0);
        layout.HintTextUp = false;
        layout.TextOffset = !up;
        layout.ShadowedText = SystemInformation.HighContrast;

        Debug.Assert(
            layout.BorderSize + layout.PaddingSize == 2,
            "borderSize + paddingSize will always be 2. Bad value for paintedBorder?");

        return layout;
    }
}
