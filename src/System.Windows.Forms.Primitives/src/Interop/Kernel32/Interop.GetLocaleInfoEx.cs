// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Kernel32
    {
        public const string LOCALE_NAME_USER_DEFAULT = null;
        public const string LOCALE_NAME_INVARIANT = "";
        public const string LOCALE_NAME_SYSTEM_DEFAULT = "!x-sys-default-locale";

        [DllImport(Libraries.Kernel32, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public unsafe static extern int GetLocaleInfoEx(string lpLocaleName, LCTYPE LCType, char* lpLCData, int cchData);
    }
}
