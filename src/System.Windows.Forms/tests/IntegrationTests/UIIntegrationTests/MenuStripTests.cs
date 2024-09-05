// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;

namespace System.Windows.Forms.UITests;

public class MenuStripTests
{
    [WinFormsTheory]
    [BoolData]
    public void MenuStrip_ProcessCmdKey_InvokeSpaceKey(bool value)
    {
        using Form form = new Form();
        using MenuStrip menuStrip = new() { TabStop = value };
        using ToolStripMenuItem toolStripMenuItem1 = new();
        using ToolStripMenuItem toolStripMenuItem2 = new() { CheckOnClick = true };
        using ToolStripMenuItem toolStripMenuItem3 = new();
        toolStripMenuItem1.DropDownItems.AddRange(toolStripMenuItem2, toolStripMenuItem3);
        menuStrip.Items.AddRange(toolStripMenuItem1);
        form.Controls.Add(menuStrip);
        form.Show();

        Message m = new();
        if (value)
        {
            menuStrip.Focused.Should().BeTrue();
        }
        else
        {
            menuStrip.Focused.Should().BeFalse();
        }

        toolStripMenuItem1.ProcessCmdKey(ref m, keyData: Keys.Enter);
        toolStripMenuItem2.Checked.Should().BeFalse();
        toolStripMenuItem2.ProcessCmdKey(ref m, keyData: Keys.Space);
        toolStripMenuItem2.Checked.Should().BeFalse();

        toolStripMenuItem3.ProcessCmdKey(ref m, keyData: Keys.Enter);
        if (value)
        {
            menuStrip.Focused.Should().BeTrue();
        }
        else
        {
            menuStrip.Focused.Should().BeFalse();
        }
    }
}
