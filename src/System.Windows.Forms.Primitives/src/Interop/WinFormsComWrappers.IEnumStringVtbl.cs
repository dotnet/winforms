// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IEnumStringVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                Com.IEnumString.Vtbl* vtblRaw = (Com.IEnumString.Vtbl*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IEnumStringVtbl), sizeof(Com.IEnumString.Vtbl));
                vtblRaw->QueryInterface_1 = (delegate* unmanaged[Stdcall]<Com.IEnumString*, Guid*, void**, HRESULT>)fpQueryInterface;
                vtblRaw->AddRef_2 = (delegate* unmanaged[Stdcall]<Com.IEnumString*, uint>)fpAddRef;
                vtblRaw->Release_3 = (delegate* unmanaged[Stdcall]<Com.IEnumString*, uint>)fpRelease;
                vtblRaw->Next_4 = &Next;
                vtblRaw->Skip_5 = &Skip;
                vtblRaw->Reset_6 = &Reset;
                vtblRaw->Clone_7 = &Clone;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Next(Com.IEnumString* @this, uint celt, PWSTR* rgelt, uint* pceltFetched)
            {
                try
                {
                    IEnumString instance = ComInterfaceDispatch.GetInstance<IEnumString>((ComInterfaceDispatch*)@this);
                    string[] elt = new string[celt];
                    var result = instance.Next((int)celt, elt, (nint)pceltFetched);
                    for (var i = 0; i < *pceltFetched; i++)
                    {
                        rgelt[i] = (char*)Marshal.StringToCoTaskMemUni(elt[i]);
                    }

                    return (HRESULT)result;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Skip(Com.IEnumString* @this, uint celt)
            {
                try
                {
                    IEnumString instance = ComInterfaceDispatch.GetInstance<IEnumString>((ComInterfaceDispatch*)@this);
                    return (HRESULT)instance.Skip((int)celt);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Reset(Com.IEnumString* @this)
            {
                try
                {
                    IEnumString instance = ComInterfaceDispatch.GetInstance<IEnumString>((ComInterfaceDispatch*)@this);
                    instance.Reset();
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT Clone(Com.IEnumString* @this, Com.IEnumString** ppenum)
            {
                try
                {
                    IEnumString instance = ComInterfaceDispatch.GetInstance<IEnumString>((ComInterfaceDispatch*)@this);
                    instance.Clone(out var cloned);
                    *ppenum = (Com.IEnumString*)WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(cloned, CreateComInterfaceFlags.None);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }
    }
}
