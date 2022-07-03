// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using static Interop.Oleaut32;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IEnumVariantVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IEnumStringVtbl), IntPtr.Size * 7);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, uint, IntPtr, uint*, HRESULT>)&Next;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, uint, HRESULT>)&Skip;
                vtblRaw[5] = (IntPtr)(delegate* unmanaged<IntPtr, HRESULT>)&Reset;
                vtblRaw[6] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&Clone;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static HRESULT Next(IntPtr thisPtr, uint celt, IntPtr rgelt, uint* pceltFetched)
            {
                try
                {
                    var instance = ComInterfaceDispatch.GetInstance<IEnumVariant>((ComInterfaceDispatch*)thisPtr);
                    var result = instance.Next(celt, rgelt, pceltFetched);
                    return result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT Skip(IntPtr thisPtr, uint celt)
            {
                try
                {
                    var instance = ComInterfaceDispatch.GetInstance<IEnumVariant>((ComInterfaceDispatch*)thisPtr);
                    return instance.Skip(celt);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT Reset(IntPtr thisPtr)
            {
                try
                {
                    var instance = ComInterfaceDispatch.GetInstance<IEnumVariant>((ComInterfaceDispatch*)thisPtr);
                    return instance.Reset();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT Clone(IntPtr thisPtr, IntPtr* ppenum)
            {
                if (ppenum is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                try
                {
                    var instance = ComInterfaceDispatch.GetInstance<IEnumVariant>((ComInterfaceDispatch*)thisPtr);
                    IEnumVariant[] cloned = new IEnumVariant[1];
                    var result = instance.Clone(cloned);
                    *ppenum = cloned[0] is null ? IntPtr.Zero : WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(cloned[0], CreateComInterfaceFlags.None);
                    return result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }
        }
    }
}
