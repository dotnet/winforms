// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Data.HtmlHelp;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="HtmlHelp(HWND, string, HTML_HELP_COMMAND, nuint)" />
    internal static unsafe HWND HtmlHelp<T>(T hwndCaller, string? pszFile, HTML_HELP_COMMAND uCommand, nuint dwData)
        where T : IHandle<HWND>
    {
        HWND result = HtmlHelp(hwndCaller.Handle, pszFile, uCommand, dwData);
        GC.KeepAlive(hwndCaller.Wrapper);
        return result;
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
