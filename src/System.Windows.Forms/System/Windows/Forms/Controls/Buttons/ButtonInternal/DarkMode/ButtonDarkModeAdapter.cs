// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.ButtonInternal;

internal class ButtonDarkModeAdapter : ButtonBaseAdapter
{
    private readonly bool _animateBackgroundColors;
    private readonly ButtonDarkModeRendererBase _buttonDarkModeRenderer;

    internal ButtonDarkModeAdapter(ButtonBase control) : base(control)
    {
        bool modern = control.EffectiveVisualStylesModeInternal >= VisualStylesMode.Net11;
        _animateBackgroundColors = modern && !SystemInformation.HighContrast;

        _buttonDarkModeRenderer = control.FlatStyle switch
        {
            // With VisualStyles (.NET 11+) the modern, WinUI-inspired renderer is used for the owner-drawn
            // styles. Otherwise FlatStyle.Standard renders with a conservative owner-drawn renderer that mimics
            // the dark-mode system button (instead of delegating to the Win32 control); this makes the owner-drawn
            // path reachable and lets Standard buttons support images, focus cues, etc.
            FlatStyle.Standard => modern ? new ModernButtonDarkModeRenderer() : new SystemButtonDarkModeRenderer(),
            FlatStyle.Flat => modern ? new ModernFlatButtonRenderer() : new FlatButtonDarkModeRenderer(),
            // FlatStyle.Popup is owner-painted directly by ButtonBase using the animated key-cap renderer; the
            // adapter is used only for layout/sizing here, for which the
            // modern renderer's metrics are a good fit.
            FlatStyle.Popup => new ModernButtonDarkModeRenderer(),
            FlatStyle.System => new SystemButtonDarkModeRenderer(),
            _ => throw new ArgumentOutOfRangeException(nameof(control))
        };

        _buttonDarkModeRenderer.DeviceDpi = control.DeviceDpi;
        _buttonDarkModeRenderer.FlatAppearance = control.FlatAppearance;
    }

    private ButtonDarkModeRendererBase ButtonDarkModeRenderer
    {
        get
        {
            _buttonDarkModeRenderer.DeviceDpi = Control.DeviceDpi;
            return _buttonDarkModeRenderer;
        }
    }

    private Color GetButtonTextColor(IDeviceContext deviceContext, PushButtonState state)
    {
        Color textColor;

        if (Control.ForeColor != Forms.Control.DefaultForeColor)
        {
            textColor = new ColorOptions(deviceContext, Control.ForeColor, Control.BackColor)
            {
                Enabled = Control.Enabled
            }.Calculate().WindowText;

            if (IsHighContrastHighlighted())
            {
                textColor = SystemColors.HighlightText;
            }
        }
        else
        {
            textColor = ButtonDarkModeRenderer.GetTextColor(state, Control.IsDefault);
        }

        return textColor;
    }

    private Color GetButtonBackColor(PushButtonState state)
    {
        Color backColor;

        if (Control.BackColor != Forms.Control.DefaultBackColor)
        {
            backColor = ButtonDarkModeRenderer.GetBackgroundColor(
                state,
                Control.IsDefault,
                Control.BackColor);

            if (IsHighContrastHighlighted())
            {
                backColor = SystemColors.HighlightText;
            }
        }
        else
        {
            backColor = ButtonDarkModeRenderer.GetBackgroundColor(
                state,
                Control.IsDefault,
                customBaseColor: Color.Empty);
        }

        if (_animateBackgroundColors)
        {
            Control.BackColorAnimator.AnimateTo(backColor);
            backColor = Control.BackColorAnimator.CurrentColor;
        }

        return backColor;
    }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        try
        {
            // Use GraphicsInternal for better performance (GDI+ best practice)
            var g = e.GraphicsInternal;
            var smoothingMode = g.SmoothingMode;
            g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias;

            LayoutData layout = CommonLayout().Layout();

            PushButtonState pushButtonState = ToPushButtonState(state, Control.Enabled);
            ButtonDarkModeRenderer.RenderButton(
                g,
                Control,
                Control.ClientRectangle,
                Control.FlatStyle,
                pushButtonState,
                Control.IsDefault,
                Control.Focused,
                Control.ShowFocusCues,
                Control.Parent?.BackColor ?? Control.BackColor,
                GetButtonBackColor(pushButtonState),
                _ => PaintImage(e, layout),
                () => PaintField(
                    e,
                    layout,
                    PaintDarkModeRender(e).Calculate(),
                    GetButtonTextColor(e, pushButtonState),
                    drawFocus: false)
            );

            g.SmoothingMode = smoothingMode;
        }
        catch (Exception)
        {
            // Handle exceptions gracefully, possibly logging them or showing a message
            Debug.Assert(false, "Exception in PaintUp: Unable to render button in dark mode.");
        }
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        try
        {
            // Use GraphicsInternal for better performance (GDI+ best practice)
            var g = e.GraphicsInternal;
            var smoothingMode = g.SmoothingMode;
            g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias;

            LayoutData layout = CommonLayout().Layout();
            ButtonDarkModeRenderer.RenderButton(
                g,
                Control,
                Control.ClientRectangle,
                Control.FlatStyle,
                PushButtonState.Pressed,
                Control.IsDefault,
                Control.Focused,
                Control.ShowFocusCues,
                Control.Parent?.BackColor ?? Control.BackColor,
                GetButtonBackColor(PushButtonState.Pressed),
                _ => PaintImage(e, layout),
                () => PaintField(
                    e,
                    layout,
                    PaintDarkModeRender(e).Calculate(),
                    GetButtonTextColor(e, PushButtonState.Pressed),
                    drawFocus: false)
            );

            g.SmoothingMode = smoothingMode;
        }
        catch (Exception)
        {
            // Handle exceptions gracefully, possibly logging them or showing a message
            Debug.Assert(false, "Exception in PaintDown: Unable to render button in dark mode.");
        }
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        try
        {
            // Use GraphicsInternal for better performance (GDI+ best practice)
            var g = e.GraphicsInternal;
            var smoothingMode = g.SmoothingMode;
            g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias;

            LayoutData layout = CommonLayout().Layout();
            ButtonDarkModeRenderer.RenderButton(
                g,
                Control,
                Control.ClientRectangle,
                Control.FlatStyle,
                PushButtonState.Hot,
                Control.IsDefault,
                Control.Focused,
                Control.ShowFocusCues,
                Control.Parent?.BackColor ?? Control.BackColor,
                GetButtonBackColor(PushButtonState.Hot),
                _ => PaintImage(e, layout),
                () => PaintField(
                    e,
                    layout,
                    PaintDarkModeRender(e).Calculate(),
                    GetButtonTextColor(e, PushButtonState.Hot),
                    drawFocus: false)
            );

            g.SmoothingMode = smoothingMode;
        }
        catch (Exception ex)
        {
            Debug.Assert(false, $"Exception in PaintOver: {ex.Message}");
        }
    }

    protected override LayoutOptions Layout(PaintEventArgs e) => CommonLayout();

    internal override Size GetPreferredSizeCore(Size proposedSize)
        => Control.FlatStyle == FlatStyle.Popup
            ? GetPopupPreferredSizeCore(CommonLayout(), proposedSize)
            : base.GetPreferredSizeCore(proposedSize);

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

    private static PushButtonState ToPushButtonState(CheckState state, bool enabled) =>
        !enabled
            ? PushButtonState.Disabled
            : state switch
            {
                CheckState.Unchecked => PushButtonState.Normal,
                CheckState.Checked => PushButtonState.Pressed,
                CheckState.Indeterminate => PushButtonState.Hot,
                _ => PushButtonState.Normal
            };
}
