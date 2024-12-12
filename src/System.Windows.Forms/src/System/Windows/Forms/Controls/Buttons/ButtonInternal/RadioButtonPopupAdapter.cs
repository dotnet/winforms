// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ButtonInternal;

internal class RadioButtonPopupAdapter : RadioButtonFlatAdapter
{
    internal RadioButtonPopupAdapter(ButtonBase control) : base(control) { }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonPopupAdapter adapter = new(Control);
            adapter.PaintUp(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
        }
        else
        {
            ColorData colors = PaintPopupRender(e).Calculate();
            LayoutData layout = Layout(e).Layout();

            PaintButtonBackground(e, Control.ClientRectangle, background: null);

            PaintImage(e, layout);

            DrawCheckBackgroundFlat(
                e,
                layout.CheckBounds,
                colors.ButtonShadow,
                colors.Options.HighContrast ? colors.ButtonFace : colors.Highlight);

            DrawCheckOnly(e, layout, colors.WindowText, disabledColors: true);

            AdjustFocusRectangle(layout);
            PaintField(e, layout, colors, colors.WindowText, drawFocus: true);
        }
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonPopupAdapter adapter = new(Control);
            adapter.PaintOver(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
        }
        else
        {
            ColorData colors = PaintPopupRender(e).Calculate();
            LayoutData layout = Layout(e).Layout();

            PaintButtonBackground(e, Control.ClientRectangle, background: null);

            PaintImage(e, layout);

            Color checkBackgroundColor = colors.Options.HighContrast ? colors.ButtonFace : colors.Highlight;
            DrawCheckBackground3DLite(e, layout.CheckBounds, checkBackgroundColor, colors, disabledColors: true);
            DrawCheckOnly(e, layout, colors.WindowText, disabledColors: true);

            AdjustFocusRectangle(layout);
            PaintField(e, layout, colors, colors.WindowText, drawFocus: true);
        }
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        if (Control.Appearance == Appearance.Button)
        {
            ButtonPopupAdapter adapter = new(Control);
            adapter.PaintDown(e, Control.Checked ? CheckState.Checked : CheckState.Unchecked);
        }
        else
        {
            ColorData colors = PaintPopupRender(e).Calculate();
            LayoutData layout = Layout(e).Layout();

            PaintButtonBackground(e, Control.ClientRectangle, background: null);

            PaintImage(e, layout);

            DrawCheckBackground3DLite(e, layout.CheckBounds, colors.Highlight, colors, disabledColors: true);
            DrawCheckOnly(e, layout, colors.ButtonShadow, disabledColors: true);

            AdjustFocusRectangle(layout);
            PaintField(e, layout, colors, colors.WindowText, drawFocus: true);
        }
    }

    protected override ButtonBaseAdapter CreateButtonAdapter() => new ButtonPopupAdapter(Control);

    protected override LayoutOptions Layout(PaintEventArgs e)
    {
        LayoutOptions layout = base.Layout(e);

        if (!Control.MouseIsDown && !Control.MouseIsOver)
        {
            layout.ShadowedText = true;
        }

        return layout;
    }
}
