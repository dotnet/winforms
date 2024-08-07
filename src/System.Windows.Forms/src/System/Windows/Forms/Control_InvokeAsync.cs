// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  Invokes the specified synchronous function asynchronously on the thread that owns the control's handle.
    /// </summary>
    /// <param name="action">The synchronous action to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation and containing the function's result.</returns>
    [Experimental(DiagnosticIDs.ExperimentalAsync, UrlFormat = "https://aka.ms/winforms-experimental/{0}")]
#pragma warning disable RS0026 // API with optional parameter(s) should have the most parameters amongst its public overloads
    public async Task InvokeAsync(Action action, CancellationToken cancellationToken = default)
#pragma warning restore RS0026 // API with optional parameter(s) should have the most parameters amongst its public overloads
    {
        ArgumentNullException.ThrowIfNull(action);

        if (cancellationToken.IsCancellationRequested)
            return;

        TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using (cancellationToken.Register(() => tcs.SetCanceled(), useSynchronizationContext: false))
        {
            BeginInvoke(WrappedAction);
            await tcs.Task.ConfigureAwait(false);
        }

        void WrappedAction()
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    tcs.SetCanceled(cancellationToken);
                    return;
                }

                action();
                tcs.TrySetResult();
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
    /// <param name="callback">The synchronous function to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation and containing the function's result.</returns>
    [Experimental(DiagnosticIDs.ExperimentalAsync, UrlFormat = "https://aka.ms/winforms-experimental/{0}")]
#pragma warning disable RS0026 // Do not add multiple public overloads with optional parameters
    public async Task<T> InvokeAsync<T>(Func<T> callback, CancellationToken cancellationToken = default)
#pragma warning restore RS0026 // Do not add multiple public overloads with optional parameters
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (cancellationToken.IsCancellationRequested)
        {
            return default!;
        }

        TaskCompletionSource<T> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using (cancellationToken.Register(() => tcs.SetCanceled(), useSynchronizationContext: false))
        {
            BeginInvoke(WrappedFunction);
            return await tcs.Task.ConfigureAwait(false);
        }

        void WrappedFunction()
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    tcs.TrySetCanceled(cancellationToken);
                    return;
                }

                T result = callback();
                tcs.TrySetResult(result);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }
    }

    /// <summary>
    ///  Executes the specified asynchronous function on the thread that owns the control's handle.
    /// </summary>
    /// <param name="callback">
    ///  The asynchronous function to execute,
    ///  which takes an input of type T and returns a <see cref="ValueTask{T}"/>.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation and containing the function's result of type T.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the control's handle is not yet created.</exception>
    [Experimental(DiagnosticIDs.ExperimentalAsync, UrlFormat = "https://aka.ms/winforms-experimental/{0}")]
#pragma warning disable RS0026 // Do not add multiple public overloads with optional parameters
    public async Task InvokeAsync(Func<CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default)
#pragma warning restore RS0026 // Do not add multiple public overloads with optional parameters
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using (cancellationToken.Register(() => tcs.SetCanceled(), useSynchronizationContext: false))
        {
            BeginInvoke(async () => await WrappedFunction().ConfigureAwait(false));
            await tcs.Task.ConfigureAwait(false);
        }

        async Task WrappedFunction()
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    tcs.TrySetCanceled(cancellationToken);
                    return;
                }

                await callback(cancellationToken).ConfigureAwait(false);
                tcs.TrySetResult();
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }
    }

    /// <summary>
    ///  Executes the specified asynchronous function on the thread that owns the control's handle.
    /// </summary>
    /// <typeparam name="T">The type of the input argument to be converted into the args array.</typeparam>
    /// <param name="callback">
    ///  The asynchronous function to execute,
    ///  which takes an input of type T and returns a <see cref="ValueTask{T}"/>.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the operation and containing the function's result of type T.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the control's handle is not yet created.</exception>
    [Experimental(DiagnosticIDs.ExperimentalAsync, UrlFormat = "https://aka.ms/winforms-experimental/{0}")]
#pragma warning disable RS0026 // API with optional parameter(s) should have the most parameters amongst its public overloads
    public async Task<T> InvokeAsync<T>(Func<CancellationToken, ValueTask<T>> callback, CancellationToken cancellationToken = default)
#pragma warning restore RS0026 // API with optional parameter(s) should have the most parameters amongst its public overloads
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (cancellationToken.IsCancellationRequested)
        {
            return default!;
        }

        TaskCompletionSource<T> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using (cancellationToken.Register(() => tcs.SetCanceled(), useSynchronizationContext: false))
        {
            BeginInvoke(async () => await WrappedCallbackAsync().ConfigureAwait(false));
            return await tcs.Task.ConfigureAwait(false);
        }

        async Task WrappedCallbackAsync()
        {
            try
            {
                var returnValue = await callback(cancellationToken).ConfigureAwait(false);
                tcs.TrySetResult(returnValue);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }
    }
}
