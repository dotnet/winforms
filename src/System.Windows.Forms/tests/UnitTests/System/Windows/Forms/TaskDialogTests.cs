// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.DotNet.RemoteExecutor;

namespace System.Windows.Forms.Tests;

public class TaskDialogTests
{
    [WinFormsFact]
    public void TaskDialog_ShowDialog_SetProperty_SameThread_Success()
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke(() =>
        {
            Application.EnableVisualStyles();
            Control.CheckForIllegalCrossThreadCalls = true;

            TaskDialogPage page = new();
            page.Created += (_, __) =>
            {
                // Set the property in the same thread.
                page.Text = "X";
                page.BoundDialog.Close();
            };

            TaskDialog.ShowDialog(page);
        });

        // verify the remote process succeeded
        Assert.Equal(RemoteExecutor.SuccessExitCode, invokerHandle.ExitCode);
    }

    [WinFormsFact]
    public void TaskDialog_ShowDialog_SetProperty_DifferentThread_ThrowsInvalidOperationException()
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke(() =>
        {
            Application.EnableVisualStyles();
            Control.CheckForIllegalCrossThreadCalls = true;

            TaskDialogPage page = new();
            page.Created += (_, __) =>
            {
                // Set the property in a different thread.
                var separateTask = Task.Run(() => page.Text = "X");
                Assert.Throws<InvalidOperationException>(separateTask.GetAwaiter().GetResult);

                page.BoundDialog.Close();
            };

            TaskDialog.ShowDialog(page);
        });

        // verify the remote process succeeded
        Assert.Equal(RemoteExecutor.SuccessExitCode, invokerHandle.ExitCode);
    }
}
