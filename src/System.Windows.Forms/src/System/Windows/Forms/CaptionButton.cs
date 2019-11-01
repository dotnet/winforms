// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the type of caption button to display.
    /// </summary>
    public enum CaptionButton
    {
        /// <summary>
        ///  A Close button.
        /// </summary>
        Close = (int)User32.DFCS.CAPTIONCLOSE,

        /// <summary>
        ///  A Help button.
        /// </summary>
        Help = (int)User32.DFCS.CAPTIONHELP,

        /// <summary>
        ///  A Maximize button.
        /// </summary>
        Maximize = (int)User32.DFCS.CAPTIONMAX,

        /// <summary>
        ///  A Minimize button.
        /// </summary>
        Minimize = (int)User32.DFCS.CAPTIONMIN,

        /// <summary>
        ///  A Restore button.
        /// </summary>
        Restore = (int)User32.DFCS.CAPTIONRESTORE,
    }
}
