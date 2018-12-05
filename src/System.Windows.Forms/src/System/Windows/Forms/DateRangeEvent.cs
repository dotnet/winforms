// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Drawing;
    using Microsoft.Win32;

    /// <devdoc>
    ///     The SelectEvent is fired when the user makes an explicit date
    ///     selection within a month calendar control.
    /// </devdoc>
    public class DateRangeEventArgs : EventArgs {

        readonly DateTime start; // The date for the first day in the user's selection range.
        readonly DateTime end;   // The date for the last day in the user's selection range.

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public DateRangeEventArgs(DateTime start, DateTime end) {
            this.start = start;
            this.end = end;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public DateTime Start {
            get { return start; }
        }
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public DateTime End {
            get { return end; }
        }
    }
}
