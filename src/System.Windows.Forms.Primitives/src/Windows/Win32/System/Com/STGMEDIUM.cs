// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using ComType = System.Runtime.InteropServices.ComTypes;

namespace Windows.Win32.System.Com;

internal unsafe partial struct STGMEDIUM
{
    [UnscopedRef]
    public ref HGLOBAL hGlobal => ref u.hGlobal;

    public static explicit operator STGMEDIUM(ComType.STGMEDIUM comTypeStg)
    {
        IUnknown* pUnkForRelease = ComHelpers.TryGetComPointer<IUnknown>(comTypeStg.pUnkForRelease, out _);

        return new()
        {
            pUnkForRelease = pUnkForRelease,
            tymed = (TYMED)comTypeStg.tymed,
            u = new()
            {
                hGlobal = (HGLOBAL)comTypeStg.unionmember
            }
        };
    }

    public static explicit operator ComType.STGMEDIUM(STGMEDIUM stg) => new()
    {
        pUnkForRelease = stg.pUnkForRelease is null
            ? null
            : Marshal.GetObjectForIUnknown((nint)stg.pUnkForRelease),
        tymed = (ComType.TYMED)stg.tymed,
        unionmember = stg.u.hGlobal
    };
}
