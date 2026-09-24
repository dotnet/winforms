// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using System.Windows.Forms.Rendering.Animation;

namespace System.Windows.Forms.Tests;

// NOTE: AnimationManager is per UI thread and driven by the real HighPrecisionTimer. To keep
// these unit tests deterministic, they exercise the AnimatedControlRenderer contract that does
// NOT touch AnimationManager. Start/stop/cycle behavior is covered by AnimationManagerTests.
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

    [WinFormsFact]
    public void AnimatedFocusIndicatorRenderer_SetFocusedWithoutAnimation_SynchronizesColor()
    {
        using Control control = new();
        int invalidatedCount = 0;
        using AnimatedFocusIndicatorRenderer renderer = new(control, () => invalidatedCount++);

        renderer.SetFocused(focused: true, animate: false);

        Assert.Equal(1f, renderer.FocusAmount);
        Assert.Equal(
            Color.Red.ToArgb(),
            renderer.GetCurrentColor(Color.Black, Color.Red).ToArgb());
        Assert.Equal(1, invalidatedCount);

        renderer.SetFocused(focused: false, animate: false);

        Assert.Equal(0f, renderer.FocusAmount);
        Assert.Equal(
            Color.Black.ToArgb(),
            renderer.GetCurrentColor(Color.Black, Color.Red).ToArgb());
        Assert.Equal(2, invalidatedCount);
    }

    [WinFormsFact]
    public void AnimatedFocusIndicatorRenderer_AnimationProc_ReversesFromCurrentBlend()
    {
        using SystemVisualSettingsTestScope settingsScope = new(clientAreaAnimationEnabled: true);
        using Control control = new();
        using AnimatedFocusIndicatorRenderer renderer = new(control, () => { });

        renderer.SetFocused(focused: true, animate: true);
        renderer.AnimationProc(0.5f);

        Assert.Equal(0.75f, renderer.FocusAmount, precision: 3);
        Assert.Equal(Color.FromArgb(255, 191, 0, 0), renderer.GetCurrentColor(Color.Black, Color.Red));

        renderer.SetFocused(focused: false, animate: true);
        renderer.AnimationProc(0.5f);

        Assert.Equal(0.1875f, renderer.FocusAmount, precision: 4);

        renderer.EndAnimation();

        Assert.False(renderer.IsRunning);
        Assert.Equal(0f, renderer.FocusAmount);
    }

    [WinFormsFact]
    public void AnimatedFocusIndicatorRenderer_DrawRoundedFocusIndicator_ClipsToBottomBand()
    {
        using Control control = new();
        using AnimatedFocusIndicatorRenderer renderer = new(control, () => { });
        renderer.Synchronize(focused: true, invalidate: false);

        using Bitmap bitmap = new(32, 24);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(1, 1, 29, 21);
        const int cornerSize = 14;
        const int focusBandHeight = 4;

        renderer.DrawRoundedFocusIndicator(
            graphics,
            bounds,
            cornerSize,
            borderThickness: 1,
            focusBandHeight,
            borderColor: Color.Black,
            focusColor: Color.Red);

        List<Point> paintedPixels = [];
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y).A > 0)
                {
                    paintedPixels.Add(new Point(x, y));
                }
            }
        }

        Assert.NotEmpty(paintedPixels);
        Assert.All(
            paintedPixels,
            point => Assert.InRange(
                point.Y,
                bounds.Bottom - focusBandHeight + 1,
                bounds.Bottom));
        Assert.Contains(paintedPixels, point => point.X < bounds.Left + (cornerSize / 2));
        Assert.Contains(paintedPixels, point => point.X > bounds.Right - (cornerSize / 2));

        graphics.Clear(Color.Transparent);
        renderer.Synchronize(focused: false, invalidate: false);
        renderer.DrawRoundedFocusIndicator(
            graphics,
            bounds,
            cornerSize,
            borderThickness: 1,
            focusBandHeight,
            borderColor: Color.Black,
            focusColor: Color.Red);

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                Assert.Equal(0, bitmap.GetPixel(x, y).A);
            }
        }
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
