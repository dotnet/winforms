// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Rendering.Animation;

namespace System.Windows.Forms.ButtonInternal;

/// <summary>
///  Animates a button's background color between interaction states.
/// </summary>
internal sealed class ButtonBackColorAnimator : AnimatedControlRenderer
{
    private const int AnimationDuration = 350;

    private Color _fromColor;
    private Color _toColor;
    private bool _hasColor;

    public ButtonBackColorAnimator(Control control) : base(control)
    {
    }

    public Color CurrentColor { get; private set; }

    public void AnimateTo(Color targetColor)
    {
        if (!_hasColor)
        {
            _hasColor = true;
            CurrentColor = targetColor;
            _toColor = targetColor;
            return;
        }

        if (targetColor == _toColor)
        {
            return;
        }

        _fromColor = CurrentColor;
        _toColor = targetColor;
        RestartAnimation();
    }

    public override void AnimationProc(float animationProgress)
    {
        base.AnimationProc(animationProgress);
        CurrentColor = Lerp(_fromColor, _toColor, animationProgress);
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
        CurrentColor = _toColor;
        AnimationProgress = 1;
        Invalidate();
    }

    private static Color Lerp(Color from, Color to, float progress)
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
}
