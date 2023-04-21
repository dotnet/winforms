﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using ComType = System.Runtime.InteropServices.ComTypes;

namespace Windows.Win32.System.Com;

internal unsafe partial struct STGMEDIUM
{
    [UnscopedRef]
    public ref nint hGlobal => ref Anonymous.hGlobal;

    public static explicit operator STGMEDIUM(ComType.STGMEDIUM comTypeStg)
    {
        IUnknown* pUnkForRelease = ComHelpers.TryGetComPointer<IUnknown>(comTypeStg.pUnkForRelease, out HRESULT hr);

        return new()
        {
            pUnkForRelease = pUnkForRelease,
            tymed = (TYMED)comTypeStg.tymed,
            Anonymous = new()
            {
                hGlobal = comTypeStg.unionmember
            }
        };
    }

    public static explicit operator ComType.STGMEDIUM(STGMEDIUM stg) => new()
    {
        pUnkForRelease = stg.pUnkForRelease is null
            ? null
            : Marshal.GetObjectForIUnknown((nint)stg.pUnkForRelease),
        tymed = (ComType.TYMED)stg.tymed,
        unionmember = stg.Anonymous.hGlobal
    };
}
