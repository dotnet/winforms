// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [DllImport(Libraries.UxTheme, ExactSpelling = true)]
        public static extern HRESULT DrawThemeParentBackground(IntPtr hwnd, IntPtr hdc, ref RECT prc);

        public static HRESULT DrawThemeParentBackground(IHandle hwnd, HandleRef hdc, ref RECT prc)
        {
            HRESULT hr = DrawThemeParentBackground(hwnd.Handle, hdc.Handle, ref prc);
            GC.KeepAlive(hwnd);
            GC.KeepAlive(hdc.Wrapper);
            return hr;
        }
    }
}
