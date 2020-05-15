// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.MonthCalendar;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class CalendarGridChildAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarChildAccessibleObject_ctor_ThrowsException_IfCalendarAccessibleObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SubObject(null, 0, CalendarChildType.CalendarBody, new AccessibleObject(), 0));
        }

        [WinFormsFact]
        public void CalendarChildAccessibleObject_ctor_ThrowsException_IfParentAccessibleObjectIsNull()
        {
            using MonthCalendar calendar = new MonthCalendar();
            MonthCalendarAccessibleObject calendarAccessibleObject = new MonthCalendarAccessibleObject(calendar);
            Assert.Throws<ArgumentNullException>(
                () => new SubObject(calendarAccessibleObject, 0, CalendarChildType.CalendarBody, null, 0));
        }

        private class SubObject : CalendarGridChildAccessibleObject
        {
            public SubObject(MonthCalendarAccessibleObject calendarAccessibleObject,
                int calendarIndex, CalendarChildType itemType,
                AccessibleObject parentAccessibleObject,
                int itemIndex) : base(calendarAccessibleObject, calendarIndex, itemType, parentAccessibleObject, itemIndex)
            { }
        }
    }
}
