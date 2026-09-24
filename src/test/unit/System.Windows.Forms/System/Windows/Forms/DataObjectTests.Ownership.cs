// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms.Tests;

/// <summary>
///  Verifies native storage-medium ownership when DataObject forwards runtime COM calls.
/// </summary>
public partial class DataObjectTests
{
    [StaTheory]
    [InlineData(false, false, false, false)]
    [InlineData(false, false, true, false)]
    [InlineData(false, true, false, false)]
    [InlineData(false, true, true, false)]
    [InlineData(true, false, false, false)]
    [InlineData(true, false, true, false)]
    [InlineData(true, true, false, false)]
    [InlineData(true, true, true, false)]
    [InlineData(true, true, false, true)]
    [InlineData(true, true, true, true)]
    public unsafe void IComDataObjectSetData_NativeAdapter_Ownership(
        bool succeeds,
        bool release,
        bool hasReleaseOwner,
        bool releaseWithinCall)
    {
        using MemoryStream data = new([42]);
        Com.ComManagedStream source = new(data);
        using var stream = ComHelpers.GetComScope<Com.IStream>(source);
        using var observer = stream.Query<Com.IUnknown>();
        uint baseline = GetSetDataReferenceCount(observer);
        using SetDataNativeRecipient recipient = new(succeeds ? HRESULT.S_OK : HRESULT.E_FAIL, releaseWithinCall);
        using var nativeDataObject = ComHelpers.GetComScope<Com.IDataObject>(recipient);

        // The pointer constructor selects Composition.Create(IDataObject*), not the runtime-object
        // overload that would call a managed mock directly and bypass NativeToRuntimeAdapter.
        DataObject dataObject = new(nativeDataObject.Value);
        GetSetDataRuntimeAdapter(dataObject);
        IComDataObject runtimeDataObject = dataObject;
        FORMATETC format = new()
        {
            cfFormat = (short)DataFormats.GetFormat(DataFormats.Text).Id,
            dwAspect = DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            tymed = TYMED.TYMED_ISTREAM
        };
        STGMEDIUM medium = new()
        {
            tymed = TYMED.TYMED_ISTREAM,
            unionmember = (nint)stream.Value,
            pUnkForRelease = hasReleaseOwner ? source : null
        };
        STGMEDIUM original = medium;

        // TYMED_ISTREAM owns a stream reference even when pUnkForRelease is non-null:
        // ReleaseStgMedium releases both. Conversion supplies only the additional release-owner reference.
        stream.Value->AddRef();

        try
        {
            Exception? exception = Record.Exception(() => runtimeDataObject.SetData(ref format, ref medium, release));
            bool transferred = succeeds && release;

            if (succeeds)
            {
                Assert.Null(exception);
            }
            else
            {
                Assert.Equal((int)HRESULT.E_FAIL, Assert.IsType<COMException>(exception).HResult);
            }

            Assert.Equal(1, recipient.SetDataCalls);
            Assert.Equal(release, recipient.ReleaseRequested);
            Assert.Equal(transferred, recipient.AcceptedOwnership);
            Assert.Equal((nint)stream.Value, recipient.DataPointer);
            Assert.Equal(hasReleaseOwner ? (nint)observer.Value : 0, recipient.ReleaseOwner);
            Assert.Equal(baseline + (hasReleaseOwner ? 2u : 1u), recipient.ReferencesDuringCall);
            Assert.Equal((ushort)format.cfFormat, recipient.Format.cfFormat);
            Assert.Equal((uint)format.tymed, recipient.Format.tymed);
            Assert.Equal(format.lindex, recipient.Format.lindex);

            // Even if ReleaseStgMedium changed the transferred native structure during the call,
            // the public managed ref parameter and its original managed owner stay usable.
            Assert.Equal(original.tymed, medium.tymed);
            Assert.Equal(original.unionmember, medium.unionmember);
            Assert.Same(original.pUnkForRelease, medium.pUnkForRelease);
            if (hasReleaseOwner)
            {
                Assert.Same(data, Assert.IsType<Com.ComManagedStream>(medium.pUnkForRelease).GetDataStream());
            }

            uint callerReferences = !transferred ? 1u : 0u;
            uint retainedReferences = transferred && !releaseWithinCall ? (hasReleaseOwner ? 2u : 1u) : 0u;
            Assert.Equal(baseline + callerReferences + retainedReferences, GetSetDataReferenceCount(observer));
            Assert.Equal(transferred && !releaseWithinCall, recipient.HasRetainedMedium);
            Assert.Equal(transferred && releaseWithinCall ? 1 : 0, recipient.MediumReleases);

            recipient.ReleaseRetainedMedium();
            Assert.Equal(baseline + callerReferences, GetSetDataReferenceCount(observer));
            Assert.Equal(transferred ? 1 : 0, recipient.MediumReleases);
            Assert.Equal(HRESULT.S_OK, stream.Value->Commit(0));
            Assert.Equal(42, source.GetDataStream().ReadByte());
        }
        finally
        {
            recipient.ReleaseRetainedMedium();
            if (!recipient.AcceptedOwnership)
            {
                stream.Value->Release();
            }

            // Keep both observer scopes safe to dispose if a regression over-released the medium.
            // Counts above are asserted before repairing test-only references.
            RestoreSetDataReferenceCount(observer, baseline);
            GC.KeepAlive(dataObject);
        }
    }

    [StaTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public unsafe void IComDataObjectSetData_NativeAdapter_InterfaceFailure_ReleasesConversionReference(
        bool release,
        bool hasReleaseOwner)
    {
        using MemoryStream data = new([42]);
        Com.ComManagedStream source = new(data);
        using var stream = ComHelpers.GetComScope<Com.IStream>(source);
        using var observer = stream.Query<Com.IUnknown>();
        uint baseline = GetSetDataReferenceCount(observer);
        using SetDataNativeRecipient recipient = new(HRESULT.S_OK, releaseWithinCall: false);
        using var nativeDataObject = ComHelpers.GetComScope<Com.IDataObject>(recipient);
        DataObject dataObject = new(nativeDataObject.Value);
        IComDataObject adapter = GetSetDataRuntimeAdapter(dataObject);
        var agileDataObject = (AgileComPointer<Com.IDataObject>)adapter.TestAccessor.Dynamic._nativeDataObject;

        // Revoke this adapter's registration so GetInterface fails after medium conversion,
        // without invalidating the independent reference used by the other Composition adapter.
        agileDataObject.Dispose();
        using var unavailable = agileDataObject.TryGetInterface(out HRESULT expectedError);
        Assert.True(expectedError.Failed);

        FORMATETC format = new() { tymed = TYMED.TYMED_ISTREAM };
        STGMEDIUM medium = new()
        {
            tymed = TYMED.TYMED_ISTREAM,
            unionmember = (nint)stream.Value,
            pUnkForRelease = hasReleaseOwner ? source : null
        };

        try
        {
            IComDataObject runtimeDataObject = dataObject;
            Exception? exception = Record.Exception(() => runtimeDataObject.SetData(ref format, ref medium, release));
            Assert.NotNull(exception);
            Assert.Equal((int)expectedError, exception.HResult);
            Assert.Equal(0, recipient.SetDataCalls);
            Assert.Equal(baseline, GetSetDataReferenceCount(observer));
            Assert.Equal(TYMED.TYMED_ISTREAM, medium.tymed);
            Assert.Equal((nint)stream.Value, medium.unionmember);
            Assert.Same(hasReleaseOwner ? source : null, medium.pUnkForRelease);
            Assert.Equal(HRESULT.S_OK, stream.Value->Commit(0));
            Assert.Equal(42, source.GetDataStream().ReadByte());
        }
        finally
        {
            RestoreSetDataReferenceCount(observer, baseline);
            GC.KeepAlive(dataObject);
        }
    }

    private static IComDataObject GetSetDataRuntimeAdapter(DataObject dataObject)
    {
        object composition = dataObject.TestAccessor.Dynamic._innerData;
        var adapter = (IComDataObject)composition.TestAccessor.Dynamic._runtimeDataObject;
        Assert.Equal("NativeToRuntimeAdapter", adapter.GetType().Name);
        return adapter;
    }

    private static unsafe uint GetSetDataReferenceCount(Com.IUnknown* unknown)
    {
        unknown->AddRef();
        return unknown->Release();
    }

    private static unsafe void RestoreSetDataReferenceCount(Com.IUnknown* observer, uint expected)
    {
        uint count = GetSetDataReferenceCount(observer);
        while (count < expected)
        {
            count = observer->AddRef();
        }

        while (count > expected)
        {
            count = observer->Release();
        }
    }

    /// <summary>
    ///  Accepts storage media through a native CCW and observes the reference handed to SetData.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Only successful calls with fRelease set consume the medium. The recipient can release it
    ///   immediately or retain it without AddRef, as required by the native IDataObject contract.
    ///  </para>
    /// </remarks>
    private sealed unsafe class SetDataNativeRecipient(HRESULT result, bool releaseWithinCall)
        : Com.IDataObject.Interface, Com.IManagedWrapper<Com.IDataObject>, IDisposable
    {
        private Com.STGMEDIUM _retainedMedium;

        public int SetDataCalls { get; private set; }
        public bool ReleaseRequested { get; private set; }
        public bool AcceptedOwnership { get; private set; }
        public bool HasRetainedMedium { get; private set; }
        public int MediumReleases { get; private set; }
        public nint DataPointer { get; private set; }
        public nint ReleaseOwner { get; private set; }
        public uint ReferencesDuringCall { get; private set; }
        public Com.FORMATETC Format { get; private set; }

        public HRESULT SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease)
        {
            SetDataCalls++;
            ReleaseRequested = fRelease;
            DataPointer = pmedium->hGlobal;
            ReleaseOwner = (nint)pmedium->pUnkForRelease;
            ReferencesDuringCall = GetSetDataReferenceCount((Com.IUnknown*)DataPointer);
            Format = *pformatetc;

            if (fRelease && result.Succeeded)
            {
                AcceptedOwnership = true;
                if (releaseWithinCall)
                {
                    PInvokeCore.ReleaseStgMedium(ref *pmedium);
                    MediumReleases++;
                }
                else
                {
                    _retainedMedium = *pmedium;
                    HasRetainedMedium = true;
                }
            }

            return result;
        }

        public void ReleaseRetainedMedium()
        {
            if (!HasRetainedMedium)
            {
                return;
            }

            Com.STGMEDIUM medium = _retainedMedium;
            _retainedMedium = default;
            HasRetainedMedium = false;
            PInvokeCore.ReleaseStgMedium(ref medium);
            MediumReleases++;
        }

        public void Dispose() => ReleaseRetainedMedium();

        public HRESULT GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium) => HRESULT.E_NOTIMPL;
        public HRESULT GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium) => HRESULT.E_NOTIMPL;
        public HRESULT QueryGetData(Com.FORMATETC* pformatetc) => HRESULT.E_NOTIMPL;
        public HRESULT GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut) => HRESULT.E_NOTIMPL;
        public HRESULT EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc) => HRESULT.E_NOTIMPL;
        public HRESULT DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection) => HRESULT.E_NOTIMPL;
        public HRESULT DUnadvise(uint dwConnection) => HRESULT.E_NOTIMPL;
        public HRESULT EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise) => HRESULT.E_NOTIMPL;
    }
}
