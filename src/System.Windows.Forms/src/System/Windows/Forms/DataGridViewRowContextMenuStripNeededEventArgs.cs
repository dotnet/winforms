// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public class DataGridViewRowContextMenuStripNeededEventArgs : EventArgs
    {
        public DataGridViewRowContextMenuStripNeededEventArgs(int rowIndex)
        {
            if (rowIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            RowIndex = rowIndex;
        }

        internal DataGridViewRowContextMenuStripNeededEventArgs(int rowIndex, ContextMenuStrip contextMenuStrip) : this(rowIndex)
        {
            ContextMenuStrip = contextMenuStrip;
        }

        public int RowIndex { get; }

        public ContextMenuStrip ContextMenuStrip { get; set; }
    }
}
