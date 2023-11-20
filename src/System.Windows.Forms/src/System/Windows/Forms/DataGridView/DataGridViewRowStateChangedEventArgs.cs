// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewRowStateChangedEventArgs : EventArgs
{
    public DataGridViewRowStateChangedEventArgs(DataGridViewRow dataGridViewRow, DataGridViewElementStates stateChanged)
    {
        Row = dataGridViewRow.OrThrowIfNull();
        StateChanged = stateChanged;
    }

    public DataGridViewRow Row { get; }

    public DataGridViewElementStates StateChanged { get; }
}
