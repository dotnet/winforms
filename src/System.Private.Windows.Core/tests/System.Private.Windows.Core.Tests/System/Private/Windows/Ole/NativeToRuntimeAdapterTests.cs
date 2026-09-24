// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using Composition = System.Private.Windows.Ole.Composition<
    System.Private.Windows.Ole.MockOleServices<System.Private.Windows.Ole.NativeToRuntimeAdapterTests>,
    System.Private.Windows.Nrbf.CoreNrbfSerializer,
    System.Private.Windows.Ole.TestFormat>;

namespace System.Private.Windows.Ole;

/// <summary>
///  Verifies runtime adapter ownership when forwarding calls to a native data object.
/// </summary>
public unsafe class NativeToRuntimeAdapterTests
{
    [Theory]
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
    public void SetData_NativeAdapter_TransfersOnlyOnSuccess(
        bool succeeds,
        bool release,
        bool hasReleaseOwner,
        bool releaseWithinCall)
    {
        using MemoryStream data = new([42]);
        ComManagedStream source = new(data);
        using var stream = ComHelpers.GetComScope<IStream>(source);
        using var observer = stream.Query<IUnknown>();
        uint baseline = GetReferenceCount(observer);
        using SetDataNativeRecipient recipient = new(succeeds ? HRESULT.S_OK : HRESULT.E_FAIL, releaseWithinCall);
        using var nativeDataObject = ComHelpers.GetComScope<IDataObject>(recipient);
        var composition = Composition.Create(nativeDataObject.Value);
        ComTypes.IDataObject runtimeDataObject = composition;
        ComTypes.FORMATETC format = new()
        {
            cfFormat = 1,
            dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            tymed = ComTypes.TYMED.TYMED_ISTREAM
        };
        ComTypes.STGMEDIUM medium = new()
        {
            tymed = ComTypes.TYMED.TYMED_ISTREAM,
            unionmember = (nint)stream.Value,
            pUnkForRelease = hasReleaseOwner ? source : null
        };
        ComTypes.STGMEDIUM original = medium;

        // The caller owns the stream reference represented by the medium.
        stream.Value->AddRef();

        try
        {
            Exception? exception = Record.Exception(() => runtimeDataObject.SetData(ref format, ref medium, release));
            bool transferred = succeeds && release;

            if (succeeds)
            {
                exception.Should().BeNull();
            }
            else
            {
                exception.Should().BeOfType<COMException>().Which.HResult.Should().Be((int)HRESULT.E_FAIL);
            }

            recipient.SetDataCalls.Should().Be(1);
            recipient.ReleaseRequested.Should().Be(release);
            recipient.AcceptedOwnership.Should().Be(transferred);
            recipient.DataPointer.Should().Be((nint)stream.Value);
            recipient.ReleaseOwner.Should().Be(hasReleaseOwner ? (nint)observer.Value : 0);
            recipient.ReferencesDuringCall.Should().Be(baseline + (hasReleaseOwner ? 2u : 1u));

            // The managed ref parameter remains unchanged even if the recipient released the native medium.
            medium.tymed.Should().Be(original.tymed);
            medium.unionmember.Should().Be(original.unionmember);
            medium.pUnkForRelease.Should().BeSameAs(original.pUnkForRelease);

            uint callerReferences = !transferred ? 1u : 0u;
            uint retainedReferences = transferred && !releaseWithinCall ? (hasReleaseOwner ? 2u : 1u) : 0u;
            GetReferenceCount(observer).Should().Be(baseline + callerReferences + retainedReferences);

            recipient.ReleaseRetainedMedium();
            GetReferenceCount(observer).Should().Be(baseline + callerReferences);
            stream.Value->Commit(0).Should().Be(HRESULT.S_OK);
            source.GetDataStream().ReadByte().Should().Be(42);
        }
        finally
        {
            recipient.ReleaseRetainedMedium();
            if (!recipient.AcceptedOwnership)
            {
                stream.Value->Release();
            }

            RestoreReferenceCount(observer, baseline);
            GC.KeepAlive(composition);
        }
    }

    private static uint GetReferenceCount(IUnknown* unknown)
    {
        unknown->AddRef();
        return unknown->Release();
    }

    private static void RestoreReferenceCount(IUnknown* observer, uint expected)
    {
        uint count = GetReferenceCount(observer);
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
    ///  Accepts a native storage medium and optionally releases it before returning from <see cref="SetData"/>.
    /// </summary>
    private sealed class SetDataNativeRecipient(HRESULT result, bool releaseWithinCall) : NativeDataObjectMock
    {
        private STGMEDIUM _retainedMedium;

        public int SetDataCalls { get; private set; }
        public bool ReleaseRequested { get; private set; }
        public bool AcceptedOwnership { get; private set; }
        public nint DataPointer { get; private set; }
        public nint ReleaseOwner { get; private set; }
        public uint ReferencesDuringCall { get; private set; }

        public override HRESULT SetData(FORMATETC* pformatetc, STGMEDIUM* pmedium, BOOL fRelease)
        {
            SetDataCalls++;
            ReleaseRequested = fRelease;
            DataPointer = pmedium->hGlobal;
            ReleaseOwner = (nint)pmedium->pUnkForRelease;
            ReferencesDuringCall = GetReferenceCount((IUnknown*)DataPointer);

            if (fRelease && result.Succeeded)
            {
                AcceptedOwnership = true;
                if (releaseWithinCall)
                {
                    PInvokeCore.ReleaseStgMedium(ref *pmedium);
                }
                else
                {
                    _retainedMedium = *pmedium;
                }
            }

            return result;
        }

        public void ReleaseRetainedMedium()
        {
            if (_retainedMedium.tymed == TYMED.TYMED_NULL)
            {
                return;
            }

            STGMEDIUM medium = _retainedMedium;
            _retainedMedium = default;
            PInvokeCore.ReleaseStgMedium(ref medium);
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseRetainedMedium();
        }
    }
}
