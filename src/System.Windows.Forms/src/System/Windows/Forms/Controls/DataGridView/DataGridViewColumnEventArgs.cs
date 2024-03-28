// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewColumnEventArgs : EventArgs
{
    public DataGridViewColumnEventArgs(DataGridViewColumn dataGridViewColumn)
    {
        ArgumentNullException.ThrowIfNull(dataGridViewColumn);

        Debug.Assert(dataGridViewColumn.Index >= -1);
        Column = dataGridViewColumn;
    }

    public DataGridViewColumn Column { get; }
}
