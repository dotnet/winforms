// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the location of tick marks in a <see cref='System.Windows.Forms.TrackBar'/>
    /// control.
    /// </devdoc>
    public enum TickStyle
    {
        /// <devdoc>
        /// No tick marks appear in the control.
        /// </devdoc>
        None = 0,

        /// <devdoc>
        /// Tick marks are located on the top of horizontal control or on the
        /// left of a vertical control.
        /// </devdoc>
        TopLeft = 1,

        /// <devdoc>
        /// Tick marks are located on the bottom of a horizontal control or on the
        /// right side of a vertical control.
        /// </devdoc>
        BottomRight = 2,

        /// <devdoc>
        /// Tick marks are located on both sides of the control.
        /// </devdoc>
        Both = 3,
    }
}
