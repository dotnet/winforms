// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the layout of multiple document interface (MDI) child windows in an MDI parent window.
    /// </summary>
    public enum MdiLayout
    {
        /// <summary>
        ///  All MDI child windows are cascaded within the client region of the
        ///  MDI parent form.
        /// </summary>
        Cascade = 0,

        /// <summary>
        ///  All MDI child windows are tiled horizontally within the client region
        ///  of the MDI parent form.
        /// </summary>
        TileHorizontal = 1,

        /// <summary>
        ///  All MDI child windows are tiled vertically within the client region of
        ///  the MDI parent form.
        /// </summary>
        TileVertical = 2,

        /// <summary>
        ///  All MDI child icons are arranged within the client region of the MDI
        ///  parent form. An application sets this layout to arrange all minimized
        ///  MDI child windows (in the bottom of the client area).
        ///  It does not affect child windows that are not minimized.
        /// </summary>
        ArrangeIcons = 3,
    }
}
