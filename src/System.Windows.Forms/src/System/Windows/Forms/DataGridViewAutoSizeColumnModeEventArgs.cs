// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public class DataGridViewAutoSizeColumnModeEventArgs : EventArgs
    {
        public DataGridViewAutoSizeColumnModeEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewAutoSizeColumnMode previousMode)
        {
            Column = dataGridViewColumn;
            PreviousMode = previousMode;
        }

        public DataGridViewColumn Column { get; }

        public DataGridViewAutoSizeColumnMode PreviousMode { get; }
    }
}
