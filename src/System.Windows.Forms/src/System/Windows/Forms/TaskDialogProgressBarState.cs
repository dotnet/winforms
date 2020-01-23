// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///   Contains constants that specify the state of a task dialog progress bar.
    /// </summary>
    public enum TaskDialogProgressBarState : int
    {
        /// <summary>
        ///   Shows a regular progress bar.
        /// </summary>
        Normal = 0,

        /// <summary>
        ///   Shows a paused (yellow) progress bar.
        /// </summary>
        Paused = 1,

        /// <summary>
        ///   Shows an error (red) progress bar.
        /// </summary>
        Error = 2,

        /// <summary>
        ///   Shows a marquee progress bar.
        /// </summary>
        Marquee = 3,

        /// <summary>
        ///   Shows a marquee progress bar where the marquee animation is paused.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   If you switch from <see cref="Marquee"/> to <see cref="MarqueePaused"/> while the
        ///   dialog is shown, the  marquee animation will stop.
        /// </para>
        /// </remarks>
        MarqueePaused = 4,

        /// <summary>
        ///   The progress bar will not be displayed.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   Note: While the dialog is showing, you cannot switch from
        ///   <see cref="None"/> to any other state, and vice versa.
        /// </para>
        /// </remarks>
        None = 5
    }
}
