// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.ButtonInternal;

internal class ButtonFlatAdapter : ButtonBaseAdapter
{
    private const int BorderSize = 1;

    internal ButtonFlatAdapter(ButtonBase control) : base(control) { }

    private void PaintBackground(PaintEventArgs e, Rectangle r, Color backColor)
    {
        Rectangle rect = r;
        rect.Inflate(-Control.FlatAppearance.BorderSize, -Control.FlatAppearance.BorderSize);
        Control.PaintBackground(e, rect, backColor, rect.Location);
    }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        bool hasCustomBorder = Control.FlatAppearance.BorderSize != BorderSize || !Control.FlatAppearance.BorderColor.IsEmpty;

        ColorData colors = PaintFlatRender(e).Calculate();
        LayoutData layout = PaintFlatLayout(
            up: !Control.FlatAppearance.CheckedBackColor.IsEmpty
                || (SystemInformation.HighContrast ? state != CheckState.Indeterminate : state == CheckState.Unchecked),
            check: !hasCustomBorder && SystemInformation.HighContrast && state == CheckState.Checked,
            Control.FlatAppearance.BorderSize).Layout();

        // Paint with the BorderColor if set.
        if (!Control.FlatAppearance.BorderColor.IsEmpty)
        {
            colors.WindowFrame = Control.FlatAppearance.BorderColor;
        }

        Rectangle r = Control.ClientRectangle;

        Color backColor = Control.BackColor;

        if (!Control.FlatAppearance.CheckedBackColor.IsEmpty)
        {
            switch (state)
            {
                case CheckState.Checked:
                    backColor = Control.FlatAppearance.CheckedBackColor;
                    break;
                case CheckState.Indeterminate:
                    backColor = Control.FlatAppearance.CheckedBackColor.MixColor(colors.ButtonFace);
                    break;
            }
        }
        else
        {
            switch (state)
            {
                case CheckState.Checked:
                    backColor = colors.Highlight;
                    break;
                case CheckState.Indeterminate:
                    backColor = colors.Highlight.MixColor(colors.ButtonFace);
                    break;
            }
        }

        PaintBackground(e, r, IsHighContrastHighlighted() ? Application.SystemColors.Highlight : backColor);

        if (Control.IsDefault)
        {
            r.Inflate(-1, -1);
        }

        PaintImage(e, layout);
        PaintField(e, layout, colors, IsHighContrastHighlighted() ? Application.SystemColors.HighlightText : colors.WindowText, drawFocus: false);

        if (Control.Focused && Control.ShowFocusCues)
        {
            DrawFlatFocus(e, layout.Focus, colors.Options.HighContrast ? colors.WindowText : colors.ContrastButtonShadow);
        }

        if (!(Control.IsDefault && Control.Focused && (Control.FlatAppearance.BorderSize == 0)))
        {
            DrawDefaultBorder(e, r, colors.WindowFrame, Control.IsDefault);
        }

        // Always check if the BorderSize is not the default.If not, we need to paint with the BorderSize set by the user.
        if (hasCustomBorder)
        {
            if (Control.FlatAppearance.BorderSize != BorderSize)
            {
                DrawFlatBorderWithSize(e, r, colors.WindowFrame, Control.FlatAppearance.BorderSize);
            }
            else
            {
                ControlPaint.DrawBorderSimple(e, r, colors.WindowFrame);
            }
        }
        else if (state == CheckState.Checked && SystemInformation.HighContrast)
        {
            ControlPaint.DrawBorderSimple(e, r, colors.WindowFrame);
            ControlPaint.DrawBorderSimple(e, r, colors.ButtonShadow);
        }
        else if (state == CheckState.Indeterminate)
        {
            Draw3DLiteBorder(e, r, colors, up: false);
        }
        else
        {
            ControlPaint.DrawBorderSimple(e, r, colors.WindowFrame);
        }
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        bool hasCustomBorder = (Control.FlatAppearance.BorderSize != BorderSize || !Control.FlatAppearance.BorderColor.IsEmpty);

        ColorData colors = PaintFlatRender(e).Calculate();
        LayoutData layout = PaintFlatLayout(
            !Control.FlatAppearance.CheckedBackColor.IsEmpty
                || (SystemInformation.HighContrast ? state != CheckState.Indeterminate : state == CheckState.Unchecked),
            !hasCustomBorder && SystemInformation.HighContrast && state == CheckState.Checked,
            Control.FlatAppearance.BorderSize).Layout();

        // Paint with the BorderColor if Set.
        if (!Control.FlatAppearance.BorderColor.IsEmpty)
        {
            colors.WindowFrame = Control.FlatAppearance.BorderColor;
        }

        Rectangle r = Control.ClientRectangle;

        Color backColor = Control.BackColor;

        if (!Control.FlatAppearance.MouseDownBackColor.IsEmpty)
        {
            backColor = Control.FlatAppearance.MouseDownBackColor;
        }
        else
        {
            switch (state)
            {
                case CheckState.Unchecked:
                case CheckState.Checked:
                    backColor = colors.Options.HighContrast ? colors.ButtonShadow : colors.LowHighlight;
                    break;
                case CheckState.Indeterminate:
                    backColor = colors.ButtonFace.MixColor(colors.Options.HighContrast
                        ? colors.ButtonShadow
                        : colors.LowHighlight);
                    break;
            }
        }

        PaintBackground(e, r, backColor);

        if (Control.IsDefault)
        {
            r.Inflate(-1, -1);
        }

        PaintImage(e, layout);
        PaintField(e, layout, colors, colors.WindowText, drawFocus: false);

        if (Control.Focused && Control.ShowFocusCues)
        {
            DrawFlatFocus(e, layout.Focus, colors.Options.HighContrast ? colors.WindowText : colors.ContrastButtonShadow);
        }

        if (!(Control.IsDefault && Control.Focused && (Control.FlatAppearance.BorderSize == 0)))
        {
            DrawDefaultBorder(e, r, colors.WindowFrame, Control.IsDefault);
        }

        // Always check if the BorderSize is not the default.If not, we need to paint with the BorderSize set by the user.
        if (hasCustomBorder)
        {
            if (Control.FlatAppearance.BorderSize != BorderSize)
            {
                DrawFlatBorderWithSize(e, r, colors.WindowFrame, Control.FlatAppearance.BorderSize);
            }
            else
            {
                ControlPaint.DrawBorderSimple(e, r, colors.WindowFrame);
            }
        }
        else if (state == CheckState.Checked && SystemInformation.HighContrast)
        {
            ControlPaint.DrawBorderSimple(e, r, colors.WindowFrame);
            ControlPaint.DrawBorderSimple(e, r, colors.ButtonShadow);
        }
        else if (state == CheckState.Indeterminate)
        {
            Draw3DLiteBorder(e, r, colors, up: false);
        }
        else
        {
            ControlPaint.DrawBorderSimple(e, r, colors.WindowFrame);
        }
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        if (SystemInformation.HighContrast)
        {
            PaintUp(e, state);
        }
        else
        {
            bool hasCustomBorder = Control.FlatAppearance.BorderSize != BorderSize || !Control.FlatAppearance.BorderColor.IsEmpty;

            ColorData colors = PaintFlatRender(e).Calculate();
            LayoutData layout = PaintFlatLayout(
                up: !Control.FlatAppearance.CheckedBackColor.IsEmpty || state == CheckState.Unchecked,
                check: false,
                Control.FlatAppearance.BorderSize).Layout();

            // Paint with the BorderColor if Set.
            if (!Control.FlatAppearance.BorderColor.IsEmpty)
            {
                colors.WindowFrame = Control.FlatAppearance.BorderColor;
            }

            Rectangle r = Control.ClientRectangle;

            Color backColor;
            if (!Control.FlatAppearance.MouseOverBackColor.IsEmpty)
            {
                backColor = Control.FlatAppearance.MouseOverBackColor;
            }
            else if (!Control.FlatAppearance.CheckedBackColor.IsEmpty)
            {
                backColor = state is CheckState.Checked or CheckState.Indeterminate
                    ? Control.FlatAppearance.CheckedBackColor.MixColor(colors.LowButtonFace)
                    : colors.LowButtonFace;
            }
            else
            {
                backColor = state is CheckState.Indeterminate
                    ? colors.ButtonFace.MixColor(colors.LowButtonFace)
                    : colors.LowButtonFace;
            }

            PaintBackground(e, r, IsHighContrastHighlighted() ? Application.SystemColors.Highlight : backColor);

            if (Control.IsDefault)
            {
                r.Inflate(-1, -1);
            }

            PaintImage(e, layout);
            PaintField(
                e,
                layout,
                colors,
                IsHighContrastHighlighted() ? Application.SystemColors.HighlightText : colors.WindowText,
                drawFocus: false);

            if (Control.Focused && Control.ShowFocusCues)
            {
                DrawFlatFocus(e, layout.Focus, colors.ContrastButtonShadow);
            }

            if (!(Control.IsDefault && Control.Focused && (Control.FlatAppearance.BorderSize == 0)))
            {
                DrawDefaultBorder(e, r, colors.WindowFrame, Control.IsDefault);
            }

            // Always check if the BorderSize is not the default.If not, we need to paint with the BorderSize set by the user.
            if (hasCustomBorder)
            {
                if (Control.FlatAppearance.BorderSize != BorderSize)
                {
                    DrawFlatBorderWithSize(e, r, colors.WindowFrame, Control.FlatAppearance.BorderSize);
                }
                else
                {
                    ControlPaint.DrawBorderSimple(e, r, colors.WindowFrame);
                }
            }
            else if (state == CheckState.Unchecked)
            {
                ControlPaint.DrawBorderSimple(e, r, colors.WindowFrame);
            }
            else
            {
                Draw3DLiteBorder(e, r, colors, up: false);
            }
        }
    }

    protected override LayoutOptions Layout(PaintEventArgs e) =>
        PaintFlatLayout(up: false, check: true, Control.FlatAppearance.BorderSize);

    internal static LayoutOptions PaintFlatLayout(
        bool up,
        bool check,
        int borderSize,
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
        layout.BorderSize = borderSize + (check ? 1 : 0);
        layout.PaddingSize = check ? 1 : 2;
        layout.FocusOddEvenFixup = false;
        layout.TextOffset = !up;
        layout.ShadowedText = SystemInformation.HighContrast;

        return layout;
    }

    private LayoutOptions PaintFlatLayout(bool up, bool check, int borderSize)
    {
        LayoutOptions layout = CommonLayout();
        layout.BorderSize = borderSize + (check ? 1 : 0);
        layout.PaddingSize = check ? 1 : 2;
        layout.FocusOddEvenFixup = false;
        layout.TextOffset = !up;
        layout.ShadowedText = SystemInformation.HighContrast;

        return layout;
    }
}
