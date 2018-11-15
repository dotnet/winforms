// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    /// <include file='doc\FlatStyle.uex' path='docs/doc[@for="FlatStyle"]/*' />
    /// <devdoc>
    ///    <para>Specifies the style of control to display.</para>
    /// </devdoc>
    public enum FlatStyle {
        /// <include file='doc\FlatStyle.uex' path='docs/doc[@for="FlatStyle.Flat"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control appears flat.
        ///    </para>
        /// </devdoc>
        Flat,
        /// <include file='doc\FlatStyle.uex' path='docs/doc[@for="FlatStyle.Popup"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A control appears flat until the mouse pointer
        ///       moves over
        ///       it, at which point it appears three-dimensional.
        ///    </para>
        /// </devdoc>
        Popup,
        /// <include file='doc\FlatStyle.uex' path='docs/doc[@for="FlatStyle.Standard"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control appears three-dimensional.
        ///    </para>
        /// </devdoc>
        Standard,
        /// <include file='doc\FlatStyle.uex' path='docs/doc[@for="FlatStyle.System"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The control appears three-dimensional.
        ///    </para>
        /// </devdoc>
        System,
    }
}

