// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using Microsoft.Win32;
    using System.Drawing;




    /// <devdoc>
    ///    <para>
    ///       Specifies the initial position of a
    ///       form.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum FormStartPosition {


        /// <devdoc>
        ///    <para>
        ///       
        ///       The location and size of the form will determine its
        ///       starting position.
        ///       
        ///    </para>
        /// </devdoc>
        Manual = 0,

        /// <devdoc>
        ///    <para>
        ///       The form is centered on the current display,
        ///       and has the dimensions specified in the form's size.
        ///    </para>
        /// </devdoc>
        CenterScreen = 1,

        /// <devdoc>
        ///    <para>
        ///       The form is positioned at the Windows default
        ///       location and has the dimensions specified in the form's size.
        ///    </para>
        /// </devdoc>
        WindowsDefaultLocation = 2,

        /// <devdoc>
        ///    <para>
        ///       The form is positioned at the Windows default
        ///       location and has the bounds determined by Windows default.
        ///    </para>
        /// </devdoc>
        WindowsDefaultBounds = 3,

        /// <devdoc>
        ///    <para>
        ///       The form is centered within the bounds of its parent form.
        ///    </para>
        /// </devdoc>
        CenterParent = 4
    }
}
