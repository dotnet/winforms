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
    public class MonthCalendar_CalendarPreviousButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarPreviousButtonAccessibleObject_ctor_default()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarPreviousButtonAccessibleObject previousButtonAccessibleObject = new(controlAccessibleObject);

            Assert.Equal(controlAccessibleObject, previousButtonAccessibleObject.TestAccessor().Dynamic._monthCalendarAccessibleObject);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarPreviousButtonAccessibleObject_Description_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarPreviousButtonAccessibleObject previousButtonAccessibleObject = new(controlAccessibleObject);

            string actual = previousButtonAccessibleObject.Description;

            Assert.Equal(SR.CalendarPreviousButtonAccessibleObjectDescription, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarPreviousButtonAccessibleObject_GetChildId_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarPreviousButtonAccessibleObject previousButtonAccessibleObject = new(controlAccessibleObject);

            int actual = previousButtonAccessibleObject.GetChildId();

            Assert.Equal(1, actual);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarPreviousButtonAccessibleObject_Name_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarPreviousButtonAccessibleObject previousButtonAccessibleObject = new(controlAccessibleObject);

            string actual = previousButtonAccessibleObject.Name;

            Assert.Equal(SR.MonthCalendarPreviousButtonAccessibleName, actual);
            Assert.False(control.IsHandleCreated);
        }
    }
}
