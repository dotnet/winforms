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
    public void SelectionRules_WithDefaultComboBox_ShouldThrowNullReferenceException()
    {
        using ComboBoxDesigner comboBoxDesigner = new();
        using ComboBox comboBox = new();
        comboBoxDesigner.Initialize(comboBox);

        Action action = () => _ = comboBoxDesigner.SelectionRules;

        action.Should().ThrowExactly<NullReferenceException>();
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
