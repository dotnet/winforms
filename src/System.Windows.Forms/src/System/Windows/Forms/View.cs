// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies how list items are displayed in a <see cref='ListView'/> control.
    /// </summary>
    public enum View
    {
        /// <summary>
        ///  Each item appears as a full-sized icon with a label below it.
        /// </summary>
        LargeIcon = NativeMethods.LVS_ICON,

        /// <summary>
        ///  Each item appears on a seperate line with further
        ///  information about each item arranged in columns. The left
        ///  most column
        ///  contains a small icon and
        ///  label, and subsequent columns contain subitems as specified by the application. A
        ///  column displays a header which can display a caption for the
        ///  column. The user can resize each column at runtime.
        /// </summary>
        Details = NativeMethods.LVS_REPORT,

        /// <summary>
        ///  Each item appears as a small icon with a label to its right.
        /// </summary>
        SmallIcon = NativeMethods.LVS_SMALLICON,

        /// <summary>
        ///  Each item
        ///  appears as a small icon with a label to its right.
        ///  Items are arranged in columns with no column headers.
        /// </summary>
        List = NativeMethods.LVS_LIST,

        /// <summary>
        ///  Tile view.
        /// </summary>
        Tile = NativeMethods.LV_VIEW_TILE,
    }
}
