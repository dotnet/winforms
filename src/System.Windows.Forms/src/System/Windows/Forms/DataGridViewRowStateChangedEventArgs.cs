// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;

    public class DataGridViewRowStateChangedEventArgs : EventArgs
    {
        private DataGridViewRow dataGridViewRow;
        private DataGridViewElementStates stateChanged;
    
        public DataGridViewRowStateChangedEventArgs(DataGridViewRow dataGridViewRow, DataGridViewElementStates stateChanged)
        {
            this.dataGridViewRow = dataGridViewRow;
            this.stateChanged = stateChanged;
        }

        public DataGridViewRow Row
        {
            get
            {
                return this.dataGridViewRow;
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
