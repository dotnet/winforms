// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.ButtonInternal;

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
            PaintFlatWorker(e, colors.WindowText, colors.Highlight, colors.WindowFrame, colors);
        }
        else
        {
            PaintFlatWorker(e, colors.ButtonShadow, colors.ButtonFace, colors.ButtonShadow, colors);
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
            PaintFlatWorker(e, colors.WindowText, colors.LowHighlight, colors.WindowFrame, colors);
        }
        else
        {
            PaintFlatWorker(e, colors.ButtonShadow, colors.ButtonFace, colors.ButtonShadow, colors);
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
            PaintFlatWorker(e, colors.WindowText, colors.Highlight, colors.WindowFrame, colors);
        }
        else
        {
            PaintFlatWorker(e, colors.ButtonShadow, colors.ButtonFace, colors.ButtonShadow, colors);
        }
    }

    private void PaintFlatWorker(PaintEventArgs e, Color checkColor, Color checkBackground, Color checkBorder, ColorData colors)
    {
        LayoutData layout = Layout(e).Layout();
        PaintButtonBackground(e, Control.ClientRectangle, null);

        PaintImage(e, layout);
        DrawCheckFlat(e, layout, checkColor, colors.Options.HighContrast ? colors.ButtonFace : checkBackground, checkBorder);
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
        layout.CheckSize = (int)(FlatCheckSize * GetDpiScaleRatio());
        layout.ShadowedText = false;

        return layout;
    }
}
