// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  Invokes the specified synchronous function asynchronously on the thread that owns the control's handle.
    /// </summary>
    /// <param name="action">The synchronous action to execute.</param>
    /// <returns>A task representing the operation and containing the function's result.</returns>
    public async Task InvokeAsync(Action action)
        => await InvokeAsync(action, CancellationToken.None).ConfigureAwait(true);

    /// <summary>
    ///  Invokes the specified synchronous function asynchronously on the thread that owns the control's handle.
    /// </summary>
    /// <param name="action">The synchronous action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation and containing the function's result.</returns>
    public async Task InvokeAsync(Action action, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (cancellationToken.IsCancellationRequested)
            return;

        TaskCompletionSource<object?> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        if (InvokeRequired)
        {
            BeginInvoke(WrappedAction);
        }
        else
        {
            WrappedAction();
        }

        using (cancellationToken.Register(() => tcs.SetCanceled(), useSynchronizationContext: false))
        {
            await tcs.Task.ConfigureAwait(false);
        }

        void WrappedAction()
        {
            try
            {
                action();
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }
    }

    /// <summary>
    ///  Invokes the specified synchronous function asynchronously on the thread that owns the control's handle.
    /// </summary>
    /// <typeparam name="TResult">The return type of the synchronous function.</typeparam>
    /// <param name="syncFunction">The synchronous function to execute.</param>
    /// <returns>A task representing the operation and containing the function's result.</returns>
    public Task<TResult> InvokeAsync<TResult>(Func<TResult> syncFunction)
        => InvokeAsync(syncFunction, CancellationToken.None);

    /// <summary>
    ///  Invokes the specified synchronous function asynchronously on the thread that owns the control's handle.
    /// </summary>
    /// <typeparam name="TResult">The return type of the synchronous function.</typeparam>
    /// <param name="syncFunction">The synchronous function to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation and containing the function's result.</returns>
    public async Task<TResult> InvokeAsync<TResult>(Func<TResult> syncFunction, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(syncFunction);

        if (cancellationToken.IsCancellationRequested)
            return default!;

        TaskCompletionSource<TResult> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        void WrappedFunction()
        {
            try
            {
                TResult result = syncFunction();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }

        if (InvokeRequired)
        {
            BeginInvoke(WrappedFunction);
        }
        else
        {
            WrappedFunction();
        }

        using (cancellationToken.Register(() => tcs.SetCanceled(), useSynchronizationContext: false))
        {
            return await tcs.Task.ConfigureAwait(false);
        }
    }

    /// <summary>
    ///  Invokes the specified asynchronous function on the thread that owns the control's handle.
    /// </summary>
    /// <param name="asyncFunc">The asynchronous function to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation.</returns>
    public async Task InvokeAsync(Func<Task> asyncFunc, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(asyncFunc);

        if (cancellationToken.IsCancellationRequested)
            return;

        TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        if (InvokeRequired)
        {
            BeginInvoke(async () => await WrappedFunctionAsync().ConfigureAwait(true));
        }
        else
        {
            await WrappedFunctionAsync().ConfigureAwait(true);
        }

        using (cancellationToken.Register(() => tcs.SetCanceled(), useSynchronizationContext: false))
        {
            await tcs.Task.ConfigureAwait(false);
        }

        async Task WrappedFunctionAsync()
        {
            try
            {
                await asyncFunc().ConfigureAwait(true);
                tcs.SetResult();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }
    }

    /// <summary>
    ///  Executes the specified asynchronous function on the thread that owns the control's handle.
    /// </summary>
    /// <typeparam name="T">The type of the input argument to be converted into the args array.</typeparam>
    /// <param name="asyncFunc">
    ///  The asynchronous function to execute,
    ///  which takes an input of type T and returns a <see cref="Task{T}"/>.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation and containing the function's result of type T.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the control's handle is not yet created.</exception>
    public async Task<T> InvokeAsync<T>(Func<Task<T>> asyncFunc, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(asyncFunc);

        if (cancellationToken.IsCancellationRequested)
            return default!;

        TaskCompletionSource<T> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        if (InvokeRequired)
        {
            BeginInvoke(async () => await WrappedFunction().ConfigureAwait(false));
        }
        else
        {
            await WrappedFunction().ConfigureAwait(true);
        }

        using (cancellationToken.Register(() => tcs.SetCanceled(), useSynchronizationContext: false))
        {
            var result = await tcs.Task.ConfigureAwait(false);
            return result;
        }

        async Task<T> WrappedFunction()
        {
            try
            {
                T result = await asyncFunc().ConfigureAwait(true);
                tcs.SetResult(result);

                return result;
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return default!;
        }
    }
}
