// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests
{
    public class MonthCalendar_CalendarTodayLinkAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarTodayLinkAccessibleObject_ctor_default()
        {
            using MonthCalendar control = new();
            var controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarTodayLinkAccessibleObject todayLinkAccessibleObject = new(controlAccessibleObject);

            Assert.Equal(controlAccessibleObject, todayLinkAccessibleObject.TestAccessor().Dynamic._monthCalendarAccessibleObject);
            Assert.False(control.IsHandleCreated);
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
    }
}
