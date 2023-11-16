// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewColumnStateChangedEventArgs : EventArgs
{
    public DataGridViewColumnStateChangedEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewElementStates stateChanged)
    {
        ArgumentNullException.ThrowIfNull(dataGridViewColumn);

        Column = dataGridViewColumn;
        StateChanged = stateChanged;
    }

    public DataGridViewColumn Column { get; }

    public DataGridViewElementStates StateChanged { get; }
}
