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
        public static extern HRESULT DrawThemeEdge(
            IntPtr hTheme,
            Gdi32.HDC hdc,
            int iPartId,
            int iStateId,
            ref RECT pDestRect,
            User32.EDGE uEdge,
            User32.BF uFlags,
            ref RECT pContentRect);

        public static HRESULT DrawThemeEdge(
            IHandle hTheme,
            Gdi32.HDC hdc,
            int iPartId,
            int iStateId,
            ref RECT pDestRect,
            User32.EDGE uEdge,
            User32.BF uFlags,
            ref RECT pContentRect)
        {
            HRESULT hr = DrawThemeEdge(hTheme.Handle, hdc, iPartId, iStateId, ref pDestRect, uEdge, uFlags, ref pContentRect);
            GC.KeepAlive(hTheme);
            return hr;
        }
    }
}
