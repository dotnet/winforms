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

    internal class RadioButtonStandardAdapter : RadioButtonBaseAdapter {

        internal RadioButtonStandardAdapter(ButtonBase control) : base(control) {}

        internal override void PaintUp(PaintEventArgs e, CheckState state) {
            if (Control.Appearance == Appearance.Button) {
                ButtonAdapter.PaintUp(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
            }
            else {
                ColorData colors = PaintRender(e.Graphics).Calculate();
                LayoutData layout = Layout(e).Layout();
                PaintButtonBackground(e, Control.ClientRectangle, null);

                PaintImage(e, layout);
                DrawCheckBox(e, layout);
                AdjustFocusRectangle(layout);
                PaintField(e, layout, colors, colors.windowText, true);
            }
        }

        internal override void PaintDown(PaintEventArgs e, CheckState state) {
            if (Control.Appearance == Appearance.Button) {
                ButtonAdapter.PaintDown(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
            }
            else {
                PaintUp(e, state);
            }
        }

        internal override void PaintOver(PaintEventArgs e, CheckState state) {
            if (Control.Appearance == Appearance.Button) {
                ButtonAdapter.PaintOver(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
            }
            else {
                PaintUp(e, state);
            }
        }        

        private new ButtonStandardAdapter ButtonAdapter {
            get {
                return ((ButtonStandardAdapter)base.ButtonAdapter);
            }
        }

        protected override ButtonBaseAdapter CreateButtonAdapter() {
            return new ButtonStandardAdapter(Control);
        }

        #region Temp

        protected override LayoutOptions Layout(PaintEventArgs e) {
            LayoutOptions layout = CommonLayout();
            layout.hintTextUp        = false;
            layout.everettButtonCompat = !Application.RenderWithVisualStyles;            

            if (Application.RenderWithVisualStyles) {
                ButtonBase b = Control; 
                using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics()) {
                    layout.checkSize = RadioButtonRenderer.GetGlyphSize(g, RadioButtonRenderer.ConvertFromButtonState(GetState(), b.MouseIsOver), b.HandleInternal).Width;
                }
            }
            else {
                layout.checkSize = (int)(layout.checkSize * GetDpiScaleRatio(e.Graphics));
            }
            
            return layout;
        }

        #endregion
    }
}
