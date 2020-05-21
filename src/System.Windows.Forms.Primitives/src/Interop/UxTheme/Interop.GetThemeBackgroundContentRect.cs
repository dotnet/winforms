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
        public static extern HRESULT GetThemeBackgroundContentRect(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref RECT pBoundingRect, out RECT pContentRect);

        public static HRESULT GetThemeBackgroundContentRect(IHandle hTheme, HandleRef hdc, int iPartId, int iStateId, ref RECT pBoundingRect, out RECT pContentRect)
        {
            HRESULT hr = GetThemeBackgroundContentRect(hTheme.Handle, hdc.Handle, iPartId, iStateId, ref pBoundingRect, out pContentRect);
            GC.KeepAlive(hTheme);
            GC.KeepAlive(hdc.Wrapper);
            return hr;
        }
    }
}
