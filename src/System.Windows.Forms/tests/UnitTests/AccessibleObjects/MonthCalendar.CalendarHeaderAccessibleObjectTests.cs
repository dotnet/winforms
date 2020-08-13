// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class MonthCalendarCalendarHeaderAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarHeaderAccessibleObject_Name_Get_ReturnsExpected()
        {
            using var calendar = new MonthCalendar();
            MonthCalendar.MonthCalendarAccessibleObject accessibleObject = Assert.IsType<MonthCalendar.MonthCalendarAccessibleObject>(calendar.AccessibilityObject);
            var headerAccessibleObject = new MonthCalendar.CalendarHeaderAccessibleObject(accessibleObject, 0);
            Assert.Empty(headerAccessibleObject.Name);
        }
    }
}
