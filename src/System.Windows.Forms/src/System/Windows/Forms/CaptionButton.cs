// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the type of caption button to display.
    /// </devdoc>
    public enum CaptionButton
    {
        /// <devdoc>
        /// A Close button.
        /// </devdoc>
        Close = NativeMethods.DFCS_CAPTIONCLOSE,

        /// <devdoc>
        /// A Help button.
        /// </devdoc>
        Help = NativeMethods.DFCS_CAPTIONHELP,

        /// <devdoc>
        /// A Maximize button.
        /// </devdoc>
        Maximize = NativeMethods.DFCS_CAPTIONMAX,

        /// <devdoc>
        /// A Minimize button.
        /// </devdoc>
        Minimize = NativeMethods.DFCS_CAPTIONMIN,

        /// <devdoc>
        /// A Restore button.
        /// </devdoc>
        Restore = NativeMethods.DFCS_CAPTIONRESTORE,
    }
}
