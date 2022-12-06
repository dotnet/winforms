// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ComWrappers = Interop.WinFormsComWrappers;

namespace Windows.Win32.System.Com;

internal unsafe partial struct IEnumFORMATETC : IVTable<IEnumFORMATETC, IEnumFORMATETC.Vtbl>
{
    static void IVTable<IEnumFORMATETC, Vtbl>.PopulateComInterfaceVTable(Vtbl* vtable)
    {
        vtable->Next_4 = &Next;
        vtable->Skip_5 = &Skip;
        vtable->Reset_6 = &Reset;
        vtable->Clone_7 = &Clone;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT Next(IEnumFORMATETC* @this, uint celt, FORMATETC* rgelt, uint* pceltFetched)
        => ComWrappers.UnwrapAndInvoke<IEnumFORMATETC, Interface>(@this, o => o.Next(celt, rgelt, pceltFetched));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT Skip(IEnumFORMATETC* @this, uint celt)
        => ComWrappers.UnwrapAndInvoke<IEnumFORMATETC, Interface>(@this, o => o.Skip(celt));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT Reset(IEnumFORMATETC* @this)
        => ComWrappers.UnwrapAndInvoke<IEnumFORMATETC, Interface>(@this, o => o.Reset());

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT Clone(IEnumFORMATETC* @this, IEnumFORMATETC** ppenum)
        => ComWrappers.UnwrapAndInvoke<IEnumFORMATETC, Interface>(@this, o => o.Clone(ppenum));
}
