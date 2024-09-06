// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms.Tests;

// Note: each registered Clipboard format is an OS singleton
// and we should not run this test at the same time as other tests using the same format.
[Collection("Sequential")]
public class ClipboardTests
{
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

    public static TheoryData<Func<bool>> ContainsMethodsTheoryData => new()
    {
        Clipboard.ContainsAudio,
        Clipboard.ContainsFileDropList,
        Clipboard.ContainsImage,
        Clipboard.ContainsText
    };

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
        IDataObject? result = Clipboard.GetDataObject();
        (result == Clipboard.GetDataObject()).Should().BeFalse();
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
        Action action = () => Clipboard.SetData(format!, data: null!);
        action.Should().Throw<ArgumentException>().WithParameterName("format");
    }

    [WinFormsFact]
    public void Clipboard_SetData_null_Success()
    {
        Action action = () => Clipboard.SetData("MyData", data: null!);
        action.Should().NotThrow();
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData("data")]
    public void Clipboard_SetDataObject_InvokeObjectNotIComDataObject_GetReturnsExpected(object data)
    {
        Clipboard.SetDataObject(data);

        var dataObject = Clipboard.GetDataObject();
        Assert.NotNull(dataObject);
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

        var actual = Clipboard.GetDataObject();
        Assert.NotNull(actual);
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

        var dataObject = Clipboard.GetDataObject();
        Assert.NotNull(dataObject);
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

        DataObject actual = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Which;
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

        DataObject dataObject = Clipboard.GetDataObject().Should().BeOfType<DataObject>().Which;
        dataObject.GetData(data.GetType()).Should().Be(data);
        Clipboard.ContainsData(data.GetType().FullName).Should().BeTrue();
    }

    public static TheoryData<Action> Clipboard_SetDataObject_Null_TheoryData => new()
    {
        () => Clipboard.SetDataObject(null!),
        () => Clipboard.SetDataObject(null!, copy: true),
        () => Clipboard.SetDataObject(null!, copy: true, retryTimes: 10, retryDelay: 0)
    };

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

    public static TheoryData<Action> NotAnStaTheoryData => new()
    {
        Clipboard.Clear,
        () => Clipboard.SetAudio(Array.Empty<byte>()),
        () => Clipboard.SetAudio(new MemoryStream()),
        () => Clipboard.SetData("format", data: null!),
        () => Clipboard.SetDataObject(null!),
        () => Clipboard.SetDataObject(null!, copy: true),
        () => Clipboard.SetDataObject(null!, copy: true, retryTimes: 10, retryDelay: 0),
        () => Clipboard.SetFileDropList(new StringCollection { "filePath" }),
        () => Clipboard.SetText("text"),
        () => Clipboard.SetText("text", TextDataFormat.Text)
    };

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
        Bitmap result = Assert.IsType<Bitmap>(Clipboard.GetImage());
        result.Size.Should().Be(bitmap.Size);
        result.GetPixel(1, 2).Should().Be(Color.FromArgb(0xFF, 0xD2, 0xD2, 0xD2));
        Clipboard.ContainsImage().Should().BeTrue();
    }

    [WinFormsFact]
    public void Clipboard_SetImage_InvokeMetafile_GetReturnsExpected()
    {
        using Metafile metafile = new("bitmaps/telescope_01.wmf");
        Clipboard.SetImage(metafile);
        Clipboard.GetImage().Should().BeNull();
        Clipboard.ContainsImage().Should().BeTrue();
    }

    [WinFormsFact]
    public void Clipboard_SetImage_InvokeEnhancedMetafile_GetReturnsExpected()
    {
        using Metafile metafile = new("bitmaps/milkmateya01.emf");
        Clipboard.SetImage(metafile);
        Clipboard.GetImage().Should().BeNull();
        Clipboard.ContainsImage().Should().BeTrue();
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
    public void Clipboard_SetData_CustomFormat_Color_BinaryFormatterDisabled_SerializesException()
    {
        using BinaryFormatterScope scope = new(enable: false);
        string format = nameof(Clipboard_SetData_CustomFormat_Color_BinaryFormatterDisabled_SerializesException);

        // This will fail and NotSupportedException will be put on the Clipboard instead.
        Clipboard.SetData(format, Color.Black);
        Clipboard.ContainsData(format).Should().BeTrue();

        using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter formatter = new();
#pragma warning restore SYSLIB0011
        try
        {
            formatter.Serialize(stream, new object());
        }
        catch (NotSupportedException)
        {
            return;
        }

        Assert.Fail("Formatting should have failed.");
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
        var dataObject = clipboardDataObject.Should().BeOfType<DataObject>().Which;
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
}
