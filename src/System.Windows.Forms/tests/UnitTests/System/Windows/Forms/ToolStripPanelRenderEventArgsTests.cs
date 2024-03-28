// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripPanelRenderEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Null_Graphics_ToolStripPanel_TestData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);

        yield return new object[] { null, null };
        yield return new object[] { graphics, null };
        yield return new object[] { null, new ToolStripPanel() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Null_Graphics_ToolStripPanel_TestData))]
    public void ToolStripPanelRenderEventArgs_Null_Graphics_ToolStripPanel_ThrowsArgumentNullException(Graphics g, ToolStripPanel toolStripPanel)
    {
        Assert.Throws<ArgumentNullException>(() => new ToolStripPanelRenderEventArgs(g, toolStripPanel));
    }

    [WinFormsFact]
    public void Ctor_Graphics_ToolStripPanel()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using ToolStripPanel toolStripPanel = new();
        ToolStripPanelRenderEventArgs e = new(graphics, toolStripPanel);
        Assert.Equal(graphics, e.Graphics);
        Assert.Equal(toolStripPanel, e.ToolStripPanel);
        Assert.False(e.Handled);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void Handled_Set_GetReturnsExpected(bool value)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using ToolStripPanel panel = new();
        ToolStripPanelRenderEventArgs e = new(graphics, panel)
        {
            Handled = value
        };
        Assert.Equal(value, e.Handled);
    }
}
