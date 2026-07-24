// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Rendering.Animation;
using System.Windows.Forms.Rendering.Button;

namespace System.Windows.Forms.Rendering.RadioButton;

/// <summary>
///  Animates and draws the modern normal-appearance RadioButton glyph.
/// </summary>
internal sealed class AnimatedRadioGlyphRenderer : AnimatedControlRenderer
{
    private const int AnimationDuration = 200;
    private const float HoverGrowth = 1.1f;

    private float _dotScaleCurrent;
    private float _dotScaleStart;
    private float _dotScaleTarget;
    private float _focusCurrent;
    private float _focusStart;
    private float _focusTarget;
    private float _hoverCurrent;
    private float _hoverStart;
    private float _hoverTarget;
    private bool _interactionInitialized;
    private bool _stateInitialized;

    public AnimatedRadioGlyphRenderer(Control control) : base(control)
    {
    }

    internal void NotifyCheckedChanged(bool newChecked)
    {
        float dotScaleTarget = newChecked ? 1f : 0f;
        if (!_stateInitialized)
        {
            _stateInitialized = true;
            _dotScaleCurrent = _dotScaleStart = _dotScaleTarget = dotScaleTarget;
            return;
        }

        if (_dotScaleTarget == dotScaleTarget)
        {
            return;
        }

        _dotScaleTarget = dotScaleTarget;
        RestartAnimation();
    }

    internal void SetInteractionState(bool hovered, bool focused)
    {
        float hoverTarget = hovered ? 1f : 0f;
        float focusTarget = focused ? 1f : 0f;
        if (!_interactionInitialized)
        {
            _interactionInitialized = true;
            _hoverCurrent = _hoverStart = _hoverTarget = hoverTarget;
            _focusCurrent = _focusStart = _focusTarget = focusTarget;
            return;
        }

        if (_hoverTarget == hoverTarget && _focusTarget == focusTarget)
        {
            return;
        }

        _hoverTarget = hoverTarget;
        _focusTarget = focusTarget;
        RestartAnimation();
    }

    internal void DrawGlyph(
        Graphics graphics,
        Rectangle bounds,
        FlatStyle flatStyle,
        bool enabled,
        bool hovered,
        bool focused,
        Color? customOnColor,
        Color? customBorderColor)
    {
        SetInteractionState(hovered, focused);

        bool isDark = Application.IsDarkModeEnabled;
        bool highContrast = SystemInformation.HighContrast;
        Color onColor = highContrast
            ? SystemColors.Highlight
            : customOnColor ?? WindowsAccentColor;

        Color borderColor = highContrast
            ? SystemColors.WindowText
            : customBorderColor
                ?? (isDark
                    ? Color.FromArgb(0x9B, 0x9B, 0x9B)
                    : SystemColors.ControlDark);

        Color backColor = highContrast
            ? SystemColors.Window
            : isDark
                ? Color.FromArgb(0x2D, 0x2D, 0x2D)
                : Color.White;

        if (!enabled)
        {
            onColor = highContrast
                ? SystemColors.GrayText
                : isDark
                    ? Color.FromArgb(0x55, 0x55, 0x55)
                    : Color.FromArgb(0xC0, 0xC0, 0xC0);
            borderColor = highContrast
                ? SystemColors.GrayText
                : isDark
                    ? Color.FromArgb(0x45, 0x45, 0x45)
                    : Color.FromArgb(0xD0, 0xD0, 0xD0);
        }

        float focus = enabled && !highContrast ? _focusCurrent : 0f;
        onColor = ApplyInteractionShade(onColor, focus);
        borderColor = ApplyInteractionShade(borderColor, focus);
        backColor = ApplyInteractionShade(backColor, focus);

        float normalOuterScale = 1f / HoverGrowth;
        float outerScale = Lerp(normalOuterScale, 1f, enabled ? _hoverCurrent : 0f);
        RectangleF outerBounds = ScaleFromCenter(bounds, outerScale);
        RectangleF normalBounds = ScaleFromCenter(bounds, normalOuterScale);

        GraphicsState? saved = graphics.Save();
        try
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (var brush = backColor.GetCachedSolidBrushScope())
            {
                graphics.FillEllipse(brush, outerBounds);
            }

            int borderThickness = Math.Max(
                1,
                Control.LogicalToDeviceUnits(flatStyle == FlatStyle.Popup ? 2 : 1));

            using (var pen = new Pen(borderColor, borderThickness))
            {
                graphics.DrawEllipse(pen, outerBounds);
            }

            if (_dotScaleCurrent > 0.001f)
            {
                float dotDiameter = normalBounds.Width * 0.5f * _dotScaleCurrent;
                RectangleF dotRectangle = new(
                    normalBounds.X + ((normalBounds.Width - dotDiameter) / 2f),
                    normalBounds.Y + ((normalBounds.Height - dotDiameter) / 2f),
                    dotDiameter,
                    dotDiameter);

                Color dotOutlineColor = highContrast
                    ? SystemColors.HighlightText
                    : PopupButtonColorMath.GetReadableForeColor(onColor, backColor);
                int outlineThickness = Math.Max(1, Control.LogicalToDeviceUnits(1));
                using var outlineBrush = dotOutlineColor.GetCachedSolidBrushScope();
                graphics.FillEllipse(outlineBrush, dotRectangle);

                RectangleF accentRectangle = RectangleF.Inflate(
                    dotRectangle,
                    -outlineThickness,
                    -outlineThickness);
                if (accentRectangle.Width > 0 && accentRectangle.Height > 0)
                {
                    using var dotBrush = onColor.GetCachedSolidBrushScope();
                    graphics.FillEllipse(dotBrush, accentRectangle);
                }
            }
        }
        finally
        {
            if (saved is not null)
            {
                graphics.Restore(saved);
            }
        }
    }

    public override void AnimationProc(float animationProgress)
    {
        base.AnimationProc(animationProgress);
        float easedProgress = EaseOut(animationProgress);
        _dotScaleCurrent = Lerp(_dotScaleStart, _dotScaleTarget, easedProgress);
        _focusCurrent = Lerp(_focusStart, _focusTarget, easedProgress);
        _hoverCurrent = Lerp(_hoverStart, _hoverTarget, easedProgress);
        Invalidate();
    }

    public override void RenderControl(Graphics graphics)
    {
    }

    protected override (int animationDuration, AnimationCycle animationCycle) OnAnimationStarted()
    {
        _dotScaleStart = _dotScaleCurrent;
        _focusStart = _focusCurrent;
        _hoverStart = _hoverCurrent;
        AnimationProgress = 0;
        return (AnimationDuration, AnimationCycle.Once);
    }

    protected override void OnAnimationStopped()
    {
    }

    protected override void OnAnimationEnded()
    {
        StopAnimation();
        _dotScaleCurrent = _dotScaleTarget;
        _focusCurrent = _focusTarget;
        _hoverCurrent = _hoverTarget;
        AnimationProgress = 1;
        Invalidate();
    }

    private static float EaseOut(float progress)
        => 1 - ((1 - progress) * (1 - progress));

    private static float Lerp(float from, float to, float progress)
        => from + ((to - from) * Math.Clamp(progress, 0f, 1f));

    private static RectangleF ScaleFromCenter(Rectangle bounds, float scale)
    {
        float width = bounds.Width * scale;
        float height = bounds.Height * scale;

        return new(
            bounds.X + ((bounds.Width - width) / 2f),
            bounds.Y + ((bounds.Height - height) / 2f),
            width,
            height);
    }
}
