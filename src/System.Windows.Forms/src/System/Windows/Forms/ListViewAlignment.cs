// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies how items align in the <see cref='ListView'/>.
    /// </summary>
    public enum ListViewAlignment
    {
        /// <summary>
        ///  When the user moves an item, it remains where it is dropped.
        /// </summary>
        Default = NativeMethods.LVA_DEFAULT,

        /// <summary>
        ///  Items are aligned to the top of the <see cref='ListView'/> control.
        /// </summary>
        Top = NativeMethods.LVA_ALIGNTOP,

        /// <summary>
        ///  Items are aligned to the left of the <see cref='ListView'/> control.
        /// </summary>
        Left = NativeMethods.LVA_ALIGNLEFT,

        /// <summary>
        ///  Items are aligned to an invisible grid in the control. When the user
        ///  moves an item, it moves to the closest juncture in the grid.
        /// </summary>
        SnapToGrid = NativeMethods.LVA_SNAPTOGRID,
    }
}
