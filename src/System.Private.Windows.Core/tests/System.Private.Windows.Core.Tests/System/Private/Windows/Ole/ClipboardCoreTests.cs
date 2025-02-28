// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

using ClipboardCore = System.Private.Windows.Ole.ClipboardCore<System.Private.Windows.Ole.MockOleServices<System.Private.Windows.Ole.ClipboardCoreTests>>;
using DataObject = System.Private.Windows.Ole.TestDataObject<System.Private.Windows.Ole.MockOleServices<System.Private.Windows.Ole.ClipboardCoreTests>>;
using ClipboardScope = System.Private.Windows.Ole.ClipboardScope<System.Private.Windows.Ole.MockOleServices<System.Private.Windows.Ole.ClipboardCoreTests>>;

namespace System.Private.Windows.Ole;

public unsafe class ClipboardCoreTests
{
    [Fact]
    public void Clear_ChecksThreadState()
    {
        Assert.Throws<ThreadStateException>(() => ClipboardCore<InvalidThreadOleServices>.Clear());
    }

    [Fact]
    public void SetData_ChecksThreadState()
    {
        Assert.Throws<ThreadStateException>(() => ClipboardCore<InvalidThreadOleServices>.SetData(null!, false));
    }

    [Fact]
    public void TryGetData_ChecksThreadState()
    {
        Assert.Throws<ThreadStateException>(() => ClipboardCore<InvalidThreadOleServices>.TryGetData(out _, out _));
    }

    private class InvalidThreadOleServices() : IOleServices
    {
        static bool IOleServices.AllowTypeWithoutResolver<T>() => throw new NotImplementedException();
        static IComVisibleDataObject IOleServices.CreateDataObject() => throw new NotImplementedException();
        static void IOleServices.EnsureThreadState() => throw new ThreadStateException();
        static unsafe HRESULT IOleServices.GetDataHere(string format, object data, FORMATETC* pformatetc, STGMEDIUM* pmedium) => throw new NotImplementedException();
        static bool IOleServices.IsValidTypeForFormat(Type type, string format) => throw new NotImplementedException();
        static HRESULT IOleServices.OleFlushClipboard() => throw new NotImplementedException();
        static unsafe HRESULT IOleServices.OleGetClipboard(IDataObject** dataObject) => throw new NotImplementedException();
        static unsafe HRESULT IOleServices.OleSetClipboard(IDataObject* dataObject) => throw new NotImplementedException();
        static unsafe bool IOleServices.TryGetObjectFromDataObject<T>(IDataObject* dataObject, string requestedFormat, [NotNullWhen(true)] out T data) => throw new NotImplementedException();
        static void IOleServices.ValidateDataStoreData(ref string format, bool autoConvert, object? data) => throw new NotImplementedException();
    }

    [Fact]
    public void MockOleServices_ValidatePointerBehavior()
    {
        DataObject dataObject = new();
        using ComScope<IDataObject> iDataObject = ComHelpers.GetComScope<IDataObject>(dataObject);
        using AgileComPointer<IDataObject> agileComPointer = new(iDataObject.Value, takeOwnership: false);
        using ComScope<IDataObject> fetched = agileComPointer.GetInterface();

        // We don't get a proxy when in process. Faking a proxy would require not using ComWrappers as we
        // cannot control QueryInterface behavior (it depends on IUnknown being it's pointer).
        Assert.Equal((nint)iDataObject.Value, (nint)fetched.Value);
    }

    [Fact]
    public void SetData_SetsClipboard()
    {
        using ClipboardScope scope = new();
        DataObject dataObject = new();
        HRESULT result = ClipboardCore.SetData(dataObject, copy: false, retryTimes: 1, retryDelay: 0);
        result.Should().Be(HRESULT.S_OK);

        result = ClipboardCore.TryGetData(out var data, out var original, retryTimes: 1, retryDelay: 0);
        using (data)
        {
            result.Should().Be(HRESULT.S_OK);
            data.IsNull.Should().BeFalse();
            original.Should().BeSameAs(dataObject);
        }
    }

    [Fact]
    public void Clear_ClearsClipboard()
    {
        HRESULT result;
        DataObject dataObject = new();

        using (ClipboardScope scope = new())
        {
            result = ClipboardCore.SetData(dataObject, copy: false, retryTimes: 1, retryDelay: 0);
            result.Should().Be(HRESULT.S_OK);
        }

        result = ClipboardCore.TryGetData(out var data, out var original, retryTimes: 1, retryDelay: 0);
        using (data)
        {
            result.Should().Be(HRESULT.CLIPBRD_E_BAD_DATA);
            data.IsNull.Should().BeTrue();
            original.Should().BeNull();
        }
    }

    [Fact]
    public void RoundTrip_Text()
    {
        using ClipboardScope scope = new();
        DataObject dataObject = new();
        dataObject.SetData(DataFormatNames.Text, "Hello, World!");
        HRESULT result = ClipboardCore.SetData(dataObject, copy: false, retryTimes: 1, retryDelay: 0);
        result.Should().Be(HRESULT.S_OK);

        result = ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out var data, retryTimes: 1, retryDelay: 0);
        result.Should().Be(HRESULT.S_OK);
        data.Should().NotBeNull();
        data!.GetDataPresent(DataFormatNames.Text).Should().BeTrue();
        data.GetData(DataFormatNames.Text).Should().Be("Hello, World!");

        data.GetDataPresent(DataFormatNames.UnicodeText).Should().BeTrue();
        data.GetData(DataFormatNames.UnicodeText).Should().Be("Hello, World!");
        data.GetDataPresent(DataFormatNames.UnicodeText, autoConvert: false).Should().BeFalse();
        data.GetData(DataFormatNames.UnicodeText, autoConvert: false).Should().BeNull();

        IDataObjectInternal iDataObject = data.Should().BeAssignableTo<IDataObjectInternal>().Subject;
        iDataObject.TryGetData(out string? text).Should().BeTrue();
        text.Should().Be("Hello, World!");

        iDataObject.TryGetData(DataFormatNames.Text, out text).Should().BeTrue();
        text.Should().Be("Hello, World!");
        iDataObject.TryGetData(DataFormatNames.UnicodeText, out text).Should().BeTrue();
        text.Should().Be("Hello, World!");
        iDataObject.TryGetData(DataFormatNames.UnicodeText, autoConvert: false, out text).Should().BeFalse();
        text.Should().BeNull();
    }

    [Fact]
    public void Clipboard_DerivedDataObject_DataPresent()
    {
        // https://github.com/dotnet/winforms/issues/12789
        SomeDataObject data = new();

        // This was provided as a workaround for the above and should not break, but should
        // also work without it.
        data.SetData(SomeDataObject.Format, data);

        ClipboardCore.SetData(data, copy: false, retryTimes: 1, retryDelay: 0);
        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out var outData).Should().Be(HRESULT.S_OK);
        outData!.GetDataPresent(SomeDataObject.Format).Should().BeTrue();
    }

    internal class SomeDataObject : DataObject
    {
        public static string Format => "SomeDataObjectId";
        public override string[] GetFormats() => [Format];

        public override bool GetDataPresent(string format, bool autoConvert)
            => format == Format || base.GetDataPresent(format, autoConvert);
    }
}
