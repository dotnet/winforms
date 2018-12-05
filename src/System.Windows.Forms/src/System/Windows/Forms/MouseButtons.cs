// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <devdoc>
    ///    <para>
    ///       Specifies constants that define which mouse button was pressed.
    ///    </para>
    /// </devdoc>
    [Flags]
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum MouseButtons {

        /// <devdoc>
        ///    <para>
        ///       
        ///       The left mouse button was pressed.
        ///       
        ///    </para>
        /// </devdoc>
        Left = 0x00100000,

        /// <devdoc>
        ///    <para>
        ///       
        ///       No mouse button was pressed.
        ///       
        ///    </para>
        /// </devdoc>
        None = 0x00000000,

        /// <devdoc>
        ///    <para>
        ///       
        ///       The right mouse button was pressed.
        ///       
        ///    </para>
        /// </devdoc>
        Right = 0x00200000,

        /// <devdoc>
        ///    <para>
        ///       
        ///       The middle mouse button was pressed.
        ///       
        ///    </para>        
        /// </devdoc>
        Middle = 0x00400000,        
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        XButton1 = 0x00800000,        
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        XButton2 = 0x01000000,
    }
}
