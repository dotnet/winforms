// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MonthCalendar_CalendarPreviousButtonAccessibleObjectTests
{
    [WinFormsFact]
    public void CalendarPreviousButtonAccessibleObject_ctor_default()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarPreviousButtonAccessibleObject previousButtonAccessibleObject = new(controlAccessibleObject);

        controlAccessibleObject.Should().BeEquivalentTo(previousButtonAccessibleObject.TestAccessor().Dynamic._monthCalendarAccessibleObject);
        control.IsHandleCreated.Should().BeFalse();
        previousButtonAccessibleObject.CanGetDescriptionInternal.Should().BeFalse();
        previousButtonAccessibleObject.CanGetNameInternal.Should().BeFalse();
    }

    [WinFormsFact]
    public void CalendarPreviousButtonAccessibleObject_Description_ReturnsExpected()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarPreviousButtonAccessibleObject previousButtonAccessibleObject = new(controlAccessibleObject);

        string actual = previousButtonAccessibleObject.Description;

        Assert.Equal(SR.CalendarPreviousButtonAccessibleObjectDescription, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarPreviousButtonAccessibleObject_GetChildId_ReturnsExpected()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarPreviousButtonAccessibleObject previousButtonAccessibleObject = new(controlAccessibleObject);

        int actual = previousButtonAccessibleObject.GetChildId();

        Assert.Equal(1, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarPreviousButtonAccessibleObject_Name_ReturnsExpected()
    {
        using MonthCalendar control = new();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarPreviousButtonAccessibleObject previousButtonAccessibleObject = new(controlAccessibleObject);

        string actual = previousButtonAccessibleObject.Name;

        Assert.Equal(SR.MonthCalendarPreviousButtonAccessibleName, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarPreviousButtonAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = new(control);
        CalendarPreviousButtonAccessibleObject prevButton = new(controlAccessibleObject);

        Assert.Equal(controlAccessibleObject, prevButton.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarPreviousButtonAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = new(control);
        CalendarPreviousButtonAccessibleObject prevButton = new(controlAccessibleObject);

        AccessibleObject nextButton = controlAccessibleObject.NextButtonAccessibleObject;

        Assert.Null(prevButton.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(nextButton, prevButton.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void CalendarPreviousButtonAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
    {
        using MonthCalendar control = new();
        MonthCalendarAccessibleObject controlAccessibleObject = new(control);
        CalendarPreviousButtonAccessibleObject prevButton = new(controlAccessibleObject);

        Assert.Null(prevButton.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(prevButton.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
    }
}
