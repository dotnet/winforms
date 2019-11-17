// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        /// <summary>
        /// Represents MonthCalendar control size and color constants.
        /// Copied form CommCtrl.h
        /// </summary>
        public enum MCSC
        {
            /// <summary>
            /// Represents MCSC_BACKGROUND const.
            /// </summary>
            BACKGROUND = 0,

            /// <summary>
            /// Represents MCSC_TEXT const.
            /// </summary>
            TEXT = 1,

            /// <summary>
            /// Represents MCSC_TITLEBK const.
            /// </summary>
            TITLEBK = 2,

            /// <summary>
            /// Represents MCSC_TITLETEXT const.
            /// </summary>
            TITLETEXT = 3,

            /// <summary>
            /// Represents MCSC_MONTHBK const.
            /// </summary>
            MONTHBK = 4,

            /// <summary>
            /// Represents MCSC_TRAILINGTEXT const.
            /// </summary>
            TRAILINGTEXT = 5
        }
    }
}
