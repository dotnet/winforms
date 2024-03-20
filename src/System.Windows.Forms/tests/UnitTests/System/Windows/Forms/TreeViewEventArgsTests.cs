// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class TreeViewEventArgsTests
{
    public static IEnumerable<object[]> Ctor_TreeNode_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new TreeNode() };
    }

    [Theory]
    [MemberData(nameof(Ctor_TreeNode_TestData))]
    public void Ctor_TreeNode(TreeNode node)
    {
        TreeViewEventArgs e = new(node);
        Assert.Equal(node, e.Node);
        Assert.Equal(TreeViewAction.Unknown, e.Action);
    }

    public static IEnumerable<object[]> Ctor_TreeNode_TreeViewAction_TestData()
    {
        yield return new object[] { null, TreeViewAction.Unknown - 1 };
        yield return new object[] { new TreeNode(), TreeViewAction.ByKeyboard };
    }

    [Theory]
    [MemberData(nameof(Ctor_TreeNode_TreeViewAction_TestData))]
    public void Ctor_TreeNode_TreeViewAction(TreeNode node, TreeViewAction action)
    {
        TreeViewEventArgs e = new(node, action);
        Assert.Equal(node, e.Node);
        Assert.Equal(action, e.Action);
    }
}
