﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Constants that specify how the date and time picker control displays
    /// date and time information.
    /// </devdoc>    
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum DateTimePickerFormat
    {
        /// <devdoc>
        /// Long format - produces output in the form "Wednesday, April 7, 1999"
        /// </devdoc>
        Long = 0x0001,

        /// <devdoc>
        /// Short format - produces output in the form "4/7/99"
        /// </devdoc>
        Short = 0x0002,

        /// <devdoc>
        /// Time format - produces output in time format
        /// </devdoc>
        Time = 0x0004,

        /// <devdoc>
        /// Custom format - produces output in custom format.
        /// </devdoc>
        Custom = 0x0008,
    }
}
