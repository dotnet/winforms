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
        public static extern HRESULT GetThemeFont(IntPtr hTheme, Gdi32.HDC hdc, int iPartId, int iStateId, int iPropId, out User32.LOGFONTW pFont);

        public static HRESULT GetThemeFont(IHandle hTheme, Gdi32.HDC hdc, int iPartId, int iStateId, int iPropId, out User32.LOGFONTW pFont)
        {
            HRESULT result = GetThemeFont(hTheme.Handle, hdc, iPartId, iStateId, iPropId, out pFont);
            GC.KeepAlive(hTheme);
            return result;
        }
    }
}
