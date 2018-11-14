// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\TabAppearance.uex' path='docs/doc[@for="TabAppearance"]/*' />
    /// <devdoc>
    ///     The various tab controls will let you configure their appearance.  This
    ///     enumeration contains the possible values.
    /// </devdoc>
    public enum TabAppearance {

        /// <include file='doc\TabAppearance.uex' path='docs/doc[@for="TabAppearance.Normal"]/*' />
        /// <devdoc>
        ///     Indicates that the tabs look like normal tabs typically seen in Property
        ///     page type situations.
        /// </devdoc>
        Normal = 0,

        /// <include file='doc\TabAppearance.uex' path='docs/doc[@for="TabAppearance.Buttons"]/*' />
        /// <devdoc>
        ///     Indicates that the tabs look like buttons as seen on the taskbar found
        ///     in Windows 95 or Windows NT.
        /// </devdoc>
        Buttons = 1,

        /// <include file='doc\TabAppearance.uex' path='docs/doc[@for="TabAppearance.FlatButtons"]/*' />
        /// <devdoc>
        ///     Indicates that buttons should be draw flat instead of like regular
        ///     windows pushbuttons.
        /// </devdoc>
        FlatButtons = 2,

    }
}
