// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern HGDIOBJ SelectObject(HDC hdc, HGDIOBJ h);

        public static HGDIOBJ SelectObject(IHandle hdc, HGDIOBJ h)
        {
            HGDIOBJ lastObject = SelectObject((HDC)hdc.Handle, h);
            GC.KeepAlive(hdc);
            return lastObject;
        }

        public static HGDIOBJ SelectObject(HandleRef hdc, HGDIOBJ h)
        {
            HGDIOBJ lastObject = SelectObject((HDC)hdc.Handle, h);
            GC.KeepAlive(hdc.Wrapper);
            return lastObject;
        }
    }
}
