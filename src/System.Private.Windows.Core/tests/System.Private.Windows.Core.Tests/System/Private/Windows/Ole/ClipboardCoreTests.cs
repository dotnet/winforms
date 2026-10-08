// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

using ClipboardCore = System.Private.Windows.Ole.ClipboardCore<System.Private.Windows.Ole.MockOleServices<System.Private.Windows.Ole.ClipboardCoreTests>>;
using ClipboardScope = System.Private.Windows.Ole.ClipboardScope<System.Private.Windows.Ole.MockOleServices<System.Private.Windows.Ole.ClipboardCoreTests>>;
using DataObject = System.Private.Windows.Ole.TestDataObject<System.Private.Windows.Ole.MockOleServices<System.Private.Windows.Ole.ClipboardCoreTests>>;

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
    public void GetDataObject_UnwrapUserDataObjectFalse_ReturnsOleProxy()
    {
        // WPF relies on this mode so format queries pass through OLE instead of the original managed object.
        using ClipboardScope scope = new();
        DataObject dataObject = new();
        dataObject.SetData(DataFormatNames.Text, autoConvert: false, "Hello, World!");
        ClipboardCore.SetData(dataObject, copy: false, retryTimes: 1, retryDelay: 0).Should().Be(HRESULT.S_OK);

        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(
            out ITestDataObject? data,
            retryTimes: 1,
            retryDelay: 0,
            unwrapUserDataObject: false).Should().Be(HRESULT.S_OK);

        data.Should().NotBeNull().And.NotBeSameAs(dataObject);
        data.GetDataPresent(DataFormatNames.Text, autoConvert: false).Should().BeTrue();
        data.GetData(DataFormatNames.Text, autoConvert: false).Should().Be("Hello, World!");
    }

    [Fact]
    public void GetDataObject_ProxyReadAfterClipboardOwnerChanged_ClearsCachedManagedObject()
    {
        // Restore the mock clipboard and ClipboardCore cache when the test completes.
        using ClipboardScope scope = new();

        // This object becomes the managed owner cached by ClipboardCore.
        DataObject original = new();

        // This object represents clipboard contents supplied later by another process.
        DataObject external = new();

        // Publish the original object without flushing it so ClipboardCore retains its managed-owner cache entry.
        ClipboardCore.SetData(original, copy: false, retryTimes: 1, retryDelay: 0).Should().Be(HRESULT.S_OK);

        // Replace only the mock OLE clipboard, leaving ClipboardCore's cached original object intentionally stale.
        MockOleServices<ClipboardCoreTests>.SimulateExternalClipboardChange(external);

        // Count ownership checks performed after the simulated external replacement.
        MockOleServices<ClipboardCoreTests>.ResetOleIsCurrentClipboardCallCount();

        // Proxy mode must still validate and clear the stale cache, but it must return an OLE-backed wrapper.
        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(
            out ITestDataObject? first,
            retryTimes: 1,
            retryDelay: 0,
            unwrapUserDataObject: false).Should().Be(HRESULT.S_OK);

        // This assertion proves the proxy-mode read, rather than the later normal read, performed the ownership check.
        MockOleServices<ClipboardCoreTests>.OleIsCurrentClipboardCallCount.Should().Be(1);

        // A normal read should now bypass cache validation because the proxy-mode read already cleared the stale entry.
        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(
            out ITestDataObject? second,
            retryTimes: 1,
            retryDelay: 0).Should().Be(HRESULT.S_OK);

        // Proxy mode returns a wrapper around the current OLE clipboard object, not the original managed instance.
        first.Should().NotBeSameAs(external);

        // Normal mode unwraps the in-process OLE object and returns the exact externally supplied managed instance.
        second.Should().BeSameAs(external);

        // No second ownership check means the stale cache was removed during the first read.
        MockOleServices<ClipboardCoreTests>.OleIsCurrentClipboardCallCount.Should().Be(1);
    }

    [Fact]
    public void GetDataObject_ConcurrentReadAndReplacement_DoesNotBlockOrClearReplacement()
    {
        using ClipboardScope scope = new();
        using ManualResetEventSlim ownershipCheckEntered = new();
        using ManualResetEventSlim continueOwnershipCheck = new();
        using ManualResetEventSlim replacementCompleted = new();
        using ManualResetEventSlim replacementCanExit = new();
        TimeSpan timeout = TimeSpan.FromSeconds(10);
        DataObject original = new();
        DataObject replacement = new();
        replacement.SetData(nameof(replacement), "replacement");
        Exception? readException = null;
        Exception? replacementException = null;
        HRESULT readResult = HRESULT.E_FAIL;
        HRESULT replacementResult = HRESULT.E_FAIL;
        HRESULT currentResult = HRESULT.E_FAIL;
        ITestDataObject? readData = null;
        ITestDataObject? current = null;

        ClipboardCore.SetData(original, copy: false, retryTimes: 1, retryDelay: 0).Should().Be(HRESULT.S_OK);
        MockOleServices<ClipboardCoreTests>.BeforeOleIsCurrentClipboard = () =>
        {
            ownershipCheckEntered.Set();
            if (!continueOwnershipCheck.Wait(timeout))
            {
                throw new TimeoutException("Timed out waiting to continue the Clipboard ownership check.");
            }
        };

        Thread readThread = new(() =>
        {
            try
            {
                readResult = ClipboardCore.GetDataObject<DataObject, ITestDataObject>(
                    out readData,
                    retryTimes: 1,
                    retryDelay: 0);
            }
            catch (Exception exception)
            {
                readException = exception;
            }
        });
        readThread.SetApartmentState(ApartmentState.STA);

        Thread replacementThread = new(() =>
        {
            try
            {
                replacementResult = ClipboardCore.SetData(
                    replacement,
                    copy: false,
                    retryTimes: 1,
                    retryDelay: 0);
            }
            catch (Exception exception)
            {
                replacementException = exception;
            }
            finally
            {
                replacementCompleted.Set();

                if (!replacementCanExit.Wait(timeout))
                {
                    replacementException ??= new TimeoutException(
                        "Timed out waiting for the concurrent Clipboard read to complete.");
                }
            }
        });
        replacementThread.SetApartmentState(ApartmentState.STA);

        try
        {
            readThread.Start();
            ownershipCheckEntered.Wait(timeout, TestContext.Current.CancellationToken).Should().BeTrue();

            replacementThread.Start();
            replacementCompleted.Wait(timeout, TestContext.Current.CancellationToken).Should().BeTrue();

            continueOwnershipCheck.Set();
            readThread.Join(timeout).Should().BeTrue();

            currentResult = ClipboardCore.GetDataObject<DataObject, ITestDataObject>(
                out current,
                retryTimes: 1,
                retryDelay: 0);
        }
        finally
        {
            continueOwnershipCheck.Set();

            if (readThread.ThreadState != ThreadState.Unstarted)
            {
                readThread.Join(timeout).Should().BeTrue();
            }

            replacementCanExit.Set();

            if (replacementThread.ThreadState != ThreadState.Unstarted)
            {
                replacementThread.Join(timeout).Should().BeTrue();
            }

            MockOleServices<ClipboardCoreTests>.BeforeOleIsCurrentClipboard = null;
        }

        readException.Should().BeNull();
        replacementException.Should().BeNull();
        replacementResult.Should().Be(HRESULT.S_OK);
        readResult.Should().Be(HRESULT.S_OK);
        readData.Should().NotBeSameAs(original);
        readData!.GetData(nameof(replacement)).Should().Be("replacement");

        currentResult.Should().Be(HRESULT.S_OK);
        current.Should().BeSameAs(replacement);
    }

    public static TheoryData<ClipboardCleanupOperation, bool> ClipboardCleanupOperations => new()
    {
        { ClipboardCleanupOperation.Clear, false },
        { ClipboardCleanupOperation.Flush, false },
        { ClipboardCleanupOperation.Copy, false },
        { ClipboardCleanupOperation.Clear, true },
        { ClipboardCleanupOperation.Flush, true },
        { ClipboardCleanupOperation.Copy, true }
    };

    [Theory]
    [MemberData(nameof(ClipboardCleanupOperations))]
    public void Cleanup_ConcurrentReplacement_TracksSuccessfulAttempt(
        ClipboardCleanupOperation operation,
        bool firstAttemptFails)
    {
        using ClipboardScope scope = new();
        using ManualResetEventSlim replacementRequested = new();
        using ManualResetEventSlim replacementCompleted = new();
        using ManualResetEventSlim replacementCanExit = new();
        TimeSpan timeout = TimeSpan.FromSeconds(10);
        DataObject original = new();
        DataObject replacement = new();
        Exception? replacementException = null;
        HRESULT replacementResult = HRESULT.E_FAIL;

        replacement.SetData(nameof(replacement), "replacement");
        ClipboardCore.SetData(original, copy: false, retryTimes: 1, retryDelay: 0).Should().Be(HRESULT.S_OK);

        Thread replacementThread = new(() =>
        {
            try
            {
                if (!replacementRequested.Wait(timeout))
                {
                    throw new TimeoutException("Timed out waiting to replace the Clipboard contents.");
                }

                replacementResult = ClipboardCore.SetData(
                    replacement,
                    copy: false,
                    retryTimes: 1,
                    retryDelay: 0);
            }
            catch (Exception exception)
            {
                replacementException = exception;
            }
            finally
            {
                replacementCompleted.Set();

                if (!replacementCanExit.Wait(timeout))
                {
                    replacementException ??= new TimeoutException(
                        "Timed out waiting for the Clipboard cleanup verification.");
                }
            }
        });
        replacementThread.SetApartmentState(ApartmentState.STA);

        void ReplaceClipboard()
        {
            MockOleServices<ClipboardCoreTests>.AfterOleFlushClipboard = null;
            MockOleServices<ClipboardCoreTests>.AfterOleSetClipboard = null;
            replacementRequested.Set();
            replacementCompleted.Wait(timeout, TestContext.Current.CancellationToken).Should().BeTrue();
        }

        switch (operation)
        {
            case ClipboardCleanupOperation.Clear:
                MockOleServices<ClipboardCoreTests>.AfterOleSetClipboard = isClear =>
                {
                    if (isClear)
                    {
                        ReplaceClipboard();
                    }
                };
                break;

            case ClipboardCleanupOperation.Flush:
            case ClipboardCleanupOperation.Copy:
                MockOleServices<ClipboardCoreTests>.AfterOleFlushClipboard = ReplaceClipboard;
                break;
        }

        if (firstAttemptFails)
        {
            if (operation == ClipboardCleanupOperation.Clear)
            {
                MockOleServices<ClipboardCoreTests>.NextOleSetClipboardResult = HRESULT.E_FAIL;
            }
            else
            {
                MockOleServices<ClipboardCoreTests>.NextOleFlushClipboardResult = HRESULT.E_FAIL;
            }
        }

        try
        {
            replacementThread.Start();

            HRESULT cleanupResult = operation switch
            {
                ClipboardCleanupOperation.Clear => ClipboardCore.Clear(retryTimes: 1, retryDelay: 0),
                ClipboardCleanupOperation.Flush => ClipboardCore.Flush(retryTimes: 1, retryDelay: 0),
                ClipboardCleanupOperation.Copy => ClipboardCore.SetData(
                    original,
                    copy: true,
                    retryTimes: 1,
                    retryDelay: 0),
                _ => throw new InvalidOperationException()
            };

            cleanupResult.Should().Be(HRESULT.S_OK);
            replacementCompleted.Wait(timeout, TestContext.Current.CancellationToken).Should().BeTrue();
            replacementException.Should().BeNull();
            replacementResult.Should().Be(HRESULT.S_OK);

            MockOleServices<ClipboardCoreTests>.ResetOleIsCurrentClipboardCallCount();
            HRESULT currentResult = ClipboardCore.GetDataObject<DataObject, ITestDataObject>(
                out ITestDataObject? current,
                retryTimes: 1,
                retryDelay: 0);

            if (firstAttemptFails && operation == ClipboardCleanupOperation.Clear)
            {
                currentResult.Should().Be(HRESULT.CLIPBRD_E_BAD_DATA);
                current.Should().BeNull();
            }
            else
            {
                currentResult.Should().Be(HRESULT.S_OK);
                current.Should().BeSameAs(replacement);
            }

            MockOleServices<ClipboardCoreTests>.OleIsCurrentClipboardCallCount.Should().Be(
                firstAttemptFails ? 0 : 1);
        }
        finally
        {
            MockOleServices<ClipboardCoreTests>.AfterOleFlushClipboard = null;
            MockOleServices<ClipboardCoreTests>.AfterOleSetClipboard = null;
            MockOleServices<ClipboardCoreTests>.NextOleFlushClipboardResult = null;
            MockOleServices<ClipboardCoreTests>.NextOleSetClipboardResult = null;
            replacementRequested.Set();
            replacementCanExit.Set();

            if (replacementThread.ThreadState != ThreadState.Unstarted)
            {
                replacementThread.Join(timeout).Should().BeTrue();
            }
        }
    }

    public enum ClipboardCleanupOperation
    {
        Clear,
        Flush,
        Copy
    }

    [Fact]
    public void DerivedDataObject_DataPresent()
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

    [Fact]
    public void SerializableObject_InProcess_DoesNotUseBinaryFormatter()
    {
        // This test ensures that the SerializableObject does not use BinaryFormatter when running in process.
        using ClipboardScope scope = new();

        DataObject dataObject = new();
        SerializablePerson person = new() { Name = "John Doe", Age = 30 };
        dataObject.SetData(person);
        HRESULT result = ClipboardCore.SetData(dataObject, copy: false, retryTimes: 1, retryDelay: 0);
        result.Should().Be(HRESULT.S_OK);

        result = ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out var data, retryTimes: 1, retryDelay: 0);
        result.Should().Be(HRESULT.S_OK);
        data.Should().NotBeNull();
        data!.GetDataPresent(typeof(SerializablePerson).FullName!).Should().BeTrue();

        data.GetData(typeof(SerializablePerson).FullName!).Should().BeSameAs(person);
    }

    [Serializable]
    internal class SerializablePerson
    {
        public string Name { get; set; } = "DefaultName";
        public int Age { get; set; }
    }

    [Fact]
    public void Clear_InvokeMultipleTimes_Success()
    {
        ClipboardCore.Clear();
        HRESULT result = ClipboardCore.TryGetData(out var data, out var original, retryTimes: 1, retryDelay: 0);
        using (data)
        {
            result.Should().Be(HRESULT.CLIPBRD_E_BAD_DATA);
            data.IsNull.Should().BeTrue();
            original.Should().BeNull();
        }

        ClipboardCore.Clear();
        result = ClipboardCore.TryGetData(out var data1, out var original1, retryTimes: 1, retryDelay: 0);
        using (data1)
        {
            result.Should().Be(HRESULT.CLIPBRD_E_BAD_DATA);
            data1.IsNull.Should().BeTrue();
            original1.Should().BeNull();
        }
    }

    [Fact]
    public void Contains_InvokeMultipleTimes_Success()
    {
        DataObject dataObject = new();
        HRESULT result = ClipboardCore.SetData(dataObject, copy: false, retryTimes: 1, retryDelay: 0);
        result.Should().Be(HRESULT.S_OK);

        ClipboardCore.Clear();

        result = ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out var data, retryTimes: 1, retryDelay: 0);
        HRESULT result2 = ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out var data2, retryTimes: 1, retryDelay: 0);
        result.Should().Be(result2);
        result.Should().Be(HRESULT.CLIPBRD_E_BAD_DATA);

        data.Should().Be(data2);
        data.Should().BeNull();
    }

    [Theory]
    [StringWithNullData]
    public void ContainsData_InvokeMultipleTimes_Success(string? format)
    {
        SetAndGetClipboardDataMultipleTimes(format, null!, out ITestDataObject? outData1, out ITestDataObject? outData2);

        outData1!.GetFormats().Should().BeEquivalentTo(outData2!.GetFormats());
    }

    [Theory]
    [EnumData<TextDataFormat>]
    public void ContainsText_TextDataFormat_InvokeMultipleTimes_Success(string format)
    {
        SetAndGetClipboardDataMultipleTimes(format, null!, out ITestDataObject? outData1, out ITestDataObject? outData2);

        outData1!.GetFormats().Contains(format).Should().BeTrue();
        outData1!.GetFormats().Should().BeEquivalentTo(outData2!.GetFormats());
    }

    [Fact]
    public void GetAudioStream_InvokeMultipleTimes_Success()
    {
        string testData = "WaveAudio";
        string format = DataFormatNames.WaveAudio;
        SetAndGetClipboardDataMultipleTimes(format, testData, out ITestDataObject? outData1, out ITestDataObject? outData2);

        VerifyResult(testData, format, outData1, outData2);
    }

    [Fact]
    public void GetDataObject_InvokeMultipleTimes_Success()
    {
        SetAndGetClipboardDataMultipleTimes(null, null!, out ITestDataObject? outData1, out ITestDataObject? outData2);

        outData1.Should().NotBeNull();
        outData2.Should().NotBeNull();
        outData1.GetFormats().Should().BeEquivalentTo(outData2.GetFormats());
    }

    [Fact]
    public void GetFileDropList_InvokeMultipleTimes_Success()
    {
        string testData = "FileDrop";
        string format = DataFormatNames.FileDrop;
        SetAndGetClipboardDataMultipleTimes(format, testData, out ITestDataObject? outData1, out ITestDataObject? outData2);

        VerifyResult(testData, format, outData1, outData2);
    }

    [Fact]
    public void GetImage_InvokeMultipleTimes_Success()
    {
        string testData = "Bitmap";
        string format = DataFormatNames.Bitmap;
        SetAndGetClipboardDataMultipleTimes(format, testData, out ITestDataObject? outData1, out ITestDataObject? outData2);

        VerifyResult(testData, format, outData1, outData2);
    }

    [Fact]
    public void GetText_InvokeMultipleTimes_Success()
    {
        string testData = "Text";
        string format = DataFormatNames.UnicodeText;
        SetAndGetClipboardDataMultipleTimes(format, testData, out ITestDataObject? outData1, out ITestDataObject? outData2);

        VerifyResult(testData, format, outData1, outData2);
    }

    [Theory]
    [EnumData<TextDataFormat>]
    public void GetText_TextDataFormat_InvokeMultipleTimes_Success(string format)
    {
        string testData = "Text";
        SetAndGetClipboardDataMultipleTimes(format, testData, out ITestDataObject? outData1, out ITestDataObject? outData2);

        VerifyResult(testData, format, outData1, outData2);
    }

    private static void SetAndGetClipboardDataMultipleTimes(string? format, string data, out ITestDataObject? outData1, out ITestDataObject? outData2)
    {
        DataObject dataObject = string.IsNullOrEmpty(format) ? new() : new(format, data);
        HRESULT result = ClipboardCore.SetData(dataObject, copy: false, retryTimes: 1, retryDelay: 0);
        result.Should().Be(HRESULT.S_OK);

        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out outData1, retryTimes: 1, retryDelay: 0);
        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out outData2, retryTimes: 1, retryDelay: 0);
    }

    private static void VerifyResult(string testData, string format, ITestDataObject? outData1, ITestDataObject? outData2)
    {
        outData1.Should().NotBeNull();
        outData2.Should().NotBeNull();
        outData1.GetDataPresent(format).Should().BeTrue();
        outData1.GetData(format, autoConvert: false).Should().Be(testData);
        outData1.GetData(format, autoConvert: false).Should().Be(outData2.GetData(format, autoConvert: false));
    }

    [Fact]
    public void SetData_Int_GetReturnsExpected()
    {
        ClipboardCore.SetData(new DataObject(SomeDataObject.Format, 1), copy: false, retryTimes: 1, retryDelay: 0);
        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out var outData, retryTimes: 1, retryDelay: 0);
        outData.Should().NotBeNull();
        outData.GetDataPresent("SomeDataObjectId").Should().BeTrue();
        outData.GetData("SomeDataObjectId").Should().Be(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(null)]
    public void SetData_EmptyOrWhitespaceFormat_ThrowsArgumentException(string? format)
    {
        Action action = () => ClipboardCore.SetData(new DataObject(format!, "data"), copy: true);
        action.Should().Throw<ArgumentException>().WithParameterName("format");
    }

    [Fact]
    public void SetFileDropList_NullFilePaths_ThrowsArgumentNullException()
    {
        Action action = () => ClipboardCore.SetFileDropList(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("filePaths");
    }

    [Fact]
    public void SetFileDropList_EmptyFilePaths_ThrowsArgumentException()
    {
        Action action = static () => ClipboardCore.SetFileDropList([]);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("\0")]
    public void SetFileDropList_InvalidFileInPaths_ThrowsArgumentException(string filePath)
    {
        StringCollection filePaths =
        [
            filePath
        ];
        Action action = () => ClipboardCore.SetFileDropList(filePaths);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetText_InvokeString_GetReturnsExpected()
    {
        ClipboardCore.SetData(new DataObject(DataFormatNames.UnicodeText, "text"), copy: false, retryTimes: 1, retryDelay: 0);
        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out var outData, retryTimes: 1, retryDelay: 0);
        outData.Should().NotBeNull();
        outData.GetDataPresent(DataFormatNames.UnicodeText).Should().BeTrue();
        outData.GetData(DataFormatNames.UnicodeText).Should().Be("text");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\t")]
    public void GetData_NullOrEmptyFormat_Returns_Null(string? format)
    {
        ClipboardCore.SetData(new DataObject(SomeDataObject.Format, null!), copy: false, retryTimes: 1, retryDelay: 0);
        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out var outData, retryTimes: 1, retryDelay: 0);
        outData.Should().NotBeNull();
        outData.GetDataPresent(format!).Should().BeFalse();
    }

    [Theory]
    [EnumData<TextDataFormat>]
    public void GetData_InvalidFormat_Returns_Null(string format)
    {
        ClipboardCore.SetData(new DataObject(SomeDataObject.Format, null!), copy: false, retryTimes: 1, retryDelay: 0);
        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out var outData, retryTimes: 1, retryDelay: 0);
        outData.Should().NotBeNull();
        outData.GetData(format).Should().BeNull();
    }

    [Theory]
    [InlineData(new byte[] { 1, 2, 3 })]
    [InlineData(new byte[] { })]
    [InlineData(null)]
    public void SetAudio_InvokeByteArray_GetReturnsExpected(byte[]? audioBytes)
    {
        DataObject dataObject = new(DataFormatNames.WaveAudio, audioBytes!);
        HRESULT result = ClipboardCore.SetData(dataObject, copy: false, retryTimes: 1, retryDelay: 0);
        result.Should().Be(HRESULT.S_OK);

        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out var outData, retryTimes: 1, retryDelay: 0);
        outData.Should().NotBeNull();
        outData.GetDataPresent(DataFormatNames.WaveAudio).Should().BeTrue();
        outData.GetData(DataFormatNames.WaveAudio).Should().Be(audioBytes);
    }

    public static IEnumerable<object[]> GetEmptyStreamData()
    {
        yield return new object[] { new MemoryStream([1, 2, 3]), new byte[] { 1, 2, 3 } };
        yield return new object[] { new MemoryStream([]), Array.Empty<byte>() };
    }

    [Theory]
    [MemberData(nameof(GetEmptyStreamData))]
    public void SetAudio_InvokeStream_GetReturnsExpected(Stream audioStream, byte[] audioBytes)
    {
        DataObject dataObject = new(DataFormatNames.WaveAudio, audioStream);
        HRESULT result = ClipboardCore.SetData(dataObject, copy: false, retryTimes: 1, retryDelay: 0);
        result.Should().Be(HRESULT.S_OK);

        ClipboardCore.GetDataObject<DataObject, ITestDataObject>(out var outData, retryTimes: 1, retryDelay: 0);
        outData.Should().NotBeNull();
        outData.GetDataPresent(DataFormatNames.WaveAudio).Should().BeTrue();
        outData.GetData(DataFormatNames.WaveAudio).Should().BeOfType<MemoryStream>().Which.ToArray().Should().Equal(audioBytes);
    }
}
