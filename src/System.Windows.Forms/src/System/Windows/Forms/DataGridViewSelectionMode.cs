// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    /// <include file='doc\DataGridViewSelectionMode.uex' path='docs/doc[@for="DataGridViewSelectionMode.DataGridViewSelectionMode"]/*' />
    /// <devdoc>
    /// <para></para>
    /// </devdoc>
    public enum DataGridViewSelectionMode 
    {
        /// <include file='doc\DataGridViewSelectionMode.uex' path='docs/doc[@for="DataGridViewSelectionMode.CellSelect"]/*' />
        CellSelect = 0,

        /// <include file='doc\DataGridViewSelectionMode.uex' path='docs/doc[@for="DataGridViewSelectionMode.FullRowSelect"]/*' />
        FullRowSelect = 1,

        /// <include file='doc\DataGridViewSelectionMode.uex' path='docs/doc[@for="DataGridViewSelectionMode.FullColumnSelect"]/*' />
        FullColumnSelect = 2,

        /// <include file='doc\DataGridViewSelectionMode.uex' path='docs/doc[@for="DataGridViewSelectionMode.RowHeaderSelect"]/*' />
        RowHeaderSelect = 3,

        /// <include file='doc\DataGridViewSelectionMode.uex' path='docs/doc[@for="DataGridViewSelectionMode.ColumnHeaderSelect"]/*' />
        ColumnHeaderSelect = 4
    }
}
