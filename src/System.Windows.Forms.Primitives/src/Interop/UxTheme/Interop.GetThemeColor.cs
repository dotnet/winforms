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
        public static extern HRESULT GetThemeColor(IntPtr hTheme, int iPartId, int iStateId, int iPropId, ref int pColor);

        public static HRESULT GetThemeColor(IHandle hTheme, int iPartId, int iStateId, int iPropId, ref int pColor)
        {
            HRESULT hr = GetThemeColor(hTheme.Handle, iPartId, iStateId, iPropId, ref pColor);
            GC.KeepAlive(hTheme);
            return hr;
        }
    }
}
