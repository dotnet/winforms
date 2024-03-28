// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32;

// https://github.com/microsoft/CsWin32/issues/882
internal static partial class PInvoke
{
    [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern BOOL Shell_NotifyIconW(NOTIFY_ICON_MESSAGE dwMessage, ref NOTIFYICONDATAW lpData);
}
