// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class MonthCalendar_CalendarNextButtonAccessibleObjectTests
{
    [WinFormsFact]
    public void CalendarNextButtonAccessibleObject_ctor_default()
    {
        using MonthCalendar control = new();
        control.CreateControl();
        control.PerformLayout();
        var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
        CalendarNextButtonAccessibleObject nextButtonAccessibleObject = new(controlAccessibleObject);

        nextButtonAccessibleObject.Description.Should().Be(SR.CalendarNextButtonAccessibleObjectDescription);
        nextButtonAccessibleObject.GetChildId().Should().Be(2);
        nextButtonAccessibleObject.Name.Should().Be(SR.MonthCalendarNextButtonAccessibleName);
        nextButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent).Should().BeSameAs(controlAccessibleObject);
        nextButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild).Should().BeNull();
        nextButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild).Should().BeNull();

        AccessibleObject previousButton = controlAccessibleObject.PreviousButtonAccessibleObject;
        AccessibleObject firstCalendar = controlAccessibleObject.CalendarsAccessibleObjects?.First?.Value;

        nextButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling).Should().BeSameAs(previousButton);
        nextButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling).Should().BeSameAs(firstCalendar);
        nextButtonAccessibleObject.CanGetDescriptionInternal.Should().BeFalse();
        nextButtonAccessibleObject.CanGetNameInternal.Should().BeFalse();

        Rectangle actual = nextButtonAccessibleObject.Bounds;
        Rectangle actualInClientCoordinates = control.RectangleToClient(actual);

        control.ClientRectangle.Contains(actualInClientCoordinates).Should().BeTrue();
    }
}
