// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ThreadingTimer = System.Threading.Timer;

namespace System.Windows.Forms.Rendering.Animation;

internal partial class AnimationManager
{
    private readonly ThreadingTimer _timer;
    private readonly Stopwatch _stopwatch;

    private readonly Dictionary<AnimatedControlRenderer, AnimationRendererItem> _renderer = [];

    private readonly WindowsFormsSynchronizationContext? _syncContext = (WindowsFormsSynchronizationContext?)SynchronizationContext.Current;
    private static AnimationManager? s_instance;
    private readonly Lock _lock = new();

    private static AnimationManager Instance
        => s_instance ??= new AnimationManager();

    /// <summary>
    ///  The frame rate of every animation.
    /// </summary>
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
        // stop the timer:
        _timer.Dispose();

        lock (_lock)
        {
            foreach (AnimatedControlRenderer renderer in _renderer.Keys)
            {
                renderer.Dispose();
            }
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
        lock (Instance._lock)
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

            Instance._renderer.Add(animationRenderer, renderItem);
        }
    }

    /// <summary>
    ///  Unregisters an animation renderer.
    /// </summary>
    /// <param name="animationRenderer">The animation renderer to unregister.</param>
    internal static void UnregisterAnimationRenderer(AnimatedControlRenderer animationRenderer)
    {
        lock (Instance._lock)
        {
            Instance._renderer.Remove(animationRenderer);
        }
    }

    /// <summary>
    ///  Handles the tick event of the timer.
    /// </summary>
    /// <param name="state">The state object.</param>
    private void OnTick(object? state)
    {
        lock (_lock)
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
                            item.Renderer.StopAnimation();
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

                    _syncContext?.Post(
                        d: _ => item.Renderer.AnimationProc(1),
                        state: null);

                    continue;
                }

                float progress = 1 - (remainingAnimationMilliseconds / (float)item.AnimationDuration);

                _syncContext?.Post(
                    d: _ => item.Renderer.AnimationProc(progress),
                    state: null);
            }
        }
    }
}
