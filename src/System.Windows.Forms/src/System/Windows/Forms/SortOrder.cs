// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;



    /// <devdoc>
    ///    <para>
    ///       Specifies how items in
    ///       a list are sorted.
    ///    </para>
    /// </devdoc>
    public enum SortOrder {

        /// <devdoc>
        ///    <para>
        ///       The items are
        ///       not sorted.
        ///    </para>
        /// </devdoc>
        None = 0,

        /// <devdoc>
        ///    <para>
        ///       The items
        ///       are sorted in ascending order.
        ///    </para>
        /// </devdoc>
        Ascending = 1,

        /// <devdoc>
        ///    <para>
        ///       The items are
        ///       sorted in descending order.
        ///    </para>
        /// </devdoc>
        Descending = 2,

    }
}
