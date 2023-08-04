// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripContentPanelRenderEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Null_Graphics_ToolStripContentPanel_TestData()
    {
        var image = new Bitmap(10, 10);
        Graphics graphics = Graphics.FromImage(image);

        yield return new object[] { null, null };
        yield return new object[] { graphics, null };
        yield return new object[] { null, new ToolStripContentPanel() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Null_Graphics_ToolStripContentPanel_TestData))]
    public void ToolStripContentPanelRenderEventArgs_Null_Graphics_ToolStripContentPanel_ThrowsArgumentNullException(Graphics g, ToolStripContentPanel toolStripContentPanel)
    {
        Assert.Throws<ArgumentNullException>(() => new ToolStripContentPanelRenderEventArgs(g, toolStripContentPanel));
    }

    [WinFormsFact]
    public void Ctor_Graphics_ToolStripContentPanel()
    {
        using var image = new Bitmap(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using var toolStripContentPanel = new ToolStripContentPanel();
        var e = new ToolStripContentPanelRenderEventArgs(graphics, toolStripContentPanel);
        Assert.Equal(graphics, e.Graphics);
        Assert.Equal(toolStripContentPanel, e.ToolStripContentPanel);
        Assert.False(e.Handled);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void Handled_Set_GetReturnsExpected(bool value)
    {
        using (var image = new Bitmap(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        {
            using var panel = new ToolStripContentPanel();
            var e = new ToolStripContentPanelRenderEventArgs(graphics, panel)
            {
                Handled = value
            };
            Assert.Equal(value, e.Handled);
        }
    }
}
