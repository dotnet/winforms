// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="UiaReturnRawElementProvider(HWND, WPARAM, LPARAM, IRawElementProviderSimple*)"/>
    public static unsafe LRESULT UiaReturnRawElementProvider<T>(
        T hwnd,
        WPARAM wParam,
        LPARAM lParam,
        IRawElementProviderSimple.Interface? el)
        where T : IHandle<HWND>
    {
        using var provider = ComHelpers.TryGetComScope<IRawElementProviderSimple>(el);
        LRESULT result = UiaReturnRawElementProvider(hwnd.Handle, wParam, lParam, provider);
        GC.KeepAlive(hwnd.Wrapper);
        return result;
    }
}
