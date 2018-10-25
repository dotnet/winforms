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
        

    internal class ButtonPopupAdapter : ButtonBaseAdapter {

        internal ButtonPopupAdapter(ButtonBase control) : base(control) {}

        internal override void PaintUp(PaintEventArgs e, CheckState state) {
            ColorData colors = PaintPopupRender(e.Graphics).Calculate();
            LayoutData layout = PaintPopupLayout(e, state == CheckState.Unchecked, 1).Layout();

            Graphics g = e.Graphics;

            Rectangle r = Control.ClientRectangle;

            if (state == CheckState.Indeterminate) {
                Brush backbrush = CreateDitherBrush(colors.highlight, colors.buttonFace);
                try {
                    PaintButtonBackground(e, r, backbrush);
                }
                finally {
                    backbrush.Dispose();
                    backbrush = null;
                }
            }
            else {
                Control.PaintBackground(e, r, IsHighContrastHighlighted2() ? SystemColors.Highlight : Control.BackColor, r.Location);
            }

            if (Control.IsDefault) {
                r.Inflate(-1, -1);
            }

            PaintImage(e, layout);
            PaintField(e, layout, colors, state != CheckState.Indeterminate && IsHighContrastHighlighted2() ? SystemColors.HighlightText : colors.windowText, true);

            DrawDefaultBorder(g, r, colors.options.highContrast ? colors.windowText : colors.buttonShadow, this.Control.IsDefault);

            if (state == CheckState.Unchecked) {
                DrawFlatBorder(g, r, colors.options.highContrast ? colors.windowText : colors.buttonShadow);
            }
            else {
                Draw3DLiteBorder(g, r, colors, false);
            }
        }

        internal override void PaintOver(PaintEventArgs e, CheckState state) {
            ColorData colors = PaintPopupRender(e.Graphics).Calculate();
            LayoutData layout = PaintPopupLayout(e, state == CheckState.Unchecked, SystemInformation.HighContrast ? 2 : 1).Layout();

            Graphics g = e.Graphics;
            //Region original = g.Clip;

            Rectangle r = Control.ClientRectangle;

            if (state == CheckState.Indeterminate) {
                Brush backbrush = CreateDitherBrush(colors.highlight, colors.buttonFace);
                try {
                    PaintButtonBackground(e, r, backbrush);
                }
                finally {
                    backbrush.Dispose();
                    backbrush = null;
                }
            }
            else {
                Control.PaintBackground(e, r, IsHighContrastHighlighted2() ? SystemColors.Highlight : Control.BackColor, r.Location);
            }

            if (Control.IsDefault) {
                r.Inflate(-1, -1);
            }

            PaintImage(e, layout);
            PaintField(e, layout, colors, IsHighContrastHighlighted2() ? SystemColors.HighlightText : colors.windowText, true);

            DrawDefaultBorder(g, r, colors.options.highContrast ? colors.windowText : colors.buttonShadow, this.Control.IsDefault);

            if (SystemInformation.HighContrast) {
                using (Pen windowFrame = new Pen(colors.windowFrame),
                       highlight = new Pen(colors.highlight),
                       buttonShadow = new Pen(colors.buttonShadow)) {

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
                }

                r.Inflate(-2, -2);
            }
            else {
                Draw3DLiteBorder(g, r, colors, true);
            }
        }

        internal override void PaintDown(PaintEventArgs e, CheckState state) {
            ColorData colors = PaintPopupRender(e.Graphics).Calculate();
            LayoutData layout = PaintPopupLayout(e, false, SystemInformation.HighContrast ? 2 : 1).Layout();

            Graphics g = e.Graphics;
            //Region original = g.Clip;

            Rectangle r = Control.ClientRectangle;
            PaintButtonBackground(e, r, null);
            if (Control.IsDefault) {
                r.Inflate(-1, -1);
            }
            r.Inflate(-1, -1);

            PaintImage(e, layout);
            PaintField(e, layout, colors, colors.windowText, true);
            
            r.Inflate(1, 1);
            DrawDefaultBorder(g, r, colors.options.highContrast ? colors.windowText : colors.windowFrame, this.Control.IsDefault);
            ControlPaint.DrawBorder(g, r, colors.options.highContrast ? colors.windowText : colors.buttonShadow, ButtonBorderStyle.Solid);
        }

        #region Layout

        protected override LayoutOptions Layout(PaintEventArgs e) {
            LayoutOptions layout = PaintPopupLayout(e, /* up = */ false, 0);
            Debug.Assert(layout.GetPreferredSizeCore(LayoutUtils.MaxSize)
                == PaintPopupLayout(e, /* up = */ true, 2).GetPreferredSizeCore(LayoutUtils.MaxSize),
                "The state of up should not effect PreferredSize");
            return layout;
        }


        // used by the DataGridViewButtonCell
        [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]  // removed graphics, may have to put it back
        internal static LayoutOptions PaintPopupLayout(Graphics g, bool up, int paintedBorder, Rectangle clientRectangle, Padding padding,
                                                       bool isDefault, Font font, string text, bool enabled, ContentAlignment textAlign, RightToLeft rtl)
        {
            LayoutOptions layout = CommonLayout(clientRectangle, padding, isDefault, font, text, enabled, textAlign, rtl);
            layout.borderSize        = paintedBorder;
            layout.paddingSize       = 2 - paintedBorder;
            layout.hintTextUp        = false;
            layout.textOffset        = !up;
            layout.shadowedText      = SystemInformation.HighContrast;

            Debug.Assert(layout.borderSize + layout.paddingSize == 2,
                "It is assemed borderSize + paddingSize will always be 2. Bad value for paintedBorder?");

            return layout;
        }


        [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]  // removed graphics, may have to put it back
        private LayoutOptions PaintPopupLayout(PaintEventArgs e, bool up, int paintedBorder) {

            LayoutOptions layout = CommonLayout();
            layout.borderSize        = paintedBorder;
            layout.paddingSize       = 2 - paintedBorder;//3 - paintedBorder - (Control.IsDefault ? 1 : 0);
            layout.hintTextUp        = false;
            layout.textOffset        = !up;
            layout.shadowedText      = SystemInformation.HighContrast;

            Debug.Assert(layout.borderSize + layout.paddingSize == 2,
                "borderSize + paddingSize will always be 2. Bad value for paintedBorder?");

            return layout;
        }

        #endregion
    }
}
