// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;

    /// <include file='doc\DataGridViewRowEventArgs.uex' path='docs/doc[@for="DataGridViewRowEventArgs"]/*' />
    public class DataGridViewRowEventArgs : EventArgs
    {
        private DataGridViewRow dataGridViewRow;

        /// <include file='doc\DataGridViewRowEventArgs.uex' path='docs/doc[@for="DataGridViewRowEventArgs.DataGridViewRowEventArgs"]/*' />
        public DataGridViewRowEventArgs(DataGridViewRow dataGridViewRow)
        {
            if (dataGridViewRow == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewRow));
            }
            Debug.Assert(dataGridViewRow.Index >= -1);
            this.dataGridViewRow = dataGridViewRow;
        }

        /// <include file='doc\DataGridViewRowEventArgs.uex' path='docs/doc[@for="DataGridViewRowEventArgs.Row"]/*' />
        public DataGridViewRow Row
        {
            get
            {
                return this.dataGridViewRow;
            }
        }
    }
}
