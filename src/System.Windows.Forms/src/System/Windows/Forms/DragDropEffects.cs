// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    [Flags]
    public enum DragDropEffects
    {
        /// <summary>
        ///  The drop target does not accept the data.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        ///  The data is copied to the drop target.
        /// </summary>
        Copy = 0x00000001,

        /// <summary>
        ///  The data from the drag source is moved to the drop target.
        /// </summary>
        Move = 0x00000002,

        /// <summary>
        ///  The data from the drag source is linked to the drop target.
        /// </summary>
        Link = 0x00000004,

        /// <summary>
        ///  Scrolling is about to start or is currently occurring in the drop target.
        /// </summary>
        Scroll = unchecked((int)0x80000000),

        /// <summary>
        ///  The data is copied, removed from the drag source, and scrolled in the
        ///  drop target. NOTE: Link is intentionally not present in All.
        /// </summary>
        All = Copy | Move | Scroll,
    }
}
