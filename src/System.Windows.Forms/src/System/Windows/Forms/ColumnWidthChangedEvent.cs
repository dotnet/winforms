// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\ColumnWidthChangedEvent.uex' path='docs/doc[@for="ColumnWidthChangedEventArgs"]/*' />
    /// <devdoc>
    /// </devdoc>
    public class ColumnWidthChangedEventArgs : EventArgs {
        readonly int columnIndex;

        /// <include file='doc\ColumnWidthChangedEvent.uex' path='docs/doc[@for="ColumnWidthEventArgs.ColumnWidthChangedEventArgs"]/*' />
        public ColumnWidthChangedEventArgs(int columnIndex) {
            this.columnIndex = columnIndex;
        }

        /// <include file='doc\ColumnWidthChangedEvent.uex' path='docs/doc[@for="ColumnWidthChangedEventArgs.ColumnIndex"]/*' />
        public int ColumnIndex {
            get {
                return columnIndex;
            }

        }
    }
}
