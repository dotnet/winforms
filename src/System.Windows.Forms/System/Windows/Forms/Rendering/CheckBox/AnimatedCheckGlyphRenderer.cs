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

    private CheckState _fromState;
    private CheckState _toState;
    private bool _initialized;

    public AnimatedCheckGlyphRenderer(Control control) : base(control)
    {
    }

    internal void NotifyCheckStateChanged(CheckState newState)
    {
        if (!_initialized)
        {
            _initialized = true;
            _fromState = newState;
            _toState = newState;
            return;
        }

        if (newState == _toState)
        {
            return;
        }

        _fromState = AnimationProgress >= 1 ? _toState : _fromState;
        _toState = newState;
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

        Color offBorderColor = customBorderColor
            ?? (isDark ? Color.FromArgb(0x9B, 0x9B, 0x9B) : SystemColors.ControlDark);

        Color offBackColor = isDark ? Color.FromArgb(0x2D, 0x2D, 0x2D) : Color.White;

        if (!enabled)
        {
            onColor = isDark ? Color.FromArgb(0x55, 0x55, 0x55) : Color.FromArgb(0xC0, 0xC0, 0xC0);
            offBorderColor = isDark ? Color.FromArgb(0x45, 0x45, 0x45) : Color.FromArgb(0xD0, 0xD0, 0xD0);
        }

        float progress = AnimationProgress;
        float fill = Lerp(FillAmount(_fromState), FillAmount(_toState), progress);
        Color backColor = LerpColor(offBackColor, onColor, fill);
        Color borderColor = LerpColor(offBorderColor, onColor, fill);

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
                ? (isDark ? Color.Black : Color.White)
                : offBackColor;

            float checkAlpha = GlyphAlpha(CheckState.Checked, progress);
            if (checkAlpha > 0)
            {
                DrawCheckmark(graphics, bounds, glyphColor, checkAlpha);
            }

            float dashAlpha = GlyphAlpha(CheckState.Indeterminate, progress);
            if (dashAlpha > 0)
            {
                DrawDash(graphics, bounds, glyphColor, dashAlpha);
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
        _fromState = _toState;
        AnimationProgress = 1;
        Invalidate();
    }

    private float GlyphAlpha(CheckState glyphState, float progress)
    {
        float alpha = 0;
        if (_fromState == glyphState)
        {
            alpha += 1 - progress;
        }

        if (_toState == glyphState)
        {
            alpha += progress;
        }

        return Math.Clamp(alpha, 0f, 1f);
    }

    private static float FillAmount(CheckState state)
        => state == CheckState.Unchecked ? 0f : 1f;

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
