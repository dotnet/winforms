// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="SHAutoComplete(HWND, SHELL_AUTOCOMPLETE_FLAGS)"/>
    public static HRESULT SHAutoComplete<T>(T hwndEdit, SHELL_AUTOCOMPLETE_FLAGS flags) where T : IHandle<HWND>
    {
        HRESULT result = SHAutoComplete(hwndEdit.Handle, flags);
        GC.KeepAlive(hwndEdit.Wrapper);
        return result;
    }
}
