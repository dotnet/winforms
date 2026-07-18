// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows.Forms.Animation;

namespace System.Windows.Forms.Primitives.Tests.Animation;

/// <summary>
///  A test synchronization context that executes posted callbacks immediately
///  on the thread pool, simulating a UI message pump for testing purposes.
/// </summary>
internal sealed class TestSynchronizationContext : SynchronizationContext
{
    public override void Post(SendOrPostCallback d, object? state)
        => ThreadPool.QueueUserWorkItem(
            _ =>
            {
                SynchronizationContext? originalContext = Current;
                SetSynchronizationContext(this);

                try
                {
                    d(state);
                }
                finally
                {
                    SetSynchronizationContext(originalContext);
                }
            });

    public override void Send(SendOrPostCallback d, object? state) => d(state);
}

// The timer is process-wide static; disable parallelization so timing-sensitive
// assertions are not perturbed by concurrently running tests.
[Collection(nameof(HighPrecisionTimerTests))]
[CollectionDefinition(nameof(HighPrecisionTimerTests), DisableParallelization = true)]
public sealed class HighPrecisionTimerTests : IDisposable
{
    private readonly SynchronizationContext? _originalContext;

    public HighPrecisionTimerTests()
    {
        _originalContext = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());
    }

    public void Dispose()
    {
        HighPrecisionTimer.Reset();
        SynchronizationContext.SetSynchronizationContext(_originalContext);
    }

    [Fact]
    public async Task SingleConsumer_ReceivesTicksAtApproximatelyExpectedRate()
    {
        ConcurrentBag<double> intervals = [];
        Stopwatch stopwatch = Stopwatch.StartNew();
        double lastTick = 0;
        int tickCount = 0;
        const int TargetTicks = 30;

        using HighPrecisionTimer.TimerRegistration registration = HighPrecisionTimer.Register(
            (tick, ct) =>
            {
                double now = stopwatch.Elapsed.TotalMilliseconds;
                if (lastTick > 0)
                {
                    intervals.Add(now - lastTick);
                }

                lastTick = now;
                Interlocked.Increment(ref tickCount);
                return ValueTask.CompletedTask;
            });

        await WaitForAsync(() => tickCount >= TargetTicks);

        List<double> sorted = [.. intervals.OrderBy(x => x)];
        double targetMs = HighPrecisionTimer.TargetFrameTimeMs;

        // Relaxed bounds to remain robust on loaded CI machines: the median must be in a
        // sane band around the target frame time.
        double median = Percentile(sorted, 0.50);
        median.Should().BeLessThan(targetMs * 3.0, "the median frame interval should stay near the target");
    }

    [Fact]
    public async Task MultipleConsumers_AllReceiveTicksIndependently()
    {
        const int ConsumerCount = 5;
        const int TargetTicks = 15;
        int[] tickCounts = new int[ConsumerCount];
        HighPrecisionTimer.TimerRegistration[] registrations = new HighPrecisionTimer.TimerRegistration[ConsumerCount];

        for (int i = 0; i < ConsumerCount; i++)
        {
            int index = i;
            registrations[i] = HighPrecisionTimer.Register(
                (tick, ct) =>
                {
                    Interlocked.Increment(ref tickCounts[index]);
                    return ValueTask.CompletedTask;
                });
        }

        await WaitForAsync(() => tickCounts.Min() >= TargetTicks);

        foreach (HighPrecisionTimer.TimerRegistration registration in registrations)
        {
            registration.Dispose();
        }

        tickCounts.Should().OnlyContain(count => count >= TargetTicks);
    }

    [Fact]
    public async Task SlowConsumer_DropsFramesInsteadOfQueuing()
    {
        ConcurrentBag<HighPrecisionTimerTick> ticks = [];

        using HighPrecisionTimer.TimerRegistration registration = HighPrecisionTimer.Register(
            async (tick, ct) =>
            {
                ticks.Add(tick);
                // Simulate slow rendering (well over one frame time).
                await Task.Delay((int)(HighPrecisionTimer.TargetFrameTimeMs * 3), ct).ConfigureAwait(false);
            });

        await WaitForAsync(() => ticks.Sum(t => t.DroppedFrames) > 0, timeoutMs: 4000);

        ticks.Sum(t => t.DroppedFrames).Should().BeGreaterThan(0, "a slow consumer should report dropped frames");
    }

    [Fact]
    public async Task Registration_Disposal_StopsCallbacks()
    {
        int tickCount = 0;

        HighPrecisionTimer.TimerRegistration registration = HighPrecisionTimer.Register(
            (tick, ct) =>
            {
                Interlocked.Increment(ref tickCount);
                return ValueTask.CompletedTask;
            });

        await WaitForAsync(() => tickCount > 0);
        registration.Dispose();
        int ticksAfterDispose = Volatile.Read(ref tickCount);

        await Task.Delay(200, TestContext.Current.CancellationToken);

        // At most a couple of in-flight callbacks may land right after disposal.
        (Volatile.Read(ref tickCount) - ticksAfterDispose).Should().BeLessThanOrEqualTo(2);
    }

    [Fact]
    public void Registration_WithoutSyncContext_Throws()
    {
        SynchronizationContext? original = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(null);

        try
        {
            Action act = () => HighPrecisionTimer.Register((tick, ct) => ValueTask.CompletedTask);
            act.Should().Throw<InvalidOperationException>();
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(original);
        }
    }

    [Fact]
    public async Task TimerTick_ProvidesElapsedAndIncreasingFrameIndex()
    {
        ConcurrentBag<double> elapsedValues = [];
        long lastFrameIndex = -1;
        long firstElapsedTicks = -1;
        long firstTimestampTicks = -1;
        bool frameIndexMonotonic = true;
        int tickCount = 0;
        const int TargetTicks = 15;

        using HighPrecisionTimer.TimerRegistration registration = HighPrecisionTimer.Register(
            (tick, ct) =>
            {
                if (tick.FrameIndex <= lastFrameIndex)
                {
                    frameIndexMonotonic = false;
                }

                lastFrameIndex = tick.FrameIndex;

                if (tick.FrameIndex == 0)
                {
                    Interlocked.CompareExchange(ref firstElapsedTicks, tick.Elapsed.Ticks, -1);
                    Interlocked.CompareExchange(ref firstTimestampTicks, tick.Timestamp.Ticks, -1);
                }
                else
                {
                    elapsedValues.Add(tick.Elapsed.TotalMilliseconds);
                }

                Interlocked.Increment(ref tickCount);
                return ValueTask.CompletedTask;
            });

        await WaitForAsync(() => tickCount >= TargetTicks);

        frameIndexMonotonic.Should().BeTrue("frame indices should increase monotonically");
        TimeSpan.FromTicks(Volatile.Read(ref firstElapsedTicks)).TotalMilliseconds.Should().BeInRange(
            HighPrecisionTimer.TargetFrameTimeMs * 0.5,
            HighPrecisionTimer.TargetFrameTimeMs * 2.0);
        TimeSpan.FromTicks(Volatile.Read(ref firstTimestampTicks)).TotalMilliseconds.Should().BeInRange(
            0,
            HighPrecisionTimer.TargetFrameTimeMs * 3.0);
        elapsedValues.Should().NotBeEmpty();
        elapsedValues.Should().OnlyContain(value => value > 0, "elapsed time between ticks should be positive");
    }

    [Fact]
    public void AbsoluteSchedule_UsesStopwatchTicksWithoutSlowDrift()
    {
        const long EpochTimestamp = 37;
        const long FrameCount = 600;
        const int FramesPerSecond = 60;

        long actual = HighPrecisionTimer.GetScheduledTimestampForTesting(
            EpochTimestamp,
            FrameCount,
            FramesPerSecond);
        long expected = EpochTimestamp + (Stopwatch.Frequency * 10);

        actual.Should().Be(expected, "600 frames at 60 Hz are exactly ten seconds on the absolute schedule");
    }

    [Fact]
    public async Task RegisterAndUnregister_ConcurrentStress_DoesNotStrandTheTimer()
    {
        const int WorkerCount = 4;
        const int RegistrationsPerWorker = 50;

        Task[] workers = Enumerable.Range(0, WorkerCount)
            .Select(
                _ => Task.Run(
                    () =>
                    {
                        SynchronizationContext? originalContext = SynchronizationContext.Current;
                        SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());

                        try
                        {
                            for (int i = 0; i < RegistrationsPerWorker; i++)
                            {
                                using HighPrecisionTimer.TimerRegistration registration = HighPrecisionTimer.Register(
                                    (tick, ct) => ValueTask.CompletedTask);
                            }
                        }
                        finally
                        {
                            SynchronizationContext.SetSynchronizationContext(originalContext);
                        }
                    },
                    TestContext.Current.CancellationToken))
            .ToArray();

        await Task.WhenAll(workers);
        await WaitForAsync(
            () => HighPrecisionTimer.RegistrationCountForTesting == 0
                && !HighPrecisionTimer.IsTimerRunningForTesting);
    }

    [Fact]
    public async Task RegisterAfterLastUnregister_StartsANewGeneration()
    {
        int firstCallbackCount = 0;
        HighPrecisionTimer.TimerRegistration firstRegistration = HighPrecisionTimer.Register(
            (tick, ct) =>
            {
                Interlocked.Increment(ref firstCallbackCount);
                return ValueTask.CompletedTask;
            });

        await WaitForAsync(() => Volatile.Read(ref firstCallbackCount) > 0);
        long firstGeneration = HighPrecisionTimer.CurrentGenerationForTesting;
        firstRegistration.Dispose();

        int secondCallbackCount = 0;
        using HighPrecisionTimer.TimerRegistration secondRegistration = HighPrecisionTimer.Register(
            (tick, ct) =>
            {
                Interlocked.Increment(ref secondCallbackCount);
                return ValueTask.CompletedTask;
            });

        await WaitForAsync(() => Volatile.Read(ref secondCallbackCount) > 0);

        HighPrecisionTimer.CurrentGenerationForTesting.Should().BeGreaterThan(
            firstGeneration,
            "the replacement loop must not share a canceled generation");
    }

    [Fact]
    public async Task LatePostedCallback_AfterDisposal_IsBenign()
    {
        SynchronizationContext? originalContext = SynchronizationContext.Current;
        QueuedSynchronizationContext queuedContext = new();
        SynchronizationContext.SetSynchronizationContext(queuedContext);
        int callbackCount = 0;

        try
        {
            HighPrecisionTimer.TimerRegistration registration = HighPrecisionTimer.Register(
                (tick, ct) =>
                {
                    Interlocked.Increment(ref callbackCount);
                    return ValueTask.CompletedTask;
                });

            SynchronizationContext.SetSynchronizationContext(null);
            await WaitForAsync(() => queuedContext.Count > 0);
            registration.Dispose();
            queuedContext.ExecuteAll();

            Volatile.Read(ref callbackCount).Should().Be(
                0,
                "callbacks posted before disposal must verify that their registration remains active");
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(originalContext);
        }
    }

    [Fact]
    public async Task Reset_IsSerializedWithTheActiveGeneration()
    {
        int callbackCount = 0;
        using HighPrecisionTimer.TimerRegistration registration = HighPrecisionTimer.Register(
            (tick, ct) =>
            {
                Interlocked.Increment(ref callbackCount);
                return ValueTask.CompletedTask;
            });

        await WaitForAsync(() => Volatile.Read(ref callbackCount) > 0);
        HighPrecisionTimer.Reset();

        HighPrecisionTimer.RegistrationCountForTesting.Should().Be(0);
        HighPrecisionTimer.IsTimerRunningForTesting.Should().BeFalse();
    }

    [Fact]
    public async Task CallbackFaults_ResetAfterSuccess()
    {
        int callbackCount = 0;
        using HighPrecisionTimer.TimerRegistration registration = HighPrecisionTimer.Register(
            (tick, ct) =>
            {
                int currentCallback = Interlocked.Increment(ref callbackCount);
                if (currentCallback is 1 or 3 or 4)
                {
                    throw new InvalidOperationException();
                }

                return ValueTask.CompletedTask;
            });

        await WaitForAsync(() => Volatile.Read(ref callbackCount) >= 5);

        HighPrecisionTimer.RegistrationCountForTesting.Should().Be(
            1,
            "a successful callback resets the consecutive-fault counter");
    }

    [Fact]
    public async Task ThreeConsecutiveCallbackFaults_AutoUnregisters()
    {
        int callbackCount = 0;
        _ = HighPrecisionTimer.Register(
            (tick, ct) =>
            {
                Interlocked.Increment(ref callbackCount);
                throw new InvalidOperationException();
            });

        await WaitForAsync(() => HighPrecisionTimer.RegistrationCountForTesting == 0);

        Volatile.Read(ref callbackCount).Should().Be(3);
        HighPrecisionTimer.IsTimerRunningForTesting.Should().BeFalse();
    }

    [Fact]
    public void DispatchCallbacks_SteadyState_DoesNotAllocate()
    {
        SynchronizationContext? originalContext = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new ImmediateSynchronizationContext());

        try
        {
            using HighPrecisionTimer.TimerRegistration registration = HighPrecisionTimer.Register(
                (tick, ct) => ValueTask.CompletedTask);

            HighPrecisionTimer.DispatchCallbacksForTesting(TimeSpan.FromMilliseconds(1));
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < 100; i++)
            {
                HighPrecisionTimer.DispatchCallbacksForTesting(
                    TimeSpan.FromMilliseconds(i + 2));
            }

            long allocatedAfter = GC.GetAllocatedBytesForCurrentThread();
            allocatedAfter.Should().Be(
                allocatedBefore,
                "steady-state dispatch uses a cached callback and per-registration state");
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(originalContext);
        }
    }

    [Fact]
    public void ResidualSpinTracing_Disabled_DoesNotAllocate()
    {
        HighPrecisionTimer.TraceResidualSpinForTesting(TimeSpan.FromMilliseconds(0.5));
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 100; i++)
        {
            HighPrecisionTimer.TraceResidualSpinForTesting(TimeSpan.FromMilliseconds(0.5));
        }

        long allocatedAfter = GC.GetAllocatedBytesForCurrentThread();
        allocatedAfter.Should().Be(
            allocatedBefore,
            "disabled residual-spin diagnostics must not allocate on the timer hot path");
    }

    [Fact]
    public async Task AsyncCallback_CancellationDuringNonblockingShutdown_IsBenign()
    {
        TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource completed = new(TaskCreationOptions.RunContinuationsAsynchronously);
        HighPrecisionTimer.TimerRegistration registration = HighPrecisionTimer.Register(
            async (tick, cancellationToken) =>
            {
                started.TrySetResult();

                try
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    completed.TrySetResult();
                }
            });

        await started.Task.WaitAsync(TestContext.Current.CancellationToken);
        registration.Dispose();
        await completed.Task.WaitAsync(TestContext.Current.CancellationToken);
        await WaitForAsync(() => !HighPrecisionTimer.IsTimerRunningForTesting);

        HighPrecisionTimer.RegistrationCountForTesting.Should().Be(0);
    }

    private static double Percentile(List<double> sortedValues, double percentile)
    {
        if (sortedValues.Count == 0)
        {
            return 0;
        }

        int index = (int)Math.Ceiling(percentile * sortedValues.Count) - 1;
        return sortedValues[Math.Max(0, index)];
    }

    private static async Task WaitForAsync(Func<bool> condition, int timeoutMs = 5000)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (!condition())
        {
            if (stopwatch.ElapsedMilliseconds > timeoutMs)
            {
                throw new TimeoutException("Timed out waiting for the expected timer ticks.");
            }

            await Task.Delay(25, TestContext.Current.CancellationToken).ConfigureAwait(false);
        }
    }

    private sealed class ImmediateSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object? state) => d(state);
    }

    private sealed class QueuedSynchronizationContext : SynchronizationContext
    {
        private readonly ConcurrentQueue<(SendOrPostCallback Callback, object? State)> _callbacks = [];

        public int Count => _callbacks.Count;

        public override void Post(SendOrPostCallback d, object? state)
            => _callbacks.Enqueue((d, state));

        public void ExecuteAll()
        {
            while (_callbacks.TryDequeue(out var callback))
            {
                callback.Callback(callback.State);
            }
        }
    }
}
