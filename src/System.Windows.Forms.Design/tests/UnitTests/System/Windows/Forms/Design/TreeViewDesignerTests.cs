// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Tests;

public sealed class TreeViewDesignerTests
{
    [Fact]
    public void AutoResizeHandles_WithCtor_ShouldBeTrue()
    {
        using TreeViewDesigner treeViewDesigner = new();

        treeViewDesigner.AutoResizeHandles.Should().BeTrue();
    }

    [Fact]
    public void ActionLists_WithDefaultTreeView_ShouldReturnExpectedCount()
    {
        using TreeViewDesigner treeViewDesigner = new();
        using TreeView treeView = new();
        treeViewDesigner.Initialize(treeView);

        treeViewDesigner.ActionLists.Count.Should().Be(1);
    }
}
