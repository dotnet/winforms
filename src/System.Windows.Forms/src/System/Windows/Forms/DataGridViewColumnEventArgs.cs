// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    public class DataGridViewColumnEventArgs : EventArgs
    {
        private DataGridViewColumn dataGridViewColumn;

        public DataGridViewColumnEventArgs(DataGridViewColumn dataGridViewColumn)
        {
            if (dataGridViewColumn == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewColumn));
            }
            Debug.Assert(dataGridViewColumn.Index >= -1);
            this.dataGridViewColumn = dataGridViewColumn;
        }

        public DataGridViewColumn Column
        {
            get
            {
                return this.dataGridViewColumn;
            }
        }
    }
}
