// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        /// <summary>
        /// Represents MonthCalendar Control Messages.
        /// Copied form CommCtrl.h
        /// </summary>
        public enum MCM
        {
            /// <summary>
            /// Represents MCM_FIRST const.
            /// </summary>
            FIRST = 0x1000,

            /// <summary>
            /// Represents MCM_GETCURSEL const.
            /// </summary>
            GETCURSEL = FIRST + 1,

            /// <summary>
            /// Represents MCM_SETMAXSELCOUNT const.
            /// </summary>
            SETMAXSELCOUNT = FIRST + 4,

            /// <summary>
            /// Represents MCM_GETSELRANGE const.
            /// </summary>
            GETSELRANGE = FIRST + 5,

            /// <summary>
            /// Represents MCM_SETSELRANGE const.
            /// </summary>
            SETSELRANGE = FIRST + 6,

            /// <summary>
            /// Represents MCM_GETMONTHRANGE const.
            /// </summary>
            GETMONTHRANGE = FIRST + 7,

            /// <summary>
            /// Represents MCM_GETMINREQRECT const.
            /// </summary>
            GETMINREQRECT = FIRST + 9,

            /// <summary>
            /// Represents MCM_SETCOLOR const.
            /// </summary>
            SETCOLOR = FIRST + 10,

            /// <summary>
            /// Represents MCM_SETTODAY const.
            /// </summary>
            SETTODAY = FIRST + 12,

            /// <summary>
            /// Represents MCM_GETTODAY const.
            /// </summary>
            GETTODAY = FIRST + 13,

            /// <summary>
            /// Represents MCM_HITTEST const.
            /// </summary>
            HITTEST = FIRST + 14,

            /// <summary>
            /// Represents MCM_SETFIRSTDAYOFWEEK const.
            /// </summary>
            SETFIRSTDAYOFWEEK = FIRST + 15,

            /// <summary>
            /// Represents MCM_GETRANGE const.
            /// </summary>
            GETRANGE = FIRST + 17,

            /// <summary>
            /// Represents MCM_SETRANGE const.
            /// </summary>
            SETRANGE = FIRST + 18,

            /// <summary>
            /// Represents MCM_SETMONTHDELTA const.
            /// </summary>
            SETMONTHDELTA = FIRST + 20,

            /// <summary>
            /// Represents MCM_GETMAXTODAYWIDTH const.
            /// </summary>
            GETMAXTODAYWIDTH = FIRST + 21,

            /// <summary>
            /// Represents MCM_GETCALENDARGRIDINFO const.
            /// </summary>
            GETCALENDARGRIDINFO = FIRST + 24
        }
    }
}
