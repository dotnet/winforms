// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Rendering.Animation;

namespace System.Windows.Forms.Rendering.CheckBox;

/// <summary>
///  Animates and draws the modern normal-appearance CheckBox glyph.
/// </summary>
internal sealed class AnimatedCheckGlyphRenderer : AnimatedControlRenderer
{
    private const int AnimationDuration = 220;

    private float _checkAlphaCurrent;
    private float _checkAlphaStart;
    private float _checkAlphaTarget;
    private float _dashAlphaCurrent;
    private float _dashAlphaStart;
    private float _dashAlphaTarget;
    private float _fillCurrent;
    private float _fillStart;
    private float _fillTarget;
    private float _interactionCurrent;
    private float _interactionStart;
    private float _interactionTarget;
    private bool _interactionInitialized;
    private bool _stateInitialized;

    public AnimatedCheckGlyphRenderer(Control control) : base(control)
    {
    }

    internal void NotifyCheckStateChanged(CheckState newState)
    {
        (float fill, float checkAlpha, float dashAlpha) = GetStateTargets(newState);
        if (!_stateInitialized)
        {
            _stateInitialized = true;
            _fillCurrent = _fillStart = _fillTarget = fill;
            _checkAlphaCurrent = _checkAlphaStart = _checkAlphaTarget = checkAlpha;
            _dashAlphaCurrent = _dashAlphaStart = _dashAlphaTarget = dashAlpha;
            return;
        }

        if (_fillTarget == fill
            && _checkAlphaTarget == checkAlpha
            && _dashAlphaTarget == dashAlpha)
        {
            return;
        }

        _fillTarget = fill;
        _checkAlphaTarget = checkAlpha;
        _dashAlphaTarget = dashAlpha;
        RestartAnimation();
    }

    internal void SetInteractionState(bool hovered, bool focused)
    {
        float interactionTarget = hovered || focused ? 1f : 0f;
        if (!_interactionInitialized)
        {
            _interactionInitialized = true;
            _interactionCurrent = _interactionStart = _interactionTarget = interactionTarget;
            return;
        }

        if (_interactionTarget == interactionTarget)
        {
            return;
        }

        _interactionTarget = interactionTarget;
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

        Color offBorderColor = highContrast
            ? SystemColors.WindowText
            : customBorderColor
                ?? (isDark
                    ? Color.FromArgb(0x9B, 0x9B, 0x9B)
                    : SystemColors.ControlDark);

        Color offBackColor = highContrast
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
            offBorderColor = highContrast
                ? SystemColors.GrayText
                : isDark
                    ? Color.FromArgb(0x45, 0x45, 0x45)
                    : Color.FromArgb(0xD0, 0xD0, 0xD0);
        }

        Color backColor = LerpColor(offBackColor, onColor, _fillCurrent);
        Color borderColor = LerpColor(offBorderColor, onColor, _fillCurrent);
        float interaction = enabled && !highContrast ? _interactionCurrent : 0f;
        backColor = ApplyInteractionShade(backColor, interaction);
        borderColor = ApplyInteractionShade(borderColor, interaction);

        GraphicsState? saved = graphics.Save();
        try
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using GraphicsPath path = CreateBoxPath(bounds, flatStyle);

            using (var brush = backColor.GetCachedSolidBrushScope())
            {
                graphics.FillPath(brush, path);
            }

            int borderThickness = Math.Max(
                1,
                Control.LogicalToDeviceUnits(flatStyle == FlatStyle.Popup ? 2 : 1));

            using (var pen = new Pen(borderColor, borderThickness) { Alignment = PenAlignment.Inset })
            {
                graphics.DrawPath(pen, path);
            }

            Color glyphColor = enabled
                ? highContrast
                    ? SystemColors.HighlightText
                    : ModernButtonColorMath.GetReadableForeColor(onColor)
                : offBackColor;

            if (_checkAlphaCurrent > 0)
            {
                DrawCheckmark(graphics, bounds, glyphColor, _checkAlphaCurrent);
            }

            if (_dashAlphaCurrent > 0)
            {
                DrawDash(graphics, bounds, glyphColor, _dashAlphaCurrent);
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
        _fillCurrent = Lerp(_fillStart, _fillTarget, easedProgress);
        _checkAlphaCurrent = Lerp(_checkAlphaStart, _checkAlphaTarget, easedProgress);
        _dashAlphaCurrent = Lerp(_dashAlphaStart, _dashAlphaTarget, easedProgress);
        _interactionCurrent = Lerp(_interactionStart, _interactionTarget, easedProgress);
        Invalidate();
    }

    public override void RenderControl(Graphics graphics)
    {
    }

    protected override (int animationDuration, AnimationCycle animationCycle) OnAnimationStarted()
    {
        _fillStart = _fillCurrent;
        _checkAlphaStart = _checkAlphaCurrent;
        _dashAlphaStart = _dashAlphaCurrent;
        _interactionStart = _interactionCurrent;
        AnimationProgress = 0;
        return (AnimationDuration, AnimationCycle.Once);
    }

    protected override void OnAnimationStopped()
    {
    }

    protected override void OnAnimationEnded()
    {
        StopAnimation();
        _fillCurrent = _fillTarget;
        _checkAlphaCurrent = _checkAlphaTarget;
        _dashAlphaCurrent = _dashAlphaTarget;
        _interactionCurrent = _interactionTarget;
        AnimationProgress = 1;
        Invalidate();
    }

    private static (float Fill, float CheckAlpha, float DashAlpha) GetStateTargets(CheckState state)
        => state switch
        {
            CheckState.Checked => (1f, 1f, 0f),
            CheckState.Indeterminate => (1f, 0f, 1f),
            _ => (0f, 0f, 0f)
        };

    private static float Lerp(float from, float to, float progress)
        => from + ((to - from) * Math.Clamp(progress, 0f, 1f));

    private static Color LerpColor(Color from, Color to, float progress)
    {
        progress = Math.Clamp(progress, 0f, 1f);

        return Color.FromArgb(
            LerpChannel(from.A, to.A, progress),
            LerpChannel(from.R, to.R, progress),
            LerpChannel(from.G, to.G, progress),
            LerpChannel(from.B, to.B, progress));

        static int LerpChannel(int from, int to, float progress)
            => from + (int)((to - from) * progress);
    }

    private static float EaseOut(float progress)
        => 1 - ((1 - progress) * (1 - progress));

    private static GraphicsPath CreateBoxPath(Rectangle bounds, FlatStyle flatStyle)
    {
        GraphicsPath path = new();
        if (flatStyle == FlatStyle.Flat)
        {
            path.AddRectangle(bounds);
            return path;
        }

        double radiusFactor = flatStyle == FlatStyle.Popup ? 0.3 : 0.2;
        int radius = Math.Max(1, (int)(Math.Min(bounds.Width, bounds.Height) * radiusFactor));
        path.AddRoundedRectangle(bounds, new Size(radius, radius));
        return path;
    }

    private static void DrawCheckmark(Graphics graphics, Rectangle bounds, Color color, float alpha)
    {
        using var pen = new Pen(
            Color.FromArgb((int)(alpha * 255), color),
            Math.Max(1.5f, bounds.Width * 0.12f))
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
            LineJoin = LineJoin.Round
        };

        PointF first = new(bounds.Left + (bounds.Width * 0.20f), bounds.Top + (bounds.Height * 0.52f));
        PointF second = new(bounds.Left + (bounds.Width * 0.42f), bounds.Top + (bounds.Height * 0.74f));
        PointF third = new(bounds.Left + (bounds.Width * 0.82f), bounds.Top + (bounds.Height * 0.28f));
        graphics.DrawLines(pen, [first, second, third]);
    }

    private static void DrawDash(Graphics graphics, Rectangle bounds, Color color, float alpha)
    {
        using var pen = new Pen(
            Color.FromArgb((int)(alpha * 255), color),
            Math.Max(1.5f, bounds.Height * 0.14f))
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };

        float y = bounds.Top + (bounds.Height * 0.5f);
        graphics.DrawLine(
            pen,
            bounds.Left + (bounds.Width * 0.2f),
            y,
            bounds.Right - (bounds.Width * 0.2f),
            y);
    }
}
