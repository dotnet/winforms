// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Diagnostics.Tracing;
using System.Windows.Forms.Animation;

namespace System.Windows.Forms.Rendering.Animation;

/// <summary>
///  Per-UI-thread dispatcher that drives <see cref="AnimatedControlRenderer"/> instances from a single
///  <c>HighPrecisionTimer</c> registration.
/// </summary>
/// <remarks>
///  <para>
///   A manager is created on demand for each UI thread. The timer captures that thread's
///   <see cref="SynchronizationContext"/>, so per-frame work can invalidate controls directly without routing
///   animations through another message loop.
///  </para>
/// </remarks>
internal partial class AnimationManager
{
    private readonly HighPrecisionTimer.TimerRegistration _timerRegistration;
    private readonly EventHandler _threadExitHandler;
    private readonly int _threadId;

    private readonly ConcurrentDictionary<AnimatedControlRenderer, AnimationRendererItem> _renderer = [];

    private bool _hasTickTimestamp;
    private int _disposed;
    private TimeSpan _lastTickTimestamp;

    [ThreadStatic]
    private static AnimationManager? s_instance;

    private static AnimationManager Instance
        => s_instance ??= new AnimationManager();

    private AnimationManager()
    {
        _threadId = Environment.CurrentManagedThreadId;
        _threadExitHandler = OnThreadExit;
        _timerRegistration = HighPrecisionTimer.Register(OnFrameTickAsync);
        Application.ThreadExit += _threadExitHandler;
    }

    /// <summary>
    ///  Disposes the animation renderers and releases the timer registration.
    /// </summary>
    private void DisposeRenderer()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        _timerRegistration.Dispose();
        Application.ThreadExit -= _threadExitHandler;

        foreach (AnimatedControlRenderer renderer in _renderer.Keys)
        {
            _ = _renderer.TryRemove(renderer, out _);
            renderer.Dispose();
        }

        if (ReferenceEquals(s_instance, this))
        {
            s_instance = null;
        }
    }

    private void OnThreadExit(object? sender, EventArgs e)
    {
        if (Environment.CurrentManagedThreadId == _threadId)
        {
            DisposeRenderer();
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
        AnimationManager manager = Instance;

        // If the renderer is already registered, update the animation parameters.
        if (manager._renderer.TryGetValue(animationRenderer, out AnimationRendererItem? renderItem))
        {
            manager.UpdateAnimationParameters(renderItem, animationDuration, animationCycle);

            return;
        }

        renderItem = new AnimationRendererItem(animationRenderer, animationDuration, animationCycle)
        {
            TargetTimestamp = manager.GetTargetTimestamp(animationDuration)
        };

        _ = manager._renderer.TryAdd(animationRenderer, renderItem);
    }

    /// <summary>
    ///  Unregisters an animation renderer.
    /// </summary>
    /// <param name="animationRenderer">The animation renderer to unregister.</param>
    internal static void UnregisterAnimationRenderer(AnimatedControlRenderer animationRenderer)
    {
        if (s_instance is AnimationManager manager)
        {
            _ = manager._renderer.TryRemove(animationRenderer, out _);
        }
    }

    internal static void Suspend(AnimatedControlRenderer animatedControlRenderer)
    {
        if (s_instance is AnimationManager manager
            && manager._renderer.TryRemove(animatedControlRenderer, out AnimationRendererItem? renderItem))
        {
            renderItem.Renderer.StopAnimationInternal();
            return;
        }

        animatedControlRenderer.StopAnimationInternal();
    }

    /// <summary>
    ///  Handles a single frame tick delivered by <c>HighPrecisionTimer</c> (on the UI thread).
    /// </summary>
    private ValueTask OnFrameTickAsync(HighPrecisionTimerTick tick, CancellationToken cancellationToken)
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            return ValueTask.CompletedTask;
        }

        _lastTickTimestamp = tick.Timestamp;
        _hasTickTimestamp = true;

        foreach (AnimationRendererItem item in _renderer.Values)
        {
            try
            {
                ProcessRenderer(item, tick.Timestamp);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                if (AnimationManagerEventSource.s_log.IsEnabled())
                {
                    AnimationManagerEventSource.s_log.RendererFault(
                        item.Renderer.GetType().FullName ?? item.Renderer.GetType().Name,
                        ex.GetType().FullName ?? ex.GetType().Name);
                }

                QuarantineRenderer(item);
            }
        }

        return ValueTask.CompletedTask;
    }

    private static void ProcessRenderer(AnimationRendererItem item, TimeSpan timestamp)
    {
        if (!item.Renderer.IsRunning)
        {
            return;
        }

        item.TargetTimestamp ??= timestamp + TimeSpan.FromMilliseconds(item.AnimationDuration);
        TimeSpan targetTimestamp = item.TargetTimestamp.Value;
        TimeSpan remaining = targetTimestamp - timestamp;

        item.FrameCount += item.FrameOffset;

        if (timestamp >= targetTimestamp)
        {
            switch (item.AnimationCycle)
            {
                case AnimationCycle.Once:
                    item.Renderer.EndAnimation();
                    break;

                case AnimationCycle.Loop:
                    item.FrameCount = 0;
                    item.TargetTimestamp = timestamp + TimeSpan.FromMilliseconds(item.AnimationDuration);
                    item.Renderer.RestartAnimation();
                    break;

                case AnimationCycle.Bounce:
                    item.FrameOffset = -item.FrameOffset;
                    item.TargetTimestamp = timestamp + TimeSpan.FromMilliseconds(item.AnimationDuration);
                    item.Renderer.RestartAnimation();
                    break;
            }

            return;
        }

        float progress = 1 - (float)(remaining.TotalMilliseconds / item.AnimationDuration);

        // We are already on the UI thread (HighPrecisionTimer marshalled us here), so invoke directly.
        item.Renderer.AnimationProc(progress);
    }

    private void QuarantineRenderer(AnimationRendererItem item)
    {
        _ = _renderer.TryRemove(item.Renderer, out _);
        item.Renderer.StopAnimationInternal();
    }

    private TimeSpan? GetTargetTimestamp(int animationDuration)
        => _hasTickTimestamp
            ? _lastTickTimestamp + TimeSpan.FromMilliseconds(animationDuration)
            : null;

    private void UpdateAnimationParameters(
        AnimationRendererItem item,
        int animationDuration,
        AnimationCycle animationCycle)
    {
        item.AnimationDuration = animationDuration;
        item.AnimationCycle = animationCycle;
        item.TargetTimestamp = GetTargetTimestamp(animationDuration);
    }

    /// <summary>
    ///  Gets the manager associated with the calling UI thread for focused tests.
    /// </summary>
    internal static AnimationManager GetCurrentForTesting() => Instance;

    /// <summary>
    ///  Disposes the manager associated with the calling UI thread for focused tests.
    /// </summary>
    internal static void DisposeCurrentForTesting() => s_instance?.DisposeRenderer();

    /// <summary>
    ///  Processes a timer tick synchronously for focused tests.
    /// </summary>
    internal ValueTask ProcessTickForTesting(HighPrecisionTimerTick tick)
        => OnFrameTickAsync(tick, CancellationToken.None);

    /// <summary>
    ///  Gets the number of renderers currently managed by this UI-thread instance for focused tests.
    /// </summary>
    internal int RendererCountForTesting => _renderer.Count;

    /// <summary>
    ///  Emits diagnostics when an individual renderer is quarantined.
    /// </summary>
    [EventSource(Name = "System.Windows.Forms.AnimationManager")]
    private sealed class AnimationManagerEventSource : EventSource
    {
        public static readonly AnimationManagerEventSource s_log = new();

        [Event(1, Level = EventLevel.Error)]
        public void RendererFault(string rendererType, string exceptionType)
            => WriteEvent(1, rendererType, exceptionType);
    }
}
