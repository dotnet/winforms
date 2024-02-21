// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using ComType = System.Runtime.InteropServices.ComTypes;

namespace Windows.Win32.System.Com;

internal unsafe partial struct STATDATA
{
    /// <summary>
    ///  Converts <see cref="STATDATA"/> to <see cref="ComType.STATDATA"/>
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This will release the <see cref="pAdvSink"/> of the <see cref="STATDATA"/> that was passed in
    ///   so that the runtime type will have ownership.
    ///  </para>
    /// </remarks>
    public static ComType.STATDATA ConvertToRuntimeStatData(STATDATA statData)
    {
        using ComScope<IAdviseSink> pAdvSink = new(statData.pAdvSink);
        ComType.STATDATA result = new()
        {
            formatetc = Unsafe.As<FORMATETC, ComType.FORMATETC>(ref statData.formatetc),
            advf = (ComType.ADVF)statData.advf,
            advSink = ComHelpers.TryGetObjectForIUnknown(
                pAdvSink.Query<IUnknown>(),
                out ComType.IAdviseSink? adviseSink)
                    ? adviseSink
                    : new AdviseSinkWrapper(pAdvSink),
            connection = (int)statData.dwConnection
        };

        return result;
    }
}
