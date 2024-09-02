// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    internal unsafe partial class Composition
    {
        /// <summary>
        ///  Maps the runtime <see cref="ComTypes.IDataObject"/> to the native <see cref="Com.IDataObject.Interface"/>.
        /// </summary>
        private class RuntimeToNativeAdapter : Com.IDataObject.Interface, ComTypes.IDataObject, Com.IManagedWrapper<Com.IDataObject>
        {
            private readonly ComTypes.IDataObject _runtimeDataObject;

            public RuntimeToNativeAdapter(ComTypes.IDataObject dataObject) => _runtimeDataObject = dataObject;

            int ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection) => _runtimeDataObject.DAdvise(ref pFormatetc, advf, adviseSink, out connection);
            void ComTypes.IDataObject.DUnadvise(int connection) => _runtimeDataObject.DUnadvise(connection);
            int ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA? enumAdvise) => _runtimeDataObject.EnumDAdvise(out enumAdvise);
            IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(DATADIR direction) => _runtimeDataObject.EnumFormatEtc(direction);
            int ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut) => _runtimeDataObject.GetCanonicalFormatEtc(ref formatIn, out formatOut);
            void ComTypes.IDataObject.GetData(ref FORMATETC format, out STGMEDIUM medium) => _runtimeDataObject.GetData(ref format, out medium);
            void ComTypes.IDataObject.GetDataHere(ref FORMATETC format, ref STGMEDIUM medium) => _runtimeDataObject.GetDataHere(ref format, ref medium);
            int ComTypes.IDataObject.QueryGetData(ref FORMATETC format) => _runtimeDataObject.QueryGetData(ref format);
            void ComTypes.IDataObject.SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release) => _runtimeDataObject.SetData(ref formatIn, ref medium, release);

            HRESULT Com.IDataObject.Interface.DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection)
            {
                var adviseSink = (IAdviseSink)ComHelpers.GetObjectForIUnknown(pAdvSink);
                return (HRESULT)((ComTypes.IDataObject)this).DAdvise(ref *(FORMATETC*)pformatetc, (ADVF)advf, adviseSink, out *(int*)pdwConnection);
            }

            HRESULT Com.IDataObject.Interface.DUnadvise(uint dwConnection)
            {
                try
                {
                    ((ComTypes.IDataObject)this).DUnadvise((int)dwConnection);
                }
                catch (Exception e)
                {
                    return (HRESULT)e.HResult;
                }

                return HRESULT.S_OK;
            }

            HRESULT Com.IDataObject.Interface.EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise)
            {
                if (ppenumAdvise is null)
                {
                    return HRESULT.E_POINTER;
                }

                *ppenumAdvise = null;

                HRESULT hr = (HRESULT)((ComTypes.IDataObject)this).EnumDAdvise(out var enumAdvice);
                if (hr.Failed)
                {
                    return hr;
                }

                *ppenumAdvise = ComHelpers.TryGetComPointer<Com.IEnumSTATDATA>(enumAdvice, out hr);
                return hr.Succeeded ? hr : HRESULT.E_NOINTERFACE;
            }

            HRESULT Com.IDataObject.Interface.EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc)
            {
                if (ppenumFormatEtc is null)
                {
                    return HRESULT.E_POINTER;
                }

                var comTypeFormatEtc = ((ComTypes.IDataObject)this).EnumFormatEtc((DATADIR)(int)dwDirection);
                *ppenumFormatEtc = ComHelpers.TryGetComPointer<Com.IEnumFORMATETC>(comTypeFormatEtc, out HRESULT hr);
                return hr.Succeeded ? HRESULT.S_OK : HRESULT.E_NOINTERFACE;
            }

            HRESULT Com.IDataObject.Interface.GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut) =>
                (HRESULT)((ComTypes.IDataObject)this).GetCanonicalFormatEtc(ref *(FORMATETC*)pformatectIn, out *(FORMATETC*)pformatetcOut);

            HRESULT Com.IDataObject.Interface.GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium)
            {
                if (pmedium is null)
                {
                    return HRESULT.E_POINTER;
                }

                try
                {
                    ((ComTypes.IDataObject)this).GetData(ref *(FORMATETC*)pformatetcIn, out STGMEDIUM medium);
                    *pmedium = (Com.STGMEDIUM)medium;
                    return HRESULT.S_OK;
                }
                catch (Exception e)
                {
                    return (HRESULT)e.HResult;
                }
            }

            HRESULT Com.IDataObject.Interface.GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium)
            {
                if (pmedium is null)
                {
                    return HRESULT.E_POINTER;
                }

                STGMEDIUM medium = (STGMEDIUM)(*pmedium);
                try
                {
                    ((ComTypes.IDataObject)this).GetDataHere(ref *(FORMATETC*)pformatetc, ref medium);
                }
                catch (Exception e)
                {
                    return (HRESULT)e.HResult;
                }

                *pmedium = (Com.STGMEDIUM)medium;
                return HRESULT.S_OK;
            }

            HRESULT Com.IDataObject.Interface.QueryGetData(Com.FORMATETC* pformatetc) =>
                (HRESULT)((ComTypes.IDataObject)this).QueryGetData(ref *(FORMATETC*)pformatetc);

            HRESULT Com.IDataObject.Interface.SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease)
            {
                if (pmedium is null)
                {
                    return HRESULT.E_POINTER;
                }

                STGMEDIUM medium = (STGMEDIUM)(*pmedium);
                try
                {
                    ((ComTypes.IDataObject)this).SetData(ref *(FORMATETC*)pformatetc, ref medium, fRelease);
                }
                catch (Exception e)
                {
                    return (HRESULT)e.HResult;
                }

                return HRESULT.S_OK;
            }
        }
    }
}
