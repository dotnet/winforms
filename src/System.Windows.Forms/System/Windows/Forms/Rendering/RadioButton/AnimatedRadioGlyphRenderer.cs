// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Rendering.Animation;

namespace System.Windows.Forms.Rendering.RadioButton;

/// <summary>
///  Animates and draws the modern normal-appearance RadioButton glyph.
/// </summary>
internal sealed class AnimatedRadioGlyphRenderer : AnimatedControlRenderer
{
    private const int AnimationDuration = 200;

    private bool _fromChecked;
    private bool _toChecked;
    private bool _initialized;

    public AnimatedRadioGlyphRenderer(Control control) : base(control)
    {
    }

    internal void NotifyCheckedChanged(bool newChecked)
    {
        if (!_initialized)
        {
            _initialized = true;
            _fromChecked = newChecked;
            _toChecked = newChecked;
            return;
        }

        if (newChecked == _toChecked)
        {
            return;
        }

        _fromChecked = AnimationProgress >= 1 ? _toChecked : _fromChecked;
        _toChecked = newChecked;
        RestartAnimation();
    }

    internal void DrawGlyph(
        Graphics graphics,
        Rectangle bounds,
        FlatStyle flatStyle,
        bool enabled,
        Color? customOnColor,
        Color? customBorderColor)
    {
        bool isDark = Application.IsDarkModeEnabled;
        Color onColor = customOnColor
            ?? (isDark ? Color.FromArgb(0x4C, 0xC2, 0xFF) : SystemColors.Highlight);

        Color borderColor = customBorderColor
            ?? (isDark ? Color.FromArgb(0x9B, 0x9B, 0x9B) : SystemColors.ControlDark);

        Color backColor = isDark ? Color.FromArgb(0x2D, 0x2D, 0x2D) : Color.White;

        if (!enabled)
        {
            onColor = isDark ? Color.FromArgb(0x55, 0x55, 0x55) : Color.FromArgb(0xC0, 0xC0, 0xC0);
            borderColor = isDark ? Color.FromArgb(0x45, 0x45, 0x45) : Color.FromArgb(0xD0, 0xD0, 0xD0);
        }

        float dotScale = Lerp(
            _fromChecked ? 1f : 0f,
            _toChecked ? 1f : 0f,
            EaseOut(AnimationProgress));

        GraphicsState? saved = graphics.Save();
        try
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (var brush = backColor.GetCachedSolidBrushScope())
            {
                graphics.FillEllipse(brush, bounds);
            }

            int borderThickness = Math.Max(
                1,
                Control.LogicalToDeviceUnits(flatStyle == FlatStyle.Popup ? 2 : 1));

            using (var pen = new Pen(borderColor, borderThickness))
            {
                graphics.DrawEllipse(pen, bounds);
            }

            if (dotScale > 0.001f)
            {
                float dotDiameter = bounds.Width * 0.5f * dotScale;
                RectangleF dotRectangle = new(
                    bounds.X + ((bounds.Width - dotDiameter) / 2f),
                    bounds.Y + ((bounds.Height - dotDiameter) / 2f),
                    dotDiameter,
                    dotDiameter);

                using var dotBrush = onColor.GetCachedSolidBrushScope();
                graphics.FillEllipse(dotBrush, dotRectangle);
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
        Invalidate();
    }

    public override void RenderControl(Graphics graphics)
    {
    }

    protected override (int animationDuration, AnimationCycle animationCycle) OnAnimationStarted()
    {
        AnimationProgress = 0;
        return (AnimationDuration, AnimationCycle.Once);
    }

    protected override void OnAnimationStopped()
    {
    }

    protected override void OnAnimationEnded()
    {
        StopAnimation();
        _fromChecked = _toChecked;
        AnimationProgress = 1;
        Invalidate();
    }

    private static float EaseOut(float progress)
        => 1 - ((1 - progress) * (1 - progress));

    private static float Lerp(float from, float to, float progress)
        => from + ((to - from) * Math.Clamp(progress, 0f, 1f));
}
