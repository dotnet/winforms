// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System.Drawing;

    /// <devdoc>
    ///    <para>Specifies a value indicating whether the text appears
    ///       from right to
    ///       left, as when using Hebrew or Arabic fonts.</para>
    /// </devdoc>
    public enum RightToLeft {

        /// <devdoc>
        ///    <para>
        ///       
        ///       The
        ///       
        ///       text reads
        ///       
        ///       from left to right. This is the default.
        ///       
        ///    </para>
        /// </devdoc>
        No = 0,

        /// <devdoc>
        ///    <para>
        ///       The text reads from
        ///       right to left.
        ///       
        ///    </para>
        /// </devdoc>
        Yes = 1,

        /// <devdoc>
        ///    <para>
        ///       The direction the
        ///       text appears in is inherited from the parent control.
        ///       
        ///    </para>
        /// </devdoc>
        Inherit = 2
    }
}
