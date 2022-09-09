// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    internal static class Win32WindowExtensions
    {
        public static User32.WS_EX GetExtendedStyle(this IWin32Window window)
        {
            User32.WS_EX style = (User32.WS_EX)User32.GetWindowLong(Control.GetSafeHandle(window).Handle, User32.GWL.EXSTYLE);
            GC.KeepAlive(window);
            return style;
        }
    }
}
