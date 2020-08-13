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
        public static extern HRESULT GetThemeBackgroundRegion(
            IntPtr hTheme,
            Gdi32.HDC hdc,
            int iPartId,
            int iStateId,
            ref RECT pRect,
            out Gdi32.HRGN pRegion);

        public static HRESULT GetThemeBackgroundRegion(
            IHandle hTheme,
            Gdi32.HDC hdc,
            int iPartId,
            int iStateId,
            ref RECT pRect,
            out Gdi32.HRGN pRegion)
        {
            HRESULT hr = GetThemeBackgroundRegion(hTheme.Handle, hdc, iPartId, iStateId, ref pRect, out pRegion);
            GC.KeepAlive(hTheme);
            return hr;
        }
    }
}
