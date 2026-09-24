// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Rendering.Button;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.ButtonInternal;

internal class ButtonDarkModeAdapter : ButtonBaseAdapter
{
    private readonly bool _animateBackgroundColors;
    private readonly ButtonDarkModeRendererBase _buttonDarkModeRenderer;
    private readonly bool _modern;

    internal ButtonDarkModeAdapter(ButtonBase control) : base(control)
    {
        _modern = control.EffectiveVisualStylesModeInternal >= VisualStylesMode.Net11;
        _animateBackgroundColors = _modern && !SystemInformation.HighContrast;

        _buttonDarkModeRenderer = control.FlatStyle switch
        {
            FlatStyle.Standard => _modern ? new ModernButtonDarkModeRenderer() : new FlatButtonDarkModeRenderer(),
            FlatStyle.Flat => _modern ? new ModernFlatButtonRenderer() : new FlatButtonDarkModeRenderer(),
            FlatStyle.Popup => _modern ? new ModernButtonDarkModeRenderer() : new PopupButtonDarkModeRenderer(),
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

    private Color GetButtonTextColor(
        IDeviceContext deviceContext,
        PushButtonState state,
        Color backColor)
    {
        Color textColor;

        if (_modern && !Control.Enabled)
        {
            return ModernControlColorMath.GetDisabledTextColor(
                Control.ForeColor,
                backColor);
        }

        bool useEffectiveForeColor = _modern
            ? Control.ShouldSerializeForeColor()
            : Control.ForeColor != Forms.Control.DefaultForeColor;

        if (useEffectiveForeColor)
        {
            textColor = Control.Enabled
                && Control.EffectiveVisualStylesModeInternal >= VisualStylesMode.Net11
                    ? Control.ForeColor
                    : new ColorOptions(deviceContext, Control.ForeColor, Control.BackColor)
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
            textColor = ButtonDarkModeRenderer.GetTextColor(state, Control.IsDefault, backColor);
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
        => PaintCore(e, ToPushButtonState(state, Control.Enabled));

    internal override void PaintDown(PaintEventArgs e, CheckState state)
        => PaintCore(e, PushButtonState.Pressed);

    internal override void PaintOver(PaintEventArgs e, CheckState state)
        => PaintCore(e, PushButtonState.Hot);

    private void PaintCore(PaintEventArgs e, PushButtonState state)
    {
        var graphics = e.GraphicsInternal;
        Drawing.Drawing2D.SmoothingMode smoothingMode = graphics.SmoothingMode;

        try
        {
            graphics.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Color backColor = GetButtonBackColor(state);
            Color parentBackColor = Control.Parent?.BackColor ?? Control.BackColor;
            Color textBackColor = PopupButtonColorMath.Composite(backColor, parentBackColor);
            ButtonDarkModeRenderer.RenderButton(
                graphics,
                Control,
                Control.ClientRectangle,
                Control.FlatStyle,
                state,
                Control.IsDefault,
                Control.Focused,
                Control.ShowFocusCues,
                parentBackColor,
                backColor,
                contentBounds =>
                {
                    LayoutData layout = GetLayoutData(contentBounds);
                    PaintBackgroundImage(e, contentBounds);
                    PaintImage(e, layout);
                    PaintField(
                        e,
                        layout,
                        PaintDarkModeRender(e).Calculate(),
                        GetButtonTextColor(e, state, textBackColor),
                        drawFocus: false);
                });
        }
        finally
        {
            graphics.SmoothingMode = smoothingMode;
        }
    }

    protected override LayoutOptions Layout(PaintEventArgs e) => CommonLayout();

    internal override Size GetPreferredSizeCore(Size proposedSize)
        => Control.FlatStyle == FlatStyle.Popup && _modern
            ? GetModernPopupPreferredSizeCore(
                CommonLayout(),
                proposedSize,
                Control.DeviceDpi,
                Control.FlatAppearance.BorderSize)
            : _modern
                ? GetModernPreferredSizeCore(
                    CommonLayout(),
                    proposedSize,
                    ButtonDarkModeRenderer.GetPreferredSizePadding().Size)
                : base.GetPreferredSizeCore(proposedSize);

    internal override LayoutOptions CommonLayout()
    {
        LayoutOptions layout = base.CommonLayout();
        layout.DisableWordWrapping = Control.AutoSize;
        layout.DotNetOneButtonCompat = !_modern;
        layout.ClipImagesToClient = _modern;
        layout.EnsureImagePreferredSizeInset = _modern;
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
