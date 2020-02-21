// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        /// <summary>
        /// Represents MonthCalendar Control HitTest values.
        /// Copied form CommCtrl.h
        /// </summary>
        public enum MCHT
        {
            /// <summary>
            /// MCHT_TITLE
            /// </summary>
            TITLE = 0x00010000,

            /// <summary>
            /// Represents MCHT_CALENDAR const.
            /// </summary>
            CALENDAR = 0x00020000,

            /// <summary>
            /// Represents MCHT_TODAYLINK const.
            /// </summary>
            TODAYLINK = 0x00030000,

            /// <summary>
            /// Represents MCHT_CALENDARCONTROL const.
            /// </summary>
            CALENDARCONTROL = 0x00100000,

            /// <summary>
            /// Represents MCHT_NEXT const.
            /// </summary>
            NEXT = 0x01000000,

            /// <summary>
            /// Represents MCHT_PREV const.
            /// </summary>
            PREV = 0x02000000,

            /// <summary>
            /// Represents MCHT_NOWHERE const.
            /// </summary>
            NOWHERE = 0x00000000,

            /// <summary>
            /// Represents MCHT_TITLEBK const.
            /// </summary>
            TITLEBK = TITLE,

            /// <summary>
            /// Represents MCHT_TITLEMONTH const.
            /// </summary>
            TITLEMONTH = TITLE | 0x0001,

            /// <summary>
            /// Represents MCHT_TITLEYEAR const.
            /// </summary>
            TITLEYEAR = TITLE | 0x0002,

            /// <summary>
            /// Represents MCHT_TITLEBTNNEXT const.
            /// </summary>
            TITLEBTNNEXT = TITLE | NEXT | 0x0003,

            /// <summary>
            /// Represents MCHT_TITLEBTNPREV const.
            /// </summary>
            TITLEBTNPREV = TITLE | PREV | 0x0003,

            /// <summary>
            /// Represents MCHT_CALENDARBK const.
            /// </summary>
            CALENDARBK = CALENDAR,

            /// <summary>
            /// Represents MCHT_CALENDARDATE const.
            /// </summary>
            CALENDARDATE = CALENDAR | 0x0001,

            /// <summary>
            /// Represents MCHT_CALENDARDATENEXT const.
            /// </summary>
            CALENDARDATENEXT = CALENDARDATE | NEXT,

            /// <summary>
            /// Represents MCHT_CALENDARDATEPREV const.
            /// </summary>
            CALENDARDATEPREV = CALENDARDATE | PREV,

            /// <summary>
            /// Represents MCHT_CALENDARDAY const.
            /// </summary>
            CALENDARDAY = CALENDAR | 0x0002,

            /// <summary>
            /// Represents MCHT_CALENDARWEEKNUM const.
            /// </summary>
            CALENDARWEEKNUM = CALENDAR | 0x0003,

            /// <summary>
            /// Represents MCHT_CALENDARDATEMIN const.
            /// </summary>
            CALENDARDATEMIN = CALENDAR | 0x0004,

            /// <summary>
            /// Represents MCHT_CALENDARDATEMAX const.
            /// </summary>
            CALENDARDATEMAX = CALENDAR | 0x0005
        }
    }
}
