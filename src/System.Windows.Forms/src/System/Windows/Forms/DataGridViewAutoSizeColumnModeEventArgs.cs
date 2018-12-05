// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    public class DataGridViewAutoSizeColumnModeEventArgs : EventArgs
    {
        private DataGridViewAutoSizeColumnMode previousMode;
        private DataGridViewColumn dataGridViewColumn;

        public DataGridViewAutoSizeColumnModeEventArgs(DataGridViewColumn dataGridViewColumn, DataGridViewAutoSizeColumnMode previousMode)
        {
            Debug.Assert(dataGridViewColumn != null);
            this.dataGridViewColumn = dataGridViewColumn;
            this.previousMode = previousMode;
        }

        public DataGridViewColumn Column
        {
            get
            {
                return this.dataGridViewColumn;
            }
        }

        public DataGridViewAutoSizeColumnMode PreviousMode
        {
            get
            {
                return this.previousMode;
            }
        }
    }
}
