// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public sealed class MonthCalendarDesignerTests
{
    [Fact]
    public void AutoResizeHandles_WithDefaultMonthCalendar_ShouldBeTrue()
    {
        using MonthCalendarDesigner monthCalendarDesigner = new();
        using MonthCalendar monthCalendar = new();
        monthCalendarDesigner.Initialize(monthCalendar);

        monthCalendarDesigner.AutoResizeHandles.Should().BeTrue();
    }

    [Fact]
    public void SelectionRules_WithDefaultMonthCalendar_ShouldReturnExpectedValue()
    {
        using MonthCalendarDesigner monthCalendarDesigner = new();
        using MonthCalendar monthCalendar = new();
        monthCalendarDesigner.Initialize(monthCalendar);

        SelectionRules selectionRules;
        using (new NoAssertContext())
        {
            selectionRules = monthCalendarDesigner.SelectionRules;
        }

        selectionRules.Should().Be(SelectionRules.BottomSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable | SelectionRules.Visible);
    }
}
