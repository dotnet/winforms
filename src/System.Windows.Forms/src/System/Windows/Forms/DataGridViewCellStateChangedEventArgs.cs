// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public class DataGridViewCellStateChangedEventArgs : EventArgs
    {
        public DataGridViewCellStateChangedEventArgs(DataGridViewCell dataGridViewCell, DataGridViewElementStates stateChanged)
        {
            Cell = dataGridViewCell ?? throw new ArgumentNullException(nameof(dataGridViewCell));
            StateChanged = stateChanged;
        }

        public DataGridViewCell Cell { get; }

        public DataGridViewElementStates StateChanged { get; }
    }
}
