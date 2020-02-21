// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        /// <summary>
        /// Represents MonthCalendar control part constants.
        /// Copied form CommCtrl.h
        /// </summary>
        public enum MCGIP : uint
        {
            /// <summary>
            /// Represents MCGIP_CALENDARCONTROL const.
            /// </summary>
            CALENDARCONTROL = 0,

            /// <summary>
            /// Represents MCGIP_NEXT const.
            /// </summary>
            NEXT = 1,

            /// <summary>
            /// Represents MCGIP_PREV const.
            /// </summary>
            PREV = 2,

            /// <summary>
            /// Represents MCGIP_FOOTER const.
            /// </summary>
            FOOTER = 3,

            /// <summary>
            /// Represents MCGIP_CALENDAR const.
            /// </summary>
            CALENDAR = 4,

            /// <summary>
            /// Represents MCGIP_CALENDARHEADER const.
            /// </summary>
            CALENDARHEADER = 5,

            /// <summary>
            /// Represents MCGIP_CALENDARBODY const.
            /// </summary>
            CALENDARBODY = 6,

            /// <summary>
            /// Represents MCGIP_CALENDARROW const.
            /// </summary>
            CALENDARROW = 7,

            /// <summary>
            /// Represents MCGIP_CALENDARCELL const.
            /// </summary>
            CALENDARCELL = 8
        }
    }
}
