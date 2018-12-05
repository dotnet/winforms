// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.ComponentModel;

    /// <include file='doc\DataGridViewCellCancelEventArgs.uex' path='docs/doc[@for="DataGridViewCellCancelEventArgs"]/*' />
    public class DataGridViewCellCancelEventArgs : CancelEventArgs
    {
        private int columnIndex;
        private int rowIndex;
    
        internal DataGridViewCellCancelEventArgs(DataGridViewCell dataGridViewCell) : this(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex)
        {
        }

        /// <include file='doc\DataGridViewCellCancelEventArgs.uex' path='docs/doc[@for="DataGridViewCellCancelEventArgs.DataGridViewCellCancelEventArgs"]/*' />
        public DataGridViewCellCancelEventArgs(int columnIndex, int rowIndex)
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

        /// <include file='doc\DataGridViewCellCancelEventArgs.uex' path='docs/doc[@for="DataGridViewCellCancelEventArgs.ColumnIndex"]/*' />
        public int ColumnIndex
        {
            get
            {
                return this.columnIndex;
            }
        }

        /// <include file='doc\DataGridViewCellCancelEventArgs.uex' path='docs/doc[@for="DataGridViewCellCancelEventArgs.RowIndex"]/*' />
        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }
    }
}
