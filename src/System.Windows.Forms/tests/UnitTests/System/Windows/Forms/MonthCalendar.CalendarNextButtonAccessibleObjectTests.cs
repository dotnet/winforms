// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests
{
    public class MonthCalendar_CalendarNextButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarNextButtonAccessibleObject_ctor_default()
        {
            using MonthCalendar control = new();
            var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarNextButtonAccessibleObject nextButtonAccessibleObject = new(controlAccessibleObject);

            Assert.Equal(controlAccessibleObject, nextButtonAccessibleObject.TestAccessor().Dynamic._monthCalendarAccessibleObject);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarNextButtonAccessibleObject_Description_ReturnsExpected()
        {
            using MonthCalendar control = new();
            var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarNextButtonAccessibleObject nextButtonAccessibleObject = new(controlAccessibleObject);

            string actual = nextButtonAccessibleObject.Description;

            Assert.Equal(SR.CalendarNextButtonAccessibleObjectDescription, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarNextButtonAccessibleObject_GetChildId_ReturnsExpected()
        {
            using MonthCalendar control = new();
            var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarNextButtonAccessibleObject nextButtonAccessibleObject = new(controlAccessibleObject);

            int actual = nextButtonAccessibleObject.GetChildId();

            Assert.Equal(2, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarNextButtonAccessibleObject_Name_ReturnsExpected()
        {
            using MonthCalendar control = new();
            var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarNextButtonAccessibleObject nextButtonAccessibleObject = new(controlAccessibleObject);

            string actual = nextButtonAccessibleObject.Name;

            Assert.Equal(SR.MonthCalendarNextButtonAccessibleName, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarNextButtonAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = new(control);
            CalendarNextButtonAccessibleObject nextButton = new(controlAccessibleObject);

            Assert.Equal(controlAccessibleObject, nextButton.FragmentNavigate(Interop.UiaCore.NavigateDirection.Parent));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarNextButtonAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = new(control);
            CalendarNextButtonAccessibleObject nextButton = new(controlAccessibleObject);

            AccessibleObject previousButton = controlAccessibleObject.PreviousButtonAccessibleObject;
            AccessibleObject firstCalendar = controlAccessibleObject.CalendarsAccessibleObjects?.First?.Value;

            Assert.Equal(previousButton, nextButton.FragmentNavigate(Interop.UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(firstCalendar, nextButton.FragmentNavigate(Interop.UiaCore.NavigateDirection.NextSibling));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarNextButtonAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = new(control);
            CalendarNextButtonAccessibleObject nextButton = new(controlAccessibleObject);

            Assert.Null(nextButton.FragmentNavigate(Interop.UiaCore.NavigateDirection.FirstChild));
            Assert.Null(nextButton.FragmentNavigate(Interop.UiaCore.NavigateDirection.LastChild));
            Assert.False(control.IsHandleCreated);
        }
    }
}
