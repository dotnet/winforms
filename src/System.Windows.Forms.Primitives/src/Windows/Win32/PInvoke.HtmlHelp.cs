// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.Data.HtmlHelp;

namespace Windows.Win32;

internal static partial class PInvoke
{
    internal static unsafe HWND HtmlHelp(HWND hwndCaller, string? pszFile, HTML_HELP_COMMAND uCommand, nuint dwData)
    {
        // Copied from generated code pending resolution of https://github.com/microsoft/win32metadata/issues/1749

        fixed (char* p = pszFile)
        {
            HWND __retVal = LocalExternFunction(hwndCaller, p, (uint)uCommand, dwData);
            return __retVal;
        }

        [DllImport(Libraries.Hhctrl, ExactSpelling = true, EntryPoint = "HtmlHelpW")]
        static extern HWND LocalExternFunction(HWND hwndCaller, PCWSTR pszFile, uint uCommand, nuint dwData);
    }

    /// <inheritdoc cref="HtmlHelp(HWND, string, HTML_HELP_COMMAND, nuint)" />
    internal static unsafe HWND HtmlHelp<T>(T hwndCaller, string? pszFile, HTML_HELP_COMMAND uCommand, string? dwData)
        where T : IHandle<HWND>
    {
        fixed (void* d = dwData)
        {
            HWND result = HtmlHelp(hwndCaller.Handle, pszFile, uCommand, (nuint)d);
            GC.KeepAlive(hwndCaller.Wrapper);
            return result;
        }
    }

    /// <inheritdoc cref="HtmlHelp(HWND, string, HTML_HELP_COMMAND, nuint)" />
    internal static unsafe HWND HtmlHelp<TCaller, TData>(
        TCaller hwndCaller,
        string? pszFile,
        HTML_HELP_COMMAND uCommand,
        ref readonly TData dwData)
        where TCaller : IHandle<HWND>
        where TData : unmanaged
    {
        fixed (void* v = &dwData)
        {
            HWND result = HtmlHelp(hwndCaller.Handle, pszFile, uCommand, (nuint)v);
            GC.KeepAlive(hwndCaller.Wrapper);
            return result;
        }
    }
}
