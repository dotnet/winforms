// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Tests;

public class GroupedContextMenuStripTests
{
    [Fact]
    public void Populate_WhenInitializedWithoutGroups_ItemsCountIsZero()
    {
        using GroupedContextMenuStrip groupedContextMenuStrip = new();

        groupedContextMenuStrip.Populate();

        groupedContextMenuStrip.Items.Count.Should().Be(0);
    }

    [Fact]
    public void Populate_WhenInitializedWithGroup_ItemsCountIsNotZero()
    {
        using GroupedContextMenuStrip groupedContextMenuStrip = new();
        using ToolStripButton toolStripButton = new();
        groupedContextMenuStrip.Groups["First"].Items.Add(toolStripButton);
        groupedContextMenuStrip.GroupOrdering.Add("First");

        groupedContextMenuStrip.Populate();

        groupedContextMenuStrip.Items.Count.Should().Be(1);
    }

    [Fact]
    public void Populate_WhenInitializedWithGroups_ItemsCountIsNotZeroAndHaveSeparator()
    {
        using GroupedContextMenuStrip groupedContextMenuStrip = new();
        using ToolStripButton toolStripButton = new();
        using ToolStripComboBox toolStripComboBox = new();
        groupedContextMenuStrip.Groups["First"].Items.Add(toolStripButton);
        groupedContextMenuStrip.Groups["Second"].Items.Add(toolStripComboBox);
        groupedContextMenuStrip.GroupOrdering.Add("First");
        groupedContextMenuStrip.GroupOrdering.Add("Second");

        groupedContextMenuStrip.Populate();

        groupedContextMenuStrip.Items.Count.Should().Be(3);
        groupedContextMenuStrip.Items[1].Should().BeOfType<ToolStripSeparator>();
    }
}
