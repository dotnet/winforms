// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\DateTimePickerFormats.uex' path='docs/doc[@for="DateTimePickerFormat"]/*' />
    /// <devdoc>
    ///      Constants that specify how the date and time picker control displays
    ///      date and time information.
    /// </devdoc>    
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum DateTimePickerFormat {

        /// <include file='doc\DateTimePickerFormats.uex' path='docs/doc[@for="DateTimePickerFormat.Long"]/*' />
        /// <devdoc>
        ///     Long format - produces output in the form "Wednesday, April 7, 1999"
        /// </devdoc>
        Long    = 0x0001,

        /// <include file='doc\DateTimePickerFormats.uex' path='docs/doc[@for="DateTimePickerFormat.Short"]/*' />
        /// <devdoc>
        ///     Short format - produces output in the form "4/7/99"
        /// </devdoc>
        Short   = 0x0002,

        /// <include file='doc\DateTimePickerFormats.uex' path='docs/doc[@for="DateTimePickerFormat.Time"]/*' />
        /// <devdoc>
        ///     Time format - produces output in time format
        /// </devdoc>
        Time    = 0x0004,

        /// <include file='doc\DateTimePickerFormats.uex' path='docs/doc[@for="DateTimePickerFormat.Custom"]/*' />
        /// <devdoc>
        ///     Custom format - produces output in custom format.
        /// </devdoc>
        Custom  = 0x0008,

    }
}
