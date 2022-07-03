// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Interop.Kernel32;
using static Interop.Oleaut32;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IDispatchVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IEnumStringVtbl), IntPtr.Size * 7);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, uint*, HRESULT >)&GetTypeInfoCount;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, uint, LCID, IntPtr*, HRESULT>)&GetTypeInfo;
                vtblRaw[5] = (IntPtr)(delegate* unmanaged<IntPtr, Guid*, IntPtr*, uint, LCID, Ole32.DispatchID*, HRESULT>)&GetIDsOfNames;
                vtblRaw[6] = (IntPtr)(delegate* unmanaged<IntPtr, Ole32.DispatchID, Guid*, LCID, DISPATCH, Oleaut32.DISPPARAMS*, IntPtr*, Oleaut32.EXCEPINFO*, uint*, HRESULT>)&Invoke;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static HRESULT GetTypeInfoCount(IntPtr thisPtr, uint* pctinfo)
            {
                return HRESULT.E_NOTIMPL;
            }

            [UnmanagedCallersOnly]
            private static HRESULT GetTypeInfo(IntPtr thisPtr, uint iTInfo, LCID lcid, IntPtr* ppTInfo)
            {
                return HRESULT.E_NOTIMPL;
            }

            [UnmanagedCallersOnly]
            private static HRESULT GetIDsOfNames(
                IntPtr thisPtr,
                Guid* riid,
                IntPtr* rgszNames,
                uint cNames,
                LCID lcid,
                Ole32.DispatchID* rgDispId)
            {
                return HRESULT.E_NOTIMPL;
            }

            [UnmanagedCallersOnly]
            private static HRESULT Invoke(
                IntPtr thisPtr,
                Ole32.DispatchID dispIdMember,
                Guid* riid,
                LCID lcid,
                DISPATCH dwFlags,
                DISPPARAMS* pDispParams,
                IntPtr* pVarResult,
                EXCEPINFO* pExcepInfo,
                uint* pArgErr)
            {
                return HRESULT.E_NOTIMPL;
            }
        }
    }
}
