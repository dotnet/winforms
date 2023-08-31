// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class UiaCore
    {
        [DllImport(Libraries.UiaCore, EntryPoint = "UiaDisconnectProvider")]
        private static extern HRESULT UiaDisconnectProviderInternal(IRawElementProviderSimple provider);

        public static void UiaDisconnectProvider(IRawElementProviderSimple? provider)
        {
            if (provider is not null)
            {
                // E_INVALIDARG is either with a null provider or one that does not have a RuntimeId.
                // S_FALSE indicates that the same object is disconnected again.
                HRESULT result = UiaDisconnectProviderInternal(provider);
                if (result.Failed)
                {
                    Debug.WriteLine($"UiaDisconnectProvider failed with {result}");
                }

                Debug.Assert(result == HRESULT.S_OK || result == HRESULT.E_INVALIDARG, $"UiaDisconnectProvider failed with {result}");
            }
        }
    }
}
