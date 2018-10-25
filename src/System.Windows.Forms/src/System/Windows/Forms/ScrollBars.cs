// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\ScrollBars.uex' path='docs/doc[@for="ScrollBars"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies
    ///       which scroll bars will be visible on a control.
    ///       
    ///    </para>
    /// </devdoc>
    public enum ScrollBars {

        /// <include file='doc\ScrollBars.uex' path='docs/doc[@for="ScrollBars.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No scroll bars are shown.
        ///       
        ///    </para>
        /// </devdoc>
        None       = 0,

        /// <include file='doc\ScrollBars.uex' path='docs/doc[@for="ScrollBars.Horizontal"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Only horizontal scroll bars are shown.
        ///       
        ///    </para>
        /// </devdoc>
        Horizontal = 1,

        /// <include file='doc\ScrollBars.uex' path='docs/doc[@for="ScrollBars.Vertical"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Only vertical scroll bars are shown.
        ///       
        ///    </para>
        /// </devdoc>
        Vertical   = 2,

        /// <include file='doc\ScrollBars.uex' path='docs/doc[@for="ScrollBars.Both"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Both horizontal and vertical scroll bars are shown.
        ///       
        ///    </para>
        /// </devdoc>
        Both       = 3,

    }
}
