// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the type of caption button to display.
    /// </devdoc>
    public enum CaptionButton
    {
        /// <summary>
        /// A Close button.
        /// </devdoc>
        Close = NativeMethods.DFCS_CAPTIONCLOSE,

        /// <summary>
        /// A Help button.
        /// </devdoc>
        Help = NativeMethods.DFCS_CAPTIONHELP,

        /// <summary>
        /// A Maximize button.
        /// </devdoc>
        Maximize = NativeMethods.DFCS_CAPTIONMAX,

        /// <summary>
        /// A Minimize button.
        /// </devdoc>
        Minimize = NativeMethods.DFCS_CAPTIONMIN,

        /// <summary>
        /// A Restore button.
        /// </devdoc>
        Restore = NativeMethods.DFCS_CAPTIONRESTORE,
    }
}
