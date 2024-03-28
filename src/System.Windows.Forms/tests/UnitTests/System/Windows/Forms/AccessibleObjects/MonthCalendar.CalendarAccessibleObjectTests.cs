// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MonthCalendar_CalendarAccessibleObjectTests
{
    [WinFormsFact]
    public void CalendarAccessibleObject_ctor_default()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        int calendarIndex = 0;
        string name = "Test name";
        CalendarAccessibleObject calendar = new(controlAccessibleObject, calendarIndex, name);

        Assert.Equal(controlAccessibleObject, calendar.Parent);
        Assert.Equal(calendarIndex, calendar.TestAccessor().Dynamic._calendarIndex);
        Assert.Equal(name, calendar.Name);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_Bounds_ReturnsExpectedSize()
    {
        using MonthCalendar control = new();

        control.CreateControl();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects.First?.Value;
        Rectangle bounds = calendar.Bounds;

        Assert.Equal(217, bounds.Width);
        Assert.Equal(135, bounds.Height);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_CalendarBodyAccessibleObject_IsNotNull()
    {
        using MonthCalendar control = new();

        control.CreateControl();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects.First?.Value;

        Assert.NotNull(calendar.CalendarBodyAccessibleObject);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_CalendarHeaderAccessibleObject_IsNotNull()
    {
        using MonthCalendar control = new();

        control.CreateControl();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects.First?.Value;

        Assert.NotNull(calendar.CalendarHeaderAccessibleObject);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(10, 10, 0, 0)]
    [InlineData(250, 200, 0, 0)]
    [InlineData(500, 200, 0, 0)]
    [InlineData(500, 200, 1, 1)]
    [InlineData(700, 200, 0, 0)]
    [InlineData(700, 200, 1, 1)]
    [InlineData(700, 200, 2, 2)]
    [InlineData(250, 300, 0, 0)]
    [InlineData(250, 300, 1, 0)]
    [InlineData(250, 450, 0, 0)]
    [InlineData(250, 450, 1, 0)]
    [InlineData(250, 450, 2, 0)]
    [InlineData(500, 300, 0, 0)]
    [InlineData(500, 300, 1, 1)]
    [InlineData(500, 300, 2, 0)]
    [InlineData(500, 300, 3, 1)]
    [InlineData(500, 450, 0, 0)]
    [InlineData(500, 450, 1, 1)]
    [InlineData(500, 450, 2, 0)]
    [InlineData(500, 450, 3, 1)]
    [InlineData(500, 450, 4, 0)]
    [InlineData(500, 450, 5, 1)]
    [InlineData(700, 450, 0, 0)]
    [InlineData(700, 450, 1, 1)]
    [InlineData(700, 450, 2, 2)]
    [InlineData(700, 450, 3, 0)]
    [InlineData(700, 450, 4, 1)]
    [InlineData(700, 450, 5, 2)]
    public void CalendarAccessibleObject_Column_ReturnsExpected(int width, int height, int calendarIndex, int expected)
    {
        using MonthCalendar control = new();
        control.Size = new Size(width, height);

        control.CreateControl();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, calendarIndex, "Test name");

        Assert.Equal(expected, calendar.Column);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_ContainingGrid_IsControlAccessibleObject()
    {
        using MonthCalendar control = new();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");

        Assert.Equal(controlAccessibleObject, calendar.ContainingGrid);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> CalendarAccessibleObject_DateRange_IsExpected_ForSpecificCalendar_InMonthView_TestData()
    {
        yield return new object[] { 0, new SelectionRange(new DateTime(2020, 12, 27), new DateTime(2021, 1, 31)) };
        yield return new object[] { 1, new SelectionRange(new DateTime(2021, 2, 1), new DateTime(2021, 2, 28)) };
        yield return new object[] { 2, new SelectionRange(new DateTime(2021, 3, 1), new DateTime(2021, 3, 31)) };
        yield return new object[] { 3, new SelectionRange(new DateTime(2021, 4, 1), new DateTime(2021, 4, 30)) };
        yield return new object[] { 4, new SelectionRange(new DateTime(2021, 5, 1), new DateTime(2021, 5, 31)) };
        yield return new object[] { 5, new SelectionRange(new DateTime(2021, 6, 1), new DateTime(2021, 7, 10)) };
    }

    [WinFormsTheory]
    [MemberData(nameof(CalendarAccessibleObject_DateRange_IsExpected_ForSpecificCalendar_InMonthView_TestData))]
    public void CalendarAccessibleObject_DateRange_IsExpected_ForSpecificCalendar_InMonthView(int calendarIndex, SelectionRange expected)
    {
        using MonthCalendar control = new();
        control.FirstDayOfWeek = Day.Sunday;
        control.Size = new Size(450, 450);
        control.SelectionStart = new DateTime(2021, 1, 1);

        control.CreateControl();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        LinkedListNode<CalendarAccessibleObject> calendarNode = controlAccessibleObject.CalendarsAccessibleObjects.First;

        for (int i = 1; i <= calendarIndex; i++)
        {
            calendarNode = calendarNode.Next;
        }

        Assert.NotNull(calendarNode);

        CalendarAccessibleObject calendar = calendarNode.Value;

        Assert.Equal(expected.Start, calendar.DateRange.Start);
        Assert.Equal(expected.End, calendar.DateRange.End);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void CalendarAccessibleObject_GetChildId_ReturnsExpected(int calendarIndex)
    {
        using MonthCalendar control = new();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, calendarIndex, "Test name");

        Assert.Equal(calendarIndex + 3, calendar.GetChildId());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_GetColumnHeaderItems_ReturnsNull()
    {
        using MonthCalendar control = new();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");

        Assert.Null(calendar.GetColumnHeaderItems());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CalendarAccessibleObject_IsKeyboardFocusable_IsTrueIfEnabled(bool enabled)
    {
        using MonthCalendar control = new();
        control.Enabled = enabled;

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");

        Assert.Equal(enabled, (bool)calendar.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_GetRowHeaderItems_ReturnsNull()
    {
        using MonthCalendar control = new();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");

        Assert.Null(calendar.GetRowHeaderItems());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_GridItemPattern_IsSupported()
    {
        using MonthCalendar control = new();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");

        Assert.True(calendar.IsPatternSupported(UIA_PATTERN_ID.UIA_GridItemPatternId));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_TableItemPattern_IsSupported()
    {
        using MonthCalendar control = new();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");

        Assert.True(calendar.IsPatternSupported(UIA_PATTERN_ID.UIA_TableItemPatternId));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_Name_HasInitValue()
    {
        using MonthCalendar control = new();
        string initName = "Test name";

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, initName);

        Assert.Equal(initName, calendar.Name);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_Parent_IsControlAccessibleObject()
    {
        using MonthCalendar control = new();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");

        Assert.Equal(controlAccessibleObject, calendar.Parent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_Role_IsClient()
    {
        using MonthCalendar control = new();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");

        Assert.Equal(AccessibleRole.Client, calendar.Role);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(3, 1)]
    [InlineData(4, 2)]
    [InlineData(5, 2)]
    public void CalendarAccessibleObject_Row_IsExpected(int calendarIndex, int expected)
    {
        using MonthCalendar control = new();
        control.Size = new Size(450, 450);

        control.CreateControl();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        LinkedListNode<CalendarAccessibleObject> calendarNode = controlAccessibleObject.CalendarsAccessibleObjects.First;

        for (int i = 1; i <= calendarIndex; i++)
        {
            calendarNode = calendarNode.Next;
        }

        Assert.NotNull(calendarNode);

        CalendarAccessibleObject calendar = calendarNode.Value;

        Assert.Equal(expected, calendar.Row);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_State_IsNone_IfControlIsNotEnabled()
    {
        using MonthCalendar control = new();
        control.Enabled = false;

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "Test name");

        Assert.Equal(AccessibleStates.None, calendar.State);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using MonthCalendar control = new();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "");

        Assert.Equal(controlAccessibleObject, calendar.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_IfSingleMonthView()
    {
        using MonthCalendar control = new();
        control.CreateControl();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

        CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects?.First?.Value;

        Assert.NotNull(calendar);
        Assert.Equal(controlAccessibleObject.TodayLinkAccessibleObject, calendar.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_IfMultiMonthView()
    {
        using MonthCalendar control = new()
        {
            CalendarDimensions = new Size { Width = 2, Height = 2 }
        };
        control.CreateControl();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

        LinkedList<CalendarAccessibleObject> calendars = controlAccessibleObject.CalendarsAccessibleObjects;
        CalendarAccessibleObject calendar1 = calendars?.First?.Value;
        CalendarAccessibleObject calendar2 = calendars?.First?.Next?.Value;
        CalendarAccessibleObject calendar3 = calendars?.First?.Next?.Next?.Value;
        CalendarAccessibleObject calendar4 = calendars?.First?.Next?.Next?.Next?.Value;

        Assert.NotNull(calendar1);
        Assert.NotNull(calendar2);
        Assert.NotNull(calendar3);
        Assert.NotNull(calendar4);

        Assert.Equal(calendar2, calendar1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(calendar3, calendar2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(calendar4, calendar3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(controlAccessibleObject.TodayLinkAccessibleObject, calendar4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_IfSingleMonthView()
    {
        using MonthCalendar control = new();
        control.CreateControl();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

        CalendarAccessibleObject calendar = controlAccessibleObject.CalendarsAccessibleObjects?.First?.Value;

        Assert.NotNull(calendar);
        Assert.Equal(controlAccessibleObject.NextButtonAccessibleObject, calendar.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_IfMultiMonthView()
    {
        using MonthCalendar control = new()
        {
            CalendarDimensions = new Size { Width = 2, Height = 2 }
        };
        control.CreateControl();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;

        LinkedList<CalendarAccessibleObject> calendars = controlAccessibleObject.CalendarsAccessibleObjects;
        CalendarAccessibleObject calendar1 = calendars?.First?.Value;
        CalendarAccessibleObject calendar2 = calendars?.First?.Next?.Value;
        CalendarAccessibleObject calendar3 = calendars?.First?.Next?.Next?.Value;
        CalendarAccessibleObject calendar4 = calendars?.First?.Next?.Next?.Next?.Value;

        Assert.NotNull(calendar1);
        Assert.NotNull(calendar2);
        Assert.NotNull(calendar3);
        Assert.NotNull(calendar4);

        Assert.Equal(controlAccessibleObject.NextButtonAccessibleObject, calendar1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(calendar1, calendar2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(calendar2, calendar3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(calendar3, calendar4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected()
    {
        using MonthCalendar control = new();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "");

        Assert.Equal(calendar.CalendarHeaderAccessibleObject, calendar.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected()
    {
        using MonthCalendar control = new();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "");

        Assert.Equal(calendar.CalendarBodyAccessibleObject, calendar.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
    }
}
