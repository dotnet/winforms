﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    internal unsafe partial class ComposedDataObject
    {
        /// <summary>
        ///  Maps native pointer <see cref="Com.IDataObject"/> to <see cref="ComTypes.IDataObject"/>.
        /// </summary>
        private class NativeDataObjectToRuntimeAdapter : ComTypes.IDataObject
        {
            private readonly AgileComPointer<Com.IDataObject> _nativeDataObject;

            public NativeDataObjectToRuntimeAdapter(Com.IDataObject* dataObject)
            {
#if DEBUG
                _nativeDataObject = new(dataObject, takeOwnership: true, trackDisposal: false);
#else
                _nativeDataObject = new(dataObject, takeOwnership: true);
#endif
            }

            int ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
            {
                using var nativeAdviseSink = ComHelpers.TryGetComScope<Com.IAdviseSink>(adviseSink);
                fixed (Com.FORMATETC* nativeFormat = &Unsafe.As<FORMATETC, Com.FORMATETC>(ref pFormatetc))
                fixed (int* pConnection = &connection)
                {
                    using var nativeDataObject = _nativeDataObject.GetInterface();
                    return nativeDataObject.Value->DAdvise(nativeFormat, (uint)advf, nativeAdviseSink, (uint*)pConnection);
                }
            }

            void ComTypes.IDataObject.DUnadvise(int connection)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                nativeDataObject.Value->DUnadvise((uint)connection).ThrowOnFailure();
            }

            int ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA? enumAdvise)
            {
                using ComScope<Com.IEnumSTATDATA> nativeStatData = new(null);
                using var nativeDataObject = _nativeDataObject.GetInterface();
                HRESULT result = nativeDataObject.Value->EnumDAdvise(nativeStatData);
                ComHelpers.TryGetObjectForIUnknown(nativeStatData.AsUnknown, out enumAdvise);
                return result;
            }

            IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(DATADIR direction)
            {
                using ComScope<Com.IEnumFORMATETC> nativeFormat = new(null);
                using var nativeDataObject = _nativeDataObject.GetInterface();
                if (nativeDataObject.Value->EnumFormatEtc((uint)direction, nativeFormat).Failed)
                {
                    throw new ExternalException(SR.ExternalException, (int)HRESULT.E_NOTIMPL);
                }

                return (IEnumFORMATETC)ComHelpers.GetObjectForIUnknown(nativeFormat);
            }

            int ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                HRESULT result = nativeDataObject.Value->GetCanonicalFormatEtc(Unsafe.As<FORMATETC, Com.FORMATETC>(ref formatIn), out Com.FORMATETC nativeFormat);
                formatOut = Unsafe.As<Com.FORMATETC, FORMATETC>(ref nativeFormat);
                return result;
            }

            void ComTypes.IDataObject.GetData(ref FORMATETC format, out STGMEDIUM medium)
            {
                Com.FORMATETC nativeFormat = Unsafe.As<FORMATETC, Com.FORMATETC>(ref format);
                Com.STGMEDIUM nativeMedium = default;
                using var nativeDataObject = _nativeDataObject.GetInterface();
                nativeDataObject.Value->GetData(&nativeFormat, &nativeMedium).ThrowOnFailure();
                medium = (STGMEDIUM)nativeMedium;
                nativeMedium.ReleaseUnknown();
            }

            void ComTypes.IDataObject.GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
            {
                Com.FORMATETC nativeFormat = Unsafe.As<FORMATETC, Com.FORMATETC>(ref format);
                Com.STGMEDIUM nativeMedium = (Com.STGMEDIUM)medium;
                using var nativeDataObject = _nativeDataObject.GetInterface();
                nativeDataObject.Value->GetDataHere(&nativeFormat, &nativeMedium).ThrowOnFailure();
                medium = (STGMEDIUM)nativeMedium;
                nativeMedium.ReleaseUnknown();
            }

            int ComTypes.IDataObject.QueryGetData(ref FORMATETC format)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->QueryGetData(Unsafe.As<FORMATETC, Com.FORMATETC>(ref format));
            }

            void ComTypes.IDataObject.SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
            {
                Com.STGMEDIUM nativeMedium = (Com.STGMEDIUM)medium;
                Com.FORMATETC nativeFormat = Unsafe.As<FORMATETC, Com.FORMATETC>(ref formatIn);
                using var nativeDataObject = _nativeDataObject.GetInterface();
                HRESULT result = nativeDataObject.Value->SetData(&nativeFormat, &nativeMedium, release);
                medium = (STGMEDIUM)nativeMedium;
                nativeMedium.ReleaseUnknown();
                result.ThrowOnFailure();
            }
        }
    }
}
