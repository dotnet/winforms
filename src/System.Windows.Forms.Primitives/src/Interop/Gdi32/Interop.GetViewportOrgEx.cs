// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        public unsafe static extern BOOL GetViewportOrgEx(HDC hdc, out Point lppoint);

        public unsafe static BOOL GetViewportOrgEx(IHandle hdc, out Point lppoint)
        {
            BOOL result = GetViewportOrgEx((HDC)hdc.Handle, out lppoint);
            GC.KeepAlive(hdc);
            return result;
        }
    }
}
