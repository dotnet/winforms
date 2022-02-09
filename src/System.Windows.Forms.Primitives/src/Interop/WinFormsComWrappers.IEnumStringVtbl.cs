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
        internal static class IEnumStringVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IEnumStringVtbl), IntPtr.Size * 7);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, int, IntPtr*, int*, int>)&Next;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, int, int>)&Skip;
                vtblRaw[5] = (IntPtr)(delegate* unmanaged<IntPtr, int>)&Reset;
                vtblRaw[6] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, int>)&Clone;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static int Next(IntPtr thisPtr, int celt, IntPtr* rgelt, int* pceltFetched)
            {
                IEnumString inst = ComInterfaceDispatch.GetInstance<IEnumString>((ComInterfaceDispatch*)thisPtr);
                string[] elt = new string[celt];
                var result = inst.Next(celt, elt, (IntPtr)pceltFetched);
                for (var i = 0; i < *pceltFetched; i++)
                {
                    rgelt[i] = Marshal.StringToCoTaskMemUni(elt[i]);
                }

                return result;
            }

            [UnmanagedCallersOnly]
            private static int Skip(IntPtr thisPtr, int celt)
            {
                IEnumString inst = ComInterfaceDispatch.GetInstance<IEnumString>((ComInterfaceDispatch*)thisPtr);
                return inst.Skip(celt);
            }

            [UnmanagedCallersOnly]
            private static int Reset(IntPtr thisPtr)
            {
                try
                {
                    IEnumString inst = ComInterfaceDispatch.GetInstance<IEnumString>((ComInterfaceDispatch*)thisPtr);
                    inst.Reset();
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }

                return S_OK;
            }

            [UnmanagedCallersOnly]
            private static int Clone(IntPtr thisPtr, IntPtr* ppenum)
            {
                try
                {
                    IEnumString inst = ComInterfaceDispatch.GetInstance<IEnumString>((ComInterfaceDispatch*)thisPtr);
                    inst.Clone(out var cloned);
                    *ppenum = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(cloned, CreateComInterfaceFlags.None);
                }
                catch (Exception ex)
                {
                    return ex.HResult;
                }

                return S_OK;
            }
        }
    }
}
