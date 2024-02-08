// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ComType = System.Runtime.InteropServices.ComTypes;

namespace Windows.Win32.System.Com;

internal unsafe partial struct FORMATETC
{
    public static implicit operator FORMATETC(ComType.FORMATETC formatEtc) =>
        new()
        {
            cfFormat = (ushort)formatEtc.cfFormat,
            ptd = (DVTARGETDEVICE*)formatEtc.ptd,
            dwAspect = (uint)formatEtc.dwAspect,
            lindex = formatEtc.lindex,
            tymed = (uint)formatEtc.tymed
        };

    public static implicit operator ComType.FORMATETC(FORMATETC formatEtc) =>
        new()
        {
            cfFormat = (short)formatEtc.cfFormat,
            ptd = (nint)formatEtc.ptd,
            dwAspect = (ComType.DVASPECT)formatEtc.dwAspect,
            lindex = formatEtc.lindex,
            tymed = (ComType.TYMED)formatEtc.tymed
        };
}
