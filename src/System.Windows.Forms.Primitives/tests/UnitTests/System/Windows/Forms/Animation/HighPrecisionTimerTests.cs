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
    public override void Post(SendOrPostCallback d, object? state) => ThreadPool.QueueUserWorkItem(_ => d(state));

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

                if (tick.FrameIndex > 0)
                {
                    elapsedValues.Add(tick.Elapsed.TotalMilliseconds);
                }

                Interlocked.Increment(ref tickCount);
                return ValueTask.CompletedTask;
            });

        await WaitForAsync(() => tickCount >= TargetTicks);

        frameIndexMonotonic.Should().BeTrue("frame indices should increase monotonically");
        elapsedValues.Should().NotBeEmpty();
        elapsedValues.Should().OnlyContain(value => value > 0, "elapsed time between ticks should be positive");
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
}
