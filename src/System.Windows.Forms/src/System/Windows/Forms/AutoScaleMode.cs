// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    
    /// <include file='doc\AutoScaleMode.uex' path='docs/doc[@for="AutoScaleMode"]/*' />
    /// <devdoc>
    ///     Specifies the auto scaling mode used by a container control.
    /// </devdoc>
    public enum AutoScaleMode {

        /// <include file='doc\AutoScaleMode.uex' path='docs/doc[@for="AutoScaleMode.None"]/*' />
        /// <devdoc>
        ///     AutoScale is turned off.
        /// </devdoc>
        None,

        /// <include file='doc\AutoScaleMode.uex' path='docs/doc[@for="AutoScaleMode.Font"]/*' />
        /// <devdoc>
        ///     Controls scale according to the dimensions of the font they are using.
        /// </devdoc>
        Font,

        /// <include file='doc\AutoScaleMode.uex' path='docs/doc[@for="AutoScaleMode.Dpi"]/*' />
        /// <devdoc>
        ///     Controls scale according to the display Dpi.
        /// </devdoc>
        Dpi,

        /// <include file='doc\AutoScaleMode.uex' path='docs/doc[@for="AutoScaleMode.Inherit"]/*' />
        /// <devdoc>
        ///     Controls scale according to their parent's scaling mode.  If there is no parent,
        ///     This behaves as if AutoScaleMode.None were set.
        /// </devdoc>
        Inherit
    }
}

