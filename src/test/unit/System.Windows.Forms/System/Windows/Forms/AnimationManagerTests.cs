// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Animation;
using System.Windows.Forms.Rendering.Animation;

namespace System.Windows.Forms.Tests;

[Collection(nameof(AnimationManagerTests))]
[CollectionDefinition(nameof(AnimationManagerTests), DisableParallelization = true)]
public sealed class AnimationManagerTests : IDisposable
{
    public void Dispose() => AnimationManager.DisposeCurrentForTesting();

    [WinFormsFact]
    public void AnimationManager_IsPerThread()
    {
        AnimationManager firstManager = AnimationManager.GetCurrentForTesting();
        AnimationManager? secondManager = null;
        Exception? threadException = null;
        using ManualResetEventSlim completed = new();
        Thread thread = new(
            () =>
            {
                SynchronizationContext.SetSynchronizationContext(new ImmediateSynchronizationContext());

                try
                {
                    secondManager = AnimationManager.GetCurrentForTesting();
                    AnimationManager.DisposeCurrentForTesting();
                }
                catch (Exception ex)
                {
                    threadException = ex;
                }
                finally
                {
                    completed.Set();
                }
            });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        completed.Wait(TestContext.Current.CancellationToken);
        thread.Join();

        Assert.Null(threadException);
        Assert.NotNull(secondManager);
        Assert.NotSame(firstManager, secondManager);
    }

    [WinFormsFact]
    public async Task AnimationManager_UsesHighPrecisionTimerTickTimeline()
    {
        using SystemVisualSettingsTestScope settingsScope = new(clientAreaAnimationEnabled: true);
        using Control control = new();
        using TestRenderer renderer = new(control);
        renderer.StartAnimation();
        AnimationManager manager = AnimationManager.GetCurrentForTesting();

        await manager.ProcessTickForTesting(CreateTick(milliseconds: 10));
        await manager.ProcessTickForTesting(CreateTick(milliseconds: 60));

        Assert.Equal(0.5f, renderer.LastProgress, precision: 3);
    }

    [WinFormsFact]
    public async Task AnimationManager_RendererFault_DoesNotStopOtherRenderers()
    {
        using SystemVisualSettingsTestScope settingsScope = new(clientAreaAnimationEnabled: true);
        using Control control = new();
        using TestRenderer healthyRenderer = new(control);
        using TestRenderer faultingRenderer = new(control, throwOnFrame: true);
        healthyRenderer.StartAnimation();
        faultingRenderer.StartAnimation();
        AnimationManager manager = AnimationManager.GetCurrentForTesting();

        await manager.ProcessTickForTesting(CreateTick(milliseconds: 10));
        await manager.ProcessTickForTesting(CreateTick(milliseconds: 60));

        Assert.True(healthyRenderer.FrameCallCount >= 2);
        Assert.False(faultingRenderer.IsRunning);
        Assert.Equal(1, faultingRenderer.FrameCallCount);
        Assert.Equal(1, manager.RendererCountForTesting);
    }

    [WinFormsFact]
    public async Task AnimationManager_DisposedManager_ToleratesLateTick()
    {
        AnimationManager manager = AnimationManager.GetCurrentForTesting();
        AnimationManager.DisposeCurrentForTesting();

        await manager.ProcessTickForTesting(CreateTick(milliseconds: 10));
        Assert.Equal(0, manager.RendererCountForTesting);
    }

    private static HighPrecisionTimerTick CreateTick(int milliseconds)
        => new()
        {
            Timestamp = TimeSpan.FromMilliseconds(milliseconds),
            Elapsed = TimeSpan.FromMilliseconds(milliseconds),
            FrameIndex = milliseconds
        };

    private sealed class ImmediateSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object? state) => d(state);
    }

    private sealed class TestRenderer : AnimatedControlRenderer
    {
        private readonly bool _throwOnFrame;

        public TestRenderer(Control control, bool throwOnFrame = false) : base(control)
        {
            _throwOnFrame = throwOnFrame;
        }

        public int FrameCallCount { get; private set; }
        public float LastProgress { get; private set; }

        public override void AnimationProc(float animationProgress)
        {
            FrameCallCount++;
            if (_throwOnFrame)
            {
                throw new InvalidOperationException();
            }

            LastProgress = animationProgress;
            base.AnimationProc(animationProgress);
        }

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
