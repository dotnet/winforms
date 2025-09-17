// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.ButtonInternal;

internal class ButtonDarkModeAdapter : ButtonBaseAdapter
{
    private readonly ButtonDarkModeRendererBase _buttonDarkModeRenderer;

    internal ButtonDarkModeAdapter(ButtonBase control) : base(control)
    {
        _buttonDarkModeRenderer = control.FlatStyle switch
        {
            FlatStyle.Standard => new FlatButtonDarkModeRenderer(),
            FlatStyle.Flat => new FlatButtonDarkModeRenderer(),
            FlatStyle.Popup => new PopupButtonDarkModeRenderer(),
            FlatStyle.System => new SystemButtonDarkModeRenderer(),
            _ => throw new ArgumentOutOfRangeException(nameof(control))
        };
    }

    private ButtonDarkModeRendererBase ButtonDarkModeRenderer =>
        _buttonDarkModeRenderer;

    private Color GetButtonTextColor(IDeviceContext deviceContext, PushButtonState state)
    {
        Color textColor;

        if (Control.ForeColorSet)
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
        Color textColor;

        if (Control.BackColorSet)
        {
            textColor = Control.BackColor;

            if (IsHighContrastHighlighted())
            {
                textColor = SystemColors.HighlightText;
            }
        }
        else
        {
            textColor = ButtonDarkModeRenderer.GetBackgroundColor(state, Control.IsDefault);
        }

        return textColor;
    }

    internal void PaintBackgroundImage(PaintEventArgs e, int borderSize, Rectangle paddedBounds)
    {
        if (Application.IsDarkModeEnabled && Control.DarkModeRequestState is true && Control.BackgroundImage is not null)
        {
            Rectangle bounds = paddedBounds;
            if (Control.FlatStyle == FlatStyle.Popup)
            {
                bounds.Inflate(-1, -1);
            }
            else
            {
                bounds.Inflate(-1 - borderSize, -1 - borderSize);
            }

            ControlPaint.DrawBackgroundImage(
                e.GraphicsInternal,
                Control.BackgroundImage,
                Color.Transparent,
                Control.BackgroundImageLayout,
                paddedBounds,
                bounds,
                bounds.Location,
                Control.RightToLeft);
        }
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
                Control.ClientRectangle,
                Control.FlatStyle,
                Control.FlatAppearance.BorderSize,
                pushButtonState,
                Control.IsDefault,
                Control.Focused,
                Control.ShowFocusCues,
                Control.Parent?.BackColor ?? Control.BackColor,
                GetButtonBackColor(pushButtonState),
                _ => PaintImage(e, layout),
                _ => PaintBackgroundImage(
                    e,
                    Control.FlatAppearance.BorderSize,
                    Control.ClientRectangle),
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
                Control.ClientRectangle,
                Control.FlatStyle,
                Control.FlatAppearance.BorderSize,
                PushButtonState.Pressed,
                Control.IsDefault,
                Control.Focused,
                Control.ShowFocusCues,
                Control.Parent?.BackColor ?? Control.BackColor,
                GetButtonBackColor(PushButtonState.Pressed),
                _ => PaintImage(e, layout),
                _ => PaintBackgroundImage(
                    e,
                    Control.FlatAppearance.BorderSize,
                    Control.ClientRectangle),
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
                Control.ClientRectangle,
                Control.FlatStyle,
                Control.FlatAppearance.BorderSize,
                PushButtonState.Hot,
                Control.IsDefault,
                Control.Focused,
                Control.ShowFocusCues,
                Control.Parent?.BackColor ?? Control.BackColor,
                GetButtonBackColor(PushButtonState.Hot),
                _ => PaintImage(e, layout),
                _ => PaintBackgroundImage(
                    e,
                    Control.FlatAppearance.BorderSize,
                    Control.ClientRectangle),
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
