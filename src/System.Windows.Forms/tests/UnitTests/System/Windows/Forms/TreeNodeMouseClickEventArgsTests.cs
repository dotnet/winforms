// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class TreeNodeMouseClickEventArgsTests
{
    public static IEnumerable<object[]> Ctor_TreeNode_MouseButtons_Int_Int_Int_Int_TestData()
    {
        yield return new object[] { null, MouseButtons.Left, 1, 2, 3 };
        yield return new object[] { new TreeNode(), (MouseButtons)1, 0, 0, 0 };
        yield return new object[] { new TreeNode(), (MouseButtons)3, -1, -1, -1 };
    }

    [Theory]
    [MemberData(nameof(Ctor_TreeNode_MouseButtons_Int_Int_Int_Int_TestData))]
    public void Ctor_TreeNode_MouseButtons_Int_Int_Int_Int(TreeNode node, MouseButtons button, int clicks, int x, int y)
    {
        TreeNodeMouseClickEventArgs e = new(node, button, clicks, x, y);
        Assert.Equal(node, e.Node);
        Assert.Equal(button, e.Button);
        Assert.Equal(clicks, e.Clicks);
        Assert.Equal(x, e.X);
        Assert.Equal(y, e.Y);
        Assert.Equal(0, e.Delta);
        Assert.Equal(new Point(x, y), e.Location);
    }
}
