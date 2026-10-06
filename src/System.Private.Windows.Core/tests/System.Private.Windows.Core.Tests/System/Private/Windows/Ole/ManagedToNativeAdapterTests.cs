// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Private.Windows.Ole;

public unsafe class ManagedToNativeAdapterTests
{
    [Fact]
    public void GetData_EnhancedMetafile_DelegatesToOleServicesWithTymedEnhMetafile()
    {
        EmfTestOleServices<ManagedToNativeAdapterTests>.Reset();
        using EnhMetafileScope source = EnhMetafileScope.Create();
        EmfPayload payload = source.CreatePayload();
        TestDataObject<EmfTestOleServices<ManagedToNativeAdapterTests>> dataObject =
            new(DataFormatNames.Emf, payload);
        FORMATETC formatEtc = CreateEmfFormatEtc();
        STGMEDIUM medium = default;

        try
        {
            dataObject.GetData(&formatEtc, &medium).Should().Be(HRESULT.S_OK);

            EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedFormat.Should().Be(DataFormatNames.Emf);
            EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedData.Should().BeSameAs(payload);
            EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedFormatEtc.cfFormat
                .Should().Be((ushort)CLIPBOARD_FORMAT.CF_ENHMETAFILE);
            EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedFormatEtc.dwAspect
                .Should().Be((uint)DVASPECT.DVASPECT_CONTENT);
            EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedFormatEtc.lindex.Should().Be(-1);
            EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedFormatEtc.tymed
                .Should().Be((uint)TYMED.TYMED_ENHMF);
            medium.tymed.Should().Be(TYMED.TYMED_ENHMF);
            EmfNativeMethods.IsValid((nint)medium.hGlobal).Should().BeTrue();
        }
        finally
        {
            ReleaseIfOwned(ref medium);
        }

        EmfNativeMethods.IsValid(
            EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedHandles.Single()).Should().BeFalse();
        source.CreatePayload().Bits.Should().Equal(payload.Bits);
    }

    [Fact]
    public void QueryGetData_EnhancedMetafile_AllowsTymedEnhMetafile()
    {
        EmfTestOleServices<ManagedToNativeAdapterTests>.Reset();
        using EnhMetafileScope source = EnhMetafileScope.Create();
        TestDataObject<EmfTestOleServices<ManagedToNativeAdapterTests>> dataObject =
            new(DataFormatNames.Emf, source.CreatePayload());
        FORMATETC formatEtc = CreateEmfFormatEtc();

        dataObject.QueryGetData(&formatEtc).Should().Be(HRESULT.S_OK);

        formatEtc.dwAspect = (uint)DVASPECT.DVASPECT_THUMBNAIL;
        dataObject.QueryGetData(&formatEtc).Should().Be(HRESULT.DV_E_DVASPECT);

        formatEtc.dwAspect = (uint)DVASPECT.DVASPECT_CONTENT;
        formatEtc.tymed = (uint)TYMED.TYMED_FILE;
        dataObject.QueryGetData(&formatEtc).Should().Be(HRESULT.DV_E_TYMED);
    }

    [Fact]
    public void GetDataHere_EnhancedMetafile_RejectsAndPreservesCallerHandle()
    {
        EmfTestOleServices<ManagedToNativeAdapterTests>.Reset();
        using EnhMetafileScope source = EnhMetafileScope.Create();
        using EnhMetafileScope callerStorage = EnhMetafileScope.Create();
        TestDataObject<EmfTestOleServices<ManagedToNativeAdapterTests>> dataObject =
            new(DataFormatNames.Emf, source.CreatePayload());
        FORMATETC formatEtc = CreateEmfFormatEtc();
        STGMEDIUM medium = new()
        {
            tymed = TYMED.TYMED_ENHMF,
            hGlobal = (HGLOBAL)callerStorage.Handle
        };

        dataObject.GetDataHere(&formatEtc, &medium).Should().Be(HRESULT.DV_E_TYMED);

        medium.tymed.Should().Be(TYMED.TYMED_ENHMF);
        ((nint)medium.hGlobal).Should().Be(callerStorage.Handle);
        EmfNativeMethods.IsValid(callerStorage.Handle).Should().BeTrue();
        EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedHandles.Should().BeEmpty();
    }

    [Fact]
    public void GetData_EnhancedMetafile_CombinedTymed_SelectsOwnedEnhMetafile()
    {
        EmfTestOleServices<ManagedToNativeAdapterTests>.Reset();
        using EnhMetafileScope source = EnhMetafileScope.Create();
        TestDataObject<EmfTestOleServices<ManagedToNativeAdapterTests>> dataObject =
            new(DataFormatNames.Emf, source.CreatePayload());
        FORMATETC formatEtc = CreateEmfFormatEtc();
        formatEtc.tymed = (uint)(TYMED.TYMED_ENHMF | TYMED.TYMED_ISTREAM | TYMED.TYMED_HGLOBAL);
        STGMEDIUM medium = default;

        try
        {
            dataObject.QueryGetData(&formatEtc).Should().Be(HRESULT.S_OK);
            dataObject.GetData(&formatEtc, &medium).Should().Be(HRESULT.S_OK);

            medium.tymed.Should().Be(TYMED.TYMED_ENHMF);
            EmfNativeMethods.IsValid((nint)medium.hGlobal).Should().BeTrue();
        }
        finally
        {
            ReleaseIfOwned(ref medium);
        }

        EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedHandles
            .Should().ContainSingle();
        EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedHandles
            .Should().OnlyContain(handle => !EmfNativeMethods.IsValid(handle));
    }

    [Fact]
    public void GetData_EnhancedMetafile_NativeFailureWithHGlobalOffered_FallsBackToHGlobal()
    {
        EmfTestOleServices<ManagedToNativeAdapterTests>.Reset();
        EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedGetDataResult = HRESULT.DV_E_TYMED;
        EmfTestOleServices<ManagedToNativeAdapterTests>.AllocateHandleBeforeFailure = true;
        using MemoryStream source = new([0x45, 0x4D, 0x46]);
        TestDataObject<EmfTestOleServices<ManagedToNativeAdapterTests>> dataObject =
            new(DataFormatNames.Emf, source);
        FORMATETC formatEtc = CreateEmfFormatEtc();
        formatEtc.tymed = (uint)(TYMED.TYMED_ENHMF | TYMED.TYMED_HGLOBAL);
        STGMEDIUM medium = default;

        try
        {
            dataObject.GetData(&formatEtc, &medium).Should().Be(HRESULT.S_OK);

            medium.tymed.Should().Be(TYMED.TYMED_HGLOBAL);
            medium.hGlobal.IsNull.Should().BeFalse();
            PInvokeCore.GlobalSize(medium.hGlobal).Should().BeGreaterThan(0);
        }
        finally
        {
            ReleaseIfOwned(ref medium);
        }

        EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedHandles.Should().ContainSingle();
        EmfNativeMethods.IsValid(
            EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedHandles.Single()).Should().BeFalse();
    }

    [Theory]
    [InlineData(DataFormatNames.Emf, false, ComTypes.TYMED.TYMED_HGLOBAL)]
    [InlineData(DataFormatNames.Emf, true, ComTypes.TYMED.TYMED_ENHMF)]
    [InlineData(DataFormatNames.BinaryFormatMetafile, true, ComTypes.TYMED.TYMED_ENHMF)]
    public void FormatEnumerator_EnhancedMetafile_AdvertisesPlatformSupportedMedium(
        string format,
        bool supportsEnhMetafile,
        ComTypes.TYMED expected)
    {
        using EnhMetafileScope source = EnhMetafileScope.Create();
        TestDataObject<MockOleServices<ManagedToNativeAdapterTests>> dataObject =
            new(format, source.CreatePayload());
        FormatEnumerator enumerator = new(
            dataObject,
            format => DataFormatsCore<TestFormat>.GetOrAddFormat(format).Id,
            _ => supportsEnhMetafile);
        ComTypes.FORMATETC[] formats = new ComTypes.FORMATETC[1];

        enumerator.Next(1, formats, pceltFetched: null).Should().Be((int)HRESULT.S_OK);

        formats[0].cfFormat.Should().Be((short)DataFormatsCore<TestFormat>.GetOrAddFormat(format).Id);
        formats[0].tymed.Should().Be(expected);
    }

    [Fact]
    public void QueryGetData_EnhancedMetafile_WithoutPlatformSupport_ReturnsDvETymed()
    {
        using EnhMetafileScope source = EnhMetafileScope.Create();
        TestDataObject<MockOleServices<ManagedToNativeAdapterTests>> dataObject =
            new(DataFormatNames.Emf, source.CreatePayload());
        FORMATETC formatEtc = CreateEmfFormatEtc();

        dataObject.QueryGetData(&formatEtc).Should().Be(HRESULT.DV_E_TYMED);

        STGMEDIUM medium = default;
        dataObject.GetData(&formatEtc, &medium).Should().Be(HRESULT.DV_E_TYMED);
        medium.hGlobal.IsNull.Should().BeTrue();
    }

    [Fact]
    public void GetData_EnhancedMetafile_PlatformFailure_ReleasesAllocatedHandle()
    {
        EmfTestOleServices<ManagedToNativeAdapterTests>.Reset();
        EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedGetDataResult = HRESULT.E_FAIL;
        EmfTestOleServices<ManagedToNativeAdapterTests>.AllocateHandleBeforeFailure = true;
        using EnhMetafileScope source = EnhMetafileScope.Create();
        TestDataObject<EmfTestOleServices<ManagedToNativeAdapterTests>> dataObject =
            new(DataFormatNames.Emf, source.CreatePayload());
        FORMATETC formatEtc = CreateEmfFormatEtc();
        STGMEDIUM medium = default;

        dataObject.GetData(&formatEtc, &medium).Should().Be(HRESULT.E_FAIL);

        medium.hGlobal.IsNull.Should().BeTrue();
        EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedHandles.Should().ContainSingle();
        EmfNativeMethods.IsValid(
            EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedHandles.Single()).Should().BeFalse();
    }

    [Fact]
    public void GetData_EnhancedMetafile_RepeatedCallsReturnIndependentOwnedHandles()
    {
        EmfTestOleServices<ManagedToNativeAdapterTests>.Reset();
        using EnhMetafileScope source = EnhMetafileScope.Create();
        TestDataObject<EmfTestOleServices<ManagedToNativeAdapterTests>> dataObject =
            new(DataFormatNames.Emf, source.CreatePayload());
        FORMATETC formatEtc = CreateEmfFormatEtc();
        STGMEDIUM first = default;
        STGMEDIUM second = default;

        try
        {
            dataObject.GetData(&formatEtc, &first).Should().Be(HRESULT.S_OK);
            dataObject.GetData(&formatEtc, &second).Should().Be(HRESULT.S_OK);
            nint firstHandle = (nint)first.hGlobal;
            nint secondHandle = (nint)second.hGlobal;

            firstHandle.Should().NotBe(secondHandle);
            EmfNativeMethods.IsValid(firstHandle).Should().BeTrue();
            EmfNativeMethods.IsValid(secondHandle).Should().BeTrue();

            PInvokeCore.ReleaseStgMedium(ref first);
            first = default;

            EmfNativeMethods.IsValid(firstHandle).Should().BeFalse();
            EmfNativeMethods.IsValid(secondHandle).Should().BeTrue();
            source.CreatePayload().RecordCount.Should().BeGreaterThan(2);
        }
        finally
        {
            ReleaseIfOwned(ref first);
            ReleaseIfOwned(ref second);
        }

        EmfTestOleServices<ManagedToNativeAdapterTests>.ManagedHandles
            .Should().OnlyContain(handle => !EmfNativeMethods.IsValid(handle));
    }

    private static FORMATETC CreateEmfFormatEtc() => new()
    {
        cfFormat = (ushort)CLIPBOARD_FORMAT.CF_ENHMETAFILE,
        dwAspect = (uint)DVASPECT.DVASPECT_CONTENT,
        lindex = -1,
        tymed = (uint)TYMED.TYMED_ENHMF
    };

    private static void ReleaseIfOwned(ref STGMEDIUM medium)
    {
        if (!medium.hGlobal.IsNull)
        {
            PInvokeCore.ReleaseStgMedium(ref medium);
            medium = default;
        }
    }
}
