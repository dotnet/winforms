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

    /// <devdoc>
    ///     Common class for RadioButtonBaseAdapter and CheckBoxBaseAdapter
    /// </devdoc>
    internal abstract class CheckableControlBaseAdapter   : ButtonBaseAdapter {
        private const int standardCheckSize = 13;
        private ButtonBaseAdapter buttonAdapter;

        internal CheckableControlBaseAdapter(ButtonBase control) : base(control) {}

        protected ButtonBaseAdapter ButtonAdapter {
            get {
                if (buttonAdapter == null) {
                    buttonAdapter = CreateButtonAdapter();
                }
                return buttonAdapter;
            }
        }

        internal override Size GetPreferredSizeCore(Size proposedSize) {
            if (Appearance == Appearance.Button) {
                return ButtonAdapter.GetPreferredSizeCore(proposedSize);
            }

            using (Graphics measurementGraphics = WindowsFormsUtils.CreateMeasurementGraphics()) {
                using (PaintEventArgs pe = new PaintEventArgs(measurementGraphics, new Rectangle())) {
                    LayoutOptions options = Layout(pe);
                    return options.GetPreferredSizeCore(proposedSize);
                }
            }        
        }
        
        protected abstract ButtonBaseAdapter CreateButtonAdapter();

        private Appearance Appearance {
            get {
                CheckBox checkBox = Control as CheckBox;
                if(checkBox != null) {
                    return checkBox.Appearance;
                }

                RadioButton radioButton = Control as RadioButton;
                if(radioButton != null) {
                    return radioButton.Appearance;
                }

                Debug.Fail("Unexpected control type '" + Control.GetType().FullName + "'");
                return Appearance.Normal;
            }
        }

        internal override LayoutOptions CommonLayout() {
            LayoutOptions layout = base.CommonLayout();
            layout.growBorderBy1PxWhenDefault = false;
            layout.borderSize        = 0;
            layout.paddingSize       = 0;
            layout.maxFocus          = false;
            layout.focusOddEvenFixup = true;
            layout.checkSize         = standardCheckSize;
            return layout;
        }

        internal double GetDpiScaleRatio(Graphics g) {
            return GetDpiScaleRatio(g, Control);
        }

        internal static double GetDpiScaleRatio(Graphics g, Control control) {

            if (DpiHelper.IsPerMonitorV2Awareness
                && control != null && control.IsHandleCreated)
            {

                return control.deviceDpi / DpiHelper.LogicalDpi;
            }

            if (g == null)
                return 1.0F;
            return g.DpiX / 96;
        }

    }
}
