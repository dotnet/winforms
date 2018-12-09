// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    /// <include file='doc\DataGridViewCellMouseEventArgs.uex' path='docs/doc[@for="DataGridViewCellMouseEventArgs"]/*' />
    public class DataGridViewCellMouseEventArgs : MouseEventArgs
    {
        private int rowIndex, columnIndex;
    
        /// <include file='doc\DataGridViewCellMouseEventArgs.uex' path='docs/doc[@for="DataGridViewCellMouseEventArgs.DataGridViewCellMouseEventArgs"]/*' />
        public DataGridViewCellMouseEventArgs(int columnIndex, 
            int rowIndex, 
            int localX, 
            int localY, 
            MouseEventArgs e) : base(e.Button, e.Clicks, localX, localY, e.Delta)
        {
            if (columnIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }
            if (rowIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            this.columnIndex = columnIndex;
            this.rowIndex = rowIndex;
        }

        /// <include file='doc\DataGridViewCellMouseEventArgs.uex' path='docs/doc[@for="DataGridViewCellMouseEventArgs.ColumnIndex"]/*' />
        public int ColumnIndex
        {
            get
            {
                return this.columnIndex;
            }
        }

        /// <include file='doc\DataGridViewCellMouseEventArgs.uex' path='docs/doc[@for="DataGridViewCellMouseEventArgs.RowIndex"]/*' />
        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }
    }
}
