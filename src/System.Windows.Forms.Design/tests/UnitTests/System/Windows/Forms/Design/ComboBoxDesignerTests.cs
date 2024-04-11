// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Tests;

public sealed class ComboBoxDesignerTests
{
    [Fact]
    public void AutoResizeHandles_WithInitialize_ShouldBeTrue()
    {
        using ComboBoxDesigner comboBoxDesigner = new();
        using ComboBox comboBox = new();
        comboBoxDesigner.Initialize(comboBox);

        comboBoxDesigner.AutoResizeHandles.Should().BeTrue();
    }

    [Fact]
    public void SnapLines_WithDefaultComboBox_ShouldReturnExpectedCount()
    {
        using ComboBoxDesigner comboBoxDesigner = new();
        using ComboBox comboBox = new();
        comboBoxDesigner.Initialize(comboBox);

        comboBoxDesigner.SnapLines.Count.Should().Be(9);
    }

    [Fact]
    public void SelectionRules_WithDefaultComboBox_ShouldReturnExpectedValue()
    {
        using ComboBoxDesigner comboBoxDesigner = new();
        using ComboBox comboBox = new();
        comboBoxDesigner.Initialize(comboBox);

        SelectionRules selectionRules;
        using (new NoAssertContext())
        {
            selectionRules = comboBoxDesigner.SelectionRules;
        }

        selectionRules.Should().Be(SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable | SelectionRules.Visible);
    }

    [Fact]
    public void ActionLists_WithDefaultComboBox_ShouldReturnExpectedCount()
    {
        using ComboBoxDesigner comboBoxDesigner = new();
        using ComboBox comboBox = new();
        comboBoxDesigner.Initialize(comboBox);

        comboBoxDesigner.ActionLists.Count.Should().Be(1);
    }
}
