// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripSeparatorRenderEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Null_Graphics_ToolStripSeparator_TestData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);

        yield return new object[] { null, null };
        yield return new object[] { null, new ToolStripSeparator() };
        yield return new object[] { graphics, null };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Null_Graphics_ToolStripSeparator_TestData))]
    public void ToolStripSeparatorRenderEventArgs_Null_Graphics_ToolStripSeparator_ThrowsArgumentNullException(Graphics g, ToolStripSeparator toolStripSeparator)
    {
        Assert.Throws<ArgumentNullException>(() => new ToolStripSeparatorRenderEventArgs(g, toolStripSeparator, true));
    }

    public static IEnumerable<object[]> Ctor_Graphics_ToolStripItem_Bool_TestData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        yield return new object[] { graphics, new ToolStripSeparator(), true };
        yield return new object[] { graphics, new ToolStripSeparator(), false };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Graphics_ToolStripItem_Bool_TestData))]
    public void Ctor_Graphics_ToolStripItem_Bool(Graphics g, ToolStripSeparator separator, bool vertical)
    {
        ToolStripSeparatorRenderEventArgs e = new(g, separator, vertical);
        Assert.Equal(g, e.Graphics);
        Assert.Equal(separator, e.Item);
        Assert.Equal(vertical, e.Vertical);
    }
}
