// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    /// <devdoc>
    ///    <para>Specifies the style of control to display.</para>
    /// </devdoc>
    public enum FlatStyle {

        /// <devdoc>
        ///    <para>
        ///       The control appears flat.
        ///    </para>
        /// </devdoc>
        Flat,

        /// <devdoc>
        ///    <para>
        ///       A control appears flat until the mouse pointer
        ///       moves over
        ///       it, at which point it appears three-dimensional.
        ///    </para>
        /// </devdoc>
        Popup,

        /// <devdoc>
        ///    <para>
        ///       The control appears three-dimensional.
        ///    </para>
        /// </devdoc>
        Standard,

        /// <devdoc>
        ///    <para>
        ///       The control appears three-dimensional.
        ///    </para>
        /// </devdoc>
        System,
    }
}

