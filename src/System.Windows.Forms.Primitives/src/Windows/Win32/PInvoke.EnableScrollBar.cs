// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static BOOL EnableScrollBar<T>(T hWnd, SCROLLBAR_CONSTANTS wSBflags, ENABLE_SCROLL_BAR_ARROWS wArrows)
            where T : IHandle<HWND>
        {
            BOOL result = EnableScrollBar(hWnd.Handle, wSBflags, wArrows);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
