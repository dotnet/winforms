// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;

    /// <devdoc>
    ///    <para>
    ///       Specifies the style of the sizing grip on a <see cref='System.Windows.Forms.Form'/>.
    ///    </para>
    /// </devdoc>
    public enum SizeGripStyle {
        /// <devdoc>
        ///    <para>
        ///       The size grip is automatically display when needed.
        ///    </para>
        /// </devdoc>
        Auto = 0,
        /// <devdoc>
        ///    <para>
        ///       The sizing grip is always shown on the form.
        ///    </para>
        /// </devdoc>
        Show = 1,
        /// <devdoc>
        ///    <para>
        ///       The sizing grip is hidden.
        ///    </para>
        /// </devdoc>
        Hide = 2,
    }
}

