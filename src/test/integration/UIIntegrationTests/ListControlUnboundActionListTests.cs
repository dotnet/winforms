// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.UITests;

public class ListControlUnboundActionListTests
{
    [WinFormsFact]
    public async Task InvokeItemsDialog_EditValueInvoked_NoExceptionAsync()
    {
        using ListView listView = new();
        using ListViewDesigner listViewDesigner = new();
        listViewDesigner.Initialize(listView);
        ListControlUnboundActionList actionList = new(listViewDesigner);
        bool formClosed = false;
        int retryCount = 3;
        int retryDelay = 100;
        var disposeTask = Task.Run(async () =>
        {
            for (int i = 0; i < retryCount; i++)
            {
                await Task.Delay(500).ConfigureAwait(true);
                foreach (Form form in Application.OpenForms)
                {
                    if (form.Text.Contains(nameof(ListViewItem)))
                    {
                        await form.InvokeAsync(new Action(() => form.Dispose())).ConfigureAwait(true);
                        formClosed = true;
                        break;
                    }
                }

                if (formClosed)
                {
                    break;
                }
                else if (i < retryCount - 1)
                {
                    await Task.Delay(retryDelay).ConfigureAwait(true);
                }
            }
        });

        actionList.InvokeItemsDialog();

        // Await the disposeTask before validating.
        await disposeTask.ConfigureAwait(true);

        // Assert the form was found and closed.
        formClosed.Should().BeTrue();
    }
}
