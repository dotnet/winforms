// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies constants that define which mouse button was pressed.
    /// </devdoc>
    [Flags]
    [ComVisible(true)]
    public enum MouseButtons
    {
        /// <devdoc>
        /// The left mouse button was pressed.
        /// </devdoc>
        Left = 0x00100000,

        /// <devdoc>
        /// No mouse button was pressed.
        /// </devdoc>
        None = 0x00000000,

        /// <devdoc>
        /// The right mouse button was pressed.
        /// </devdoc>
        Right = 0x00200000,

        /// <devdoc>
        /// The middle mouse button was pressed.
        /// </devdoc>
        Middle = 0x00400000,        
        
        XButton1 = 0x00800000,        
        
        XButton2 = 0x01000000,
    }
}
