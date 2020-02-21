// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class MonthCalendar
    {
        /// <summary>
        ///  This enumeration has specific areas of the MonthCalendar control as
        ///  its enumerated values. The hitArea member of System.Windows.Forms.Win32.HitTestInfo
        ///  will be one of these enumerated values, and indicates which portion of
        ///  a month calendar is under a specific point.
        /// </summary>
        public enum HitArea
        {
            /// <summary>
            ///  The given point was not on the month calendar control, or it was
            ///  in an inactive portion of the control.
            /// </summary>
            Nowhere = 0,

            /// <summary>
            ///  The given point was over the background of a month's title
            /// </summary>
            TitleBackground = 1,

            /// <summary>
            ///  The given point was in a month's title bar, over a month name
            /// </summary>
            TitleMonth = 2,

            /// <summary>
            ///  The given point was in a month's title bar, over the year value
            /// </summary>
            TitleYear = 3,

            /// <summary>
            ///  The given point was over the button at the top right corner of
            ///  the control. If the user clicks here, the month calendar will
            ///  scroll its display to the next month or set of months
            /// </summary>
            NextMonthButton = 4,

            /// <summary>
            ///  The given point was over the button at the top left corner of
            ///  the control. If the user clicks here, the month calendar will
            ///  scroll its display to the previous month or set of months
            /// </summary>
            PrevMonthButton = 5,

            /// <summary>
            ///  The given point was in the calendar's background
            /// </summary>
            CalendarBackground = 6,

            /// <summary>
            ///  The given point was on a particular date within the calendar,
            ///  and the time member of HitTestInfo will be set to the date at
            ///  the given point.
            /// </summary>
            Date = 7,

            /// <summary>
            ///  The given point was over a date from the next month (partially
            ///  displayed at the end of the currently displayed month). If the
            ///  user clicks here, the month calendar will scroll its display to
            ///  the next month or set of months.
            /// </summary>
            NextMonthDate = 8,

            /// <summary>
            ///  The given point was over a date from the previous month (partially
            ///  displayed at the end of the currently displayed month). If the
            ///  user clicks here, the month calendar will scroll its display to
            ///  the previous month or set of months.
            /// </summary>
            PrevMonthDate = 9,

            /// <summary>
            ///  The given point was over a day abbreviation ("Fri", for example).
            ///  The time member of HitTestInfo will be set to the corresponding
            ///  date on the top row.
            /// </summary>
            DayOfWeek = 10,

            /// <summary>
            ///  The given point was over a week number. This will only occur if
            ///  the showWeekNumbers property of MonthCalendar is enabled. The
            ///  time member of HitTestInfo will be set to the corresponding date
            ///  in the leftmost column.
            /// </summary>
            WeekNumbers = 11,

            /// <summary>
            ///  The given point was on the "today" link at the bottom of the
            ///  month calendar control
            /// </summary>
            TodayLink = 12,
        }
    }
}
