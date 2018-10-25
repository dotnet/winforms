// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using Microsoft.Win32;
    using System.Drawing;

    /// <include file='doc\FormBorderStyle.uex' path='docs/doc[@for="FormBorderStyle"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the border styles for a form.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum FormBorderStyle {

        /// <include file='doc\FormBorderStyle.uex' path='docs/doc[@for="FormBorderStyle.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       No border.
        ///
        ///    </para>
        /// </devdoc>
        None = 0,

        /// <include file='doc\FormBorderStyle.uex' path='docs/doc[@for="FormBorderStyle.FixedSingle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A fixed, single line border.
        ///
        ///    </para>
        /// </devdoc>
        FixedSingle = 1,

        /// <include file='doc\FormBorderStyle.uex' path='docs/doc[@for="FormBorderStyle.Fixed3D"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A fixed, three-dimensional border.
        ///       
        ///    </para>
        /// </devdoc>
        Fixed3D = 2,

        /// <include file='doc\FormBorderStyle.uex' path='docs/doc[@for="FormBorderStyle.FixedDialog"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A thick, fixed dialog-style border.
        ///
        ///    </para>
        /// </devdoc>
        FixedDialog = 3,

        /// <include file='doc\FormBorderStyle.uex' path='docs/doc[@for="FormBorderStyle.Sizable"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A resizable border.
        ///       
        ///    </para>
        /// </devdoc>
        Sizable = 4,

        /// <include file='doc\FormBorderStyle.uex' path='docs/doc[@for="FormBorderStyle.FixedToolWindow"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A tool window border
        ///       that is not resizable.
        ///    </para>
        /// </devdoc>
        FixedToolWindow = 5,

        /// <include file='doc\FormBorderStyle.uex' path='docs/doc[@for="FormBorderStyle.SizableToolWindow"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A resizable tool window border.
        ///       
        ///    </para>
        /// </devdoc>
        SizableToolWindow = 6,

    }
}
