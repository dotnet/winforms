// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    [SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags")]
    [Flags]
    public enum DragDropEffects
    {
        /// <devdoc>
        /// The drop target does not accept the data.
        /// </devdoc>
        None = 0x00000000,

        /// <devdoc>
        /// The data is copied to the drop target.
        /// </devdoc>
        Copy = 0x00000001,

        /// <devdoc>
        /// The data from the drag source is moved to the drop target.
        /// </devdoc>
        Move = 0x00000002,

        /// <devdoc>
        /// The data from the drag source is linked to the drop target.
        /// </devdoc>
        Link = 0x00000004,

        /// <devdoc>
        /// Scrolling is about to start or is currently occurring in the drop target.
        /// </devdoc>
        Scroll = unchecked((int)0x80000000),

        /// <devdoc>
        /// The data is copied, removed from the drag source, and scrolled in the 
        /// drop target. NOTE: Link is intentionally not present in All.
        /// </devdoc>
        All = Copy | Move | Scroll,
    }
}
