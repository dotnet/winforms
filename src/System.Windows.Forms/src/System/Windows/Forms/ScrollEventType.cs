// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the type of action used to raise the <see cref='System.Windows.Forms.ScrollBar.Scroll'/> event.
    /// </devdoc>
    [ComVisible(true)]
    public enum ScrollEventType
    {
        /// <summary>
        /// The scroll box was moved a small distance. The user clicked the
        /// left (horizontal) or top (vertical) scroll arrow or pressed
        /// the UP ARROW
        /// </devdoc>
        SmallDecrement = NativeMethods.SB_LINELEFT,

        /// <summary>
        /// The scroll box was moved a small distance. The user clicked the
        /// right (horizontal) or bottom (vertical) scroll arrow or pressed
        /// the DOWN ARROW key.
        /// </devdoc>
        SmallIncrement = NativeMethods.SB_LINERIGHT,

        /// <summary>
        /// The scroll box moved a large distance. The user clicked the scroll bar
        /// to the left (horizontal) or above (vertical) the scroll box, or pressed
        /// the PAGE UP key.
        /// </devdoc>
        LargeDecrement = NativeMethods.SB_PAGELEFT,

        /// <summary>
        /// The scroll box moved a large distance. The user clicked the scroll bar
        /// to the right (horizontal) or below (vertical) the scroll box, or pressed
        /// the PAGE DOWN key.
        /// </devdoc>
        LargeIncrement = NativeMethods.SB_PAGERIGHT,

        /// <summary>
        ///    <para>
        /// The scroll box was moved.
        /// </devdoc>
        ThumbPosition = NativeMethods.SB_THUMBPOSITION,

        /// <summary>
        /// The scroll box is currently being moved.
        /// </devdoc>
        ThumbTrack = NativeMethods.SB_THUMBTRACK,

        /// <summary>
        /// The scroll box was moved to the <see cref='System.Windows.Forms.ScrollBar.Minimum'/>
        /// position.
        /// </devdoc>
        First = NativeMethods.SB_LEFT,

        /// <summary>
        /// The scroll box was moved to the <see cref='System.Windows.Forms.ScrollBar.Maximum'/>
        /// position.
        /// </devdoc>
        Last = NativeMethods.SB_RIGHT,

        /// <summary>
        /// The scroll box has stopped moving.
        /// </devdoc>
        EndScroll = NativeMethods.SB_ENDSCROLL
    }
}
