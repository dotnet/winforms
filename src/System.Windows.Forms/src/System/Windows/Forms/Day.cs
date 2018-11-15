// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <include file='doc\Day.uex' path='docs/doc[@for="Day"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the day of the week.
    ///
    ///    </para>
    /// </devdoc>
    public enum Day {
        /// <include file='doc\Day.uex' path='docs/doc[@for="Day.Monday"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The day Monday.
        ///    </para>
        /// </devdoc>
        Monday = 0,
        /// <include file='doc\Day.uex' path='docs/doc[@for="Day.Tuesday"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The day Tuesday.
        ///    </para>
        /// </devdoc>
        Tuesday = 1,
        /// <include file='doc\Day.uex' path='docs/doc[@for="Day.Wednesday"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The day Wednesday.
        ///    </para>
        /// </devdoc>
        Wednesday = 2,
        /// <include file='doc\Day.uex' path='docs/doc[@for="Day.Thursday"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The day Thursday.
        ///    </para>
        /// </devdoc>
        Thursday = 3,
        /// <include file='doc\Day.uex' path='docs/doc[@for="Day.Friday"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The day Friday.
        ///    </para>
        /// </devdoc>
        Friday = 4,
        /// <include file='doc\Day.uex' path='docs/doc[@for="Day.Saturday"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The day Saturday.
        ///    </para>
        /// </devdoc>
        Saturday  = 5,
        /// <include file='doc\Day.uex' path='docs/doc[@for="Day.Sunday"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The day Sunday.
        ///    </para>
        /// </devdoc>
        Sunday = 6,
        /// <include file='doc\Day.uex' path='docs/doc[@for="Day.Default"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A default day of the week specified by the application.
        ///    </para>
        /// </devdoc>
        Default = 7,
    }
}
