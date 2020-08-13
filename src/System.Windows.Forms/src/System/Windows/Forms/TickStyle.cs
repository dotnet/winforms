// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the location of tick marks in a <see cref='TrackBar'/>
    ///  control.
    /// </summary>
    public enum TickStyle
    {
        /// <summary>
        ///  No tick marks appear in the control.
        /// </summary>
        None = 0,

        /// <summary>
        ///  Tick marks are located on the top of horizontal control or on the
        ///  left of a vertical control.
        /// </summary>
        TopLeft = 1,

        /// <summary>
        ///  Tick marks are located on the bottom of a horizontal control or on the
        ///  right side of a vertical control.
        /// </summary>
        BottomRight = 2,

        /// <summary>
        ///  Tick marks are located on both sides of the control.
        /// </summary>
        Both = 3,
    }
}
