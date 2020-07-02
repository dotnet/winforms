// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [DllImport(Libraries.UxTheme, ExactSpelling = true)]
        public unsafe static extern HRESULT GetThemeMargins(
            IntPtr hTheme,
            Gdi32.HDC hdc,
            int iPartId,
            int iStateId,
            int iPropId,
            RECT* prc,
            out MARGINS pMargins);

        public unsafe static HRESULT GetThemeMargins(
            IHandle hTheme,
            Gdi32.HDC hdc,
            int iPartId,
            int iStateId,
            int iPropId,
            RECT* prc,
            out MARGINS pMargins)
        {
            HRESULT hr = GetThemeMargins(hTheme.Handle, hdc, iPartId, iStateId, iPropId, prc, out pMargins);
            GC.KeepAlive(hTheme);
            return hr;
        }
    }
}
