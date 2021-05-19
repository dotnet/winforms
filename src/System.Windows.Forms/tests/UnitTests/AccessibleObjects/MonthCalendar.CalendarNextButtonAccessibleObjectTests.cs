﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;
using static System.Windows.Forms.MonthCalendar;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class MonthCalendar_CalendarNextButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarNextButtonAccessibleObject_ctor_default()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarNextButtonAccessibleObject nextButtonAccessibleObject = new(controlAccessibleObject);

            Assert.Equal(controlAccessibleObject, nextButtonAccessibleObject.TestAccessor().Dynamic._monthCalendarAccessibleObject);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarNextButtonAccessibleObject_Description_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarNextButtonAccessibleObject nextButtonAccessibleObject = new(controlAccessibleObject);

            string actual = nextButtonAccessibleObject.Description;

            Assert.Equal(SR.CalendarNextButtonAccessibleObjectDescription, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarNextButtonAccessibleObject_GetChildId_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarNextButtonAccessibleObject nextButtonAccessibleObject = new(controlAccessibleObject);

            int actual = nextButtonAccessibleObject.GetChildId();

            Assert.Equal(2, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarNextButtonAccessibleObject_Name_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarNextButtonAccessibleObject nextButtonAccessibleObject = new(controlAccessibleObject);

            string actual = nextButtonAccessibleObject.Name;

            Assert.Equal(SR.MonthCalendarNextButtonAccessibleName, actual);
            Assert.False(control.IsHandleCreated);
        }
    }
}
