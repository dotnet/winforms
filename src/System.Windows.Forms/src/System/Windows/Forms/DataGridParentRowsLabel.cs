// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies how parent row labels of a DataGrid
    /// control are displayed.
    /// </devdoc>
    public enum DataGridParentRowsLabelStyle
    {
        /// <summary>
        /// Display no parent row labels.
        /// </devdoc>
        None = 0,

        /// <summary>
        /// Displaya the parent table name.
        /// </devdoc>
        TableName = 1,

        /// <summary>
        /// Displaya the parent column name.
        /// </devdoc>
        ColumnName = 2,

        /// <summary>
        /// Displays
        /// both the parent table and column names.
        /// </devdoc>
        Both = 3,
    }
}
