﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [DllImport(Libraries.UxTheme, ExactSpelling = true)]
        public static extern unsafe HRESULT DrawThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref RECT pRect, RECT* pClipRect);

        public static unsafe HRESULT DrawThemeBackground(IHandle hTheme, HandleRef hdc, int iPartId, int iStateId, ref RECT pRect, RECT* pClipRect)
        {
            HRESULT hr = DrawThemeBackground(hTheme.Handle, hdc.Handle, iPartId, iStateId, ref pRect, pClipRect);
            GC.KeepAlive(hTheme);
            GC.KeepAlive(hdc.Wrapper);
            return hr;
        }
    }
}
