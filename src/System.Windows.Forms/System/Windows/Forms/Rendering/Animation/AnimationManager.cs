// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Windows.Forms.Animation;

namespace System.Windows.Forms.Rendering.Animation;

/// <summary>
///  Process-wide dispatcher that drives all <see cref="AnimatedControlRenderer"/> instances from a single
///  <c>HighPrecisionTimer</c> registration.
/// </summary>
/// <remarks>
///  <para>
///   The frame cadence is provided by <c>HighPrecisionTimer</c> (60 Hz where supported, otherwise 30 Hz).
///   Because that timer marshals its callback back to the <see cref="SynchronizationContext"/> captured when this
///   manager was constructed, the per-frame work runs on the UI thread and can invalidate controls directly.
///  </para>
/// </remarks>
internal partial class AnimationManager
{
    private readonly Stopwatch _stopwatch;
    private readonly HighPrecisionTimer.TimerRegistration _timerRegistration;

    private readonly ConcurrentDictionary<AnimatedControlRenderer, AnimationRendererItem> _renderer = [];

    private static AnimationManager? s_instance;

    private static AnimationManager Instance
        => s_instance ??= new AnimationManager();

    private AnimationManager()
    {
        _stopwatch = Stopwatch.StartNew();

        // HighPrecisionTimer captures the current SynchronizationContext and posts each tick back to it,
        // so OnFrameTickAsync runs on the UI thread. A SynchronizationContext is expected here because the
        // first animation is always started from the UI thread.
        _timerRegistration = HighPrecisionTimer.Register(OnFrameTickAsync);

        Application.ApplicationExit += (sender, e) => DisposeRenderer();
    }

    /// <summary>
    ///  Disposes the animation renderers and releases the timer registration.
    /// </summary>
    private void DisposeRenderer()
    {
        // Stop the timer.
        _timerRegistration.Dispose();

        foreach (AnimatedControlRenderer renderer in _renderer.Keys)
        {
            renderer.Dispose();
        }
    }

    /// <summary>
    ///  Registers an animation renderer.
    /// </summary>
    /// <param name="animationRenderer">The animation renderer to register.</param>
    /// <param name="animationDuration">The duration of the animation.</param>
    /// <param name="animationCycle">The animation cycle.</param>
    public static void RegisterOrUpdateAnimationRenderer(
        AnimatedControlRenderer animationRenderer,
        int animationDuration,
        AnimationCycle animationCycle)
    {
        // If the renderer is already registered, update the animation parameters.
        if (Instance._renderer.TryGetValue(animationRenderer, out AnimationRendererItem? renderItem))
        {
            renderItem.StopwatchTarget = Instance._stopwatch.ElapsedMilliseconds + animationDuration;
            renderItem.AnimationDuration = animationDuration;
            renderItem.AnimationCycle = animationCycle;

            return;
        }

        renderItem = new AnimationRendererItem(animationRenderer, animationDuration, animationCycle)
        {
            StopwatchTarget = Instance._stopwatch.ElapsedMilliseconds + animationDuration,
        };

        _ = Instance._renderer.TryAdd(animationRenderer, renderItem);
    }

    /// <summary>
    ///  Unregisters an animation renderer.
    /// </summary>
    /// <param name="animationRenderer">The animation renderer to unregister.</param>
    internal static void UnregisterAnimationRenderer(AnimatedControlRenderer animationRenderer)
    {
        _ = Instance._renderer.TryRemove(animationRenderer, out _);
    }

    internal static void Suspend(AnimatedControlRenderer animatedControlRenderer)
    {
        if (Instance._renderer.TryGetValue(animatedControlRenderer, out AnimationRendererItem? renderItem))
        {
            renderItem.Renderer.StopAnimationInternal();
        }
    }

    /// <summary>
    ///  Handles a single frame tick delivered by <c>HighPrecisionTimer</c> (on the UI thread).
    /// </summary>
    private ValueTask OnFrameTickAsync(HighPrecisionTimerTick tick, CancellationToken cancellationToken)
    {
        long elapsedStopwatchMilliseconds = _stopwatch.ElapsedMilliseconds;

        foreach (AnimationRendererItem item in _renderer.Values)
        {
            if (!item.Renderer.IsRunning)
            {
                continue;
            }

            long remainingAnimationMilliseconds = item.StopwatchTarget - elapsedStopwatchMilliseconds;

            item.FrameCount += item.FrameOffset;

            if (elapsedStopwatchMilliseconds >= item.StopwatchTarget)
            {
                switch (item.AnimationCycle)
                {
                    case AnimationCycle.Once:
                        item.Renderer.EndAnimation();
                        break;

                    case AnimationCycle.Loop:
                        item.FrameCount = 0;
                        item.StopwatchTarget = elapsedStopwatchMilliseconds + item.AnimationDuration;
                        item.Renderer.RestartAnimation();
                        break;

                    case AnimationCycle.Bounce:
                        item.FrameOffset = -item.FrameOffset;
                        item.StopwatchTarget = elapsedStopwatchMilliseconds + item.AnimationDuration;
                        item.Renderer.RestartAnimation();
                        break;
                }

                continue;
            }

            float progress = 1 - (remainingAnimationMilliseconds / (float)item.AnimationDuration);

            // We are already on the UI thread (HighPrecisionTimer marshalled us here), so invoke directly.
            item.Renderer.AnimationProc(progress);
        }

        return ValueTask.CompletedTask;
    }
}
