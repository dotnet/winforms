// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    /// <include file='doc\DataGridViewHitTestType.uex' path='docs/doc[@for="DataGridViewHitTestType"]/*' />
    /// <devdoc>
    /// <para>Specifies the part of the <see cref='System.Windows.Forms.DataGridView'/> control where the mouse is.</para>
    /// </devdoc>
    public enum DataGridViewHitTestType
    {
        /// <include file='doc\DataGridViewHitTestType.uex' path='docs/doc[@for="DataGridViewHitTestType.None"]/*' />
        None          = 0,

        /// <include file='doc\DataGridViewHitTestType.uex' path='docs/doc[@for="DataGridViewHitTestType.Cell"]/*' />
        Cell          = 1,

        /// <include file='doc\DataGridViewHitTestType.uex' path='docs/doc[@for="DataGridViewHitTestType.ColumnHeader"]/*' />
        ColumnHeader  = 2,

        /// <include file='doc\DataGridViewHitTestType.uex' path='docs/doc[@for="DataGridViewHitTestType.RowHeader"]/*' />
        RowHeader     = 3,

        /// <include file='doc\DataGridViewHitTestType.uex' path='docs/doc[@for="DataGridViewHitTestType.TopLeftHeader"]/*' />
        TopLeftHeader = 4,

        /// <include file='doc\DataGridViewHitTestType.uex' path='docs/doc[@for="DataGridViewHitTestType.HorizontalScrollBar"]/*' />
        HorizontalScrollBar = 5,

        /// <include file='doc\DataGridViewHitTestType.uex' path='docs/doc[@for="DataGridViewHitTestType.VerticalScrollBar"]/*' />
        VerticalScrollBar = 6
    }
}
