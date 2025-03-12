// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.System.Com;

internal unsafe partial struct IUnknown
{
    // https://github.com/microsoft/CsWin32/issues/724
    internal interface Interface
    {
        internal unsafe HRESULT QueryInterface(Guid* riid, void** ppvObject);
        internal uint AddRef();
        internal uint Release();
    }
}
