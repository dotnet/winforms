// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;

    /// <include file='doc\DataGridViewCellStateChangedEventArgs.uex' path='docs/doc[@for="DataGridViewCellStateChangedEventArgs"]/*' />
    public class DataGridViewCellStateChangedEventArgs : EventArgs
    {
        private DataGridViewCell dataGridViewCell;
        private DataGridViewElementStates stateChanged;

        /// <include file='doc\DataGridViewCellStateChangedEventArgs.uex' path='docs/doc[@for="DataGridViewCellStateChangedEventArgs.DataGridViewCellStateChangedEventArgs"]/*' />
        public DataGridViewCellStateChangedEventArgs(DataGridViewCell dataGridViewCell, DataGridViewElementStates stateChanged)
        {
            if (dataGridViewCell == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewCell));
            }
            this.dataGridViewCell = dataGridViewCell;
            this.stateChanged = stateChanged;
        }

        /// <include file='doc\DataGridViewCellStateChangedEventArgs.uex' path='docs/doc[@for="DataGridViewCellStateChangedEventArgs.Cell"]/*' />
        public DataGridViewCell Cell
        {
            get
            {
                return this.dataGridViewCell;
            }
        }

        /// <include file='doc\DataGridViewCellStateChangedEventArgs.uex' path='docs/doc[@for="DataGridViewCellStateChangedEventArgs.StateChanged"]/*' />
        public DataGridViewElementStates StateChanged
        {
            get
            {
                return this.stateChanged;
            }
        }
    }
}
