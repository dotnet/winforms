// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Rendering.V10.CheckBox;
using ThreadingTimer = System.Threading.Timer;

namespace System.Windows.Forms.Rendering;

internal partial class AnimationManager
{
#pragma warning disable IDE0052 // Remove unread private members
    private readonly ThreadingTimer _timer;
#pragma warning restore IDE0052 // Remove unread private members

    private readonly Stopwatch _stopwatch;

    private readonly Dictionary<AnimatedControlRenderer, AnimationRendererItem> _renderer = [];
    private static readonly AnimationManager s_instance = new();

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
            // This is the smallest interval we can safely use.
            // If we want to go faster, we would need to use a Multimedia Timer.
            // That, however, would change the threading time-slice interval systemwide.
            // We could think about quirking that over the project settings.
            period: 1000 / FrameRate);

        _stopwatch = Stopwatch.StartNew();

        Application.ApplicationExit += (sender, e) => DisposeRenderer();
    }

    /// <summary>
    ///  Disposes the animation renderers.
    /// </summary>
    private void DisposeRenderer()
    {
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
    public static void RegisterAnimationRenderer(AnimatedControlRenderer animationRenderer, int animationDuration, AnimationCycle animationCycle)
    {
        AnimationRendererItem renderItem = new(animationRenderer, animationDuration, animationCycle)
        {
            StopwatchTarget = s_instance._stopwatch.ElapsedMilliseconds + animationDuration,
            IsRunning = true
        };

        s_instance._renderer.Add(animationRenderer, renderItem);
    }

    /// <summary>
    ///  Unregisters an animation renderer.
    /// </summary>
    /// <param name="animationRenderer">The animation renderer to unregister.</param>
    internal static void UnregisterAnimationRenderer(AnimatedControlRenderer animationRenderer)
    {
        s_instance._renderer.Remove(animationRenderer);
    }

    /// <summary>
    ///  Restarts the animation for the specified control renderer.
    /// </summary>
    /// <param name="animatedControlRenderer">The control renderer to restart the animation for.</param>
    internal static void Restart(AnimatedControlRenderer animatedControlRenderer)
    {
        if (s_instance._renderer.TryGetValue(animatedControlRenderer, out AnimationRendererItem? item))
        {
            item.StopwatchTarget = s_instance._stopwatch.ElapsedMilliseconds + item.AnimationDuration;
            item.IsRunning = true;
        }
    }

    /// <summary>
    ///  Stops the animation for the specified control renderer.
    /// </summary>
    /// <param name="animatedControlRenderer">The control renderer to stop the animation for.</param>
    internal static void Stop(AnimatedControlRenderer animatedControlRenderer)
    {
        if (s_instance._renderer.TryGetValue(animatedControlRenderer, out AnimationRendererItem? item))
        {
            item.IsRunning = false;
        }
    }

    /// <summary>
    ///  Continues the animation for the specified control renderer.
    /// </summary>
    /// <param name="animatedControlRenderer">The control renderer to continue the animation for.</param>
    internal static void Continue(AnimatedControlRenderer animatedControlRenderer)
    {
        if (s_instance._renderer.TryGetValue(animatedControlRenderer, out AnimationRendererItem? item))
        {
            item.IsRunning = true;
        }
    }

    /// <summary>
    ///  Handles the tick event of the timer.
    /// </summary>
    /// <param name="state">The state object.</param>
    private void OnTick(object? state)
    {
        foreach (AnimationRendererItem item in _renderer.Values)
        {
            if (!item.IsRunning)
            {
                continue;
            }

            item.FrameCount += item.FrameOffset;

            if (s_instance._stopwatch.ElapsedMilliseconds >= item.StopwatchTarget)
            {
                switch (item.AnimationCycle)
                {
                    case AnimationCycle.Once:
                        item.Renderer.StopAnimation();
                        item.IsRunning = false;
                        break;

                    case AnimationCycle.Loop:
                        item.FrameCount = 0;
                        break;

                    case AnimationCycle.Bounce:
                        item.FrameCount = 0;
                        item.FrameOffset = -item.FrameOffset;
                        break;
                }
            }
        }
    }
}
