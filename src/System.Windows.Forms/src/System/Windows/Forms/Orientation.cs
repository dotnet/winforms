// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;



    /// <include file='doc\Orientation.uex' path='docs/doc[@for="Orientation"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the orientation of controls or elements of controls.
    ///       
    ///    </para>
    /// </devdoc>
    public enum Orientation {
        /// <include file='doc\Orientation.uex' path='docs/doc[@for="Orientation.Horizontal"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control or element is oriented horizontally.
        ///    </para>
        /// </devdoc>
        Horizontal = 0,

        /// <include file='doc\Orientation.uex' path='docs/doc[@for="Orientation.Vertical"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control or element is oriented vertically.
        ///    </para>
        /// </devdoc>
        Vertical = 1,

    }
}
