// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

namespace System.Windows.Forms
{
    [Flags]
    public enum MessageBoxOptions
    {
        /// <summary>
        ///  Specifies that the message box is displayed on the active desktop.
        /// </summary>
        ServiceNotification = (int)MB.SERVICE_NOTIFICATION,

        /// <summary>
        ///  Specifies that the message box is displayed on the active desktop.
        /// </summary>
        DefaultDesktopOnly = (int)MB.DEFAULT_DESKTOP_ONLY,

        /// <summary>
        ///  Specifies that the message box text is right-aligned.
        /// </summary>
        RightAlign = (int)MB.RIGHT,

        /// <summary>
        ///  Specifies that the message box text is displayed with Rtl reading order.
        /// </summary>
        RtlReading = (int)MB.RTLREADING,
    }
}
