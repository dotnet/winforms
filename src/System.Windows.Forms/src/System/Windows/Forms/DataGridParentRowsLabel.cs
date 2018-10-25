// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
        /// <include file='doc\DataGridParentRowsLabel.uex' path='docs/doc[@for="DataGridParentRowsLabelStyle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies how parent row labels of a DataGrid
        ///       control are displayed.
        ///    </para>
        /// </devdoc>
        public enum DataGridParentRowsLabelStyle {
            /// <include file='doc\DataGridParentRowsLabel.uex' path='docs/doc[@for="DataGridParentRowsLabelStyle.None"]/*' />
            /// <devdoc>
            ///    <para>
            ///       Display no parent row labels.
            ///    </para>
            /// </devdoc>
            None              = 0,
            /// <include file='doc\DataGridParentRowsLabel.uex' path='docs/doc[@for="DataGridParentRowsLabelStyle.TableName"]/*' />
            /// <devdoc>
            ///    <para>
            ///       Displaya the parent table name.
            ///    </para>
            /// </devdoc>
            TableName            = 1,
            /// <include file='doc\DataGridParentRowsLabel.uex' path='docs/doc[@for="DataGridParentRowsLabelStyle.ColumnName"]/*' />
            /// <devdoc>
            ///    <para>
            ///       Displaya the parent column name.
            ///    </para>
            /// </devdoc>
            ColumnName           = 2,
            /// <include file='doc\DataGridParentRowsLabel.uex' path='docs/doc[@for="DataGridParentRowsLabelStyle.Both"]/*' />
            /// <devdoc>
            ///    <para>
            ///       Displays
            ///       both the parent table and column names.
            ///    </para>
            /// </devdoc>
            Both  = 3,
        }
}
