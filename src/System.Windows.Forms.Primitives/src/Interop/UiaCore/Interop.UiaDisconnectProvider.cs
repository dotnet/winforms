// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
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
                // E_INVALIDARG error result is for several reasons. The most common is that the `provider` argument is null.
                // S_FALSE indicates that the same object is disconnected again.
                HRESULT result = UiaDisconnectProviderInternal(provider);
                Debug.Assert(result == HRESULT.S_OK);
            }
        }
    }
}
