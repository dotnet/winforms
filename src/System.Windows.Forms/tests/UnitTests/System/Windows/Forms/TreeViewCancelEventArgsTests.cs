// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class TreeViewCancelEventArgsTests
{
    public static IEnumerable<object[]> Ctor_TreeNode_Bool_TreeViewAction_TestData()
    {
        yield return new object[] { null, false, TreeViewAction.Unknown - 1 };
        yield return new object[] { new TreeNode(), true, TreeViewAction.ByKeyboard };
    }

    [Theory]
    [MemberData(nameof(Ctor_TreeNode_Bool_TreeViewAction_TestData))]
    public void Ctor_TreeNode_Bool_TreeViewAction(TreeNode node, bool cancel, TreeViewAction action)
    {
        TreeViewCancelEventArgs e = new(node, cancel, action);
        Assert.Equal(node, e.Node);
        Assert.Equal(cancel, e.Cancel);
        Assert.Equal(action, e.Action);
    }
}
