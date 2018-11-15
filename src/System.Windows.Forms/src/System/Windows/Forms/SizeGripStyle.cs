// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;

    /// <include file='doc\SizeGripStyle.uex' path='docs/doc[@for="SizeGripStyle"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the style of the sizing grip on a <see cref='System.Windows.Forms.Form'/>.
    ///    </para>
    /// </devdoc>
    public enum SizeGripStyle {
        /// <include file='doc\SizeGripStyle.uex' path='docs/doc[@for="SizeGripStyle.Auto"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The size grip is automatically display when needed.
        ///    </para>
        /// </devdoc>
        Auto = 0,
        /// <include file='doc\SizeGripStyle.uex' path='docs/doc[@for="SizeGripStyle.Show"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The sizing grip is always shown on the form.
        ///    </para>
        /// </devdoc>
        Show = 1,
        /// <include file='doc\SizeGripStyle.uex' path='docs/doc[@for="SizeGripStyle.Hide"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The sizing grip is hidden.
        ///    </para>
        /// </devdoc>
        Hide = 2,
    }
}

