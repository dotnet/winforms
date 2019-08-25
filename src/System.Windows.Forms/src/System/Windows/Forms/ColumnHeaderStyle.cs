// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies how <see cref='ListView'/> column headers
    ///  behave.
    /// </summary>
    public enum ColumnHeaderStyle
    {
        /// <summary>
        ///  No visible column header.
        /// </summary>
        None = 0,

        /// <summary>
        ///  Visible column header that does not respond to clicking.
        /// </summary>
        Nonclickable = 1,

        /// <summary>
        ///  Visible column header that responds to clicking.
        /// </summary>
        Clickable = 2,
    }
}
