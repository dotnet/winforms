﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [DllImport(Libraries.UxTheme, ExactSpelling = true)]
        public unsafe static extern HRESULT GetThemeTextMetrics(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, out TextMetrics ptm);

        public unsafe static HRESULT GetThemeTextMetrics(IHandle hTheme, HandleRef hdc, int iPartId, int iStateId, out TextMetrics ptm)
        {
            HRESULT hr = GetThemeTextMetrics(hTheme.Handle, hdc.Handle, iPartId, iStateId, out ptm);
            GC.KeepAlive(hTheme);
            GC.KeepAlive(hdc.Wrapper);
            return hr;
        }
    }
}
