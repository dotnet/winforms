// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewRowContextMenuStripNeededEventArgs : EventArgs
{
    public DataGridViewRowContextMenuStripNeededEventArgs(int rowIndex)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(rowIndex, -1);

        RowIndex = rowIndex;
    }

    internal DataGridViewRowContextMenuStripNeededEventArgs(int rowIndex, ContextMenuStrip? contextMenuStrip)
        : this(rowIndex)
    {
        ContextMenuStrip = contextMenuStrip;
    }

    public int RowIndex { get; }

    public ContextMenuStrip? ContextMenuStrip { get; set; }
}
