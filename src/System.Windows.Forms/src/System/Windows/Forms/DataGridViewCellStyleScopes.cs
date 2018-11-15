// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    /// <include file='doc\DataGridViewCellStyleScopes.uex' path='docs/doc[@for="DataGridViewCellStyleScopes.DataGridViewCellStyleScopes"]/*' />
    /// <devdoc>
    /// <para></para>
    /// </devdoc>
    [Flags]
    public enum DataGridViewCellStyleScopes
    {
        /// <include file='doc\DataGridViewCellStyleScopes.uex' path='docs/doc[@for="DataGridViewCellStyleScopes.None"]/*' />
        None = 0x00,

        /// <include file='doc\DataGridViewCellStyleScopes.uex' path='docs/doc[@for="DataGridViewCellStyleScopes.Cell"]/*' />
        Cell = 0x01,

        /// <include file='doc\DataGridViewCellStyleScopes.uex' path='docs/doc[@for="DataGridViewCellStyleScopes.Column"]/*' />
        Column = 0x02,

        /// <include file='doc\DataGridViewCellStyleScopes.uex' path='docs/doc[@for="DataGridViewCellStyleScopes.Row"]/*' />
        Row = 0x04,

        /// <include file='doc\DataGridViewCellStyleScopes.uex' path='docs/doc[@for="DataGridViewCellStyleScopes.DataGridView"]/*' />
        DataGridView = 0x08,

        /// <include file='doc\DataGridViewCellStyleScopes.uex' path='docs/doc[@for="DataGridViewCellStyleScopes.ColumnHeaders"]/*' />
        ColumnHeaders = 0x10,

        /// <include file='doc\DataGridViewCellStyleScopes.uex' path='docs/doc[@for="DataGridViewCellStyleScopes.RowHeaders"]/*' />
        RowHeaders = 0x20,

        /// <include file='doc\DataGridViewCellStyleScopes.uex' path='docs/doc[@for="DataGridViewCellStyleScopes.Rows"]/*' />
        Rows = 0x40,

        /// <include file='doc\DataGridViewCellStyleScopes.uex' path='docs/doc[@for="DataGridViewCellStyleScopes.AlternatingRows"]/*' />
        AlternatingRows = 0x80
    }
}
