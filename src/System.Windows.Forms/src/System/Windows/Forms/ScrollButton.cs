// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the type of scroll arrow to create on a scroll bar.
    /// </summary>
    public enum ScrollButton
    {
        /// <summary>
        ///  A down-scroll arrow.
        /// </summary>
        Down = (int)User32.DFCS.SCROLLDOWN,

        /// <summary>
        ///  A left-scroll arrow.
        /// </summary>
        Left = (int)User32.DFCS.SCROLLLEFT,

        /// <summary>
        ///  A right-scroll arrow.
        /// </summary>
        Right = (int)User32.DFCS.SCROLLRIGHT,

        /// <summary>
        ///  An up-scroll arrow.
        /// </summary>
        Up = (int)User32.DFCS.SCROLLUP,

        Min = (int)User32.DFCS.SCROLLUP,

        Max = (int)User32.DFCS.SCROLLRIGHT,
    }
}
