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
        public static extern HRESULT GetThemeFont(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, int iPropId, ref User32.LOGFONTW pFont);

        public static HRESULT GetThemeFont(HandleRef hTheme, HandleRef hdc, int iPartId, int iStateId, int iPropId, ref User32.LOGFONTW pFont)
        {
            HRESULT result = GetThemeFont(hTheme.Handle, hdc.Handle, iPartId, iStateId, iPropId, ref pFont);
            GC.KeepAlive(hTheme.Wrapper);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
