// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the direction the system uses to arrange minimized windows.
    /// </devdoc>
    [ComVisible(true)]
    [Flags]
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification =  "Maps to native enum")]
    public enum ArrangeDirection
    {
        /// <summary>
        /// Arranges vertically, from top to bottom.
        /// </devdoc>
        Down = NativeMethods.ARW_DOWN,

        /// <summary>
        /// Arranges horizontally, from left to right.
        /// </devdoc>
        Left = NativeMethods.ARW_LEFT,

        /// <summary>
        /// Arranges horizontally, from right to left.
        /// </devdoc>
        Right = NativeMethods.ARW_RIGHT,

        /// <summary>
        /// Arranges vertically, from bottom to top.
        /// </devdoc>
        Up = NativeMethods.ARW_UP,
    }
}
