// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVCFMT : int
        {
            // Same as HDF_LEFT
            LEFT = 0x0000,

            // Same as HDF_RIGHT
            RIGHT = 0x0001,

            // Same as HDF_CENTER
            CENTER = 0x0002,

            // Same as HDF_JUSTIFYMASK
            JUSTIFYMASK = 0x0003,

            // Same as HDF_IMAGE
            IMAGE = 0x0800,

            // Same as HDF_BITMAP_ON_RIGHT
            BITMAP_ON_RIGHT = 0x1000,

            // Same as HDF_OWNERDRAW
            COL_HAS_IMAGES = 0x8000,

            /// <summary>
            /// Can't resize the column
            /// </summary>
            // same as HDF_FIXEDWIDTH
            FIXED_WIDTH = 0x00100,

            /// <summary>
            /// If not set, CCM_DPISCALE will govern scaling up fixed width
            /// </summary>
            NO_DPI_SCALE = 0x40000,

            /// <summary>
            /// Width will augment with the row height
            /// </summary>
            FIXED_RATIO = 0x80000,

            //
            // ListView specific flags
            //

            /// <summary>
            /// Forces the column to wrap to the top of the next list of columns.
            /// This flag is <c>ListView</c> specific.
            /// </summary>
            LINE_BREAK = 0x100000,

            /// <summary>
            /// Fills the remainder of the tile area. Might have a title.
            /// This flag is <c>ListView</c> specific.
            /// </summary>
            FILL = 0x200000,

            /// <summary>
            /// Allows the column to wrap within the remaining space in its list of columns.
            /// This flag is <c>ListView</c> specific.
            /// </summary>
            WRAP = 0x400000,

            /// <summary>
            /// Removes the title from the subitem.
            /// This flag is <c>ListView</c> specific.
            /// </summary>
            NO_TITLE = 0x800000,

            /// <summary>
            /// Equivalent to a combination of LVCFMT_LINE_BREAK and LVCFMT_FILL.
            /// This flag is <c>ListView</c> specific.
            /// </summary>
            TILE_PLACEMENTMASK = (LINE_BREAK | FILL),

            /// <summary>
            /// Column is a split button; same as HDF_SPLITBUTTON
            /// This flag is <c>ListView</c> specific.
            /// </summary>
            SPLITBUTTON = 0x1000000
        }
    }
}
