// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;



    /// <include file='doc\TabSizeMode.uex' path='docs/doc[@for="TabSizeMode"]/*' />
    /// <devdoc>
    ///     Controls the automatic sizing of certain objects.  This is typically
    ///     used for the sizing of Tabs in a TabStrip control.
    /// </devdoc>
    public enum TabSizeMode {

        /// <include file='doc\TabSizeMode.uex' path='docs/doc[@for="TabSizeMode.Normal"]/*' />
        /// <devdoc>
        ///     Indicates that items are only as wide as they need to be to display
        ///     their information.  Empty space on the right is left as such
        /// </devdoc>
        Normal = 0,

        /// <include file='doc\TabSizeMode.uex' path='docs/doc[@for="TabSizeMode.FillToRight"]/*' />
        /// <devdoc>
        ///     indicates that the tags are stretched to ensure they reach the far
        ///     right of the strip, if necesary.  This is only applicable to tab
        ///     strips with more than one row.
        /// </devdoc>
        FillToRight = 1,

        /// <include file='doc\TabSizeMode.uex' path='docs/doc[@for="TabSizeMode.Fixed"]/*' />
        /// <devdoc>
        ///     Indicates that all tabs are the same width. period.
        /// </devdoc>
        Fixed = 2,

    }
}
