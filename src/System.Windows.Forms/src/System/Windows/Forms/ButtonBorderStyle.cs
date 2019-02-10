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
    ///       the border style for a button control.
    ///    </para>
    /// </devdoc>
    public enum ButtonBorderStyle {
        /// <devdoc>
        ///    <para>
        ///       No border.
        ///    </para>
        /// </devdoc>
        None,
        /// <devdoc>
        ///    <para>
        ///       A dotted-line border.
        ///    </para>
        /// </devdoc>
        Dotted,
        /// <devdoc>
        ///    <para>
        ///       A dashed border.
        ///    </para>
        /// </devdoc>
        Dashed,
        /// <devdoc>
        ///    <para>
        ///       A solid border.
        ///    </para>
        /// </devdoc>
        Solid,
        /// <devdoc>
        ///    <para>
        ///       A sunken border.
        ///    </para>
        /// </devdoc>
        Inset,
        /// <devdoc>
        ///    <para>
        ///       A raised border.
        ///    </para>
        /// </devdoc>
        Outset,
    }
}
