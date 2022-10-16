// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ComType = System.Runtime.InteropServices.ComTypes;

namespace Windows.Win32.System.Com
{
    internal partial struct FORMATETC
    {
        public static unsafe implicit operator FORMATETC(ComType.FORMATETC formatetc) =>
            new()
            {
                cfFormat = (ushort)formatetc.cfFormat,
                ptd = (DVTARGETDEVICE*)formatetc.ptd,
                dwAspect = (uint)formatetc.dwAspect,
                lindex = formatetc.lindex,
                tymed = (uint)formatetc.tymed
            };
    }
}
