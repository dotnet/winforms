// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.ButtonInternal;

internal class ButtonDarkModeAdapter : ButtonBaseAdapter
{
    // Magic numbers for PushButtonState mapping
    private const PushButtonState DisabledPushButtonState = PushButtonState.Disabled;
    private const PushButtonState NormalPushButtonState = PushButtonState.Normal;
    private const PushButtonState PressedPushButtonState = PushButtonState.Pressed;
    private const PushButtonState HotPushButtonState = PushButtonState.Hot;

    internal ButtonDarkModeAdapter(ButtonBase control) : base(control) { }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        var smoothingMode = e.Graphics.SmoothingMode;
        e.Graphics.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias;

        LayoutData layout = CommonLayout().Layout();

        ButtonDarkModeRenderer.RenderButton(
            e.Graphics,
            Control.ClientRectangle,
            Control.FlatStyle,
            ToPushButtonState(state, Control.Enabled),
            Control.IsDefault,
            Control.Focused,
            Control.ShowFocusCues,
            Control.Parent?.BackColor ?? Control.BackColor,
            _ => PaintImage(e, layout),
            (_, textColor, drawFocus) => PaintField(
                e,
                layout,
                PaintDarkModeRender(e).Calculate(),
                textColor,
                drawFocus: false)
        );

        e.Graphics.SmoothingMode = smoothingMode;
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        // Set the smoothing mode to AntiAlias for better rendering quality
        var smoothingMode = e.Graphics.SmoothingMode;
        e.Graphics.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias;

        LayoutData layout = CommonLayout().Layout();
        ButtonDarkModeRenderer.RenderButton(
            e.Graphics,
            Control.ClientRectangle,
            Control.FlatStyle,
            PressedPushButtonState,
            Control.IsDefault,
            Control.Focused,
            Control.ShowFocusCues,
            Control.Parent?.BackColor ?? Control.BackColor,
            _ => PaintImage(e, layout),
            (_, textColor, drawFocus) => PaintField(
                e,
                layout,
                PaintDarkModeRender(e).Calculate(),
                textColor,
                drawFocus: false)
        );

        // Restore the original smoothing mode
        e.Graphics.SmoothingMode = smoothingMode;
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        // Set the smoothing mode to AntiAlias for better rendering quality
        var smoothingMode = e.Graphics.SmoothingMode;
        e.Graphics.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias;

        LayoutData layout = CommonLayout().Layout();
        ButtonDarkModeRenderer.RenderButton(
            e.Graphics,
            Control.ClientRectangle,
            Control.FlatStyle,
            HotPushButtonState,
            Control.IsDefault,
            Control.Focused,
            Control.ShowFocusCues,
            Control.Parent?.BackColor ?? Control.BackColor,
            _ => PaintImage(e, layout),
            (_, textColor, drawFocus) => PaintField(
                e,
                layout,
                PaintDarkModeRender(e).Calculate(),
                textColor,
                drawFocus: false)
        );

        // Restore the original smoothing mode
        e.Graphics.SmoothingMode = smoothingMode;
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
            ? DisabledPushButtonState
            : state switch
            {
                CheckState.Unchecked => NormalPushButtonState,
                CheckState.Checked => PressedPushButtonState,
                CheckState.Indeterminate => HotPushButtonState,
                _ => NormalPushButtonState
            };
    }
}
