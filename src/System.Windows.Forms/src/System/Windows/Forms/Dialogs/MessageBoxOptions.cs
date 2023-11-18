// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

[Flags]
public enum MessageBoxOptions
{
    /// <summary>
    ///  Specifies that the message box is displayed on the active desktop.
    /// </summary>
    ServiceNotification = (int)MESSAGEBOX_STYLE.MB_SERVICE_NOTIFICATION,

    /// <summary>
    ///  Specifies that the message box is displayed on the active desktop.
    /// </summary>
    DefaultDesktopOnly = (int)MESSAGEBOX_STYLE.MB_DEFAULT_DESKTOP_ONLY,

    /// <summary>
    ///  Specifies that the message box text is right-aligned.
    /// </summary>
    RightAlign = (int)MESSAGEBOX_STYLE.MB_RIGHT,

    /// <summary>
    ///  Specifies that the message box text is displayed with Rtl reading order.
    /// </summary>
    RtlReading = (int)MESSAGEBOX_STYLE.MB_RTLREADING,
}
