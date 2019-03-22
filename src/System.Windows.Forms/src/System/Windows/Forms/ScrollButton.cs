// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    //// Specifies the type of scroll arrow to create on a scroll bar.
    ////
    /// </devdoc>
    public enum ScrollButton
    {
        /// <devdoc>
        //// A down-scroll arrow.
        /// </devdoc>
        Down = NativeMethods.DFCS_SCROLLDOWN,

        /// <devdoc>
        //// A left-scroll arrow.
        /// </devdoc>
        Left = NativeMethods.DFCS_SCROLLLEFT,

        /// <devdoc>
        //// A right-scroll arrow.
        /// </devdoc>
        Right = NativeMethods.DFCS_SCROLLRIGHT,

        /// <devdoc>
        //// An up-scroll arrow.
        /// </devdoc>
        Up = NativeMethods.DFCS_SCROLLUP,

        Min = NativeMethods.DFCS_SCROLLUP,

        Max = NativeMethods.DFCS_SCROLLRIGHT,
    }
}
