// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [DllImport(Libraries.UxTheme, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern HRESULT DrawThemeText(
            IntPtr hTheme,
            Gdi32.HDC hdc,
            int iPartId,
            int iStateId,
            string pszText,
            int iCharCount,
            User32.DT dwTextFlags,
            uint dwTextFlags2,
            ref RECT pRect);

        public static HRESULT DrawThemeText(
            IHandle hTheme,
            Gdi32.HDC hdc,
            int iPartId,
            int iStateId,
            string pszText,
            int iCharCount,
            User32.DT dwTextFlags,
            uint dwTextFlags2,
            ref RECT pRect)
        {
            HRESULT hr = DrawThemeText(hTheme.Handle, hdc, iPartId, iStateId, pszText, iCharCount, dwTextFlags, dwTextFlags2, ref pRect);
            GC.KeepAlive(hTheme);
            return hr;
        }
    }
}
