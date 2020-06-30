// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        public static extern BOOL OffsetViewportOrgEx(HDC hdc, int x, int y, ref Point lppt);

        public static BOOL OffsetViewportOrgEx(IHandle hdc, int x, int y, ref Point lppt)
        {
            BOOL result = OffsetViewportOrgEx((HDC)hdc.Handle, x, y, ref lppt);
            GC.KeepAlive(hdc);
            return result;
        }
    }
}
