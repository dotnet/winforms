// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.ButtonInternal
{
    internal class RadioButtonFlatAdapter : RadioButtonBaseAdapter
    {
        protected const int FlatCheckSize = 12;

        internal RadioButtonFlatAdapter(ButtonBase control) : base(control) { }

        internal override void PaintDown(PaintEventArgs e, CheckState state)
        {
            if (Control.Appearance == Appearance.Button)
            {
                ButtonFlatAdapter adapter = new ButtonFlatAdapter(Control);
                adapter.PaintDown(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
                return;
            }

            ColorData colors = PaintFlatRender(e).Calculate();
            if (Control.Enabled)
            {
                PaintFlatWorker(e, colors.windowText, colors.highlight, colors.windowFrame, colors);
            }
            else
            {
                PaintFlatWorker(e, colors.buttonShadow, colors.buttonFace, colors.buttonShadow, colors);
            }
        }

        internal override void PaintOver(PaintEventArgs e, CheckState state)
        {
            if (Control.Appearance == Appearance.Button)
            {
                ButtonFlatAdapter adapter = new ButtonFlatAdapter(Control);
                adapter.PaintOver(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
                return;
            }

            ColorData colors = PaintFlatRender(e).Calculate();
            if (Control.Enabled)
            {
                PaintFlatWorker(e, colors.windowText, colors.lowHighlight, colors.windowFrame, colors);
            }
            else
            {
                PaintFlatWorker(e, colors.buttonShadow, colors.buttonFace, colors.buttonShadow, colors);
            }
        }

        internal override void PaintUp(PaintEventArgs e, CheckState state)
        {
            if (Control.Appearance == Appearance.Button)
            {
                ButtonFlatAdapter adapter = new ButtonFlatAdapter(Control);
                adapter.PaintUp(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
                return;
            }

            ColorData colors = PaintFlatRender(e).Calculate();
            if (Control.Enabled)
            {
                PaintFlatWorker(e, colors.windowText, colors.highlight, colors.windowFrame, colors);
            }
            else
            {
                PaintFlatWorker(e, colors.buttonShadow, colors.buttonFace, colors.buttonShadow, colors);
            }
        }

        void PaintFlatWorker(PaintEventArgs e, Color checkColor, Color checkBackground, Color checkBorder, ColorData colors)
        {
            LayoutData layout = Layout(e).Layout();
            PaintButtonBackground(e, Control.ClientRectangle, null);

            PaintImage(e, layout);
            DrawCheckFlat(e, layout, checkColor, colors.options.HighContrast ? colors.buttonFace : checkBackground, checkBorder);
            AdjustFocusRectangle(layout);
            PaintField(e, layout, colors, checkColor, true);
        }

        protected override ButtonBaseAdapter CreateButtonAdapter()
        {
            return new ButtonFlatAdapter(Control);
        }

        // RadioButtonPopupLayout also uses this layout for down and over
        protected override LayoutOptions Layout(PaintEventArgs e)
        {
            LayoutOptions layout = CommonLayout();
            layout.checkSize = (int)(FlatCheckSize * GetDpiScaleRatio());
            layout.shadowedText = false;

            return layout;
        }
    }
}
