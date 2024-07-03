// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
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

        headerAccessibleObject.RuntimeId.Length.Should().Be(4);
        headerAccessibleObject.GetChildId().Should().Be(1);
        headerAccessibleObject.Name.Should().BeEmpty();
        headerAccessibleObject.Parent.Should().BeOfType<CalendarAccessibleObject>();
        headerAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent).Should().BeOfType<CalendarAccessibleObject>();
        headerAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling).Should().BeOfType<CalendarBodyAccessibleObject>();
        headerAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling).Should().BeNull();
        headerAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild).Should().BeNull();
        headerAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild).Should().BeNull();
        headerAccessibleObject.CanGetNameInternal.Should().BeFalse();
    }

    [WinFormsFact]
    public void CalendarHeaderAccessibleObject_Name_ReturnsExpected()
    {
        using MonthCalendar control = new();

        control.CreateControl();
        control.SetSelectionRange(new DateTime(2020, 8, 19), new DateTime(2020, 8, 19));
        CalendarHeaderAccessibleObject headerAccessibleObject = CreateCalendarHeaderAccessibleObject(control);
        string actual = headerAccessibleObject.Name;

        actual.Should().Be("August 2020");
        control.IsHandleCreated.Should().BeTrue();
    }

    [WinFormsFact]
    public void CalendarHeaderAccessibleObject_Bounds_ReturnsExpected()
    {
        using MonthCalendar control = new();
        control.CreateControl();
        control.PerformLayout();
        CalendarHeaderAccessibleObject headerAccessibleObject = CreateCalendarHeaderAccessibleObject(control);
        Rectangle actual = headerAccessibleObject.Bounds;

        Rectangle actualInClientCoordinates = control.RectangleToClient(actual);
        control.ClientRectangle.Contains(actualInClientCoordinates).Should().BeTrue();
    }

    private CalendarHeaderAccessibleObject CreateCalendarHeaderAccessibleObject(MonthCalendar control, int calendarIndex = 0)
    {
        MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Test name");
        CalendarHeaderAccessibleObject headerAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);

        return headerAccessibleObject;
    }
}
