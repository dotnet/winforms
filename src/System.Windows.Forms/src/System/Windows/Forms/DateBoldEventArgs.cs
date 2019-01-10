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

    /// <include file='doc\DateBoldEvent.uex' path='docs/doc[@for="DateBoldEventArgs"]/*' />
    /// <internalonly/>
    /// <devdoc>
    ///     The month calendar control fires this event to request information
    ///     about how the days within the visible months should be displayed.
    /// </devdoc>
    //
    // 
    public class DateBoldEventArgs : EventArgs {
        readonly DateTime   startDate;  //the starting date
        readonly int        size; // requested length of array
        int[]            daysToBold = null;

        internal DateBoldEventArgs(DateTime start, int size) {
            startDate = start;
            this.size = size;
        }
        /// <include file='doc\DateBoldEvent.uex' path='docs/doc[@for="DateBoldEventArgs.StartDate"]/*' />
        public DateTime StartDate {
            get { return startDate; }
        }
        /// <include file='doc\DateBoldEvent.uex' path='docs/doc[@for="DateBoldEventArgs.Size"]/*' />
        public int Size {
            get { return size; }
        }
        /// <include file='doc\DateBoldEvent.uex' path='docs/doc[@for="DateBoldEventArgs.DaysToBold"]/*' />
        public int[] DaysToBold {
            get { return daysToBold; }
            set { daysToBold = value; }
        }
    }
}
