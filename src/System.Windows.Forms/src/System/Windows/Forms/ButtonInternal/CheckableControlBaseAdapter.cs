// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.ButtonInternal
{
    /// <summary>
    ///  Common class for RadioButtonBaseAdapter and CheckBoxBaseAdapter
    /// </summary>
    internal abstract class CheckableControlBaseAdapter : ButtonBaseAdapter
    {
        private const int standardCheckSize = 13;
        private ButtonBaseAdapter buttonAdapter;

        internal CheckableControlBaseAdapter(ButtonBase control) : base(control) { }

        protected ButtonBaseAdapter ButtonAdapter
        {
            get
            {
                if (buttonAdapter == null)
                {
                    buttonAdapter = CreateButtonAdapter();
                }
                return buttonAdapter;
            }
        }

        internal override Size GetPreferredSizeCore(Size proposedSize)
        {
            if (Appearance == Appearance.Button)
            {
                return ButtonAdapter.GetPreferredSizeCore(proposedSize);
            }

            using (Graphics measurementGraphics = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                using (PaintEventArgs pe = new PaintEventArgs(measurementGraphics, new Rectangle()))
                {
                    LayoutOptions options = Layout(pe);
                    return options.GetPreferredSizeCore(proposedSize);
                }
            }
        }

        protected abstract ButtonBaseAdapter CreateButtonAdapter();

        private Appearance Appearance
        {
            get
            {
                if (Control is CheckBox checkBox)
                {
                    return checkBox.Appearance;
                }

                if (Control is RadioButton radioButton)
                {
                    return radioButton.Appearance;
                }

                Debug.Fail("Unexpected control type '" + Control.GetType().FullName + "'");
                return Appearance.Normal;
            }
        }

        internal override LayoutOptions CommonLayout()
        {
            LayoutOptions layout = base.CommonLayout();
            layout.growBorderBy1PxWhenDefault = false;
            layout.borderSize = 0;
            layout.paddingSize = 0;
            layout.maxFocus = false;
            layout.focusOddEvenFixup = true;
            layout.checkSize = standardCheckSize;
            return layout;
        }

        internal double GetDpiScaleRatio(Graphics g)
        {
            return GetDpiScaleRatio(g, Control);
        }

        internal static double GetDpiScaleRatio(Graphics g, Control control)
        {
            if (DpiHelper.IsPerMonitorV2Awareness
                && control != null && control.IsHandleCreated)
            {
                return control._deviceDpi / DpiHelper.LogicalDpi;
            }

            if (g == null)
            {
                return 1.0F;
            }

            return g.DpiX / 96;
        }

    }
}
