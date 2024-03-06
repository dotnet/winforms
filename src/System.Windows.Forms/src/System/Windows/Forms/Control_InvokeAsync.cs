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
        if (!IsHandleCreated)
            throw new InvalidOperationException("Control handle not created.");

        var tcs = new TaskCompletionSource<T>();

        var callback = async () =>
        {
            try
            {
                var result = await asyncFunc().ConfigureAwait(false);
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        };

        // Marshal the asyncFunc execution back to the UI thread
        Invoke(async () => await Task.Run(callback).ConfigureAwait(false));

        T? result = default;

        try
        {
            result = tcs.Task.Result;
        }
        catch (Exception ex)
        {
            ExceptionDispatchInfo.Throw(ex);
        }

        return result;
    }

    /// <summary>
    ///  Executes the specified asynchronous function on the thread that owns the control's handle.
    /// </summary>
    /// <typeparam name="T">The type of the input argument to be converted into the args array.</typeparam>
    /// <typeparam name="U">The return type of the asynchronous function.</typeparam>
    /// <param name="asyncFunc">
    ///  The asynchronous function to execute, which takes an input of type T
    ///  and returns a <see cref="Task{U}"/>.</param>
    /// <param name="input">The input of type T to be used by the asynchronous function.</param>
    /// <returns>A task representing the operation and containing the function's result of type U.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the control's handle is not yet created.</exception>
    public Task<U> InvokeAsync<T, U>(Func<T, Task<U>> asyncFunc, T input)
    {
        var tcs = new TaskCompletionSource<U>();

        if (!IsHandleCreated)
        {
            tcs.SetException(new InvalidOperationException("Control handle not created."));
            return tcs.Task;
        }

        BeginInvoke(new Action(async () =>
        {
            try
            {
                U result = await asyncFunc(input).ConfigureAwait(true);
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }));

        return tcs.Task;
    }

    /// <summary>
    ///  Executes the specified delegate on the thread that owns the control's handle.
    /// </summary>
    /// <param name="method">The delegate to execute.</param>
    /// <param name="args">The arguments to pass to the delegate.</param>
    /// <returns>A task representing the operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the control's handle is not yet created.</exception>
    public Task<object?> InvokeAsync(Delegate method, params object[] args)
    {
        var tcs = new TaskCompletionSource<object?>();

        BeginInvoke(new Action(() =>
        {
            try
            {
                object? result = method.DynamicInvoke(args);
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }));

        return tcs.Task;
    }

    /// <summary>
    ///  Executes the specified asynchronous function on the thread that owns the control's handle.
    /// </summary>
    /// <typeparam name="T">The type of the input argument to be converted into the args array.</typeparam>
    /// <typeparam name="U">The return type of the asynchronous function.</typeparam>
    /// <param name="asyncFunc">
    ///  The asynchronous function to execute, which takes an input of type T
    ///  and returns a <see cref="Task{U}"/>.</param>
    /// <param name="input">The input of type T to be used by the asynchronous function.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>A task representing the operation and containing the function's result of type U.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the control's handle is not yet created.</exception>
    public Task<U> InvokeAsync<T, U>(Func<T, CancellationToken, Task<U>> asyncFunc, T input, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<U>();

        if (!IsHandleCreated || cancellationToken.IsCancellationRequested)
        {
            tcs.SetException(new InvalidOperationException("Control handle not created or operation cancelled."));
            return tcs.Task;
        }

        BeginInvoke(new Action(async () =>
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    tcs.SetCanceled();
                    return;
                }

                U result = await asyncFunc(input, cancellationToken).ConfigureAwait(true);
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    tcs.SetException(ex);
                }
                else
                {
                    tcs.SetCanceled();
                }
            }
        }));

        return tcs.Task;
    }

    /// <summary>
    ///  Executes the specified delegate on the thread that owns the control's handle.
    /// </summary>
    /// <param name="method">The delegate to execute.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <param name="args">The arguments to pass to the delegate.</param>
    /// <returns>A task representing the operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the control's handle is not yet created.</exception>
    public Task<object?> InvokeAsync(Delegate method, CancellationToken cancellationToken, params object[] args)
    {
        var tcs = new TaskCompletionSource<object?>();

        if (!IsHandleCreated || cancellationToken.IsCancellationRequested)
        {
            tcs.SetException(new InvalidOperationException("Control handle not created or operation cancelled."));
            return tcs.Task;
        }

        BeginInvoke(new Action(() =>
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    tcs.SetCanceled();
                    return;
                }

                object? result = method.DynamicInvoke(args);
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    tcs.SetException(ex);
                }
                else
                {
                    tcs.SetCanceled();
                }
            }
        }));

        return tcs.Task;
    }
}
