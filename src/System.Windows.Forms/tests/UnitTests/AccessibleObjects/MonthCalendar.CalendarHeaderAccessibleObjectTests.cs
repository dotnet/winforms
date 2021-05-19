﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class MonthCalendar_CalendarHeaderAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
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

        private CalendarHeaderAccessibleObject CreateCalendarHeaderAccessibleObject(MonthCalendar control, int calendarIndex = 0)
        {
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarAccessibleObject calendarAccessibleObject = new(controlAccessibleObject, calendarIndex, "Test name");
            CalendarHeaderAccessibleObject headerAccessibleObject = new(calendarAccessibleObject, controlAccessibleObject, calendarIndex);

            return headerAccessibleObject;
        }
    }
}
