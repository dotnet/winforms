// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MonthCalendar_CalendarTodayLinkAccessibleObjectTests
{
    [WinFormsFact]
    public void CalendarTodayLinkAccessibleObject_ctor_default()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarTodayLinkAccessibleObject todayLinkAccessibleObject = new(controlAccessibleObject);

        controlAccessibleObject.Should().BeEquivalentTo(todayLinkAccessibleObject.TestAccessor().Dynamic._monthCalendarAccessibleObject);
        control.IsHandleCreated.Should().BeFalse();

        bool canGetDescriptionInternalResult = todayLinkAccessibleObject.TestAccessor().Dynamic.CanGetDescriptionInternal;
        canGetDescriptionInternalResult.Should().BeFalse();

        bool CanGetNameInternalResult = todayLinkAccessibleObject.TestAccessor().Dynamic.CanGetNameInternal;
        CanGetNameInternalResult.Should().BeFalse();
    }

    [WinFormsFact]
    public void CalendarTodayLinkAccessibleObject_Description_ReturnsExpected()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarTodayLinkAccessibleObject todayLinkAccessibleObject = new(controlAccessibleObject);

        string actual = todayLinkAccessibleObject.Description;

        Assert.Equal(SR.CalendarTodayLinkAccessibleObjectDescription, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarTodayLinkAccessibleObject_GetChildId_ReturnsExpected()
    {
        using MonthCalendar control = new();

        control.CreateControl();

        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarTodayLinkAccessibleObject todayLinkAccessibleObject = new(controlAccessibleObject);

        int expected = 3 + controlAccessibleObject.CalendarsAccessibleObjects.Count;
        int actual = todayLinkAccessibleObject.GetChildId();

        Assert.Equal(expected, actual);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarTodayLinkAccessibleObject_GetChildId_ReturnsExpected_IfCalendarsAccessibleObjectsIsNull()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarTodayLinkAccessibleObject todayLinkAccessibleObject = new(controlAccessibleObject);

        int actual = todayLinkAccessibleObject.GetChildId();

        Assert.Null(controlAccessibleObject.CalendarsAccessibleObjects);
        Assert.Equal(-1, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarTodayLinkAccessibleObject_Name_ReturnsExpected()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarTodayLinkAccessibleObject todayLinkAccessibleObject = new(controlAccessibleObject);

        string expected = string.Format(SR.MonthCalendarTodayButtonAccessibleName,
            DateTime.Today.ToShortDateString());
        string actual = todayLinkAccessibleObject.Name;

        Assert.Equal(expected, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarTodayLinkAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = new(control);
        CalendarTodayLinkAccessibleObject todayLink = new(controlAccessibleObject);

        Assert.Equal(controlAccessibleObject, todayLink.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarTodayLinkAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = new(control);
        CalendarTodayLinkAccessibleObject todayLink = new(controlAccessibleObject);

        AccessibleObject lastCalendar = controlAccessibleObject.CalendarsAccessibleObjects?.Last?.Value;

        Assert.Equal(lastCalendar, todayLink.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(todayLink.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarTodayLinkAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = new(control);
        CalendarTodayLinkAccessibleObject todayLink = new(controlAccessibleObject);

        Assert.Null(todayLink.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(todayLink.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
    }
}
