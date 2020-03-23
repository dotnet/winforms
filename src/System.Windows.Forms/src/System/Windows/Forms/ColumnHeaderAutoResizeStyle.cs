// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies how the column headers are autoresized in a <see cref='ListView'/>
    ///  control.
    /// </summary>
    public enum ColumnHeaderAutoResizeStyle
    {
        /// <summary>
        ///  Do not auto resize the column headers.
        /// </summary>
        None,

        /// <summary>
        ///  Autoresize the column headers based on the width of just the column
        ///  header.
        /// </summary>
        HeaderSize,

        /// <summary>
        ///  Autoresize the column headers based on the width of the largest
        ///  subitem in the column.
        /// </summary>
        ColumnContent,
    }
}
