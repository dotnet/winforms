// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the starting position that the system uses to arrange minimized
    /// windows.
    /// </devdoc>
    [Flags]
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Maps to native enum.")]
    public enum ArrangeStartingPosition
    {
        /// <devdoc>
        /// Starts at the lower-left corner of the screen, which is the default position.
        /// </devdoc>
        BottomLeft = NativeMethods.ARW_BOTTOMLEFT,

        /// <devdoc>
        /// Starts at the lower-right corner of the screen.
        /// </devdoc>
        BottomRight = NativeMethods.ARW_BOTTOMRIGHT,

        /// <devdoc>
        /// Hides minimized windows by moving them off the visible area of the
        /// screen.
        /// </devdoc>
        Hide = NativeMethods.ARW_HIDE,

        /// <devdoc>
        /// Starts at the upper-left corner of the screen.
        /// </devdoc>

        TopLeft = NativeMethods.ARW_TOPLEFT,

        /// <devdoc>
        /// Starts at the upper-right corner of the screen.
        /// </devdoc>
        TopRight = NativeMethods.ARW_TOPRIGHT,
    }
}

