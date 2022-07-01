﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [LibraryImport(Libraries.User32, SetLastError = true)]
        public static partial BOOL EnableScrollBar(IntPtr hWnd, SB wSBflags, ESB wArrows);

        public static BOOL EnableScrollBar(IHandle hWnd, SB wSBflags, ESB wArrows)
        {
            BOOL result = EnableScrollBar(hWnd.Handle, wSBflags, wArrows);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}
