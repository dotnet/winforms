// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ButtonInternal {
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.Windows.Forms;
    using System.Windows.Forms.Layout;
    using System.Diagnostics.CodeAnalysis;
    
    internal class ButtonFlatAdapter : ButtonBaseAdapter {

        private const int BORDERSIZE = 1;

        internal ButtonFlatAdapter(ButtonBase control) : base(control) {}

        private void PaintBackground(PaintEventArgs e, Rectangle r, Color backColor) {
            Rectangle rect = r;
            rect.Inflate(-Control.FlatAppearance.BorderSize,-Control.FlatAppearance.BorderSize);
            Control.PaintBackground(e, rect, backColor, rect.Location);
        }

        internal override void PaintUp(PaintEventArgs e, CheckState state) {
            bool hasCustomBorder = (Control.FlatAppearance.BorderSize != BORDERSIZE || !Control.FlatAppearance.BorderColor.IsEmpty);

            ColorData colors = PaintFlatRender(e.Graphics).Calculate();
            LayoutData layout = PaintFlatLayout(e, 
                !Control.FlatAppearance.CheckedBackColor.IsEmpty || (SystemInformation.HighContrast ? state != CheckState.Indeterminate : state == CheckState.Unchecked),
                !hasCustomBorder && SystemInformation.HighContrast && state == CheckState.Checked,
                Control.FlatAppearance.BorderSize).Layout();

            //Paint with the BorderColor if Set.
            ///
            if (!Control.FlatAppearance.BorderColor.IsEmpty) {
                colors.windowFrame = Control.FlatAppearance.BorderColor;
            }
            
            Graphics g = e.Graphics;
            //Region original = g.Clip;

            Rectangle r = Control.ClientRectangle;

            Color backColor = Control.BackColor;

            if (!Control.FlatAppearance.CheckedBackColor.IsEmpty) {
                switch (state) {
                    case CheckState.Checked:
                        backColor = Control.FlatAppearance.CheckedBackColor;
                        break;
                    case CheckState.Indeterminate:
                        backColor = MixedColor(Control.FlatAppearance.CheckedBackColor, colors.buttonFace);
                        break;
                }
            }
            else {
                switch (state) {
                    case CheckState.Checked:
                        backColor = colors.highlight;
                        break;
                    case CheckState.Indeterminate:
                        backColor = MixedColor(colors.highlight, colors.buttonFace);
                        break;
                }
            }

            PaintBackground(e, r, IsHighContrastHighlighted2() ? SystemColors.Highlight : backColor);

            if (Control.IsDefault) {
                r.Inflate(-1, -1);
            }

            PaintImage(e, layout);
            PaintField(e, layout, colors, IsHighContrastHighlighted2() ? SystemColors.HighlightText : colors.windowText, false);

            if (Control.Focused && Control.ShowFocusCues) {
                DrawFlatFocus(g, layout.focus, colors.options.highContrast ? colors.windowText : colors.constrastButtonShadow);
            }


            if (!(Control.IsDefault && Control.Focused && (Control.FlatAppearance.BorderSize == 0))) {
                DrawDefaultBorder(g, r, colors.windowFrame, this.Control.IsDefault);
            }

            //Always check if the BorderSize is not the default.If not, we need to paint with the BorderSize set by the user.
            if (hasCustomBorder) {
                if (Control.FlatAppearance.BorderSize != BORDERSIZE) {
                    DrawFlatBorderWithSize(g, r, colors.windowFrame, Control.FlatAppearance.BorderSize); 
                }
                else {
                    DrawFlatBorder(g, r, colors.windowFrame);
                }
            }
            else if (state == CheckState.Checked && SystemInformation.HighContrast) {
                DrawFlatBorder(g, r, colors.windowFrame);
                DrawFlatBorder(g, r, colors.buttonShadow);
            }
            else if (state == CheckState.Indeterminate) {
                Draw3DLiteBorder(g, r, colors, false);
            }
            else {
                DrawFlatBorder(g, r, colors.windowFrame);
            }
        }

        internal override void PaintDown(PaintEventArgs e, CheckState state) {
            bool hasCustomBorder = (Control.FlatAppearance.BorderSize != BORDERSIZE || !Control.FlatAppearance.BorderColor.IsEmpty);

            ColorData colors = PaintFlatRender(e.Graphics).Calculate();
            LayoutData layout = PaintFlatLayout(e, 
                !Control.FlatAppearance.CheckedBackColor.IsEmpty || (SystemInformation.HighContrast ? state != CheckState.Indeterminate : state == CheckState.Unchecked),
                !hasCustomBorder && SystemInformation.HighContrast && state == CheckState.Checked,
                Control.FlatAppearance.BorderSize).Layout();

            //Paint with the BorderColor if Set.
            if (!Control.FlatAppearance.BorderColor.IsEmpty) {
                colors.windowFrame = Control.FlatAppearance.BorderColor;
            }
            
            Graphics g = e.Graphics;
            //Region original = g.Clip;

            Rectangle r = Control.ClientRectangle;

            Color backColor = Control.BackColor;

            if (!Control.FlatAppearance.MouseDownBackColor.IsEmpty) {
                backColor = Control.FlatAppearance.MouseDownBackColor;
            }
            else {
                switch (state) {
                    case CheckState.Unchecked:
                    case CheckState.Checked:
                        backColor = colors.options.highContrast ? colors.buttonShadow : colors.lowHighlight;
                        break;
                    case CheckState.Indeterminate:
                        backColor = MixedColor(colors.options.highContrast ? colors.buttonShadow : colors.lowHighlight, colors.buttonFace);
                        break;
                }
            }

            PaintBackground(e, r, backColor);

            if (Control.IsDefault) {
                r.Inflate(-1, -1);
            }

            PaintImage(e, layout);
            PaintField(e, layout, colors, colors.windowText, false);

            if (Control.Focused && Control.ShowFocusCues) {
                DrawFlatFocus(g, layout.focus, colors.options.highContrast ? colors.windowText : colors.constrastButtonShadow);
            }

            if (!(Control.IsDefault && Control.Focused && (Control.FlatAppearance.BorderSize == 0))) {
                DrawDefaultBorder(g, r, colors.windowFrame, this.Control.IsDefault);
            }

            //Always check if the BorderSize is not the default.If not, we need to paint with the BorderSize set by the user.
            if (hasCustomBorder) {
                if (Control.FlatAppearance.BorderSize != BORDERSIZE) {
                    DrawFlatBorderWithSize(g, r, colors.windowFrame, Control.FlatAppearance.BorderSize); 
                }
                else {
                    DrawFlatBorder(g, r, colors.windowFrame);
                }
            }
            else if (state == CheckState.Checked && SystemInformation.HighContrast) {
                DrawFlatBorder(g, r, colors.windowFrame);
                DrawFlatBorder(g, r, colors.buttonShadow);
            }
            else if (state == CheckState.Indeterminate) {
                Draw3DLiteBorder(g, r, colors, false);
            }
            else {
                DrawFlatBorder(g, r, colors.windowFrame);
            }
        }
        
        internal override void PaintOver(PaintEventArgs e, CheckState state) {
            if (SystemInformation.HighContrast) {
                PaintUp(e, state);
            }
            else {
                bool hasCustomBorder = (Control.FlatAppearance.BorderSize != BORDERSIZE || !Control.FlatAppearance.BorderColor.IsEmpty);

                ColorData colors = PaintFlatRender(e.Graphics).Calculate();
                LayoutData layout = PaintFlatLayout(e,
                    !Control.FlatAppearance.CheckedBackColor.IsEmpty || state == CheckState.Unchecked,
                    false,
                    Control.FlatAppearance.BorderSize).Layout();

                //Paint with the BorderColor if Set.
                ///
                if (!Control.FlatAppearance.BorderColor.IsEmpty) {
                    colors.windowFrame = Control.FlatAppearance.BorderColor;
                }
                
                Graphics g = e.Graphics;
                //Region original = g.Clip;

                Rectangle r = Control.ClientRectangle;

                Color backColor = Control.BackColor;

                if (!Control.FlatAppearance.MouseOverBackColor.IsEmpty) {
                    backColor = Control.FlatAppearance.MouseOverBackColor;
                }
                else if (!Control.FlatAppearance.CheckedBackColor.IsEmpty) {
                    if (state == CheckState.Checked || state == CheckState.Indeterminate) {
                        backColor = MixedColor(Control.FlatAppearance.CheckedBackColor, colors.lowButtonFace);
                    }
                    else {
                        backColor = colors.lowButtonFace;
                    }
                }
                else {
                    if (state == CheckState.Indeterminate) {
                        backColor = MixedColor(colors.buttonFace, colors.lowButtonFace);
                    }
                    else {
                        backColor = colors.lowButtonFace;
                    }
                }

                PaintBackground(e, r, IsHighContrastHighlighted2() ? SystemColors.Highlight : backColor);

                if (Control.IsDefault) {
                    r.Inflate(-1, -1);
                }

                PaintImage(e, layout);
                PaintField(e, layout, colors, IsHighContrastHighlighted2() ? SystemColors.HighlightText : colors.windowText, false);

                if (Control.Focused && Control.ShowFocusCues) {
                    DrawFlatFocus(g, layout.focus, colors.constrastButtonShadow);
                }

                if (!(Control.IsDefault && Control.Focused && (Control.FlatAppearance.BorderSize == 0))) {
                    DrawDefaultBorder(g, r, colors.windowFrame, this.Control.IsDefault);
                }

                //Always check if the BorderSize is not the default.If not, we need to paint with the BorderSize set by the user.
                if (hasCustomBorder) {
                    if (Control.FlatAppearance.BorderSize != BORDERSIZE) {
                        DrawFlatBorderWithSize(g, r, colors.windowFrame, Control.FlatAppearance.BorderSize); 
                    }
                    else {
                        DrawFlatBorder(g, r, colors.windowFrame);
                    }
                }
                else if (state == CheckState.Unchecked) {
                    DrawFlatBorder(g, r, colors.windowFrame);
                }
                else {
                    Draw3DLiteBorder(g, r, colors, false);
                }
            }
        }


        #region LayoutData

        protected override LayoutOptions Layout(PaintEventArgs e) {
            LayoutOptions layout = PaintFlatLayout(e, /* up = */ false, /* check = */ true, Control.FlatAppearance.BorderSize);
#if DEBUG
            Size prefSize = layout.GetPreferredSizeCore(LayoutUtils.MaxSize);
            Debug.Assert(
                prefSize == PaintFlatLayout(e, /* up = */ false, /* check = */ false, Control.FlatAppearance.BorderSize).GetPreferredSizeCore(LayoutUtils.MaxSize) &&
                prefSize == PaintFlatLayout(e, /* up = */ true, /* check = */ false, Control.FlatAppearance.BorderSize).GetPreferredSizeCore(LayoutUtils.MaxSize) &&
                prefSize == PaintFlatLayout(e, /* up = */ true, /* check = */ true, Control.FlatAppearance.BorderSize).GetPreferredSizeCore(LayoutUtils.MaxSize),
                "The state of up and check should not effect PreferredSize");
#endif
            return layout;
        }
        

        // used by DataGridViewButtonCell        
        [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]  // removed graphics, may have to put it back
        internal static LayoutOptions PaintFlatLayout(Graphics g, bool up, bool check, int borderSize, Rectangle clientRectangle, Padding padding,
                                                      bool isDefault, Font font, string text, bool enabled, ContentAlignment textAlign, RightToLeft rtl)
        {
            LayoutOptions layout = CommonLayout(clientRectangle, padding, isDefault, font, text, enabled, textAlign, rtl);
            layout.borderSize        = borderSize + (check ? 1 : 0);
            layout.paddingSize       = check ? 1 : 2;
            layout.focusOddEvenFixup = false;
            layout.textOffset        = !up;
            layout.shadowedText      = SystemInformation.HighContrast;

            return layout;
        }


        [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]  // removed graphics, may have to put it back
        private LayoutOptions PaintFlatLayout(PaintEventArgs e, bool up, bool check, int borderSize) {
            LayoutOptions layout = CommonLayout();
            layout.borderSize        = borderSize + (check ? 1 : 0);
            layout.paddingSize       = check ? 1 : 2;
            layout.focusOddEvenFixup = false;
            layout.textOffset        = !up;
            layout.shadowedText      = SystemInformation.HighContrast;

            return layout;
        }

        #endregion
    }
}
