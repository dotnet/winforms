// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <devdoc>
    ///    <para>
    ///       Specifies the type of action used to raise the <see cref='System.Windows.Forms.ScrollBar.Scroll'/> event.
    ///       
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum ScrollEventType {

        /// <devdoc>
        ///    <para>
        ///       The
        ///       scroll box was
        ///       moved a small
        ///       distance. The user clicked the left(horizontal) or top(vertical) scroll arrow or pressed the UP ARROW
        ///       key.
        ///       
        ///    </para>
        /// </devdoc>
        SmallDecrement = NativeMethods.SB_LINELEFT,

        /// <devdoc>
        ///    <para>
        ///       The
        ///       scroll box was
        ///       moved a small distance. The user clicked the right(horizontal) or bottom(vertical) scroll arrow or
        ///       pressed the DOWN ARROW key.
        ///       
        ///    </para>
        /// </devdoc>
        SmallIncrement = NativeMethods.SB_LINERIGHT,

        /// <devdoc>
        ///    <para>
        ///       The scroll box
        ///       moved a large distance. The user clicked the scroll bar to the left(horizontal) or above(vertical)
        ///       the scroll box, or pressed the PAGE UP key.
        ///       
        ///    </para>
        /// </devdoc>
        LargeDecrement = NativeMethods.SB_PAGELEFT,

        /// <devdoc>
        ///    <para>
        ///       The scroll box moved a large distance. The user clicked the scroll bar to
        ///       the right(horizontal) or below(vertical) the scroll box, or pressed the PAGE DOWN key.
        ///       
        ///    </para>
        /// </devdoc>
        LargeIncrement = NativeMethods.SB_PAGERIGHT,

        /// <devdoc>
        ///    <para>
        ///       The scroll box was moved.
        ///       
        ///    </para>
        /// </devdoc>
        ThumbPosition = NativeMethods.SB_THUMBPOSITION,

        /// <devdoc>
        ///    <para>
        ///       The scroll box
        ///       is currently being moved.
        ///       
        ///    </para>
        /// </devdoc>
        ThumbTrack = NativeMethods.SB_THUMBTRACK,

        /// <devdoc>
        ///    <para>
        ///       The
        ///       scroll box was moved to the <see cref='System.Windows.Forms.ScrollBar.Minimum'/>
        ///       position.
        ///       
        ///    </para>
        /// </devdoc>
        First = NativeMethods.SB_LEFT,

        /// <devdoc>
        ///    <para>
        ///       The
        ///       scroll box was moved to the <see cref='System.Windows.Forms.ScrollBar.Maximum'/>
        ///       position.
        ///       
        ///    </para>
        /// </devdoc>
        Last = NativeMethods.SB_RIGHT,

        /// <devdoc>
        ///    <para>
        ///       The scroll box has stopped moving.
        ///       
        ///    </para>
        /// </devdoc>
        EndScroll = NativeMethods.SB_ENDSCROLL,

    }
}
