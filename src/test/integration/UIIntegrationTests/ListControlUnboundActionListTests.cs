// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.UITests;

public class ListControlUnboundActionListTests
{
    [WinFormsFact]
    public void InvokeItemsDialog_EditValueInvoked_NoException()
    {
        using ListView listView = new();
        using ListViewDesigner listViewDesigner = new();
        listViewDesigner.Initialize(listView);
        var actionList = new ListControlUnboundActionList(listViewDesigner);
        var task = Task.Run(async () =>
        {
            await Task.Delay(1000).ConfigureAwait(false);
            foreach (Form form in Application.OpenForms)
            {
                if (form.Text.Contains("ListViewItem"))
                {
                    await form.InvokeAsync(new Action(() => form.Dispose())).ConfigureAwait(false);
                    break;
                }
            }
        });

        Action action = actionList.InvokeItemsDialog;
        action.Should().NotThrow();
    }
}
