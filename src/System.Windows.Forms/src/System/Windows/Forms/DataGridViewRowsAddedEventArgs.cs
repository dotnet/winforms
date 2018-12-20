// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    /// <include file='doc\DataGridViewRowsAddedEventArgs.uex' path='docs/doc[@for="DataGridViewRowsAddedEventArgs"]/*' />
    public class DataGridViewRowsAddedEventArgs : EventArgs
    {
        private int rowIndex, rowCount;
    
        /// <include file='doc\DataGridViewRowsAddedEventArgs.uex' path='docs/doc[@for="DataGridViewRowsAddedEventArgs.DataGridViewRowsAddedEventArgs"]/*' />
        public DataGridViewRowsAddedEventArgs(int rowIndex, int rowCount)
        {
            Debug.Assert(rowIndex >= 0);
            Debug.Assert(rowCount >= 1);
            this.rowIndex = rowIndex;
            this.rowCount = rowCount;
        }

        /// <include file='doc\DataGridViewRowsAddedEventArgs.uex' path='docs/doc[@for="DataGridViewRowsAddedEventArgs.RowIndex"]/*' />
        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }

        /// <include file='doc\DataGridViewRowsAddedEventArgs.uex' path='docs/doc[@for="DataGridViewRowsAddedEventArgs.RowCount"]/*' />
        public int RowCount
        {
            get
            {
                return this.rowCount;
            }
        }
    }
}
