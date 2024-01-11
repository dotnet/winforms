// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using Moq;
using Windows.Win32.System.Ole;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using Point = System.Drawing.Point;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataObjectTests
{
    private static readonly string[] s_clipboardFormats =
    {
        DataFormats.CommaSeparatedValue,
        DataFormats.Dib,
        DataFormats.Dif,
        DataFormats.PenData,
        DataFormats.Riff,
        DataFormats.Tiff,
        DataFormats.WaveAudio,
        DataFormats.SymbolicLink,
        DataFormats.StringFormat,
        DataFormats.Bitmap,
        DataFormats.EnhancedMetafile,
        DataFormats.FileDrop,
        DataFormats.Html,
        DataFormats.MetafilePict,
        DataFormats.OemText,
        DataFormats.Palette,
        DataFormats.Rtf,
        DataFormats.Text,
        DataFormats.UnicodeText,
        DataFormats.Serializable,
        "something custom",
    };

    private static readonly string[] s_mappedFormats =
    {
        typeof(Bitmap).FullName,
        "FileName",
        "FileNameW"
    };

    [Fact]
    public void DataObject_ContainsAudio_InvokeDefault_ReturnsFalse()
    {
        DataObject dataObject = new();
        Assert.False(dataObject.ContainsAudio());
    }

    [Theory]
    [BoolData]
    public void DataObject_ContainsAudio_InvokeMocked_CallsGetDataPresent(bool result)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.ContainsAudio())
            .CallBase();
        mockDataObject
            .Setup(o => o.GetDataPresent(DataFormats.WaveAudio, false))
            .Returns(result)
            .Verifiable();
        Assert.Equal(result, mockDataObject.Object.ContainsAudio());
        mockDataObject.Verify(o => o.GetDataPresent(DataFormats.WaveAudio, false), Times.Once());
    }

    [Fact]
    public void DataObject_ContainsFileDropList_InvokeDefault_ReturnsFalse()
    {
        DataObject dataObject = new();
        Assert.False(dataObject.ContainsFileDropList());
    }

    [Theory]
    [BoolData]
    public void DataObject_ContainsFileDropList_InvokeMocked_CallsGetDataPresent(bool result)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.ContainsFileDropList())
            .CallBase();
        mockDataObject
            .Setup(o => o.GetDataPresent(DataFormats.FileDrop, true))
            .Returns(result)
            .Verifiable();
        Assert.Equal(result, mockDataObject.Object.ContainsFileDropList());
        mockDataObject.Verify(o => o.GetDataPresent(DataFormats.FileDrop, true), Times.Once());
    }

    [Fact]
    public void DataObject_ContainsImage_InvokeDefault_ReturnsFalse()
    {
        DataObject dataObject = new();
        Assert.False(dataObject.ContainsImage());
    }

    [Theory]
    [BoolData]
    public void DataObject_ContainsImage_InvokeMocked_CallsGetDataPresent(bool result)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.ContainsImage())
            .CallBase();
        mockDataObject
            .Setup(o => o.GetDataPresent(DataFormats.Bitmap, true))
            .Returns(result)
            .Verifiable();
        Assert.Equal(result, mockDataObject.Object.ContainsImage());
        mockDataObject.Verify(o => o.GetDataPresent(DataFormats.Bitmap, true), Times.Once());
    }

    [Fact]
    public void DataObject_ContainsText_InvokeDefault_ReturnsFalse()
    {
        DataObject dataObject = new();
        Assert.False(dataObject.ContainsText());
    }

    [Theory]
    [BoolData]
    public void DataObject_ContainsText_InvokeMocked_CallsGetDataPresent(bool result)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.ContainsText())
            .CallBase();
        mockDataObject
            .Setup(o => o.ContainsText(TextDataFormat.UnicodeText))
            .Returns(result)
            .Verifiable();
        Assert.Equal(result, mockDataObject.Object.ContainsText());
        mockDataObject.Verify(o => o.ContainsText(TextDataFormat.UnicodeText), Times.Once());
    }

    [Theory]
    [EnumData<TextDataFormat>]
    public void DataObject_ContainsText_InvokeTextDataFormat_ReturnsFalse(TextDataFormat format)
    {
        DataObject dataObject = new();
        Assert.False(dataObject.ContainsText(format));
    }

    public static IEnumerable<object[]> ContainsText_TextDataFormat_TestData()
    {
        foreach (bool result in new bool[] { true, false })
        {
            yield return new object[] { TextDataFormat.Text, DataFormats.UnicodeText, result };
            yield return new object[] { TextDataFormat.UnicodeText, DataFormats.UnicodeText, result };
            yield return new object[] { TextDataFormat.Rtf, DataFormats.Rtf, result };
            yield return new object[] { TextDataFormat.Html, DataFormats.Html, result };
            yield return new object[] { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, result };
        }
    }

    [Theory]
    [MemberData(nameof(ContainsText_TextDataFormat_TestData))]
    public void DataObject_ContainsText_InvokeTextDataFormatMocked_CallsGetDataPresent(TextDataFormat format, string expectedFormat, bool result)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.ContainsText(format))
            .CallBase();
        mockDataObject
            .Setup(o => o.GetDataPresent(expectedFormat, false))
            .Returns(result)
            .Verifiable();
        Assert.Equal(result, mockDataObject.Object.ContainsText(format));
        mockDataObject.Verify(o => o.GetDataPresent(expectedFormat, false), Times.Once());
    }

    [Theory]
    [InvalidEnumData<TextDataFormat>]
    public void DataObject_ContainsText_InvokeInvalidTextDataFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        DataObject dataObject = new();
        Assert.Throws<InvalidEnumArgumentException>("format", () => dataObject.ContainsText(format));
    }

    [Fact]
    public void DataObject_GetAudioStream_InvokeDefault_ReturnsNull()
    {
        DataObject dataObject = new();
        Assert.Null(dataObject.GetAudioStream());
    }

    public static IEnumerable<object[]> GetAudioStream_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new(), null };

        MemoryStream stream = new();
        yield return new object[] { stream, stream };
    }

    [Theory]
    [MemberData(nameof(GetAudioStream_TestData))]
    public void DataObject_GetAudioStream_InvokeWithData_ReturnsExpected(object result, Stream expected)
    {
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.WaveAudio, result);
        Assert.Same(expected, dataObject.GetAudioStream());
    }

    [Theory]
    [MemberData(nameof(GetAudioStream_TestData))]
    public void DataObject_GetAudioStream_InvokeMocked_ReturnsExpected(object result, Stream expected)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetAudioStream())
            .CallBase();
        mockDataObject
            .Setup(o => o.GetData(DataFormats.WaveAudio, false))
            .Returns(result)
            .Verifiable();
        Assert.Same(expected, mockDataObject.Object.GetAudioStream());
        mockDataObject.Verify(o => o.GetData(DataFormats.WaveAudio, false), Times.Once());
    }

    public static IEnumerable<object[]> GetData_String_TestData()
    {
        foreach (string format in s_clipboardFormats)
        {
            yield return new object[] { format };
        }

        foreach (string format in s_mappedFormats)
        {
            yield return new object[] { format };
        }

        yield return new object[] { "  " };
        yield return new object[] { string.Empty };
        yield return new object[] { null };
    }

    [Theory]
    [MemberData(nameof(GetData_String_TestData))]
    public void DataObject_GetData_InvokeStringDefault_ReturnsNull(string format)
    {
        DataObject dataObject = new();
        Assert.Null(dataObject.GetData(format));
    }

    public static IEnumerable<object[]> GetData_InvokeStringMocked_TestData()
    {
        foreach (object result in new object[] { new(), null })
        {
            yield return new object[] { "format", result };
            yield return new object[] { "  ", result };
            yield return new object[] { string.Empty, result };
            yield return new object[] { null, result };
        }
    }

    [Theory]
    [MemberData(nameof(GetData_InvokeStringMocked_TestData))]
    public void DataObject_GetData_InvokeStringMocked_CallsGetData(string format, object result)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetData(format))
            .CallBase();
        mockDataObject
            .Setup(o => o.GetData(format, true))
            .Returns(result)
            .Verifiable();
        Assert.Same(result, mockDataObject.Object.GetData(format));
        mockDataObject.Verify(o => o.GetData(format, true), Times.Once());
    }

    public static IEnumerable<object[]> GetData_StringIDataObject_TestData()
    {
        foreach (object result in new object[] { new(), null })
        {
            yield return new object[] { "format", result };
            yield return new object[] { "  ", result };
            yield return new object[] { string.Empty, result };
            yield return new object[] { null, result };
        }
    }

    [Theory]
    [MemberData(nameof(GetData_StringIDataObject_TestData))]
    public void DataObject_GetData_InvokeStringIDataObject_ReturnsExpected(string format, object result)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetData(format, true))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        Assert.Same(result, dataObject.GetData(format));
        mockDataObject.Verify(o => o.GetData(format, true), Times.Once());
    }

    public static IEnumerable<object[]> GetData_StringBool_TestData()
    {
        foreach (bool autoConvert in new bool[] { true, false })
        {
            foreach (string format in s_clipboardFormats)
            {
                yield return new object[] { format, autoConvert };
            }

            foreach (string format in s_mappedFormats)
            {
                yield return new object[] { format, autoConvert };
            }

            yield return new object[] { "  ", autoConvert };
            yield return new object[] { string.Empty, autoConvert };
            yield return new object[] { null, autoConvert };
        }
    }

    [Theory]
    [MemberData(nameof(GetData_StringBool_TestData))]
    public void DataObject_GetData_InvokeStringBoolDefault_ReturnsNull(string format, bool autoConvert)
    {
        DataObject dataObject = new();
        Assert.Null(dataObject.GetData(format, autoConvert));
    }

    public static IEnumerable<object[]> GetData_StringBoolIDataObject_TestData()
    {
        foreach (bool autoConvert in new bool[] { true, false })
        {
            foreach (object result in new object[] { new(), null })
            {
                yield return new object[] { "format", autoConvert, result };
                yield return new object[] { "  ", autoConvert, result };
                yield return new object[] { string.Empty, autoConvert, result };
                yield return new object[] { null, autoConvert, result };
            }
        }
    }

    [Theory]
    [MemberData(nameof(GetData_StringBoolIDataObject_TestData))]
    public void DataObject_GetData_InvokeStringBoolIDataObject_ReturnsExpected(string format, bool autoConvert, object result)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetData(format, autoConvert))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        Assert.Same(result, dataObject.GetData(format, autoConvert));
        mockDataObject.Verify(o => o.GetData(format, autoConvert), Times.Once());
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(null)]
    public void DataObject_GetData_InvokeTypeDefault_ReturnsNull(Type format)
    {
        DataObject dataObject = new();
        Assert.Null(dataObject.GetData(format));
    }

    public static IEnumerable<object[]> GetData_InvokeTypeMocked_TestData()
    {
        foreach (object result in new object[] { new(), null })
        {
            yield return new object[] { typeof(int), result, 1, result };
            yield return new object[] { null, result, 0, null };
        }
    }

    [Theory]
    [MemberData(nameof(GetData_InvokeTypeMocked_TestData))]
    public void DataObject_GetData_InvokeTypeMocked_CallsGetData(Type format, object result, int expectedCallCount, object expectedResult)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetData(It.IsAny<Type>()))
            .CallBase();
        string formatName = format?.FullName ?? "(null)";
        mockDataObject
            .Setup(o => o.GetData(formatName))
            .Returns(result)
            .Verifiable();
        Assert.Same(expectedResult, mockDataObject.Object.GetData(format));
        mockDataObject.Verify(o => o.GetData(formatName), Times.Exactly(expectedCallCount));
    }

    public static IEnumerable<object[]> GetDataPresent_String_TestData()
    {
        foreach (string format in s_clipboardFormats)
        {
            yield return new object[] { format };
        }

        foreach (string format in s_mappedFormats)
        {
            yield return new object[] { format };
        }

        yield return new object[] { "  " };
        yield return new object[] { string.Empty };
        yield return new object[] { null };
    }

    [Theory]
    [MemberData(nameof(GetDataPresent_String_TestData))]
    public void DataObject_GetDataPresent_InvokeStringDefault_ReturnsFalse(string format)
    {
        DataObject dataObject = new();
        Assert.False(dataObject.GetDataPresent(format));
    }

    public static IEnumerable<object[]> GetDataPresent_StringMocked_TestData()
    {
        foreach (bool result in new bool[] { true, false })
        {
            yield return new object[] { "format", result };
            yield return new object[] { "  ", result };
            yield return new object[] { string.Empty, result };
            yield return new object[] { null, result };
        }
    }

    [Theory]
    [MemberData(nameof(GetDataPresent_StringMocked_TestData))]
    public void DataObject_GetDataPresent_InvokeStringMocked_CallsGetDataPresent(string format, bool result)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetDataPresent(format))
            .CallBase();
        mockDataObject
            .Setup(o => o.GetDataPresent(format, true))
            .Returns(result)
            .Verifiable();
        Assert.Equal(result, mockDataObject.Object.GetDataPresent(format));
        mockDataObject.Verify(o => o.GetDataPresent(format, true), Times.Once());
    }

    public static IEnumerable<object[]> GetDataPresent_StringIDataObject_TestData()
    {
        foreach (bool result in new bool[] { true, false })
        {
            yield return new object[] { "format", result };
            yield return new object[] { "  ", result };
            yield return new object[] { string.Empty, result };
            yield return new object[] { null, result };
        }
    }

    [Theory]
    [MemberData(nameof(GetDataPresent_StringIDataObject_TestData))]
    public void DataObject_GetDataPresent_InvokeStringIDataObject_ReturnsExpected(string format, bool result)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetDataPresent(format, true))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        Assert.Equal(result, dataObject.GetDataPresent(format));
        mockDataObject.Verify(o => o.GetDataPresent(format, true), Times.Once());
    }

    public static IEnumerable<object[]> GetDataPresent_StringBool_TestData()
    {
        foreach (bool autoConvert in new bool[] { true, false })
        {
            foreach (string format in s_clipboardFormats)
            {
                yield return new object[] { format, autoConvert };
            }

            foreach (string format in s_mappedFormats)
            {
                yield return new object[] { format, autoConvert };
            }

            yield return new object[] { "  ", autoConvert };
            yield return new object[] { string.Empty, autoConvert };
            yield return new object[] { null, autoConvert };
        }
    }

    [Theory]
    [MemberData(nameof(GetDataPresent_StringBool_TestData))]
    public void DataObject_GetDataPresent_InvokeStringBoolDefault_ReturnsFalse(string format, bool autoConvert)
    {
        DataObject dataObject = new();
        Assert.False(dataObject.GetDataPresent(format, autoConvert));
    }

    public static IEnumerable<object[]> GetDataPresent_StringBoolIDataObject_TestData()
    {
        foreach (bool autoConvert in new bool[] { true, false })
        {
            foreach (bool result in new bool[] { true, false })
            {
                yield return new object[] { "format", autoConvert, result };
                yield return new object[] { "  ", autoConvert, result };
                yield return new object[] { string.Empty, autoConvert, result };
                yield return new object[] { null, autoConvert, result };
            }
        }
    }

    [Theory]
    [MemberData(nameof(GetDataPresent_StringBoolIDataObject_TestData))]
    public void DataObject_GetDataPresent_InvokeStringBoolIDataObject_ReturnsExpected(string format, bool autoConvert, bool result)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetDataPresent(format, autoConvert))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        Assert.Equal(result, dataObject.GetDataPresent(format, autoConvert));
        mockDataObject.Verify(o => o.GetDataPresent(format, autoConvert), Times.Once());
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(null)]
    public void DataObject_GetDataPresentPresent_InvokeTypeDefault_ReturnsFalse(Type format)
    {
        DataObject dataObject = new();
        Assert.False(dataObject.GetDataPresent(format));
    }

    public static IEnumerable<object[]> GetDataPresent_InvokeTypeMocked_TestData()
    {
        foreach (bool result in new bool[] { true, false })
        {
            yield return new object[] { typeof(int), result, 1, result, typeof(int).FullName };
            yield return new object[] { null, result, 0, false, "(null)" };
        }
    }

    [Theory]
    [MemberData(nameof(GetDataPresent_InvokeTypeMocked_TestData))]
    public void DataObject_GetDataPresent_InvokeTypeMocked_CallsGetDataPresent(Type format, bool result, int expectedCallCount, bool expectedResult, string expectedFormatName)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetDataPresent(It.IsAny<Type>()))
            .CallBase();
        mockDataObject
            .Setup(o => o.GetDataPresent(expectedFormatName))
            .Returns(result)
            .Verifiable();
        Assert.Equal(expectedResult, mockDataObject.Object.GetDataPresent(format));
        mockDataObject.Verify(o => o.GetDataPresent(expectedFormatName), Times.Exactly(expectedCallCount));
    }

    [Fact]
    public void DataObject_GetFileDropList_InvokeDefault_ReturnsEmpty()
    {
        DataObject dataObject = new();
        Assert.Empty(dataObject.GetFileDropList());
    }

    public static IEnumerable<object[]> GetFileDropList_TestData()
    {
        yield return new object[] { null, Array.Empty<string>() };
        yield return new object[] { new(), Array.Empty<string>() };

        string[] list = ["a", "  ", string.Empty, null];
        yield return new object[] { list, list };
    }

    [Theory]
    [MemberData(nameof(GetFileDropList_TestData))]
    public void DataObject_GetFileDropList_InvokeWithData_ReturnsExpected(object result, string[] expected)
    {
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.FileDrop, result);
        Assert.Equal(expected, dataObject.GetFileDropList().Cast<string>());
    }

    [Theory]
    [MemberData(nameof(GetFileDropList_TestData))]
    public void DataObject_GetFileDropList_InvokeMocked_ReturnsExpected(object result, string[] expected)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFileDropList())
            .CallBase();
        mockDataObject
            .Setup(o => o.GetData(DataFormats.FileDrop, true))
            .Returns(result)
            .Verifiable();
        Assert.Equal(expected, mockDataObject.Object.GetFileDropList().Cast<string>());
        mockDataObject.Verify(o => o.GetData(DataFormats.FileDrop, true), Times.Once());
    }

    [Fact]
    public void DataObject_GetFormats_InvokeDefault_ReturnsEmpty()
    {
        DataObject dataObject = new();
        Assert.Empty(dataObject.GetFormats());
    }

    [WinFormsFact]
    public void DataObject_GetFormats_InvokeWithValues_ReturnsExpected()
    {
        DataObject dataObject = new();
        dataObject.SetData("format1", "data1");
        Assert.Equal(new string[] { "format1" }, dataObject.GetFormats());

        dataObject.SetText("data2");
        Assert.Equal(new string[] { "format1", "UnicodeText" }, dataObject.GetFormats());

        using Bitmap bitmap1 = new(10, 10);
        dataObject.SetData("format2", bitmap1);
        Assert.Equal(new string[] { "format1", "format2", "UnicodeText" }, dataObject.GetFormats().OrderBy(s => s));

        using Bitmap bitmap2 = new(10, 10);
        dataObject.SetData(bitmap2);
        Assert.Equal(new string[] { "Bitmap", "format1", "format2", "System.Drawing.Bitmap", "UnicodeText", "WindowsForms10PersistentObject" }, dataObject.GetFormats().OrderBy(s => s));
    }

    public static IEnumerable<object[]> GetFormats_Mocked_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { Array.Empty<string>() };
        yield return new object[] { new string[] { "a", "  ", string.Empty, null } };
    }

    [Theory]
    [MemberData(nameof(GetFormats_Mocked_TestData))]
    public void DataObject_GetFormats_InvokeMocked_ReturnsExpected(string[] result)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats())
            .CallBase();
        mockDataObject
            .Setup(o => o.GetFormats(true))
            .Returns(result)
            .Verifiable();
        Assert.Same(result, mockDataObject.Object.GetFormats());
        mockDataObject.Verify(o => o.GetFormats(true), Times.Once());
    }

    public static IEnumerable<object[]> GetFormats_IDataObject_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { Array.Empty<string>() };
        yield return new object[] { new string[] { "a", string.Empty, null } };
        yield return new object[] { new string[] { "a", "  ", string.Empty, null } };
    }

    [Theory]
    [MemberData(nameof(GetFormats_IDataObject_TestData))]
    public void DataObject_GetFormats_InvokeIDataObject_ReturnsExpected(string[] result)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats(true))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        Assert.Same(result, dataObject.GetFormats());
        mockDataObject.Verify(o => o.GetFormats(true), Times.Once());
    }

    [Theory]
    [BoolData]
    public void DataObject_GetFormats_InvokeBoolDefault_ReturnsEmpty(bool autoConvert)
    {
        DataObject dataObject = new();
        Assert.Empty(dataObject.GetFormats(autoConvert));
    }

    [WinFormsFact]
    public void DataObject_GetFormats_InvokeBoolWithValues_ReturnsExpected()
    {
        DataObject dataObject = new();
        dataObject.SetData("format1", "data1");
        Assert.Equal(new string[] { "format1" }, dataObject.GetFormats(autoConvert: true));
        Assert.Equal(new string[] { "format1" }, dataObject.GetFormats(autoConvert: false));

        dataObject.SetText("data2");
        Assert.Equal(new string[] { "format1", "UnicodeText" }, dataObject.GetFormats(autoConvert: true));
        Assert.Equal(new string[] { "format1", "UnicodeText" }, dataObject.GetFormats(autoConvert: false));

        using Bitmap bitmap1 = new(10, 10);
        dataObject.SetData("format2", bitmap1);
        Assert.Equal(new string[] { "format1", "format2", "UnicodeText" }, dataObject.GetFormats(autoConvert: true).OrderBy(s => s));
        Assert.Equal(new string[] { "format1", "format2", "UnicodeText" }, dataObject.GetFormats(autoConvert: false).OrderBy(s => s));

        using Bitmap bitmap2 = new(10, 10);
        dataObject.SetData(bitmap2);
        Assert.Equal(new string[] { "Bitmap", "format1", "format2", "System.Drawing.Bitmap", "UnicodeText", "WindowsForms10PersistentObject" }, dataObject.GetFormats(autoConvert: true).OrderBy(s => s));
        Assert.Equal(new string[] { "format1", "format2", "System.Drawing.Bitmap", "UnicodeText", "WindowsForms10PersistentObject" }, dataObject.GetFormats(autoConvert: false).OrderBy(s => s));
    }

    public static IEnumerable<object[]> GetFormats_BoolIDataObject_TestData()
    {
        foreach (bool autoConvert in new bool[] { true, false })
        {
            yield return new object[] { autoConvert, null };
            yield return new object[] { autoConvert, Array.Empty<string>() };
            yield return new object[] { autoConvert, new string[] { "a", "  ", string.Empty, null } };
        }
    }

    [Theory]
    [MemberData(nameof(GetFormats_BoolIDataObject_TestData))]
    public void DataObject_GetFormats_InvokeBoolIDataObject_ReturnsExpected(bool autoConvert, string[] result)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats(autoConvert))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        Assert.Same(result, dataObject.GetFormats(autoConvert));
        mockDataObject.Verify(o => o.GetFormats(autoConvert), Times.Once());
    }

    [Fact]
    public void DataObject_GetImage_InvokeDefault_ReturnsNull()
    {
        DataObject dataObject = new();
        Assert.Null(dataObject.GetImage());
    }

    public static IEnumerable<object[]> GetImage_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new(), null };

        Bitmap image = new(10, 10);
        yield return new object[] { image, image };
    }

    [Theory]
    [MemberData(nameof(GetImage_TestData))]
    public void DataObject_GetImage_InvokeWithData_ReturnsExpected(object result, Image expected)
    {
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.Bitmap, result);
        Assert.Same(expected, dataObject.GetImage());
    }

    [Theory]
    [MemberData(nameof(GetImage_TestData))]
    public void DataObject_GetImage_InvokeMocked_ReturnsExpected(object result, Image expected)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetImage())
            .CallBase();
        mockDataObject
            .Setup(o => o.GetData(DataFormats.Bitmap, true))
            .Returns(result)
            .Verifiable();
        Assert.Same(expected, mockDataObject.Object.GetImage());
        mockDataObject.Verify(o => o.GetData(DataFormats.Bitmap, true), Times.Once());
    }

    [Fact]
    public void DataObject_GetText_InvokeDefault_ReturnsEmpty()
    {
        DataObject dataObject = new();
        Assert.Empty(dataObject.GetText());
    }

    public static IEnumerable<object[]> GetText_TestData()
    {
        yield return new object[] { null, string.Empty };
        yield return new object[] { string.Empty, string.Empty };
        yield return new object[] { "  ", "  " };
        yield return new object[] { "a", "a" };
    }

    [Theory]
    [MemberData(nameof(GetText_TestData))]
    public void DataObject_GetText_InvokeWithData_ReturnsExpected(object result, string expected)
    {
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.UnicodeText, result);
        Assert.Equal(expected, dataObject.GetText());
    }

    [Theory]
    [StringWithNullData]
    public void DataObject_GetText_InvokeMocked_ReturnsExpected(string result)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetText())
            .CallBase();
        mockDataObject
            .Setup(o => o.GetText(TextDataFormat.UnicodeText))
            .Returns(result)
            .Verifiable();
        Assert.Same(result, mockDataObject.Object.GetText());
        mockDataObject.Verify(o => o.GetText(TextDataFormat.UnicodeText), Times.Once());
    }

    [Theory]
    [EnumData<TextDataFormat>]
    public void DataObject_GetText_InvokeTextDataFormatDefault_ReturnsEmpty(TextDataFormat format)
    {
        DataObject dataObject = new();
        Assert.Empty(dataObject.GetText(format));
    }

    public static IEnumerable<object[]> GetText_TextDataFormat_TestData()
    {
        yield return new object[] { TextDataFormat.Text, DataFormats.UnicodeText, null, string.Empty };
        yield return new object[] { TextDataFormat.Text, DataFormats.UnicodeText, new(), string.Empty };
        yield return new object[] { TextDataFormat.Text, DataFormats.UnicodeText, string.Empty, string.Empty };
        yield return new object[] { TextDataFormat.Text, DataFormats.UnicodeText, "  ", "  " };
        yield return new object[] { TextDataFormat.Text, DataFormats.UnicodeText, "a", "a" };

        yield return new object[] { TextDataFormat.UnicodeText, DataFormats.UnicodeText, null, string.Empty };
        yield return new object[] { TextDataFormat.UnicodeText, DataFormats.UnicodeText, new(), string.Empty };
        yield return new object[] { TextDataFormat.UnicodeText, DataFormats.UnicodeText, string.Empty, string.Empty };
        yield return new object[] { TextDataFormat.UnicodeText, DataFormats.UnicodeText, "  ", "  " };
        yield return new object[] { TextDataFormat.UnicodeText, DataFormats.UnicodeText, "a", "a" };

        yield return new object[] { TextDataFormat.Rtf, DataFormats.Rtf, null, string.Empty };
        yield return new object[] { TextDataFormat.Rtf, DataFormats.Rtf, new(), string.Empty };
        yield return new object[] { TextDataFormat.Rtf, DataFormats.Rtf, string.Empty, string.Empty };
        yield return new object[] { TextDataFormat.Rtf, DataFormats.Rtf, "  ", "  " };
        yield return new object[] { TextDataFormat.Rtf, DataFormats.Rtf, "a", "a" };

        yield return new object[] { TextDataFormat.Html, DataFormats.Html, null, string.Empty };
        yield return new object[] { TextDataFormat.Html, DataFormats.Html, new(), string.Empty };
        yield return new object[] { TextDataFormat.Html, DataFormats.Html, string.Empty, string.Empty };
        yield return new object[] { TextDataFormat.Html, DataFormats.Html, "  ", "  " };
        yield return new object[] { TextDataFormat.Html, DataFormats.Html, "a", "a" };

        yield return new object[] { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, null, string.Empty };
        yield return new object[] { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, new(), string.Empty };
        yield return new object[] { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, string.Empty, string.Empty };
        yield return new object[] { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, "  ", "  " };
        yield return new object[] { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, "a", "a" };
    }

    [Theory]
    [MemberData(nameof(GetText_TextDataFormat_TestData))]
    public void DataObject_GetText_InvokeTextDataFormatWithData_ReturnsExpected(TextDataFormat format, string expectedFormat, object result, string expected)
    {
        DataObject dataObject = new();
        dataObject.SetData(expectedFormat, result);
        Assert.Same(expected, dataObject.GetText(format));
    }

    [Theory]
    [MemberData(nameof(GetText_TextDataFormat_TestData))]
    public void DataObject_GetText_InvokeTextDataFormatMocked_ReturnsExpected(TextDataFormat format, string expectedFormat, object result, string expected)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetText(format))
            .CallBase();
        mockDataObject
            .Setup(o => o.GetData(expectedFormat, false))
            .Returns(result)
            .Verifiable();
        Assert.Same(expected, mockDataObject.Object.GetText(format));
        mockDataObject.Verify(o => o.GetData(expectedFormat, false), Times.Once());
    }

    [Theory]
    [InvalidEnumData<TextDataFormat>]
    public void DataObject_GetText_InvokeInvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        DataObject dataObject = new();
        Assert.Throws<InvalidEnumArgumentException>("format", () => dataObject.GetText(format));
    }

    public static IEnumerable<object[]> SetAudio_ByteArray_TestData()
    {
        yield return new object[] { Array.Empty<byte>() };
        yield return new object[] { new byte[] { 1, 2, 3 } };
    }

    [Theory]
    [MemberData(nameof(SetAudio_ByteArray_TestData))]
    public void DataObject_SetAudio_InvokeByteArray_GetReturnsExpected(byte[] audioBytes)
    {
        DataObject dataObject = new();
        dataObject.SetAudio(audioBytes);
        Assert.Equal(audioBytes, Assert.IsType<MemoryStream>(dataObject.GetAudioStream()).ToArray());
        Assert.Equal(audioBytes, Assert.IsType<MemoryStream>(dataObject.GetData(DataFormats.WaveAudio, autoConvert: true)).ToArray());
        Assert.Equal(audioBytes, Assert.IsType<MemoryStream>(dataObject.GetData(DataFormats.WaveAudio, autoConvert: false)).ToArray());
        Assert.True(dataObject.ContainsAudio());
        Assert.True(dataObject.GetDataPresent(DataFormats.WaveAudio, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(DataFormats.WaveAudio, autoConvert: false));
    }

    [Theory]
    [MemberData(nameof(SetAudio_ByteArray_TestData))]
    public void DataObject_SetAudio_InvokeByteArrayMocked_CallsSetAudio(byte[] audioBytes)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetAudio(audioBytes))
            .CallBase();
        mockDataObject
            .Setup(o => o.SetAudio(It.IsAny<MemoryStream>()))
            .Verifiable();
        mockDataObject.Object.SetAudio(audioBytes);
        mockDataObject.Verify(o => o.SetAudio(It.IsAny<MemoryStream>()), Times.Once());
    }

    [Theory]
    [MemberData(nameof(SetAudio_ByteArray_TestData))]
    public void DataObject_SetAudio_InvokeByteArrayIDataObject_CallsSetData(byte[] audioBytes)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(DataFormats.WaveAudio, false, It.IsAny<MemoryStream>()))
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        dataObject.SetAudio(audioBytes);
        mockDataObject.Verify(o => o.SetData(DataFormats.WaveAudio, false, It.IsAny<MemoryStream>()), Times.Once());
    }

    [Fact]
    public void DataObject_SetAudio_NullAudioBytes_ThrowsArgumentNullException()
    {
        DataObject dataObject = new();
        Assert.Throws<ArgumentNullException>("audioBytes", () => dataObject.SetAudio((byte[])null));
    }

    public static IEnumerable<object[]> SetAudio_Stream_TestData()
    {
        yield return new object[] { new MemoryStream(Array.Empty<byte>()) };
        yield return new object[] { new MemoryStream(new byte[] { 1, 2, 3 }) };
    }

    [Theory]
    [MemberData(nameof(SetAudio_Stream_TestData))]
    public void DataObject_SetAudio_InvokeStream_GetReturnsExpected(Stream audioStream)
    {
        DataObject dataObject = new();
        dataObject.SetAudio(audioStream);
        Assert.Same(audioStream, dataObject.GetAudioStream());
        Assert.Same(audioStream, dataObject.GetData(DataFormats.WaveAudio, autoConvert: true));
        Assert.Same(audioStream, dataObject.GetData(DataFormats.WaveAudio, autoConvert: false));
        Assert.True(dataObject.ContainsAudio());
        Assert.True(dataObject.GetDataPresent(DataFormats.WaveAudio, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(DataFormats.WaveAudio, autoConvert: false));
    }

    [Theory]
    [MemberData(nameof(SetAudio_Stream_TestData))]
    public void DataObject_SetAudio_InvokeStreamMocked_CallsSetAudio(Stream audioStream)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetAudio(audioStream))
            .CallBase();
        mockDataObject
            .Setup(o => o.SetData(DataFormats.WaveAudio, false, audioStream))
            .Verifiable();
        mockDataObject.Object.SetAudio(audioStream);
        mockDataObject.Verify(o => o.SetData(DataFormats.WaveAudio, false, audioStream), Times.Once());
    }

    [Theory]
    [MemberData(nameof(SetAudio_Stream_TestData))]
    public void DataObject_SetAudio_InvokeStreamIDataObject_CallsSetData(Stream audioStream)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(DataFormats.WaveAudio, false, audioStream))
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        dataObject.SetAudio(audioStream);
        mockDataObject.Verify(o => o.SetData(DataFormats.WaveAudio, false, audioStream), Times.Once());
    }

    [Fact]
    public void DataObject_SetAudio_NullAudioStream_ThrowsArgumentNullException()
    {
        DataObject dataObject = new();
        Assert.Throws<ArgumentNullException>("audioStream", () => dataObject.SetAudio((Stream)null));
    }

    public static IEnumerable<object[]> SetData_Object_TestData()
    {
        yield return new object[] { new(), typeof(object).FullName };
        yield return new object[] { new Bitmap(10, 10), typeof(Bitmap).FullName };
        yield return new object[] { new Mock<ISerializable>(MockBehavior.Strict).Object, DataFormats.Serializable };
    }

    [Theory]
    [MemberData(nameof(SetData_Object_TestData))]
    public void SetData_Object_GetDataReturnsExpected(object data, string expectedFormat)
    {
        DataObject dataObject = new();
        dataObject.SetData(data);
        Assert.Same(data, dataObject.GetData(expectedFormat, autoConvert: false));
        Assert.Same(data, dataObject.GetData(expectedFormat, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(expectedFormat, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(expectedFormat, autoConvert: false));
    }

    [Fact]
    public void DataObject_SetData_MultipleNonSerializable_GetDataReturnsExpected()
    {
        object data1 = new();
        object data2 = new();

        DataObject dataObject = new();
        dataObject.SetData(data1);
        dataObject.SetData(data2);
        Assert.Same(data2, dataObject.GetData(data1.GetType().FullName, autoConvert: false));
        Assert.Same(data2, dataObject.GetData(data1.GetType().FullName, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(data1.GetType().FullName, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(data1.GetType().FullName, autoConvert: false));
    }

    [Fact]
    public void DataObject_SetData_MultipleSerializable_GetDataReturnsExpected()
    {
        var data1 = new Mock<ISerializable>(MockBehavior.Strict).Object;
        var data2 = new Mock<ISerializable>(MockBehavior.Strict).Object;

        DataObject dataObject = new();
        dataObject.SetData(data1);
        dataObject.SetData(data2);
        Assert.Same(data1, dataObject.GetData(DataFormats.Serializable, autoConvert: false));
        Assert.Same(data1, dataObject.GetData(DataFormats.Serializable, autoConvert: true));
        Assert.Same(data2, dataObject.GetData(data2.GetType().FullName, autoConvert: false));
        Assert.Same(data2, dataObject.GetData(data2.GetType().FullName, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(DataFormats.Serializable, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(DataFormats.Serializable, autoConvert: false));
        Assert.True(dataObject.GetDataPresent(data2.GetType().FullName, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(data2.GetType().FullName, autoConvert: false));
    }

    public static IEnumerable<object[]> SetData_ObjectIDataObject_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
    }

    [Theory]
    [MemberData(nameof(SetData_ObjectIDataObject_TestData))]
    public void DataObject_SetData_InvokeObjectIDataObject_CallsSetData(object data)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(data))
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        dataObject.SetData(data);
        mockDataObject.Verify(o => o.SetData(data), Times.Once());
    }

    [Fact]
    public void DataObject_SetData_NullData_ThrowsArgumentNullException()
    {
        DataObject dataObject = new();
        Assert.Throws<ArgumentNullException>("data", () => dataObject.SetData(null));
    }

    public static IEnumerable<object[]> SetData_StringObject_TestData()
    {
        foreach (string format in s_clipboardFormats)
        {
            yield return new object[] { format, null, format == DataFormats.FileDrop, format == DataFormats.Bitmap };
            yield return new object[] { format, "input", format == DataFormats.FileDrop, format == DataFormats.Bitmap };
        }

        yield return new object[] { typeof(Bitmap).FullName, null, false, true };
        yield return new object[] { typeof(Bitmap).FullName, "input", false, true };

        yield return new object[] { "FileName", null, true, false };
        yield return new object[] { "FileName", "input", true, false };

        yield return new object[] { "FileNameW", null, true, false };
        yield return new object[] { "FileNameW", "input", true, false };
    }

    [Theory]
    [MemberData(nameof(SetData_StringObject_TestData))]
    private void DataObject_SetData_InvokeStringObject_GetReturnsExpected(string format, object input, bool expectedContainsFileDropList, bool expectedContainsImage)
    {
        DataObject dataObject = new();
        dataObject.SetData(format, input);
        Assert.True(dataObject.GetDataPresent(format));
        Assert.True(dataObject.GetDataPresent(format, autoConvert: false));
        Assert.True(dataObject.GetDataPresent(format, autoConvert: true));
        Assert.Same(input, dataObject.GetData(format));
        Assert.Same(input, dataObject.GetData(format, autoConvert: false));
        Assert.Same(input, dataObject.GetData(format, autoConvert: true));
        Assert.Equal(format == DataFormats.WaveAudio, dataObject.ContainsAudio());
        Assert.Equal(expectedContainsFileDropList, dataObject.ContainsFileDropList());
        Assert.Equal(expectedContainsImage, dataObject.ContainsImage());
        Assert.Equal(format == DataFormats.UnicodeText, dataObject.ContainsText());
        Assert.Equal(format == DataFormats.UnicodeText, dataObject.ContainsText(TextDataFormat.Text));
        Assert.Equal(format == DataFormats.UnicodeText, dataObject.ContainsText(TextDataFormat.UnicodeText));
        Assert.Equal(format == DataFormats.Rtf, dataObject.ContainsText(TextDataFormat.Rtf));
        Assert.Equal(format == DataFormats.Html, dataObject.ContainsText(TextDataFormat.Html));
        Assert.Equal(format == DataFormats.CommaSeparatedValue, dataObject.ContainsText(TextDataFormat.CommaSeparatedValue));
    }

    [Fact]
    public void DataObject_SetData_InvokeStringObjectDibBitmapAutoConvert_GetDataReturnsExpected()
    {
        Bitmap image = new(10, 10);
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.Dib, true, image);
        Assert.Same(image, dataObject.GetImage());
        Assert.Same(image, dataObject.GetData(DataFormats.Bitmap, autoConvert: true));
        Assert.Same(image, dataObject.GetData(DataFormats.Bitmap, autoConvert: false));
        Assert.Same(image, dataObject.GetData(typeof(Bitmap).FullName, autoConvert: true));
        Assert.Null(dataObject.GetData(typeof(Bitmap).FullName, autoConvert: false));
        Assert.Null(dataObject.GetData(DataFormats.Dib, autoConvert: true));
        Assert.Null(dataObject.GetData(DataFormats.Dib, autoConvert: false));

        Assert.True(dataObject.ContainsImage());
        Assert.True(dataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: false));
        Assert.True(dataObject.GetDataPresent(typeof(Bitmap).FullName, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(typeof(Bitmap).FullName, autoConvert: false));
        Assert.False(dataObject.GetDataPresent(DataFormats.Dib, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(DataFormats.Dib, autoConvert: false));
    }

    public static IEnumerable<object[]> SetData_StringObjectIDataObject_TestData()
    {
        foreach (string format in new string[] { "format", "  ", string.Empty, null })
        {
            yield return new object[] { format, null };
            yield return new object[] { format, new() };
        }
    }

    [Theory]
    [MemberData(nameof(SetData_StringObjectIDataObject_TestData))]
    public void DataObject_SetData_InvokeStringObjectIDataObject_CallsSetData(string format, object data)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(format, data))
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        dataObject.SetData(format, data);
        mockDataObject.Verify(o => o.SetData(format, data), Times.Once());
    }

    public static IEnumerable<object[]> SetData_StringBoolObject_TestData()
    {
        foreach (string format in s_clipboardFormats)
        {
            foreach (bool autoConvert in new bool[] { true, false })
            {
                yield return new object[] { format, autoConvert, null, format == DataFormats.FileDrop, format == DataFormats.Bitmap };
                yield return new object[] { format, autoConvert, "input", format == DataFormats.FileDrop, format == DataFormats.Bitmap };
            }
        }

        yield return new object[] { typeof(Bitmap).FullName, false, null, false, false };
        yield return new object[] { typeof(Bitmap).FullName, false, "input", false, false };
        yield return new object[] { typeof(Bitmap).FullName, true, null, false, true };
        yield return new object[] { typeof(Bitmap).FullName, true, "input", false, true };

        yield return new object[] { "FileName", false, null, false, false };
        yield return new object[] { "FileName", false, "input", false, false };
        yield return new object[] { "FileName", true, null, true, false };
        yield return new object[] { "FileName", true, "input", true, false };

        yield return new object[] { "FileNameW", false, null, false, false };
        yield return new object[] { "FileNameW", false, "input", false, false };
        yield return new object[] { "FileNameW", true, null, true, false };
        yield return new object[] { "FileNameW", true, "input", true, false };
    }

    [Theory]
    [MemberData(nameof(SetData_StringBoolObject_TestData))]
    private void DataObject_SetData_InvokeStringBoolObject_GetReturnsExpected(string format, bool autoConvert, object input, bool expectedContainsFileDropList, bool expectedContainsImage)
    {
        DataObject dataObject = new();
        dataObject.SetData(format, autoConvert, input);
        Assert.Same(input, dataObject.GetData(format, autoConvert: false));
        Assert.Same(input, dataObject.GetData(format, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(format, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(format, autoConvert: false));
        Assert.Equal(format == DataFormats.WaveAudio, dataObject.ContainsAudio());
        Assert.Equal(expectedContainsFileDropList, dataObject.ContainsFileDropList());
        Assert.Equal(expectedContainsImage, dataObject.ContainsImage());
        Assert.Equal(format == DataFormats.UnicodeText, dataObject.ContainsText());
        Assert.Equal(format == DataFormats.UnicodeText, dataObject.ContainsText(TextDataFormat.Text));
        Assert.Equal(format == DataFormats.UnicodeText, dataObject.ContainsText(TextDataFormat.UnicodeText));
        Assert.Equal(format == DataFormats.Rtf, dataObject.ContainsText(TextDataFormat.Rtf));
        Assert.Equal(format == DataFormats.Html, dataObject.ContainsText(TextDataFormat.Html));
        Assert.Equal(format == DataFormats.CommaSeparatedValue, dataObject.ContainsText(TextDataFormat.CommaSeparatedValue));
    }

    [Fact]
    public void DataObject_SetData_InvokeStringBoolObjectDibBitmapAutoConvert_GetDataReturnsExpected()
    {
        Bitmap image = new(10, 10);
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.Dib, true, image);
        Assert.Same(image, dataObject.GetImage());
        Assert.Same(image, dataObject.GetData(DataFormats.Bitmap, autoConvert: true));
        Assert.Same(image, dataObject.GetData(DataFormats.Bitmap, autoConvert: false));
        Assert.Same(image, dataObject.GetData(typeof(Bitmap).FullName, autoConvert: true));
        Assert.Null(dataObject.GetData(typeof(Bitmap).FullName, autoConvert: false));
        Assert.Null(dataObject.GetData(DataFormats.Dib, autoConvert: true));
        Assert.Null(dataObject.GetData(DataFormats.Dib, autoConvert: false));

        Assert.True(dataObject.ContainsImage());
        Assert.True(dataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: false));
        Assert.True(dataObject.GetDataPresent(typeof(Bitmap).FullName, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(typeof(Bitmap).FullName, autoConvert: false));
        Assert.False(dataObject.GetDataPresent(DataFormats.Dib, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(DataFormats.Dib, autoConvert: false));
    }

    public static IEnumerable<object[]> SetData_StringBoolObjectIDataObject_TestData()
    {
        foreach (string format in new string[] { "format", "  ", string.Empty, null })
        {
            foreach (bool autoConvert in new bool[] { true, false })
            {
                yield return new object[] { format, autoConvert, null };
                yield return new object[] { format, autoConvert, new() };
            }
        }
    }

    [Theory]
    [MemberData(nameof(SetData_StringBoolObjectIDataObject_TestData))]
    public void DataObject_SetData_InvokeStringBoolObjectIDataObject_CallsSetData(string format, bool autoConvert, object data)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(format, autoConvert, data))
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        dataObject.SetData(format, autoConvert, data);
        mockDataObject.Verify(o => o.SetData(format, autoConvert, data), Times.Once());
    }

    public static IEnumerable<object[]> SetData_TypeObjectIDataObject_TestData()
    {
        foreach (Type format in new Type[] { typeof(int), null })
        {
            yield return new object[] { format, null };
            yield return new object[] { format, new() };
        }
    }

    [Theory]
    [MemberData(nameof(SetData_TypeObjectIDataObject_TestData))]
    public void DataObject_SetData_InvokeTypeObjectIDataObject_CallsSetData(Type format, object data)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(format, data))
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        dataObject.SetData(format, data);
        mockDataObject.Verify(o => o.SetData(format, data), Times.Once());
    }

    [Fact]
    public void DataObject_SetData_NullFormat_ThrowsArgumentNullException()
    {
        DataObject dataObject = new();
        Assert.Throws<ArgumentNullException>("format", () => dataObject.SetData((string)null, new object()));
        Assert.Throws<ArgumentNullException>("format", () => dataObject.SetData(null, true, new object()));
        Assert.Throws<ArgumentNullException>("format", () => dataObject.SetData(null, false, new object()));
        Assert.Throws<ArgumentNullException>("format", () => dataObject.SetData((Type)null, new object()));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void DataObject_SetData_WhitespaceOrEmptyFormat_ThrowsArgumentException(string format)
    {
        DataObject dataObject = new();
        Assert.Throws<ArgumentException>("format", () => dataObject.SetData(format, new object()));
        Assert.Throws<ArgumentException>("format", () => dataObject.SetData(format, true, new object()));
        Assert.Throws<ArgumentException>("format", () => dataObject.SetData(format, false, new object()));
    }

    [Fact]
    public void DataObject_SetData_DibBitmapNoAutoConvert_ThrowsNotSupportedException()
    {
        DataObject dataObject = new();
        Assert.Throws<NotSupportedException>(() => dataObject.SetData(DataFormats.Dib, false, new Bitmap(10, 10)));
    }

    public static IEnumerable<object[]> SetFileDropList_TestData()
    {
        yield return new object[] { new StringCollection() };
        yield return new object[] { new StringCollection { "file", "  ", string.Empty, null } };
    }

    [Theory]
    [MemberData(nameof(SetFileDropList_TestData))]
    public void DataObject_SetFileDropList_Invoke_GetReturnsExpected(StringCollection filePaths)
    {
        DataObject dataObject = new();
        dataObject.SetFileDropList(filePaths);
        Assert.Equal(filePaths, dataObject.GetFileDropList());
        Assert.Equal(filePaths.Cast<string>(), dataObject.GetData(DataFormats.FileDrop, autoConvert: true));
        Assert.Equal(filePaths.Cast<string>(), dataObject.GetData(DataFormats.FileDrop, autoConvert: false));
        Assert.Equal(filePaths.Cast<string>(), dataObject.GetData("FileName", autoConvert: true));
        Assert.Null(dataObject.GetData("FileName", autoConvert: false));
        Assert.Equal(filePaths.Cast<string>(), dataObject.GetData("FileNameW", autoConvert: true));
        Assert.Null(dataObject.GetData("FileNameW", autoConvert: false));

        Assert.True(dataObject.ContainsFileDropList());
        Assert.True(dataObject.GetDataPresent(DataFormats.FileDrop, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(DataFormats.FileDrop, autoConvert: false));
        Assert.True(dataObject.GetDataPresent("FileName", autoConvert: true));
        Assert.False(dataObject.GetDataPresent("FileName", autoConvert: false));
        Assert.True(dataObject.GetDataPresent("FileNameW", autoConvert: true));
        Assert.False(dataObject.GetDataPresent("FileNameW", autoConvert: false));
    }

    [Theory]
    [MemberData(nameof(SetFileDropList_TestData))]
    public void DataObject_SetFileDropList_InvokeMocked_CallsSetFileDropList(StringCollection filePaths)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetFileDropList(filePaths))
            .CallBase();
        mockDataObject
            .Setup(o => o.SetData(DataFormats.FileDrop, true, It.IsAny<string[]>()))
            .Verifiable();
        mockDataObject.Object.SetFileDropList(filePaths);
        mockDataObject.Verify(o => o.SetData(DataFormats.FileDrop, true, It.IsAny<string[]>()), Times.Once());
    }

    [Theory]
    [MemberData(nameof(SetFileDropList_TestData))]
    public void DataObject_SetFileDropList_InvokeIDataObject_CallsSetData(StringCollection filePaths)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(DataFormats.FileDrop, true, It.IsAny<string[]>()))
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        dataObject.SetFileDropList(filePaths);
        mockDataObject.Verify(o => o.SetData(DataFormats.FileDrop, true, It.IsAny<string[]>()), Times.Once());
    }

    [Fact]
    public void DataObject_SetFileDropList_NullFilePaths_ThrowsArgumentNullException()
    {
        DataObject dataObject = new();
        Assert.Throws<ArgumentNullException>("filePaths", () => dataObject.SetFileDropList(null));
    }

    public static IEnumerable<object[]> SetImage_TestData()
    {
        yield return new object[] { new Bitmap(10, 10) };
    }

    [Theory]
    [MemberData(nameof(SetImage_TestData))]
    public void DataObject_SetImage_Invoke_GetReturnsExpected(Image image)
    {
        DataObject dataObject = new();
        dataObject.SetImage(image);
        Assert.Same(image, dataObject.GetImage());
        Assert.Same(image, dataObject.GetData(DataFormats.Bitmap, autoConvert: true));
        Assert.Same(image, dataObject.GetData(DataFormats.Bitmap, autoConvert: false));
        Assert.Same(image, dataObject.GetData(typeof(Bitmap).FullName, autoConvert: true));
        Assert.Null(dataObject.GetData(typeof(Bitmap).FullName, autoConvert: false));
        Assert.Null(dataObject.GetData(DataFormats.Dib, autoConvert: true));
        Assert.Null(dataObject.GetData(DataFormats.Dib, autoConvert: false));

        Assert.True(dataObject.ContainsImage());
        Assert.True(dataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: false));
        Assert.True(dataObject.GetDataPresent(typeof(Bitmap).FullName, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(typeof(Bitmap).FullName, autoConvert: false));
        Assert.False(dataObject.GetDataPresent(DataFormats.Dib, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(DataFormats.Dib, autoConvert: false));
    }

    [Theory]
    [MemberData(nameof(SetImage_TestData))]
    public void DataObject_SetImage_InvokeMocked_CallsSetImage(Image image)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetImage(image))
            .CallBase();
        mockDataObject
            .Setup(o => o.SetData(DataFormats.Bitmap, true, image))
            .Verifiable();
        mockDataObject.Object.SetImage(image);
        mockDataObject.Verify(o => o.SetData(DataFormats.Bitmap, true, image), Times.Once());
    }

    [Theory]
    [MemberData(nameof(SetImage_TestData))]
    public void DataObject_SetImage_InvokeIDataObject_CallsSetData(Image image)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(DataFormats.Bitmap, true, image))
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        dataObject.SetImage(image);
        mockDataObject.Verify(o => o.SetData(DataFormats.Bitmap, true, image), Times.Once());
    }

    [Fact]
    public void DataObject_SetImage_NullImage_ThrowsArgumentNullException()
    {
        DataObject dataObject = new();
        Assert.Throws<ArgumentNullException>("image", () => dataObject.SetImage(null));
    }

    public static IEnumerable<object[]> SetText_String_TestData()
    {
        yield return new object[] { "  " };
        yield return new object[] { "textData" };
    }

    [Theory]
    [MemberData(nameof(SetText_String_TestData))]
    public void DataObject_SetText_InvokeString_GetReturnsExpected(string textData)
    {
        DataObject dataObject = new();
        dataObject.SetText(textData);
        Assert.Same(textData, dataObject.GetText());
        Assert.Same(textData, dataObject.GetData(DataFormats.UnicodeText, autoConvert: true));
        Assert.Same(textData, dataObject.GetData(DataFormats.UnicodeText, autoConvert: false));
        Assert.Same(textData, dataObject.GetData(DataFormats.Text, autoConvert: true));
        Assert.Null(dataObject.GetData(DataFormats.Text, autoConvert: false));
        Assert.Same(textData, dataObject.GetData(DataFormats.StringFormat, autoConvert: true));
        Assert.Null(dataObject.GetData(DataFormats.StringFormat, autoConvert: false));
        Assert.Null(dataObject.GetData(DataFormats.Rtf, autoConvert: true));
        Assert.Null(dataObject.GetData(DataFormats.Rtf, autoConvert: false));
        Assert.Null(dataObject.GetData(DataFormats.Html, autoConvert: true));
        Assert.Null(dataObject.GetData(DataFormats.Html, autoConvert: false));
        Assert.Null(dataObject.GetData(DataFormats.CommaSeparatedValue, autoConvert: true));
        Assert.Null(dataObject.GetData(DataFormats.CommaSeparatedValue, autoConvert: false));

        Assert.True(dataObject.ContainsText());
        Assert.True(dataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: true));
        Assert.True(dataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: false));
        Assert.False(dataObject.GetDataPresent(DataFormats.Text, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(DataFormats.Text, autoConvert: false));
        Assert.False(dataObject.GetDataPresent(DataFormats.StringFormat, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(DataFormats.StringFormat, autoConvert: false));
        Assert.False(dataObject.GetDataPresent(DataFormats.Rtf, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(DataFormats.Rtf, autoConvert: false));
        Assert.False(dataObject.GetDataPresent(DataFormats.Html, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(DataFormats.Html, autoConvert: false));
        Assert.False(dataObject.GetDataPresent(DataFormats.CommaSeparatedValue, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(DataFormats.CommaSeparatedValue, autoConvert: false));
    }

    [Theory]
    [MemberData(nameof(SetText_String_TestData))]
    public void DataObject_SetText_InvokeStringMocked_CallsSetText(string textData)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetText(textData))
            .CallBase();
        mockDataObject
            .Setup(o => o.SetText(textData, TextDataFormat.UnicodeText))
            .Verifiable();
        mockDataObject.Object.SetText(textData);
        mockDataObject.Verify(o => o.SetText(textData, TextDataFormat.UnicodeText), Times.Once());
    }

    [Theory]
    [MemberData(nameof(SetText_String_TestData))]
    public void DataObject_SetText_InvokeStringIDataObject_CallsSetData(string textData)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(DataFormats.UnicodeText, false, textData))
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        dataObject.SetText(textData);
        mockDataObject.Verify(o => o.SetData(DataFormats.UnicodeText, false, textData), Times.Once());
    }

    public static IEnumerable<object[]> SetText_StringTextDataFormat_TestData()
    {
        foreach (string textData in new string[] { "textData", "  " })
        {
            yield return new object[] { textData, TextDataFormat.Text, textData, null, null, null };
            yield return new object[] { textData, TextDataFormat.UnicodeText, textData, null, null, null };
            yield return new object[] { textData, TextDataFormat.Rtf, null, textData, null, null };
            yield return new object[] { textData, TextDataFormat.Html, null, null, textData, null };
            yield return new object[] { textData, TextDataFormat.CommaSeparatedValue, null, null, null, textData };
        }
    }

    [Theory]
    [MemberData(nameof(SetText_StringTextDataFormat_TestData))]
    public void DataObject_SetText_InvokeStringTextDataFormat_GetReturnsExpected(string textData, TextDataFormat format, string expectedUnicodeText, string expectedRtfText, string expectedHtmlText, string expectedCsvText)
    {
        DataObject dataObject = new();
        dataObject.SetText(textData, format);
        Assert.Equal(textData, dataObject.GetText(format));
        Assert.Equal(expectedUnicodeText, dataObject.GetData(DataFormats.UnicodeText, autoConvert: true));
        Assert.Equal(expectedUnicodeText, dataObject.GetData(DataFormats.UnicodeText, autoConvert: false));
        Assert.Equal(expectedUnicodeText, dataObject.GetData(DataFormats.Text, autoConvert: true));
        Assert.Null(dataObject.GetData(DataFormats.Text, autoConvert: false));
        Assert.Equal(expectedUnicodeText, dataObject.GetData(DataFormats.StringFormat, autoConvert: true));
        Assert.Null(dataObject.GetData(DataFormats.StringFormat, autoConvert: false));
        Assert.Equal(expectedRtfText, dataObject.GetData(DataFormats.Rtf, autoConvert: true));
        Assert.Equal(expectedRtfText, dataObject.GetData(DataFormats.Rtf, autoConvert: false));
        Assert.Equal(expectedHtmlText, dataObject.GetData(DataFormats.Html, autoConvert: true));
        Assert.Equal(expectedHtmlText, dataObject.GetData(DataFormats.Html, autoConvert: false));
        Assert.Equal(expectedCsvText, dataObject.GetData(DataFormats.CommaSeparatedValue, autoConvert: true));
        Assert.Equal(expectedCsvText, dataObject.GetData(DataFormats.CommaSeparatedValue, autoConvert: false));

        Assert.True(dataObject.ContainsText(format));
        Assert.Equal(expectedUnicodeText is not null, dataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: true));
        Assert.Equal(expectedUnicodeText is not null, dataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: false));
        Assert.False(dataObject.GetDataPresent(DataFormats.Text, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(DataFormats.Text, autoConvert: false));
        Assert.False(dataObject.GetDataPresent(DataFormats.StringFormat, autoConvert: true));
        Assert.False(dataObject.GetDataPresent(DataFormats.StringFormat, autoConvert: false));
        Assert.Equal(expectedRtfText is not null, dataObject.GetDataPresent(DataFormats.Rtf, autoConvert: true));
        Assert.Equal(expectedRtfText is not null, dataObject.GetDataPresent(DataFormats.Rtf, autoConvert: false));
        Assert.Equal(expectedHtmlText is not null, dataObject.GetDataPresent(DataFormats.Html, autoConvert: true));
        Assert.Equal(expectedHtmlText is not null, dataObject.GetDataPresent(DataFormats.Html, autoConvert: false));
        Assert.Equal(expectedCsvText is not null, dataObject.GetDataPresent(DataFormats.CommaSeparatedValue, autoConvert: true));
        Assert.Equal(expectedCsvText is not null, dataObject.GetDataPresent(DataFormats.CommaSeparatedValue, autoConvert: false));
    }

    public static IEnumerable<object[]> SetText_StringTextDataFormatMocked_TestData()
    {
        foreach (string textData in new string[] { "textData", "  " })
        {
            yield return new object[] { textData, TextDataFormat.Text, DataFormats.UnicodeText };
            yield return new object[] { textData, TextDataFormat.UnicodeText, DataFormats.UnicodeText };
            yield return new object[] { textData, TextDataFormat.Rtf, DataFormats.Rtf };
            yield return new object[] { textData, TextDataFormat.Html, DataFormats.Html };
            yield return new object[] { textData, TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue };
        }
    }

    [Theory]
    [MemberData(nameof(SetText_StringTextDataFormatMocked_TestData))]
    public void DataObject_SetText_InvokeStringTextDataFormatMocked_CallsSetText(string textData, TextDataFormat format, string expectedFormat)
    {
        Mock<DataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetText(textData, format))
            .CallBase();
        mockDataObject
            .Setup(o => o.SetData(expectedFormat, false, textData))
            .Verifiable();
        mockDataObject.Object.SetText(textData, format);
        mockDataObject.Verify(o => o.SetData(expectedFormat, false, textData), Times.Once());
    }

    [Theory]
    [MemberData(nameof(SetText_StringTextDataFormatMocked_TestData))]
    public void DataObject_SetText_InvokeStringTextDataFormatIDataObject_CallsSetData(string textData, TextDataFormat format, string expectedFormat)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(expectedFormat, false, textData))
            .Verifiable();
        DataObject dataObject = new((object)mockDataObject.Object);
        dataObject.SetText(textData, format);
        mockDataObject.Verify(o => o.SetData(expectedFormat, false, textData), Times.Once());
    }

    [Theory]
    [NullAndEmptyStringData]
    public void DataObject_SetText_NullOrEmptyTextData_ThrowsArgumentNullException(string textData)
    {
        DataObject dataObject = new();
        Assert.Throws<ArgumentNullException>("textData", () => dataObject.SetText(textData));
        Assert.Throws<ArgumentNullException>("textData", () => dataObject.SetText(textData, TextDataFormat.Text));
    }

    [Theory]
    [InvalidEnumData<TextDataFormat>]
    public void DataObject_SetText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        DataObject dataObject = new();
        Assert.Throws<InvalidEnumArgumentException>("format", () => dataObject.SetText("text", format));
    }

    [WinFormsFact]
    public void DataObject_GetData_EnhancedMetafile_DoesNotTerminateProcess()
    {
        DataObject data = new(new DataObjectIgnoringStorageMediumForEnhancedMetafile());

        // Office ignores the storage medium in GetData(EnhancedMetafile) and always returns a handle,
        // even when asked for a stream. This used to crash the process when DataObject interpreted the
        // handle as a pointer to a COM IStream without checking the storage medium it retrieved.

        Assert.Null(data.GetData(DataFormats.EnhancedMetafile));
    }

    private sealed class DataObjectIgnoringStorageMediumForEnhancedMetafile : IComDataObject
    {
        public void GetData(ref FORMATETC format, out STGMEDIUM medium)
        {
            Marshal.ThrowExceptionForHR(QueryGetData(ref format));

            using var metafile = new Drawing.Imaging.Metafile("bitmaps/milkmateya01.emf");

            medium = default;
            medium.tymed = TYMED.TYMED_ENHMF;
            medium.unionmember = metafile.GetHenhmetafile();
        }

        public int QueryGetData(ref FORMATETC format)
        {
            // do not check the requested storage medium, we always return a metafile handle, thats what Office does

            if (format.cfFormat != (short)CLIPBOARD_FORMAT.CF_ENHMETAFILE || format.dwAspect != DVASPECT.DVASPECT_CONTENT || format.lindex != -1)
                return (int)HRESULT.DV_E_FORMATETC;

            return 0;
        }

        public IEnumFORMATETC EnumFormatEtc(DATADIR direction) => throw new NotImplementedException();
        public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium) => throw new NotImplementedException();
        public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut) => throw new NotImplementedException();
        public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release) => throw new NotImplementedException();
        public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection) => throw new NotImplementedException();
        public void DUnadvise(int connection) => throw new NotImplementedException();
        public int EnumDAdvise(out IEnumSTATDATA enumAdvise) => throw new NotImplementedException();
    }

    public static IEnumerable<object[]> DAdvise_TestData()
    {
        yield return new object[] { ADVF.ADVF_DATAONSTOP, null };

        Mock<IAdviseSink> mockAdviseSink = new(MockBehavior.Strict);
        yield return new object[] { ADVF.ADVF_DATAONSTOP, mockAdviseSink.Object };
    }

    [WinFormsTheory]
    [MemberData(nameof(DAdvise_TestData))]
    public void DataObject_DAdvise_InvokeDefault_Success(ADVF advf, IAdviseSink adviseSink)
    {
        DataObject dataObject = new();
        IComDataObject comDataObject = dataObject;
        FORMATETC formatetc = new();
        Assert.Equal(HRESULT.E_NOTIMPL, (HRESULT)comDataObject.DAdvise(ref formatetc, advf, adviseSink, out int connection));
        Assert.Equal(0, connection);
    }

    private delegate void DAdviseCallback(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection);

    [WinFormsTheory]
    [MemberData(nameof(DAdvise_TestData))]
    public void DataObject_DAdvise_InvokeCustomComDataObject_Success(ADVF advf, IAdviseSink adviseSink)
    {
        Mock<IComDataObject> mockComDataObject = new(MockBehavior.Strict);
        mockComDataObject
            .Setup(o => o.DAdvise(ref It.Ref<FORMATETC>.IsAny, advf, adviseSink, out It.Ref<int>.IsAny))
            .Callback((DAdviseCallback)((ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection) =>
            {
                pFormatetc.cfFormat = 3;
                connection = 2;
            }))
            .Returns(1);
        DataObject dataObject = new(mockComDataObject.Object);
        IComDataObject comDataObject = dataObject;
        FORMATETC formatetc = new();
        Assert.Equal(1, comDataObject.DAdvise(ref formatetc, advf, adviseSink, out int connection));
        Assert.Equal(2, connection);
        Assert.Equal(3, formatetc.cfFormat);
        mockComDataObject.Verify(o => o.DAdvise(ref It.Ref<FORMATETC>.IsAny, advf, adviseSink, out It.Ref<int>.IsAny), Times.Once());
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    public void DataObject_DUnadvise_InvokeDefault_Success(int connection)
    {
        DataObject dataObject = new();
        IComDataObject comDataObject = dataObject;
        Assert.Throws<NotImplementedException>(() => comDataObject.DUnadvise(connection));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    public void DataObject_DUnadvise_InvokeCustomComDataObject_Success(int connection)
    {
        Mock<IComDataObject> mockComDataObject = new(MockBehavior.Strict);
        mockComDataObject
            .Setup(o => o.DUnadvise(connection))
            .Verifiable();
        DataObject dataObject = new(mockComDataObject.Object);
        IComDataObject comDataObject = dataObject;
        comDataObject.DUnadvise(connection);
        mockComDataObject.Verify(o => o.DUnadvise(connection), Times.Once());
    }

    [WinFormsFact]
    public void DataObject_EnumDAdvise_InvokeDefault_Success()
    {
        DataObject dataObject = new();
        IComDataObject comDataObject = dataObject;
        Assert.Equal(HRESULT.OLE_E_ADVISENOTSUPPORTED, (HRESULT)comDataObject.EnumDAdvise(out IEnumSTATDATA enumAdvise));
        Assert.Null(enumAdvise);
    }

    private delegate void EnumDAdviseCallback(out IEnumSTATDATA enumAdvise);

    public static IEnumerable<object[]> EnumDAdvise_CustomComDataObject_TestData()
    {
        yield return new object[] { null };

        Mock<IEnumSTATDATA> mockEnumStatData = new(MockBehavior.Strict);
        yield return new object[] { mockEnumStatData.Object };
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumDAdvise_CustomComDataObject_TestData))]
    public void DataObject_EnumDAdvise_InvokeCustomComDataObject_Success(IEnumSTATDATA result)
    {
        Mock<IComDataObject> mockComDataObject = new(MockBehavior.Strict);
        mockComDataObject
            .Setup(o => o.EnumDAdvise(out It.Ref<IEnumSTATDATA>.IsAny))
            .Callback((EnumDAdviseCallback)((out IEnumSTATDATA enumAdvise) =>
            {
                enumAdvise = result;
            }))
            .Returns(1);
        DataObject dataObject = new(mockComDataObject.Object);
        IComDataObject comDataObject = dataObject;
        Assert.Equal(1, comDataObject.EnumDAdvise(out IEnumSTATDATA enumStatData));
        Assert.Same(result, enumStatData);
        mockComDataObject.Verify(o => o.EnumDAdvise(out It.Ref<IEnumSTATDATA>.IsAny), Times.Once());
    }

    public static IEnumerable<object[]> EnumFormatEtc_Default_TestData()
    {
        yield return new object[] { -1 };
        yield return new object[] { 0 };
        yield return new object[] { 1 };
        yield return new object[] { 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_Default_TestData))]
    public void DataObject_EnumFormatEtc_InvokeDefault_Success(int celt)
    {
        DataObject dataObject = new();
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC enumerator = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        Assert.NotNull(enumerator);

        var result = new FORMATETC[1];
        int[] fetched = new int[1];

        for (int i = 0; i < 2; i++)
        {
            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(celt, result, fetched));
            Assert.Equal(0, result[0].cfFormat);
            Assert.Equal(0, fetched[0]);

            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(celt, null, null));

            Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Reset());
        }
    }

    public static IEnumerable<object[]> EnumFormatEtc_TestData()
    {
        yield return new object[] { DataFormats.Bitmap, TYMED.TYMED_GDI };
        yield return new object[] { DataFormats.CommaSeparatedValue, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.Dib, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.Dif, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.EnhancedMetafile, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.FileDrop, TYMED.TYMED_HGLOBAL };
        yield return new object[] { "FileName", TYMED.TYMED_HGLOBAL };
        yield return new object[] { "FileNameW", TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.Html, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.Locale, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.MetafilePict, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.OemText, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.Palette, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.PenData, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.Riff, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.Rtf, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.Serializable, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.StringFormat, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.SymbolicLink, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.Text, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.Tiff, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.UnicodeText, TYMED.TYMED_HGLOBAL };
        yield return new object[] { DataFormats.WaveAudio, TYMED.TYMED_HGLOBAL };
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_TestData))]
    public void DataObject_EnumFormatEtc_InvokeWithValues_Success(string format1, TYMED expectedTymed)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats(true))
            .Returns(new string[] { format1, "Format2" });
        DataObject dataObject = new(mockDataObject.Object);
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC enumerator = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        Assert.NotNull(enumerator);

        var result = new FORMATETC[2];
        int[] fetched = new int[2];

        for (int i = 0; i < 1; i++)
        {
            // Fetch nothing.
            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(0, result, fetched));
            Assert.Equal(0, fetched[0]);

            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(0, null, null));

            // Fetch negative.
            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(-1, result, fetched));
            Assert.Equal(0, fetched[0]);

            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(-1, null, null));

            // Null.
            Assert.Throws<NullReferenceException>(() => enumerator.Next(1, null, fetched));

            // Fetch first.
            Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Next(i + 1, result, fetched));
            Assert.Equal(unchecked((short)(ushort)(DataFormats.GetFormat(format1).Id)), result[0].cfFormat);
            Assert.Equal(DVASPECT.DVASPECT_CONTENT, result[0].dwAspect);
            Assert.Equal(-1, result[0].lindex);
            Assert.Equal(IntPtr.Zero, result[0].ptd);
            Assert.Equal(expectedTymed, result[0].tymed);
            Assert.Equal(0, result[1].cfFormat);
            Assert.Equal(1, fetched[0]);

            // Fetch second.
            Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Next(i + 1, result, fetched));
            Assert.NotEqual(0, result[0].cfFormat);
            Assert.Equal(DVASPECT.DVASPECT_CONTENT, result[0].dwAspect);
            Assert.Equal(-1, result[0].lindex);
            Assert.Equal(IntPtr.Zero, result[0].ptd);
            Assert.Equal(TYMED.TYMED_HGLOBAL, result[0].tymed);
            Assert.Equal(0, result[1].cfFormat);
            Assert.Equal(1, fetched[0]);

            // Fetch another.
            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(1, null, null));
            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(1, null, null));

            // Reset.
            Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Reset());
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_Default_TestData))]
    public void DataObject_EnumFormatEtc_InvokeNullFormats_Success(int celt)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats(true))
            .Returns((string[])null);
        DataObject dataObject = new(mockDataObject.Object);
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC enumerator = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        Assert.NotNull(enumerator);

        var result = new FORMATETC[1];
        int[] fetched = new int[1];

        for (int i = 0; i < 2; i++)
        {
            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(celt, result, fetched));
            Assert.Equal(0, result[0].cfFormat);
            Assert.Equal(0, fetched[0]);

            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(celt, null, null));

            Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Reset());
        }
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void DataObject_EnumFormatEtc_SkipDefault_Success(int celt)
    {
        DataObject dataObject = new();
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC enumerator = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        Assert.NotNull(enumerator);

        var result = new FORMATETC[1];
        int[] fetched = new int[1];
        Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Skip(celt));
        Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(1, result, fetched));

        // Negative.
        Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Skip(-1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => enumerator.Next(1, result, fetched));
    }

    [WinFormsFact]
    public void DataObject_EnumFormatEtc_SkipCustom_Success()
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats(true))
            .Returns(new string[] { "Format1", DataFormats.Bitmap, "Format2" });
        DataObject dataObject = new(mockDataObject.Object);
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC enumerator = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        Assert.NotNull(enumerator);

        var result = new FORMATETC[1];
        int[] fetched = new int[1];
        Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Skip(1));
        Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Next(1, result, fetched));
        Assert.Equal(2, result[0].cfFormat);
        Assert.Equal(DVASPECT.DVASPECT_CONTENT, result[0].dwAspect);
        Assert.Equal(-1, result[0].lindex);
        Assert.Equal(IntPtr.Zero, result[0].ptd);
        Assert.Equal(TYMED.TYMED_GDI, result[0].tymed);
        Assert.Equal(1, fetched[0]);

        // Skip negative.
        Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Skip(-2));
        Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Next(1, result, fetched));
        Assert.Equal(unchecked((short)(ushort)(DataFormats.GetFormat("Format1").Id)), result[0].cfFormat);
        Assert.Equal(DVASPECT.DVASPECT_CONTENT, result[0].dwAspect);
        Assert.Equal(-1, result[0].lindex);
        Assert.Equal(IntPtr.Zero, result[0].ptd);
        Assert.Equal(TYMED.TYMED_HGLOBAL, result[0].tymed);
        Assert.Equal(1, fetched[0]);

        // Skip end.
        Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Skip(1));
        Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Next(1, result, fetched));
        Assert.Equal(unchecked((short)(ushort)(DataFormats.GetFormat("Format2").Id)), result[0].cfFormat);
        Assert.Equal(DVASPECT.DVASPECT_CONTENT, result[0].dwAspect);
        Assert.Equal(-1, result[0].lindex);
        Assert.Equal(IntPtr.Zero, result[0].ptd);
        Assert.Equal(TYMED.TYMED_HGLOBAL, result[0].tymed);
        Assert.Equal(1, fetched[0]);

        Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Skip(0));
        Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Skip(1));

        // Negative.
        Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Skip(-4));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => enumerator.Next(1, result, fetched));
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_Default_TestData))]
    public void DataObject_EnumFormatEtc_CloneDefault_Success(int celt)
    {
        DataObject dataObject = new();
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC source = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        source.Clone(out IEnumFORMATETC enumerator);
        Assert.NotNull(enumerator);

        var result = new FORMATETC[1];
        int[] fetched = new int[1];

        for (int i = 0; i < 2; i++)
        {
            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(celt, result, fetched));
            Assert.Equal(0, result[0].cfFormat);
            Assert.Equal(0, fetched[0]);

            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(celt, null, null));

            Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Reset());
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_TestData))]
    public void DataObject_EnumFormatEtc_CloneWithValues_Success(string format1, TYMED expectedTymed)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats(true))
            .Returns(new string[] { format1, "Format2" });
        DataObject dataObject = new(mockDataObject.Object);
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC source = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        source.Clone(out IEnumFORMATETC enumerator);
        Assert.NotNull(enumerator);

        var result = new FORMATETC[2];
        int[] fetched = new int[2];

        for (int i = 0; i < 1; i++)
        {
            // Fetch nothing.
            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(0, result, fetched));
            Assert.Equal(0, fetched[0]);

            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(0, null, null));

            // Fetch negative.
            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(-1, result, fetched));
            Assert.Equal(0, fetched[0]);

            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(-1, null, null));

            // Null.
            Assert.Throws<NullReferenceException>(() => enumerator.Next(1, null, fetched));

            // Fetch first.
            Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Next(i + 1, result, fetched));
            Assert.Equal(unchecked((short)(ushort)(DataFormats.GetFormat(format1).Id)), result[0].cfFormat);
            Assert.Equal(DVASPECT.DVASPECT_CONTENT, result[0].dwAspect);
            Assert.Equal(-1, result[0].lindex);
            Assert.Equal(IntPtr.Zero, result[0].ptd);
            Assert.Equal(expectedTymed, result[0].tymed);
            Assert.Equal(0, result[1].cfFormat);
            Assert.Equal(1, fetched[0]);

            // Fetch second.
            Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Next(i + 1, result, fetched));
            Assert.NotEqual(0, result[0].cfFormat);
            Assert.Equal(DVASPECT.DVASPECT_CONTENT, result[0].dwAspect);
            Assert.Equal(-1, result[0].lindex);
            Assert.Equal(IntPtr.Zero, result[0].ptd);
            Assert.Equal(TYMED.TYMED_HGLOBAL, result[0].tymed);
            Assert.Equal(0, result[1].cfFormat);
            Assert.Equal(1, fetched[0]);

            // Fetch another.
            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(1, null, null));
            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(1, null, null));

            // Reset.
            Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Reset());
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_Default_TestData))]
    public void DataObject_EnumFormatEtc_CloneNullFormats_Success(int celt)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats(true))
            .Returns((string[])null);
        DataObject dataObject = new(mockDataObject.Object);
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC source = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        source.Clone(out IEnumFORMATETC enumerator);
        Assert.NotNull(enumerator);

        var result = new FORMATETC[1];
        int[] fetched = new int[1];

        for (int i = 0; i < 2; i++)
        {
            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(celt, result, fetched));
            Assert.Equal(0, result[0].cfFormat);
            Assert.Equal(0, fetched[0]);

            Assert.Equal(HRESULT.S_FALSE, (HRESULT)enumerator.Next(celt, null, null));

            Assert.Equal(HRESULT.S_OK, (HRESULT)enumerator.Reset());
        }
    }

    [WinFormsTheory]
    [InlineData(DATADIR.DATADIR_SET)]
    [InlineData(DATADIR.DATADIR_GET - 1)]
    [InlineData(DATADIR.DATADIR_SET + 1)]
    public void DataObject_EnumFormatEtc_InvokeNotGet_ThrowsExternalException(DATADIR dwDirection)
    {
        DataObject dataObject = new();
        IComDataObject comDataObject = dataObject;
        Assert.Throws<ExternalException>(() => comDataObject.EnumFormatEtc(dwDirection));
    }

    public static IEnumerable<object[]> EnumFormatEtc_CustomComDataObject_TestData()
    {
        yield return new object[] { DATADIR.DATADIR_GET, null };
        yield return new object[] { DATADIR.DATADIR_SET, null };
        yield return new object[] { DATADIR.DATADIR_GET - 1, null };
        yield return new object[] { DATADIR.DATADIR_SET + 1, null };

        Mock<IEnumFORMATETC> mockEnumFormatEtc = new(MockBehavior.Strict);
        yield return new object[] { DATADIR.DATADIR_GET, mockEnumFormatEtc.Object };
        yield return new object[] { DATADIR.DATADIR_SET, mockEnumFormatEtc.Object };
        yield return new object[] { DATADIR.DATADIR_GET - 1, mockEnumFormatEtc.Object };
        yield return new object[] { DATADIR.DATADIR_SET + 1, mockEnumFormatEtc.Object };
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_CustomComDataObject_TestData))]
    public void DataObject_EnumFormatEtc_InvokeCustomComDataObject_Success(DATADIR dwDirection, IEnumFORMATETC result)
    {
        Mock<IComDataObject> mockComDataObject = new(MockBehavior.Strict);
        mockComDataObject
            .Setup(o => o.EnumFormatEtc(dwDirection))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new(mockComDataObject.Object);
        IComDataObject comDataObject = dataObject;
        Assert.Same(result, comDataObject.EnumFormatEtc(dwDirection));
        mockComDataObject.Verify(o => o.EnumFormatEtc(dwDirection), Times.Once());
    }

    public static IEnumerable<object[]> GetDataHere_Text_TestData()
    {
        yield return new object[] { TextDataFormat.Rtf, (short)DataFormats.GetFormat(DataFormats.Rtf).Id };
        yield return new object[] { TextDataFormat.Html, (short)DataFormats.GetFormat(DataFormats.Html).Id };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetDataHere_Text_TestData))]
    public unsafe void IComDataObjectGetDataHere_Text_Success(TextDataFormat textDataFormat, short cfFormat)
    {
        DataObject dataObject = new();
        dataObject.SetText("text", textDataFormat);
        IComDataObject iComDataObject = dataObject;

        FORMATETC formatetc = new()
        {
            tymed = TYMED.TYMED_HGLOBAL,
            cfFormat = cfFormat
        };

        STGMEDIUM stgMedium = new()
        {
            tymed = TYMED.TYMED_HGLOBAL
        };

        HGLOBAL handle = PInvokeCore.GlobalAlloc(
            GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT,
            1);

        try
        {
            stgMedium.unionmember = handle;
            iComDataObject.GetDataHere(ref formatetc, ref stgMedium);

            sbyte* pChar = *(sbyte**)stgMedium.unionmember;
            Assert.Equal("text", new string(pChar));
        }
        finally
        {
            PInvokeCore.GlobalFree(handle);
        }
    }

    public static IEnumerable<object[]> GetDataHere_UnicodeText_TestData()
    {
        yield return new object[] { TextDataFormat.Text, (short)CLIPBOARD_FORMAT.CF_UNICODETEXT };
        yield return new object[] { TextDataFormat.UnicodeText, (short)CLIPBOARD_FORMAT.CF_UNICODETEXT };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetDataHere_UnicodeText_TestData))]
    public unsafe void IComDataObjectGetDataHere_UnicodeText_Success(TextDataFormat textDataFormat, short cfFormat)
    {
        DataObject dataObject = new();
        dataObject.SetText("text", textDataFormat);
        IComDataObject iComDataObject = dataObject;

        FORMATETC formatetc = new()
        {
            tymed = TYMED.TYMED_HGLOBAL,
            cfFormat = cfFormat
        };

        STGMEDIUM stgMedium = new()
        {
            tymed = TYMED.TYMED_HGLOBAL
        };

        HGLOBAL handle = PInvokeCore.GlobalAlloc(
            GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT,
            1);

        try
        {
            stgMedium.unionmember = handle;
            iComDataObject.GetDataHere(ref formatetc, ref stgMedium);

            char* pChar = *(char**)stgMedium.unionmember;
            Assert.Equal("text", new string(pChar));
        }
        finally
        {
            PInvokeCore.GlobalFree(handle);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(GetDataHere_Text_TestData))]
    public unsafe void IComDataObjectGetDataHere_TextNoData_ThrowsArgumentException(TextDataFormat textDataFormat, short cfFormat)
    {
        DataObject dataObject = new();
        dataObject.SetText("text", textDataFormat);
        IComDataObject iComDataObject = dataObject;

        FORMATETC formatetc = new()
        {
            tymed = TYMED.TYMED_HGLOBAL,
            cfFormat = cfFormat
        };

        STGMEDIUM stgMedium = new()
        {
            tymed = TYMED.TYMED_HGLOBAL
        };

        Assert.Throws<ArgumentException>(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium));
    }

    [WinFormsTheory]
    [MemberData(nameof(GetDataHere_UnicodeText_TestData))]
    public unsafe void IComDataObjectGetDataHere_UnicodeTextNoData_ThrowsArgumentException(TextDataFormat textDataFormat, short cfFormat)
    {
        DataObject dataObject = new();
        dataObject.SetText("text", textDataFormat);
        IComDataObject iComDataObject = dataObject;

        FORMATETC formatetc = new()
        {
            tymed = TYMED.TYMED_HGLOBAL,
            cfFormat = cfFormat
        };

        STGMEDIUM stgMedium = new()
        {
            tymed = TYMED.TYMED_HGLOBAL
        };

        Assert.Throws<ArgumentException>(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium));
    }

    [WinFormsFact]
    public unsafe void IComDataObjectGetDataHere_FileNames_Success()
    {
        DataObject dataObject = new();
        dataObject.SetFileDropList(new StringCollection { "Path1", "Path2" });
        IComDataObject iComDataObject = dataObject;

        FORMATETC formatetc = new()
        {
            tymed = TYMED.TYMED_HGLOBAL,
            cfFormat = (short)CLIPBOARD_FORMAT.CF_HDROP
        };

        STGMEDIUM stgMedium = new()
        {
            tymed = TYMED.TYMED_HGLOBAL
        };

        HGLOBAL handle = PInvokeCore.GlobalAlloc(
            GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT,
            1);

        try
        {
            stgMedium.unionmember = handle;
            iComDataObject.GetDataHere(ref formatetc, ref stgMedium);

            DROPFILES* pDropFiles = *(DROPFILES**)stgMedium.unionmember;
            Assert.Equal(20u, pDropFiles->pFiles);
            Assert.Equal(Point.Empty, pDropFiles->pt);
            Assert.False(pDropFiles->fNC);
            Assert.True(pDropFiles->fWide);
            char* text = (char*)IntPtr.Add((IntPtr)pDropFiles, (int)pDropFiles->pFiles);
            Assert.Equal("Path1\0Path2\0\0", new string(text, 0, "Path1".Length + 1 + "Path2".Length + 1 + 1));
        }
        finally
        {
            PInvokeCore.GlobalFree(handle);
        }
    }

    [WinFormsFact]
    public unsafe void IComDataObjectGetDataHere_EmptyFileNames_Success()
    {
        DataObject dataObject = new();
        dataObject.SetFileDropList(new StringCollection());
        IComDataObject iComDataObject = dataObject;

        FORMATETC formatetc = new()
        {
            tymed = TYMED.TYMED_HGLOBAL,
            cfFormat = (short)CLIPBOARD_FORMAT.CF_HDROP
        };

        STGMEDIUM stgMedium = new()
        {
            tymed = TYMED.TYMED_HGLOBAL
        };

        HGLOBAL handle = PInvokeCore.GlobalAlloc(
           GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT,
           (uint)sizeof(DROPFILES));

        try
        {
            stgMedium.unionmember = handle;
            iComDataObject.GetDataHere(ref formatetc, ref stgMedium);

            DROPFILES* pDropFiles = *(DROPFILES**)stgMedium.unionmember;
            Assert.Equal(0u, pDropFiles->pFiles);
            Assert.Equal(Point.Empty, pDropFiles->pt);
            Assert.False(pDropFiles->fNC);
            Assert.False(pDropFiles->fWide);
        }
        finally
        {
            PInvokeCore.GlobalFree(handle);
        }
    }

    [WinFormsFact]
    public unsafe void IComDataObjectGetDataHere_FileNamesNoData_ThrowsArgumentException()
    {
        DataObject dataObject = new();
        dataObject.SetFileDropList(new StringCollection { "Path1", "Path2" });
        IComDataObject iComDataObject = dataObject;

        FORMATETC formatetc = new()
        {
            tymed = TYMED.TYMED_HGLOBAL,
            cfFormat = (short)CLIPBOARD_FORMAT.CF_HDROP
        };
        STGMEDIUM stgMedium = new()
        {
            tymed = TYMED.TYMED_HGLOBAL
        };
        Assert.Throws<ArgumentException>(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium));
    }

    [WinFormsFact]
    public unsafe void IComDataObjectGetDataHere_EmptyFileNamesNoData_Success()
    {
        DataObject dataObject = new();
        dataObject.SetFileDropList(new StringCollection());
        IComDataObject iComDataObject = dataObject;

        FORMATETC formatetc = new()
        {
            tymed = TYMED.TYMED_HGLOBAL,
            cfFormat = (short)CLIPBOARD_FORMAT.CF_HDROP
        };
        STGMEDIUM stgMedium = new()
        {
            tymed = TYMED.TYMED_HGLOBAL
        };
        iComDataObject.GetDataHere(ref formatetc, ref stgMedium);
    }

    [WinFormsTheory]
    [InlineData(TYMED.TYMED_FILE, TYMED.TYMED_FILE)]
    [InlineData(TYMED.TYMED_ISTORAGE, TYMED.TYMED_ISTORAGE)]
    [InlineData(TYMED.TYMED_MFPICT, TYMED.TYMED_MFPICT)]
    [InlineData(TYMED.TYMED_ENHMF, TYMED.TYMED_ENHMF)]
    [InlineData(TYMED.TYMED_NULL, TYMED.TYMED_NULL)]
    public void IComDataObjectGetDataHere_InvalidTymed_ThrowsCOMException(TYMED formatetcTymed, TYMED stgMediumTymed)
    {
        DataObject dataObject = new();
        IComDataObject iComDataObject = dataObject;

        FORMATETC formatetc = new()
        {
            tymed = formatetcTymed
        };
        STGMEDIUM stgMedium = new()
        {
            tymed = stgMediumTymed
        };
        COMException ex = Assert.Throws<COMException>(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium));
        Assert.Equal(HRESULT.DV_E_TYMED, (HRESULT)ex.HResult);
    }

    [WinFormsTheory]
    [InlineData(TYMED.TYMED_HGLOBAL, TYMED.TYMED_HGLOBAL, (short)CLIPBOARD_FORMAT.CF_UNICODETEXT)]
    [InlineData(TYMED.TYMED_HGLOBAL, TYMED.TYMED_HGLOBAL, (short)CLIPBOARD_FORMAT.CF_HDROP)]
    [InlineData(TYMED.TYMED_ISTREAM, TYMED.TYMED_ISTREAM, (short)CLIPBOARD_FORMAT.CF_UNICODETEXT)]
    [InlineData(TYMED.TYMED_ISTREAM, TYMED.TYMED_ISTREAM, (short)CLIPBOARD_FORMAT.CF_HDROP)]
    [InlineData(TYMED.TYMED_GDI, TYMED.TYMED_GDI, (short)CLIPBOARD_FORMAT.CF_UNICODETEXT)]
    [InlineData(TYMED.TYMED_GDI, TYMED.TYMED_GDI, (short)CLIPBOARD_FORMAT.CF_HDROP)]
    public void IComDataObjectGetDataHere_NoDataPresentNoData_ThrowsCOMException(TYMED formatetcTymed, TYMED stgMediumTymed, short cfFormat)
    {
        DataObject dataObject = new();
        IComDataObject iComDataObject = dataObject;

        FORMATETC formatetc = new()
        {
            tymed = formatetcTymed,
            cfFormat = cfFormat
        };
        STGMEDIUM stgMedium = new()
        {
            tymed = stgMediumTymed
        };
        COMException ex = Assert.Throws<COMException>(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium));
        Assert.Equal(HRESULT.DV_E_FORMATETC, (HRESULT)ex.HResult);
    }

    private class DerivedDataObject : DataObject { }

    private class CustomDataObject : IComDataObject, IDataObject
    {
        public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection) => throw new NotImplementedException();
        public void DUnadvise(int connection) => throw new NotImplementedException();
        public int EnumDAdvise(out IEnumSTATDATA enumAdvise) => throw new NotImplementedException();
        public IEnumFORMATETC EnumFormatEtc(DATADIR direction) => throw new NotImplementedException();
        public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut) => throw new NotImplementedException();
        public void GetData(ref FORMATETC format, out STGMEDIUM medium) => throw new NotImplementedException();
        public object GetData(string format, bool autoConvert) => throw new NotImplementedException();
        public object GetData(string format) => throw new NotImplementedException();
        public object GetData(Type format) => throw new NotImplementedException();
        public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium) => throw new NotImplementedException();
        public bool GetDataPresent(string format, bool autoConvert) => throw new NotImplementedException();
        public bool GetDataPresent(string format) => throw new NotImplementedException();
        public bool GetDataPresent(Type format) => throw new NotImplementedException();
        public string[] GetFormats(bool autoConvert) => throw new NotImplementedException();
        public string[] GetFormats() => throw new NotImplementedException();
        public int QueryGetData(ref FORMATETC format) => throw new NotImplementedException();
        public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release) => throw new NotImplementedException();
        public void SetData(string format, bool autoConvert, object data) => throw new NotImplementedException();
        public void SetData(string format, object data) => throw new NotImplementedException();
        public void SetData(Type format, object data) => throw new NotImplementedException();
        public void SetData(object data) => throw new NotImplementedException();
    }

    public static IEnumerable<object[]> DataObjectMockRoundTripData()
    {
        yield return new object[] { new DataObject() };
        yield return new object[] { new DerivedDataObject() };
        yield return new object[] { new CustomDataObject() };
    }

    [WinFormsTheory]
    [MemberData(nameof(DataObjectMockRoundTripData))]
    public void DataObject_MockRoundTrip_IsSame(object data)
    {
        dynamic controlAccessor = typeof(Control).TestAccessor().Dynamic;
        dynamic dropTargetAccessor = typeof(DropTarget).TestAccessor().Dynamic;

        IComDataObject inData = controlAccessor.PrepareIncomingDragData(data);
        inData.Should().BeSameAs(data);

        IDataObject outData = dropTargetAccessor.PrepareOutgoingDropData(inData);
        outData.Should().BeSameAs(data);
    }

    [WinFormsFact]
    public void DataObject_StringData_MockRoundTrip_IsWrapped()
    {
        string testString = "Test";
        dynamic accessor = typeof(Control).TestAccessor().Dynamic;
        dynamic dropTargetAccessor = typeof(DropTarget).TestAccessor().Dynamic;

        IComDataObject inData = accessor.PrepareIncomingDragData(testString);
        inData.Should().BeAssignableTo<DataObject>();

        IDataObject outData = dropTargetAccessor.PrepareOutgoingDropData(inData);
        outData.Should().BeSameAs(inData);
        outData.GetData(typeof(string)).Should().Be(testString);
    }

    [WinFormsFact]
    public void DataObject_IDataObject_MockRoundTrip_IsWrapped()
    {
        CustomIDataObject data = new();
        dynamic accessor = typeof(Control).TestAccessor().Dynamic;
        dynamic dropTargetAccessor = typeof(DropTarget).TestAccessor().Dynamic;

        IComDataObject inData = accessor.PrepareIncomingDragData(data);
        inData.Should().BeAssignableTo<DataObject>();
        inData.Should().NotBeSameAs(data);

        IDataObject outData = dropTargetAccessor.PrepareOutgoingDropData(inData);
        outData.Should().BeSameAs(inData);
    }

    private class CustomIDataObject : IDataObject
    {
        public object GetData(string format, bool autoConvert) => throw new NotImplementedException();
        public object GetData(string format) => throw new NotImplementedException();
        public object GetData(Type format) => throw new NotImplementedException();
        public bool GetDataPresent(string format, bool autoConvert) => throw new NotImplementedException();
        public bool GetDataPresent(string format) => throw new NotImplementedException();
        public bool GetDataPresent(Type format) => throw new NotImplementedException();
        public string[] GetFormats(bool autoConvert) => throw new NotImplementedException();
        public string[] GetFormats() => throw new NotImplementedException();
        public void SetData(string format, bool autoConvert, object data) => throw new NotImplementedException();
        public void SetData(string format, object data) => throw new NotImplementedException();
        public void SetData(Type format, object data) => throw new NotImplementedException();
        public void SetData(object data) => throw new NotImplementedException();
    }

    [WinFormsFact]
    public void DataObject_ComTypesIDataObject_MockRoundTrip_IsWrapped()
    {
        CustomComTypesDataObject data = new();
        dynamic accessor = typeof(Control).TestAccessor().Dynamic;
        dynamic dropTargetAccessor = typeof(DropTarget).TestAccessor().Dynamic;

        IComDataObject inData = accessor.PrepareIncomingDragData(data);
        inData.Should().BeSameAs(data);

        IDataObject outData = dropTargetAccessor.PrepareOutgoingDropData(inData);
        outData.Should().BeAssignableTo<DataObject>();
        outData.Should().NotBeSameAs(inData);
    }

    private class CustomComTypesDataObject : IComDataObject
    {
        public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection) => throw new NotImplementedException();
        public void DUnadvise(int connection) => throw new NotImplementedException();
        public int EnumDAdvise(out IEnumSTATDATA enumAdvise) => throw new NotImplementedException();
        public IEnumFORMATETC EnumFormatEtc(DATADIR direction) => throw new NotImplementedException();
        public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut) => throw new NotImplementedException();
        public void GetData(ref FORMATETC format, out STGMEDIUM medium) => throw new NotImplementedException();
        public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium) => throw new NotImplementedException();
        public int QueryGetData(ref FORMATETC format) => throw new NotImplementedException();
        public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release) => throw new NotImplementedException();
    }
}
