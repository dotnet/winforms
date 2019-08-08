// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    [Flags]
    public enum MessageBoxOptions
    {
        /// <summary>
        ///  Specifies that the message box is displayed on the active desktop.
        /// </summary>
        ServiceNotification = 0x00200000,

        /// <summary>
        ///  Specifies that the message box is displayed on the active desktop.
        /// </summary>
        DefaultDesktopOnly = 0x00020000,

        /// <summary>
        ///  Specifies that the message box text is right-aligned.
        /// </summary>
        RightAlign = 0x00080000,

        /// <summary>
        ///  Specifies that the message box text is displayed with Rtl reading order.
        /// </summary>
        RtlReading = 0x00100000,
    }
}
