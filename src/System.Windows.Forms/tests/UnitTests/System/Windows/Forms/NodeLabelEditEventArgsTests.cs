// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class NodeLabelEditEventArgsTests
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
        NodeLabelEditEventArgs e = new(node);
        Assert.Equal(node, e.Node);
        Assert.Null(e.Label);
        Assert.False(e.CancelEdit);
    }

    public static IEnumerable<object[]> Ctor_TreeNode_String_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new TreeNode(), "" };
        yield return new object[] { new TreeNode(), "label" };
    }

    [Theory]
    [MemberData(nameof(Ctor_TreeNode_String_TestData))]
    public void Ctor_TreeNode_String(TreeNode node, string label)
    {
        NodeLabelEditEventArgs e = new(node, label);
        Assert.Equal(node, e.Node);
        Assert.Equal(label, e.Label);
        Assert.False(e.CancelEdit);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CancelEdit_Set_GetReturnsExpected(bool value)
    {
        NodeLabelEditEventArgs e = new(new TreeNode())
        {
            CancelEdit = value
        };
        Assert.Equal(value, e.CancelEdit);
    }
}
