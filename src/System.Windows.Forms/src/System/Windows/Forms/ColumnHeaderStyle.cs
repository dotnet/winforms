// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies how <see cref='System.Windows.Forms.ListView'/> column headers
    /// behave.
    /// </devdoc>
    public enum ColumnHeaderStyle
    {
        /// <devdoc>
        /// No visible column header.
        /// </devdoc>
        None = 0,

        /// <devdoc>
        /// Visible column header that does not respond to clicking.
        /// </devdoc>
        Nonclickable = 1,

        /// <devdoc>
        /// Visible column header that responds to clicking.
        /// </devdoc>
        Clickable = 2,
    }
}
