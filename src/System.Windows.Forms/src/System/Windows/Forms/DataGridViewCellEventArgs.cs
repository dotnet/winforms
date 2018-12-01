// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    /// <include file='doc\DataGridViewCellEventArgs.uex' path='docs/doc[@for="DataGridViewCellEventArgs"]/*' />
    public class DataGridViewCellEventArgs : EventArgs
    {
        private int columnIndex;
        private int rowIndex;
    
        internal DataGridViewCellEventArgs(DataGridViewCell dataGridViewCell) : this(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex)
        {
        }

        /// <include file='doc\DataGridViewCellEventArgs.uex' path='docs/doc[@for="DataGridViewCellEventArgs.DataGridViewCellEventArgs"]/*' />
        public DataGridViewCellEventArgs(int columnIndex, int rowIndex)
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

        /// <include file='doc\DataGridViewCellEventArgs.uex' path='docs/doc[@for="DataGridViewCellEventArgs.ColumnIndex"]/*' />
        public int ColumnIndex
        {
            get
            {
                return this.columnIndex;
            }
        }

        /// <include file='doc\DataGridViewCellEventArgs.uex' path='docs/doc[@for="DataGridViewCellEventArgs.RowIndex"]/*' />
        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }
    }
}
