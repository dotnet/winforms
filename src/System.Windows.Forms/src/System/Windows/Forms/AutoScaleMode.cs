// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    

    /// <devdoc>
    ///     Specifies the auto scaling mode used by a container control.
    /// </devdoc>
    public enum AutoScaleMode {


        /// <devdoc>
        ///     AutoScale is turned off.
        /// </devdoc>
        None,


        /// <devdoc>
        ///     Controls scale according to the dimensions of the font they are using.
        /// </devdoc>
        Font,


        /// <devdoc>
        ///     Controls scale according to the display Dpi.
        /// </devdoc>
        Dpi,


        /// <devdoc>
        ///     Controls scale according to their parent's scaling mode.  If there is no parent,
        ///     This behaves as if AutoScaleMode.None were set.
        /// </devdoc>
        Inherit
    }
}

