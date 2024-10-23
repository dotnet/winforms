// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Formats.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Windows.Forms.Primitives;
using System.Text.Json;
using Windows.Win32.System.Ole;
using static System.Windows.Forms.Tests.BinaryFormatUtilitiesTests;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms.Tests;

// Note: each registered Clipboard format is an OS singleton
// and we should not run this test at the same time as other tests using the same format.
[Collection("Sequential")]
[UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
public class ClipboardTests
{
#pragma warning disable WFDEV005 // Type or member is obsolete

    [WinFormsFact]
    public void Clipboard_SetText_InvokeString_GetReturnsExpected()
    {
        Clipboard.SetText("text");
        Clipboard.GetText().Should().Be("text");
        Clipboard.ContainsText().Should().BeTrue();
    }

    [WinFormsFact]
    public void Clipboard_Clear_InvokeMultipleTimes_Success()
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
    public void Clipboard_Contains_InvokeMultipleTimes_Success(Func<bool> contains)
    {
        Clipboard.Clear();
        bool result = contains.Invoke();
        contains.Invoke().Should().Be(result);
        result.Should().BeFalse();
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void Clipboard_ContainsData_InvokeMultipleTimes_Success(string format)
    {
        bool result = Clipboard.ContainsData(format);
        Clipboard.ContainsData(format).Should().Be(result);
        result.Should().BeFalse();
    }

    [WinFormsTheory]
    [EnumData<TextDataFormat>]
    public void Clipboard_ContainsText_TextDataFormat_InvokeMultipleTimes_Success(TextDataFormat format)
    {
        bool result = Clipboard.ContainsText(format);
        Clipboard.ContainsText(format).Should().Be(result);
    }

    [WinFormsTheory]
    [InvalidEnumData<TextDataFormat>]
    public void Clipboard_ContainsText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        Action action = () => Clipboard.ContainsText(format);
        action.Should().Throw<InvalidEnumArgumentException>().WithParameterName("format");
    }

    [WinFormsFact]
    public void Clipboard_GetAudioStream_InvokeMultipleTimes_Success()
    {
        Stream? result = Clipboard.GetAudioStream();
        (Clipboard.GetAudioStream() == result).Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\t")]
    public void Clipboard_GetData_NullOrEmptyFormat_Returns_Null(string? format)
    {
        object? result = Clipboard.GetData(format!);
        result.Should().BeNull();
        result = Clipboard.GetData(format!);
        result.Should().BeNull();
    }

    [WinFormsFact]
    public void Clipboard_GetDataObject_InvokeMultipleTimes_Success()
    {
        DataObject result1 = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        DataObject result2 = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        result1.GetFormats().Should().BeEquivalentTo(result2.GetFormats());
    }

    [WinFormsFact]
    public void Clipboard_GetFileDropList_InvokeMultipleTimes_Success()
    {
        StringCollection result = Clipboard.GetFileDropList();
        Clipboard.GetFileDropList().Should().BeEquivalentTo(result);
    }

    [WinFormsFact]
    public void Clipboard_GetImage_InvokeMultipleTimes_Success()
    {
        Image? result = Clipboard.GetImage();
        Clipboard.GetImage().Should().BeEquivalentTo(result);
    }

    [WinFormsFact]
    public void Clipboard_GetText_InvokeMultipleTimes_Success()
    {
        string result = Clipboard.GetText();
        Clipboard.GetText().Should().Be(result);
    }

    [WinFormsTheory]
    [EnumData<TextDataFormat>]
    public void Clipboard_GetText_TextDataFormat_InvokeMultipleTimes_Success(TextDataFormat format)
    {
        string result = Clipboard.GetText(format);
        Clipboard.GetText(format).Should().Be(result);
    }

    [WinFormsTheory]
    [InvalidEnumData<TextDataFormat>]
    public void Clipboard_GetText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        Action action = () => Clipboard.GetText(format);
        action.Should().Throw<InvalidEnumArgumentException>().WithParameterName("format");
    }

    [WinFormsFact]
    public void Clipboard_SetAudio_InvokeByteArray_GetReturnsExpected()
    {
        byte[] audioBytes = [1, 2, 3];
        Clipboard.SetAudio(audioBytes);

        Clipboard.GetAudioStream().Should().BeOfType<MemoryStream>().Which.ToArray().Should().Equal(audioBytes);
        Clipboard.GetData(DataFormats.WaveAudio).Should().BeOfType<MemoryStream>().Which.ToArray().Should().Equal(audioBytes);
        Clipboard.ContainsAudio().Should().BeTrue();
        Clipboard.ContainsData(DataFormats.WaveAudio).Should().BeTrue();
    }

    [WinFormsFact]
    public void Clipboard_SetAudio_InvokeEmptyByteArray_GetReturnsExpected()
    {
        byte[] audioBytes = Array.Empty<byte>();
        Clipboard.SetAudio(audioBytes);

        Clipboard.GetAudioStream().Should().BeNull();
        Clipboard.GetData(DataFormats.WaveAudio).Should().BeNull();
        Clipboard.ContainsAudio().Should().BeTrue();
        Clipboard.ContainsData(DataFormats.WaveAudio).Should().BeTrue();
    }

    [WinFormsFact]
    public void Clipboard_SetAudio_NullAudioBytes_ThrowsArgumentNullException()
    {
        Action action = () => Clipboard.SetAudio((byte[])null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("audioBytes");
    }

    [WinFormsFact]
    public void Clipboard_SetAudio_InvokeStream_GetReturnsExpected()
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
    public void Clipboard_SetAudio_InvokeEmptyStream_GetReturnsExpected()
    {
        using MemoryStream audioStream = new();
        Clipboard.SetAudio(audioStream);

        Clipboard.GetAudioStream().Should().BeNull();
        Clipboard.GetData(DataFormats.WaveAudio).Should().BeNull();
        Clipboard.ContainsAudio().Should().BeTrue();
        Clipboard.ContainsData(DataFormats.WaveAudio).Should().BeTrue();
    }

    [WinFormsFact]
    public void Clipboard_SetAudio_NullAudioStream_ThrowsArgumentNullException()
    {
        Action action = () => Clipboard.SetAudio((Stream)null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("audioStream");
    }

    [WinFormsTheory]
    [InlineData("format", null)]
    [InlineData("format", 1)]
    public void Clipboard_SetData_Invoke_GetReturnsExpected(string format, object? data)
    {
        Clipboard.SetData(format, data!);
        Clipboard.GetData(format).Should().Be(data);
        Clipboard.ContainsData(format).Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(null)]
    public void Clipboard_SetData_EmptyOrWhitespaceFormat_ThrowsArgumentException(string? format)
    {
        Action action = () => Clipboard.SetData(format!, "data");
        action.Should().Throw<ArgumentException>().WithParameterName("format");
    }

    [WinFormsFact]
    public void Clipboard_SetData_null_NotThrow()
    {
        try
        {
            Action action = () => Clipboard.SetData("MyData", data: null!);
            action.Should().NotThrow();
            // Clipboard flushes format only, content is not stored.
            // GetData will hit "Data on clipboard is invalid (0x800401D3 (CLIPBRD_E_BAD_DATA))"
            Clipboard.ContainsData("MyData").Should().BeTrue();
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
    public void Clipboard_SetDataObject_InvokeObjectNotIComDataObject_GetReturnsExpected(object data)
    {
        Clipboard.SetDataObject(data);

        DataObject dataObject = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        dataObject.GetData(data.GetType()).Should().Be(data);
        Clipboard.ContainsData(data.GetType().FullName).Should().BeTrue();
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData("data")]
    public void Clipboard_SetDataObject_InvokeObjectIComDataObject_GetReturnsExpected(object data)
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
    public void Clipboard_SetDataObject_InvokeObjectBoolNotIComDataObject_GetReturnsExpected(object data, bool copy)
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
    public void Clipboard_SetDataObject_InvokeObjectBoolIComDataObject_GetReturnsExpected(object data, bool copy, int retryTimes, int retryDelay)
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
    public void Clipboard_SetDataObject_InvokeObjectBoolIntIntNotIComDataObject_GetReturnsExpected(object data, bool copy, int retryTimes, int retryDelay)
    {
        Clipboard.SetDataObject(data, copy, retryTimes, retryDelay);

        DataObject dataObject = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        dataObject.GetData(data.GetType()).Should().Be(data);
        Clipboard.ContainsData(data.GetType().FullName).Should().BeTrue();
    }

    public static TheoryData<Action> Clipboard_SetDataObject_Null_TheoryData =>
    [
        () => Clipboard.SetDataObject(null!),
        () => Clipboard.SetDataObject(null!, copy: true),
        () => Clipboard.SetDataObject(null!, copy: true, retryTimes: 10, retryDelay: 0)
    ];

    [WinFormsTheory]
    [MemberData(nameof(Clipboard_SetDataObject_Null_TheoryData))]
    public void Clipboard_SetDataObject_NullData_ThrowsArgumentNullException(Action action)
    {
        action.Should().Throw<ArgumentNullException>().WithParameterName("data");
    }

    [WinFormsFact]
    public void Clipboard_SetDataObject_NegativeRetryTimes_ThrowsArgumentOutOfRangeException()
    {
        Action action = () => Clipboard.SetDataObject(new object(), copy: true, retryTimes: -1, retryDelay: 0);
        action.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("retryTimes");
    }

    [WinFormsFact]
    public void Clipboard_SetDataObject_NegativeRetryDelay_ThrowsArgumentOutOfRangeException()
    {
        Action action = () => Clipboard.SetDataObject(new object(), copy: true, retryTimes: 10, retryDelay: -1);
        action.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("retryDelay");
    }

    public static TheoryData<Action> NotAnStaTheoryData =>
    [
        Clipboard.Clear,
        () => Clipboard.SetAudio(Array.Empty<byte>()),
        () => Clipboard.SetAudio(new MemoryStream()),
        () => Clipboard.SetData("format", data: null!),
        () => Clipboard.SetDataObject(null!),
        () => Clipboard.SetDataObject(null!, copy: true),
        () => Clipboard.SetDataObject(null!, copy: true, retryTimes: 10, retryDelay: 0),
        () => Clipboard.SetFileDropList(["filePath"]),
        () => Clipboard.SetText("text"),
        () => Clipboard.SetText("text", TextDataFormat.Text)
    ];

    [Theory] // x-thread
    [MemberData(nameof(NotAnStaTheoryData))]
    public void Clipboard_NotSta_ThrowsThreadStateException(Action action)
    {
        action.Should().Throw<ThreadStateException>();
    }

    [Fact] // x-thread
    public void Clipboard_SetImage_NotSta_ThrowsThreadStateException()
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
    public void Clipboard_SetFileDropList_Invoke_GetReturnsExpected()
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
    public void Clipboard_SetFileDropList_NullFilePaths_ThrowsArgumentNullException()
    {
        Action action = () => Clipboard.SetFileDropList(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("filePaths");
    }

    [WinFormsFact]
    public void Clipboard_SetFileDropList_EmptyFilePaths_ThrowsArgumentException()
    {
        Action action = static () => Clipboard.SetFileDropList([]);
        action.Should().Throw<ArgumentException>();
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData("\0")]
    public void Clipboard_SetFileDropList_InvalidFileInPaths_ThrowsArgumentException(string filePath)
    {
        StringCollection filePaths =
        [
            filePath
        ];
        Action action = () => Clipboard.SetFileDropList(filePaths);
        action.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void Clipboard_SetImage_InvokeBitmap_GetReturnsExpected()
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
    public void Clipboard_SetImage_InvokeMetafile_GetReturnsExpected()
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
    public void Clipboard_SetImage_InvokeEnhancedMetafile_GetReturnsExpected()
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
    public void Clipboard_SetImage_NullImage_ThrowsArgumentNullException()
    {
        Action action = () => Clipboard.SetImage(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("image");
    }

    [WinFormsTheory]
    [EnumData<TextDataFormat>]
    public void Clipboard_SetText_InvokeStringTextDataFormat_GetReturnsExpected(TextDataFormat format)
    {
        Clipboard.SetText("text", format);
        Clipboard.GetText(format).Should().Be("text");
        Clipboard.ContainsText(format).Should().BeTrue();
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void Clipboard_SetText_NullOrEmptyText_ThrowsArgumentNullException(string text)
    {
        Action action = () => Clipboard.SetText(text);
        action.Should().Throw<ArgumentNullException>().WithParameterName("text");
        action = () => Clipboard.SetText(text, TextDataFormat.Text);
        action.Should().Throw<ArgumentNullException>().WithParameterName("text");
    }

    [WinFormsTheory]
    [InvalidEnumData<TextDataFormat>]
    public void Clipboard_SetText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        Action action = () => Clipboard.SetText("text", format);
        action.Should().Throw<InvalidEnumArgumentException>().WithParameterName("format");
    }

    [WinFormsFact]
    public void Clipboard_SetData_CustomFormat_Color()
    {
        string format = nameof(Clipboard_SetData_CustomFormat_Color);
        Clipboard.SetData(format, Color.Black);

        Clipboard.ContainsData(format).Should().BeTrue();
        Clipboard.GetData(format).Should().Be(Color.Black);
    }

    [WinFormsFact]
    public void Clipboard_SetData_CustomFormat_Exception_BinaryFormatterDisabled_SerializesException()
    {
        using BinaryFormatterScope scope = new(enable: false);
        string format = nameof(Clipboard_SetData_CustomFormat_Exception_BinaryFormatterDisabled_SerializesException);

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
    public unsafe void Clipboard_GetClipboard_ReturnsProxy()
    {
        DataObject data = new();
        using var dataScope = ComHelpers.GetComScope<Com.IDataObject>(data);
        PInvoke.OleSetClipboard(dataScope).Succeeded.Should().BeTrue();

        using ComScope<Com.IDataObject> proxy = new(null);
        PInvoke.OleGetClipboard(proxy).Succeeded.Should().BeTrue();
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
    public void Clipboard_Set_DoesNotWrapTwice()
    {
        string realDataObject = string.Empty;
        Clipboard.SetDataObject(realDataObject);

        IDataObject? clipboardDataObject = Clipboard.GetDataObject();
        var dataObject = clipboardDataObject.Should().BeOfType<DataObject>().Subject;
        dataObject.IsWrappedForClipboard.Should().BeTrue();

        Clipboard.SetDataObject(clipboardDataObject!);
        IDataObject? clipboardDataObject2 = Clipboard.GetDataObject();
        clipboardDataObject2.Should().NotBeNull();
        clipboardDataObject2.Should().BeSameAs(clipboardDataObject);
    }

    [WinFormsFact]
    public void Clipboard_GetSet_RoundTrip_ReturnsExpected()
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
    public unsafe void Clipboard_RawClipboard_SetClipboardData_ReturnsExpected()
    {
        Clipboard.Clear();

        OpenClipboard(HWND.Null).Should().BeTrue();
        string testString = "test";
        SetClipboardData((uint)CLIPBOARD_FORMAT.CF_UNICODETEXT, (HANDLE)Marshal.StringToHGlobalUni(testString));
        CloseClipboard().Should().BeTrue();

        DataObject dataObject = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Subject;
        dataObject.GetData(DataFormats.Text).Should().Be(testString);
    }

    [WinFormsFact]
    public void Clipboard_BinaryFormatter_AppContextSwitch()
    {
        // Test the switch to ensure it works as expected in the context of this test assembly.
        LocalAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization.Should().BeFalse();

        using (BinaryFormatterInClipboardDragDropScope scope = new(enable: true))
        {
            LocalAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization.Should().BeTrue();
        }

        LocalAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization.Should().BeFalse();

        using (BinaryFormatterInClipboardDragDropScope scope = new(enable: false))
        {
            LocalAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization.Should().BeFalse();
        }

        LocalAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization.Should().BeFalse();
    }

    [WinFormsFact]
    public void Clipboard_NrbfSerializer_AppContextSwitch()
    {
        // Test the switch to ensure it works as expected in the context of this test assembly.
        LocalAppContextSwitches.ClipboardDragDropEnableNrbfSerialization.Should().BeTrue();

        using (NrbfSerializerInClipboardDragDropScope scope = new(enable: false))
        {
            LocalAppContextSwitches.ClipboardDragDropEnableNrbfSerialization.Should().BeFalse();
        }

        LocalAppContextSwitches.ClipboardDragDropEnableNrbfSerialization.Should().BeTrue();

        using (NrbfSerializerInClipboardDragDropScope scope = new(enable: true))
        {
            LocalAppContextSwitches.ClipboardDragDropEnableNrbfSerialization.Should().BeTrue();
        }

        LocalAppContextSwitches.ClipboardDragDropEnableNrbfSerialization.Should().BeTrue();
    }

    [WinFormsFact]
    public void Clipboard_TryGetInt_ReturnsExpected()
    {
        int expected = 101;
        using (BinaryFormatterScope scope = new(enable: true))
        {
            Clipboard.SetData("TestData", expected);
        }

        Clipboard.TryGetData("TestData", out int? data).Should().BeTrue();
        data.Should().Be(expected);
    }

    [WinFormsFact]
    public void Clipboard_TryGetTestData()
    {
        TestData expected = new(DateTime.Now);
        string format = "TestData";
        using BinaryFormatterFullCompatScope scope = new();
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
        Action tryGetData = () => Clipboard.TryGetData(format, out testData);
        tryGetData.Should().Throw<NotSupportedException>();

        // This is the safe switch configuration, custom types can't be resolved
        using NrbfSerializerInClipboardDragDropScope nrbfScope2 = new(enable: false);
        using BinaryFormatterInClipboardDragDropScope binaryScope2 = new(enable: false);
        Action tryGetDataWithResolver = () => Clipboard.TryGetData(format, TestData.TestDataResolver, out testData);
        tryGetDataWithResolver.Should().Throw<NotSupportedException>();
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
    public void Clipboard_TryGetObject_Throws()
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
    public void Clipboard_TryGetRectangleAsObject_Throws()
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
    public void Clipboard_TryGetNotSupportedException()
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
    public void Clipboard_TryGetOffsetArray()
    {
        Array value = Array.CreateInstance(typeof(uint), lengths: [2, 3], lowerBounds: [1, 2]);
        value.SetValue(101u, 1, 2);
        value.SetValue(102u, 1, 3);
        value.SetValue(103u, 1, 4);
        value.SetValue(201u, 2, 2);
        value.SetValue(202u, 2, 3);
        value.SetValue(203u, 2, 4);

        using BinaryFormatterFullCompatScope scope = new();
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

        Action tryGetData = () => Clipboard.TryGetData("test", out uint[,]? data);
        // Can't decode the root record, thus can't validate the T.
        tryGetData.Should().Throw<NotSupportedException>();
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Clipboard_SetDataAsJson_EmptyFormat_Throws(string? format)
    {
        Action action = () => Clipboard.SetDataAsJson(format!, 1);
        action.Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public void Clipboard_SetDataAsJson_DataObject_Throws()
    {
        Action action = () => Clipboard.SetDataAsJson("format", new DataObject());
        action.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void Clipboard_SetDataAsJson_ReturnsExpected()
    {
        Point point = new() { X = 1, Y = 1 };

        Clipboard.SetDataAsJson("point", point);
        IDataObject? dataObject = Clipboard.GetDataObject();
        dataObject.Should().NotBeNull();
        dataObject!.GetDataPresent("point").Should().BeTrue();
        Point deserialized = dataObject.GetData("point").Should().BeOfType<Point>().Which;
        deserialized.Should().BeEquivalentTo(point);
    }

    [WinFormsTheory]
    [BoolData]
    public void Clipboard_SetDataObject_WithJson_ReturnsExpected(bool copy)
    {
        Point point = new() { X = 1, Y = 1 };

        DataObject dataObject = new();
        dataObject.SetDataAsJson("point", point);

        Clipboard.SetDataObject(dataObject, copy);
        IDataObject? returnedDataObject = Clipboard.GetDataObject();
        returnedDataObject.Should().NotBeNull();
        Point deserialized = returnedDataObject!.GetData("point").Should().BeOfType<Point>().Which;
        deserialized.Should().BeEquivalentTo(point);
    }

    [WinFormsTheory]
    [BoolData]
    public void Clipboard_SetDataObject_WithMultipleData_ReturnsExpected(bool copy)
    {
        Point point1 = new() { X = 1, Y = 1 };
        Point point2 = new() { Y = 2, X = 2 };
        DataObject data = new();
        data.SetDataAsJson("point1", point1);
        data.SetDataAsJson("point2", point2);
        data.SetData("Mystring", "test");
        Clipboard.SetDataObject(data, copy);

        Clipboard.GetData("point1").Should().BeOfType<Point>().Which.Should().BeEquivalentTo(point1);
        Clipboard.GetData("point2").Should().BeOfType<Point>().Which.Should().BeEquivalentTo(point2);
        Clipboard.GetData("Mystring").Should().Be("test");
    }

    [WinFormsFact]
    public unsafe void Clipboard_Deserialize_FromStream_Manually()
    {
        // This test demonstrates how a user can manually deserialize JsonData<T> that has been serialized onto
        // the clipboard from stream. This may need to be done if type JsonData<T> does not exist in the .NET version
        // the user is utilizing.
        Point point = new(1, 1);
        Clipboard.SetDataAsJson("testFormat", point);

        // Manually retrieve the serialized stream.
        ComTypes.IDataObject dataObject = Clipboard.GetDataObject().Should().BeAssignableTo<ComTypes.IDataObject>().Which;
        ComTypes.FORMATETC formatetc = new()
        {
            cfFormat = (short)DataFormats.GetFormat("testFormat").Id,
            dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            tymed = ComTypes.TYMED.TYMED_HGLOBAL
        };
        dataObject.GetData(ref formatetc, out ComTypes.STGMEDIUM medium);
        HGLOBAL hglobal = (HGLOBAL)medium.unionmember;
        Stream stream;
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
        SerializationRecord record = NrbfDecoder.Decode(stream, leaveOpen: true);
        ClassRecord types = record.Should().BeAssignableTo<ClassRecord>().Which;
        types.HasMember("<JsonBytes>k__BackingField").Should().BeTrue();
        SZArrayRecord<byte> byteData = types.GetRawValue("<JsonBytes>k__BackingField").Should().BeAssignableTo<SZArrayRecord<byte>>().Which;
        TypeName.TryParse(types.TypeName.FullName, out TypeName? result).Should().BeTrue();
        TypeName checkedResult = result.Should().BeOfType<TypeName>().Which;
        TypeName genericTypeName = checkedResult.GetGenericArguments().SingleOrDefault().Should().BeOfType<TypeName>().Subject;
        Type.GetType(genericTypeName.AssemblyQualifiedName).Should().Be(typeof(Point));

        JsonSerializer.Deserialize(byteData.GetArray(), typeof(Point)).Should().BeEquivalentTo(point);
    }
}
