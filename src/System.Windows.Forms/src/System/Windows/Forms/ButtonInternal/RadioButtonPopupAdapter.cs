// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.ButtonInternal
{
    internal class RadioButtonPopupAdapter : RadioButtonFlatAdapter
    {
        internal RadioButtonPopupAdapter(ButtonBase control) : base(control) { }

        internal override void PaintUp(PaintEventArgs e, CheckState state)
        {
            if (Control.Appearance == Appearance.Button)
            {
                ButtonPopupAdapter adapter = new ButtonPopupAdapter(Control);
                adapter.PaintUp(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
            }
            else
            {
                ColorData colors = PaintPopupRender(e).Calculate();
                LayoutData layout = Layout(e).Layout();

                PaintButtonBackground(e, Control.ClientRectangle, null);

                PaintImage(e, layout);

                DrawCheckBackgroundFlat(
                    e,
                    layout.checkBounds,
                    colors.buttonShadow,
                    colors.options.HighContrast ? colors.buttonFace : colors.highlight);

                DrawCheckOnly(e, layout, colors.windowText, colors.highlight, true);

                AdjustFocusRectangle(layout);
                PaintField(e, layout, colors, colors.windowText, true);
            }
        }

        internal override void PaintOver(PaintEventArgs e, CheckState state)
        {
            if (Control.Appearance == Appearance.Button)
            {
                ButtonPopupAdapter adapter = new ButtonPopupAdapter(Control);
                adapter.PaintOver(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
            }
            else
            {
                ColorData colors = PaintPopupRender(e).Calculate();
                LayoutData layout = Layout(e).Layout();

                PaintButtonBackground(e, Control.ClientRectangle, null);

                PaintImage(e, layout);

                Color checkBackgroundColor = colors.options.HighContrast ? colors.buttonFace : colors.highlight;
                DrawCheckBackground3DLite(e, layout.checkBounds, checkBackgroundColor, colors, true);
                DrawCheckOnly(e, layout, colors.windowText, colors.highlight, true);

                AdjustFocusRectangle(layout);
                PaintField(e, layout, colors, colors.windowText, true);
            }
        }

        internal override void PaintDown(PaintEventArgs e, CheckState state)
        {
            if (Control.Appearance == Appearance.Button)
            {
                ButtonPopupAdapter adapter = new ButtonPopupAdapter(Control);
                adapter.PaintDown(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
            }
            else
            {
                ColorData colors = PaintPopupRender(e).Calculate();
                LayoutData layout = Layout(e).Layout();

                PaintButtonBackground(e, Control.ClientRectangle, null);

                PaintImage(e, layout);

                DrawCheckBackground3DLite(e, layout.checkBounds, colors.highlight, colors, true);
                DrawCheckOnly(e, layout, colors.buttonShadow, colors.highlight, true);

                AdjustFocusRectangle(layout);
                PaintField(e, layout, colors, colors.windowText, true);
            }
        }

        #region Layout

        protected override ButtonBaseAdapter CreateButtonAdapter()
        {
            return new ButtonPopupAdapter(Control);
        }

        protected override LayoutOptions Layout(PaintEventArgs e)
        {
            LayoutOptions layout = base.Layout(e);

            if (!Control.MouseIsDown && !Control.MouseIsOver)
            {
                layout.shadowedText = true;
            }

            return layout;
        }

        #endregion
    }
}
