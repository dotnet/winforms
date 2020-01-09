// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Constants that specify how the date and time picker control displays
    ///  date and time information.
    /// </summary>
    public enum DateTimePickerFormat
    {
        /// <summary>
        ///  Long format - produces output in the form "Wednesday, April 7, 1999"
        /// </summary>
        Long = 0x0001,

        /// <summary>
        ///  Short format - produces output in the form "4/7/99"
        /// </summary>
        Short = 0x0002,

        /// <summary>
        ///  Time format - produces output in time format
        /// </summary>
        Time = 0x0004,

        /// <summary>
        ///  Custom format - produces output in custom format.
        /// </summary>
        Custom = 0x0008,
    }
}
