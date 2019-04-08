// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the appearance of a button.
    /// </devdoc>
    [Flags]
    public enum ButtonState
    {
        /// <devdoc>
        /// The button has a checked or latched appearance. Use this appearance to
        /// show that a toggle button has been pressed.
        /// </devdoc>
        Checked = NativeMethods.DFCS_CHECKED,

        /// <devdoc>
        /// The button has a flat, two-dimensional appearance.
        /// </devdoc>
        Flat = NativeMethods.DFCS_FLAT,

        /// <devdoc>
        /// The button is inactive (grayed).
        /// </devdoc>
        Inactive = NativeMethods.DFCS_INACTIVE,

        /// <devdoc>
        /// The button has its normal appearance (three-dimensional and not pressed).
        /// </devdoc>
        Normal = 0,

        /// <devdoc>
        /// The button is currently pressed.
        /// </devdoc>
        Pushed = NativeMethods.DFCS_PUSHED,

        /// <devdoc>
        /// All viable flags in the bit mask are used.
        /// </devdoc>
        All = Flat | Checked | Pushed | Inactive,
    }
}
