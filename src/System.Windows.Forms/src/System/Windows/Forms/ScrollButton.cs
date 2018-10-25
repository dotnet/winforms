// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;



    /// <include file='doc\ScrollButton.uex' path='docs/doc[@for="ScrollButton"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the type of
    ///       scroll arrow to create on a scroll bar.
    ///       
    ///    </para>
    /// </devdoc>
    public enum ScrollButton {

        /// <include file='doc\ScrollButton.uex' path='docs/doc[@for="ScrollButton.Down"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A down-scroll arrow.
        ///       
        ///    </para>
        /// </devdoc>
        Down = NativeMethods.DFCS_SCROLLDOWN,

        /// <include file='doc\ScrollButton.uex' path='docs/doc[@for="ScrollButton.Left"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A left-scroll arrow.
        ///       
        ///    </para>
        /// </devdoc>
        Left = NativeMethods.DFCS_SCROLLLEFT,

        /// <include file='doc\ScrollButton.uex' path='docs/doc[@for="ScrollButton.Right"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A right-scroll arrow.
        ///       
        ///    </para>
        /// </devdoc>
        Right = NativeMethods.DFCS_SCROLLRIGHT,

        /// <include file='doc\ScrollButton.uex' path='docs/doc[@for="ScrollButton.Up"]/*' />
        /// <devdoc>
        ///    <para>
        ///       An up-scroll arrow.
        ///       
        ///    </para>
        /// </devdoc>
        Up = NativeMethods.DFCS_SCROLLUP,

        /// <include file='doc\ScrollButton.uex' path='docs/doc[@for="ScrollButton.Min"]/*' />
        /// <devdoc>
        /// </devdoc>
        Min = NativeMethods.DFCS_SCROLLUP,
        
        /// <include file='doc\ScrollButton.uex' path='docs/doc[@for="ScrollButton.Max"]/*' />
        Max = NativeMethods.DFCS_SCROLLRIGHT,

    }
}
