// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the type of action used to raise the <see cref='ScrollBar.Scroll'/> event.
    /// </summary>
    public enum ScrollEventType
    {
        /// <summary>
        ///  The scroll box was moved a small distance. The user clicked the
        ///  left (horizontal) or top (vertical) scroll arrow or pressed
        ///  the UP ARROW
        /// </summary>
        SmallDecrement = User32.SBH.LINELEFT,

        /// <summary>
        ///  The scroll box was moved a small distance. The user clicked the
        ///  right (horizontal) or bottom (vertical) scroll arrow or pressed
        ///  the DOWN ARROW key.
        /// </summary>
        SmallIncrement = User32.SBH.LINERIGHT,

        /// <summary>
        ///  The scroll box moved a large distance. The user clicked the scroll bar
        ///  to the left (horizontal) or above (vertical) the scroll box, or pressed
        ///  the PAGE UP key.
        /// </summary>
        LargeDecrement = User32.SBH.PAGELEFT,

        /// <summary>
        ///  The scroll box moved a large distance. The user clicked the scroll bar
        ///  to the right (horizontal) or below (vertical) the scroll box, or pressed
        ///  the PAGE DOWN key.
        /// </summary>
        LargeIncrement = User32.SBH.PAGERIGHT,

        /// <summary>
        ///  The scroll box was moved.
        /// </summary>
        ThumbPosition = User32.SBH.THUMBPOSITION,

        /// <summary>
        ///  The scroll box is currently being moved.
        /// </summary>
        ThumbTrack = User32.SBH.THUMBTRACK,

        /// <summary>
        ///  The scroll box was moved to the <see cref='ScrollBar.Minimum'/>
        ///  position.
        /// </summary>
        First = User32.SBH.LEFT,

        /// <summary>
        ///  The scroll box was moved to the <see cref='ScrollBar.Maximum'/>
        ///  position.
        /// </summary>
        Last = User32.SBH.RIGHT,

        /// <summary>
        ///  The scroll box has stopped moving.
        /// </summary>
        EndScroll = User32.SBH.ENDSCROLL
    }
}
