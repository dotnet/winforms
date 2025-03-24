// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewCellContextMenuStripNeededEventArgs : DataGridViewCellEventArgs
{
    public DataGridViewCellContextMenuStripNeededEventArgs(int columnIndex, int rowIndex)
        : base(columnIndex, rowIndex)
    {
    }

    internal DataGridViewCellContextMenuStripNeededEventArgs(int columnIndex, int rowIndex, ContextMenuStrip? contextMenuStrip)
        : base(columnIndex, rowIndex)
    {
        ContextMenuStrip = contextMenuStrip;
    }

    public ContextMenuStrip? ContextMenuStrip { get; set; }
}
