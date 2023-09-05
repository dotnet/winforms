// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices.Marshalling;
using Windows.Win32.System.Com;

namespace Windows.Win32.Foundation;

internal static unsafe partial class GlobalInterfaceTable
{
    /// <summary>
    ///  Strategy for <see cref="StrategyBasedComWrappers"/> that uses the <see cref="GlobalInterfaceTable"/>.
    /// </summary>
    private unsafe class UnknownStrategy : IIUnknownStrategy
    {
        private uint _cookie;

        void* IIUnknownStrategy.CreateInstancePointer(void* unknown)
        {
            Debug.Assert(_cookie == 0, "A cookie has already been generated for this instance.");
            _cookie = RegisterInterface((IUnknown*)unknown);
            return unknown;
        }

        int IIUnknownStrategy.QueryInterface(void* instancePtr, in Guid iid, out void* ppObj)
            => s_globalInterfaceTable->GetInterfaceFromGlobal(_cookie, iid, out ppObj);

        int IIUnknownStrategy.Release(void* instancePtr)
        {
            uint cookie = Interlocked.Exchange(ref _cookie, 0);
            return cookie != 0 ? (int)HRESULT.S_OK : (int)RevokeInterface(_cookie);
        }
    }
}
