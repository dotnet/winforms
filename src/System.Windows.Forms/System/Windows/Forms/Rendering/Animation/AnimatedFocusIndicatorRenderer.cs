// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Rendering.Button;

namespace System.Windows.Forms.Rendering.Animation;

/// <summary>
///  Animates a rounded focus indicator between a control's border and focus colors.
/// </summary>
internal sealed class AnimatedFocusIndicatorRenderer : AnimatedControlRenderer
{
    private const int AnimationDurationMilliseconds = 200;

    private readonly Action _invalidate;
    private float _focusCurrent;
    private float _focusStart;
    private float _focusTarget;
    private bool _initialized;

    public AnimatedFocusIndicatorRenderer(Control control, Action invalidate)
        : base(control)
    {
        ArgumentNullException.ThrowIfNull(invalidate);
        _invalidate = invalidate;
    }

    internal float FocusAmount
    {
        get
        {
            EnsureInitialized(Control.Focused ? 1f : 0f);
            return _focusCurrent;
        }
    }

    internal Color GetCurrentColor(Color borderColor, Color focusColor)
        => PopupButtonColorMath.Blend(borderColor, focusColor, FocusAmount);

    internal void SetFocused(bool focused, bool animate)
    {
        float target = focused ? 1f : 0f;
        EnsureInitialized(1f - target);

        if (_focusTarget == target && (!IsRunning || animate))
        {
            return;
        }

        _focusStart = _focusCurrent;
        _focusTarget = target;

        if (!animate)
        {
            Synchronize(focused, invalidate: true);
            return;
        }

        RestartAnimation();
        _invalidate();
    }

    internal void Synchronize(bool focused, bool invalidate)
    {
        if (IsRunning)
        {
            StopAnimation();
        }

        _focusCurrent = focused ? 1f : 0f;
        _focusStart = _focusCurrent;
        _focusTarget = _focusCurrent;
        _initialized = true;
        AnimationProgress = 1f;

        if (invalidate)
        {
            _invalidate();
        }
    }

    internal void DrawRoundedFocusIndicator(
        Graphics graphics,
        Rectangle bounds,
        int cornerSize,
        int borderThickness,
        int focusBandHeight,
        Color borderColor,
        Color focusColor)
    {
        if (bounds.Width <= 0
            || bounds.Height <= 0
            || cornerSize <= 0
            || borderThickness <= 0
            || focusBandHeight <= 0
            || FocusAmount <= 0f)
        {
            return;
        }

        int focusBandTop = Math.Max(bounds.Top, bounds.Bottom - focusBandHeight + 1);
        Rectangle focusClip = Rectangle.FromLTRB(
            bounds.Left,
            focusBandTop,
            bounds.Right + 1,
            bounds.Bottom + 1);

        using GraphicsStateScope state = new(graphics);
        graphics.SetClip(focusClip, CombineMode.Intersect);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using GraphicsPath focusPath = new();
        focusPath.AddRoundedRectangle(bounds, new Size(cornerSize, cornerSize));

        Color color = GetCurrentColor(borderColor, focusColor);
        using var focusPen = color.GetCachedPenScope(borderThickness);
        graphics.DrawPath(focusPen, focusPath);
    }

    public override void AnimationProc(float animationProgress)
    {
        base.AnimationProc(animationProgress);
        float easedProgress = EaseOut(animationProgress);
        _focusCurrent = Lerp(_focusStart, _focusTarget, easedProgress);
        _invalidate();
    }

    public override void RenderControl(Graphics graphics)
    {
    }

    protected override (int animationDuration, AnimationCycle animationCycle) OnAnimationStarted()
    {
        AnimationProgress = 0f;
        return (AnimationDurationMilliseconds, AnimationCycle.Once);
    }

    protected override void OnAnimationStopped()
    {
    }

    protected override void OnAnimationEnded()
    {
        StopAnimation();
        _focusCurrent = _focusTarget;
        _focusStart = _focusCurrent;
        AnimationProgress = 1f;
        _invalidate();
    }

    private void EnsureInitialized(float initialValue)
    {
        if (_initialized)
        {
            return;
        }

        _focusCurrent = initialValue;
        _focusStart = initialValue;
        _focusTarget = initialValue;
        _initialized = true;
    }

    private static float EaseOut(float progress)
        => 1f - ((1f - progress) * (1f - progress));

    private static float Lerp(float start, float end, float amount)
        => start + ((end - start) * Math.Clamp(amount, 0f, 1f));
}
