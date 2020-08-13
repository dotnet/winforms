// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the appearance of a button.
    /// </summary>
    [Flags]
    public enum ButtonState
    {
        /// <summary>
        ///  The button has a checked or latched appearance. Use this appearance to
        ///  show that a toggle button has been pressed.
        /// </summary>
        Checked = (int)User32.DFCS.CHECKED,

        /// <summary>
        ///  The button has a flat, two-dimensional appearance.
        /// </summary>
        Flat = (int)User32.DFCS.FLAT,

        /// <summary>
        ///  The button is inactive (grayed).
        /// </summary>
        Inactive = (int)User32.DFCS.INACTIVE,

        /// <summary>
        ///  The button has its normal appearance (three-dimensional and not pressed).
        /// </summary>
        Normal = 0,

        /// <summary>
        ///  The button is currently pressed.
        /// </summary>
        Pushed = (int)User32.DFCS.PUSHED,

        /// <summary>
        ///  All viable flags in the bit mask are used.
        /// </summary>
        All = Flat | Checked | Pushed | Inactive,
    }
}
