// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Windows.Forms.Design;
    using Microsoft.Win32;


    /// <include file='doc\AnchorStyles.uex' path='docs/doc[@for="AnchorStyles"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies how
    ///       a
    ///       control anchors to the edges of its container.
    ///    </para>
    /// </devdoc>
    [
        Editor("System.Windows.Forms.Design.AnchorEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
    ]
    [Flags]
    public enum AnchorStyles {

        /// <include file='doc\AnchorStyles.uex' path='docs/doc[@for="AnchorStyles.Top"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control is anchored to the top edge of its container.
        ///    </para>
        /// </devdoc>
        Top         = 0x01,
        /// <include file='doc\AnchorStyles.uex' path='docs/doc[@for="AnchorStyles.Bottom"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control is anchored to the bottom edge of its container.
        ///    </para>
        /// </devdoc>
        Bottom      = 0x02,
        /// <include file='doc\AnchorStyles.uex' path='docs/doc[@for="AnchorStyles.Left"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control is anchored to the left edge of its container.
        ///    </para>
        /// </devdoc>
        Left        = 0x04,
        /// <include file='doc\AnchorStyles.uex' path='docs/doc[@for="AnchorStyles.Right"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control is anchored to the right edge of its container.
        ///    </para>
        /// </devdoc>
        Right       = 0x08,

        /// <include file='doc\AnchorStyles.uex' path='docs/doc[@for="AnchorStyles.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control is not anchored to any edges of its container.
        ///    </para>
        /// </devdoc>
        None            = 0,
    }
}
