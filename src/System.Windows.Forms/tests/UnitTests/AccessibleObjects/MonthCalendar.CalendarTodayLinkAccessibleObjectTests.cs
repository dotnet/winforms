// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;
using static System.Windows.Forms.MonthCalendar;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class MonthCalendar_CalendarTodayLinkAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarTodayLinkAccessibleObject_ctor_default()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarTodayLinkAccessibleObject todayLinkAccessibleObject = new(controlAccessibleObject);

            Assert.Equal(controlAccessibleObject, todayLinkAccessibleObject.TestAccessor().Dynamic._monthCalendarAccessibleObject);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CalendarTodayLinkAccessibleObject_Description_ReturnsExpected()
        {
            using MonthCalendar control = new();
            MonthCalendarAccessibleObject controlAccessibleObject = (MonthCalendarAccessibleObject)control.AccessibilityObject;
            CalendarTodayLinkAccessibleObject todayLinkAccessibleObject = new(controlAccessibleObject);

            string actual = todayLinkAccessibleObject.Description;

            Assert.Equal(SR.CalendarTodayLinkAccessibleObjectDescription, actual);
            Assert.False(control.IsHandleCreated);
        }
    }
}
