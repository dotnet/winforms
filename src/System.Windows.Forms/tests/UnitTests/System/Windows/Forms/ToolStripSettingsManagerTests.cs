// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ToolStripSettingsManagerTests : IClassFixture<UserConfigDisposableFixture>
{
    [WinFormsFact]
    public void ToolStripSettingsManager_Save_Load_RoundTripExpected()
    {
        using Form mainForm = new();

        using ToolStrip toolStrip = new();
        toolStrip.Name = "Child";
        toolStrip.Size = new Drawing.Size(10, 10);
        toolStrip.Visible = false;
        mainForm.Controls.Add(toolStrip);

        ToolStripSettingsManager toolStripSettingsManager = new(mainForm, "MainForm");

        toolStripSettingsManager.Save();

        toolStrip.Size = new Drawing.Size(5, 5);
        toolStrip.Visible = true;

        toolStripSettingsManager.Load();

        Assert.Equal(new Drawing.Size(10, 10), toolStrip.Size);
        Assert.False(toolStrip.Visible);
    }
}
