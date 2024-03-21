// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DrawTreeNodeEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Graphics_TreeNode_Rectangle_TreeNodeStates_TestData()
    {
        yield return new object[] { null, Rectangle.Empty, TreeNodeStates.Checked - 1 };
        yield return new object[] { new TreeNode(), new Rectangle(1, 2, 3, 4), TreeNodeStates.Checked };
        yield return new object[] { new TreeNode(), new Rectangle(-1, -2, -3, -4), TreeNodeStates.Checked };
    }

    [Theory]
    [MemberData(nameof(Ctor_Graphics_TreeNode_Rectangle_TreeNodeStates_TestData))]
    public void Ctor_Graphics_TreeNode_Rectangle_TreeNodeStates(TreeNode node, Rectangle bounds, TreeNodeStates state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawTreeNodeEventArgs e = new(graphics, node, bounds, state);
        Assert.Equal(graphics, e.Graphics);
        Assert.Equal(node, e.Node);
        Assert.Equal(bounds, e.Bounds);
        Assert.Equal(state, e.State);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void DrawDefault_Set_GetReturnsExpected(bool value)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        DrawTreeNodeEventArgs e = new(graphics, new TreeNode(), new Rectangle(1, 2, 3, 4), TreeNodeStates.Checked)
        {
            DrawDefault = value
        };
        Assert.Equal(value, e.DrawDefault);
    }
}
