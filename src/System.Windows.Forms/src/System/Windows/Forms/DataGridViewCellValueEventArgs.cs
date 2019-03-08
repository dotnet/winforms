// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms
{
    public class DataGridViewCellValueEventArgs : EventArgs
    {
        internal DataGridViewCellValueEventArgs()
        {
            ColumnIndex = -1;
            RowIndex = -1;
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

            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
        }

        public int ColumnIndex { get; private set; }

        public int RowIndex { get; private set; }

        public object Value { get; set; }

        internal void SetProperties(int columnIndex, int rowIndex, object value)
        {
            Debug.Assert(columnIndex >= -1);
            Debug.Assert(rowIndex >= -1);
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
            Value = value;
        }
    }
}
