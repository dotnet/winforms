// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;



    /// <include file='doc\TabDrawMode.uex' path='docs/doc[@for="TabDrawMode"]/*' />
    /// <devdoc>
    ///     The TabStrip and TabControl both support ownerdraw functionality, but
    ///     only one type, in which you can paint the tabs individually.  This
    ///     enumeration contains the valid values for it's drawMode property.
    /// </devdoc>
    public enum TabDrawMode {

        /// <include file='doc\TabDrawMode.uex' path='docs/doc[@for="TabDrawMode.Normal"]/*' />
        /// <devdoc>
        ///     All the items in the control are painted by the system and are of the
        ///     same size
        /// </devdoc>
        Normal = 0,

        /// <include file='doc\TabDrawMode.uex' path='docs/doc[@for="TabDrawMode.OwnerDrawFixed"]/*' />
        /// <devdoc>
        ///     The user paints the items in the control manually
        /// </devdoc>
        OwnerDrawFixed = 1,

    }
}
