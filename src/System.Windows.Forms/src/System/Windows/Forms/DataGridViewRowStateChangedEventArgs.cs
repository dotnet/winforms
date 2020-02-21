// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public class DataGridViewRowStateChangedEventArgs : EventArgs
    {
        public DataGridViewRowStateChangedEventArgs(DataGridViewRow dataGridViewRow, DataGridViewElementStates stateChanged)
        {
            Row = dataGridViewRow;
            StateChanged = stateChanged;
        }

        public DataGridViewRow Row { get; }

        public DataGridViewElementStates StateChanged { get; }
    }
}
