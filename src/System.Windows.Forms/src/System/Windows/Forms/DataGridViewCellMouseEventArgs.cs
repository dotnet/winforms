// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public class DataGridViewCellMouseEventArgs : MouseEventArgs
    {
        public DataGridViewCellMouseEventArgs(int columnIndex,
            int rowIndex,
            int localX,
            int localY,
            MouseEventArgs e) : base(e?.Button ?? MouseButtons.None, e?.Clicks ?? 0, localX, localY, e?.Delta ?? 0)
        {
            if (columnIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }
            if (rowIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
        }

        public int ColumnIndex { get; }

        public int RowIndex { get; }
    }
}
