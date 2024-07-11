// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MonthCalendar_CalendarRowAccessibleObjectTests
{
    [WinFormsFact]
    public void CalendarRowAccessibleObject_ctor_default()
    {
        using MonthCalendar control = new();
        CalendarRowAccessibleObject rowAccessibleObject = CreateCalendarRowAccessibleObject(control);

        int calendarIndexResult = rowAccessibleObject.TestAccessor().Dynamic._calendarIndex;
        calendarIndexResult.Should().Be(0);

        int rowIndexResult = rowAccessibleObject.TestAccessor().Dynamic._rowIndex;
        rowIndexResult.Should().Be(0);

        control.IsHandleCreated.Should().BeFalse();
        rowAccessibleObject.CanGetDescriptionInternal.Should().BeFalse();
        rowAccessibleObject.CanGetNameInternal.Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void CalendarRowAccessibleObject_GetChildId_ReturnExpected(int rowIndex)
    {
        using MonthCalendar control = new();
        CalendarRowAccessibleObject rowAccessibleObject = CreateCalendarRowAccessibleObject(control, 0, rowIndex);

        int actual = rowAccessibleObject.GetChildId();

        Assert.Equal(rowIndex + 1, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarRowAccessibleObject_ControlType_IsPane()
    {
        using MonthCalendar control = new();
        CalendarRowAccessibleObject rowAccessibleObject = CreateCalendarRowAccessibleObject(control);

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)rowAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarRowAccessibleObject_Name_IsNull()
    {
        using MonthCalendar control = new();
        CalendarRowAccessibleObject rowAccessibleObject = CreateCalendarRowAccessibleObject(control);

        string actual = rowAccessibleObject.Name;

        Assert.Null(actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarRowAccessibleObject_Parent_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, 0, "Test name");
        CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, 0);
        CalendarRowAccessibleObject rowAccessibleObject = new(bodyAccessibleObject, controlAccessibleObject, 0, 0);

        AccessibleObject actual = rowAccessibleObject.Parent;

        Assert.Equal(bodyAccessibleObject, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarRowAccessibleObject_Role_IsRow()
    {
        using MonthCalendar control = new();
        CalendarRowAccessibleObject rowAccessibleObject = CreateCalendarRowAccessibleObject(control);

        AccessibleRole actual = rowAccessibleObject.Role;

        Assert.Equal(AccessibleRole.Row, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void CalendarRowAccessibleObject_Row_ReturnsExpected(int rowIndex)
    {
        using MonthCalendar control = new();
        CalendarRowAccessibleObject rowAccessibleObject = CreateCalendarRowAccessibleObject(control, 0, rowIndex);

        int actual = rowAccessibleObject.Row;

        Assert.Equal(rowIndex, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarRowAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "");
        CalendarBodyAccessibleObject body = new(calendar, controlAccessibleObject, 0);
        CalendarRowAccessibleObject row = new(body, controlAccessibleObject, 0, 2);

        Assert.Equal(body, row.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarRowAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using MonthCalendar control = new();
        control.CreateControl();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

        CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects.First?.Value;
        Assert.NotNull(calendar);

        CalendarBodyAccessibleObject body = calendar.CalendarBodyAccessibleObject;
        Assert.NotNull(body);

        CalendarRowAccessibleObject daysOfWeekRow = body.RowsAccessibleObjects?.First?.Value;
        Assert.NotNull(daysOfWeekRow);

        CalendarRowAccessibleObject firstWeek = body.RowsAccessibleObjects?.First?.Next?.Value;
        Assert.NotNull(firstWeek);

        CalendarRowAccessibleObject secondWeek = body.RowsAccessibleObjects?.First?.Next?.Next?.Value;
        Assert.NotNull(secondWeek);

        CalendarRowAccessibleObject thirdWeek = body.RowsAccessibleObjects?.First?.Next?.Next?.Next?.Value;
        Assert.NotNull(thirdWeek);

        Assert.Null(daysOfWeekRow.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(daysOfWeekRow, firstWeek.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(firstWeek, secondWeek.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(firstWeek, daysOfWeekRow.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(secondWeek, firstWeek.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(thirdWeek, secondWeek.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
    }

    [WinFormsFact]
    public void CalendarRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
    {
        using MonthCalendar control = new();
        control.CreateControl();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

        CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects.First?.Value;
        Assert.NotNull(calendar);

        CalendarBodyAccessibleObject body = calendar.CalendarBodyAccessibleObject;
        Assert.NotNull(body);

        CalendarRowAccessibleObject firstWeek = body.RowsAccessibleObjects?.First?.Next?.Value;
        Assert.NotNull(firstWeek);

        CalendarCellAccessibleObject sunday = firstWeek.CellsAccessibleObjects?.First?.Value;
        CalendarCellAccessibleObject saturday = firstWeek.CellsAccessibleObjects?.Last?.Value;

        Assert.Equal(sunday, firstWeek.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(saturday, firstWeek.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsFact]
    public void CalendarRowAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfWeekNumbersVisible()
    {
        using MonthCalendar control = new() { ShowWeekNumbers = true };
        control.CreateControl();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

        CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects.First?.Value;
        Assert.NotNull(calendar);

        CalendarBodyAccessibleObject body = calendar.CalendarBodyAccessibleObject;
        Assert.NotNull(body);

        CalendarRowAccessibleObject firstWeek = body.RowsAccessibleObjects?.First?.Next?.Value;
        Assert.NotNull(firstWeek);

        CalendarWeekNumberCellAccessibleObject weekNumber = firstWeek.WeekNumberCellAccessibleObject;
        CalendarCellAccessibleObject saturday = firstWeek.CellsAccessibleObjects?.Last?.Value;

        Assert.Equal(weekNumber, firstWeek.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(saturday, firstWeek.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    private CalendarRowAccessibleObject CreateCalendarRowAccessibleObject(MonthCalendar control, int calendarIndex = 0, int rowIndex = 0)
    {
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Test name");
        CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);
        CalendarRowAccessibleObject rowAccessibleObject = new(bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex);

        return rowAccessibleObject;
    }
}
