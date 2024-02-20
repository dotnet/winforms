// Licensed to the .NET Foundation under one or more agreements.
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
        private class NativeDataObjectToRuntimeAdapter : Com.IDataObject.Interface, ComTypes.IDataObject
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

            HRESULT Com.IDataObject.Interface.GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->GetData(pformatetcIn, pmedium);
            }

            HRESULT Com.IDataObject.Interface.GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->GetDataHere(pformatetc, pmedium);
            }

            HRESULT Com.IDataObject.Interface.QueryGetData(Com.FORMATETC* pformatetc)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->QueryGetData(pformatetc);
            }

            HRESULT Com.IDataObject.Interface.GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->GetCanonicalFormatEtc(pformatectIn, pformatetcOut);
            }

            HRESULT Com.IDataObject.Interface.SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->SetData(pformatetc, pmedium, fRelease);
            }

            HRESULT Com.IDataObject.Interface.EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->EnumFormatEtc(dwDirection, ppenumFormatEtc);
            }

            HRESULT Com.IDataObject.Interface.DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->DAdvise(pformatetc, advf, pAdvSink, pdwConnection);
            }

            HRESULT Com.IDataObject.Interface.DUnadvise(uint dwConnection)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->DUnadvise(dwConnection);
            }

            HRESULT Com.IDataObject.Interface.EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->EnumDAdvise(ppenumAdvise);
            }

            int ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
            {
                using var nativeAdviseSink = ComHelpers.TryGetComScope<Com.IAdviseSink>(adviseSink);
                fixed (Com.FORMATETC* nativeFormat = &Unsafe.As<FORMATETC, Com.FORMATETC>(ref pFormatetc))
                fixed (int* pConnection = &connection)
                {
                    return ((Com.IDataObject.Interface)this).DAdvise(nativeFormat, (uint)advf, nativeAdviseSink, (uint*)pConnection);
                }
            }

            void ComTypes.IDataObject.DUnadvise(int connection) => ((Com.IDataObject.Interface)this).DUnadvise((uint)connection).ThrowOnFailure();

            int ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA? enumAdvise)
            {
                using ComScope<Com.IEnumSTATDATA> nativeStatData = new(null);
                HRESULT result = ((Com.IDataObject.Interface)this).EnumDAdvise(nativeStatData);
                ComHelpers.TryGetObjectForIUnknown(nativeStatData.AsUnknown, out enumAdvise);
                return result;
            }

            IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(DATADIR direction)
            {
                using ComScope<Com.IEnumFORMATETC> nativeFormat = new(null);
                if (((Com.IDataObject.Interface)this).EnumFormatEtc((uint)direction, nativeFormat).Failed)
                {
                    throw new ExternalException(SR.ExternalException, (int)HRESULT.E_NOTIMPL);
                }

                return (IEnumFORMATETC)ComHelpers.GetObjectForIUnknown(nativeFormat.AsUnknown);
            }

            int ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
            {
                HRESULT result = this.GetCanonicalFormatEtc(Unsafe.As<FORMATETC, Com.FORMATETC>(ref formatIn), out Com.FORMATETC nativeFormat);
                formatOut = Unsafe.As<Com.FORMATETC, FORMATETC>(ref nativeFormat);
                return result;
            }

            void ComTypes.IDataObject.GetData(ref FORMATETC format, out STGMEDIUM medium)
            {
                Com.FORMATETC nativeFormat = Unsafe.As<FORMATETC, Com.FORMATETC>(ref format);
                Com.STGMEDIUM nativeMedium = default;
                ((Com.IDataObject.Interface)this).GetData(&nativeFormat, &nativeMedium).ThrowOnFailure();
                medium = (STGMEDIUM)nativeMedium;
                nativeMedium.ReleaseUnknown();
            }

            void ComTypes.IDataObject.GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
            {
                Com.FORMATETC nativeFormat = Unsafe.As<FORMATETC, Com.FORMATETC>(ref format);
                Com.STGMEDIUM nativeMedium = (Com.STGMEDIUM)medium;
                ((Com.IDataObject.Interface)this).GetDataHere(&nativeFormat, &nativeMedium).ThrowOnFailure();
                medium = (STGMEDIUM)nativeMedium;
                nativeMedium.ReleaseUnknown();
            }

            int ComTypes.IDataObject.QueryGetData(ref FORMATETC format) => this.QueryGetData(Unsafe.As<FORMATETC, Com.FORMATETC>(ref format));

            void ComTypes.IDataObject.SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
            {
                Com.STGMEDIUM nativeMedium = (Com.STGMEDIUM)medium;
                Com.FORMATETC nativeFormat = Unsafe.As<FORMATETC, Com.FORMATETC>(ref formatIn);
                HRESULT result = ((Com.IDataObject.Interface)this).SetData(&nativeFormat, &nativeMedium, release);
                medium = (STGMEDIUM)nativeMedium;
                nativeMedium.ReleaseUnknown();
                result.ThrowOnFailure();
            }
        }
    }
}
