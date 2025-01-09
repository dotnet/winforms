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
    internal unsafe partial class Composition
    {
        /// <summary>
        ///  Maps native pointer <see cref="Com.IDataObject"/> to <see cref="ComTypes.IDataObject"/>.
        /// </summary>
        private class NativeToRuntimeAdapter : ComTypes.IDataObject
        {
            private readonly AgileComPointer<Com.IDataObject> _nativeDataObject;

            public NativeToRuntimeAdapter(Com.IDataObject* dataObject)
            {
#if DEBUG
                _nativeDataObject = new(dataObject, takeOwnership: true, trackDisposal: false);
#else
                _nativeDataObject = new(dataObject, takeOwnership: true);
#endif
            }

            public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
            {
                using var nativeAdviseSink = ComHelpers.TryGetComScope<Com.IAdviseSink>(adviseSink);
                fixed (Com.FORMATETC* nativeFormat = &Unsafe.As<FORMATETC, Com.FORMATETC>(ref pFormatetc))
                fixed (int* pConnection = &connection)
                {
                    using var nativeDataObject = _nativeDataObject.GetInterface();
                    return nativeDataObject.Value->DAdvise(nativeFormat, (uint)advf, nativeAdviseSink, (uint*)pConnection);
                }
            }

            public void DUnadvise(int connection)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                nativeDataObject.Value->DUnadvise((uint)connection).ThrowOnFailure();
            }

            public int EnumDAdvise(out IEnumSTATDATA? enumAdvise)
            {
                using ComScope<Com.IEnumSTATDATA> nativeStatData = new(null);
                using var nativeDataObject = _nativeDataObject.GetInterface();
                HRESULT result = nativeDataObject.Value->EnumDAdvise(nativeStatData);
                ComHelpers.TryGetObjectForIUnknown(nativeStatData.AsUnknown, out enumAdvise);
                return result;
            }

            public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
            {
                using ComScope<Com.IEnumFORMATETC> nativeFormat = new(null);
                using var nativeDataObject = _nativeDataObject.GetInterface();
                if (nativeDataObject.Value->EnumFormatEtc((uint)direction, nativeFormat).Failed)
                {
                    throw new ExternalException(SR.ExternalException, (int)HRESULT.E_NOTIMPL);
                }

                return (IEnumFORMATETC)ComHelpers.GetObjectForIUnknown(nativeFormat);
            }

            public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                HRESULT result = nativeDataObject.Value->GetCanonicalFormatEtc(Unsafe.As<FORMATETC, Com.FORMATETC>(ref formatIn), out Com.FORMATETC nativeFormat);
                formatOut = Unsafe.As<Com.FORMATETC, FORMATETC>(ref nativeFormat);
                return result;
            }

            public void GetData(ref FORMATETC format, out STGMEDIUM medium)
            {
                Com.FORMATETC nativeFormat = Unsafe.As<FORMATETC, Com.FORMATETC>(ref format);
                Com.STGMEDIUM nativeMedium = default;
                using var nativeDataObject = _nativeDataObject.GetInterface();
                nativeDataObject.Value->GetData(&nativeFormat, &nativeMedium).ThrowOnFailure();
                medium = (STGMEDIUM)nativeMedium;
                nativeMedium.ReleaseUnknown();
            }

            public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
            {
                Com.FORMATETC nativeFormat = Unsafe.As<FORMATETC, Com.FORMATETC>(ref format);
                Com.STGMEDIUM nativeMedium = (Com.STGMEDIUM)medium;
                using var nativeDataObject = _nativeDataObject.GetInterface();
                nativeDataObject.Value->GetDataHere(&nativeFormat, &nativeMedium).ThrowOnFailure();
                medium = (STGMEDIUM)nativeMedium;
                nativeMedium.ReleaseUnknown();
            }

            public int QueryGetData(ref FORMATETC format)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->QueryGetData(Unsafe.As<FORMATETC, Com.FORMATETC>(ref format));
            }

            public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
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
