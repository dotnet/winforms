// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        internal enum CalendarChildType
        {
            Undefined = -1,
            NextButton = 1,
            PreviousButton = 2,
            Footer = 3,
            Calendar = 4,
            CalendarHeader = 5,
            CalendarBody = 6,
            CalendarRow = 7,
            CalendarCell = 8,
            TodayLink = 9
        }
    }
}
