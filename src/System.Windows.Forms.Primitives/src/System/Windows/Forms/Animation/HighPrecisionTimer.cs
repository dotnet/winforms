// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;

namespace System.Windows.Forms.Animation;

/// <summary>
///  A static animation timer that delivers frames at up to 60 Hz on the synchronization
///  context captured for each registration.
/// </summary>
/// <remarks>
///  <para>
///   The pacer uses a single <see cref="Stopwatch"/> timeline and schedules every frame from
///   an absolute epoch. Stopping a generation cancels it without joining its worker; the active
///   generation and registration identity are verified before every dispatch, so late work is benign.
///  </para>
/// </remarks>
internal static partial class HighPrecisionTimer
{
    private const int MaximumFramesPerSecond = 60;
    private const int FallbackFramesPerSecond = 30;
    private const int ConsecutiveFaultLimit = 3;
    private const long MicrosecondsPerSecond = 1_000_000;
    private const double DriftThresholdRatio = 0.20;
    private const uint CreateWaitableTimerHighResolution = 0x00000002;
    private const uint TimerAllAccess = 0x001F0003;
    private const uint WaitObject0 = 0;
    private const uint Infinite = 0xFFFFFFFF;

    private static readonly Lock s_lock = new();
    private static readonly Dictionary<long, Registration> s_registrations = [];
    private static readonly SendOrPostCallback s_dispatchCallback = static state
        => DispatchCallback((Registration)state!);

    private static Registration[] s_registrationSnapshot = [];
    private static long s_nextId;
    private static long s_nextGeneration;
    private static int s_requestedFramesPerSecond = MaximumFramesPerSecond;
    private static int s_effectiveFramesPerSecond = MaximumFramesPerSecond;
    private static int s_highResolutionAvailable;
    private static LoopState? s_loopState;

    /// <summary>
    ///  Gets the current target frame time in milliseconds.
    /// </summary>
    internal static double TargetFrameTimeMs
        => 1000.0 / Volatile.Read(ref s_effectiveFramesPerSecond);

    /// <summary>
    ///  Gets or sets the requested pacer rate. The active rate is capped at 30 Hz when
    ///  high-resolution waitable timers are unavailable.
    /// </summary>
    internal static int TargetFramesPerSecond
    {
        get => Volatile.Read(ref s_requestedFramesPerSecond);
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaximumFramesPerSecond);

            lock (s_lock)
            {
                if (s_requestedFramesPerSecond == value)
                {
                    return;
                }

                s_requestedFramesPerSecond = value;
                if (s_loopState is not null)
                {
                    StopTimerLocked();
                    StartTimerLocked();
                }
            }
        }
    }

    /// <summary>
    ///  Gets whether high-resolution waitable timers are available for the current generation.
    /// </summary>
    internal static bool IsHighResolutionAvailable
        => Volatile.Read(ref s_highResolutionAvailable) != 0;

    /// <summary>
    ///  Registers a callback to be invoked on each animation frame tick. The current
    ///  <see cref="SynchronizationContext"/> is captured for dispatch.
    /// </summary>
    /// <param name="callback">The callback invoked for each delivered frame.</param>
    /// <returns>A <see cref="TimerRegistration"/> that unregisters the callback when disposed.</returns>
    /// <exception cref="InvalidOperationException">
    ///  Thrown when no <see cref="SynchronizationContext"/> is available on the current thread.
    /// </exception>
    internal static TimerRegistration Register(Func<HighPrecisionTimerTick, CancellationToken, ValueTask> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        SynchronizationContext syncContext = SynchronizationContext.Current
            ?? throw new InvalidOperationException(
                "A SynchronizationContext must be available on the calling thread. " +
                "Ensure registration is performed from a UI thread.");

        lock (s_lock)
        {
            long id = ++s_nextId;
            Registration registration = new(id, callback, syncContext);
            s_registrations.Add(id, registration);
            UpdateRegistrationSnapshotLocked();

            if (s_loopState is null)
            {
                StartTimerLocked();
            }

            return new TimerRegistration(id);
        }
    }

    /// <summary>
    ///  Unregisters a previously registered callback.
    /// </summary>
    internal static void Unregister(long registrationId)
        => Unregister(registrationId, expectedRegistration: null);

    private static void Unregister(long registrationId, Registration? expectedRegistration)
    {
        if (registrationId == 0)
        {
            return;
        }

        lock (s_lock)
        {
            if (!s_registrations.TryGetValue(registrationId, out Registration? registration)
                || (expectedRegistration is not null
                    && !ReferenceEquals(registration, expectedRegistration)))
            {
                return;
            }

            Volatile.Write(ref registration.IsActive, 0);
            _ = s_registrations.Remove(registrationId);
            UpdateRegistrationSnapshotLocked();

            if (s_registrations.Count == 0)
            {
                StopTimerLocked();
            }
        }
    }

    private static void StartTimerLocked()
    {
        if (s_loopState is not null)
        {
            return;
        }

        SafeWaitableTimerHandle? waitableTimer = TryCreateHighResolutionWaitableTimer();
        int framesPerSecond = waitableTimer is null
            ? Math.Min(s_requestedFramesPerSecond, FallbackFramesPerSecond)
            : s_requestedFramesPerSecond;
        LoopState state = new(++s_nextGeneration, framesPerSecond, waitableTimer);

        s_loopState = state;
        Volatile.Write(ref s_effectiveFramesPerSecond, framesPerSecond);
        Volatile.Write(ref s_highResolutionAvailable, waitableTimer is null ? 0 : 1);
        state.Start();
    }

    private static void StopTimerLocked()
    {
        LoopState? state = s_loopState;
        s_loopState = null;
        Volatile.Write(ref s_highResolutionAvailable, 0);

        // Cancellation is intentionally nonblocking. The old loop owns and disposes its resources
        // after its outstanding native wait completes, and cannot dispatch once it is no longer current.
        state?.Cancel();
    }

    private static void TimerLoop(LoopState state)
    {
        bool restart = false;

        try
        {
            long scheduledFrame = 1;

            while (!state.Token.IsCancellationRequested)
            {
                long dueTimestamp = GetScheduledTimestamp(
                    state.EpochTimestamp,
                    scheduledFrame,
                    state.FramesPerSecond);

                if (!WaitUntilDue(state, dueTimestamp))
                {
                    break;
                }

                long timestamp = Stopwatch.GetTimestamp();
                if (state.Token.IsCancellationRequested)
                {
                    break;
                }

                double expectedMilliseconds = 1000.0 / state.FramesPerSecond;
                long driftMicroseconds = StopwatchTicksToMicroseconds(timestamp - dueTimestamp);
                if (Math.Abs(driftMicroseconds) > expectedMilliseconds * 1000 * DriftThresholdRatio
                    && HighPrecisionTimerEventSource.s_log.IsEnabled(
                        EventLevel.Warning,
                        EventKeywords.None))
                {
                    HighPrecisionTimerEventSource.s_log.Drift(
                        driftMicroseconds,
                        scheduledFrame);
                }

                DispatchCallbacks(state, timestamp);

                scheduledFrame++;
                scheduledFrame = Math.Max(
                    scheduledFrame,
                    GetFirstFutureFrameIndex(
                        state.EpochTimestamp,
                        Stopwatch.GetTimestamp(),
                        state.FramesPerSecond));
            }

            restart = !state.Token.IsCancellationRequested;
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            HighPrecisionTimerEventSource.s_log.LoopFault(
                ex.GetType().FullName ?? ex.GetType().Name);
            restart = !state.Token.IsCancellationRequested;
        }
        finally
        {
            OnTimerLoopStopped(state, restart);
            state.Dispose();
        }
    }

    private static void OnTimerLoopStopped(LoopState state, bool restart)
    {
        lock (s_lock)
        {
            if (!ReferenceEquals(s_loopState, state))
            {
                return;
            }

            s_loopState = null;
            Volatile.Write(ref s_highResolutionAvailable, 0);

            if (restart && s_registrations.Count > 0)
            {
                StartTimerLocked();
            }
        }
    }

    private static bool WaitUntilDue(LoopState state, long dueTimestamp)
    {
        if (state.Token.IsCancellationRequested)
        {
            return false;
        }

        long remainingStopwatchTicks = dueTimestamp - Stopwatch.GetTimestamp();
        if (remainingStopwatchTicks <= 0)
        {
            return true;
        }

        if (remainingStopwatchTicks <= Stopwatch.Frequency / 1000)
        {
            SpinToDueTimestamp(state, dueTimestamp);
            return !state.Token.IsCancellationRequested;
        }

        if (state.WaitableTimer is not null
            && WaitForHighResolutionTimer(state.WaitableTimer, remainingStopwatchTicks))
        {
            remainingStopwatchTicks = dueTimestamp - Stopwatch.GetTimestamp();
            if (remainingStopwatchTicks <= 0)
            {
                return true;
            }

            if (remainingStopwatchTicks <= Stopwatch.Frequency / 1000)
            {
                SpinToDueTimestamp(state, dueTimestamp);
                return !state.Token.IsCancellationRequested;
            }
        }

        return WaitCoarselyUntilDue(state, dueTimestamp);
    }

    private static bool WaitCoarselyUntilDue(LoopState state, long dueTimestamp)
    {
        long residualSpinThreshold = Stopwatch.Frequency / 1000;

        while (!state.Token.IsCancellationRequested)
        {
            long remainingStopwatchTicks = dueTimestamp - Stopwatch.GetTimestamp();
            if (remainingStopwatchTicks <= 0)
            {
                return true;
            }

            if (remainingStopwatchTicks <= residualSpinThreshold)
            {
                SpinToDueTimestamp(state, dueTimestamp);
                return !state.Token.IsCancellationRequested;
            }

            long sleepMilliseconds = Math.Max(
                1,
                StopwatchTicksToMilliseconds(remainingStopwatchTicks - residualSpinThreshold));
            Thread.Sleep((int)Math.Min(sleepMilliseconds, int.MaxValue));
        }

        return false;
    }

    private static void SpinToDueTimestamp(LoopState state, long dueTimestamp)
    {
        long spinStart = Stopwatch.GetTimestamp();
        SpinWait spinner = default;

        while (!state.Token.IsCancellationRequested
            && Stopwatch.GetTimestamp() < dueTimestamp)
        {
            // This path is entered only for the sub-millisecond remainder after a wait.
            spinner.SpinOnce(sleep1Threshold: -1);
        }

        long spinTicks = Stopwatch.GetTimestamp() - spinStart;
        TraceResidualSpin(spinTicks);
    }

    private static void TraceResidualSpin(long spinTicks)
    {
        if (spinTicks > 0
            && HighPrecisionTimerEventSource.s_log.IsEnabled(
                EventLevel.Informational,
                EventKeywords.None))
        {
            HighPrecisionTimerEventSource.s_log.ResidualSpin(
                StopwatchTicksToMicroseconds(spinTicks));
        }
    }

    private static bool WaitForHighResolutionTimer(
        SafeWaitableTimerHandle waitableTimer,
        long remainingStopwatchTicks)
    {
        long relativeDueTime = -Math.Max(
            1,
            StopwatchTicksToTimeSpanTicks(remainingStopwatchTicks));

        return NativeMethods.SetWaitableTimer(
                waitableTimer,
                in relativeDueTime,
                period: 0,
                completionRoutine: IntPtr.Zero,
                argumentToCompletionRoutine: IntPtr.Zero,
                resume: false)
            && NativeMethods.WaitForSingleObject(waitableTimer, Infinite) == WaitObject0;
    }

    private static void DispatchCallbacks(LoopState state, long timestamp)
    {
        if (!ReferenceEquals(Volatile.Read(ref s_loopState), state))
        {
            return;
        }

        Registration[] registrations = Volatile.Read(ref s_registrationSnapshot);
        for (int i = 0; i < registrations.Length; i++)
        {
            Registration registration = registrations[i];
            if (Volatile.Read(ref registration.IsActive) == 0)
            {
                continue;
            }

            if (Interlocked.CompareExchange(ref registration.InFlight, 1, 0) != 0)
            {
                int droppedFrames = Interlocked.Increment(ref registration.DroppedFrames);
                HighPrecisionTimerEventSource.s_log.DroppedFrames(
                    registration.Id,
                    droppedFrames);
                continue;
            }

            long previousTimestamp = Interlocked.Exchange(
                ref registration.LastDeliveredTimestamp,
                timestamp);
            long elapsedTimestamp = previousTimestamp == 0
                ? state.FramePeriodStopwatchTicks
                : timestamp - previousTimestamp;
            long frameIndex = Interlocked.Increment(ref registration.FrameIndex) - 1;
            int dropped = Interlocked.Exchange(ref registration.DroppedFrames, 0);

            registration.PendingTick = new HighPrecisionTimerTick
            {
                Timestamp = StopwatchTicksToTimeSpan(timestamp - state.EpochTimestamp),
                Elapsed = StopwatchTicksToTimeSpan(elapsedTimestamp),
                DroppedFrames = dropped,
                FrameIndex = frameIndex
            };
            registration.LoopState = state;

            try
            {
                registration.SyncContext.Post(s_dispatchCallback, registration);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                TraceCallbackFault(registration, ex);
                CompleteCallback(registration, succeeded: false);
            }
        }
    }

    private static void DispatchCallback(Registration registration)
    {
        LoopState? state = registration.LoopState;
        if (state is null
            || Volatile.Read(ref registration.IsActive) == 0
            || !ReferenceEquals(Volatile.Read(ref s_loopState), state))
        {
            Interlocked.Exchange(ref registration.InFlight, 0);
            return;
        }

        try
        {
            ValueTask callbackTask = registration.Callback(registration.PendingTick, state.Token);
            if (callbackTask.IsCompletedSuccessfully)
            {
                CompleteCallback(registration, succeeded: true);
                return;
            }

            _ = AwaitCallbackAsync(registration, state, callbackTask);
        }
        catch (OperationCanceledException) when (state.Token.IsCancellationRequested)
        {
            Interlocked.Exchange(ref registration.InFlight, 0);
        }
        catch (Exception ex)
        {
            TraceCallbackFault(registration, ex);
            CompleteCallback(registration, succeeded: false);
        }
    }

    private static async Task AwaitCallbackAsync(
        Registration registration,
        LoopState state,
        ValueTask callbackTask)
    {
        try
        {
            await callbackTask.ConfigureAwait(false);
            CompleteCallback(registration, succeeded: true);
        }
        catch (OperationCanceledException) when (state.Token.IsCancellationRequested)
        {
            Interlocked.Exchange(ref registration.InFlight, 0);
        }
        catch (Exception ex)
        {
            TraceCallbackFault(registration, ex);
            CompleteCallback(registration, succeeded: false);
        }
    }

    private static void CompleteCallback(Registration registration, bool succeeded)
    {
        if (succeeded)
        {
            Interlocked.Exchange(ref registration.ConsecutiveFaults, 0);
        }
        else if (Interlocked.Increment(ref registration.ConsecutiveFaults) >= ConsecutiveFaultLimit)
        {
            Unregister(registration.Id, registration);
        }

        Interlocked.Exchange(ref registration.InFlight, 0);
    }

    private static void TraceCallbackFault(Registration registration, Exception exception)
    {
        HighPrecisionTimerEventSource.s_log.CallbackFault(
            registration.Id,
            exception.GetType().FullName ?? exception.GetType().Name);
    }

    [SupportedOSPlatform("windows10.0.17134.0")]
    private static SafeWaitableTimerHandle? TryCreateHighResolutionWaitableTimer()
    {
        if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17134))
        {
            return null;
        }

        try
        {
            SafeWaitableTimerHandle waitableTimer = NativeMethods.CreateWaitableTimerEx(
                IntPtr.Zero,
                timerName: null,
                flags: CreateWaitableTimerHighResolution,
                desiredAccess: TimerAllAccess);
            if (!waitableTimer.IsInvalid)
            {
                return waitableTimer;
            }

            waitableTimer.Dispose();
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            // The coarse path is intentionally used when this optional Windows capability is unavailable.
        }

        return null;
    }

    private static long GetScheduledTimestamp(
        long epochTimestamp,
        long frameIndex,
        int framesPerSecond)
    {
        long wholeSeconds = frameIndex / framesPerSecond;
        long partialSecondFrames = frameIndex % framesPerSecond;
        return epochTimestamp
            + (wholeSeconds * Stopwatch.Frequency)
            + ((partialSecondFrames * Stopwatch.Frequency) / framesPerSecond);
    }

    private static long GetFirstFutureFrameIndex(
        long epochTimestamp,
        long timestamp,
        int framesPerSecond)
    {
        long elapsed = Math.Max(0, timestamp - epochTimestamp);
        long wholeSeconds = elapsed / Stopwatch.Frequency;
        long remainder = elapsed % Stopwatch.Frequency;
        return (wholeSeconds * framesPerSecond)
            + ((remainder * framesPerSecond) / Stopwatch.Frequency)
            + 1;
    }

    private static long StopwatchTicksToMilliseconds(long stopwatchTicks)
        => (stopwatchTicks / Stopwatch.Frequency * 1000)
            + ((stopwatchTicks % Stopwatch.Frequency * 1000) / Stopwatch.Frequency);

    private static long StopwatchTicksToMicroseconds(long stopwatchTicks)
        => (stopwatchTicks / Stopwatch.Frequency * MicrosecondsPerSecond)
            + ((stopwatchTicks % Stopwatch.Frequency * MicrosecondsPerSecond)
                / Stopwatch.Frequency);

    private static long StopwatchTicksToTimeSpanTicks(long stopwatchTicks)
        => (stopwatchTicks / Stopwatch.Frequency * TimeSpan.TicksPerSecond)
            + ((stopwatchTicks % Stopwatch.Frequency * TimeSpan.TicksPerSecond)
                / Stopwatch.Frequency);

    private static long TimeSpanTicksToStopwatchTicks(long timeSpanTicks)
        => (timeSpanTicks / TimeSpan.TicksPerSecond * Stopwatch.Frequency)
            + ((timeSpanTicks % TimeSpan.TicksPerSecond * Stopwatch.Frequency)
                / TimeSpan.TicksPerSecond);

    private static TimeSpan StopwatchTicksToTimeSpan(long stopwatchTicks)
        => TimeSpan.FromTicks(StopwatchTicksToTimeSpanTicks(stopwatchTicks));

    /// <summary>
    ///  Gets an absolute schedule timestamp for deterministic timer tests.
    /// </summary>
    internal static long GetScheduledTimestampForTesting(
        long epochTimestamp,
        long frameIndex,
        int framesPerSecond)
        => GetScheduledTimestamp(epochTimestamp, frameIndex, framesPerSecond);

    /// <summary>
    ///  Dispatches a supplied timestamp through the current generation for allocation tests.
    /// </summary>
    internal static void DispatchCallbacksForTesting(TimeSpan timestamp)
    {
        LoopState? state = Volatile.Read(ref s_loopState);
        if (state is not null)
        {
            DispatchCallbacks(
                state,
                state.EpochTimestamp + TimeSpanTicksToStopwatchTicks(timestamp.Ticks));
        }
    }

    /// <summary>
    ///  Emits the residual-spin diagnostic for allocation tests.
    /// </summary>
    internal static void TraceResidualSpinForTesting(TimeSpan duration)
        => TraceResidualSpin(TimeSpanTicksToStopwatchTicks(duration.Ticks));

    /// <summary>
    ///  Gets the current generation for deterministic timer tests.
    /// </summary>
    internal static long CurrentGenerationForTesting
    {
        get
        {
            lock (s_lock)
            {
                return s_loopState?.Generation ?? 0;
            }
        }
    }

    /// <summary>
    ///  Gets the number of active registrations for deterministic timer tests.
    /// </summary>
    internal static int RegistrationCountForTesting
    {
        get
        {
            lock (s_lock)
            {
                return s_registrations.Count;
            }
        }
    }

    /// <summary>
    ///  Gets whether a current generation is running for deterministic timer tests.
    /// </summary>
    internal static bool IsTimerRunningForTesting
    {
        get
        {
            lock (s_lock)
            {
                return s_loopState is not null;
            }
        }
    }

    /// <summary>
    ///  Resets internal state. This test hook is serialized with registration changes and never
    ///  reuses registration identifiers, so late callbacks and disposals remain harmless.
    /// </summary>
    internal static void Reset()
    {
        lock (s_lock)
        {
            StopTimerLocked();

            foreach (Registration registration in s_registrations.Values)
            {
                Volatile.Write(ref registration.IsActive, 0);
            }

            s_registrations.Clear();
            Volatile.Write(ref s_registrationSnapshot, []);
            s_requestedFramesPerSecond = MaximumFramesPerSecond;
            Volatile.Write(ref s_effectiveFramesPerSecond, MaximumFramesPerSecond);
            s_nextGeneration++;
        }
    }

    private static void UpdateRegistrationSnapshotLocked()
    {
        Registration[] registrations = new Registration[s_registrations.Count];
        s_registrations.Values.CopyTo(registrations, 0);
        Volatile.Write(ref s_registrationSnapshot, registrations);
    }

    /// <summary>
    ///  Holds state owned by one nonblocking timer-loop generation.
    /// </summary>
    private sealed class LoopState : IDisposable
    {
        public LoopState(
            long generation,
            int framesPerSecond,
            SafeWaitableTimerHandle? waitableTimer)
        {
            CancellationSource = new CancellationTokenSource();
            Token = CancellationSource.Token;
            EpochTimestamp = Stopwatch.GetTimestamp();
            FramePeriodStopwatchTicks = GetScheduledTimestamp(
                epochTimestamp: 0,
                frameIndex: 1,
                framesPerSecond);
            Generation = generation;
            FramesPerSecond = framesPerSecond;
            WaitableTimer = waitableTimer;
        }

        public CancellationTokenSource CancellationSource { get; }
        public CancellationToken Token { get; }
        public long EpochTimestamp { get; }
        public long FramePeriodStopwatchTicks { get; }
        public long Generation { get; }
        public int FramesPerSecond { get; }
        public SafeWaitableTimerHandle? WaitableTimer { get; }

        public void Start()
        {
            _ = Task.Factory.StartNew(
                static state => TimerLoop((LoopState)state!),
                this,
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        public void Cancel() => CancellationSource.Cancel();

        public void Dispose()
        {
            WaitableTimer?.Dispose();
            CancellationSource.Dispose();
        }
    }

    /// <summary>
    ///  Holds a callback registration and its reusable dispatch state.
    /// </summary>
    private sealed class Registration(
        long id,
        Func<HighPrecisionTimerTick, CancellationToken, ValueTask> callback,
        SynchronizationContext syncContext)
    {
        public long Id { get; } = id;
        public Func<HighPrecisionTimerTick, CancellationToken, ValueTask> Callback { get; } = callback;
        public SynchronizationContext SyncContext { get; } = syncContext;
        public HighPrecisionTimerTick PendingTick;
        public LoopState? LoopState;
        public long FrameIndex;
        public long LastDeliveredTimestamp;
        public int ConsecutiveFaults;
        public int DroppedFrames;
        public int InFlight;
        public int IsActive = 1;
    }

    /// <summary>
    ///  Releases waitable-timer handles through <c>CloseHandle</c>.
    /// </summary>
    private sealed class SafeWaitableTimerHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeWaitableTimerHandle() : base(ownsHandle: true)
        {
        }

        protected override bool ReleaseHandle() => NativeMethods.CloseHandle(handle);
    }

    /// <summary>
    ///  Emits diagnostic events for timer drift, coalescing, residual spin, and callback faults.
    /// </summary>
    [EventSource(Name = "System.Windows.Forms.HighPrecisionTimer")]
    private sealed class HighPrecisionTimerEventSource : EventSource
    {
        public static readonly HighPrecisionTimerEventSource s_log = new();

        [Event(1, Level = EventLevel.Warning)]
        public void Drift(long microseconds, long frameIndex)
            => WriteEvent(1, microseconds, frameIndex);

        [Event(2, Level = EventLevel.Informational)]
        public void DroppedFrames(long registrationId, int droppedFrames)
            => WriteEvent(2, registrationId, droppedFrames);

        [Event(3, Level = EventLevel.Informational)]
        public void ResidualSpin(long microseconds)
            => WriteEvent(3, microseconds);

        [Event(4, Level = EventLevel.Error)]
        public void CallbackFault(long registrationId, string exceptionType)
            => WriteEvent(4, registrationId, exceptionType);

        [Event(5, Level = EventLevel.Error)]
        public void LoopFault(string exceptionType)
            => WriteEvent(5, exceptionType);
    }

    private static partial class NativeMethods
    {
        [LibraryImport(
            "kernel32.dll",
            EntryPoint = "CreateWaitableTimerExW",
            SetLastError = true,
            StringMarshalling = StringMarshalling.Utf16)]
        internal static partial SafeWaitableTimerHandle CreateWaitableTimerEx(
            IntPtr timerAttributes,
            string? timerName,
            uint flags,
            uint desiredAccess);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetWaitableTimer(
            SafeWaitableTimerHandle timer,
            in long dueTime,
            int period,
            IntPtr completionRoutine,
            IntPtr argumentToCompletionRoutine,
            [MarshalAs(UnmanagedType.Bool)] bool resume);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        internal static partial uint WaitForSingleObject(
            SafeWaitableTimerHandle handle,
            uint milliseconds);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool CloseHandle(IntPtr handle);
    }

    /// <summary>
    ///  Represents a timer registration. Dispose to unregister.
    /// </summary>
    internal readonly struct TimerRegistration : IDisposable
    {
        private readonly long _id;

        internal TimerRegistration(long id) => _id = id;

        /// <summary>
        ///  Gets the registration identifier.
        /// </summary>
        public long Id => _id;

        /// <summary>
        ///  Unregisters this callback from the timer.
        /// </summary>
        public void Dispose() => Unregister(_id);
    }
}
