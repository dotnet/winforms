// Licensed to the .NET Foundation under one or more agreements.
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
        public unsafe static extern HRESULT GetThemePosition(IntPtr hTheme, int iPartId, int iStateId, int iPropId, out Point pPoint);

        public unsafe static HRESULT GetThemePosition(IHandle hTheme, int iPartId, int iStateId, int iPropId, out Point pPoint)
        {
            HRESULT hr = GetThemePosition(hTheme.Handle, iPartId, iStateId, iPropId, out pPoint);
            GC.KeepAlive(hTheme);
            return hr;
        }
    }
}
