// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Formats.Nrbf;
using System.Private.Windows.Ole;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms.TestUtilities;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using static System.Windows.Forms.TestUtilities.DataObjectTestHelpers;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms.Tests;

// Note: each registered Clipboard format is an OS singleton
// and we should not run this test at the same time as other tests using the same format.
[Collection("Sequential")]
// Try up to 3 times before failing.
[UISettings(MaxAttempts = 3)]
public class ClipboardTests
{
#pragma warning disable WFDEV005 // Type or member is obsolete

    [WinFormsFact]
    public void SetText_InvokeString_GetReturnsExpected()
    {
        Clipboard.SetText("text");
        Clipboard.GetText().Should().Be("text");
        Clipboard.ContainsText().Should().BeTrue();
    }

    [WinFormsFact]
    public void Clear_InvokeMultipleTimes_Success()
    {
        Clipboard.Clear();
        Clipboard.ContainsAudio().Should().BeFalse();
        Clipboard.ContainsData("format").Should().BeFalse();
        Clipboard.ContainsFileDropList().Should().BeFalse();
        Clipboard.ContainsImage().Should().BeFalse();
        Clipboard.ContainsText().Should().BeFalse();

        Clipboard.Clear();
        Clipboard.ContainsAudio().Should().BeFalse();
        Clipboard.ContainsData("format").Should().BeFalse();
        Clipboard.ContainsFileDropList().Should().BeFalse();
        Clipboard.ContainsImage().Should().BeFalse();
        Clipboard.ContainsText().Should().BeFalse();
    }

    public static TheoryData<Func<bool>> ContainsMethodsTheoryData =>
    [
        Clipboard.ContainsAudio,
        Clipboard.ContainsFileDropList,
        Clipboard.ContainsImage,
        Clipboard.ContainsText
    ];

    [WinFormsTheory]
    [MemberData(nameof(ContainsMethodsTheoryData))]
    public void Contains_InvokeMultipleTimes_Success(Func<bool> contains)
    {
        Clipboard.Clear();
        bool result = contains.Invoke();
        contains.Invoke().Should().Be(result);
        result.Should().BeFalse();
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ContainsData_InvokeMultipleTimes_Success(string format)
    {
        bool result = Clipboard.ContainsData(format);
        Clipboard.ContainsData(format).Should().Be(result);
        result.Should().BeFalse();
    }

    [WinFormsTheory]
    [EnumData<TextDataFormat>]
    public void ContainsText_TextDataFormat_InvokeMultipleTimes_Success(TextDataFormat format)
    {
        bool result = Clipboard.ContainsText(format);
        Clipboard.ContainsText(format).Should().Be(result);
    }

    [WinFormsTheory]
    [InvalidEnumData<TextDataFormat>]
    public void ContainsText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        Action action = () => Clipboard.ContainsText(format);
        action.Should().Throw<InvalidEnumArgumentException>().WithParameterName("format");
    }

    [WinFormsFact]
    public void GetAudioStream_InvokeMultipleTimes_Success()
    {
        Stream? result = Clipboard.GetAudioStream();
        (Clipboard.GetAudioStream() == result).Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\t")]
    public void GetData_NullOrEmptyFormat_Returns_Null(string? format)
    {
        object? result = Clipboard.GetData(format!);
        result.Should().BeNull();
        result = Clipboard.GetData(format!);
        result.Should().BeNull();
    }

    [WinFormsFact]
    public void GetDataObject_InvokeMultipleTimes_Success()
    {
        DataObject result1 = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        DataObject result2 = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        result1.GetFormats().Should().BeEquivalentTo(result2.GetFormats());
    }

    [WinFormsFact]
    public void GetFileDropList_InvokeMultipleTimes_Success()
    {
        StringCollection result = Clipboard.GetFileDropList();
        Clipboard.GetFileDropList().Should().BeEquivalentTo(result);
    }

    [WinFormsFact]
    public void GetImage_InvokeMultipleTimes_Success()
    {
        Image? result = Clipboard.GetImage();
        Clipboard.GetImage().Should().BeEquivalentTo(result);
    }

    [WinFormsFact]
    public void GetText_InvokeMultipleTimes_Success()
    {
        string result = Clipboard.GetText();
        Clipboard.GetText().Should().Be(result);
    }

    [WinFormsTheory]
    [EnumData<TextDataFormat>]
    public void GetText_TextDataFormat_InvokeMultipleTimes_Success(TextDataFormat format)
    {
        string result = Clipboard.GetText(format);
        Clipboard.GetText(format).Should().Be(result);
    }

    [WinFormsTheory]
    [InvalidEnumData<TextDataFormat>]
    public void GetText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        Action action = () => Clipboard.GetText(format);
        action.Should().Throw<InvalidEnumArgumentException>().WithParameterName("format");
    }

    [WinFormsFact]
    public void SetAudio_InvokeByteArray_GetReturnsExpected()
    {
        byte[] audioBytes = [1, 2, 3];
        Clipboard.SetAudio(audioBytes);

        Clipboard.GetAudioStream().Should().BeOfType<MemoryStream>().Which.ToArray().Should().Equal(audioBytes);
        Clipboard.GetData(DataFormats.WaveAudio).Should().BeOfType<MemoryStream>().Which.ToArray().Should().Equal(audioBytes);
        Clipboard.ContainsAudio().Should().BeTrue();
        Clipboard.ContainsData(DataFormats.WaveAudio).Should().BeTrue();
    }

    [WinFormsFact]
    public void SetAudio_InvokeEmptyByteArray_GetReturnsExpected()
    {
        byte[] audioBytes = Array.Empty<byte>();
        Clipboard.SetAudio(audioBytes);

        Clipboard.GetAudioStream().Should().BeNull();
        Clipboard.GetData(DataFormats.WaveAudio).Should().BeNull();
        Clipboard.ContainsAudio().Should().BeTrue();
        Clipboard.ContainsData(DataFormats.WaveAudio).Should().BeTrue();
    }

    [WinFormsFact]
    public void SetAudio_NullAudioBytes_ThrowsArgumentNullException()
    {
        Action action = () => Clipboard.SetAudio((byte[])null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("audioBytes");
    }

    [WinFormsFact]
    public void SetAudio_InvokeStream_GetReturnsExpected()
    {
        byte[] audioBytes = [1, 2, 3];
        using MemoryStream audioStream = new(audioBytes);
        Clipboard.SetAudio(audioStream);

        Clipboard.GetAudioStream().Should().BeOfType<MemoryStream>().Which.ToArray().Should().Equal(audioBytes);
        Clipboard.GetData(DataFormats.WaveAudio).Should().BeOfType<MemoryStream>().Which.ToArray().Should().Equal(audioBytes);
        Clipboard.ContainsAudio().Should().BeTrue();
        Clipboard.ContainsData(DataFormats.WaveAudio).Should().BeTrue();
    }

    [WinFormsFact]
    public void SetAudio_InvokeEmptyStream_GetReturnsExpected()
    {
        using MemoryStream audioStream = new();
        Clipboard.SetAudio(audioStream);

        Clipboard.GetAudioStream().Should().BeNull();
        Clipboard.GetData(DataFormats.WaveAudio).Should().BeNull();
        Clipboard.ContainsAudio().Should().BeTrue();
        Clipboard.ContainsData(DataFormats.WaveAudio).Should().BeTrue();
    }

    [WinFormsFact]
    public void SetAudio_NullAudioStream_ThrowsArgumentNullException()
    {
        Action action = () => Clipboard.SetAudio((Stream)null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("audioStream");
    }

    [WinFormsFact]
    public void SetData_NullData_ThrowsArgumentNullException()
    {
        Action action = () => Clipboard.SetData("MyData", data: null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("data");
    }

    [WinFormsFact]
    public void SetData_Int_GetReturnsExpected()
    {
        Clipboard.SetData("format", 1);
        Clipboard.GetData("format").Should().Be(1);
        Clipboard.ContainsData("format").Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(null)]
    public void SetData_EmptyOrWhitespaceFormat_ThrowsArgumentException(string? format)
    {
        Action action = () => Clipboard.SetData(format!, "data");
        action.Should().Throw<ArgumentException>().WithParameterName("format");
    }

    [WinFormsFact]
    public void SetData_Null_ThrowsArgumentNullException()
    {
        try
        {
            Action action = () => Clipboard.SetData("MyData", data: null!);
            action.Should().Throw<ArgumentNullException>().WithParameterName("data");
            // Clipboard flushes format only, content is not stored.
            // GetData will hit "Data on clipboard is invalid (0x800401D3 (CLIPBRD_E_BAD_DATA))"
            Clipboard.ContainsData("MyData").Should().BeFalse();
            Clipboard.GetData("MyData").Should().BeNull();
            Clipboard.TryGetData("MyData", out string? data).Should().BeFalse();
        }
        finally
        {
            Clipboard.Clear();
        }
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData("data")]
    public void SetDataObject_InvokeObjectNotIComDataObject_GetReturnsExpected(object data)
    {
        Clipboard.SetDataObject(data);

        DataObject dataObject = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        dataObject.GetData(data.GetType()).Should().Be(data);
        Clipboard.ContainsData(data.GetType().FullName).Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData("data")]
    public void SetDataObject_InvokeObjectIComDataObject_GetReturnsExpected(object data)
    {
        DataObject dataObject = new(data);
        Clipboard.SetDataObject(dataObject);

        DataObject actual = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        actual.GetData(data.GetType()).Should().Be(data);
        Clipboard.ContainsData(data.GetType().FullName).Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(1, true)]
    [InlineData(1, false)]
    [InlineData("data", true)]
    [InlineData("data", false)]
    public void SetDataObject_InvokeObjectBoolNotIComDataObject_GetReturnsExpected(object data, bool copy)
    {
        Clipboard.SetDataObject(data, copy);

        DataObject dataObject = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        dataObject.GetData(data.GetType()).Should().Be(data);
        Clipboard.ContainsData(data.GetType().FullName).Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(1, true, 0, 0)]
    [InlineData(1, false, 1, 2)]
    [InlineData("data", true, 0, 0)]
    [InlineData("data", false, 1, 2)]
    public void SetDataObject_InvokeObjectBoolIComDataObject_GetReturnsExpected(object data, bool copy, int retryTimes, int retryDelay)
    {
        DataObject dataObject = new(data);
        Clipboard.SetDataObject(dataObject, copy, retryTimes, retryDelay);

        DataObject actual = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        actual.GetData(data.GetType()).Should().Be(data);
        Clipboard.ContainsData(data.GetType().FullName).Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(1, true, 0, 0)]
    [InlineData(1, false, 1, 2)]
    [InlineData("data", true, 0, 0)]
    [InlineData("data", false, 1, 2)]
    public void SetDataObject_InvokeObjectBoolIntIntNotIComDataObject_GetReturnsExpected(object data, bool copy, int retryTimes, int retryDelay)
    {
        Clipboard.SetDataObject(data, copy, retryTimes, retryDelay);

        DataObject dataObject = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        dataObject.GetData(data.GetType()).Should().Be(data);
        Clipboard.ContainsData(data.GetType().FullName).Should().BeTrue();
    }

    public static TheoryData<Action> SetDataObject_Null_TheoryData =>
    [
        () => Clipboard.SetDataObject(null!),
        () => Clipboard.SetDataObject(null!, copy: true),
        () => Clipboard.SetDataObject(null!, copy: true, retryTimes: 10, retryDelay: 0)
    ];

    [WinFormsTheory]
    [MemberData(nameof(SetDataObject_Null_TheoryData))]
    public void SetDataObject_NullData_ThrowsArgumentNullException(Action action)
    {
        action.Should().Throw<ArgumentNullException>().WithParameterName("data");
    }

    [WinFormsFact]
    public void SetDataObject_NegativeRetryTimes_ThrowsArgumentOutOfRangeException()
    {
        Action action = () => Clipboard.SetDataObject(new object(), copy: true, retryTimes: -1, retryDelay: 0);
        action.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("retryTimes");
    }

    [WinFormsFact]
    public void SetDataObject_NegativeRetryDelay_ThrowsArgumentOutOfRangeException()
    {
        Action action = () => Clipboard.SetDataObject(new object(), copy: true, retryTimes: 10, retryDelay: -1);
        action.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("retryDelay");
    }

    public static TheoryData<Action> NotAnStaTheoryData =>
    [
        Clipboard.Clear,
        () => Clipboard.SetAudio(Array.Empty<byte>()),
        () => Clipboard.SetAudio(new MemoryStream()),
        () => Clipboard.SetDataObject(new DataObject()),
        () => Clipboard.SetDataObject(new DataObject(), copy: true),
        () => Clipboard.SetDataObject(new DataObject(), copy: true, retryTimes: 10, retryDelay: 0),
        () => Clipboard.SetFileDropList(["filePath"]),
        () => Clipboard.SetText("text"),
        () => Clipboard.SetText("text", TextDataFormat.Text)
    ];

    [Theory] // x-thread
    [MemberData(nameof(NotAnStaTheoryData))]
    public void NotSta_ThrowsThreadStateException(Action action)
    {
        action.Should().Throw<ThreadStateException>();
    }

    [Fact] // x-thread
    public void SetImage_NotSta_ThrowsThreadStateException()
    {
        using Bitmap bitmap = new(10, 10);
        using Metafile metafile = new("bitmaps/telescope_01.wmf");
        using Metafile enhancedMetafile = new("bitmaps/milkmateya01.emf");
        Action action = () => Clipboard.SetImage(bitmap);
        action.Should().Throw<ThreadStateException>();
        action = () => Clipboard.SetImage(metafile);
        action.Should().Throw<ThreadStateException>();
        action = () => Clipboard.SetImage(enhancedMetafile);
        action.Should().Throw<ThreadStateException>();
    }

    [WinFormsFact]
    public void SetFileDropList_Invoke_GetReturnsExpected()
    {
        StringCollection filePaths =
        [
            "filePath",
            "filePath2"
        ];
        Clipboard.SetFileDropList(filePaths);

        Clipboard.GetFileDropList().Should().BeEquivalentTo(filePaths);
        Clipboard.ContainsFileDropList().Should().BeTrue();
    }

    [WinFormsFact]
    public void SetFileDropList_NullFilePaths_ThrowsArgumentNullException()
    {
        Action action = () => Clipboard.SetFileDropList(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("filePaths");
    }

    [WinFormsFact]
    public void SetFileDropList_EmptyFilePaths_ThrowsArgumentException()
    {
        Action action = static () => Clipboard.SetFileDropList([]);
        action.Should().Throw<ArgumentException>();
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData("\0")]
    public void SetFileDropList_InvalidFileInPaths_ThrowsArgumentException(string filePath)
    {
        StringCollection filePaths =
        [
            filePath
        ];
        Action action = () => Clipboard.SetFileDropList(filePaths);
        action.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void SetImage_InvokeBitmap_GetReturnsExpected()
    {
        using Bitmap bitmap = new(10, 10);
        bitmap.SetPixel(1, 2, Color.FromArgb(0x01, 0x02, 0x03, 0x04));
        Clipboard.SetImage(bitmap);

        var result = Clipboard.GetImage().Should().BeOfType<Bitmap>().Subject;
        result.Size.Should().Be(bitmap.Size);
        result.GetPixel(1, 2).Should().Be(Color.FromArgb(0xFF, 0xD2, 0xD2, 0xD2));
        Clipboard.ContainsImage().Should().BeTrue();
    }

    [WinFormsFact]
    public void SetImage_InvokeMetafile_GetReturnsExpected()
    {
        try
        {
            using Metafile metafile = new("bitmaps/telescope_01.wmf");
            using BinaryFormatterScope scope = new(enable: true);
            using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

            // SetImage fails silently and corrupts the clipboard state for anything other than a bitmap.
            Clipboard.SetImage(metafile);

            Clipboard.GetImage().Should().BeNull();
            Clipboard.ContainsImage().Should().BeTrue();
        }
        finally
        {
            Clipboard.Clear();
        }
    }

    [WinFormsFact]
    public void SetImage_InvokeEnhancedMetafile_GetReturnsExpected()
    {
        try
        {
            using Metafile metafile = new("bitmaps/milkmateya01.emf");
            using BinaryFormatterScope scope = new(enable: true);
            using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

            // SetImage fails silently and corrupts the clipboard for everything other than a bitmap.
            Clipboard.SetImage(metafile);

            Clipboard.GetImage().Should().BeNull();
            Clipboard.ContainsImage().Should().BeTrue();
        }
        finally
        {
            Clipboard.Clear();
        }
    }

    [WinFormsFact]
    public void SetImage_NullImage_ThrowsArgumentNullException()
    {
        Action action = () => Clipboard.SetImage(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("image");
    }

    [WinFormsTheory]
    [EnumData<TextDataFormat>]
    public void SetText_InvokeStringTextDataFormat_GetReturnsExpected(TextDataFormat format)
    {
        Clipboard.SetText("text", format);
        Clipboard.GetText(format).Should().Be("text");
        Clipboard.ContainsText(format).Should().BeTrue();
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void SetText_NullOrEmptyText_ThrowsArgumentNullException(string text)
    {
        Action action = () => Clipboard.SetText(text);
        action.Should().Throw<ArgumentNullException>().WithParameterName("text");
        action = () => Clipboard.SetText(text, TextDataFormat.Text);
        action.Should().Throw<ArgumentNullException>().WithParameterName("text");
    }

    [WinFormsTheory]
    [InvalidEnumData<TextDataFormat>]
    public void SetText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        Action action = () => Clipboard.SetText("text", format);
        action.Should().Throw<InvalidEnumArgumentException>().WithParameterName("format");
    }

    [WinFormsFact]
    public void SetData_CustomFormat_Color()
    {
        string format = nameof(SetData_CustomFormat_Color);
        Clipboard.SetData(format, Color.Black);

        Clipboard.ContainsData(format).Should().BeTrue();
        Clipboard.GetData(format).Should().Be(Color.Black);
    }

    [WinFormsFact]
    public void SetData_CustomFormat_Exception_BinaryFormatterDisabled_SerializesException()
    {
        using BinaryFormatterScope scope = new(enable: false);
        string format = nameof(SetData_CustomFormat_Exception_BinaryFormatterDisabled_SerializesException);

        // This will fail because BinaryFormatter is disabled in the clipboard APIs, thus NotSupportedException
        // will be put on the Clipboard instead.
        Clipboard.SetData(format, new FileNotFoundException());
        Clipboard.ContainsData(format).Should().BeTrue();
        // However we don't need binary formatter to read this exception off of the clipboard.
        Clipboard.TryGetData(format, out NotSupportedException? exception).Should().BeTrue();
        exception.Should().NotBeNull();
        Clipboard.GetData(format).Should().BeOfType<NotSupportedException>();
    }

    [WinFormsFact]
    public unsafe void GetReturnsProxy()
    {
        DataObject data = new();
        using var dataScope = ComHelpers.GetComScope<Com.IDataObject>(data);
        PInvokeCore.OleSetClipboard(dataScope).Succeeded.Should().BeTrue();

        using ComScope<Com.IDataObject> proxy = new(null);
        PInvokeCore.OleGetClipboard(proxy).Succeeded.Should().BeTrue();
        ((nint)proxy.Value).Should().NotBe((nint)dataScope.Value);

        using var dataUnknown = dataScope.Query<Com.IUnknown>();
        using var proxyUnknown = proxy.Query<Com.IUnknown>();
        ((nint)proxyUnknown.Value).Should().NotBe((nint)dataUnknown.Value);

        // The proxy does not know about this interface, it should give back the real pointer.
        using var realDataPointer = proxy.Query<Com.IComCallableWrapper>();
        using var realDataPointerUnknown = realDataPointer.Query<Com.IUnknown>();
        ((nint)proxyUnknown.Value).Should().NotBe((nint)realDataPointerUnknown.Value);
        ((nint)dataUnknown.Value).Should().Be((nint)realDataPointerUnknown.Value);
    }

    [WinFormsFact]
    public void SetDataObject_DerivedDataObject_ReturnsExpected()
    {
        DerivedDataObject derived = new();
        Clipboard.SetDataObject(derived);
        Clipboard.GetDataObject().Should().BeSameAs(derived);
    }

    [WinFormsFact]
    public void GetSet_RoundTrip_ReturnsExpected()
    {
        CustomDataObject realDataObject = new();
        Clipboard.SetDataObject(realDataObject);

        IDataObject? clipboardDataObject = Clipboard.GetDataObject();
        clipboardDataObject.Should().BeSameAs(realDataObject);
        clipboardDataObject!.GetDataPresent("Foo").Should().BeTrue();
        clipboardDataObject.GetData("Foo").Should().Be("Bar");
    }

    private class CustomDataObject : IDataObject, ComTypes.IDataObject
    {
        [DllImport("shell32.dll")]
        public static extern int SHCreateStdEnumFmtEtc(uint cfmt, ComTypes.FORMATETC[] afmt, out ComTypes.IEnumFORMATETC ppenumFormatEtc);

        int ComTypes.IDataObject.DAdvise(ref ComTypes.FORMATETC pFormatetc, ComTypes.ADVF advf, ComTypes.IAdviseSink adviseSink, out int connection) => throw new NotImplementedException();
        void ComTypes.IDataObject.DUnadvise(int connection) => throw new NotImplementedException();
        int ComTypes.IDataObject.EnumDAdvise(out ComTypes.IEnumSTATDATA enumAdvise) => throw new NotImplementedException();
        ComTypes.IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(ComTypes.DATADIR direction)
        {
            if (direction == ComTypes.DATADIR.DATADIR_GET)
            {
                // Create enumerator and return it
                ComTypes.IEnumFORMATETC enumerator;
                if (SHCreateStdEnumFmtEtc(0, [], out enumerator) == 0)
                {
                    return enumerator;
                }
            }

            throw new NotImplementedException();
        }

        int ComTypes.IDataObject.GetCanonicalFormatEtc(ref ComTypes.FORMATETC formatIn, out ComTypes.FORMATETC formatOut) => throw new NotImplementedException();
        object IDataObject.GetData(string format, bool autoConvert) => format == "Foo" ? "Bar" : null!;
        object IDataObject.GetData(string format) => format == "Foo" ? "Bar" : null!;
        object IDataObject.GetData(Type format) => null!;
        void ComTypes.IDataObject.GetData(ref ComTypes.FORMATETC format, out ComTypes.STGMEDIUM medium) => throw new NotImplementedException();
        void ComTypes.IDataObject.GetDataHere(ref ComTypes.FORMATETC format, ref ComTypes.STGMEDIUM medium) => throw new NotImplementedException();
        bool IDataObject.GetDataPresent(string format, bool autoConvert) => format == "Foo";
        bool IDataObject.GetDataPresent(string format) => format == "Foo";
        bool IDataObject.GetDataPresent(Type format) => false;
        string[] IDataObject.GetFormats(bool autoConvert) => ["Foo"];
        string[] IDataObject.GetFormats() => ["Foo"];
        int ComTypes.IDataObject.QueryGetData(ref ComTypes.FORMATETC format) => throw new NotImplementedException();
        void IDataObject.SetData(string format, bool autoConvert, object? data) => throw new NotImplementedException();
        void IDataObject.SetData(string format, object? data) => throw new NotImplementedException();
        void IDataObject.SetData(Type format, object? data) => throw new NotImplementedException();
        void IDataObject.SetData(object? data) => throw new NotImplementedException();
        void ComTypes.IDataObject.SetData(ref ComTypes.FORMATETC formatIn, ref ComTypes.STGMEDIUM medium, bool release) => throw new NotImplementedException();
    }

    [DllImport("user32.dll")]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll")]
    private static extern bool OpenClipboard(HWND hWndNewOwner);

    [DllImport("user32.dll")]
    private static extern bool SetClipboardData(uint uFormat, HANDLE data);

    [WinFormsFact]
    public unsafe void RawSetClipboardData_ReturnsExpected()
    {
        Clipboard.Clear();

        OpenClipboard(HWND.Null).Should().BeTrue();
        string testString = "test";
        SetClipboardData((uint)CLIPBOARD_FORMAT.CF_UNICODETEXT, (HANDLE)Marshal.StringToHGlobalUni(testString));
        CloseClipboard().Should().BeTrue();

        DataObject dataObject = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        dataObject.GetData(DataFormats.Text).Should().Be(testString);

        Clipboard.ContainsText().Should().BeTrue();
        Clipboard.ContainsData(DataFormats.Text).Should().BeTrue();
        Clipboard.ContainsData(DataFormats.UnicodeText).Should().BeTrue();

        Clipboard.GetText().Should().Be(testString);
        Clipboard.GetText(TextDataFormat.Text).Should().Be(testString);
        Clipboard.GetText(TextDataFormat.UnicodeText).Should().Be(testString);

        Clipboard.GetData("System.String").Should().BeNull();
    }

    [WinFormsFact]
    public void BinaryFormatter_AppContextSwitch()
    {
        // Test the switch to ensure it works as expected in the context of this test assembly.
        CoreAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization.Should().BeFalse();

        using (BinaryFormatterInClipboardDragDropScope scope = new(enable: true))
        {
            CoreAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization.Should().BeTrue();
        }

        CoreAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization.Should().BeFalse();

        using (BinaryFormatterInClipboardDragDropScope scope = new(enable: false))
        {
            CoreAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization.Should().BeFalse();
        }

        CoreAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization.Should().BeFalse();
    }

    [WinFormsFact]
    public void NrbfSerializer_AppContextSwitch()
    {
        // Test the switch to ensure it works as expected in the context of this test assembly.
        CoreAppContextSwitches.ClipboardDragDropEnableNrbfSerialization.Should().BeTrue();

        using (NrbfSerializerInClipboardDragDropScope scope = new(enable: false))
        {
            CoreAppContextSwitches.ClipboardDragDropEnableNrbfSerialization.Should().BeFalse();
        }

        CoreAppContextSwitches.ClipboardDragDropEnableNrbfSerialization.Should().BeTrue();

        using (NrbfSerializerInClipboardDragDropScope scope = new(enable: true))
        {
            CoreAppContextSwitches.ClipboardDragDropEnableNrbfSerialization.Should().BeTrue();
        }

        CoreAppContextSwitches.ClipboardDragDropEnableNrbfSerialization.Should().BeTrue();
    }

    [WinFormsFact]
    public void TryGetInt_ReturnsExpected()
    {
        int expected = 101;
        using (BinaryFormatterScope scope = new(enable: true))
        {
            Clipboard.SetData("TestData", expected);
        }

        Clipboard.TryGetData("TestData", out int? data).Should().BeFalse();
        data.HasValue.Should().BeFalse();
    }

    [WinFormsFact]
    public void TryGetTestData()
    {
        TestData expected = new(DateTime.Now);
        string format = "TestData";
        using ClipboardBinaryFormatterFullCompatScope scope = new();
        Clipboard.SetData(format, expected);

        Clipboard.TryGetData(format, TestData.TestDataResolver, out TestData? data).Should().BeTrue();
        var result = data.Should().BeOfType<TestData>().Subject;
        expected.Equals(result);

        // We are still in the less safe switch configuration, but now we prefer the
        // NRBF deserialization over the BinaryFormatter full compatibility mode.
        using NrbfSerializerInClipboardDragDropScope nrbfScope = new(enable: true);
        Clipboard.TryGetData(format, TestData.TestDataResolver, out TestData? testData).Should().BeTrue();
        expected.Equals(testData.Should().BeOfType<TestData>().Subject);

        // Resolver is required to read this type.
        Clipboard.TryGetData(format, out testData).Should().BeFalse();
        testData.Should().BeNull();

        // This is the safe switch configuration, custom types can't be resolved
        using NrbfSerializerInClipboardDragDropScope nrbfScope2 = new(enable: false);
        using BinaryFormatterInClipboardDragDropScope binaryScope2 = new(enable: false);
        Clipboard.TryGetData(format, TestData.TestDataResolver, out testData).Should().BeFalse();
        testData.Should().BeNull();
    }

    [Serializable]
    private class TestData
    {
        public TestData(DateTime dateTime)
        {
            _count = 2;
            _dateTime = dateTime;
        }

        private readonly int _count;
        private readonly DateTime _dateTime;
        private readonly TestData1 _testData1 = new();

        public void Equals(TestData actual)
        {
            _count.Should().Be(actual._count);
            _dateTime.Should().Be(actual._dateTime);
            _testData1.Text.Should().Be(actual._testData1.Text);
        }

        public static Type TestDataResolver(TypeName typeName)
        {
            string fullName = typeName.FullName;
            if (typeof(TestData).FullName == fullName)
            {
                return typeof(TestData);
            }

            if (typeof(TestData1).FullName == fullName)
            {
                return typeof(TestData1);
            }

            throw new NotSupportedException($"Can't resolve {typeName.AssemblyQualifiedName}");
        }
    }

    [Serializable]
    public class TestData1
    {
        public string Text { get; } = "a";
    }

    [WinFormsFact]
    public void TryGetObject_Throws()
    {
        object expected = new();
        string format = "TestData";
        Action tryGetData = () => Clipboard.TryGetData(format, out object? data);

        using BinaryFormatterScope scope = new(enable: true);

        using (BinaryFormatterInClipboardDragDropScope binaryFormatterScope = new(enable: true))
        {
            Clipboard.SetData(format, expected);

            tryGetData.Should().Throw<NotSupportedException>();
        }

        tryGetData.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void TryGetRectangleAsObject_Throws()
    {
        Rectangle expected = new(1, 1, 2, 2);
        string format = "TestData";
        using BinaryFormatterScope scope = new(enable: true);
        Clipboard.SetData(format, expected);

        Action tryGetData = () => Clipboard.TryGetData(format, out object? data);
        tryGetData.Should().Throw<NotSupportedException>();

        using BinaryFormatterInClipboardDragDropScope binaryFormatterScope = new(enable: true);
        tryGetData.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void TryGetNotSupportedException()
    {
        object expected = new();
        string format = "TestData";
        Action tryGetData = () => Clipboard.TryGetData(format, out object? data);

        using BinaryFormatterScope scope = new(enable: true);
        // BinaryFormatterInClipboardDragDropScope is off, the NotSupportedException is written to the clipboard.
        Clipboard.SetData(format, expected);

        using (BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true))
        {
            Clipboard.SetData(format, expected);

            tryGetData.Should().Throw<NotSupportedException>();
        }

        tryGetData.Should().Throw<NotSupportedException>();
    }

    [WinFormsFact]
    public void TryGetOffsetArray()
    {
        Array value = Array.CreateInstance(typeof(uint), lengths: [2, 3], lowerBounds: [1, 2]);
        value.SetValue(101u, 1, 2);
        value.SetValue(102u, 1, 3);
        value.SetValue(103u, 1, 4);
        value.SetValue(201u, 2, 2);
        value.SetValue(202u, 2, 3);
        value.SetValue(203u, 2, 4);

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        Clipboard.SetData("test", value);

        var result = Clipboard.GetData("test").Should().BeOfType<uint[,]>().Subject;
        result.Rank.Should().Be(2);
        result.GetLength(0).Should().Be(2);
        result.GetLength(1).Should().Be(3);
        result.GetLowerBound(0).Should().Be(1);
        result.GetLowerBound(1).Should().Be(2);
        result.GetValue(1, 2).Should().Be(101u);
        result.GetValue(1, 3).Should().Be(102u);
        result.GetValue(1, 4).Should().Be(103u);
        result.GetValue(2, 2).Should().Be(201u);
        result.GetValue(2, 3).Should().Be(202u);
        result.GetValue(2, 4).Should().Be(203u);

        Clipboard.TryGetData("test", out uint[,]? data).Should().BeFalse();
        data.Should().BeNull();

        Clipboard.TryGetData(
            "test",
            (typeName) => typeName.FullName == typeof(uint[,]).FullName
                ? typeof(uint[,])
                : throw new NotSupportedException(),
            out data).Should().BeTrue();

        // FluentAssertions doesn't support non-zero indexed arrays.
        Assert.Equal(data, value);
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void SetDataAsJson_EmptyFormat_Throws(string? format)
    {
        Action action = () => Clipboard.SetDataAsJson(format!, 1);
        action.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void SetDataAsJson_DataObject_Throws()
    {
        string format = "format";
        Action action = () => Clipboard.SetDataAsJson(format, new DataObject());
        action.Should().Throw<ArgumentException>();
        Action clipboardSet2 = () => Clipboard.SetDataAsJson(format, new DerivedDataObject());
        clipboardSet2.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void SetDataAsJson_WithGeneric_ReturnsExpected()
    {
        List<Point> generic1 = [];
        string format = "list";
        Clipboard.SetDataAsJson(format, generic1);
        DataObject dataObject = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;

        // Reading a JSON-serialized payload through the untyped APIs always works.
        dataObject.GetData(format).Should().BeEquivalentTo(generic1);

        Clipboard.TryGetData(format, out List<Point>? points).Should().BeTrue();
        points.Should().BeEquivalentTo(generic1);

        // List of primitives is an intrinsic type, ensure it is treated as JSON.
        List<int> generic2 = [];
        Clipboard.SetDataAsJson(format, generic2);
        dataObject = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        var result2 = dataObject.GetData(format);
        result2.Should().BeEquivalentTo(generic2);

        Clipboard.TryGetData(format, out List<int>? intList).Should().BeTrue();
        intList.Should().BeEquivalentTo(generic2);
    }

    [WinFormsFact]
    public void SetDataAsJson_ReturnsExpected()
    {
        SimpleTestData testData = new() { X = 1, Y = 1 };

        Clipboard.SetDataAsJson("testDataFormat", testData);
        IDataObject dataObject = Clipboard.GetDataObject().Should().BeAssignableTo<IDataObject>().Subject;
        dataObject.GetDataPresent("testDataFormat").Should().BeTrue();
        dataObject.TryGetData("testDataFormat", out SimpleTestData deserialized).Should().BeTrue();
        deserialized.Should().BeEquivalentTo(testData);
    }

    [WinFormsFact]
    public void SetDataAsJson_GetData()
    {
        SimpleTestData testData = new() { X = 1, Y = 1 };
        // Note that this simulates out of process scenario.
        Clipboard.SetDataAsJson("test", testData);

        Clipboard.GetData("test").Should().Be(testData);

        using BinaryFormatterInClipboardDragDropScope scope = new(enable: true);
        Clipboard.GetData("test").Should().Be(testData);

        using BinaryFormatterScope scope2 = new(enable: true);
        Clipboard.GetData("test").Should().Be(testData);
    }

    [WinFormsTheory]
    [BoolData]
    public void SetDataObject_WithJson_ReturnsExpected(bool copy)
    {
        SimpleTestData testData = new() { X = 1, Y = 1 };

        DataObject dataObject = new();
        dataObject.SetDataAsJson("testDataFormat", testData);

        Clipboard.SetDataObject(dataObject, copy);
        ITypedDataObject returnedDataObject = Clipboard.GetDataObject().Should().BeAssignableTo<ITypedDataObject>().Subject;
        returnedDataObject.TryGetData("testDataFormat", out SimpleTestData deserialized).Should().BeTrue();
        deserialized.Should().BeEquivalentTo(testData);

        // JsonData should work via legacy APIs.
        var legacyResult = Clipboard.GetData("testDataFormat");
        legacyResult.Should().Be(testData);
    }

    [WinFormsTheory]
    [BoolData]
    public void SetDataObject_WithMultipleData_ReturnsExpected(bool copy)
    {
        SimpleTestData testData1 = new() { X = 1, Y = 1 };
        SimpleTestData testData2 = new() { Y = 2, X = 2 };
        DataObject data = new();
        data.SetDataAsJson("testData1", testData1);
        data.SetDataAsJson("testData2", testData2);
        data.SetData("Mystring", "test");
        Clipboard.SetDataObject(data, copy);

        Clipboard.TryGetData("testData1", out SimpleTestData deserializedTestData1).Should().BeTrue();
        deserializedTestData1.Should().BeEquivalentTo(testData1);
        Clipboard.TryGetData("testData2", out SimpleTestData deserializedTestData2).Should().BeTrue();
        deserializedTestData2.Should().BeEquivalentTo(testData2);
        Clipboard.TryGetData("Mystring", out string? deserializedString).Should().BeTrue();
        deserializedString.Should().Be("test");
    }

    [WinFormsFact]
    public unsafe void Deserialize_FromStream_Manually()
    {
        // This test demonstrates how a user can manually deserialize JsonData<T> that has been serialized onto
        // the clipboard from stream. This may need to be done if type JsonData<T> does not exist in the .NET version
        // the user is utilizing.
        SimpleTestData testData = new() { X = 1, Y = 1 };
        Clipboard.SetDataAsJson("testFormat", testData);

        // Manually retrieve the serialized stream.
        ComTypes.IDataObject dataObject = Clipboard.GetDataObject().Should().BeAssignableTo<ComTypes.IDataObject>().Subject;
        ComTypes.FORMATETC formatetc = new()
        {
            cfFormat = (short)DataFormats.GetFormat("testFormat").Id,
            dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            tymed = ComTypes.TYMED.TYMED_HGLOBAL
        };

        dataObject.GetData(ref formatetc, out ComTypes.STGMEDIUM medium);
        HGLOBAL hglobal = (HGLOBAL)medium.unionmember;
        MemoryStream? stream = null;
        try
        {
            try
            {
                void* buffer = PInvokeCore.GlobalLock(hglobal);
                int size = (int)PInvokeCore.GlobalSize(hglobal);
                byte[] bytes = new byte[size];
                Marshal.Copy((nint)buffer, bytes, 0, size);
                // this comes from DataObject.Composition.s_serializedObjectID
                int index = 16;
                stream = new MemoryStream(bytes, index, bytes.Length - index);
            }
            finally
            {
                PInvokeCore.GlobalUnlock(hglobal);
            }

            stream.Should().NotBeNull();
            // Use NrbfDecoder to decode the stream and rehydrate the type.
            SerializationRecord record = NrbfDecoder.Decode(stream);
            ClassRecord types = record.Should().BeAssignableTo<ClassRecord>().Subject;
            types.HasMember("<JsonBytes>k__BackingField").Should().BeTrue();
            types.HasMember("<InnerTypeAssemblyQualifiedName>k__BackingField").Should().BeTrue();
            SZArrayRecord<byte> byteData = types.GetRawValue("<JsonBytes>k__BackingField").Should().BeAssignableTo<SZArrayRecord<byte>>().Subject;
            string innerTypeAssemblyQualifiedName = types.GetRawValue("<InnerTypeAssemblyQualifiedName>k__BackingField").Should().BeOfType<string>().Subject;
            TypeName.TryParse(innerTypeAssemblyQualifiedName, out TypeName? innerTypeName).Should().BeTrue();
            TypeName checkedResult = innerTypeName.Should().BeOfType<TypeName>().Subject;

            typeof(SimpleTestData).AssemblyQualifiedName.Should().Be(checkedResult.AssemblyQualifiedName);
            typeof(SimpleTestData).ToTypeName().Matches(checkedResult).Should().BeTrue();

            JsonSerializer.Deserialize(byteData.GetArray(), typeof(SimpleTestData)).Should().BeEquivalentTo(testData);
        }
        finally
        {
            stream?.Dispose();
        }
    }

    [WinFormsFact]
    public void JsonError_NotRethrown()
    {
        using Font font = new("Microsoft Sans Serif", emSize: 10);
        byte[] serialized = JsonSerializer.SerializeToUtf8Bytes(font);
        Action a1 = () => JsonSerializer.Deserialize<Font>(serialized);
        a1.Should().Throw<NotSupportedException>();

        string format = "font";
        Clipboard.SetDataAsJson(format, font);
        Clipboard.TryGetData(format, out Font? result).Should().BeFalse();
        result.Should().BeNull();

        DataObject dataObject = Clipboard.GetDataObject().Should().BeAssignableTo<DataObject>().Subject;
        dataObject.TryGetData(format, out result).Should().BeFalse();
        result.Should().BeNull();
    }

    [WinFormsTheory]
    [BoolData]
    public void CustomDataObject_AvoidBinaryFormatter(bool copy)
    {
        string format = "customFormat";
        SimpleTestData data = new() { X = 1, Y = 1 };
        Clipboard.SetData(format, data);
        // BinaryFormatter not enabled.
        Clipboard.GetData(format).Should().BeOfType<NotSupportedException>();

        Clipboard.Clear();
        JsonDataObject jsonDataObject = new();
        jsonDataObject.SetData(format, data);

        Clipboard.SetDataObject(jsonDataObject, copy);

        if (copy)
        {
            // Pasting in different process has been simulated. Manual Json deserialization will need to occur.
            IDataObject received = Clipboard.GetDataObject().Should().BeAssignableTo<IDataObject>().Subject;
            received.Should().NotBe(jsonDataObject);
            received.Should().BeAssignableTo<ITypedDataObject>();
            byte[] jsonBytes = Clipboard.GetData(format).Should().BeOfType<byte[]>().Subject;
            JsonSerializer.Deserialize(jsonBytes, typeof(SimpleTestData)).Should().BeEquivalentTo(data);
            received.TryGetData(format, out byte[]? jsonBytes1).Should().BeTrue();
            jsonBytes1.Should().BeEquivalentTo(jsonBytes);
        }
        else
        {
            JsonDataObject received = Clipboard.GetDataObject().Should().BeOfType<JsonDataObject>().Subject;
            received.Should().Be(jsonDataObject);
            received.Deserialize<SimpleTestData>(format).Should().BeEquivalentTo(data);
            Action tryGetData = () => received.TryGetData(format, out byte[]? _);
            tryGetData.Should().Throw<NotSupportedException>();
        }
    }

    // Test class to demonstrate one way to write IDataObject to totally control serialization/deserialization
    // and have it avoid BinaryFormatter.
    private class JsonDataObject : IDataObject, ComTypes.IDataObject
    {
        private readonly Dictionary<string, byte[]> _formatToJson = [];
        private readonly Dictionary<string, string> _formatToTypeName = [];

        public T? Deserialize<T>(string format)
        {
            if (typeof(T).AssemblyQualifiedName != _formatToTypeName[format])
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(_formatToJson[format]);
        }

        public object GetData(string format, bool autoConvert) => GetData(format);
        public object GetData(string format) => _formatToJson[format];
        public object GetData(Type format) => throw new NotImplementedException();
        public bool GetDataPresent(string format, bool autoConvert) => throw new NotImplementedException();
        public bool GetDataPresent(string format) => _formatToJson.ContainsKey(format);
        public bool GetDataPresent(Type format) => throw new NotImplementedException();
        public string[] GetFormats(bool autoConvert) => throw new NotImplementedException();
        public string[] GetFormats() => _formatToJson.Keys.ToArray();
        public void SetData(string format, bool autoConvert, object? data) => throw new NotImplementedException();
        public void SetData(string format, object? data)
        {
            _formatToTypeName.Add(format, data!.GetType().AssemblyQualifiedName!);
            _formatToJson.Add(format, JsonSerializer.SerializeToUtf8Bytes(data));
        }

        public void SetData(Type format, object? data) => throw new NotImplementedException();
        public void SetData(object? data) => throw new NotImplementedException();

        public int DAdvise(ref ComTypes.FORMATETC pFormatetc, ComTypes.ADVF advf, ComTypes.IAdviseSink adviseSink, out int connection) => throw new NotImplementedException();
        public void DUnadvise(int connection) => throw new NotImplementedException();
        public int EnumDAdvise(out ComTypes.IEnumSTATDATA? enumAdvise) => throw new NotImplementedException();
        public ComTypes.IEnumFORMATETC EnumFormatEtc(ComTypes.DATADIR direction) => throw new NotImplementedException();
        public int GetCanonicalFormatEtc(ref ComTypes.FORMATETC formatIn, out ComTypes.FORMATETC formatOut) => throw new NotImplementedException();
        public void SetData(ref ComTypes.FORMATETC formatIn, ref ComTypes.STGMEDIUM medium, bool release) => throw new NotImplementedException();
        public void GetData(ref ComTypes.FORMATETC format, out ComTypes.STGMEDIUM medium) => throw new NotImplementedException();
        public void GetDataHere(ref ComTypes.FORMATETC format, ref ComTypes.STGMEDIUM medium) => throw new NotImplementedException();
        public int QueryGetData(ref ComTypes.FORMATETC format) => throw new NotImplementedException();
    }

    private class DerivedDataObject : DataObject { }

    [WinFormsFact]
    public void SetDataAsJson_NullData_Throws()
    {
        Action clipboardSet = () => Clipboard.SetDataAsJson<string>("format", null!);
        clipboardSet.Should().Throw<ArgumentNullException>();
    }

    [WinFormsFact]
    public void SetData_Text_Format_AllUpper()
    {
        // The fact that casing on input matters is likely incorrect, but behavior has been this way.
        Clipboard.SetData("TEXT", "Hello, World!");
        Clipboard.ContainsText().Should().BeTrue();
        Clipboard.ContainsData("TEXT").Should().BeTrue();
        Clipboard.ContainsData(DataFormats.Text).Should().BeTrue();
        Clipboard.ContainsData(DataFormats.UnicodeText).Should().BeTrue();

        IDataObject dataObject = Clipboard.GetDataObject().Should().BeAssignableTo<IDataObject>().Subject;
        string[] formats = dataObject.GetFormats();
        formats.Should().BeEquivalentTo(["System.String", "UnicodeText", "Text"]);

        formats = dataObject.GetFormats(autoConvert: false);
        formats.Should().BeEquivalentTo(["Text"]);

        // CLIPBRD_E_BAD_DATA returned when trying to get clipboard data.
        Clipboard.GetText().Should().BeEmpty();
        Clipboard.GetText(TextDataFormat.Text).Should().BeEmpty();
        Clipboard.GetText(TextDataFormat.UnicodeText).Should().BeEmpty();

        Clipboard.GetData("System.String").Should().BeNull();
        Clipboard.GetData("TEXT").Should().BeNull();
    }

    [WinFormsFact]
    public void SetData_Text_Format_CanonicalCase()
    {
        string expected = "Hello, World!";
        Clipboard.SetData("Text", expected);
        Clipboard.ContainsText().Should().BeTrue();
        Clipboard.ContainsData("TEXT").Should().BeTrue();
        Clipboard.ContainsData(DataFormats.Text).Should().BeTrue();
        Clipboard.ContainsData(DataFormats.UnicodeText).Should().BeTrue();

        IDataObject dataObject = Clipboard.GetDataObject().Should().BeAssignableTo<IDataObject>().Subject;
        string[] formats = dataObject.GetFormats();
        formats.Should().BeEquivalentTo(["System.String", "UnicodeText", "Text"]);

        formats = dataObject.GetFormats(autoConvert: false);
        formats.Should().BeEquivalentTo(["System.String", "UnicodeText", "Text"]);

        Clipboard.GetText().Should().Be(expected);
        Clipboard.GetText(TextDataFormat.Text).Should().Be(expected);
        Clipboard.GetText(TextDataFormat.UnicodeText).Should().Be(expected);

        Clipboard.GetData("System.String").Should().Be(expected);

        // Case sensitivity matters so we end up reading stream/object from HGLOBAL instead of string.
        MemoryStream stream = Clipboard.GetData("TEXT").Should().BeOfType<MemoryStream>().Subject;
        byte[] array = stream.ToArray();
        array.Should().BeEquivalentTo("Hello, World!\0"u8.ToArray());
    }

    [WinFormsFact]
    public void SetDataObject_Text()
    {
        string expected = "Hello, World!";
        Clipboard.SetDataObject(expected);
        Clipboard.ContainsText().Should().BeTrue();
        Clipboard.ContainsData("TEXT").Should().BeTrue();
        Clipboard.ContainsData(DataFormats.Text).Should().BeTrue();
        Clipboard.ContainsData(DataFormats.UnicodeText).Should().BeTrue();

        IDataObject dataObject = Clipboard.GetDataObject().Should().BeAssignableTo<IDataObject>().Subject;
        string[] formats = dataObject.GetFormats();
        formats.Should().BeEquivalentTo(["System.String", "UnicodeText", "Text"]);

        formats = dataObject.GetFormats(autoConvert: false);
        formats.Should().BeEquivalentTo(["System.String", "UnicodeText", "Text"]);

        Clipboard.GetText().Should().Be(expected);
        Clipboard.GetText(TextDataFormat.Text).Should().Be(expected);
        Clipboard.GetText(TextDataFormat.UnicodeText).Should().Be(expected);

        Clipboard.GetData("System.String").Should().Be(expected);

        // Case sensitivity matters so we end up reading stream/object from HGLOBAL instead of string.
        MemoryStream stream = Clipboard.GetData("TEXT").Should().BeOfType<MemoryStream>().Subject;
        byte[] array = stream.ToArray();
        array.Should().BeEquivalentTo("Hello, World!\0"u8.ToArray());
    }

    [WinFormsFact]
    public void DerivedDataObject_DataPresent()
    {
        // https://github.com/dotnet/winforms/issues/12789
        SomeDataObject data = new();

        // This was provided as a workaround for the above and should not break, but should
        // also work without it.
        data.SetData(SomeDataObject.Format, data);

        Clipboard.SetDataObject(data);
        Clipboard.ContainsData(SomeDataObject.Format).Should().BeTrue();
        Clipboard.GetDataObject()!.GetDataPresent(SomeDataObject.Format).Should().BeTrue();

        data = new();
        Clipboard.SetDataObject(data);
        Clipboard.ContainsData(SomeDataObject.Format).Should().BeTrue();
        Clipboard.GetDataObject()!.GetDataPresent(SomeDataObject.Format).Should().BeTrue();
    }

    public class SomeDataObject : DataObject
    {
        public static string Format => "SomeDataObjectId";
        public override string[] GetFormats() => [Format];

        public override bool GetDataPresent(string format, bool autoConvert)
            => format == Format || base.GetDataPresent(format, autoConvert);
    }

    [WinFormsTheory]
    [BoolData]
    public void RoundTrip_DataObject_SupportsTypedInterface(bool copy) =>
        CustomDataObject_RoundTrip_SupportsTypedInterface<DataObject>(copy);

    [WinFormsTheory]
    [BoolData]
    public void RoundTrip_ManagedAndRuntimeDataObject_SupportsTypedInterface(bool copy) =>
        CustomDataObject_RoundTrip_SupportsTypedInterface<ManagedAndRuntimeDataObject>(copy);

    [WinFormsTheory]
    [BoolData]
    public void RoundTrip_TypedAndRuntimeDataObject_SupportsTypedInterface(bool copy) =>
        CustomDataObject_RoundTrip_SupportsTypedInterface<TypedAndRuntimeDataObject>(copy);

    [WinFormsTheory]
    [BoolData]
    public void RoundTrip_TypedDataObject_SupportsTypedInterface(bool copy) =>
        CustomDataObject_RoundTrip_SupportsTypedInterface<TypedDataObject>(copy);

    [WinFormsTheory]
    [BoolData]
    public void RoundTrip_ManagedDataObject_SupportsTypedInterface(bool copy) =>
        CustomDataObject_RoundTrip_SupportsTypedInterface<ManagedDataObject>(copy);

    [WinFormsTheory]
    [BoolData]
    public void RoundTrip_Object_SupportsTypedInterface(bool copy)
    {
        SerializableTestData data = new();
        string format = typeof(SerializableTestData).FullName!;

        // Opt-in into access to the binary formatted stream.
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);
        // We need the BinaryFormatter to flush the data from the managed object to the HGLOBAL
        // and to write data to HGLOBAL as a binary formatted stream now if it hadn't been flushed.
        using BinaryFormatterScope scope = new(enable: true);

        Clipboard.SetDataObject(data, copy);

        DataObject received = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;

        received.TryGetData(
            format,
            (TypeName name) => name.FullName == typeof(SerializableTestData).FullName ? typeof(SerializableTestData) : null,
            autoConvert: false,
            out SerializableTestData? result).Should().BeTrue();

        result.Should().BeEquivalentTo(data);

        Clipboard.TryGetData(
            format,
            (TypeName name) => name.FullName == typeof(SerializableTestData).FullName ? typeof(SerializableTestData) : null,
            out result).Should().BeTrue();

        result.Should().BeEquivalentTo(data);
    }

    private static void CustomDataObject_RoundTrip_SupportsTypedInterface<T>(bool copy) where T : IDataObject, new()
    {
        SerializableTestData data = new();
        T testDataObject = new();
        string format = ManagedDataObject.s_format;
        testDataObject.SetData(format, data);

        // Opt-in into access the binary formatted stream.
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: copy);
        // We need the BinaryFormatter to flush the data from the managed object to the HGLOBAL.
        using (BinaryFormatterScope scope = new(enable: copy))
        {
            Clipboard.SetDataObject(testDataObject, copy);
        }

        // copy == true => data was flushed to HGLOBAL and we read it with a WinForms DataObject.
        // Otherwise this is the user-implemented ITypedDataObject or the WinForms wrapper.
        if (copy || typeof(T).IsAssignableTo(typeof(ITypedDataObject)))
        {
            // User types always should require BinaryFormatter to be enabled unless they are using TrySerializeAsJson.
            using BinaryFormatterScope scope = new(enable: copy);
            ITypedDataObject received = Clipboard.GetDataObject().Should().BeAssignableTo<ITypedDataObject>().Subject;

            // Need an explict resolver to hit the BinaryFormatter path if the data was copied out.
            received.TryGetData(format, out SerializableTestData? result).Should().Be(!copy);
            if (copy)
            {
                result.Should().BeNull();
            }
            else
            {
                result.Should().BeEquivalentTo(data);
            }

            received.TryGetData(
                format,
                (TypeName name) => name.FullName == typeof(SerializableTestData).FullName ? typeof(SerializableTestData) : null,
                autoConvert: false,
                out result).Should().BeTrue();

            result.Should().BeEquivalentTo(data);

            Clipboard.TryGetData(format, out result).Should().Be(!copy);
            if (copy)
            {
                result.Should().BeNull();
            }
            else
            {
                result.Should().BeEquivalentTo(data);
            }

            Clipboard.TryGetData(
                format,
                (TypeName name) => name.FullName == typeof(SerializableTestData).FullName ? typeof(SerializableTestData) : null,
                out result).Should().BeTrue();

            result.Should().BeEquivalentTo(data);
        }
        else
        {
            T received = Clipboard.GetDataObject().Should().BeOfType<T>().Subject;
            received.Should().Be(testDataObject);
            // When we are not flushing the data to the HGLOBAL, we are reading from our DataStore or the managed test data object.
            Action tryGetData = () => received.TryGetData(format, out SerializableTestData? result);
            tryGetData.Should().Throw<NotSupportedException>();
        }
    }

    [WinFormsFact]
    public unsafe void OleGetClipboard_ProxyBehavior()
    {
        DataObject dataObject = new("MyFormat", "My Data");
        using var iDataObject = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        using ComScope<IUnknown> originalUnknown = iDataObject.Query<IUnknown>();

        // IUnknown is generated by ComWrappers
        ((nint)originalUnknown.Value).Should().NotBe((nint)iDataObject.Value);

        PInvokeCore.OleSetClipboard(iDataObject).Should().Be(HRESULT.S_OK);

        using ComScope<Com.IDataObject> receivedIDataObject = default;
        PInvokeCore.OleGetClipboard(receivedIDataObject).Should().Be(HRESULT.S_OK);

        // Should have the proxy pointer here (CClipDataObject)
        ((nint)iDataObject.Value).Should().NotBe((nint)receivedIDataObject.Value);

        using ComScope<IUnknown> unknown = receivedIDataObject.Query<IUnknown>();

        // Unknown is the proxy
        ((nint)unknown.Value).Should().Be((nint)receivedIDataObject.Value);

        using ComScope<IComCallableWrapper> wrapper = receivedIDataObject.Query<IComCallableWrapper>();

        // Direct back to our original IComCallableWrapper implementation
        ((nint)wrapper.Value).Should().NotBe((nint)receivedIDataObject.Value);
        ((nint)wrapper.Value).Should().NotBe((nint)iDataObject.Value);

        using ComScope<IUnknown> wrapperUnknown = wrapper.Query<IUnknown>();
        ((nint)wrapperUnknown.Value).Should().Be((nint)originalUnknown.Value);
    }
}
