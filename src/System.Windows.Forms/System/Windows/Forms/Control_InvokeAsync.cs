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
    ///   <b>Note:</b> If the <see cref="CancellationToken"/> is already cancelled when this method is called,
    ///   the method returns immediately without throwing an exception. The returned task will be completed
    ///   (not cancelled) to avoid allocation overhead. If cancellation occurs after the method is called but
    ///   before the callback executes, the callback will not be executed and the task will be cancelled.
    ///  </para>
    ///  <para>
    ///   When the callback executes, it runs on the UI thread and blocks it for the duration of its execution.
    ///   InvokeAsync queues the callback to the end of the message queue and returns immediately, but once the
    ///   callback executes, it will block the UI thread. For this reason, only execute short-running synchronous
    ///   operations in the callback, such as updating control properties.
    ///  </para>
    ///  <para>
    ///   For long-running operations, consider using the asynchronous overloads
    ///   <see cref="InvokeAsync(Func{CancellationToken, ValueTask}, CancellationToken)"/> or
    ///   <see cref="InvokeAsync{T}(Func{CancellationToken, ValueTask{T}}, CancellationToken)"/>.
    ///  </para>
    ///  <para>
    ///   <b>Note:</b> If the control is disposed (or its handle is destroyed) before the
    ///   marshaled callback runs, the returned task may never complete. This is the same
    ///   behavior as <see cref="BeginInvoke(Delegate)"/>.
    ///   To avoid this, either:
    ///  </para>
    ///  <list type="bullet">
    ///   <item>
    ///    <description>Ensure the control outlives the awaited operation, or</description>
    ///   </item>
    ///   <item>
    ///    <description>
    ///     Always pass a <see cref="CancellationToken"/> that you cancel when the
    ///     control is disposing/its handle is destroyed (recommended).
    ///    </description>
    ///   </item>
    ///  </list>
    /// </remarks>
    public async Task InvokeAsync(Action callback, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (!IsHandleCreated)
        {
            throw new InvalidOperationException(SR.ErrorNoMarshalingThread);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using (cancellationToken.Register(
            () => completion.TrySetCanceled(cancellationToken),
            useSynchronizationContext: false))
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
                    completion.TrySetCanceled(cancellationToken);
                    return;
                }

                callback();
                completion.TrySetResult();
            }
            catch (Exception ex)
            {
                HandleInternalDelegateException(completion, ex, cancellationToken);
            }
        }
    }

    private static void HandleInternalDelegateException(
        TaskCompletionSource completion,
        Exception ex,
        CancellationToken cancellationToken)
    {
        if (ex is OperationCanceledException oce
            && oce.CancellationToken == cancellationToken)
        {
            completion.TrySetCanceled(cancellationToken);
        }
        else
        {
            completion.TrySetException(ex);
        }
    }

    private static void HandleInternalDelegateException<T>(
        TaskCompletionSource<T> completion,
        Exception ex,
        CancellationToken cancellationToken)
    {
        if (ex is OperationCanceledException oce
            && oce.CancellationToken == cancellationToken)
        {
            completion.TrySetCanceled(cancellationToken);
        }
        else
        {
            completion.TrySetException(ex);
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
    ///   <b>Note:</b> If the <see cref="CancellationToken"/> is already cancelled when this method is called,
    ///   the method returns immediately with a default value without throwing an exception. The returned task
    ///   will be completed (not cancelled) to avoid allocation overhead. If cancellation occurs after the method
    ///   is called but before the callback executes, the callback will not be executed and the task will be cancelled.
    ///  </para>
    ///  <para>
    ///   When the callback executes, it runs on the UI thread and blocks it for the duration of its execution.
    ///   InvokeAsync queues the callback to the end of the message queue and returns immediately, but once the
    ///   callback executes, it will block the UI thread. For this reason, only execute short-running synchronous
    ///   operations in the callback, such as retrieving control properties.
    ///  </para>
    ///  <para>
    ///   For long-running operations, consider using the asynchronous overloads
    ///   <see cref="InvokeAsync(Func{CancellationToken, ValueTask}, CancellationToken)"/> or
    ///   <see cref="InvokeAsync{T}(Func{CancellationToken, ValueTask{T}}, CancellationToken)"/>.
    ///  </para>
    ///  <para>
    ///   <b>Important:</b> If you pass a callback that returns a <see cref="Task"/> or <see cref="Task{T}"/>,
    ///   the task will NOT be awaited. The task is treated as a return value and will be returned immediately
    ///   in a "fire-and-forget" manner. To properly await asynchronous operations, use the overloads that accept
    ///   <see cref="Func{CancellationToken, ValueTask}"/> (or <see cref="ValueTask{T}"/>).
    ///  </para>
    ///  <para>
    ///   <b>Note:</b> If the control is disposed (or its handle is destroyed) before the
    ///   marshaled callback runs, the returned task may never complete. This is the same
    ///   behavior as <see cref="BeginInvoke(Delegate)"/>.
    ///   To avoid this, either:
    ///  </para>
    ///  <list type="bullet">
    ///   <item>
    ///    <description>Ensure the control outlives the awaited operation, or</description>
    ///   </item>
    ///   <item>
    ///    <description>
    ///     Always pass a <see cref="CancellationToken"/> that you cancel when the
    ///     control is disposing/its handle is destroyed (recommended).
    ///    </description>
    ///   </item>
    ///  </list>
    /// </remarks>
    public async Task<T> InvokeAsync<T>(Func<T> callback, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (!IsHandleCreated)
        {
            throw new InvalidOperationException(SR.ErrorNoMarshalingThread);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return default!;
        }

        TaskCompletionSource<T> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using (
            cancellationToken.Register(() => completion.TrySetCanceled(cancellationToken),
            useSynchronizationContext: false))
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
                HandleInternalDelegateException(completion, ex, cancellationToken);
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
    /// <exception cref="ArgumentNullException">Thrown if the callback is null.</exception>
    /// <remarks>
    ///  <para>
    ///   <b>Note:</b> If the <see cref="CancellationToken"/> is already cancelled when this method is called,
    ///   the method returns immediately without throwing an exception. The returned task will be completed
    ///   (not cancelled) to avoid allocation overhead. If cancellation occurs after the method is called,
    ///   the callback may still execute but will receive the cancellation token to handle cancellation appropriately.
    ///  </para>
    ///  <para>
    ///   The callback is marshalled to the thread that owns the control's handle and then awaited.
    ///   Exceptions thrown by the callback are propagated back to the caller. The returned task represents
    ///   the entire operation, including marshalling to the UI thread and executing the callback.
    ///  </para>
    ///  <para>
    ///   The <see cref="CancellationToken"/> is passed to the callback, allowing it to respond to cancellation
    ///   requests. The callback should check the token periodically for long-running operations.
    ///  </para>
    ///  <para>
    ///   To pass a callback that returns a <see cref="Task"/> instead of <see cref="ValueTask"/>,
    ///   wrap it using the ValueTask constructor: <c>new ValueTask(yourTask)</c>.
    ///  </para>
    ///  <para>
    ///   For synchronous operations, use <see cref="InvokeAsync(Action, CancellationToken)"/> or
    ///   <see cref="InvokeAsync{T}(Func{T}, CancellationToken)"/>.
    ///  </para>
    ///  <para>
    ///   <b>Note:</b> If the control is disposed (or its handle is destroyed) before the
    ///   marshaled callback runs, the returned task may never complete. This is the same
    ///   behavior as <see cref="BeginInvoke(Delegate)"/>.
    ///   To avoid this, either:
    ///  </para>
    ///  <list type="bullet">
    ///   <item>
    ///    <description>Ensure the control outlives the awaited operation, or</description>
    ///   </item>
    ///   <item>
    ///    <description>
    ///     Always pass a <see cref="CancellationToken"/> that you cancel when the
    ///     control is disposing/its handle is destroyed (recommended).
    ///    </description>
    ///   </item>
    ///  </list>
    /// </remarks>
    public async Task InvokeAsync(
        Func<CancellationToken, ValueTask> callback,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (!IsHandleCreated)
        {
            throw new InvalidOperationException(SR.ErrorNoMarshalingThread);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        CancellationTokenRegistration registration = cancellationToken.Register(
            () => completion.TrySetCanceled(cancellationToken),
            useSynchronizationContext: false);

        BeginInvoke(async () => await WrappedCallbackAsync()
            .ConfigureAwait(false));

        await completion.Task.ConfigureAwait(false);

        async Task WrappedCallbackAsync()
        {
            try
            {
                using (registration)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        completion.TrySetCanceled(cancellationToken);
                        return;
                    }

                    await callback(cancellationToken).ConfigureAwait(false);
                    completion.TrySetResult();
                }
            }
            catch (Exception ex)
            {
                HandleInternalDelegateException(completion, ex, cancellationToken);
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
    /// <exception cref="ArgumentNullException">Thrown if the callback is null.</exception>
    /// <remarks>
    ///  <para>
    ///   <b>Note:</b> If the <see cref="CancellationToken"/> is already cancelled when this method is called,
    ///   the method returns immediately with a default value without throwing an exception. The returned task
    ///   will be completed (not cancelled) to avoid allocation overhead. If cancellation occurs after the method
    ///   is called, the callback may still execute but will receive the cancellation token to handle cancellation
    ///   appropriately.
    ///  </para>
    ///  <para>
    ///   The callback is marshalled to the thread that owns the control's handle and then awaited.
    ///   Exceptions thrown by the callback are propagated back to the caller. The returned task represents
    ///   the entire operation, including marshalling to the UI thread and executing the callback.
    ///  </para>
    ///  <para>
    ///   The <see cref="CancellationToken"/> is passed to the callback, allowing it to respond to cancellation
    ///   requests. The callback should check the token periodically for long-running operations.
    ///  </para>
    ///  <para>
    ///   To pass a callback that returns a <see cref="Task{T}"/> instead of <see cref="ValueTask{T}"/>,
    ///   wrap it using the ValueTask constructor: <c>new ValueTask&lt;T&gt;(yourTask)</c>.
    ///  </para>
    ///  <para>
    ///   For synchronous operations, use <see cref="InvokeAsync(Action, CancellationToken)"/> or
    ///   <see cref="InvokeAsync{T}(Func{T}, CancellationToken)"/>.
    ///  </para>
    ///  <para>
    ///   <b>Note:</b> If the control is disposed (or its handle is destroyed) before the
    ///   marshaled callback runs, the returned task may never complete. This is the same
    ///   behavior as <see cref="BeginInvoke(Delegate)"/>.
    ///   To avoid this, either:
    ///  </para>
    ///  <list type="bullet">
    ///   <item>
    ///    <description>Ensure the control outlives the awaited operation, or</description>
    ///   </item>
    ///   <item>
    ///    <description>
    ///     Always pass a <see cref="CancellationToken"/> that you cancel when the
    ///     control is disposing/its handle is destroyed (recommended).
    ///    </description>
    ///   </item>
    ///  </list>
    /// </remarks>
    public async Task<T> InvokeAsync<T>(
        Func<CancellationToken, ValueTask<T>> callback,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (!IsHandleCreated)
        {
            throw new InvalidOperationException(SR.ErrorNoMarshalingThread);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return default!;
        }

        TaskCompletionSource<T> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        CancellationTokenRegistration registration = cancellationToken.Register(
            () => completion.TrySetCanceled(cancellationToken),
            useSynchronizationContext: false);

        BeginInvoke(async () => await WrappedCallbackAsync()
            .ConfigureAwait(false));

        return await completion.Task.ConfigureAwait(false);

        async Task WrappedCallbackAsync()
        {
            try
            {
                using (registration)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        completion.TrySetCanceled(cancellationToken);
                        return;
                    }

                    T returnValue = await callback(cancellationToken).ConfigureAwait(false);
                    completion.TrySetResult(returnValue);
                }
            }
            catch (Exception ex)
            {
                HandleInternalDelegateException(completion, ex, cancellationToken);
            }
        }
    }
}
