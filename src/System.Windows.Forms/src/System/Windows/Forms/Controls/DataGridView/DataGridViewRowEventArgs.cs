// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewRowEventArgs : EventArgs
{
    public DataGridViewRowEventArgs(DataGridViewRow dataGridViewRow)
    {
        Row = dataGridViewRow.OrThrowIfNull();
    }

    public DataGridViewRow Row { get; }
}
