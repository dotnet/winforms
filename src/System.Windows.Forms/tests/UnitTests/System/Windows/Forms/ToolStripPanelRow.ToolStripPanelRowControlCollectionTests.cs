// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripPanelRowControlCollectionTests
{
    [WinFormsFact]
    public void GetControl_ShouldNotThrowIndexOutOfRangeException()
    {
        // https://github.com/dotnet/winforms/issues/9126
        using Form form = new();
        using ToolStripContainer toolStripContainer = new();
        using ToolStrip toolStrip1 = new();
        using ToolStrip toolStrip2 = new();

        toolStripContainer.TopToolStripPanel.SuspendLayout();
        toolStripContainer.SuspendLayout();
        form.SuspendLayout();

        toolStripContainer.TopToolStripPanel.Controls.Add(toolStrip1);
        toolStripContainer.TopToolStripPanel.Controls.Add(toolStrip2);

        toolStrip1.Location = new Point(3, 0);
        toolStrip2.Location = new Point(148, 0);

        form.Controls.Add(toolStripContainer);
        toolStripContainer.TopToolStripPanel.ResumeLayout(false);
        toolStripContainer.TopToolStripPanel.PerformLayout();
        toolStripContainer.ResumeLayout(false);
        toolStripContainer.PerformLayout();
        form.ResumeLayout(false);

        var exception = Record.Exception(form.Show);

        Assert.Null(exception);
    }
}
