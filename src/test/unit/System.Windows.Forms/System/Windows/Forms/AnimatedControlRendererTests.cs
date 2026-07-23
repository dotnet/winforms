// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using System.Windows.Forms.Rendering.Animation;

namespace System.Windows.Forms.Tests;

// NOTE: AnimationManager is a process-wide singleton driven by the real HighPrecisionTimer
// (which raises the system timer resolution and starts a background loop). To keep these unit
// tests deterministic and free of process-wide side effects, they exercise the
// AnimatedControlRenderer contract that does NOT touch AnimationManager. Start/stop/cycle
// behavior is covered by the control-level tests and manual exploratory testing.
public class AnimatedControlRendererTests
{
    [WinFormsFact]
    public void AnimatedControlRenderer_DefaultIsRunning_IsFalse()
    {
        using Control control = new();
        using SubAnimatedControlRenderer renderer = new(control);

        Assert.False(renderer.IsRunning);
    }

    [WinFormsTheory]
    [InlineData(0f)]
    [InlineData(0.5f)]
    [InlineData(1f)]
    public void AnimatedControlRenderer_AnimationProc_StoresProgress(float progress)
    {
        using Control control = new();
        using SubAnimatedControlRenderer renderer = new(control);

        renderer.AnimationProc(progress);

        Assert.Equal(progress, renderer.AnimationProgressAccessor);
    }

    [WinFormsFact]
    public void AnimatedControlRenderer_Invalidate_InvalidatesControl()
    {
        using SubControl control = new();
        control.CreateControl();
        using SubAnimatedControlRenderer renderer = new(control);

        int invalidatedCount = 0;
        control.Invalidated += (s, e) => invalidatedCount++;

        renderer.Invalidate();

        Assert.True(invalidatedCount >= 1);
    }

    [WinFormsFact]
    public void AnimatedControlRenderer_Dispose_DoesNotThrow_WhenNeverStarted()
    {
        using Control control = new();
        SubAnimatedControlRenderer renderer = new(control);

        // Never started, so this must not require the AnimationManager singleton.
        renderer.Dispose();
    }

    private sealed class SubControl : Control
    {
    }

    private sealed class SubAnimatedControlRenderer(Control control) : AnimatedControlRenderer(control)
    {
        public float AnimationProgressAccessor => AnimationProgress;

        public override void RenderControl(Graphics graphics)
        {
        }

        protected override (int animationDuration, AnimationCycle animationCycle) OnAnimationStarted()
            => (100, AnimationCycle.Once);

        protected override void OnAnimationEnded()
        {
        }

        protected override void OnAnimationStopped()
        {
        }
    }
}
