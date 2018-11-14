// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\LeftRightAlignment.uex' path='docs/doc[@for="LeftRightAlignment"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies whether an object or text is aligned to
    ///       the left or
    ///       right of a reference point.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum LeftRightAlignment {

        /// <include file='doc\LeftRightAlignment.uex' path='docs/doc[@for="LeftRightAlignment.Left"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The object or
        ///       text is aligned to the left of the reference
        ///       point.
        ///    </para>
        /// </devdoc>
        Left = 0,

        /// <include file='doc\LeftRightAlignment.uex' path='docs/doc[@for="LeftRightAlignment.Right"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The object or text is aligned to the right of the reference point.
        ///    </para>
        /// </devdoc>
        Right = 1,

    }
}
