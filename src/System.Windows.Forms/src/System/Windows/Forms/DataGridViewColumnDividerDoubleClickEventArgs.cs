// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public class DataGridViewColumnDividerDoubleClickEventArgs : HandledMouseEventArgs
    {
        private int columnIndex;

        public DataGridViewColumnDividerDoubleClickEventArgs(int columnIndex, HandledMouseEventArgs e) : base(e.Button, e.Clicks, e.X, e.Y, e.Delta, e.Handled)
        {
            if (columnIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            }
            this.columnIndex = columnIndex;
        }

        public int ColumnIndex
        {
            get
            {
                return this.columnIndex;
            }
        }
    }
}
