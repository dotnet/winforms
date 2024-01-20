// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="GetAncestor(HWND, GET_ANCESTOR_FLAGS)"/>
    public static HWND GetAncestor<T>(T hwnd, GET_ANCESTOR_FLAGS flags) where T : IHandle<HWND>
    {
        HWND result = GetAncestor(hwnd.Handle, flags);
        GC.KeepAlive(hwnd.Wrapper);
        return result;
    }
}
