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
        public unsafe static extern HRESULT GetThemeFilename(IntPtr hTheme, int iPartId, int iStateId, int iPropId, char* pszThemeFilename, int cchMaxBuffChars);

        public unsafe static HRESULT GetThemeFilename(IHandle hTheme, int iPartId, int iStateId, int iPropId, char* pszThemeFilename, int cchMaxBuffChars)
        {
            HRESULT hr = GetThemeFilename(hTheme.Handle, iPartId, iStateId, iPropId, pszThemeFilename, cchMaxBuffChars);
            GC.KeepAlive(hTheme);
            return hr;
        }
    }
}
