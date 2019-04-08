// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies how parent row labels of a DataGrid
    /// control are displayed.
    /// </devdoc>
    public enum DataGridParentRowsLabelStyle
    {
        /// <devdoc>
        /// Display no parent row labels.
        /// </devdoc>
        None = 0,

        /// <devdoc>
        /// Displaya the parent table name.
        /// </devdoc>
        TableName = 1,

        /// <devdoc>
        /// Displaya the parent column name.
        /// </devdoc>
        ColumnName = 2,

        /// <devdoc>
        /// Displays
        /// both the parent table and column names.
        /// </devdoc>
        Both = 3,
    }
}
