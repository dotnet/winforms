// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms.Tests;

public partial class ClipboardTests
{
    [WinFormsFact]
    public void Clipboard_Clear_InvokeMultipleTimes_Success()
    {
        Clipboard.Clear();
        Assert.False(Clipboard.ContainsAudio());
        Assert.False(Clipboard.ContainsData("format"));
        Assert.False(Clipboard.ContainsFileDropList());
        Assert.False(Clipboard.ContainsImage());
        Assert.False(Clipboard.ContainsText());

        Clipboard.Clear();
        Assert.False(Clipboard.ContainsAudio());
        Assert.False(Clipboard.ContainsData("format"));
        Assert.False(Clipboard.ContainsFileDropList());
        Assert.False(Clipboard.ContainsImage());
        Assert.False(Clipboard.ContainsText());
    }

    [Fact] // x-thread
    public void Clipboard_Clear_NotSta_ThrowsThreadStateException()
    {
        Assert.Throws<ThreadStateException>(() => Clipboard.Clear());
    }

    [WinFormsFact]
    public void Clipboard_ContainsAudio_InvokeMultipleTimes_Success()
    {
        bool result = Clipboard.ContainsAudio();
        Assert.Equal(result, Clipboard.ContainsAudio());
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void Clipboard_ContainsData_InvokeMultipleTimes_Success(string format)
    {
        bool result = Clipboard.ContainsData(format);
        Assert.Equal(result, Clipboard.ContainsData(format));
        Assert.False(result);
    }

    [WinFormsFact]
    public void Clipboard_ContainsFileDropList_InvokeMultipleTimes_Success()
    {
        bool result = Clipboard.ContainsFileDropList();
        Assert.Equal(result, Clipboard.ContainsFileDropList());
    }

    [WinFormsFact]
    public void Clipboard_ContainsImage_InvokeMultipleTimes_Success()
    {
        bool result = Clipboard.ContainsImage();
        Assert.Equal(result, Clipboard.ContainsImage());
    }

    [WinFormsFact]
    public void Clipboard_ContainsText_InvokeMultipleTimes_Success()
    {
        bool result = Clipboard.ContainsText();
        Assert.Equal(result, Clipboard.ContainsText());
    }

    [WinFormsTheory]
    [EnumData<TextDataFormat>]
    public void Clipboard_ContainsText_InvokeTextDataFormatMultipleTimes_Success(TextDataFormat format)
    {
        bool result = Clipboard.ContainsText(format);
        Assert.Equal(result, Clipboard.ContainsText(format));
    }

    [WinFormsTheory]
    [InvalidEnumData<TextDataFormat>]
    public void Clipboard_ContainsText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        Assert.Throws<InvalidEnumArgumentException>("format", () => Clipboard.ContainsText(format));
    }

    [WinFormsFact]
    public void Clipboard_GetAudioStream_InvokeMultipleTimes_Success()
    {
        Stream result = Clipboard.GetAudioStream();
        Assert.Equal(result, Clipboard.GetAudioStream());
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\t")]
    public void Clipboard_GetData_NullOrEmptyFormat_Returns_Null(string format)
    {
        object result = Clipboard.GetData(format);
        Assert.Equal(result, Clipboard.GetData(format));
        Assert.Null(result);
    }

    [WinFormsFact]
    public void Clipboard_GetDataObject_InvokeMultipleTimes_Success()
    {
        object result = Clipboard.GetDataObject();
        Assert.NotEqual(result, Clipboard.GetDataObject());
    }

    [WinFormsFact]
    public void Clipboard_GetFileDropList_InvokeMultipleTimes_Success()
    {
        bool result = Clipboard.ContainsFileDropList();
        Assert.Equal(result, Clipboard.ContainsFileDropList());
    }

    [WinFormsFact]
    public void Clipboard_GetImage_InvokeMultipleTimes_Success()
    {
        bool result = Clipboard.ContainsImage();
        Assert.Equal(result, Clipboard.ContainsImage());
    }

    [WinFormsFact]
    public void Clipboard_GetText_InvokeMultipleTimes_Success()
    {
        bool result = Clipboard.ContainsText();
        Assert.Equal(result, Clipboard.ContainsText());
    }

    [WinFormsTheory]
    [EnumData<TextDataFormat>]
    public void Clipboard_GetText_InvokeTextDataFormatMultipleTimes_Success(TextDataFormat format)
    {
        string result = Clipboard.GetText(format);
        Assert.Equal(result, Clipboard.GetText(format));
    }

    [WinFormsTheory]
    [InvalidEnumData<TextDataFormat>]
    public void Clipboard_GetText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        Assert.Throws<InvalidEnumArgumentException>("format", () => Clipboard.GetText(format));
    }

    [WinFormsFact]
    public void Clipboard_SetAudio_InvokeByteArray_GetReturnsExpected()
    {
        byte[] audioBytes = [1, 2, 3];
        Clipboard.SetAudio(audioBytes);
        Assert.Equal(audioBytes, Assert.IsType<MemoryStream>(Clipboard.GetAudioStream()).ToArray());
        Assert.Equal(audioBytes, Assert.IsType<MemoryStream>(Clipboard.GetData(DataFormats.WaveAudio)).ToArray());
        Assert.True(Clipboard.ContainsAudio());
        Assert.True(Clipboard.ContainsData(DataFormats.WaveAudio));
    }

    [WinFormsFact]
    public void Clipboard_SetAudio_InvokeEmptyByteArray_GetReturnsExpected()
    {
        byte[] audioBytes = Array.Empty<byte>();
        Clipboard.SetAudio(audioBytes);
        Assert.Null(Clipboard.GetAudioStream());
        Assert.Null(Clipboard.GetData(DataFormats.WaveAudio));
        Assert.True(Clipboard.ContainsAudio());
        Assert.True(Clipboard.ContainsData(DataFormats.WaveAudio));
    }

    [WinFormsFact]
    public void Clipboard_SetAudio_NullAudioBytes_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("audioBytes", () => Clipboard.SetAudio((byte[])null));
    }

    [WinFormsFact]
    public void Clipboard_SetAudio_InvokeStream_GetReturnsExpected()
    {
        byte[] audioBytes = [1, 2, 3];
        using MemoryStream audioStream = new(audioBytes);
        Clipboard.SetAudio(audioStream);
        Assert.Equal(audioBytes, Assert.IsType<MemoryStream>(Clipboard.GetAudioStream()).ToArray());
        Assert.Equal(audioBytes, Assert.IsType<MemoryStream>(Clipboard.GetData(DataFormats.WaveAudio)).ToArray());
        Assert.True(Clipboard.ContainsAudio());
        Assert.True(Clipboard.ContainsData(DataFormats.WaveAudio));
    }

    [WinFormsFact]
    public void Clipboard_SetAudio_InvokeEmptyStream_GetReturnsExpected()
    {
        MemoryStream audioStream = new();
        Clipboard.SetAudio(audioStream);
        Assert.Null(Clipboard.GetAudioStream());
        Assert.Null(Clipboard.GetData(DataFormats.WaveAudio));
        Assert.True(Clipboard.ContainsAudio());
        Assert.True(Clipboard.ContainsData(DataFormats.WaveAudio));
    }

    [WinFormsFact]
    public void Clipboard_SetAudio_NullAudioStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("audioStream", () => Clipboard.SetAudio((Stream)null));
    }

    [Fact] // x-thread
    public void Clipboard_SetAudio_NotSta_ThrowsThreadStateException()
    {
        Assert.Throws<ThreadStateException>(() => Clipboard.SetAudio(Array.Empty<byte>()));
        Assert.Throws<ThreadStateException>(() => Clipboard.SetAudio(new MemoryStream()));
    }

    [WinFormsTheory]
    [InlineData("format", null)]
    [InlineData("format", 1)]
    public void Clipboard_SetData_Invoke_GetReturnsExpected(string format, object data)
    {
        Clipboard.SetData(format, data);
        Assert.Equal(data, Clipboard.GetData(format));
        Assert.True(Clipboard.ContainsData(format));
    }

    [WinFormsFact]
    public void Clipboard_SetData_NullFormat_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Clipboard.SetData(format: null, data: new object()));
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Clipboard_SetData_EmptyOrWhitespaceFormat_ThrowsArgumentException(string format)
    {
        Assert.Throws<ArgumentException>(() => Clipboard.SetData(format, data: null));
    }

    [Fact] // x-thread
    public void Clipboard_SetData_NotSta_ThrowsThreadStateException()
    {
        Assert.Throws<ThreadStateException>(() => Clipboard.SetData("format", data: null));
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData("data")]
    public void Clipboard_SetDataObject_InvokeObjectNotIComDataObject_GetReturnsExpected(object data)
    {
        Clipboard.SetDataObject(data);
        Assert.Equal(data, Clipboard.GetDataObject().GetData(data.GetType()));
        Assert.True(Clipboard.ContainsData(data.GetType().FullName));
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData("data")]
    public void Clipboard_SetDataObject_InvokeObjectIComDataObject_GetReturnsExpected(object data)
    {
        DataObject dataObject = new(data);
        Clipboard.SetDataObject(dataObject);
        Assert.Equal(data, Clipboard.GetDataObject().GetData(data.GetType()));
        Assert.True(Clipboard.ContainsData(data.GetType().FullName));
    }

    [WinFormsTheory]
    [InlineData(1, true)]
    [InlineData(1, false)]
    [InlineData("data", true)]
    [InlineData("data", false)]
    public void Clipboard_SetDataObject_InvokeObjectBoolNotIComDataObject_GetReturnsExpected(object data, bool copy)
    {
        Clipboard.SetDataObject(data, copy);
        Assert.Equal(data, Clipboard.GetDataObject().GetData(data.GetType()));
        Assert.True(Clipboard.ContainsData(data.GetType().FullName));
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
        Assert.Equal(data, Clipboard.GetDataObject().GetData(data.GetType()));
        Assert.True(Clipboard.ContainsData(data.GetType().FullName));
    }

    [WinFormsTheory]
    [InlineData(1, true, 0, 0)]
    [InlineData(1, false, 1, 2)]
    [InlineData("data", true, 0, 0)]
    [InlineData("data", false, 1, 2)]
    public void Clipboard_SetDataObject_InvokeObjectBoolIntIntNotIComDataObject_GetReturnsExpected(object data, bool copy, int retryTimes, int retryDelay)
    {
        Clipboard.SetDataObject(data, copy, retryTimes, retryDelay);
        Assert.Equal(data, Clipboard.GetDataObject().GetData(data.GetType()));
        Assert.True(Clipboard.ContainsData(data.GetType().FullName));
    }

    [WinFormsFact]
    public void Clipboard_SetDataObject_NullData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("data", () => Clipboard.SetDataObject(null));
        Assert.Throws<ArgumentNullException>("data", () => Clipboard.SetDataObject(null, copy: true));
        Assert.Throws<ArgumentNullException>("data", () => Clipboard.SetDataObject(null, copy: true, retryTimes: 10, retryDelay: 0));
    }

    [WinFormsFact]
    public void Clipboard_SetDataObject_NegativeRetryTimes_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>("retryTimes", () => Clipboard.SetDataObject(new object(), copy: true, retryTimes: -1, retryDelay: 0));
    }

    [WinFormsFact]
    public void Clipboard_SetDataObject_NegativeRetryDelay_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>("retryDelay", () => Clipboard.SetDataObject(new object(), copy: true, retryTimes: 10, retryDelay: -1));
    }

    [Fact] // x-thread
    public void Clipboard_SetDataObject_NotSta_ThrowsThreadStateException()
    {
        Assert.Throws<ThreadStateException>(() => Clipboard.SetDataObject(null));
        Assert.Throws<ThreadStateException>(() => Clipboard.SetDataObject(null, copy: true));
        Assert.Throws<ThreadStateException>(() => Clipboard.SetDataObject(null, copy: true, retryTimes: 10, retryDelay: 0));
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
        Assert.Equal(filePaths, Clipboard.GetFileDropList());
        Assert.True(Clipboard.ContainsFileDropList());
    }

    [WinFormsFact]
    public void Clipboard_SetFileDropList_NullFilePaths_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("filePaths", () => Clipboard.SetFileDropList(null));
    }

    [WinFormsFact]
    public void Clipboard_SetFileDropList_EmptyFilePaths_ThrowsArgumentException()
    {
        StringCollection filePaths = [];
        Assert.Throws<ArgumentException>(() => Clipboard.SetFileDropList(filePaths));
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
        Assert.Throws<ArgumentException>(() => Clipboard.SetFileDropList(filePaths));
    }

    [Fact] // x-thread
    public void Clipboard_SetFileDropList_NotSta_ThrowsThreadStateException()
    {
        StringCollection filePaths =
        [
            "filePath"
        ];
        Assert.Throws<ThreadStateException>(() => Clipboard.SetFileDropList(filePaths));
    }

    [WinFormsFact]
    public void Clipboard_SetImage_InvokeBitmap_GetReturnsExpected()
    {
        using Bitmap bitmap = new(10, 10);
        bitmap.SetPixel(1, 2, Color.FromArgb(0x01, 0x02, 0x03, 0x04));
        Clipboard.SetImage(bitmap);
        Bitmap result = Assert.IsType<Bitmap>(Clipboard.GetImage());
        Assert.Equal(bitmap.Size, result.Size);
        Assert.Equal(Color.FromArgb(0xFF, 0xD2, 0xD2, 0xD2), result.GetPixel(1, 2));
        Assert.True(Clipboard.ContainsImage());
    }

    [WinFormsFact]
    public void Clipboard_SetImage_InvokeMetafile_GetReturnsExpected()
    {
        using Metafile metafile = new("bitmaps/telescope_01.wmf");
        Clipboard.SetImage(metafile);
        Assert.Null(Clipboard.GetImage());
        Assert.True(Clipboard.ContainsImage());
    }

    [WinFormsFact]
    public void Clipboard_SetImage_InvokeEnhancedMetafile_GetReturnsExpected()
    {
        using Metafile metafile = new("bitmaps/milkmateya01.emf");
        Clipboard.SetImage(metafile);
        Assert.Null(Clipboard.GetImage());
        Assert.True(Clipboard.ContainsImage());
    }

    [WinFormsFact]
    public void Clipboard_SetImage_NullImage_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("image", () => Clipboard.SetImage(null));
    }

    [Fact] // x-thread
    public void Clipboard_SetImage_NotSta_ThrowsThreadStateException()
    {
        using Bitmap bitmap = new(10, 10);
        using Metafile metafile = new("bitmaps/telescope_01.wmf");
        using Metafile enhancedMetafile = new("bitmaps/milkmateya01.emf");
        Assert.Throws<ThreadStateException>(() => Clipboard.SetImage(bitmap));
        Assert.Throws<ThreadStateException>(() => Clipboard.SetImage(metafile));
        Assert.Throws<ThreadStateException>(() => Clipboard.SetImage(enhancedMetafile));
    }

    [WinFormsTheory]
    [EnumData<TextDataFormat>]
    public void Clipboard_SetText_InvokeStringTextDataFormat_GetReturnsExpected(TextDataFormat format)
    {
        Clipboard.SetText("text", format);
        Assert.Equal("text", Clipboard.GetText(format));
        Assert.True(Clipboard.ContainsText(format));
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void Clipboard_SetText_NullOrEmptyText_ThrowsArgumentNullException(string text)
    {
        Assert.Throws<ArgumentNullException>("text", () => Clipboard.SetText(text));
        Assert.Throws<ArgumentNullException>("text", () => Clipboard.SetText(text, TextDataFormat.Text));
    }

    [WinFormsTheory]
    [InvalidEnumData<TextDataFormat>]
    public void Clipboard_SetText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        Assert.Throws<InvalidEnumArgumentException>("format", () => Clipboard.SetText("text", format));
    }

    [Fact] // x-thread
    public void Clipboard_SetText_NotSta_ThrowsThreadStateException()
    {
        Assert.Throws<ThreadStateException>(() => Clipboard.SetText("text"));
        Assert.Throws<ThreadStateException>(() => Clipboard.SetText("text", TextDataFormat.Text));
    }

    [WinFormsFact]
    public void ClipBoard_SetData_CustomFormat_Color()
    {
        using BinaryFormatterScope scope = new(enable: true);
        string format = nameof(ClipBoard_SetData_CustomFormat_Color);
        Clipboard.SetData(format, Color.Black);
        Assert.True(Clipboard.ContainsData(format));
        Assert.Equal(Color.Black, Clipboard.GetData(format));
    }

    [WinFormsFact]
    public void ClipBoard_SetData_CustomFormat_Color_BinaryFormatterDisabled_SerializesException()
    {
        using BinaryFormatterScope scope = new(enable: false);
        string format = nameof(ClipBoard_SetData_CustomFormat_Color);

        // This will fail and NotSupportedException will be put on the Clipboard instead.
        Clipboard.SetData(format, Color.Black);
        Assert.True(Clipboard.ContainsData(format));

        NotSupportedException value = (NotSupportedException)Clipboard.GetData(format);

        using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter formatter = new();
#pragma warning restore SYSLIB0011
        try
        {
            formatter.Serialize(stream, new object());
        }
        catch (NotSupportedException ex)
        {
            Assert.Equal(ex.Message, value.Message);
            return;
        }

        Assert.Fail("Formatting should have failed.");
    }

    [WinFormsFact]
    public unsafe void ClipBoard_GetClipboard_ReturnsProxy()
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
    public void ClipBoard_Set_DoesNotWrapTwice()
    {
        string realDataObject = string.Empty;
        Clipboard.SetDataObject(realDataObject);
        IDataObject clipboardDataObject = Clipboard.GetDataObject();
        clipboardDataObject.Should().BeOfType(typeof(DataObject));
        ((DataObject)clipboardDataObject).IsWrappedForClipboard.Should().BeTrue();

        Clipboard.SetDataObject(clipboardDataObject);
        IDataObject clipboardDataObject2 = Clipboard.GetDataObject();
        clipboardDataObject2.Should().BeSameAs(clipboardDataObject);
    }

    [WinFormsFact]
    public void ClipBoard_GetSet_RoundTrip_ReturnsExpected()
    {
        CustomDataObject realDataObject = new();
        Clipboard.SetDataObject(realDataObject);
        IDataObject clipboardDataObject = Clipboard.GetDataObject();
        clipboardDataObject.Should().BeSameAs(realDataObject);
        clipboardDataObject.GetDataPresent("Foo").Should().BeTrue();
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
        object IDataObject.GetData(string format, bool autoConvert) => format == "Foo" ? "Bar" : null;
        object IDataObject.GetData(string format) => format == "Foo" ? "Bar" : null;
        object IDataObject.GetData(Type format) => null;
        void ComTypes.IDataObject.GetData(ref ComTypes.FORMATETC format, out ComTypes.STGMEDIUM medium) => throw new NotImplementedException();
        void ComTypes.IDataObject.GetDataHere(ref ComTypes.FORMATETC format, ref ComTypes.STGMEDIUM medium) => throw new NotImplementedException();
        bool IDataObject.GetDataPresent(string format, bool autoConvert) => format == "Foo";
        bool IDataObject.GetDataPresent(string format) => format == "Foo";
        bool IDataObject.GetDataPresent(Type format) => false;
        string[] IDataObject.GetFormats(bool autoConvert) => ["Foo"];
        string[] IDataObject.GetFormats() => ["Foo"];
        int ComTypes.IDataObject.QueryGetData(ref ComTypes.FORMATETC format) => throw new NotImplementedException();
        void IDataObject.SetData(string format, bool autoConvert, object data) => throw new NotImplementedException();
        void IDataObject.SetData(string format, object data) => throw new NotImplementedException();
        void IDataObject.SetData(Type format, object data) => throw new NotImplementedException();
        void IDataObject.SetData(object data) => throw new NotImplementedException();
        void ComTypes.IDataObject.SetData(ref ComTypes.FORMATETC formatIn, ref ComTypes.STGMEDIUM medium, bool release) => throw new NotImplementedException();
    }
}
