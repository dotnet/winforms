// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public class DataGridViewCellEventArgs : EventArgs
    {
        internal DataGridViewCellEventArgs(DataGridViewCell dataGridViewCell) : this(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex)
        {
        }

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

            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
        }

        public int ColumnIndex { get; }

        public int RowIndex { get; }
    }
}
