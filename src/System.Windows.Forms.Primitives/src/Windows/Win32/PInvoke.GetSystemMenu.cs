// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static HMENU GetSystemMenu<T>(in T hwnd, BOOL bRevert) where T : IHandle<HWND>
        {
            HMENU result = GetSystemMenu(hwnd.Handle, bRevert);
            GC.KeepAlive(hwnd.Wrapper);
            return result;
        }
    }
}
