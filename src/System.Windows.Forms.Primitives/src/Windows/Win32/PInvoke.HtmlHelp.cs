// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Data.HtmlHelp;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static HWND HtmlHelp(HandleRef<HWND> hwndCaller, string? pszFile, HTML_HELP_COMMAND uCommand, nuint dwData)
        {
            HWND result = HtmlHelp(hwndCaller.Handle, pszFile, uCommand, dwData);
            GC.KeepAlive(hwndCaller.Wrapper);
            return result;
        }

        public static unsafe HWND HtmlHelp(HandleRef<HWND> hwndCaller, string? pszFile, HTML_HELP_COMMAND uCommand, string data)
        {
            fixed (char* dwData = data)
            {
                return HtmlHelp(hwndCaller, pszFile, uCommand, (nuint)dwData);
            }
        }
    }
}
