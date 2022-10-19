﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static unsafe BOOL ValidateRect<T>(T hWnd, RECT* lpRect)
            where T : IHandle<HWND>
        {
            BOOL result = ValidateRect(hWnd.Handle, lpRect);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
