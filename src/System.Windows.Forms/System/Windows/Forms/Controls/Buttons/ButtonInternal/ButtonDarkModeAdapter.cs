// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.ButtonInternal;

internal class ButtonDarkModeAdapter : ButtonBaseAdapter
{
    internal ButtonDarkModeAdapter(ButtonBase control) : base(control) { }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        LayoutData layout = CommonLayout().Layout();
        ButtonDarkModeRenderer.RenderButton(
            e.Graphics,
            Control.ClientRectangle,
            Control.FlatStyle,
            ToPushButtonState(state, Control.Enabled),
            Control.IsDefault,
            Control.Focused,
            Control.ShowFocusCues,
            _ => PaintImage(e, layout),
            (_, textColor, drawFocus) => PaintField(
                e,
                layout,
                PaintDarkModeRender(e).Calculate(),
                textColor,
                drawFocus: false)
        );
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        LayoutData layout = CommonLayout().Layout();
        ButtonDarkModeRenderer.RenderButton(
            e.Graphics,
            Control.ClientRectangle,
            Control.FlatStyle,
            PushButtonState.Pressed,
            Control.IsDefault,
            Control.Focused,
            Control.ShowFocusCues,
            _ => PaintImage(e, layout),
            (_, textColor, drawFocus) => PaintField(
                e,
                layout,
                PaintDarkModeRender(e).Calculate(),
                textColor,
                drawFocus: false)
        );
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        LayoutData layout = CommonLayout().Layout();
        ButtonDarkModeRenderer.RenderButton(
            e.Graphics,
            Control.ClientRectangle,
            Control.FlatStyle,
            PushButtonState.Hot,
            Control.IsDefault,
            Control.Focused,
            Control.ShowFocusCues,
            _ => PaintImage(e, layout),
            (_, textColor, drawFocus) => PaintField(
                e,
                layout,
                PaintDarkModeRender(e).Calculate(),
                textColor,
                drawFocus: false)
        );
    }

    protected override LayoutOptions Layout(PaintEventArgs e) => CommonLayout();

    private new LayoutOptions CommonLayout()
    {
        LayoutOptions layout = base.CommonLayout();
        layout.FocusOddEvenFixup = false;
        layout.ShadowedText = false;

        return layout;
    }

    private ColorOptions PaintDarkModeRender(IDeviceContext deviceContext) =>
        new(deviceContext, Control.ForeColor, Control.BackColor)
        {
            Enabled = Control.Enabled
        };

    private static PushButtonState ToPushButtonState(CheckState state, bool enabled)
    {
        return !enabled
            ? PushButtonState.Disabled
            : state switch
        {
            CheckState.Unchecked => PushButtonState.Normal,
            CheckState.Checked => PushButtonState.Pressed,
            CheckState.Indeterminate => PushButtonState.Hot,
            _ => PushButtonState.Normal
        };
    }
}
