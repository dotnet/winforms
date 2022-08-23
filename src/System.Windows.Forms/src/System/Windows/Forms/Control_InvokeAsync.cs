// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  Executes the specified asynchronous function on the thread that owns the control's handle.
    /// </summary>
    /// <param name="asyncAction">The asynchronous function to execute.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <returns>A task representing the operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the control's handle is not yet created.</exception>
    public Task<object> InvokeAsync(Func<object[], Task<object>> asyncAction, params object[] args)
    {
        var tcs = new TaskCompletionSource<object>();

        if (!this.IsHandleCreated)
        {
            tcs.SetException(new InvalidOperationException("Control handle not created."));
            return tcs.Task;
        }

        BeginInvoke(new Action(async () =>
        {
            try
            {
                var result = await asyncAction(args).ConfigureAwait(true);
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
                var result = method.DynamicInvoke(args);
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }));

        return tcs.Task;
    }
}
