// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        /// <summary>
        /// Represents MonthCalendar Control styles constatnts.
        /// Copied form CommCtrl.h
        /// </summary>
        public enum MCS
        {
            /// <summary>
            /// Represents MCS_DAYSTATE const.
            /// </summary>
            DAYSTATE = 0x0001,

            /// <summary>
            /// Represents MCS_MULTISELECT const.
            /// </summary>
            MULTISELECT = 0x0002,

            /// <summary>
            /// Represents MCS_WEEKNUMBERS const.
            /// </summary>
            WEEKNUMBERS = 0x0004,

            /// <summary>
            /// Represents MCS_NOTODAYCIRCLE const.
            /// </summary>
            NOTODAYCIRCLE = 0x0008,

            /// <summary>
            /// Represents MCS_NOTODAY const.
            /// </summary>
            NOTODAY = 0x0010
        }
    }
}
