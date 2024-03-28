// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MonthCalendar_CalendarHeaderAccessibleObjectTests
{
    [WinFormsFact]
    public void CalendarHeaderAccessibleObject_ctor_default()
    {
        using MonthCalendar control = new();
        CalendarHeaderAccessibleObject headerAccessibleObject = CreateCalendarHeaderAccessibleObject(control);

        Assert.Equal(0, headerAccessibleObject.TestAccessor().Dynamic._calendarIndex);
        Assert.Equal(4, headerAccessibleObject.RuntimeId.Length);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarHeaderAccessibleObject_GetChildId_ReturnsExpected()
    {
        using MonthCalendar control = new();
        CalendarHeaderAccessibleObject headerAccessibleObject = CreateCalendarHeaderAccessibleObject(control);

        int actual = headerAccessibleObject.GetChildId();

        Assert.Equal(1, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarHeaderAccessibleObject_Name_IsEmpty_IfControlIsNotCreated()
    {
        using MonthCalendar control = new();
        CalendarHeaderAccessibleObject headerAccessibleObject = CreateCalendarHeaderAccessibleObject(control);

        string actual = headerAccessibleObject.Name;

        Assert.Empty(actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarHeaderAccessibleObject_Name_ReturnsExpected()
    {
        using MonthCalendar control = new();

        control.CreateControl();
        control.SetSelectionRange(new DateTime(2020, 8, 19), new DateTime(2020, 8, 19));
        CalendarHeaderAccessibleObject headerAccessibleObject = CreateCalendarHeaderAccessibleObject(control);
        string actual = headerAccessibleObject.Name;

        Assert.Equal("August 2020", actual);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarHeaderAccessibleObject_Parent_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, 0, "Test name");
        CalendarHeaderAccessibleObject headerAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, 0);

        AccessibleObject actual = headerAccessibleObject.Parent;

        Assert.Equal(calendarAccessibleObject, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarHeaderAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "");
        CalendarHeaderAccessibleObject header = new(calendar, controlAccessibleObject, 0);

        Assert.Equal(calendar, header.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarHeaderAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendar = new(controlAccessibleObject, 0, "");
        CalendarHeaderAccessibleObject header = new(calendar, controlAccessibleObject, 0);

        Assert.Equal(calendar.CalendarBodyAccessibleObject, header.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(header.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarHeaderAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
    {
        using MonthCalendar control = new();
        CalendarHeaderAccessibleObject header = CreateCalendarHeaderAccessibleObject(control, 0);

        Assert.Null(header.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(header.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
    }

    private CalendarHeaderAccessibleObject CreateCalendarHeaderAccessibleObject(MonthCalendar control, int calendarIndex = 0)
    {
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Test name");
        CalendarHeaderAccessibleObject headerAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);

        return headerAccessibleObject;
    }
}
