// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <devdoc>
    ///    <para>
    ///       Specifies the day of the week.
    ///
    ///    </para>
    /// </devdoc>
    public enum Day {
        /// <devdoc>
        ///    <para>
        ///       The day Monday.
        ///    </para>
        /// </devdoc>
        Monday = 0,
        /// <devdoc>
        ///    <para>
        ///       The day Tuesday.
        ///    </para>
        /// </devdoc>
        Tuesday = 1,
        /// <devdoc>
        ///    <para>
        ///       The day Wednesday.
        ///    </para>
        /// </devdoc>
        Wednesday = 2,
        /// <devdoc>
        ///    <para>
        ///       The day Thursday.
        ///    </para>
        /// </devdoc>
        Thursday = 3,
        /// <devdoc>
        ///    <para>
        ///       The day Friday.
        ///    </para>
        /// </devdoc>
        Friday = 4,
        /// <devdoc>
        ///    <para>
        ///       The day Saturday.
        ///    </para>
        /// </devdoc>
        Saturday  = 5,
        /// <devdoc>
        ///    <para>
        ///       The day Sunday.
        ///    </para>
        /// </devdoc>
        Sunday = 6,
        /// <devdoc>
        ///    <para>
        ///       A default day of the week specified by the application.
        ///    </para>
        /// </devdoc>
        Default = 7,
    }
}
