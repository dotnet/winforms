// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.Windows.Forms.Tests;

internal static class FormAsyncExtensions
{
    public static Task WaitForHandleCreatedAsync(this Form form)
    {
        ArgumentNullException.ThrowIfNull(form, nameof(form));

        TaskCompletionSource tcs = new TaskCompletionSource(new WeakReference(form));
        form.HandleCreated += Form_HandleCreated;

        return tcs.Task;

        void Form_HandleCreated(object sender, EventArgs e)
        {
            ((Form)sender).HandleCreated -= Form_HandleCreated;
            tcs.TrySetResult();
        }
    }

    public static Form ToForm(this Task task)
    {
        ArgumentNullException.ThrowIfNull(task, nameof(task));

        if (task.AsyncState is WeakReference<Form> weakRefToForm)
        {
            if (weakRefToForm.TryGetTarget(out Form form))
            {
                return form;
            }
        }

        return null;
    }
}
