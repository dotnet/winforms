// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\TabAlignment.uex' path='docs/doc[@for="TabAlignment"]/*' />
    /// <devdoc>
    ///     Controls where the tabs will be located in a Tab Control.
    /// </devdoc>
    public enum TabAlignment {

        /// <include file='doc\TabAlignment.uex' path='docs/doc[@for="TabAlignment.Top"]/*' />
        /// <devdoc>
        ///     Tabs will be located across the top of the control.
        /// </devdoc>
        Top = 0,

        /// <include file='doc\TabAlignment.uex' path='docs/doc[@for="TabAlignment.Bottom"]/*' />
        /// <devdoc>
        ///     Tabs will be located across the bottom of the control.
        /// </devdoc>
        Bottom = 1,

        /// <include file='doc\TabAlignment.uex' path='docs/doc[@for="TabAlignment.Left"]/*' />
        /// <devdoc>
        ///     Tabs will be located along the left edge of the control.
        /// </devdoc>
        Left = 2,

        /// <include file='doc\TabAlignment.uex' path='docs/doc[@for="TabAlignment.Right"]/*' />
        /// <devdoc>
        ///     Tabs will be located along the right edge of the control.
        /// </devdoc>
        Right = 3,

    }
}
