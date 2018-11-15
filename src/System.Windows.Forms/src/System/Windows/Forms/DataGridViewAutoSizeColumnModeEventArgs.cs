// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    /// <include file='doc\DataGridViewAutoSizeColumnModeEventArgs.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnModeEventArgs"]/*' />
    public class DataGridViewAutoSizeColumnModeEventArgs : EventArgs
    {
        private DataGridViewAutoSizeColumnMode previousMode;
        private DataGridViewColumn dataGridViewColumn;

        /// <include file='doc\DataGridViewAutoSizeColumnModeEventArgs.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnModeEventArgs.DataGridViewAutoSizeColumnModeEventArgs"]/*' />
        public DataGridViewAutoSizeColumnModeEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewAutoSizeColumnMode previousMode)
        {
            Debug.Assert(dataGridViewColumn != null);
            this.dataGridViewColumn = dataGridViewColumn;
            this.previousMode = previousMode;
        }

        /// <include file='doc\DataGridViewAutoSizeColumnModeEventArgs.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnModeEventArgs.Column"]/*' />
        public DataGridViewColumn Column
        {
            get
            {
                return this.dataGridViewColumn;
            }
        }

        /// <include file='doc\DataGridViewAutoSizeColumnModeEventArgs.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnModeEventArgs.PreviousMode"]/*' />
        public DataGridViewAutoSizeColumnMode PreviousMode
        {
            get
            {
                return this.previousMode;
            }
        }
    }
}
