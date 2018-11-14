// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\StatusBarPanelStyle.uex' path='docs/doc[@for="StatusBarPanelStyle"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies whether a panel on
    ///       a status bar is owner drawn or system drawn.
    ///    </para>
    /// </devdoc>
    public enum StatusBarPanelStyle {

        /// <include file='doc\StatusBarPanelStyle.uex' path='docs/doc[@for="StatusBarPanelStyle.Text"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The panel is
        ///       drawn by the system.
        ///    </para>
        /// </devdoc>
        Text        = 1,

        /// <include file='doc\StatusBarPanelStyle.uex' path='docs/doc[@for="StatusBarPanelStyle.OwnerDraw"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The panel is
        ///       drawn by the owner.
        ///    </para>
        /// </devdoc>
        OwnerDraw   = 2,

    }
}
