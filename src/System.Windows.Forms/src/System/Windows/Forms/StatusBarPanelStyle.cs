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
    ///       Specifies whether a panel on
    ///       a status bar is owner drawn or system drawn.
    ///    </para>
    /// </devdoc>
    public enum StatusBarPanelStyle {


        /// <devdoc>
        ///    <para>
        ///       The panel is
        ///       drawn by the system.
        ///    </para>
        /// </devdoc>
        Text        = 1,


        /// <devdoc>
        ///    <para>
        ///       The panel is
        ///       drawn by the owner.
        ///    </para>
        /// </devdoc>
        OwnerDraw   = 2,

    }
}
