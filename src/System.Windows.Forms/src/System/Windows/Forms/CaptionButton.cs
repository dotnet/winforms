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
    ///       Specifies the type
    ///       of caption button to display.
    ///    </para>
    /// </devdoc>
    public enum CaptionButton {


        /// <devdoc>
        ///    <para>
        ///       A Close button.
        ///
        ///    </para>
        /// </devdoc>
        Close = NativeMethods.DFCS_CAPTIONCLOSE,


        /// <devdoc>
        ///    <para>
        ///       A Help button.
        ///    </para>
        /// </devdoc>
        Help = NativeMethods.DFCS_CAPTIONHELP,


        /// <devdoc>
        ///    <para>
        ///       A Maximize button.
        ///    </para>
        /// </devdoc>
        Maximize = NativeMethods.DFCS_CAPTIONMAX,


        /// <devdoc>
        ///    <para>
        ///       A Minimize button.
        ///    </para>
        /// </devdoc>
        Minimize = NativeMethods.DFCS_CAPTIONMIN,


        /// <devdoc>
        ///    <para>
        ///       A Restore button.
        ///    </para>
        /// </devdoc>
        Restore = NativeMethods.DFCS_CAPTIONRESTORE,
    }
}
