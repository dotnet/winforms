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
        using Form form = new();
        using MenuStrip menuStrip = new() { TabStop = value };
        using ToolStripMenuItem toolStripMenuItem1 = new();
        using ToolStripMenuItem toolStripMenuItem2 = new() { CheckOnClick = true };
        using ToolStripMenuItem toolStripMenuItem3 = new();
        toolStripMenuItem1.DropDownItems.AddRange(toolStripMenuItem2, toolStripMenuItem3);
        menuStrip.Items.AddRange(toolStripMenuItem1);
        form.Controls.Add(menuStrip);
        form.Show();

        Message message = default;
        menuStrip.Focused.Should().Be(value);

        toolStripMenuItem1.ProcessCmdKey(ref message, keyData: Keys.Enter);
        toolStripMenuItem2.Checked.Should().BeFalse();
        toolStripMenuItem2.ProcessCmdKey(ref message, keyData: Keys.Space);
        toolStripMenuItem2.Checked.Should().BeFalse();

        toolStripMenuItem3.ProcessCmdKey(ref message, keyData: Keys.Enter);
        menuStrip.Focused.Should().Be(value);
    }
}
