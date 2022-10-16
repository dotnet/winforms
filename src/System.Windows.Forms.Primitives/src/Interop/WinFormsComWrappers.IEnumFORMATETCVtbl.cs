// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IEnumFORMATETCVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                Com.IEnumFORMATETC.Vtbl* vtblRaw = (Com.IEnumFORMATETC.Vtbl*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IEnumFORMATETCVtbl), sizeof(Com.IEnumFORMATETC.Vtbl));
                vtblRaw->QueryInterface_1 = (delegate* unmanaged[Stdcall]<Com.IEnumFORMATETC*, Guid*, void**, HRESULT>)fpQueryInterface;
                vtblRaw->AddRef_2 = (delegate* unmanaged[Stdcall]<Com.IEnumFORMATETC*, uint>)fpAddRef;
                vtblRaw->Release_3 = (delegate* unmanaged[Stdcall]<Com.IEnumFORMATETC*, uint>)fpRelease;
                vtblRaw->Next_4 = &Next;
                vtblRaw->Skip_5 = &Skip;
                vtblRaw->Reset_6 = &Reset;
                vtblRaw->Clone_7 = &Clone;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Next(Com.IEnumFORMATETC* @this, uint celt, Com.FORMATETC* rgelt, uint* pceltFetched)
            {
                try
                {
                    IEnumFORMATETC instance = ComInterfaceDispatch.GetInstance<IEnumFORMATETC>((ComInterfaceDispatch*)@this);
                    FORMATETC[] elt = new FORMATETC[celt];
                    int[] celtFetched = new int[1];

                    // Eliminate null bang after https://github.com/dotnet/runtime/pull/68537 lands, or 
                    // IEnumFORMATETC annotations would be corrected.
                    var result = instance.Next((int)celt, elt, pceltFetched is null ? null! : celtFetched);
                    for (var i = 0; i < celt; i++)
                    {
                        rgelt[i] = elt[i];
                    }

                    if (pceltFetched is not null)
                    {
                        *pceltFetched = (uint)celtFetched[0];
                    }

                    return (HRESULT)result;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Skip(Com.IEnumFORMATETC* @this, uint celt)
            {
                IEnumFORMATETC instance = ComInterfaceDispatch.GetInstance<IEnumFORMATETC>((ComInterfaceDispatch*)@this);
                try
                {
                    return (HRESULT)instance.Skip((int)celt);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Reset(Com.IEnumFORMATETC* @this)
            {
                try
                {
                    IEnumFORMATETC instance = ComInterfaceDispatch.GetInstance<IEnumFORMATETC>((ComInterfaceDispatch*)@this);
                    instance.Reset();
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Clone(Com.IEnumFORMATETC* @this, Com.IEnumFORMATETC** ppenum)
            {
                try
                {
                    IEnumFORMATETC instance = ComInterfaceDispatch.GetInstance<IEnumFORMATETC>((ComInterfaceDispatch*)@this);
                    instance.Clone(out var cloned);
                    *ppenum = null;

                    if (Instance.TryGetComPointer(cloned, IID.IEnumFORMATETC, out Com.IEnumFORMATETC* p))
                    {
                        *ppenum = p;
                        return HRESULT.S_OK;
                    }

                    Debug.Fail("Why couldn't we get a COM pointer for the cloned object?");
                    return HRESULT.E_FAIL;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }
    }
}
