// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripGripRenderEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Null_Graphics_ToolStrip_TestData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);

        yield return new object[] { null, null };
        yield return new object[] { null, new ToolStrip() };
        yield return new object[] { graphics, null };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Null_Graphics_ToolStrip_TestData))]
    public void ToolStripGripRenderEventArgs_Null_Graphics_ToolStrip_ThrowsArgumentNullException(Graphics g, ToolStrip toolStrip)
    {
        Assert.Throws<ArgumentNullException>(() => new ToolStripGripRenderEventArgs(g, toolStrip));
    }

    public static IEnumerable<object[]> Ctor_Graphics_ToolStrip_TestData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);

        yield return new object[] { graphics, new ToolStrip() };
        yield return new object[]
        {
            graphics, new ToolStrip
            {
                LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow,
                GripStyle = ToolStripGripStyle.Visible
            }
        };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Graphics_ToolStrip_TestData))]
    public void Ctor_Graphics_ToolStrip(Graphics g, ToolStrip toolStrip)
    {
        ToolStripGripRenderEventArgs e = new(g, toolStrip);
        Assert.Equal(g, e.Graphics);
        Assert.Equal(toolStrip, e.ToolStrip);
        Assert.Equal(new Rectangle(Point.Empty, toolStrip.Size), e.AffectedBounds);
        Assert.Equal(toolStrip.GripRectangle, e.GripBounds);
        Assert.Equal(toolStrip.GripDisplayStyle, e.GripDisplayStyle);
        Assert.Equal(toolStrip.GripStyle, e.GripStyle);
    }
}
