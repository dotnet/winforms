// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.ComponentModel;

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public class DataGridViewRowCancelEventArgs : CancelEventArgs
    {
        private DataGridViewRow dataGridViewRow;
    
        public DataGridViewRowCancelEventArgs(DataGridViewRow dataGridViewRow)
        {
            Debug.Assert(dataGridViewRow != null);
            Debug.Assert(dataGridViewRow.Index >= 0);
            this.dataGridViewRow = dataGridViewRow;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public DataGridViewRow Row
        {
            get
            {
                return this.dataGridViewRow;
            }
        }
    }
}
