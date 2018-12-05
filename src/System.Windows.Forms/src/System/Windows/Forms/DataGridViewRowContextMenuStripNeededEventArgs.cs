// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    public class DataGridViewRowContextMenuStripNeededEventArgs : EventArgs
    {
        private int rowIndex;
        private ContextMenuStrip contextMenuStrip;

        public DataGridViewRowContextMenuStripNeededEventArgs(int rowIndex)
        {
            if (rowIndex < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            this.rowIndex = rowIndex;
        }

        internal DataGridViewRowContextMenuStripNeededEventArgs(int rowIndex, ContextMenuStrip contextMenuStrip) : this(rowIndex)
        {
            this.contextMenuStrip = contextMenuStrip;
        }

        public int RowIndex
        {
            get
            {
                return this.rowIndex;
            }
        }

        public ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return this.contextMenuStrip;
            }
            set
            {
                this.contextMenuStrip = value;
            }
        }
    }
}
