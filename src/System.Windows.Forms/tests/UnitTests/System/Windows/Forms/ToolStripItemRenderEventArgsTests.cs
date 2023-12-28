// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripItemRenderEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Null_Graphics_ToolStripItem_TestData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);

        yield return new object[] { null, null };
        yield return new object[] { null, new ToolStripButton() };
        yield return new object[] { graphics, null };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Null_Graphics_ToolStripItem_TestData))]
    public void ToolStripItemRenderEventArgs_Null_Graphics_ToolStripItem_ThrowsArgumentNullException(Graphics g, ToolStripItem toolStripItem)
    {
        Assert.Throws<ArgumentNullException>(() => new ToolStripItemRenderEventArgs(g, toolStripItem));
    }

    public static IEnumerable<object[]> Ctor_Graphics_ToolStripItem_TestData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);

        yield return new object[] { graphics, new ToolStripButton(), null };

        ToolStripButton toolStripItem = new();
        ToolStrip toolStrip = new();
        toolStrip.Items.Add(toolStripItem);
        yield return new object[] { graphics, toolStripItem, null };
        yield return new object[] { graphics, toolStrip.OverflowButton, toolStrip };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Graphics_ToolStripItem_TestData))]
    public void Ctor_Graphics_ToolStripItem(Graphics g, ToolStripItem item, ToolStrip expectedToolStrip)
    {
        ToolStripItemRenderEventArgs e = new(g, item);
        Assert.Equal(g, e.Graphics);
        Assert.Equal(item, e.Item);
        Assert.Equal(expectedToolStrip, e.ToolStrip);
    }
}
