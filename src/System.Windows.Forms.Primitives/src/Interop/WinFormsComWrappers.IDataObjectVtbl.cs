// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
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
                var instance = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    instance.GetData(ref *format, out var medium);
                    pMedium->pUnkForRelease = medium.pUnkForRelease == null ? IntPtr.Zero : Marshal.GetIUnknownForObject(medium.pUnkForRelease);
                    pMedium->tymed = medium.tymed;
                    pMedium->unionmember = medium.unionmember;
                    return HRESULT.Values.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT GetDataHere(IntPtr thisPtr, FORMATETC* format, STGMEDIUM_Raw* pMedium)
            {
                var instance = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    STGMEDIUM medium = new()
                    {
                        pUnkForRelease = pMedium->pUnkForRelease == IntPtr.Zero ? null : Marshal.GetObjectForIUnknown(pMedium->pUnkForRelease),
                        tymed = pMedium->tymed,
                        unionmember = pMedium->unionmember,
                    };

                    instance.GetDataHere(ref *format, ref medium);
                    pMedium->pUnkForRelease = medium.pUnkForRelease == null ? IntPtr.Zero : Marshal.GetIUnknownForObject(medium.pUnkForRelease);
                    pMedium->tymed = medium.tymed;
                    pMedium->unionmember = medium.unionmember;
                    return HRESULT.Values.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT QueryGetData(IntPtr thisPtr, FORMATETC* format)
            {
                var instance = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                return (HRESULT)instance.QueryGetData(ref *format);
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT GetCanonicalFormatEtc(IntPtr thisPtr, FORMATETC* formatIn, FORMATETC* formatOut)
            {
                var instance = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                return (HRESULT)instance.GetCanonicalFormatEtc(ref *formatIn, out *formatOut);
            }

            [UnmanagedCallersOnly]
            private static HRESULT SetData(IntPtr thisPtr, FORMATETC* format, STGMEDIUM_Raw* pMedium, int release)
            {
                var instance = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    STGMEDIUM medium = new()
                    {
                        pUnkForRelease = pMedium->pUnkForRelease == IntPtr.Zero ? null : Marshal.GetObjectForIUnknown(pMedium->pUnkForRelease),
                        tymed = pMedium->tymed,
                        unionmember = pMedium->unionmember,
                    };

                    instance.SetData(ref *format, ref medium, release != 0);
                    return HRESULT.Values.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT EnumFormatEtc(IntPtr thisPtr, DATADIR direction, IntPtr* pEnumFormatC)
            {
                var instance = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var formatEtc = instance.EnumFormatEtc(direction);
                    var result = WinFormsComWrappers.Instance.TryGetComPointer(formatEtc, IID.IEnumFORMATETC, out var formatEtcPtr);
                    if (result.Failed)
                    {
                        return result;
                    }

                    *pEnumFormatC = formatEtcPtr;
                    return HRESULT.Values.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT DAdvise(IntPtr thisPtr, FORMATETC* pFormatetc, ADVF advf, IntPtr pAdviseSink, int* connection)
            {
                var instance = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                var adviseSink = (IAdviseSink)Marshal.GetObjectForIUnknown(pAdviseSink);
                return (HRESULT)instance.DAdvise(ref *pFormatetc, advf, adviseSink, out *connection);
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT DUnadvise(IntPtr thisPtr, int connection)
            {
                var instance = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    instance.DUnadvise(connection);
                    return HRESULT.Values.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT EnumDAdvise(IntPtr thisPtr, IntPtr* pEnumAdvise)
            {
                var instance = ComInterfaceDispatch.GetInstance<IDataObject>((ComInterfaceDispatch*)thisPtr);
                var result = (HRESULT)instance.EnumDAdvise(out var enumAdvice);
                if (result.Failed)
                {
                    return result;
                }

                result = WinFormsComWrappers.Instance.TryGetComPointer(enumAdvice, IID.IEnumSTATDATA, out var enumAdvicePtr);
                return result;
            }

            internal struct STGMEDIUM_Raw
            {
                public TYMED tymed;
                public IntPtr unionmember;
                public IntPtr pUnkForRelease;
            }
        }
    }
}
