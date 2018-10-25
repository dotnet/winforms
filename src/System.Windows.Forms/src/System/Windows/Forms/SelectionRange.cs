// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;

    using System.Diagnostics;

    using System;
    using System.Globalization;
    using System.Drawing;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.IO;
    using Microsoft.Win32;

    /// <include file='doc\SelectionRange.uex' path='docs/doc[@for="SelectionRange"]/*' />
    /// <devdoc>
    ///     This is a class that represents the date selection range of a MonthCalendar control.
    /// </devdoc>
    [
    TypeConverterAttribute(typeof(SelectionRangeConverter))
    ]
    public sealed class SelectionRange {
        /// <include file='doc\SelectionRange.uex' path='docs/doc[@for="SelectionRange.start"]/*' />
        /// <devdoc>
        ///     The lower limit of the selection range.
        /// </devdoc>
        private DateTime start = DateTime.MinValue.Date;

        /// <include file='doc\SelectionRange.uex' path='docs/doc[@for="SelectionRange.end"]/*' />
        /// <devdoc>
        ///     The upper limit of the selection range.
        /// </devdoc>
        private DateTime end = DateTime.MaxValue.Date;

        /// <include file='doc\SelectionRange.uex' path='docs/doc[@for="SelectionRange.SelectionRange"]/*' />
        /// <devdoc>
        ///     Create a new SelectionRange object with the range [null, null].
        /// </devdoc>
        public SelectionRange() {
        }

        /// <include file='doc\SelectionRange.uex' path='docs/doc[@for="SelectionRange.SelectionRange1"]/*' />
        /// <devdoc>
        ///     Create a new SelectionRange object with the given range.
        /// </devdoc>
        public SelectionRange(DateTime lower, DateTime upper) {
            //NOTE: simcooke: we explicitly DO NOT want to throw an exception here - just silently
            //                swap them around. This is because the win32 control can return non-
            //                normalized ranges.
            
            // We use lower.Date and upper.Date to remove any time component
            //
            if (lower < upper) {
                start = lower.Date;
                end = upper.Date;
            }
            else {
                start = upper.Date;
                end = lower.Date;
            }                        
        }

        /// <include file='doc\SelectionRange.uex' path='docs/doc[@for="SelectionRange.SelectionRange2"]/*' />
        /// <devdoc>
        ///     Create a new SelectionRange object given an existing SelectionRange object.
        /// </devdoc>
        public SelectionRange(SelectionRange range) {
            this.start = range.start;
            this.end = range.end;
        }

        /// <include file='doc\SelectionRange.uex' path='docs/doc[@for="SelectionRange.End"]/*' />
        /// <devdoc>
        ///     Returns the ending time of this range.
        /// </devdoc>
        public DateTime End {
            get { 
                return end;
            }
            set { 
                end = value.Date;
            }
        }

        /// <include file='doc\SelectionRange.uex' path='docs/doc[@for="SelectionRange.Start"]/*' />
        /// <devdoc>
        ///     Starting time of this range
        /// </devdoc>
        public DateTime Start {
            get {
                return start;
            }
            set { 
                start = value.Date;
            }
        }

        /// <include file='doc\SelectionRange.uex' path='docs/doc[@for="SelectionRange.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {
            return "SelectionRange: Start: " + start.ToString() + ", End: " + end.ToString();
        }
    }
}

