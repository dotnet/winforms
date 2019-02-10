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
    ///       Specifies the
    ///       location of tick marks in a <see cref='System.Windows.Forms.TrackBar'/>
    ///       control.
    ///    </para>
    /// </devdoc>
    public enum TickStyle {


        /// <devdoc>
        ///    <para>
        ///       No tick marks appear in the control.
        ///    </para>
        /// </devdoc>
        None = 0,


        /// <devdoc>
        ///    <para>
        ///       Tick
        ///       marks are located on the top of horizontal control or on the left of a vertical control.
        ///    </para>
        /// </devdoc>
        TopLeft = 1,


        /// <devdoc>
        ///    <para>
        ///       Tick marks are
        ///       located on the bottom of a horizontal control or on the right side of a vertical control.
        ///    </para>
        /// </devdoc>
        BottomRight = 2,


        /// <devdoc>
        ///    <para>
        ///       Tick
        ///       marks are located on both sides of the control.
        ///    </para>
        /// </devdoc>
        Both = 3,

    }
}
