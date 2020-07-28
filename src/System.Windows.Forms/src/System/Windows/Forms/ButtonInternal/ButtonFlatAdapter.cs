// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.ButtonInternal
{
    internal class ButtonFlatAdapter : ButtonBaseAdapter
    {
        private const int BORDERSIZE = 1;

        internal ButtonFlatAdapter(ButtonBase control) : base(control) { }

        private void PaintBackground(PaintEventArgs e, Rectangle r, Color backColor)
        {
            Rectangle rect = r;
            rect.Inflate(-Control.FlatAppearance.BorderSize, -Control.FlatAppearance.BorderSize);
            Control.PaintBackground(e, rect, backColor, rect.Location);
        }

        internal override void PaintUp(PaintEventArgs e, CheckState state)
        {
            bool hasCustomBorder = (Control.FlatAppearance.BorderSize != BORDERSIZE || !Control.FlatAppearance.BorderColor.IsEmpty);

            ColorData colors = PaintFlatRender(e).Calculate();
            LayoutData layout = PaintFlatLayout(
                up: !Control.FlatAppearance.CheckedBackColor.IsEmpty
                    || (SystemInformation.HighContrast ? state != CheckState.Indeterminate : state == CheckState.Unchecked),
                check: !hasCustomBorder && SystemInformation.HighContrast && state == CheckState.Checked,
                Control.FlatAppearance.BorderSize).Layout();

            // Paint with the BorderColor if set.
            if (!Control.FlatAppearance.BorderColor.IsEmpty)
            {
                colors.windowFrame = Control.FlatAppearance.BorderColor;
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
                        backColor = Control.FlatAppearance.CheckedBackColor.MixColor(colors.buttonFace);
                        break;
                }
            }
            else
            {
                switch (state)
                {
                    case CheckState.Checked:
                        backColor = colors.highlight;
                        break;
                    case CheckState.Indeterminate:
                        backColor = colors.highlight.MixColor(colors.buttonFace);
                        break;
                }
            }

            PaintBackground(e, r, IsHighContrastHighlighted() ? SystemColors.Highlight : backColor);

            if (Control.IsDefault)
            {
                r.Inflate(-1, -1);
            }

            PaintImage(e, layout);
            PaintField(e, layout, colors, IsHighContrastHighlighted() ? SystemColors.HighlightText : colors.windowText, false);

            if (Control.Focused && Control.ShowFocusCues)
            {
                DrawFlatFocus(e, layout.focus, colors.options.HighContrast ? colors.windowText : colors.constrastButtonShadow);
            }

            if (!(Control.IsDefault && Control.Focused && (Control.FlatAppearance.BorderSize == 0)))
            {
                DrawDefaultBorder(e, r, colors.windowFrame, Control.IsDefault);
            }

            // Always check if the BorderSize is not the default.If not, we need to paint with the BorderSize set by the user.
            if (hasCustomBorder)
            {
                if (Control.FlatAppearance.BorderSize != BORDERSIZE)
                {
                    DrawFlatBorderWithSize(e, r, colors.windowFrame, Control.FlatAppearance.BorderSize);
                }
                else
                {
                    ControlPaint.DrawBorderSimple(e, r, colors.windowFrame);
                }
            }
            else if (state == CheckState.Checked && SystemInformation.HighContrast)
            {
                ControlPaint.DrawBorderSimple(e, r, colors.windowFrame);
                ControlPaint.DrawBorderSimple(e, r, colors.buttonShadow);
            }
            else if (state == CheckState.Indeterminate)
            {
                Draw3DLiteBorder(e, r, colors, false);
            }
            else
            {
                ControlPaint.DrawBorderSimple(e, r, colors.windowFrame);
            }
        }

        internal override void PaintDown(PaintEventArgs e, CheckState state)
        {
            bool hasCustomBorder = (Control.FlatAppearance.BorderSize != BORDERSIZE || !Control.FlatAppearance.BorderColor.IsEmpty);

            ColorData colors = PaintFlatRender(e).Calculate();
            LayoutData layout = PaintFlatLayout(
                !Control.FlatAppearance.CheckedBackColor.IsEmpty
                    || (SystemInformation.HighContrast ? state != CheckState.Indeterminate : state == CheckState.Unchecked),
                !hasCustomBorder && SystemInformation.HighContrast && state == CheckState.Checked,
                Control.FlatAppearance.BorderSize).Layout();

            //Paint with the BorderColor if Set.
            if (!Control.FlatAppearance.BorderColor.IsEmpty)
            {
                colors.windowFrame = Control.FlatAppearance.BorderColor;
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
                        backColor = colors.options.HighContrast ? colors.buttonShadow : colors.lowHighlight;
                        break;
                    case CheckState.Indeterminate:
                        backColor = colors.buttonFace.MixColor(colors.options.HighContrast
                            ? colors.buttonShadow
                            : colors.lowHighlight);
                        break;
                }
            }

            PaintBackground(e, r, backColor);

            if (Control.IsDefault)
            {
                r.Inflate(-1, -1);
            }

            PaintImage(e, layout);
            PaintField(e, layout, colors, colors.windowText, false);

            if (Control.Focused && Control.ShowFocusCues)
            {
                DrawFlatFocus(e, layout.focus, colors.options.HighContrast ? colors.windowText : colors.constrastButtonShadow);
            }

            if (!(Control.IsDefault && Control.Focused && (Control.FlatAppearance.BorderSize == 0)))
            {
                DrawDefaultBorder(e, r, colors.windowFrame, Control.IsDefault);
            }

            //Always check if the BorderSize is not the default.If not, we need to paint with the BorderSize set by the user.
            if (hasCustomBorder)
            {
                if (Control.FlatAppearance.BorderSize != BORDERSIZE)
                {
                    DrawFlatBorderWithSize(e, r, colors.windowFrame, Control.FlatAppearance.BorderSize);
                }
                else
                {
                    ControlPaint.DrawBorderSimple(e, r, colors.windowFrame);
                }
            }
            else if (state == CheckState.Checked && SystemInformation.HighContrast)
            {
                ControlPaint.DrawBorderSimple(e, r, colors.windowFrame);
                ControlPaint.DrawBorderSimple(e, r, colors.buttonShadow);
            }
            else if (state == CheckState.Indeterminate)
            {
                Draw3DLiteBorder(e, r, colors, false);
            }
            else
            {
                ControlPaint.DrawBorderSimple(e, r, colors.windowFrame);
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
                bool hasCustomBorder = (Control.FlatAppearance.BorderSize != BORDERSIZE || !Control.FlatAppearance.BorderColor.IsEmpty);

                ColorData colors = PaintFlatRender(e).Calculate();
                LayoutData layout = PaintFlatLayout(
                    up: !Control.FlatAppearance.CheckedBackColor.IsEmpty || state == CheckState.Unchecked,
                    check: false,
                    Control.FlatAppearance.BorderSize).Layout();

                // Paint with the BorderColor if Set.
                if (!Control.FlatAppearance.BorderColor.IsEmpty)
                {
                    colors.windowFrame = Control.FlatAppearance.BorderColor;
                }

                Rectangle r = Control.ClientRectangle;

                Color backColor;
                if (!Control.FlatAppearance.MouseOverBackColor.IsEmpty)
                {
                    backColor = Control.FlatAppearance.MouseOverBackColor;
                }
                else if (!Control.FlatAppearance.CheckedBackColor.IsEmpty)
                {
                    backColor = state == CheckState.Checked || state == CheckState.Indeterminate
                        ? Control.FlatAppearance.CheckedBackColor.MixColor(colors.lowButtonFace)
                        : colors.lowButtonFace;
                }
                else
                {
                    backColor = state == CheckState.Indeterminate
                        ? colors.buttonFace.MixColor(colors.lowButtonFace)
                        : colors.lowButtonFace;
                }

                PaintBackground(e, r, IsHighContrastHighlighted() ? SystemColors.Highlight : backColor);

                if (Control.IsDefault)
                {
                    r.Inflate(-1, -1);
                }

                PaintImage(e, layout);
                PaintField(e, layout, colors, IsHighContrastHighlighted() ? SystemColors.HighlightText : colors.windowText, false);

                if (Control.Focused && Control.ShowFocusCues)
                {
                    DrawFlatFocus(e, layout.focus, colors.constrastButtonShadow);
                }

                if (!(Control.IsDefault && Control.Focused && (Control.FlatAppearance.BorderSize == 0)))
                {
                    DrawDefaultBorder(e, r, colors.windowFrame, Control.IsDefault);
                }

                // Always check if the BorderSize is not the default.If not, we need to paint with the BorderSize set by the user.
                if (hasCustomBorder)
                {
                    if (Control.FlatAppearance.BorderSize != BORDERSIZE)
                    {
                        DrawFlatBorderWithSize(e, r, colors.windowFrame, Control.FlatAppearance.BorderSize);
                    }
                    else
                    {
                        ControlPaint.DrawBorderSimple(e, r, colors.windowFrame);
                    }
                }
                else if (state == CheckState.Unchecked)
                {
                    ControlPaint.DrawBorderSimple(e, r, colors.windowFrame);
                }
                else
                {
                    Draw3DLiteBorder(e, r, colors, false);
                }
            }
        }

        protected override LayoutOptions Layout(PaintEventArgs e)
            => PaintFlatLayout(up: false, check: true, Control.FlatAppearance.BorderSize);

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
            layout.borderSize = borderSize + (check ? 1 : 0);
            layout.paddingSize = check ? 1 : 2;
            layout.focusOddEvenFixup = false;
            layout.textOffset = !up;
            layout.shadowedText = SystemInformation.HighContrast;

            return layout;
        }

        private LayoutOptions PaintFlatLayout(bool up, bool check, int borderSize)
        {
            LayoutOptions layout = CommonLayout();
            layout.borderSize = borderSize + (check ? 1 : 0);
            layout.paddingSize = check ? 1 : 2;
            layout.focusOddEvenFixup = false;
            layout.textOffset = !up;
            layout.shadowedText = SystemInformation.HighContrast;

            return layout;
        }
    }
}
