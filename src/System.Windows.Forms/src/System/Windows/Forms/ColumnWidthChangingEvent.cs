// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
using System;
using System.ComponentModel;

    /// <include file='doc\ColumnWidthChangingEvent.uex' path='docs/doc[@for="ColumnWidthChangingEventArgs"]/*' />
    public class ColumnWidthChangingEventArgs : CancelEventArgs {
        int columnIndex;
        int newWidth;

        /// <include file='doc\ColumnWidthChangingEvent.uex' path='docs/doc[@for="ColumnWidthChanging.ColumnWidthChanging"]/*' />
        /// <devdoc>
        ///     Creates a new ColumnWidthChanging event
        /// </devdoc>
        public ColumnWidthChangingEventArgs(int columnIndex, int newWidth, bool cancel) : base (cancel) {
            this.columnIndex = columnIndex;
            this.newWidth = newWidth;
        }

        /// <include file='doc\ColumnWidthChangingEvent.uex' path='docs/doc[@for="ColumnWidthChanging.ColumnWidthChanging1"]/*' />
        /// <devdoc>
        ///     Creates a new ColumnWidthChanging event
        /// </devdoc>
        public ColumnWidthChangingEventArgs(int columnIndex, int newWidth) : base() {
            this.columnIndex = columnIndex;
            this.newWidth = newWidth;
        }

        /// <include file='doc\ColumnWidthChangingEvent.uex' path='docs/doc[@for="ColumnWidthChanging.ColumnIndex"]/*' />
        /// <devdoc>
        ///     Returns the index of the column header whose width is changing
        /// </devdoc>
        public int ColumnIndex {
            get {
                return this.columnIndex;
            }
        }

        /// <include file='doc\ColumnWidthChangingEvent.uex' path='docs/doc[@for="ColumnWidthChanging.NewWidth"]/*' />
        /// <devdoc>
        ///     Returns the new width for the column header who is changing
        /// </devdoc>
        public int NewWidth {
            get {
                return this.newWidth;
            }
            set {
                this.newWidth = value;
            }
        }
    }
}
