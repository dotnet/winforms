// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the direction the system uses to arrange minimized windows.
    /// </summary>
    [ComVisible(true)]
    [Flags]
    public enum ArrangeDirection
    {
        /// <summary>
        ///  Arranges vertically, from top to bottom.
        /// </summary>
        Down = NativeMethods.ARW_DOWN,

        /// <summary>
        ///  Arranges horizontally, from left to right.
        /// </summary>
        Left = NativeMethods.ARW_LEFT,

        /// <summary>
        ///  Arranges horizontally, from right to left.
        /// </summary>
        Right = NativeMethods.ARW_RIGHT,

        /// <summary>
        ///  Arranges vertically, from bottom to top.
        /// </summary>
        Up = NativeMethods.ARW_UP,
    }
}
