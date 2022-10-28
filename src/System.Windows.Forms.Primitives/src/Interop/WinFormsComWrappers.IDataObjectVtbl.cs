﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using Windows.Win32.System.Com;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IDataObjectVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IDataObject.Vtbl* vtblRaw = (IDataObject.Vtbl*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IDataObject.Vtbl), sizeof(IDataObject.Vtbl));
                vtblRaw->QueryInterface_1 = (delegate* unmanaged[Stdcall]<IDataObject*, Guid*, void**, HRESULT>)fpQueryInterface;
                vtblRaw->AddRef_2 = (delegate* unmanaged[Stdcall]<IDataObject*, uint>)fpAddRef;
                vtblRaw->Release_3 = (delegate* unmanaged[Stdcall]<IDataObject*, uint>)fpRelease;
                vtblRaw->GetData_4 = &GetData;
                vtblRaw->GetDataHere_5 = &GetDataHere;
                vtblRaw->QueryGetData_6 = &QueryGetData;
                vtblRaw->GetCanonicalFormatEtc_7 = &GetCanonicalFormatEtc;
                vtblRaw->SetData_8 = &SetData;
                vtblRaw->EnumFormatEtc_9 = &EnumFormatEtc;
                vtblRaw->DAdvise_10 = &DAdvise;
                vtblRaw->DUnadvise_11 = &DUnadvise;
                vtblRaw->EnumDAdvise_12 = &EnumDAdvise;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT GetData(IDataObject* @this, FORMATETC* format, STGMEDIUM* pMedium)
            {
                var instance = ComInterfaceDispatch.GetInstance<ComTypes.IDataObject>((ComInterfaceDispatch*)@this);
                try
                {
                    instance.GetData(ref *(ComTypes.FORMATETC*)format, out var medium);
                    pMedium->pUnkForRelease = medium.pUnkForRelease == null
                        ? null
                        : (IUnknown*)(void*)Marshal.GetIUnknownForObject(medium.pUnkForRelease);
                    pMedium->tymed = (TYMED)medium.tymed;
                    pMedium->Anonymous.hGlobal = medium.unionmember;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static unsafe HRESULT GetDataHere(IDataObject* @this, FORMATETC* format, STGMEDIUM* pMedium)
            {
                if (pMedium is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<ComTypes.IDataObject>((ComInterfaceDispatch*)@this);
                try
                {
                    ComTypes.STGMEDIUM medium = new()
                    {
                        pUnkForRelease = pMedium->pUnkForRelease is null
                            ? null
                            : Marshal.GetObjectForIUnknown((nint)pMedium->pUnkForRelease),
                        tymed = (ComTypes.TYMED)pMedium->tymed,
                        unionmember = pMedium->Anonymous.hGlobal
                    };

                    instance.GetDataHere(ref *(ComTypes.FORMATETC*)format, ref medium);
                    pMedium->pUnkForRelease = medium.pUnkForRelease is null
                        ? null
                        : (IUnknown*)(void*)Marshal.GetIUnknownForObject(medium.pUnkForRelease);
                    pMedium->tymed = (TYMED)medium.tymed;
                    pMedium->Anonymous.hGlobal = medium.unionmember;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static unsafe HRESULT QueryGetData(IDataObject* @this, FORMATETC* format)
            {
                var instance = ComInterfaceDispatch.GetInstance<ComTypes.IDataObject>((ComInterfaceDispatch*)@this);
                return (HRESULT)instance.QueryGetData(ref *(ComTypes.FORMATETC*)format);
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static unsafe HRESULT GetCanonicalFormatEtc(IDataObject* @this, FORMATETC* formatIn, FORMATETC* formatOut)
            {
                var instance = ComInterfaceDispatch.GetInstance<ComTypes.IDataObject>((ComInterfaceDispatch*)@this);
                return (HRESULT)instance.GetCanonicalFormatEtc(ref *(ComTypes.FORMATETC*)formatIn, out *(ComTypes.FORMATETC*)formatOut);
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT SetData(IDataObject* @this, FORMATETC* format, STGMEDIUM* pMedium, BOOL release)
            {
                if (pMedium is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<ComTypes.IDataObject>((ComInterfaceDispatch*)@this);
                try
                {
                    ComTypes.STGMEDIUM medium = new()
                    {
                        pUnkForRelease = pMedium->pUnkForRelease is null
                            ? null
                            : Marshal.GetObjectForIUnknown((nint)pMedium->pUnkForRelease),
                        tymed = (ComTypes.TYMED)pMedium->tymed,
                        unionmember = pMedium->Anonymous.hGlobal
                    };

                    instance.SetData(ref *(ComTypes.FORMATETC*)format, ref medium, release != 0);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static HRESULT EnumFormatEtc(IDataObject* @this, uint direction, IEnumFORMATETC** pEnumFormatC)
            {
                var instance = ComInterfaceDispatch.GetInstance<ComTypes.IDataObject>((ComInterfaceDispatch*)@this);
                try
                {
                    var formatEtc = instance.EnumFormatEtc((ComTypes.DATADIR)(int)direction);
                    if (!ComHelpers.TryGetComPointer(formatEtc, IID.IEnumFORMATETC, out IEnumFORMATETC* formatEtcPtr))
                    {
                        return HRESULT.E_NOINTERFACE;
                    }

                    *pEnumFormatC = formatEtcPtr;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static unsafe HRESULT DAdvise(IDataObject* @this, FORMATETC* pFormatetc, uint advf, IAdviseSink* pAdviseSink, uint* connection)
            {
                var instance = ComInterfaceDispatch.GetInstance<ComTypes.IDataObject>((ComInterfaceDispatch*)@this);
                var adviseSink = (ComTypes.IAdviseSink)Marshal.GetObjectForIUnknown((nint)(void*)pAdviseSink);
                return (HRESULT)instance.DAdvise(ref *(ComTypes.FORMATETC*)pFormatetc, (ComTypes.ADVF)advf, adviseSink, out *(int*)connection);
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static unsafe HRESULT DUnadvise(IDataObject* @this, uint connection)
            {
                var instance = ComInterfaceDispatch.GetInstance<ComTypes.IDataObject>((ComInterfaceDispatch*)@this);
                try
                {
                    instance.DUnadvise((int)connection);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            private static unsafe HRESULT EnumDAdvise(IDataObject* @this, IEnumSTATDATA** pEnumAdvise)
            {
                *pEnumAdvise = null;

                var instance = ComInterfaceDispatch.GetInstance<ComTypes.IDataObject>((ComInterfaceDispatch*)@this);
                var result = (HRESULT)instance.EnumDAdvise(out var enumAdvice);
                if (result.Failed)
                {
                    return result;
                }

                if (!ComHelpers.TryGetComPointer(enumAdvice, IID.IEnumSTATDATA, out IEnumSTATDATA* enumAdvicePtr))
                {
                    return HRESULT.E_NOINTERFACE;
                }

                *pEnumAdvise = enumAdvicePtr;
                return result;
            }
        }
    }
}
