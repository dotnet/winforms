// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    /// <summary>
    ///  A wrapper that responds to <see cref="ComTypes.IDataObject"/> calls by
    ///  forwarding to the underlying native <see cref="Com.IDataObject"/>.
    /// </summary>
    private sealed partial class ComDataObjectWrapper : ComTypes.IDataObject
    {
        private readonly AgileComPointer<Com.IDataObject> _dataObject;

        public ComDataObjectWrapper(Com.IDataObject* dataObject)
        {
#if DEBUG
            _dataObject = new(dataObject, takeOwnership: true, trackDisposal: false);
#else
            _dataObject = new(dataObject, takeOwnership: true);
#endif
        }

        int ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
        {
            using var pAdviseSink = ComHelpers.GetComScope<Com.IAdviseSink>(adviseSink);
            using ComScope<Com.IDataObject> dataObject = _dataObject.GetInterface();
            HRESULT hr = dataObject.Value->DAdvise(pFormatetc, (uint)advf, pAdviseSink, out uint connectionResult);
            connection = (int)connectionResult;
            return hr;
        }

        void ComTypes.IDataObject.DUnadvise(int connection)
        {
            using ComScope<Com.IDataObject> dataObject = _dataObject.GetInterface();
            dataObject.Value->DUnadvise((uint)connection).ThrowOnFailure();
        }

        int ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA? enumAdvise)
        {
            using ComScope<Com.IDataObject> dataObject = _dataObject.GetInterface();
            ComScope<Com.IEnumSTATDATA> pEnumAdvise = new(null);
            HRESULT result = dataObject.Value->EnumDAdvise(pEnumAdvise);

            ComScope<Com.IUnknown> unknown = pEnumAdvise.Query<Com.IUnknown>();
            enumAdvise = ComHelpers.TryGetObjectForIUnknown(unknown, out IEnumSTATDATA? enumStatData)
                ? enumStatData
                : new EnumStatDataWrapper(pEnumAdvise);

            if (enumStatData is not null)
            {
                pEnumAdvise.Dispose();
            }
            else
            {
                unknown.Dispose();
            }

            return result;
        }

        IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(DATADIR direction)
        {
            using ComScope<Com.IDataObject> dataObject = _dataObject.GetInterface();
            ComScope<Com.IEnumFORMATETC> pEnumFormatEtc = new(null);
            dataObject.Value->EnumFormatEtc((uint)direction, pEnumFormatEtc).ThrowOnFailure();

            ComScope<Com.IUnknown> unknown = pEnumFormatEtc.Query<Com.IUnknown>();
            IEnumFORMATETC result = ComHelpers.TryGetObjectForIUnknown(unknown, out IEnumFORMATETC? enumFormat)
                ? enumFormat
                : new EnumFormatEtcWrapper(pEnumFormatEtc);

            if (enumFormat is not null)
            {
                pEnumFormatEtc.Dispose();
            }
            else
            {
                unknown.Dispose();
            }

            return result;
        }

        int ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
        {
            using ComScope<Com.IDataObject> dataObject = _dataObject.GetInterface();
            HRESULT hr = dataObject.Value->GetCanonicalFormatEtc(formatIn, out Com.FORMATETC pFormatOut);
            formatOut = pFormatOut;
            return hr;
        }

        void ComTypes.IDataObject.GetData(ref FORMATETC format, out STGMEDIUM medium)
        {
            using ComScope<Com.IDataObject> dataObject = _dataObject.GetInterface();
            dataObject.Value->GetData(format, out Com.STGMEDIUM comMedium).ThrowOnFailure();
            medium = (STGMEDIUM)comMedium;
        }

        void ComTypes.IDataObject.GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
        {
            using ComScope<Com.IDataObject> dataObject = _dataObject.GetInterface();
            Com.STGMEDIUM comMedium = (Com.STGMEDIUM)medium;
            dataObject.Value->GetDataHere(format, ref comMedium).ThrowOnFailure();

            medium.pUnkForRelease = comMedium.pUnkForRelease is null
                ? null
                : ComHelpers.GetObjectForIUnknown(comMedium.pUnkForRelease);
            medium.tymed = (TYMED)comMedium.tymed;
            medium.unionmember = comMedium.u.hGlobal;
        }

        int ComTypes.IDataObject.QueryGetData(ref FORMATETC format)
        {
            using ComScope<Com.IDataObject> dataObject = _dataObject.GetInterface();
            return dataObject.Value->QueryGetData(format);
        }

        void ComTypes.IDataObject.SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
        {
            using ComScope<Com.IDataObject> dataObject = _dataObject.GetInterface();
            dataObject.Value->SetData(formatIn, (Com.STGMEDIUM)medium, release).ThrowOnFailure();
        }
    }
}
