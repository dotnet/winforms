// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ButtonInternal;

internal class RadioButtonStandardAdapter : RadioButtonBaseAdapter
{
    internal RadioButtonStandardAdapter(ButtonBase control) : base(control) { }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonAdapter.PaintUp(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
        }
        else
        {
            ColorData colors = PaintRender(e).Calculate();
            LayoutData layout = Layout(e).Layout();
            PaintButtonBackground(e, Control.ClientRectangle, background: null);

            PaintImage(e, layout);
            DrawCheckBox(e, layout);
            AdjustFocusRectangle(layout);
            PaintField(e, layout, colors, colors.WindowText, drawFocus: true);
        }
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonAdapter.PaintDown(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
        }
        else
        {
            PaintUp(e, state);
        }
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonAdapter.PaintOver(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
        }
        else
        {
            PaintUp(e, state);
        }
    }

    private new ButtonStandardAdapter ButtonAdapter => (ButtonStandardAdapter)base.ButtonAdapter;

    protected override ButtonBaseAdapter CreateButtonAdapter() => new ButtonStandardAdapter(Control);

    protected override LayoutOptions Layout(PaintEventArgs e)
    {
        LayoutOptions layout = CommonLayout();
        layout.HintTextUp = false;
        layout.DotNetOneButtonCompat = !Application.RenderWithVisualStyles;

        if (Application.RenderWithVisualStyles)
        {
            ButtonBase b = Control;
            using var screen = GdiCache.GetScreenHdc();
            layout.CheckSize = RadioButtonRenderer.GetGlyphSize(
                screen,
                RadioButtonRenderer.ConvertFromButtonState(GetState(), b.MouseIsOver),
                b.HWNDInternal).Width;
        }
        else
        {
            layout.CheckSize = (int)(layout.CheckSize * GetDpiScaleRatio());
        }

        return layout;
    }
}
