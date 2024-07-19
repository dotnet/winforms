// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MonthCalendar_CalendarDayOfWeekCellAccessibleObjectTests
{
    [WinFormsFact]
    public void CalendarDayOfWeekCellAccessibleObject_ctor_default()
    {
        using MonthCalendar control = new();
        CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

        int columnIndexResult = cellAccessibleObject.TestAccessor().Dynamic._columnIndex;
        columnIndexResult.Should().Be(0);

        int rowIndexResult = cellAccessibleObject.TestAccessor().Dynamic._rowIndex;
        rowIndexResult.Should().Be(0);

        int calendarIndexResult = cellAccessibleObject.TestAccessor().Dynamic._calendarIndex;
        calendarIndexResult.Should().Be(0);

        cellAccessibleObject.Name.Should().Be("Test name");
        cellAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ExpandCollapsePatternId).Should().BeFalse();
        control.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void CalendarDayOfWeekCellAccessibleObject_DateRange_IsNull()
    {
        using MonthCalendar control = new();
        CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

        Assert.Null(cellAccessibleObject.DateRange);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarDayOfWeekCellAccessibleObject_DefaultAction_IsEmpty()
    {
        using MonthCalendar control = new();
        CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

        Assert.Empty(cellAccessibleObject.DefaultAction);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarDayOfWeekCellAccessibleObject_Description_IsNull()
    {
        using MonthCalendar control = new();
        CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

        Assert.Null(cellAccessibleObject.Description);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarDayOfWeekCellAccessibleObject_ControlType_IsHeader()
    {
        using MonthCalendar control = new();
        CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)cellAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_HeaderControlTypeId, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarDayOfWeekCellAccessibleObject_HasKeyboardFocus_IsFalse()
    {
        using MonthCalendar control = new();
        CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

        bool actual = (bool)cellAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);

        Assert.False(actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarDayOfWeekCellAccessibleObject_Role_IsColumnHeader()
    {
        using MonthCalendar control = new();
        CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

        AccessibleRole actual = cellAccessibleObject.Role;

        Assert.Equal(AccessibleRole.ColumnHeader, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarDayOfWeekCellAccessibleObject_State_IsNone()
    {
        using MonthCalendar control = new();
        CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = CreateCalendarDayOfWeekCellCellAccessibleObject(control);

        AccessibleStates actual = cellAccessibleObject.State;

        Assert.Equal(AccessibleStates.None, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarDayOfWeekCellAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "");
        CalendarBodyAccessibleObject body = new(calendar, controlAccessibleObject, 0);
        CalendarRowAccessibleObject row = new(body, controlAccessibleObject, 0, 0);
        CalendarDayOfWeekCellAccessibleObject cell = new(row, body, controlAccessibleObject, 0, 0, 0, "");

        Assert.Equal(row, cell.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarDayOfWeekCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using MonthCalendar control = new()
        {
            SelectionStart = new DateTime(2022, 10, 1) // Set a date to have a stable test case
        };
        control.CreateControl();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

        CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects.First?.Value;
        Assert.NotNull(calendar);

        CalendarBodyAccessibleObject body = calendar.CalendarBodyAccessibleObject;
        Assert.NotNull(body);

        CalendarRowAccessibleObject daysOfWeekRow = body.RowsAccessibleObjects?.First?.Value;
        Assert.NotNull(daysOfWeekRow);

        LinkedList<CalendarCellAccessibleObject> days = daysOfWeekRow.CellsAccessibleObjects;
        CalendarDayOfWeekCellAccessibleObject sunday = days?.First?.Value as CalendarDayOfWeekCellAccessibleObject;
        CalendarDayOfWeekCellAccessibleObject monday = days?.First?.Next?.Value as CalendarDayOfWeekCellAccessibleObject;
        CalendarDayOfWeekCellAccessibleObject tuesday = days?.First?.Next?.Next?.Value as CalendarDayOfWeekCellAccessibleObject;
        CalendarDayOfWeekCellAccessibleObject friday = days?.Last?.Previous?.Value as CalendarDayOfWeekCellAccessibleObject;
        CalendarDayOfWeekCellAccessibleObject saturday = days?.Last?.Value as CalendarDayOfWeekCellAccessibleObject;

        Assert.NotNull(sunday);
        Assert.NotNull(monday);
        Assert.NotNull(tuesday);
        Assert.NotNull(friday);
        Assert.NotNull(saturday);

        Assert.Null(sunday.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(monday, sunday.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(sunday, monday.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(tuesday, monday.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(friday, saturday.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(saturday.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
    }

    [WinFormsFact]
    public void CalendarDayOfWeekCellAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_IfWeekNumbersVisible()
    {
        using MonthCalendar control = new()
        {
            ShowWeekNumbers = true,
            SelectionStart = new DateTime(2022, 10, 1) // Set a date to have a stable test case
        };
        control.CreateControl();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

        CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects.First?.Value;
        Assert.NotNull(calendar);

        CalendarBodyAccessibleObject body = calendar.CalendarBodyAccessibleObject;
        Assert.NotNull(body);

        CalendarRowAccessibleObject daysOfWeekRow = body.RowsAccessibleObjects?.First?.Value;
        Assert.NotNull(daysOfWeekRow);

        LinkedList<CalendarCellAccessibleObject> days = daysOfWeekRow.CellsAccessibleObjects;
        CalendarDayOfWeekCellAccessibleObject sunday = days?.First?.Value as CalendarDayOfWeekCellAccessibleObject;
        CalendarDayOfWeekCellAccessibleObject monday = days?.First?.Next?.Value as CalendarDayOfWeekCellAccessibleObject;

        Assert.NotNull(sunday);
        Assert.NotNull(monday);

        Assert.Null(sunday.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(monday, sunday.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
    }

    [WinFormsFact]
    public void CalendarDayOfWeekCellAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
    {
        using MonthCalendar control = new();
        CalendarDayOfWeekCellAccessibleObject cell = CreateCalendarDayOfWeekCellCellAccessibleObject(control, 0, 0, 0);

        Assert.Null(cell.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(cell.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
    }

    private CalendarDayOfWeekCellAccessibleObject CreateCalendarDayOfWeekCellCellAccessibleObject(MonthCalendar control, int calendarIndex = 0, int rowIndex = 0, int columnIndex = 0)
    {
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Test name");
        CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);
        CalendarRowAccessibleObject rowAccessibleObject = new(bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex);
        CalendarDayOfWeekCellAccessibleObject cellAccessibleObject = new(rowAccessibleObject, bodyAccessibleObject, controlAccessibleObject, calendarIndex, rowIndex, columnIndex, "Test name");

        return cellAccessibleObject;
    }
}
