// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;

    public class DataGridViewColumnStateChangedEventArgs : EventArgs
    {
        private DataGridViewColumn dataGridViewColumn;
        private DataGridViewElementStates stateChanged;
    
        public DataGridViewColumnStateChangedEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewElementStates stateChanged)
        {
            this.dataGridViewColumn = dataGridViewColumn;
            this.stateChanged = stateChanged;
        }

        public DataGridViewColumn Column
        {
            get
            {
                return this.dataGridViewColumn;
            }
        }

        public DataGridViewElementStates StateChanged
        {
            get
            {
                return this.stateChanged;
            }
        }
    }
}
