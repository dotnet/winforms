// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <devdoc>
    ///    <para>
    ///       Specifies the starting position that the system uses to
    ///       arrange minimized
    ///       windows.
    ///    </para>
    /// </devdoc>
    [Flags]
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum ArrangeStartingPosition {

        /// <devdoc>
        ///    <para>
        ///       Starts at the lower-left corner of the screen, which is the default position.
        ///    </para>
        /// </devdoc>
        BottomLeft = NativeMethods.ARW_BOTTOMLEFT,

        /// <devdoc>
        ///    <para>
        ///       Starts at the lower-right corner of the screen.
        ///    </para>
        /// </devdoc>
        BottomRight = NativeMethods.ARW_BOTTOMRIGHT,

        /// <devdoc>
        ///    <para>
        ///       Hides minimized windows by moving them off the visible area of the
        ///       screen.
        ///    </para>
        /// </devdoc>
        Hide = NativeMethods.ARW_HIDE,

        /// <devdoc>
        ///    <para>
        ///       Starts at the upper-left corner of the screen.
        ///    </para>
        /// </devdoc>

        TopLeft = NativeMethods.ARW_TOPLEFT,

        /// <devdoc>
        ///    <para>
        ///       Starts at the upper-right corner of the screen.
        ///    </para>
        /// </devdoc>
        TopRight = NativeMethods.ARW_TOPRIGHT,
    }
}

