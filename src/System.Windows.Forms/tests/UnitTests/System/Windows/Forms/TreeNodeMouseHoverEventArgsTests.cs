// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class TreeNodeMouseHoverEventArgsTests
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
        TreeNodeMouseHoverEventArgs e = new(node);
        Assert.Equal(node, e.Node);
    }
}
