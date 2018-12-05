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
    ///       Specifies
    ///       the appearance of a control.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum Appearance {

        /// <devdoc>
        ///    <para>
        ///       The default appearance defined by the control
        ///       class.
        ///    </para>
        /// </devdoc>
        Normal              = 0,

        /// <devdoc>
        ///    <para>
        ///       The appearance of a Windows
        ///       button.
        ///    </para>
        /// </devdoc>
        Button              = 1,

    }
}
