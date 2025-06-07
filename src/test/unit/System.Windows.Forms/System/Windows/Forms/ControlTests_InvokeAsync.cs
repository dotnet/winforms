// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public partial class ControlTests
{
    private sealed class TestControl : Control
    {
        public void EnsureHandle() => _ = Handle;

        public void DestroyTestHandle() => DestroyHandle();
    }

    [WinFormsFact]
    [Diagnostics.CodeAnalysis.SuppressMessage(
        category: "Usage",
        checkId: "xUnit1051",
        Justification = "We need to test a series of specific overloads and/or exceptions.")]
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
            await control.InvokeAsync(uiAccessAction)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        Assert.NotNull(newTaskThread);
        Assert.NotEqual(originalThread.Value, newTaskThread.Value);
        Assert.Equal(originalThread, invokeThread);

        // Local function, which becomes the Action to be invoked.
        void uiAccessAction()
        {
            invokeThread = Environment.CurrentManagedThreadId;

            for (int i = 0; i < 10; i++)
            {
                control.Text += i.ToString();
            }
        }
    }

    [WinFormsFact]
    [Diagnostics.CodeAnalysis.SuppressMessage(
        category: "Usage",
        checkId: "xUnit1051",
        Justification = "We need to test a series of specific overloads and/or exceptions.")]
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
            result = await control.InvokeAsync(uiAccessFunc)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        Assert.NotNull(newTaskThread);
        Assert.NotEqual(originalThread.Value, newTaskThread.Value);
        Assert.Equal(originalThread, invokeThread);
        Assert.Equal(42, result);

        // Local function, which becomes the Func<T> to be invoked.
        int uiAccessFunc()
        {
            invokeThread = Environment.CurrentManagedThreadId;

            return 42;
        }
    }

    [WinFormsFact]
    [Diagnostics.CodeAnalysis.SuppressMessage(
        category: "Usage",
        checkId: "xUnit1051",
        Justification = "We need to test a series of specific overloads and/or exceptions.")]
    public async Task InvokeAsync_AsyncCallback_ExecutesOnUIThread()
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

            // Invoke the async callback on the UI thread.
            await control.InvokeAsync(uiAccessAsyncCallback)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        Assert.NotNull(newTaskThread);
        Assert.NotEqual(originalThread.Value, newTaskThread.Value);
        Assert.Equal(originalThread, invokeThread);

        // Local function, which becomes the async callback to be invoked.
        async ValueTask uiAccessAsyncCallback(CancellationToken ct)
        {
            await Task.Delay(10, ct).ConfigureAwait(false);
            invokeThread = Environment.CurrentManagedThreadId;
        }
    }

    [WinFormsFact]
    [Diagnostics.CodeAnalysis.SuppressMessage(
        category: "Usage",
        checkId: "xUnit1051",
        Justification = "We need to test a series of specific overloads and/or exceptions.")]
    public async Task InvokeAsync_AsyncCallbackT_ExecutesOnUIThread_AndReturnsValue()
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

            // Invoke the async callback on the UI thread.
            result = await control.InvokeAsync(uiAccessAsyncCallback)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);

        Assert.NotNull(newTaskThread);
        Assert.NotEqual(originalThread.Value, newTaskThread.Value);
        Assert.Equal(originalThread, invokeThread);
        Assert.Equal(99, result);

        // Local function, which becomes the async callback to be invoked.
        async ValueTask<int> uiAccessAsyncCallback(CancellationToken ct)
        {
            await Task.Delay(10, ct).ConfigureAwait(false);
            invokeThread = Environment.CurrentManagedThreadId;

            return 99;
        }
    }

    [WinFormsFact]
    public async Task InvokeAsync_Action_Cancellation_PreCancelledToken_ReturnsEarly()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await control.InvokeAsync(
            () => throw new ArgumentOutOfRangeException("Should not run"), cts.Token);
    }

    [WinFormsFact]
    public async Task InvokeAsync_FuncT_Cancellation_PreCancelledToken_ReturnsDefault()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        int result = await control.InvokeAsync(
            CallBack,
            cts.Token);

        Assert.Equal(0, result);

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
        await cts.CancelAsync();

        await control.InvokeAsync(
            ct => throw new ArgumentOutOfRangeException("Should not run"),
            cts.Token);
    }

    [WinFormsFact]
    public async Task InvokeAsync_AsyncCallbackT_Cancellation_PreCancelledToken_ReturnsDefault()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        int result = await control.InvokeAsync<int>(
            ct => throw new ArgumentOutOfRangeException("Should not run"),
            cts.Token);

        Assert.Equal(0, result);
    }

    [WinFormsFact]
    public async Task InvokeAsync_Action_Cancellation_WhileQueued()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        using var cts = new CancellationTokenSource();

        Task task = control.InvokeAsync(() =>
        {
            Thread.Sleep(50);
        }, cts.Token);

        await cts.CancelAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await task.ConfigureAwait(false));
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
            async () => await task.ConfigureAwait(false));
    }

    [WinFormsFact]
    public async Task InvokeAsync_Throws_InvalidOperationException_IfHandleNotCreated()
    {
        using var control = new TestControl();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync(ct => default, CancellationToken.None));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync(
                ct => new ValueTask<int>(1),
                CancellationToken.None));
    }

    [WinFormsFact]
    [Diagnostics.CodeAnalysis.SuppressMessage(
        category: "Usage",
        checkId: "xUnit1051",
        Justification = "We need to test a series of specific overloads and/or exceptions.")]
    public async Task InvokeAsync_Propagates_Exception_FromCallback()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync(() => throw new InvalidOperationException()));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync<int>(() => throw new InvalidOperationException()));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync(ct => throw new InvalidOperationException()));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            control.InvokeAsync<int>(ct => throw new InvalidOperationException()));
    }

    [WinFormsFact]
    [Diagnostics.CodeAnalysis.SuppressMessage(
        category: "Usage",
        checkId: "xUnit1051",
        Justification = "We need to test a series of specific overloads and/or exceptions.")]
    public async Task InvokeAsync_Reentry_Supported()
    {
        using var control = new TestControl();
        control.EnsureHandle();

        bool innerCalled = false;

        await control.InvokeAsync(async ct =>
        {
            await control.InvokeAsync(() => innerCalled = true, ct).ConfigureAwait(false);
        });

        Assert.True(innerCalled);
    }

    [WinFormsFact]
    [Diagnostics.CodeAnalysis.SuppressMessage(
        category: "Usage",
        checkId: "xUnit1051",
        Justification = "We need to test a series of specific overloads and/or exceptions.")]
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

        await Task.WhenAll(tasks);
        Assert.Equal(10, counter);
    }
}
