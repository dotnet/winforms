// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="UiaDisconnectProvider(IRawElementProviderSimple*)"/>
    public static unsafe void UiaDisconnectProvider(IRawElementProviderSimple.Interface? provider, bool skipOSCheck = false)
    {
        if (provider is not null && (skipOSCheck || OsVersion.IsWindows8OrGreater()))
        {
            using var providerScope = ComHelpers.GetComScope<IRawElementProviderSimple>(provider);
#pragma warning disable CA1416 // call site is reachable on 'windows' 6.1 and later
            HRESULT result = UiaDisconnectProvider(providerScope);
#pragma warning restore
            if (result.Failed)
            {
                Debug.WriteLine($"UiaDisconnectProvider failed with {result}");
            }

            Debug.Assert(result == HRESULT.S_OK || result == HRESULT.E_INVALIDARG, $"UiaDisconnectProvider failed with {result}");
        }
    }
}
