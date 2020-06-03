// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the direction the system uses to arrange minimized windows.
    /// </summary>
    [Flags]
    public enum ArrangeDirection
    {
        /// <summary>
        ///  Arranges vertically, from top to bottom.
        /// </summary>
        Down = User32.ARW.DOWN,

        /// <summary>
        ///  Arranges horizontally, from left to right.
        /// </summary>
        Left = User32.ARW.LEFT,

        /// <summary>
        ///  Arranges horizontally, from right to left.
        /// </summary>
        Right = User32.ARW.RIGHT,

        /// <summary>
        ///  Arranges vertically, from bottom to top.
        /// </summary>
        Up = User32.ARW.UP,
    }
}
