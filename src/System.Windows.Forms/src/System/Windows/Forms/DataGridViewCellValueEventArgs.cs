// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    public class DataGridViewCellValueEventArgs : EventArgs
    {
        private int rowIndex, columnIndex;
        private object val;

        internal DataGridViewCellValueEventArgs()
        {
            this.columnIndex = this.rowIndex = -1;
        }

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

        public int ColumnIndex
        {
            get
            {
                return this.columnIndex;
            }
        }

        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }

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
