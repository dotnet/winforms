// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ComWrappers = Interop.WinFormsComWrappers;

namespace Windows.Win32.System.Com;

internal unsafe partial struct ISequentialStream : IVTable<ISequentialStream, ISequentialStream.Vtbl>
{
    static void IVTable<ISequentialStream, Vtbl>.PopulateComInterfaceVTable(Vtbl* vtable)
    {
        vtable->Read_4 = &Read;
        vtable->Write_5 = &Write;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT Read(ISequentialStream* @this, void* pv, uint cb, uint* pcbRead)
            => ComWrappers.UnwrapAndInvoke<ISequentialStream, Interface>(@this, o => o.Read(pv, cb, pcbRead));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT Write(ISequentialStream* @this, void* pv, uint cb, uint* pcbWritten)
        => ComWrappers.UnwrapAndInvoke<ISequentialStream, Interface>(@this, o => o.Write(pv, cb, pcbWritten));
}
