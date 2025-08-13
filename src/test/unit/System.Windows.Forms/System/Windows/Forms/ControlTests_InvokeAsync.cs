// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// We need to do tests with this, so need to disable this rule.
#pragma warning disable xUnit1030 // Do not call ConfigureAwait(false) in test method
#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
public partial class ControlTests
{
    private sealed class TestControl : Control
    {
        public void EnsureHandle() => _ = Handle;
        public void DestroyTestHandle() => DestroyHandle();
    }

    [WinFormsFact]
    public async Task InvokeAsync_Action_ExecutesOnUIThread()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        int? originalThread = Environment.CurrentManagedThreadId;
        Assert.NotNull(originalThread);

        int? newTaskThread = null;
        int? invokeThread = null;

        await Task.Run(async () =>
        {
            newTaskThread = Environment.CurrentManagedThreadId;
            Assert.True(control.InvokeRequired);

            // Invoke the Action on the UI thread.
            await control.InvokeAsync(UiAccessAction)
                .ConfigureAwait(false);
        }).ConfigureAwait(true);

        Assert.NotNull(newTaskThread);
        Assert.NotEqual(originalThread.Value, newTaskThread.Value);
        Assert.Equal(originalThread, invokeThread);

        // Add verification that we're back on the original thread
        Assert.Equal(originalThread.Value, Environment.CurrentManagedThreadId);

        // Local function, which becomes the Action to be invoked.
        void UiAccessAction()
        {
            invokeThread = Environment.CurrentManagedThreadId;

            for (int i = 0; i < 10; i++)
            {
                control.Text += i.ToString();
            }
        }
    }

    [WinFormsFact]
    public async Task InvokeAsync_FuncT_ExecutesOnUIThread_AndReturnsValue()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        int? originalThread = Environment.CurrentManagedThreadId;
        Assert.NotNull(originalThread);

        int? newTaskThread = null;
        int? invokeThread = null;
        int result = 0;

        await Task.Run(async () =>
        {
            newTaskThread = Environment.CurrentManagedThreadId;
            Assert.True(control.InvokeRequired);

            // Invoke the Func<T> on the UI thread.
            result = await control.InvokeAsync(UiAccessFunc)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        Assert.NotNull(newTaskThread);
        Assert.NotEqual(originalThread.Value, newTaskThread.Value);
        Assert.Equal(originalThread, invokeThread);
        Assert.Equal(42, result);

        // Local function, which becomes the Func<T> to be invoked.
        int UiAccessFunc()
        {
            invokeThread = Environment.CurrentManagedThreadId;

            return 42;
        }
    }

    [WinFormsFact]
    public async Task InvokeAsync_AsyncCallback_ExecutesOnUIThread()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        int? originalThread = Environment.CurrentManagedThreadId;
        Assert.NotNull(originalThread);

        int? newTaskThread = null;
        int? invokeThreadBeforeAwaitInInvokeDelegate = null;
        int? invokeThreadAfterAwaitInInvokeDelegate = null;

        await Task.Run(async () =>
        {
            newTaskThread = Environment.CurrentManagedThreadId;
            Assert.True(control.InvokeRequired);

            // Invoke the async callback on the UI thread.
            await control.InvokeAsync(UiAccessAsyncCallback)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        Assert.NotNull(newTaskThread);
        Assert.NotEqual(originalThread.Value, newTaskThread.Value);
        Assert.Equal(originalThread, invokeThreadBeforeAwaitInInvokeDelegate);

        // Local function, which becomes the async callback to be invoked.
        async ValueTask UiAccessAsyncCallback(CancellationToken ct)
        {
            invokeThreadBeforeAwaitInInvokeDelegate = Environment.CurrentManagedThreadId;

            await Task.Delay(10, ct).ConfigureAwait(true);

            invokeThreadAfterAwaitInInvokeDelegate = Environment.CurrentManagedThreadId;
            Assert.Equal(invokeThreadBeforeAwaitInInvokeDelegate, invokeThreadAfterAwaitInInvokeDelegate);

            await Task.Delay(10, ct).ConfigureAwait(false);

            invokeThreadAfterAwaitInInvokeDelegate = Environment.CurrentManagedThreadId;
            Assert.NotEqual(invokeThreadBeforeAwaitInInvokeDelegate, invokeThreadAfterAwaitInInvokeDelegate);
        }
    }

    [WinFormsFact]
    public async Task InvokeAsync_AsyncCallback_ExecutesOnUIThread_ControlInvocation()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        int? originalThread = Environment.CurrentManagedThreadId;
        Assert.NotNull(originalThread);

        int? newTaskThread = null;
        int? invokeThreadInInvokeDelegate = null;

        await Task.Run(async () =>
        {
            newTaskThread = Environment.CurrentManagedThreadId;
            Assert.True(control.InvokeRequired);

            // Invoke the async callback on the UI thread.
            await control.InvokeAsync(UiAccessAsyncCallback)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        Assert.NotNull(newTaskThread);
        Assert.NotEqual(originalThread.Value, newTaskThread.Value);
        Assert.Equal(originalThread, invokeThreadInInvokeDelegate);

        // Local function, which becomes the async callback to be invoked.
        async ValueTask UiAccessAsyncCallback(CancellationToken ct)
        {
            invokeThreadInInvokeDelegate = Environment.CurrentManagedThreadId;

            await Task.Delay(10, ct).ConfigureAwait(true);
            Assert.False(control.InvokeRequired, "InvokeAsync should not be required after the first await with ConfigureAwait(true).");

            await Task.Delay(10, ct).ConfigureAwait(true);
            Assert.False(control.InvokeRequired, "InvokeAsync should not be required after a subsequent await with ConfigureAwait(true).");

            await Task.Delay(10, ct).ConfigureAwait(false);
            Assert.True(control.InvokeRequired, "InvokeAsync should always be required after an await with ConfigureAwait(false).");
        }
    }

    [WinFormsFact]
    public async Task InvokeAsync_AsyncCallbackT_ExecutesOnUIThread_AndReturnsValue()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        int? originalThread = Environment.CurrentManagedThreadId;
        Assert.NotNull(originalThread);

        int? newTaskThread = null;
        int? invokeThreadBeforeAwaitInInvokeDelegate = null;
        int? invokeThreadAfterAwaitInInvokeDelegate = null;
        int result = 0;

        await Task.Run(async () =>
        {
            newTaskThread = Environment.CurrentManagedThreadId;
            Assert.True(control.InvokeRequired);

            // Invoke the async callback on the UI thread.
            result = await control.InvokeAsync(UiAccessAsyncCallback)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        Assert.NotNull(newTaskThread);
        Assert.NotEqual(originalThread.Value, newTaskThread.Value);
        Assert.Equal(originalThread, invokeThreadBeforeAwaitInInvokeDelegate);
        Assert.Equal(99, result);

        // Local function, which becomes the async callback to be invoked.
        async ValueTask<int> UiAccessAsyncCallback(CancellationToken ct)
        {
            invokeThreadBeforeAwaitInInvokeDelegate = Environment.CurrentManagedThreadId;

            await Task.Delay(10, ct).ConfigureAwait(true);

            invokeThreadAfterAwaitInInvokeDelegate = Environment.CurrentManagedThreadId;
            Assert.Equal(invokeThreadBeforeAwaitInInvokeDelegate, invokeThreadAfterAwaitInInvokeDelegate);

            await Task.Delay(10, ct).ConfigureAwait(false);

            invokeThreadAfterAwaitInInvokeDelegate = Environment.CurrentManagedThreadId;
            Assert.NotEqual(invokeThreadBeforeAwaitInInvokeDelegate, invokeThreadAfterAwaitInInvokeDelegate);

            return 99;
        }
    }

    [WinFormsFact]
    public async Task InvokeAsync_Action_Cancellation_PreCancelledToken_ReturnsEarly()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync().ConfigureAwait(false);

        await control.InvokeAsync(
            () => throw new ArgumentOutOfRangeException("Should not run"), cts.Token)
            .ConfigureAwait(false);
    }

    [WinFormsFact]
    public async Task InvokeAsync_FuncT_Cancellation_PreCancelledToken_ReturnsDefault()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync().ConfigureAwait(false);

        int result = await control.InvokeAsync(
            CallBack,
            cts.Token).ConfigureAwait(false);

        Assert.Equal(0, result);

        // Not using the CancellationToken in the callback, but we need to meet the signature.
        static ValueTask<int> CallBack(CancellationToken ct)
        {
            throw new ArgumentOutOfRangeException("Should not run");
        }
    }

    [WinFormsFact]
    public async Task InvokeAsync_AsyncCallback_Cancellation_PreCancelledToken_ReturnsEarly()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync().ConfigureAwait(true);

        await control.InvokeAsync(
            ct => throw new ArgumentOutOfRangeException("Should not run"),
            cts.Token).ConfigureAwait(true);
    }

    [WinFormsFact]
    public async Task InvokeAsync_AsyncCallbackT_Cancellation_PreCancelledToken_ReturnsDefault()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync().ConfigureAwait(false);

        int result = await control.InvokeAsync<int>(
            ct => throw new ArgumentOutOfRangeException("Should not run"),
            cts.Token).ConfigureAwait(false);

        Assert.Equal(0, result);
    }

    [WinFormsFact]
    public async Task InvokeAsync_Action_Cancellation_BeforeExecution()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        using var cts = new CancellationTokenSource();

        // Queue multiple operations to ensure our cancelled one is truly queued
        var blockingTask = control.InvokeAsync(() => Thread.Sleep(50));

        // Immediately queue another task with pre-cancelled token
        cts.Cancel();

        var cancelledTask = control.InvokeAsync(
            () => throw new InvalidOperationException("Should not execute"),
            cts.Token);

        // The task should complete successfully without executing the callback
        // This is the documented behavior for pre-cancelled tokens
        await cancelledTask.ConfigureAwait(false);

        // The blocking task should complete normally
        await blockingTask.ConfigureAwait(false);
    }

    [WinFormsFact]
    public async Task InvokeAsync_Action_Cancellation_DuringExecution()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        using var cts = new CancellationTokenSource();
        var started = new TaskCompletionSource<bool>();

        Task task = control.InvokeAsync(async ct =>
        {
            started.SetResult(true);

            // Simulate work that checks cancellation
            for (int i = 0; i < 10; i++)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Delay(10, ct).ConfigureAwait(false);
            }
        }, cts.Token);

        // Wait for it to start
        await started.Task.ConfigureAwait(false);

        // Cancel while running
        await cts.CancelAsync().ConfigureAwait(false);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await task.ConfigureAwait(false))
            .ConfigureAwait(false);
    }

    [WinFormsFact]
    public async Task InvokeAsync_AsyncCallback_Cancellation_WhileRunning()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        using var cts = new CancellationTokenSource();

        Task task = control.InvokeAsync(async ct =>
        {
            await cts.CancelAsync().ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();

            await Task.Delay(10, ct).ConfigureAwait(false);
        }, cts.Token);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await task.ConfigureAwait(false)).ConfigureAwait(false);
    }

    [WinFormsFact]
    public async Task InvokeAsync_Throws_InvalidOperationException_IfHandleNotCreated()
    {
        using var control = new TestControl();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync(ct => default, CancellationToken.None)).ConfigureAwait(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync(
                ct => new ValueTask<int>(1),
                CancellationToken.None)).ConfigureAwait(false);

        // Test for destroyed handle
        control.EnsureHandle();
        control.DestroyTestHandle();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync(() => { })).ConfigureAwait(false);
    }

    [WinFormsFact]
    public async Task InvokeAsync_Propagates_Exception_FromCallback()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync(() => throw new InvalidOperationException()))
            .ConfigureAwait(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync<int>(() => throw new InvalidOperationException()))
            .ConfigureAwait(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync(ct => throw new InvalidOperationException()))
            .ConfigureAwait(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync<int>(ct => throw new InvalidOperationException()))
            .ConfigureAwait(false);
    }

    [WinFormsFact]
    public async Task InvokeAsync_Reentry_Supported()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        bool innerCalled = false;

        await control.InvokeAsync(async ct =>
        {
            await control.InvokeAsync(() => innerCalled = true, ct)
            .ConfigureAwait(false);
        }).ConfigureAwait(false);

        Assert.True(innerCalled);
    }

    [WinFormsFact]
    public async Task InvokeAsync_MultipleConcurrentCalls_AreThreadSafe()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        int counter = 0;
        Task[] tasks = new Task[10];

        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = control.InvokeAsync(() => Interlocked.Increment(ref counter));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        Assert.Equal(10, counter);
    }
}
#pragma warning restore xUnit1030 // Do not call ConfigureAwait(false) in test method
#pragma warning restore xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
