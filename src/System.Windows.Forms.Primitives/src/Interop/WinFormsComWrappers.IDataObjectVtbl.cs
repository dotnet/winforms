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
        internal static class IDataObjectVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IDataObjectVtbl), IntPtr.Size * 12);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, FORMATETC*, STGMEDIUM_Raw*, HRESULT>)&GetData;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, FORMATETC*, STGMEDIUM_Raw*, HRESULT>)&GetDataHere;
                vtblRaw[5] = (IntPtr)(delegate* unmanaged<IntPtr, FORMATETC*, HRESULT>)&QueryGetData;
                vtblRaw[6] = (IntPtr)(delegate* unmanaged<IntPtr, FORMATETC*, FORMATETC*, HRESULT>)&GetCanonicalFormatEtc;
                vtblRaw[7] = (IntPtr)(delegate* unmanaged<IntPtr, FORMATETC*, STGMEDIUM_Raw*, int, HRESULT>)&SetData;
                vtblRaw[8] = (IntPtr)(delegate* unmanaged<IntPtr, DATADIR, IntPtr*, HRESULT>)&EnumFormatEtc;
                vtblRaw[9] = (IntPtr)(delegate* unmanaged<IntPtr, FORMATETC*, ADVF, IntPtr, int*, HRESULT>)&DAdvise;
                vtblRaw[10] = (IntPtr)(delegate* unmanaged<IntPtr, int, HRESULT>)&DUnadvise;
                vtblRaw[11] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&EnumDAdvise;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static HRESULT GetData(IntPtr thisPtr, FORMATETC* format, STGMEDIUM_Raw* pMedium)
            {
                var inst = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    inst.GetData(ref *format, out var medium);
                    pMedium->pUnkForRelease = medium.pUnkForRelease == null ? IntPtr.Zero : Marshal.GetIUnknownForObject(medium.pUnkForRelease);
                    pMedium->tymed = medium.tymed;
                    pMedium->unionmember = medium.unionmember;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT GetDataHere(IntPtr thisPtr, FORMATETC* format, STGMEDIUM_Raw* pMedium)
            {
                var inst = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    STGMEDIUM medium = new();
                    medium.pUnkForRelease = Marshal.GetObjectForIUnknown(pMedium->pUnkForRelease);
                    medium.tymed = pMedium->tymed;
                    medium.unionmember = pMedium->unionmember;
                    inst.GetDataHere(ref *format, ref medium);
                    pMedium->pUnkForRelease = medium.pUnkForRelease == null ? IntPtr.Zero : Marshal.GetIUnknownForObject(medium.pUnkForRelease);
                    pMedium->tymed = medium.tymed;
                    pMedium->unionmember = medium.unionmember;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT QueryGetData(IntPtr thisPtr, FORMATETC* format)
            {
                var inst = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                return (HRESULT)inst.QueryGetData(ref *format);
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT GetCanonicalFormatEtc(IntPtr thisPtr, FORMATETC* formatIn, FORMATETC* formatOut)
            {
                var inst = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                return (HRESULT)inst.GetCanonicalFormatEtc(ref *formatIn, out *formatOut);
            }

            [UnmanagedCallersOnly]
            private static HRESULT SetData(IntPtr thisPtr, FORMATETC* format, STGMEDIUM_Raw* pMedium, int release)
            {
                var inst = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    STGMEDIUM medium = new();
                    medium.pUnkForRelease = Marshal.GetObjectForIUnknown(pMedium->pUnkForRelease);
                    medium.tymed = pMedium->tymed;
                    medium.unionmember = pMedium->unionmember;
                    inst.SetData(ref *format, ref medium, release != 0);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT EnumFormatEtc(IntPtr thisPtr, DATADIR direction, IntPtr* pEnumFormatC)
            {
                var inst = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var formatEtc = inst.EnumFormatEtc(direction);
                    var formatEtcPtr = Marshal.GetIUnknownForObject(formatEtc);
                    if (formatEtcPtr != IntPtr.Zero)
                    {
                        var iid = IID.IEnumFORMATETC;
                        var result = (HRESULT)Marshal.QueryInterface(formatEtcPtr, ref iid, out formatEtcPtr);
                        if (result.Failed())
                        {
                            return result;
                        }
                    }

                    *pEnumFormatC = formatEtcPtr;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT DAdvise(IntPtr thisPtr, FORMATETC* pFormatetc, ADVF advf, IntPtr pAdviseSink, int* connection)
            {
                var inst = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                var adviseSink = (IAdviseSink)Marshal.GetObjectForIUnknown(pAdviseSink);
                return (HRESULT)inst.DAdvise(ref *pFormatetc, advf, adviseSink, out *connection);
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT DUnadvise(IntPtr thisPtr, int connection)
            {
                var inst = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    inst.DUnadvise(connection);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT EnumDAdvise(IntPtr thisPtr, IntPtr* pEnumAdvise)
            {
                var inst = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                var result = (HRESULT)inst.EnumDAdvise(out var enumAdvice);
                if (enumAdvice == null)
                {
                    *pEnumAdvise = IntPtr.Zero;
                }
                else
                {
                    var enumAdvicePtr = Marshal.GetIUnknownForObject(enumAdvice);
                    var iid = IID.IEnumSTATDATA;
                    result = (HRESULT)Marshal.QueryInterface(enumAdvicePtr, ref iid, out enumAdvicePtr);
                    *pEnumAdvise = enumAdvicePtr;
                }                

                return result;
            }

            internal struct STGMEDIUM_Raw
            {
                public IntPtr pUnkForRelease;
                public TYMED tymed;
                public IntPtr unionmember;
            }
        }
    }
}
