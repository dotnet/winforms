// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("00000112-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleObject
        {
            [PreserveSig]
            HRESULT SetClientSite(
                IOleClientSite pClientSite);

            [PreserveSig]
            HRESULT GetClientSite(
                out IOleClientSite ppClientSite);

            [PreserveSig]
            HRESULT SetHostNames(
                [MarshalAs(UnmanagedType.LPWStr)] string szContainerApp,
                [MarshalAs(UnmanagedType.LPWStr)] string szContainerObj);

            [PreserveSig]
            HRESULT Close(
                OLECLOSE dwSaveOption);

            [PreserveSig]
            HRESULT SetMoniker(
                OLEWHICHMK dwWhichMoniker,
                [MarshalAs(UnmanagedType.Interface)] object pmk);

            [PreserveSig]
            HRESULT GetMoniker(
                OLEGETMONIKER dwAssign,
                OLEWHICHMK dwWhichMoniker,
                IntPtr* ppmk);

            [PreserveSig]
            HRESULT InitFromData(
                IDataObject pDataObject,
                BOOL fCreation,
                uint dwReserved);

            [PreserveSig]
            HRESULT GetClipboardData(
                uint dwReserved,
                out IDataObject ppDataObject);

            [PreserveSig]
            HRESULT DoVerb(
                OLEIVERB iVerb,
                User32.MSG* lpmsg,
                IOleClientSite pActiveSite,
                int lindex,
                IntPtr hwndParent,
                RECT* lprcPosRect);

            [PreserveSig]
            HRESULT EnumVerbs(
                out IEnumOLEVERB ppEnumOleVerb);

            [PreserveSig]
            HRESULT OleUpdate();

            [PreserveSig]
            HRESULT IsUpToDate();

            [PreserveSig]
            HRESULT GetUserClassID(
                Guid* pClsid);

            [PreserveSig]
            HRESULT GetUserType(
                USERCLASSTYPE dwFormOfType,
                [MarshalAs(UnmanagedType.LPWStr)] out string userType);

            [PreserveSig]
            HRESULT SetExtent(
                DVASPECT dwDrawAspect,
                Size* pSizel);

            [PreserveSig]
            HRESULT GetExtent(
                DVASPECT dwDrawAspect,
                Size* pSizel);

            [PreserveSig]
            HRESULT Advise(
                IAdviseSink pAdvSink,
                uint* pdwConnection);

            [PreserveSig]
            HRESULT Unadvise(
                uint dwConnection);

            [PreserveSig]
            HRESULT EnumAdvise(
                out IEnumSTATDATA e);

            [PreserveSig]
            HRESULT GetMiscStatus(
                DVASPECT dwAspect,
                OLEMISC* pdwStatus);

            [PreserveSig]
            HRESULT SetColorScheme(
                Gdi32.LOGPALETTE* pLogpal);
        }
    }
}
