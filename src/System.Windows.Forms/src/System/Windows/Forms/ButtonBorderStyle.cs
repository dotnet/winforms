// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\ButtonBorderStyle.uex' path='docs/doc[@for="ButtonBorderStyle"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies
    ///       the border style for a button control.
    ///    </para>
    /// </devdoc>
    public enum ButtonBorderStyle {
        /// <include file='doc\ButtonBorderStyle.uex' path='docs/doc[@for="ButtonBorderStyle.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No border.
        ///    </para>
        /// </devdoc>
        None,
        /// <include file='doc\ButtonBorderStyle.uex' path='docs/doc[@for="ButtonBorderStyle.Dotted"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A dotted-line border.
        ///    </para>
        /// </devdoc>
        Dotted,
        /// <include file='doc\ButtonBorderStyle.uex' path='docs/doc[@for="ButtonBorderStyle.Dashed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A dashed border.
        ///    </para>
        /// </devdoc>
        Dashed,
        /// <include file='doc\ButtonBorderStyle.uex' path='docs/doc[@for="ButtonBorderStyle.Solid"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A solid border.
        ///    </para>
        /// </devdoc>
        Solid,
        /// <include file='doc\ButtonBorderStyle.uex' path='docs/doc[@for="ButtonBorderStyle.Inset"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A sunken border.
        ///    </para>
        /// </devdoc>
        Inset,
        /// <include file='doc\ButtonBorderStyle.uex' path='docs/doc[@for="ButtonBorderStyle.Outset"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A raised border.
        ///    </para>
        /// </devdoc>
        Outset,
    }
}
