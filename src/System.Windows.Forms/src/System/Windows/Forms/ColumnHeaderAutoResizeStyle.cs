// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.Windows.Forms {
    /// <include file='doc\ColumnHeaderAutoResizeStyle.uex' path='docs/doc[@for="ColumnHeaderAutoResizeStyle"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies how the column headers are autoresized in 
    ///       a <see cref='System.Windows.Forms.ListView'/> control.
    ///    </para>
    /// </devdoc>
    public enum ColumnHeaderAutoResizeStyle {
        /// <include file='doc\ColumnHeaderAutoResizeStyle.uex' path='docs/doc[@for="ColumnHeaderAutoResizeStyle.None"]/*' />
        /// <devdoc>
        ///     Do not auto resize the column headers.
        /// </devdoc>
        None,
        /// <include file='doc\ColumnHeaderAutoResizeStyle.uex' path='docs/doc[@for="ColumnHeaderAutoResizeStyle.HeaderSize"]/*' />
        /// <devdoc>
        ///     Autoresize the column headers based on the width of just the column header.
        /// </devdoc>
        HeaderSize,
        /// <include file='doc\ColumnHeaderAutoResizeStyle.uex' path='docs/doc[@for="ColumnHeaderAutoResizeStyle.ColumnContent"]/*' />
        /// <devdoc>
        ///     Autoresize the column headers based on the width of the largest subitem in the column.
        /// </devdoc>
        ColumnContent,
    }
}
