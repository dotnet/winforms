// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\StatusBarPanelBorderStyle.uex' path='docs/doc[@for="StatusBarPanelBorderStyle"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the border style of a panel on the <see cref='System.Windows.Forms.StatusBar'/>.
    ///    </para>
    /// </devdoc>
    public enum StatusBarPanelBorderStyle {
        /// <include file='doc\StatusBarPanelBorderStyle.uex' path='docs/doc[@for="StatusBarPanelBorderStyle.None"]/*' />
        /// <devdoc>
        ///     No border.
        /// </devdoc>
        None        = 1,

        /// <include file='doc\StatusBarPanelBorderStyle.uex' path='docs/doc[@for="StatusBarPanelBorderStyle.Raised"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A raised border.
        ///    </para>
        /// </devdoc>
        Raised      = 2,

        /// <include file='doc\StatusBarPanelBorderStyle.uex' path='docs/doc[@for="StatusBarPanelBorderStyle.Sunken"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A sunken border.
        ///    </para>
        /// </devdoc>
        Sunken      = 3,

    }
}
