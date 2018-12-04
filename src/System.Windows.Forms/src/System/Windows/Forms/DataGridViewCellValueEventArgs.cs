// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    /// <include file='doc\DataGridViewCellValueEventArgs.uex' path='docs/doc[@for="DataGridViewCellValueEventArgs"]/*' />
    public class DataGridViewCellValueEventArgs : EventArgs
    {
        private int rowIndex, columnIndex;
        private object val;

        internal DataGridViewCellValueEventArgs()
        {
            this.columnIndex = this.rowIndex = -1;
        }

        /// <include file='doc\DataGridViewCellValueEventArgs.uex' path='docs/doc[@for="DataGridViewCellValueEventArgs.DataGridViewCellValueEventArgs"]/*' />
        public DataGridViewCellValueEventArgs(int columnIndex, int rowIndex)
        {
            if (columnIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }
            if (rowIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
        }

        /// <include file='doc\DataGridViewCellValueEventArgs.uex' path='docs/doc[@for="DataGridViewCellValueEventArgs.ColumnIndex"]/*' />
        public int ColumnIndex
        {
            get
            {
                return this.columnIndex;
            }
        }

        /// <include file='doc\DataGridViewCellValueEventArgs.uex' path='docs/doc[@for="DataGridViewCellValueEventArgs.RowIndex"]/*' />
        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }

        /// <include file='doc\DataGridViewCellValueEventArgs.uex' path='docs/doc[@for="DataGridViewCellValueEventArgs.Value"]/*' />
        public object Value
        {
            get
            {
                return this.val;
            }
            set
            {
                this.val = value;
            }
        }

        internal void SetProperties(int columnIndex, int rowIndex, object value)
        {
            Debug.Assert(columnIndex >= -1);
            Debug.Assert(rowIndex >= -1);
            this.columnIndex = columnIndex;
            this.rowIndex = rowIndex;
            this.val = value;
        }
    }
}
