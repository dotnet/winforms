// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the type of action used to raise the <see cref='ScrollBar.Scroll'/> event.
    /// </summary>
    [ComVisible(true)]
    public enum ScrollEventType
    {
        /// <summary>
        ///  The scroll box was moved a small distance. The user clicked the
        ///  left (horizontal) or top (vertical) scroll arrow or pressed
        ///  the UP ARROW
        /// </summary>
        SmallDecrement = NativeMethods.SB_LINELEFT,

        /// <summary>
        ///  The scroll box was moved a small distance. The user clicked the
        ///  right (horizontal) or bottom (vertical) scroll arrow or pressed
        ///  the DOWN ARROW key.
        /// </summary>
        SmallIncrement = NativeMethods.SB_LINERIGHT,

        /// <summary>
        ///  The scroll box moved a large distance. The user clicked the scroll bar
        ///  to the left (horizontal) or above (vertical) the scroll box, or pressed
        ///  the PAGE UP key.
        /// </summary>
        LargeDecrement = NativeMethods.SB_PAGELEFT,

        /// <summary>
        ///  The scroll box moved a large distance. The user clicked the scroll bar
        ///  to the right (horizontal) or below (vertical) the scroll box, or pressed
        ///  the PAGE DOWN key.
        /// </summary>
        LargeIncrement = NativeMethods.SB_PAGERIGHT,

        /// <summary>
        ///  The scroll box was moved.
        /// </summary>
        ThumbPosition = NativeMethods.SB_THUMBPOSITION,

        /// <summary>
        ///  The scroll box is currently being moved.
        /// </summary>
        ThumbTrack = NativeMethods.SB_THUMBTRACK,

        /// <summary>
        ///  The scroll box was moved to the <see cref='ScrollBar.Minimum'/>
        ///  position.
        /// </summary>
        First = NativeMethods.SB_LEFT,

        /// <summary>
        ///  The scroll box was moved to the <see cref='ScrollBar.Maximum'/>
        ///  position.
        /// </summary>
        Last = NativeMethods.SB_RIGHT,

        /// <summary>
        ///  The scroll box has stopped moving.
        /// </summary>
        EndScroll = NativeMethods.SB_ENDSCROLL
    }
}
