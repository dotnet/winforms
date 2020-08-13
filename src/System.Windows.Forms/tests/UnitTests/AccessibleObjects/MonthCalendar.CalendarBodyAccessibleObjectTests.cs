// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;
using static System.Windows.Forms.MonthCalendar;
using static Interop.ComCtl32;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class CalendarBodyAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CalendarBodyAccessibleObject_ctor_Default()
        {
            using MonthCalendar calendar = new MonthCalendar();
            MonthCalendarAccessibleObject calendarAccessibleObject = new MonthCalendarAccessibleObject(calendar);
            CalendarBodyAccessibleObject bodyAccessibleObject = new CalendarBodyAccessibleObject(calendarAccessibleObject, 0);
            Assert.Equal(calendarAccessibleObject, bodyAccessibleObject.Parent);
        }

        [WinFormsFact]
        public void CalendarBodyAccessibleObject_ctor_ThrowsException_IfCalendarAccessibleObjectIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CalendarBodyAccessibleObject(null, 0));
        }
    }
}
