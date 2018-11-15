// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;



    /// <include file='doc\View.uex' path='docs/doc[@for="View"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies how list items are displayed in
    ///       a <see cref='System.Windows.Forms.ListView'/> control.
    ///    </para>
    /// </devdoc>
    public enum View {

        /// <include file='doc\View.uex' path='docs/doc[@for="View.LargeIcon"]/*' />
        /// <devdoc>
        ///     Each item appears as a full-sized icon with a label below it.
        /// </devdoc>
        LargeIcon = NativeMethods.LVS_ICON,

        /// <include file='doc\View.uex' path='docs/doc[@for="View.Details"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Each item appears on a seperate line with further
        ///       information about each item arranged in columns. The left
        ///       most column
        ///       contains a small icon and
        ///       label, and subsequent columns contain subitems as specified by the application. A
        ///       column displays a header which can display a caption for the
        ///       column. The user can resize each column at runtime.
        ///    </para>
        /// </devdoc>
        Details = NativeMethods.LVS_REPORT,

        /// <include file='doc\View.uex' path='docs/doc[@for="View.SmallIcon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Each item appears as a small icon with a label to its right.
        ///    </para>
        /// </devdoc>
        SmallIcon = NativeMethods.LVS_SMALLICON,

        /// <include file='doc\View.uex' path='docs/doc[@for="View.List"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Each item
        ///       appears as a small icon with a label to its right.
        ///       Items are arranged in columns with no column headers.
        ///    </para>
        /// </devdoc>
        List = NativeMethods.LVS_LIST,

        /// <include file='doc\View.uex' path='docs/doc[@for="View.Tile"]/*' />
        /// <devdoc>
        ///    <para>
        ///         Tile view.
        ///    </para>
        /// </devdoc>
        Tile = NativeMethods.LV_VIEW_TILE,

    }
}
