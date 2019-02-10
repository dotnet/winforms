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
    ///       Specifies the Fixed Panel in the SplitContainer Control.
    ///       
    ///    </para>
    /// </devdoc>
    public enum FixedPanel {

        /// <devdoc>
        ///    <para>
        ///       No panel is fixed. Resize causes the Resize of both the panels.
        ///    </para>
        /// </devdoc>
        None = 0,


        /// <devdoc>
        ///    <para>
        ///       Panel1 is Fixed. The resize will increase the size of second panel.
        ///    </para>
        /// </devdoc>
        Panel1 = 1,


        /// <devdoc>
        ///    <para>
        ///       Panel2 is Fixed. The resize will increase the size of first panel.
        ///    </para>
        /// </devdoc>
        Panel2 = 2,

    }
}
