// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IEnumFORMATETCVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IEnumFORMATETCVtbl), IntPtr.Size * 7);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, int, FORMATETC*, int*, int>)&Next;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, int, int>)&Skip;
                vtblRaw[5] = (IntPtr)(delegate* unmanaged<IntPtr, int>)&Reset;
                vtblRaw[6] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, int>)&Clone;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static int Next(IntPtr thisPtr, int celt, FORMATETC* rgelt, int* pceltFetched)
            {
                try
                {
                    IEnumFORMATETC instance = ComInterfaceDispatch.GetInstance<IEnumFORMATETC>((ComInterfaceDispatch*)thisPtr);
                    FORMATETC[] elt = new FORMATETC[celt];
                    int[] celtFetched = new int[1];
                    var result = instance.Next(celt, elt, pceltFetched is null ? null! : celtFetched);
                    for (var i = 0; i < celt; i++)
                    {
                        rgelt[i] = elt[i];
                    }

                    if (pceltFetched is not null)
                    {
                        *pceltFetched = celtFetched[0];
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int Skip(IntPtr thisPtr, int celt)
            {
                IEnumFORMATETC instance = ComInterfaceDispatch.GetInstance<IEnumFORMATETC>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    return instance.Skip(celt);
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int Reset(IntPtr thisPtr)
            {
                try
                {
                    IEnumFORMATETC instance = ComInterfaceDispatch.GetInstance<IEnumFORMATETC>((ComInterfaceDispatch*)thisPtr);
                    instance.Reset();
                    return S_OK;
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static int Clone(IntPtr thisPtr, IntPtr* ppenum)
            {
                try
                {
                    IEnumFORMATETC instance = ComInterfaceDispatch.GetInstance<IEnumFORMATETC>((ComInterfaceDispatch*)thisPtr);
                    instance.Clone(out var cloned);
                    *ppenum = WinFormsComWrappers.Instance.GetComPointer(cloned, IID.IEnumFORMATETC);
                    return S_OK;
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }
            }
        }
    }
}
