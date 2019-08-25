// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the starting position that the system uses to arrange minimized
    ///  windows.
    /// </summary>
    [Flags]
    public enum ArrangeStartingPosition
    {
        /// <summary>
        ///  Starts at the lower-left corner of the screen, which is the default position.
        /// </summary>
        BottomLeft = NativeMethods.ARW_BOTTOMLEFT,

        /// <summary>
        ///  Starts at the lower-right corner of the screen.
        /// </summary>
        BottomRight = NativeMethods.ARW_BOTTOMRIGHT,

        /// <summary>
        ///  Hides minimized windows by moving them off the visible area of the
        ///  screen.
        /// </summary>
        Hide = NativeMethods.ARW_HIDE,

        /// <summary>
        ///  Starts at the upper-left corner of the screen.
        /// </summary>
        TopLeft = NativeMethods.ARW_TOPLEFT,

        /// <summary>
        ///  Starts at the upper-right corner of the screen.
        /// </summary>
        TopRight = NativeMethods.ARW_TOPRIGHT,
    }
}
