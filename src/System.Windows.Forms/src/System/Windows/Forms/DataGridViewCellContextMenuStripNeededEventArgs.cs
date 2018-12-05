// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    public class DataGridViewCellContextMenuStripNeededEventArgs : DataGridViewCellEventArgs
    {
        private ContextMenuStrip contextMenuStrip;

        public DataGridViewCellContextMenuStripNeededEventArgs(int columnIndex, int rowIndex) : base(columnIndex, rowIndex)
        {
        }

        internal DataGridViewCellContextMenuStripNeededEventArgs(
            int columnIndex,
            int rowIndex,
            ContextMenuStrip contextMenuStrip) : base(columnIndex, rowIndex)
        {
            this.contextMenuStrip = contextMenuStrip;
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
