// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\BorderStyle.uex' path='docs/doc[@for="BorderStyle"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the
    ///       border style for a control or form.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum BorderStyle {

        /// <include file='doc\BorderStyle.uex' path='docs/doc[@for="BorderStyle.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No border.
        ///    </para>
        /// </devdoc>
        None = 0,

        /// <include file='doc\BorderStyle.uex' path='docs/doc[@for="BorderStyle.FixedSingle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A single-line border.
        ///    </para>
        /// </devdoc>
        FixedSingle = 1,

        /// <include file='doc\BorderStyle.uex' path='docs/doc[@for="BorderStyle.Fixed3D"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A three-dimensional border.
        ///    </para>
        /// </devdoc>
        Fixed3D = 2,

    }
}
