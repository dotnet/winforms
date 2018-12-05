// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.Diagnostics;

    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <devdoc>
    ///    <para>Specifies the effects of a drag-and-drop operation.</para>
    /// </devdoc>
    [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags")]
    [Flags]
    public enum DragDropEffects {

        /// <devdoc>
        ///    <para>
        ///       The drop target does not accept the data.
        ///    </para>
        /// </devdoc>
        None = 0x00000000,
        /// <devdoc>
        ///    <para>
        ///       The data is copied to the drop target.
        ///    </para>
        /// </devdoc>
        Copy = 0x00000001,
        /// <devdoc>
        ///    <para>
        ///       The data from the drag source is moved to the drop target.
        ///    </para>
        /// </devdoc>
        Move = 0x00000002,
        /// <devdoc>
        ///    <para>
        ///       The data from the drag source is linked to the drop target.
        ///    </para>
        /// </devdoc>
        Link = 0x00000004,
        /// <devdoc>
        ///    <para>
        ///       Scrolling is about to start or is currently occurring in the drop target.
        ///    </para>
        /// </devdoc>
        Scroll = unchecked((int)0x80000000),
        /// <devdoc>
        ///    <para>
        ///       The data is copied, removed from the drag source, and scrolled in the 
        ///	      drop target.  NOTE: Link is intentionally not present in All.
        ///    </para>
        /// </devdoc>
        All = Copy | Move | Scroll,
    }
}
