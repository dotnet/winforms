﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static HWND ChildWindowFromPointEx<T>(T hwndParent, Point pt, CWP_FLAGS uFlags)
            where T : IHandle<HWND>
        {
            HWND result = ChildWindowFromPointEx(hwndParent.Handle, pt, uFlags);
            GC.KeepAlive(hwndParent.Wrapper);
            return result;
        }
    }
}
