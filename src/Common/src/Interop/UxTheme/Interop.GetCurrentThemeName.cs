// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [DllImport(Libraries.UxTheme, CharSet = CharSet.Unicode)]
        public static extern unsafe HRESULT GetCurrentThemeName(char *pszThemeFileName, int dwMaxNameChars, char *pszColorBuff, int dwMaxColorChars, char *pszSizeBuff, int cchMaxSizeChars);
    }
}
