// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ComType = System.Runtime.InteropServices.ComTypes;

namespace Windows.Win32.System.Com;

internal unsafe partial struct STATDATA
{
    public static implicit operator STATDATA(ComType.STATDATA statData) =>
        new()
        {
            formatetc = statData.formatetc,
            advf = (uint)statData.advf,
            pAdvSink = ComHelpers.GetComPointer<IAdviseSink>(statData.advSink),
            dwConnection = (uint)statData.connection
        };
}
