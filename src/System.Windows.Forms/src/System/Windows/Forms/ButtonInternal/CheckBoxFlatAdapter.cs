// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ButtonInternal {
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.Windows.Forms;

    internal class CheckBoxFlatAdapter : CheckBoxBaseAdapter {

        internal CheckBoxFlatAdapter(ButtonBase control) : base(control) {}
        
        internal override void PaintDown(PaintEventArgs e, CheckState state) {
            if (Control.Appearance == Appearance.Button) {
                ButtonAdapter.PaintDown(e, Control.CheckState);
                return;
            }

            ColorData colors = PaintFlatRender(e.Graphics).Calculate();
            if (Control.Enabled) {
                PaintFlatWorker(e, colors.windowText, colors.highlight, colors.windowFrame, colors);
            }
            else {
                PaintFlatWorker(e, colors.buttonShadow, colors.buttonFace, colors.buttonShadow, colors);
            }
        } 
        
        internal override void PaintOver(PaintEventArgs e, CheckState state) {
            if (Control.Appearance == Appearance.Button) {
                ButtonAdapter.PaintOver(e, Control.CheckState);
                return;
            }

            ColorData colors = PaintFlatRender(e.Graphics).Calculate();
            if (Control.Enabled) {
                PaintFlatWorker(e, colors.windowText, colors.lowHighlight, colors.windowFrame, colors);
            }
            else {
                PaintFlatWorker(e, colors.buttonShadow, colors.buttonFace, colors.buttonShadow, colors);
            }
        }
        
        internal override void PaintUp(PaintEventArgs e, CheckState state) {
            if (Control.Appearance == Appearance.Button) {
                ButtonAdapter.PaintUp(e, Control.CheckState);
                return;
            }

            ColorData colors = PaintFlatRender(e.Graphics).Calculate();
            if (Control.Enabled) {
                PaintFlatWorker(e, colors.windowText, colors.highlight, colors.windowFrame, colors);
            }
            else {
                PaintFlatWorker(e, colors.buttonShadow, colors.buttonFace, colors.buttonShadow, colors);
            }
        }
        
        private void PaintFlatWorker(PaintEventArgs e, Color checkColor, Color checkBackground, Color checkBorder, ColorData colors) {
            System.Drawing.Graphics g = e.Graphics;
            LayoutData layout = Layout(e).Layout();
            PaintButtonBackground(e, Control.ClientRectangle, null);

            PaintImage(e, layout);
            DrawCheckFlat(e, layout, checkColor, colors.options.highContrast ? colors.buttonFace : checkBackground, checkBorder, colors);
            AdjustFocusRectangle(layout);
            PaintField(e, layout, colors, checkColor, true);
        }

        #region Layout

        private new ButtonFlatAdapter ButtonAdapter {
            get {
                return ((ButtonFlatAdapter)base.ButtonAdapter);
            }
        }

        protected override ButtonBaseAdapter CreateButtonAdapter() {
            return new ButtonFlatAdapter(Control);
        }

        protected override LayoutOptions Layout(PaintEventArgs e) {
            LayoutOptions layout = CommonLayout();
            layout.checkSize         = (int)(flatCheckSize * GetDpiScaleRatio(e.Graphics));
            layout.shadowedText      = false;

            return layout;
        }

        #endregion
    }
}
