// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MonthCalendar_CalendarBodyAccessibleObjectTests
{
    [WinFormsFact]
    public void CalendarBodyAccessibleObject_ctor_default()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, 0, "Test name");
        CalendarBodyAccessibleObject accessibleObject = new(calendarAccessibleObject, controlAccessibleObject, 0);

        accessibleObject.Parent.Should().Be(calendarAccessibleObject);
        control.IsHandleCreated.Should().BeFalse();
        accessibleObject.RowOrColumnMajor.Should().Be(RowOrColumnMajor.RowOrColumnMajor_RowMajor);
    }

    [WinFormsFact]
    public void CalendarBodyAccessibleObject_Bounds_HasExpectedSize()
    {
        using MonthCalendar control = new();

        control.CreateControl();
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

        Assert.Equal(217, accessibleObject.Bounds.Width);
        Assert.Equal(106, accessibleObject.Bounds.Height);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH, 7)]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_YEAR, 4)]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_DECADE, 4)]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_CENTURY, 4)]
    public void CalendarBodyAccessibleObject_ColumnCount_ReturnsExpected(int view, int expected)
    {
        using MonthCalendar control = new();

        control.CreateControl();
        PInvokeCore.SendMessage(control, PInvoke.MCM_SETCURRENTVIEW, 0, view);
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

        Assert.Equal(expected, accessibleObject.ColumnCount);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarBodyAccessibleObject_GetChildId_ReturnsExpected()
    {
        using MonthCalendar control = new();
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

        Assert.Equal(2, accessibleObject.GetChildId());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_YEAR)]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_DECADE)]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_CENTURY)]
    public void CalendarBodyAccessibleObject_GetColumnHeaders_IsNull_IfViewIsNotMonth(int view)
    {
        using MonthCalendar control = new();

        control.CreateControl();
        PInvokeCore.SendMessage(control, PInvoke.MCM_SETCURRENTVIEW, 0, view);
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

        Assert.Null(accessibleObject.GetColumnHeaders());
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarBodyAccessibleObject_GetColumnHeaders_HasSevenCells()
    {
        using MonthCalendar control = new();

        control.CreateControl();
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

        Assert.Equal(7, accessibleObject.GetColumnHeaders().Length); // Contains days of week
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarBodyAccessibleObject_ControlType_IsTable()
    {
        using MonthCalendar control = new();
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_TableControlTypeId, (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_GridPatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TablePatternId)]
    public void CalendarBodyAccessibleObject_Supports_GridAndTablePatterns(int pattern)
    {
        using MonthCalendar control = new();
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)pattern));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH, 0, "January 2021")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH, 1, "February 2021")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH, 2, "March 2021")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH, 3, "April 2021")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_YEAR, 0, "2021")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_YEAR, 1, "2022")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_YEAR, 2, "2023")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_YEAR, 3, "2024")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_DECADE, 0, "2020-2029")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_DECADE, 1, "2030-2039")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_DECADE, 2, "2040-2049")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_DECADE, 3, "2050-2059")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_CENTURY, 0, "2000-2099")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_CENTURY, 1, "2100-2199")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_CENTURY, 2, "2200-2299")]
    [InlineData((int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_CENTURY, 3, "2300-2399")]
    public void CalendarBodyAccessibleObject_Name_IsExpected(int view, int calendarIndex, string expected)
    {
        using MonthCalendar control = new();
        control.FirstDayOfWeek = Day.Sunday;
        control.Size = new Size(450, 450);
        control.SelectionStart = new DateTime(2021, 1, 1);

        control.CreateControl();
        PInvokeCore.SendMessage(control, PInvoke.MCM_SETCURRENTVIEW, 0, (nint)view);
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        LinkedListNode<CalendarAccessibleObject> calendarNode = controlAccessibleObject.CalendarsAccessibleObjects.First;

        for (int i = 1; i <= calendarIndex; i++)
        {
            calendarNode = calendarNode.Next;
        }

        Assert.NotNull(calendarNode);

        CalendarBodyAccessibleObject calendarBody = calendarNode.Value.CalendarBodyAccessibleObject;

        Assert.Equal(expected, calendarBody.Name);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarBodyAccessibleObject_Parent_IsCalendarAccessibleObject()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, 0, "Test name");
        CalendarBodyAccessibleObject accessibleObject = new(calendarAccessibleObject, controlAccessibleObject, 0);

        Assert.Equal(calendarAccessibleObject, accessibleObject.Parent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarBodyAccessibleObject_Role_IsTable()
    {
        using MonthCalendar control = new();
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

        Assert.Equal(AccessibleRole.Table, accessibleObject.Role);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarBodyAccessibleObject_State_IsDefault()
    {
        using MonthCalendar control = new();
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

        Assert.Equal(AccessibleStates.Default, accessibleObject.State);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarBodyAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");
        CalendarBodyAccessibleObject calendarBody = new(calendar, controlAccessibleObject, 0);

        Assert.Equal(calendar, calendarBody.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarBodyAccessibleObject_FragmentNavigate_NextSibling_ReturnsNull()
    {
        using MonthCalendar control = new();
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control, 0);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarBodyAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");
        CalendarBodyAccessibleObject calendarBody = new(calendar, controlAccessibleObject, 0);

        AccessibleObject expected = calendar.CalendarHeaderAccessibleObject;

        Assert.Equal(expected, calendarBody.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarBodyAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
    {
        using MonthCalendar control = new();
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control, 0);

        AccessibleObject firstRow = accessibleObject.RowsAccessibleObjects?.First();
        AccessibleObject lastRow = accessibleObject.RowsAccessibleObjects?.Last();

        Assert.Equal(firstRow, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(lastRow, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
    }

    private CalendarBodyAccessibleObject CreateCalendarBodyAccessibleObject(MonthCalendar control, int calendarIndex = 0)
    {
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Test name");
        CalendarBodyAccessibleObject bodyAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);

        return bodyAccessibleObject;
    }

    [WinFormsTheory]
    [InlineData(-1, -1, null)]
    [InlineData(5, 6, typeof(CalendarCellAccessibleObject))]
    public void CalendarBodyAccessibleObject_GetItem_ReturnsExpectedResult_ForGivenIndices(int rowIndex, int columnIndex, Type expectedType)
    {
        using MonthCalendar control = new();
        control.CreateControl();
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

        var result = accessibleObject.GetItem(rowIndex, columnIndex);

        if (expectedType is null)
        {
            result.Should().BeNull();
        }
        else
        {
            result.Should().NotBeNull();
            result.Should().BeOfType(expectedType);
        }

        control.IsHandleCreated.Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(true, (int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH, 6)]
    [InlineData(false, (int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_MONTH, 0)]
    [InlineData(true, (int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_YEAR, 0)]
    [InlineData(true, (int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_DECADE, 0)]
    [InlineData(true, (int)MONTH_CALDENDAR_MESSAGES_VIEW.MCMV_CENTURY, 0)]
    public void CalendarBodyAccessibleObject_GetRowHeaders_ReturnsExpected(bool showWeekNumbers, int viewInt, int expectedCount)
    {
        MONTH_CALDENDAR_MESSAGES_VIEW view = (MONTH_CALDENDAR_MESSAGES_VIEW)viewInt;
        using MonthCalendar control = new() { ShowWeekNumbers = showWeekNumbers };
        control.CreateControl();
        PInvokeCore.SendMessage(control, PInvoke.MCM_SETCURRENTVIEW, 0, (nint)view);
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

        var rowHeaders = accessibleObject.GetRowHeaders();

        if (expectedCount == 0)
        {
            rowHeaders.Should().BeNull();
        }
        else
        {
            rowHeaders.Should().NotBeNull();
            rowHeaders.Length.Should().Be(expectedCount);
        }

        control.IsHandleCreated.Should().BeTrue();
    }

    [WinFormsFact]
    public void CalendarBodyAccessibleObject_GetRowHeaders_ReturnsCorrectType()
    {
        using MonthCalendar control = new() { ShowWeekNumbers = true };
        control.CreateControl();
        CalendarBodyAccessibleObject accessibleObject = CreateCalendarBodyAccessibleObject(control);

        var rowHeaders = accessibleObject.GetRowHeaders();

        rowHeaders.Should().AllBeOfType<CalendarWeekNumberCellAccessibleObject>();
        control.IsHandleCreated.Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(null, false)]
    [InlineData("CustomName", false)]
    [InlineData("AccessibleName", false, true)]
    public void CalendarBodyAccessibleObject_CanGetNameInternal_ShouldBeConsistentlyFalse(string name, bool expected, bool isAccessibleName = false)
    {
        using MonthCalendar control = new();
        if (isAccessibleName)
        {
            control.AccessibleName = name;
        }
        else
        {
            control.Name = name;
        }

        var accessibleObject = CreateCalendarBodyAccessibleObject(control);
        accessibleObject.CanGetNameInternal.Should().Be(expected);
    }
}
