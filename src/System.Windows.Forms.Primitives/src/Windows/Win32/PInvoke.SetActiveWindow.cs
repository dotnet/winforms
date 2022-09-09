﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static HWND SetActiveWindow<T>(in T hWnd) where T : IHandle<HWND>
        {
            HWND result = SetActiveWindow(hWnd.Handle);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
