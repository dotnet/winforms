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
        rect.Inflate(-ButtonBaseControl.FlatAppearance.BorderSize, -ButtonBaseControl.FlatAppearance.BorderSize);
        ButtonBaseControl.PaintBackground(e, rect, backColor, rect.Location);
    }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        bool hasCustomBorder = ButtonBaseControl.FlatAppearance.BorderSize != BorderSize || !ButtonBaseControl.FlatAppearance.BorderColor.IsEmpty;

        ColorData colors = PaintFlatRender(e).Calculate();
        LayoutData layout = PaintFlatLayout(
            up: !ButtonBaseControl.FlatAppearance.CheckedBackColor.IsEmpty
                || (SystemInformation.HighContrast ? state != CheckState.Indeterminate : state == CheckState.Unchecked),
            check: !hasCustomBorder && SystemInformation.HighContrast && state == CheckState.Checked,
            ButtonBaseControl.FlatAppearance.BorderSize).Layout();

        // Paint with the BorderColor if set.
        if (!ButtonBaseControl.FlatAppearance.BorderColor.IsEmpty)
        {
            colors.WindowFrame = ButtonBaseControl.FlatAppearance.BorderColor;
        }

        Rectangle r = ButtonBaseControl.ClientRectangle;

        Color backColor = ButtonBaseControl.BackColor;

        if (!ButtonBaseControl.FlatAppearance.CheckedBackColor.IsEmpty)
        {
            switch (state)
            {
                case CheckState.Checked:
                    backColor = ButtonBaseControl.FlatAppearance.CheckedBackColor;
                    break;
                case CheckState.Indeterminate:
                    backColor = ButtonBaseControl.FlatAppearance.CheckedBackColor.MixColor(colors.ButtonFace);
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

        PaintBackground(e, r, IsHighContrastHighlighted() ? SystemColors.Highlight : backColor);

        if (ButtonBaseControl.IsDefault)
        {
            r.Inflate(-1, -1);
        }

        PaintImage(e, layout);
        PaintField(e, layout, colors, IsHighContrastHighlighted() ? SystemColors.HighlightText : colors.WindowText, drawFocus: false);

        if (ButtonBaseControl.Focused && ButtonBaseControl.ShowFocusCues)
        {
            DrawFlatFocus(e, layout.Focus, colors.Options.HighContrast ? colors.WindowText : colors.ContrastButtonShadow);
        }

        if (!(ButtonBaseControl.IsDefault && ButtonBaseControl.Focused && (ButtonBaseControl.FlatAppearance.BorderSize == 0)))
        {
            DrawDefaultBorder(e, r, colors.WindowFrame, ButtonBaseControl.IsDefault);
        }

        // Always check if the BorderSize is not the default.If not, we need to paint with the BorderSize set by the user.
        if (hasCustomBorder)
        {
            if (ButtonBaseControl.FlatAppearance.BorderSize != BorderSize)
            {
                DrawFlatBorderWithSize(e, r, colors.WindowFrame, ButtonBaseControl.FlatAppearance.BorderSize);
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
        bool hasCustomBorder = (ButtonBaseControl.FlatAppearance.BorderSize != BorderSize || !ButtonBaseControl.FlatAppearance.BorderColor.IsEmpty);

        ColorData colors = PaintFlatRender(e).Calculate();
        LayoutData layout = PaintFlatLayout(
            !ButtonBaseControl.FlatAppearance.CheckedBackColor.IsEmpty
                || (SystemInformation.HighContrast ? state != CheckState.Indeterminate : state == CheckState.Unchecked),
            !hasCustomBorder && SystemInformation.HighContrast && state == CheckState.Checked,
            ButtonBaseControl.FlatAppearance.BorderSize).Layout();

        // Paint with the BorderColor if Set.
        if (!ButtonBaseControl.FlatAppearance.BorderColor.IsEmpty)
        {
            colors.WindowFrame = ButtonBaseControl.FlatAppearance.BorderColor;
        }

        Rectangle r = ButtonBaseControl.ClientRectangle;

        Color backColor = ButtonBaseControl.BackColor;

        if (!ButtonBaseControl.FlatAppearance.MouseDownBackColor.IsEmpty)
        {
            backColor = ButtonBaseControl.FlatAppearance.MouseDownBackColor;
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

        if (ButtonBaseControl.IsDefault)
        {
            r.Inflate(-1, -1);
        }

        PaintImage(e, layout);
        PaintField(e, layout, colors, colors.WindowText, drawFocus: false);

        if (ButtonBaseControl.Focused && ButtonBaseControl.ShowFocusCues)
        {
            DrawFlatFocus(e, layout.Focus, colors.Options.HighContrast ? colors.WindowText : colors.ContrastButtonShadow);
        }

        if (!(ButtonBaseControl.IsDefault && ButtonBaseControl.Focused && (ButtonBaseControl.FlatAppearance.BorderSize == 0)))
        {
            DrawDefaultBorder(e, r, colors.WindowFrame, ButtonBaseControl.IsDefault);
        }

        // Always check if the BorderSize is not the default.If not, we need to paint with the BorderSize set by the user.
        if (hasCustomBorder)
        {
            if (ButtonBaseControl.FlatAppearance.BorderSize != BorderSize)
            {
                DrawFlatBorderWithSize(e, r, colors.WindowFrame, ButtonBaseControl.FlatAppearance.BorderSize);
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
            bool hasCustomBorder = ButtonBaseControl.FlatAppearance.BorderSize != BorderSize || !ButtonBaseControl.FlatAppearance.BorderColor.IsEmpty;

            ColorData colors = PaintFlatRender(e).Calculate();
            LayoutData layout = PaintFlatLayout(
                up: !ButtonBaseControl.FlatAppearance.CheckedBackColor.IsEmpty || state == CheckState.Unchecked,
                check: false,
                ButtonBaseControl.FlatAppearance.BorderSize).Layout();

            // Paint with the BorderColor if Set.
            if (!ButtonBaseControl.FlatAppearance.BorderColor.IsEmpty)
            {
                colors.WindowFrame = ButtonBaseControl.FlatAppearance.BorderColor;
            }

            Rectangle r = ButtonBaseControl.ClientRectangle;

            Color backColor = !ButtonBaseControl.FlatAppearance.MouseOverBackColor.IsEmpty
                ? ButtonBaseControl.FlatAppearance.MouseOverBackColor
                : !ButtonBaseControl.FlatAppearance.CheckedBackColor.IsEmpty
                    ? state is CheckState.Checked or CheckState.Indeterminate
                        ? ButtonBaseControl.FlatAppearance.CheckedBackColor.MixColor(colors.LowButtonFace)
                        : colors.LowButtonFace
                    : state is CheckState.Indeterminate
                        ? colors.ButtonFace.MixColor(colors.LowButtonFace)
                        : colors.LowButtonFace;

            PaintBackground(e, r, IsHighContrastHighlighted() ? SystemColors.Highlight : backColor);

            if (ButtonBaseControl.IsDefault)
            {
                r.Inflate(-1, -1);
            }

            PaintImage(e, layout);

            PaintField(
                e,
                layout,
                colors,
                IsHighContrastHighlighted()
                    ? SystemColors.HighlightText
                    : colors.WindowText,
                drawFocus: false);

            if (ButtonBaseControl.Focused && ButtonBaseControl.ShowFocusCues)
            {
                DrawFlatFocus(e, layout.Focus, colors.ContrastButtonShadow);
            }

            if (!(ButtonBaseControl.IsDefault && ButtonBaseControl.Focused && (ButtonBaseControl.FlatAppearance.BorderSize == 0)))
            {
                DrawDefaultBorder(e, r, colors.WindowFrame, ButtonBaseControl.IsDefault);
            }

            // Always check if the BorderSize is not the default.If not, we need to paint with the BorderSize set by the user.
            if (hasCustomBorder)
            {
                if (ButtonBaseControl.FlatAppearance.BorderSize != BorderSize)
                {
                    DrawFlatBorderWithSize(e, r, colors.WindowFrame, ButtonBaseControl.FlatAppearance.BorderSize);
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
        PaintFlatLayout(up: false, check: true, ButtonBaseControl.FlatAppearance.BorderSize);

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
