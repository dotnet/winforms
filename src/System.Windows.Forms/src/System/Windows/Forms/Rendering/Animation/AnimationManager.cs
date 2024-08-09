// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using ThreadingTimer = System.Threading.Timer;

namespace System.Windows.Forms.Rendering.Animation;

internal partial class AnimationManager
{
    private readonly ThreadingTimer _timer;
    private readonly Stopwatch _stopwatch;

    private readonly ConcurrentDictionary<AnimatedControlRenderer, AnimationRendererItem> _renderer = [];

    private readonly WindowsFormsSynchronizationContext? _syncContext = (WindowsFormsSynchronizationContext?)SynchronizationContext.Current;
    private static AnimationManager? s_instance;

    private static AnimationManager Instance
        => s_instance ??= new AnimationManager();

    /// <summary>
    ///  The frame rate of every animation per second.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Experience from previous projects has shown that a frame rate of 40 frames per second we are
    ///   - for simple animations - able to achieve a smooth animation while we still have a fine enough
    ///   timer resolution and wiggle room for running the actual animations. Windows traditionally operates
    ///   with a system timer tick rate of 15.625 milliseconds (64 Hz) by default. This is a compromise between
    ///   power efficiency and responsiveness. However, this can be adjusted through various means to achieve
    ///   higher resolution timers. The smallest timer resolution that Windows can typically achieve under normal
    ///   circumstances is 1 millisecond. To that end, when a multimedia timer is set using functions like `timeBeginPeriod`,
    ///   it changes the global system timer resolution, affecting the entire system including the threading timeslice.
    ///  </para>
    ///  <para>
    ///   Note, that there are parts of WPF, which are enabling the smaller timer resolution for animations. (See
    ///   https://aka.ms/Wpf_MediaContext.cs, `EnterInterlockedPresentation()`, ff. Usually WPF tries to stay in the range
    ///   of the system timer resolution and aims for 60 Herz; Media intensive WPF applications however change the resolution,
    ///   and - by own past experiences - appear not to change it back.
    ///  </para>
    /// </remarks>
    public const int FrameRate = 40;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimationManager"/> class.
    /// </summary>
    private AnimationManager()
    {
        _timer = new ThreadingTimer(
            callback: OnTick,
            state: null,
            dueTime: 0,
            // This is the smallest interval we can safely achieve.
            // If we want to go faster, we would need to use a Multimedia Timer.
            period: 1000 / FrameRate);

        _stopwatch = Stopwatch.StartNew();

        Application.ApplicationExit += (sender, e) => DisposeRenderer();
    }

    /// <summary>
    ///  Disposes the animation renderers.
    /// </summary>
    private void DisposeRenderer()
    {
        // Stop the timer:
        _timer.Dispose();

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
    ///  Handles the tick event of the timer.
    /// </summary>
    /// <param name="state">The state object.</param>
    private void OnTick(object? state)
    {
        long elapsedStopwatchMilliseconds = Instance._stopwatch.ElapsedMilliseconds;

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

            _syncContext?.Post(
                d: _ => item.Renderer.AnimationProc(progress),
                state: null);
        }
    }
}
