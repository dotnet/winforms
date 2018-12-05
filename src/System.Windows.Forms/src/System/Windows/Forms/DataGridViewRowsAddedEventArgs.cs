// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class DataGridViewRowsAddedEventArgs : EventArgs
    {
        private int rowIndex, rowCount;
    
        public DataGridViewRowsAddedEventArgs(int rowIndex, int rowCount)
        {
            Debug.Assert(rowIndex >= 0);
            Debug.Assert(rowCount >= 1);
            this.rowIndex = rowIndex;
            this.rowCount = rowCount;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int RowCount
        {
            get
            {
                return this.rowCount;
            }
        }
    }
}
