// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;

    public class DataGridViewCellStateChangedEventArgs : EventArgs
    {
        private DataGridViewCell dataGridViewCell;
        private DataGridViewElementStates stateChanged;

        public DataGridViewCellStateChangedEventArgs(DataGridViewCell dataGridViewCell, DataGridViewElementStates stateChanged)
        {
            if (dataGridViewCell == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewCell));
            }
            this.dataGridViewCell = dataGridViewCell;
            this.stateChanged = stateChanged;
        }

        public DataGridViewCell Cell
        {
            get
            {
                return this.dataGridViewCell;
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
