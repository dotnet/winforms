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
    ///       Specifies the type of
    ///       scroll arrow to create on a scroll bar.
    ///       
    ///    </para>
    /// </devdoc>
    public enum ScrollButton {

        /// <devdoc>
        ///    <para>
        ///       A down-scroll arrow.
        ///       
        ///    </para>
        /// </devdoc>
        Down = NativeMethods.DFCS_SCROLLDOWN,

        /// <devdoc>
        ///    <para>
        ///       A left-scroll arrow.
        ///       
        ///    </para>
        /// </devdoc>
        Left = NativeMethods.DFCS_SCROLLLEFT,

        /// <devdoc>
        ///    <para>
        ///       A right-scroll arrow.
        ///       
        ///    </para>
        /// </devdoc>
        Right = NativeMethods.DFCS_SCROLLRIGHT,

        /// <devdoc>
        ///    <para>
        ///       An up-scroll arrow.
        ///       
        ///    </para>
        /// </devdoc>
        Up = NativeMethods.DFCS_SCROLLUP,

        /// <devdoc>
        /// </devdoc>
        Min = NativeMethods.DFCS_SCROLLUP,
        
        Max = NativeMethods.DFCS_SCROLLRIGHT,

    }
}
