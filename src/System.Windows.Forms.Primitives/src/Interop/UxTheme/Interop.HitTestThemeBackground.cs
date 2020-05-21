﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [DllImport(Libraries.UxTheme, ExactSpelling = true)]
        public unsafe static extern HRESULT HitTestThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, uint dwOptions, ref RECT pRect, IntPtr hrgn, Point ptTest, out ushort pwHitTestCode);

        public unsafe static HRESULT HitTestThemeBackground(IHandle hTheme, HandleRef hdc, int iPartId, int iStateId, uint dwOptions, ref RECT pRect, IntPtr hrgn, Point ptTest, out ushort pwHitTestCode)
        {
            HRESULT hr = HitTestThemeBackground(hTheme.Handle, hdc.Handle, iPartId, iStateId, dwOptions, ref pRect, hrgn, ptTest, out pwHitTestCode);
            GC.KeepAlive(hTheme);
            GC.KeepAlive(hdc.Wrapper);
            return hr;
        }
    }
}
