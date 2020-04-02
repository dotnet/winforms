// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class MonthCalendarAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void MonthCalendarAccessibleObject_CalendarCountIsCorrect_IfCalendarContainerIsBig ()
        {
            var calendar = new MonthCalendar();
            calendar.Size = new System.Drawing.Size(1000, 500);
            MonthCalendar.MonthCalendarAccessibleObject accessibleObject = Assert.IsType<MonthCalendar.MonthCalendarAccessibleObject>(calendar.AccessibilityObject);
            int childCalendarCount = accessibleObject.GetCalendarCount();
            Assert.Equal(12, childCalendarCount);

            //TO DO: написать тест (на размер календаря 1000х500 приходится 27 чайлд компанентов[3 кнопки и 24 элемента(по 2 на каждый месяц)])
            //почему-то, при наведении на любой месяц, в фокус попадают элементы первого месяца
        }
    }
}
