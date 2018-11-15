// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;


    /// <include file='doc\ScreenOrientation.uex' path='docs/doc[@for="ScreenOrientation"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the angle of screen orientation
    ///    </para>
    /// </devdoc>
    public enum ScreenOrientation {
        /// <include file='doc\ScreenOrientation.uex' path='docs/doc[@for="Day.Angle0"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The screen is oriented at 0 degrees
        ///    </para>
        /// </devdoc>
        Angle0 = 0,

        /// <include file='doc\ScreenOrientation.uex' path='docs/doc[@for="Day.Angle90"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The screen is oriented at 90 degrees
        ///    </para>
        /// </devdoc>
        Angle90 = 1,

        /// <include file='doc\ScreenOrientation.uex' path='docs/doc[@for="Day.Angle180"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The screen is oriented at 180 degrees.
        ///    </para>
        /// </devdoc>
        Angle180 = 2,

        /// <include file='doc\ScreenOrientation.uex' path='docs/doc[@for="Day.Angle270"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The screen is oriented at 270 degrees.
        ///    </para>
        /// </devdoc>
        Angle270 = 3,
    }
}

