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

    [WinFormsTheory]
    [InlineData(DockStyle.Top, 4, 0, 4, 0)]
    [InlineData(DockStyle.Top, 0, 0, 0, 0)]
    [InlineData(DockStyle.Top, 4, 4, 4, 4)]
    [InlineData(DockStyle.Top, 4, 2, 11, 7)]
    [InlineData(DockStyle.Left, 0, 4, 0, 4)]
    [InlineData(DockStyle.Left, 0, 0, 0, 0)]
    [InlineData(DockStyle.Left, 4, 4, 4, 4)]
    [InlineData(DockStyle.Left, 2, 4, 7, 11)]
    public void ToolStripSettingsManager_SaveLoad_PanelPadding_NoDriftAcrossMultipleCycles(
        DockStyle dock, int left, int top, int right, int bottom)
    {
        string settingsKey = $"{nameof(ToolStripSettingsManager_SaveLoad_PanelPadding_NoDriftAcrossMultipleCycles)}_{dock}_{left}_{top}_{right}_{bottom}";
        bool horizontal = dock == DockStyle.Top;

        Drawing.Point expectedLocation1 = Drawing.Point.Empty;
        Drawing.Point expectedLocation2 = Drawing.Point.Empty;
        Drawing.Point expectedLocation3 = Drawing.Point.Empty;

        for (int cycle = 0; cycle < 3; cycle++)
        {
            using Form form = new() { Name = settingsKey, ClientSize = new Drawing.Size(600, 600) };
            using ToolStripContainer container = new() { Dock = DockStyle.Fill, Name = "Container" };
            using ToolStrip toolStrip1 = new() { Name = "ToolStripMain" };
            using ToolStrip toolStrip2 = new() { Name = "ToolStripFilters" };
            using ToolStrip toolStrip3 = new() { Name = "ToolStripDetails" };

            ToolStripPanel panel = horizontal ? container.TopToolStripPanel : container.LeftToolStripPanel;
            panel.Padding = new Padding(left, top, right, bottom);
            toolStrip1.Items.Add(new ToolStripButton("Btn1") { Name = "ButtonMain" });
            toolStrip2.Items.Add(new ToolStripButton("Btn2") { Name = "ButtonFilters" });
            toolStrip3.Items.Add(new ToolStripButton("Btn3") { Name = "ButtonDetails" });

            form.Controls.Add(container);
            form.Show();

            if (cycle == 0)
            {
                panel.Join(toolStrip1, new Drawing.Point(left + 20, top + 20));
                panel.Join(toolStrip2, horizontal
                    ? new Drawing.Point(toolStrip1.Right + 20, toolStrip1.Top)
                    : new Drawing.Point(toolStrip1.Left, toolStrip1.Bottom + 20));
                panel.Join(toolStrip3, horizontal
                    ? new Drawing.Point(toolStrip1.Left, toolStrip1.Bottom + 10)
                    : new Drawing.Point(toolStrip1.Right + 10, toolStrip1.Top));
                panel.PerformLayout();

                expectedLocation1 = toolStrip1.Location;
                expectedLocation2 = toolStrip2.Location;
                expectedLocation3 = toolStrip3.Location;
                ToolStripManager.SaveSettings(form, settingsKey);
            }
            else
            {
                // Restore into fresh rows rather than repositioning already-joined cells.
                toolStrip1.Dock = DockStyle.None;
                toolStrip2.Dock = DockStyle.None;
                toolStrip3.Dock = DockStyle.None;
                toolStrip1.Location = new Drawing.Point(100, 100);
                toolStrip2.Location = new Drawing.Point(200, 200);
                toolStrip3.Location = new Drawing.Point(300, 300);
                container.ContentPanel.Controls.Add(toolStrip1);
                container.ContentPanel.Controls.Add(toolStrip2);
                container.ContentPanel.Controls.Add(toolStrip3);

                Assert.NotEqual(expectedLocation1, toolStrip1.Location);
                Assert.NotEqual(expectedLocation2, toolStrip2.Location);
                Assert.NotEqual(expectedLocation3, toolStrip3.Location);
            }

            ToolStripManager.LoadSettings(form, settingsKey);
            panel.PerformLayout();

            Assert.Equal(expectedLocation1, toolStrip1.Location);
            Assert.Equal(expectedLocation2, toolStrip2.Location);
            Assert.Equal(expectedLocation3, toolStrip3.Location);
            Assert.Equal(2, panel.RowsInternal.Count);
            Assert.Contains(toolStrip1, panel.RowsInternal[0].Controls);
            Assert.Contains(toolStrip2, panel.RowsInternal[0].Controls);
            Assert.Contains(toolStrip3, panel.RowsInternal[1].Controls);

            ToolStripManager.SaveSettings(form, settingsKey);
        }
    }
}
