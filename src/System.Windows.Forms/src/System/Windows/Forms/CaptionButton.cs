// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\CaptionButton.uex' path='docs/doc[@for="CaptionButton"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the type
    ///       of caption button to display.
    ///    </para>
    /// </devdoc>
    public enum CaptionButton {

        /// <include file='doc\CaptionButton.uex' path='docs/doc[@for="CaptionButton.Close"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A Close button.
        ///
        ///    </para>
        /// </devdoc>
        Close = NativeMethods.DFCS_CAPTIONCLOSE,

        /// <include file='doc\CaptionButton.uex' path='docs/doc[@for="CaptionButton.Help"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A Help button.
        ///    </para>
        /// </devdoc>
        Help = NativeMethods.DFCS_CAPTIONHELP,

        /// <include file='doc\CaptionButton.uex' path='docs/doc[@for="CaptionButton.Maximize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A Maximize button.
        ///    </para>
        /// </devdoc>
        Maximize = NativeMethods.DFCS_CAPTIONMAX,

        /// <include file='doc\CaptionButton.uex' path='docs/doc[@for="CaptionButton.Minimize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A Minimize button.
        ///    </para>
        /// </devdoc>
        Minimize = NativeMethods.DFCS_CAPTIONMIN,

        /// <include file='doc\CaptionButton.uex' path='docs/doc[@for="CaptionButton.Restore"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A Restore button.
        ///    </para>
        /// </devdoc>
        Restore = NativeMethods.DFCS_CAPTIONRESTORE,
    }
}
