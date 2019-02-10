// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

        /// <devdoc>
        ///    <para>
        ///       Specifies how parent row labels of a DataGrid
        ///       control are displayed.
        ///    </para>
        /// </devdoc>
        public enum DataGridParentRowsLabelStyle {

            /// <devdoc>
            ///    <para>
            ///       Display no parent row labels.
            ///    </para>
            /// </devdoc>
            None              = 0,

            /// <devdoc>
            ///    <para>
            ///       Displaya the parent table name.
            ///    </para>
            /// </devdoc>
            TableName            = 1,

            /// <devdoc>
            ///    <para>
            ///       Displaya the parent column name.
            ///    </para>
            /// </devdoc>
            ColumnName           = 2,

            /// <devdoc>
            ///    <para>
            ///       Displays
            ///       both the parent table and column names.
            ///    </para>
            /// </devdoc>
            Both  = 3,
        }
}
