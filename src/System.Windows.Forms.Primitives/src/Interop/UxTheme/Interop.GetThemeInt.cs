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
        public static extern HRESULT GetThemeInt(IntPtr hTheme, int iPartId, int iStateId, int iPropId, ref int piVal);

        public static HRESULT GetThemeInt(IHandle hTheme, int iPartId, int iStateId, int iPropId, ref int piVal)
        {
            HRESULT hr = GetThemeInt(hTheme.Handle, iPartId, iStateId, iPropId, ref piVal);
            GC.KeepAlive(hTheme);
            return hr;
        }
    }
}
