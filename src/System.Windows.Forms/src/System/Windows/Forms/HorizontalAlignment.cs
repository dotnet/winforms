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
    ///       Specifies how an object or text in a control is
    ///       horizontally aligned relative to an element of the control.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum HorizontalAlignment {


        /// <devdoc>
        ///    <para>
        ///       The object or text is aligned on the left of the control element.
        ///    </para>
        /// </devdoc>
        Left = 0,


        /// <devdoc>
        ///    <para>
        ///       The object or text is aligned on the right of the control element.
        ///    </para>
        /// </devdoc>
        Right = 1,


        /// <devdoc>
        ///    <para>
        ///       The object or text is aligned in the center of the control element.
        ///    </para>
        /// </devdoc>
        Center = 2,

    }
}
