// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  Invokes the specified synchronous callback asynchronously on the thread that owns the control's handle.
    /// </summary>
    /// <param name="callback">The synchronous action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation.</returns>
    /// <remarks>
    ///  <para>
    ///   <b>Note:</b> When you pass a <see cref="CancellationToken"/> to this method, the method will return,
    ///   but the callback will still be executed. The callback will be running on the UI thread and will be
    ///   also blocking the UI thread. InvokeAsync in this case is just queuing the callback to the end of the
    ///   message queue and returns immediately, but as soon as the callback gets executed, it will still block
    ///   the UI thread for the time it is running. For this reason, it is recommended to only execute short sync running
    ///   operations in the callback, like updating a control's property or similar.
    ///  </para>
    ///  <para>
    ///   If you want to execute a long-running operation, consider using asynchronous callbacks instead,
    ///   by making sure that you use either the overload
    ///   <see cref="InvokeAsync(Func{CancellationToken, ValueTask}, CancellationToken)"/> or
    ///   <see cref="InvokeAsync{T}(Func{CancellationToken, ValueTask{T}}, CancellationToken)"/>.
    ///  </para>
    /// </remarks>
    public async Task InvokeAsync(Action callback, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using (cancellationToken.Register(completion.SetCanceled, useSynchronizationContext: false))
        {
            BeginInvoke(WrappedAction);
            await completion.Task.ConfigureAwait(false);
        }

        void WrappedAction()
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    completion.SetCanceled(cancellationToken);
                    return;
                }

                callback();
                completion.TrySetResult();
            }
            catch (Exception ex)
            {
                completion.TrySetException(ex);
            }
        }
    }

    /// <summary>
    ///  Invokes the specified synchronous callback asynchronously on the thread that owns the control's handle.
    /// </summary>
    /// <typeparam name="T">The return type of the synchronous callback.</typeparam>
    /// <param name="callback">The synchronous function to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation and containing the function's result.</returns>
    /// <remarks>
    ///  <para>
    ///   <b>Note:</b> When you pass a <see cref="CancellationToken"/> to this method, the method will return,
    ///   but the callback will still be executed. The callback will be running on the UI thread and will be
    ///   also blocking the UI thread. InvokeAsync in this case is just queuing the callback to the end of the
    ///   message queue and returns immediately, but as soon as the callback is executed, it will still block
    ///   the UI for the time it is running. For this reason, it is recommended to only execute short sync running
    ///   operations in the callback, like updating a control's property or similar.
    ///  </para>
    ///  <para>
    ///   If you want to execute a long-running operation, consider using asynchronous callbacks instead, which you use
    ///   with the overloads of InvokeAsync described below.
    ///  </para>
    ///  <para>
    ///   <b>Important:</b> Also note that if you use this overload to pass a callback which returns a <see cref="Task"/>
    ///   that this Task will NOT be awaited but return immediately and has the characteristics of an
    ///   "engage-and-forget". If you want the task which you pass to be awaited, make sure that you
    ///   use either the overload
    ///   <see cref="InvokeAsync(Func{CancellationToken, ValueTask}, CancellationToken)"/> or
    ///   <see cref="InvokeAsync{T}(Func{CancellationToken, ValueTask{T}}, CancellationToken)"/>.
    ///  </para>
    /// </remarks>
    public async Task<T> InvokeAsync<T>(Func<T> callback, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (cancellationToken.IsCancellationRequested)
        {
            return default!;
        }

        TaskCompletionSource<T> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using (cancellationToken.Register(completion.SetCanceled, useSynchronizationContext: false))
        {
            BeginInvoke(WrappedCallback);
            return await completion.Task.ConfigureAwait(false);
        }

        void WrappedCallback()
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    completion.TrySetCanceled(cancellationToken);
                    return;
                }

                T result = callback();
                completion.TrySetResult(result);
            }
            catch (Exception ex)
            {
                completion.TrySetException(ex);
            }
        }
    }

    /// <summary>
    ///  Executes the specified asynchronous callback on the thread that owns the control's handle asynchronously.
    /// </summary>
    /// <param name="callback">
    ///  The asynchronous function to execute, which takes a <see cref="CancellationToken"/>
    ///  and returns a <see cref="ValueTask"/>.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///  A task representing the operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown if the control's handle is not yet created.</exception>
    /// <remarks>
    ///  <para>
    ///   <b>Note:</b> The callback will be marshalled to the thread that owns the control's handle,
    ///   and then awaited. Exceptions will be propagated back to the caller. Also note that the returned task
    ///   is not the task associated with the callback, but a task representing the operation of marshalling the
    ///   callback to the UI thread. If you need to pass a callback returning a <see cref="Task"/> rather than a
    ///   <see cref="ValueTask"/>, use the ValueTask's constructor to create a new ValueTask which wraps the original
    ///   Task. The <see cref="CancellationToken"/> will be both taken into account when marshalling the callback to the
    ///   thread that owns the control's handle, and when executing the callback.
    ///  </para>
    ///  <para>
    ///   If you want to asynchronously execute a synchronous callback, use the overload
    ///   <see cref="InvokeAsync{T}(Func{T}, CancellationToken)"/> or the overload
    ///   <see cref="InvokeAsync(Action, CancellationToken)"/>.
    ///  </para>
    /// </remarks>
    public async Task InvokeAsync(Func<CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using (cancellationToken.Register(completion.SetCanceled, useSynchronizationContext: false))
        {
            BeginInvoke(async () => await WrappedCallbackAsync().ConfigureAwait(false));
            await completion.Task.ConfigureAwait(false);
        }

        async Task WrappedCallbackAsync()
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    completion.TrySetCanceled(cancellationToken);
                    return;
                }

                await callback(cancellationToken).ConfigureAwait(false);
                completion.TrySetResult();
            }
            catch (Exception ex)
            {
                completion.TrySetException(ex);
            }
        }
    }

    /// <summary>
    ///  Executes the specified asynchronous callback on the thread that owns the control's handle.
    /// </summary>
    /// <typeparam name="T">The return type of the asynchronous callback.</typeparam>
    /// <param name="callback">
    ///  The asynchronous function to execute, which takes a <see cref="CancellationToken"/>
    ///  and returns a <see cref="ValueTask{T}"/>.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation and containing the function's result of type T.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the control's handle is not yet created.</exception>
    /// <remarks>
    ///  <para>
    ///   <b>Note:</b> The callback will be marshalled to the thread that owns the control's handle,
    ///   and then be awaited. Exceptions will be propagated back to the caller. Also note that the returned task
    ///   is not the task associated with the callback, but a task representing the operation of marshalling the
    ///   callback to the UI thread. If you need to pass a callback returning a <see cref="Task"/> rather than a
    ///   <see cref="ValueTask"/>, use the ValueTask's constructor to create a new ValueTask which wraps the original
    ///   Task. The <see cref="CancellationToken"/> will be both taken into account when marshalling the callback to the
    ///   thread that owns the control's handle, and when executing the callback.
    ///  </para>
    ///  <para>
    ///   If you want to asynchronously execute a synchronous callback, use the overload
    ///   <see cref="InvokeAsync{T}(Func{T}, CancellationToken)"/> or the overload
    ///   <see cref="InvokeAsync(Action, CancellationToken)"/>.
    ///  </para>
    /// </remarks>
    public async Task<T> InvokeAsync<T>(Func<CancellationToken, ValueTask<T>> callback, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (cancellationToken.IsCancellationRequested)
        {
            return default!;
        }

        TaskCompletionSource<T> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using (cancellationToken.Register(completion.SetCanceled, useSynchronizationContext: false))
        {
            BeginInvoke(async () => await WrappedCallbackAsync().ConfigureAwait(false));
            return await completion.Task.ConfigureAwait(false);
        }

        async Task WrappedCallbackAsync()
        {
            try
            {
                var returnValue = await callback(cancellationToken).ConfigureAwait(false);
                completion.TrySetResult(returnValue);
            }
            catch (Exception ex)
            {
                completion.TrySetException(ex);
            }
        }
    }
}
