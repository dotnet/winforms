// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies constants that define which mouse button was pressed.
    /// </summary>
    [Flags]
    public enum MouseButtons
    {
        /// <summary>
        ///  The left mouse button was pressed.
        /// </summary>
        Left = 0x00100000,

        /// <summary>
        ///  No mouse button was pressed.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        ///  The right mouse button was pressed.
        /// </summary>
        Right = 0x00200000,

        /// <summary>
        ///  The middle mouse button was pressed.
        /// </summary>
        Middle = 0x00400000,

        XButton1 = 0x00800000,

        XButton2 = 0x01000000,
    }
}
