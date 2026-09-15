// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

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

    [WinFormsFact]
    public void ToolStripSettingsManager_SaveLoad_AsymmetricPanelPadding_NoDriftAcrossMultipleCycles()
    {
        string settingsKey = nameof(ToolStripSettingsManager_SaveLoad_AsymmetricPanelPadding_NoDriftAcrossMultipleCycles);

        Drawing.Point? expectedLocation1 = null;
        Drawing.Point? expectedLocation2 = null;

        for (int cycle = 0; cycle < 3; cycle++)
        {
            using Form form = new() { Name = settingsKey, ClientSize = new Drawing.Size(600, 200) };
            using ToolStripContainer container = new() { Dock = DockStyle.Fill, Name = "Container" };
            using ToolStrip toolStrip1 = new() { Name = "ToolStripMain" };
            using ToolStrip toolStrip2 = new() { Name = "ToolStripFilters" };

            container.TopToolStripPanel.Padding = new Padding(4, 0, 4, 0);
            toolStrip1.Items.Add(new ToolStripButton("Btn1"));
            toolStrip2.Items.Add(new ToolStripButton("Btn2"));

            form.Controls.Add(container);
            form.Show();

            ToolStripPanel topPanel = container.TopToolStripPanel;

            if (cycle == 0)
            {
                topPanel.Join(toolStrip1, new Drawing.Point(7, 0));
                topPanel.Join(toolStrip2, new Drawing.Point(7, toolStrip1.Bottom + 5));
                topPanel.PerformLayout();
                Application.DoEvents();

                Assert.True(toolStrip2.Location.Y > toolStrip1.Location.Y);
            }
            else
            {
                topPanel.Join(toolStrip1, Drawing.Point.Empty);
                topPanel.Join(toolStrip2, new Drawing.Point(0, 500));
                topPanel.PerformLayout();
                Application.DoEvents();
            }

            ToolStripManager.SaveSettings(form, settingsKey);
            ToolStripManager.LoadSettings(form, settingsKey);
            topPanel.PerformLayout();
            Application.DoEvents();

            if (expectedLocation1 is null)
            {
                expectedLocation1 = toolStrip1.Location;
                expectedLocation2 = toolStrip2.Location;
            }
            else
            {
                Assert.Equal(expectedLocation1.Value, toolStrip1.Location);
                Assert.Equal(expectedLocation2.Value, toolStrip2.Location);
            }
        }
    }
}
