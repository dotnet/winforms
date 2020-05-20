// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [DllImport(Libraries.UxTheme, ExactSpelling = true)]
        public unsafe static extern HRESULT GetThemePartSize(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, RECT* prc, ThemeSizeType eSize, out Size psz);

        public unsafe static HRESULT GetThemePartSize(IHandle hTheme, HandleRef hdc, int iPartId, int iStateId, RECT* prc, ThemeSizeType eSize, out Size psz)
        {
            HRESULT hr = GetThemePartSize(hTheme.Handle, hdc.Handle, iPartId, iStateId, prc, eSize, out psz);
            GC.KeepAlive(hTheme);
            GC.KeepAlive(hdc.Wrapper);
            return hr;
        }

        public unsafe static HRESULT GetThemePartSize(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, RECT* prc, ThemeSizeType eSize, out Size psz)
        {
            HRESULT hr = GetThemePartSize(hTheme.Handle, hdc.Handle, iPartId, iStateId, prc, eSize, out psz);
            GC.KeepAlive(hTheme.Wrapper);
            GC.KeepAlive(hdc.Wrapper);
            return hr;
        }
    }
}
