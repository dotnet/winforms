// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="UiaHostProviderFromHwnd(HWND, IRawElementProviderSimple**)"/>
    public static unsafe HRESULT UiaHostProviderFromHwnd<T>(T hwnd, out IRawElementProviderSimple* ppProvider) where T : IHandle<HWND>
    {
        IRawElementProviderSimple* provider;
        HRESULT result = UiaHostProviderFromHwnd(hwnd.Handle, &provider);
        GC.KeepAlive(hwnd.Wrapper);
        ppProvider = provider;
        return result;
    }
}
