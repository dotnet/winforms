// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.ExceptionServices;

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  Executes an asynchronous function in a Task to avoid UI deadlocks, and blocks until the operation completes.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="asyncFunc">The asynchronous function to execute.</param>
    /// <returns>The result of the asynchronous operation.</returns>
    public T? AsyncInvoke<T>(Func<Task<T>> asyncFunc)
    {
        ArgumentNullException.ThrowIfNull(asyncFunc);

        if (!IsHandleCreated)
        {
            throw new InvalidOperationException("Control handle not created.");
        }

        // We need this to capture the result of the asynchronous operation.
        // We don't run the passed task directly, because then we couldn't return the result synchronously.
        // Instead, we run the task in a separate method and capture the result in a TaskCompletionSource.
        // Then we can block synchronously on the TaskCompletionSource's Task to get the result.
        var tcs = new TaskCompletionSource<T>();

        if (!InvokeRequired)
        {
            // We're already on the UI thread, so we spin up a new task to avoid blocking the UI thread.
            _ = Invoke(async () => await Task.Run(Callback).ConfigureAwait(true));
        }
        else
        {
            // We're already on a different thread, so we can just invoke the callback directly.
            _ = Invoke(async () => await Callback().ConfigureAwait(true));
        }

        T? result = default;

        try
        {
            result = tcs.Task.Result;
        }
        catch (Exception ex)
        {
            // Should the task-wrapper throw, we want to preserve the original exception.
            ExceptionDispatchInfo.Throw(ex);
        }

        return result;

        async Task Callback()
        {
            try
            {
                var result = await asyncFunc().ConfigureAwait(true);
                tcs.TrySetResult(result);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }
    }

    /// <summary>
    ///  Invokes the specified synchronous function asynchronously on the thread that owns the control's handle.
    /// </summary>
    /// <typeparam name="T">The return type of the synchronous function.</typeparam>
    /// <param name="syncFunction">The synchronous function to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation and containing the function's result.</returns>
    public async Task<T> InvokeSyncAsync<T>(Func<T> syncFunction, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<T>();

        if (!IsHandleCreated)
        {
            tcs.TrySetException(new InvalidOperationException("Control handle not created."));

            return await tcs.Task.ConfigureAwait(true);
        }

        var result = await Task.Run(
            () => Invoke(() => syncFunction()),
            cancellationToken).ConfigureAwait(true);

        tcs.TrySetResult(result);

        return await tcs.Task.ConfigureAwait(true);
    }

    /// <summary>
    ///  Invokes the specified asynchronous function on the thread that owns the control's handle.
    /// </summary>
    /// <param name="function">The asynchronous function to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation.</returns>
    public async Task InvokeAsync(Func<Task> function, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource();

        if (!IsHandleCreated)
        {
            tcs.TrySetException(new InvalidOperationException("Control handle not created."));

            await tcs.Task.ConfigureAwait(true);
        }

        await Task.Run(
            () => Invoke(async () => await function().ConfigureAwait(true)),
            cancellationToken).ConfigureAwait(true);

        tcs.TrySetResult();

        await tcs.Task.ConfigureAwait(true);
    }

    /// <summary>
    ///  Executes the specified asynchronous function on the thread that owns the control's handle.
    /// </summary>
    /// <typeparam name="T">The type of the input argument to be converted into the args array.</typeparam>
    /// <param name="asyncFunc">
    ///  The asynchronous function to execute,
    ///  which takes an input of type T and returns a <see cref="Task{U}"/>.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation and containing the function's result of type U.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the control's handle is not yet created.</exception>
    public async Task<T> InvokeAsync<T>(Func<Task<T>> asyncFunc, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<T>();

        if (!IsHandleCreated)
        {
            tcs.TrySetException(new InvalidOperationException("Control handle not created."));
            return await tcs.Task.ConfigureAwait(true);
        }

        var result = await Task.Run(
            () => Invoke(async () => await asyncFunc().ConfigureAwait(true)),
            cancellationToken).ConfigureAwait(true);

        tcs.TrySetResult(result);

        return await tcs.Task.ConfigureAwait(true);
    }

    /// <summary>
    ///  Executes the specified asynchronous function on the thread that owns the control's handle.
    /// </summary>
    /// <typeparam name="T">The type of the input argument to be converted into the args array.</typeparam>
    /// <typeparam name="U">The return type of the asynchronous function.</typeparam>
    /// <param name="asyncFunc">
    ///  The asynchronous function to execute, which takes an input of type T
    ///  and returns a <see cref="Task{U}"/>.
    ///  </param>
    /// <param name="arg">The input argument to be converted into the args array.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation and containing the function's result of type U.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the control's handle is not yet created.</exception>
    public async Task<U> InvokeAsync<T, U>(Func<T, Task<U>> asyncFunc, T arg, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<U>();

        if (!IsHandleCreated)
        {
            tcs.TrySetException(new InvalidOperationException("Control handle not created."));
            return await tcs.Task.ConfigureAwait(true);
        }

        await Task.Run(
            () => Invoke(async () => await Callback().ConfigureAwait(true)),
            cancellationToken).ConfigureAwait(true);

        return await tcs.Task.ConfigureAwait(true);

        async Task Callback()
        {
            try
            {
                var result = await asyncFunc(arg).ConfigureAwait(false);
                tcs.TrySetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }
    }
}
