// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [DllImport(Libraries.UxTheme, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public unsafe static extern HRESULT GetThemeTextExtent(
            IntPtr hTheme,
            Gdi32.HDC hdc,
            int iPartId,
            int iStateId,
            string pszText,
            int cchCharCount,
            uint dwTextFlags,
            RECT* pBoundingRect,
            out RECT pExtentRect);

        public unsafe static HRESULT GetThemeTextExtent(
            IHandle hTheme,
            Gdi32.HDC hdc,
            int iPartId,
            int iStateId,
            string pszText,
            int cchCharCount,
            uint dwTextFlags,
            RECT* pBoundingRect,
            out RECT pExtentRect)
        {
            HRESULT hr = GetThemeTextExtent(
                hTheme.Handle,
                hdc,
                iPartId,
                iStateId,
                pszText,
                cchCharCount,
                dwTextFlags,
                pBoundingRect,
                out pExtentRect);

            GC.KeepAlive(hTheme);
            return hr;
        }
    }
}
