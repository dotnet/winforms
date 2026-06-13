// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System.Windows.Forms.Animation;

/// <summary>
///  A high-precision static timer designed to trigger WinForms control animations
///  at 60 Hz (or 30 Hz on systems without high-resolution timer support).
///  Controls register callbacks that are marshalled back to the UI thread via
///  the captured <see cref="SynchronizationContext"/>.
/// </summary>
internal static partial class HighPrecisionTimer
{
    // Target frame intervals.
    private const double TargetFrameTimeMs60Hz = 16.667;
    private const double TargetFrameTimeMs30Hz = 33.333;

    // We aim the PeriodicTimer earlier than the target frame time to allow
    // for spin-wait refinement to hit the target precisely.
    private const double TimerTickMs60Hz = 14.0;
    private const double TimerTickMs30Hz = 30.0;

    // Maximum drift before we assert (20% of target frame time sustained over 10 frames).
    private const int MaxDriftFrames = 10;
    private const double MaxDriftThresholdRatio = 0.20;

    private static readonly Lock s_lock = new();
    private static readonly ConcurrentDictionary<long, Registration> s_registrations = new();
    private static long s_nextId;
    private static CancellationTokenSource? s_cts;
    private static Task? s_loopTask;
    private static bool s_highResolutionAvailable;
    private static double s_targetFrameTimeMs;
    private static double s_timerTickMs;

    /// <summary>
    ///  Gets the current target frame time in milliseconds.
    /// </summary>
    internal static double TargetFrameTimeMs => s_targetFrameTimeMs;

    /// <summary>
    ///  Gets whether high-resolution timing (60 Hz) is available on this system.
    /// </summary>
    internal static bool IsHighResolutionAvailable => s_highResolutionAvailable;

    /// <summary>
    ///  Registers a callback to be invoked on each animation frame tick.
    ///  The current <see cref="SynchronizationContext"/> is captured and used
    ///  to marshal the callback to the appropriate thread.
    /// </summary>
    /// <param name="callback">
    ///  The async callback invoked each frame. Receives timing information and a cancellation token.
    /// </param>
    /// <returns>A <see cref="TimerRegistration"/> that must be disposed to unregister.</returns>
    /// <exception cref="InvalidOperationException">
    ///  Thrown when no <see cref="SynchronizationContext"/> is available on the current thread.
    /// </exception>
    internal static TimerRegistration Register(Func<HighPrecisionTimerTick, CancellationToken, ValueTask> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        SynchronizationContext? syncContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException(
                "A SynchronizationContext must be available on the calling thread. " +
                "Ensure registration is performed from a UI thread.");

        long id = Interlocked.Increment(ref s_nextId);
        Registration registration = new(id, callback, syncContext);
        s_registrations.TryAdd(id, registration);

        EnsureRunning();

        return new TimerRegistration(id);
    }

    /// <summary>
    ///  Unregisters a previously registered callback.
    /// </summary>
    internal static void Unregister(long registrationId)
    {
        s_registrations.TryRemove(registrationId, out _);

        if (s_registrations.IsEmpty)
        {
            StopTimer();
        }
    }

    private static void EnsureRunning()
    {
        lock (s_lock)
        {
            if (s_loopTask is not null)
            {
                return;
            }

            s_highResolutionAvailable = TrySetHighResolutionTimerMode();
            s_targetFrameTimeMs = s_highResolutionAvailable ? TargetFrameTimeMs60Hz : TargetFrameTimeMs30Hz;
            s_timerTickMs = s_highResolutionAvailable ? TimerTickMs60Hz : TimerTickMs30Hz;

            s_cts = new CancellationTokenSource();
            CancellationToken cancellationToken = s_cts.Token;
            s_loopTask = Task.Factory.StartNew(
                () => TimerLoopAsync(cancellationToken),
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default).Unwrap();
        }
    }

    private static void StopTimer()
    {
        CancellationTokenSource? cts;

        lock (s_lock)
        {
            cts = s_cts;
            s_cts = null;
            s_loopTask = null;
        }

        if (cts is not null)
        {
            cts.Cancel();
            cts.Dispose();
        }

        // Best-effort: restore timer resolution.
        if (s_highResolutionAvailable)
        {
            ResetTimerResolution();
        }
    }

    private static async Task TimerLoopAsync(CancellationToken cancellationToken)
    {
        using PeriodicTimer periodicTimer = new(TimeSpan.FromMilliseconds(s_timerTickMs));
        Stopwatch stopwatch = Stopwatch.StartNew();
        long lastTickTimestamp = 0;
        int consecutiveDriftFrames = 0;

        try
        {
            while (await periodicTimer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                // Spin-wait refinement: if we woke up early, spin until target time.
                double targetMs = lastTickTimestamp + s_targetFrameTimeMs;
                SpinToTarget(stopwatch, targetMs);

                long currentTimestamp = stopwatch.ElapsedMilliseconds;
                double elapsed = currentTimestamp - lastTickTimestamp;

                // Drift detection: check if we are consistently overshooting.
                double drift = elapsed - s_targetFrameTimeMs;
                if (Math.Abs(drift) > s_targetFrameTimeMs * MaxDriftThresholdRatio)
                {
                    consecutiveDriftFrames++;
                    Debug.Assert(
                        consecutiveDriftFrames < MaxDriftFrames,
                        $"HighPrecisionTimer: Excessive drift detected. " +
                        $"Drift: {drift:F2}ms over {consecutiveDriftFrames} consecutive frames.");
                }
                else
                {
                    consecutiveDriftFrames = 0;
                }

                lastTickTimestamp = currentTimestamp;

                // Dispatch to all registered callbacks.
                DispatchCallbacks(
                    TimeSpan.FromMilliseconds(currentTimestamp),
                    TimeSpan.FromMilliseconds(elapsed),
                    cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Normal shutdown.
        }
    }

    private static void SpinToTarget(Stopwatch stopwatch, double targetMs)
    {
        SpinWait spinner = default;

        while (stopwatch.Elapsed.TotalMilliseconds < targetMs)
        {
            // SpinOnce with sleep1Threshold=-1 ensures we stay in PAUSE/yield
            // mode and never escalate to Thread.Sleep(1).
            spinner.SpinOnce(sleep1Threshold: -1);
        }
    }

    private static void DispatchCallbacks(
        TimeSpan timestamp,
        TimeSpan elapsed,
        CancellationToken cancellationToken)
    {
        foreach (KeyValuePair<long, Registration> kvp in s_registrations)
        {
            Registration registration = kvp.Value;

            // Skip if previous callback is still in flight (frame coalescing).
            if (Interlocked.CompareExchange(ref registration.InFlight, 1, 0) != 0)
            {
                Interlocked.Increment(ref registration.DroppedFrames);
                continue;
            }

            long frameIndex = Interlocked.Increment(ref registration.FrameIndex) - 1;
            int dropped = Interlocked.Exchange(ref registration.DroppedFrames, 0);

            HighPrecisionTimerTick tick = new()
            {
                Timestamp = timestamp,
                Elapsed = elapsed,
                DroppedFrames = dropped,
                FrameIndex = frameIndex
            };

            registration.SyncContext.Post(
                _ => _ = InvokeCallbackAsync(registration, tick, cancellationToken),
                null);
        }
    }

    private static async Task InvokeCallbackAsync(
        Registration registration,
        HighPrecisionTimerTick tick,
        CancellationToken cancellationToken)
    {
        try
        {
            await registration.Callback(tick, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Debug.Fail($"HighPrecisionTimer: Unhandled exception in callback: {ex.Message}");
        }
        finally
        {
            Interlocked.Exchange(ref registration.InFlight, 0);
        }
    }

    [SupportedOSPlatform("windows10.0.17134.0")]
    private static bool TrySetHighResolutionTimerMode()
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17134))
        {
            return false;
        }

        try
        {
            // Use timeBeginPeriod for documented, reliable high-resolution timing.
            return NativeMethods.TimeBeginPeriod(1) == 0; // TIMERR_NOERROR
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            return false;
        }
    }

    private static void ResetTimerResolution()
    {
        try
        {
            NativeMethods.TimeEndPeriod(1);
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            // Best effort.
        }
    }

    private static partial class NativeMethods
    {
        [LibraryImport("winmm.dll")]
        internal static partial int TimeBeginPeriod(int uPeriod);

        [LibraryImport("winmm.dll")]
        internal static partial int TimeEndPeriod(int uPeriod);
    }

    private sealed class Registration(
        long id,
        Func<HighPrecisionTimerTick, CancellationToken, ValueTask> callback,
        SynchronizationContext syncContext)
    {
        public long Id { get; } = id;
        public Func<HighPrecisionTimerTick, CancellationToken, ValueTask> Callback { get; } = callback;
        public SynchronizationContext SyncContext { get; } = syncContext;
        public int InFlight;
        public int DroppedFrames;
        public long FrameIndex;
    }

    /// <summary>
    ///  Represents a timer registration. Dispose to unregister.
    /// </summary>
    internal readonly struct TimerRegistration : IDisposable
    {
        private readonly long _id;

        internal TimerRegistration(long id) => _id = id;

        /// <summary>Gets the registration identifier.</summary>
        public long Id => _id;

        /// <summary>Unregisters this callback from the timer.</summary>
        public void Dispose() => Unregister(_id);
    }

    /// <summary>
    ///  Resets internal state. For testing purposes only.
    /// </summary>
    internal static void Reset()
    {
        StopTimer();
        s_registrations.Clear();
        s_nextId = 0;
    }
}
