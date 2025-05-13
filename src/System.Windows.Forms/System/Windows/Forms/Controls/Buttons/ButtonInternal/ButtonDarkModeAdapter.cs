// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.ButtonInternal;

internal class ButtonDarkModeAdapter : ButtonBaseAdapter
{
    internal ButtonDarkModeAdapter(ButtonBase control) : base(control) { }

    private IButtonDarkModeRenderer GetRenderer()
        => ButtonDarkModeRendererFactory.GetRenderer(Control.FlatStyle);

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        bool isDefault = Control.IsDefault;
        var renderer = GetRenderer();

        Rectangle bounds = Control.ClientRectangle;
        // Draw button background and get content bounds
        Rectangle contentBounds = renderer.DrawButtonBackground(e.Graphics, bounds, ToPushButtonState(state, Control.Enabled), isDefault);

        LayoutData layout = CommonLayout().Layout();
        PaintImage(e, layout);
        PaintField(
            e,
            layout,
            PaintDarkModeRender(e).Calculate(),
            renderer.GetTextColor(ToPushButtonState(state, Control.Enabled), isDefault),
            drawFocus: false);

        if (Control.Focused && Control.ShowFocusCues)
        {
            renderer.DrawFocusIndicator(e.Graphics, contentBounds, isDefault);
        }
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        bool isDefault = Control.IsDefault;
        var renderer = GetRenderer();

        Rectangle bounds = Control.ClientRectangle;
        Rectangle contentBounds = renderer.DrawButtonBackground(e.Graphics, bounds, PushButtonState.Pressed, isDefault);

        LayoutData layout = CommonLayout().Layout();
        PaintImage(e, layout);
        PaintField(
            e,
            layout,
            PaintDarkModeRender(e).Calculate(),
            renderer.GetTextColor(PushButtonState.Pressed, isDefault),
            drawFocus: false);

        if (Control.Focused && Control.ShowFocusCues)
        {
            renderer.DrawFocusIndicator(e.Graphics, contentBounds, isDefault);
        }
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        bool isDefault = Control.IsDefault;
        var renderer = GetRenderer();

        Rectangle bounds = Control.ClientRectangle;
        Rectangle contentBounds = renderer.DrawButtonBackground(e.Graphics, bounds, PushButtonState.Hot, isDefault);

        LayoutData layout = CommonLayout().Layout();
        PaintImage(e, layout);
        PaintField(
            e,
            layout,
            PaintDarkModeRender(e).Calculate(),
            renderer.GetTextColor(PushButtonState.Hot, isDefault),
            drawFocus: false);

        if (Control.Focused && Control.ShowFocusCues)
        {
            renderer.DrawFocusIndicator(e.Graphics, contentBounds, isDefault);
        }
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
        if (!enabled)
            return PushButtonState.Disabled;
        return state switch
        {
            CheckState.Unchecked => PushButtonState.Normal,
            CheckState.Checked => PushButtonState.Pressed,
            CheckState.Indeterminate => PushButtonState.Hot,
            _ => PushButtonState.Normal
        };
    }
}
