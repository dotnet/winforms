// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <devdoc>
    /// </devdoc>
    public class ColumnWidthChangedEventArgs : EventArgs {
        readonly int columnIndex;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public ColumnWidthChangedEventArgs(int columnIndex) {
            this.columnIndex = columnIndex;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int ColumnIndex {
            get {
                return columnIndex;
            }

        }
    }
}
