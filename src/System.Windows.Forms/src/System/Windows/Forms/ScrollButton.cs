// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ////  Specifies the type of scroll arrow to create on a scroll bar.
    ////
    /// </summary>
    public enum ScrollButton
    {
        /// <summary>
        ////  A down-scroll arrow.
        /// </summary>
        Down = NativeMethods.DFCS_SCROLLDOWN,

        /// <summary>
        ////  A left-scroll arrow.
        /// </summary>
        Left = NativeMethods.DFCS_SCROLLLEFT,

        /// <summary>
        ////  A right-scroll arrow.
        /// </summary>
        Right = NativeMethods.DFCS_SCROLLRIGHT,

        /// <summary>
        ////  An up-scroll arrow.
        /// </summary>
        Up = NativeMethods.DFCS_SCROLLUP,

        Min = NativeMethods.DFCS_SCROLLUP,

        Max = NativeMethods.DFCS_SCROLLRIGHT,
    }
}
