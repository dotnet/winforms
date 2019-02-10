// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;


    /// <devdoc>
    ///    <para>
    ///       Specifies the direction the system uses to arrange
    ///       minimized windows.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    [Flags]
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum ArrangeDirection {


        /// <devdoc>
        ///    <para>
        ///       Arranges vertically, from top to bottom.
        ///    </para>
        /// </devdoc>
        Down = NativeMethods.ARW_DOWN,


        /// <devdoc>
        ///    <para>
        ///       Arranges horizontally, from left to right.
        ///    </para>
        /// </devdoc>
        Left = NativeMethods.ARW_LEFT,


        /// <devdoc>
        ///    <para>
        ///       Arranges horizontally, from right to left.
        ///    </para>
        /// </devdoc>
        Right = NativeMethods.ARW_RIGHT,


        /// <devdoc>
        ///    <para>
        ///       Arranges vertically, from bottom to top.
        ///    </para>
        /// </devdoc>
        Up = NativeMethods.ARW_UP,
    }
}

