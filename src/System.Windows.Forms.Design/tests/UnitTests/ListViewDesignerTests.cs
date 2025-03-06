// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.Windows.Forms.Design.Tests;

public class ListViewDesignerTests
{
    [WinFormsFact]
    public void ListViewDesigner_AssociatedComponentsTest()
    {
        using ListViewDesigner listViewDesigner = new();
        using ListView listView = new();
        listViewDesigner.Initialize(listView);

        Assert.Empty(listViewDesigner.AssociatedComponents);

        listView.Columns.Add("123");
        listView.Columns.Add("abc");

        Assert.Equal(2, listViewDesigner.AssociatedComponents.Count);
    }
}
