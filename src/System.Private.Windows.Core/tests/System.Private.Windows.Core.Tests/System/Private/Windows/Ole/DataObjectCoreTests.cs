// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

public unsafe class DataObjectCoreTests
{
    [Fact]
    public void TryGetData_MetafileMappedFormat_RespectsAutoConvert()
    {
        using EnhMetafileScope source = EnhMetafileScope.Create();
        EmfPayload payload = source.CreatePayload();
        TestDataObject<MockOleServices<DataObjectCoreTests>> dataObject =
            new(DataFormatNames.Emf, payload);

        dataObject.TryGetData(
            DataFormatNames.Emf,
            autoConvert: false,
            out EmfPayload? exact).Should().BeTrue();
        exact.Should().BeSameAs(payload);

        dataObject.TryGetData(
            DataFormatNames.BinaryFormatMetafile,
            autoConvert: false,
            out EmfPayload? noConversion).Should().BeFalse();
        noConversion.Should().BeNull();

        dataObject.TryGetData(
            DataFormatNames.BinaryFormatMetafile,
            autoConvert: true,
            out EmfPayload? converted).Should().BeTrue();
        converted.Should().BeSameAs(payload);
    }

    [Fact]
    public void TryGetData_EnhancedMetafile_ReverseMappingRespectsAutoConvert()
    {
        using EnhMetafileScope source = EnhMetafileScope.Create();
        EmfPayload payload = source.CreatePayload();
        TestDataObject<MockOleServices<DataObjectCoreTests>> dataObject =
            new(DataFormatNames.BinaryFormatMetafile, payload);

        dataObject.TryGetData(
            DataFormatNames.BinaryFormatMetafile,
            autoConvert: false,
            out EmfPayload? exact).Should().BeTrue();
        exact.Should().BeSameAs(payload);

        dataObject.TryGetData(
            DataFormatNames.Emf,
            autoConvert: false,
            out EmfPayload? noConversion).Should().BeFalse();
        noConversion.Should().BeNull();

        dataObject.TryGetData(
            DataFormatNames.Emf,
            autoConvert: true,
            out EmfPayload? converted).Should().BeTrue();
        converted.Should().BeSameAs(payload);
    }
}
