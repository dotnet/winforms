// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public sealed class DateTimePickerDesignerTests
{
    [Fact]
    public void DateTimePickerDesigner_Constructor_Initialize_AutoResizeHandles()
    {
        using DateTimePickerDesigner dateTimePickerDesigner = new();
        bool autoResizeHandles = dateTimePickerDesigner.AutoResizeHandles;

        dateTimePickerDesigner.Should().NotBeNull();
        autoResizeHandles.Should().BeTrue();
    }

    [Fact]
    public void DateTimePickerDesigner_SnapLines_WithDefaultDateTimePicker_ShouldReturnExpectedCount()
    {
        using DateTimePickerDesigner dateTimePickerDesigner = new();
        using DateTimePicker dateTimePicker = new();
        dateTimePickerDesigner.Initialize(dateTimePicker);

        dateTimePickerDesigner.SnapLines.Count.Should().Be(9);
    }

    [Fact]
    public void DateTimePickerDesigner_SelectionRules_WithDefaultDateTimePicker_ShouldReturnExpectedValue()
    {
        using DateTimePickerDesigner dateTimePickerDesigner = new();
        using DateTimePicker dateTimePicker = new();
        dateTimePickerDesigner.Initialize(dateTimePicker);

        SelectionRules selectionRules;
        using (new NoAssertContext())
        {
            selectionRules = dateTimePickerDesigner.SelectionRules;
        }

        selectionRules.Should().Be(SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable | SelectionRules.Visible);
    }
}
