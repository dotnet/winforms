// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Private.Windows;
using System.Private.Windows.Core.Ole;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms.TestUtilities;
using Moq;
using Windows.Win32.System.Ole;
using Com = Windows.Win32.System.Com;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using Point = System.Drawing.Point;
using SimpleTestData = System.Windows.Forms.TestUtilities.DataObjectTestHelpers.SimpleTestData;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public partial class DataObjectTests
{
#pragma warning disable WFDEV005  // Type or member is obsolete
    private static readonly string[] s_restrictedClipboardFormats =
    [
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
        "FileName",
        "FileNameW",
        "System.Drawing.Bitmap",
        "  ",  // the last 3 return null and don't process the payload.
        string.Empty,
        null
    ];

    private static readonly string[] s_unboundedClipboardFormats =
    [
        DataFormats.Serializable,
        "something custom"
    ];

    [Fact]
    public void DataObject_ContainsAudio_InvokeDefault_ReturnsFalse()
    {
        DataObject dataObject = new();
        dataObject.ContainsAudio().Should().BeFalse();
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
        mockDataObject.Object.ContainsAudio().Should().Be(result);
        mockDataObject.Verify(o => o.GetDataPresent(DataFormats.WaveAudio, false), Times.Once());
    }

    [Fact]
    public void DataObject_ContainsFileDropList_InvokeDefault_ReturnsFalse()
    {
        DataObject dataObject = new();
        dataObject.ContainsFileDropList().Should().BeFalse();
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
        mockDataObject.Object.ContainsFileDropList().Should().Be(result);
        mockDataObject.Verify(o => o.GetDataPresent(DataFormats.FileDrop, true), Times.Once());
    }

    [Fact]
    public void DataObject_ContainsImage_InvokeDefault_ReturnsFalse()
    {
        DataObject dataObject = new();
        dataObject.ContainsImage().Should().BeFalse();
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
        mockDataObject.Object.ContainsImage().Should().Be(result);
        mockDataObject.Verify(o => o.GetDataPresent(DataFormats.Bitmap, true), Times.Once());
    }

    [Fact]
    public void DataObject_ContainsText_InvokeDefault_ReturnsFalse()
    {
        DataObject dataObject = new();
        dataObject.ContainsText().Should().BeFalse();
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
        mockDataObject.Object.ContainsText().Should().Be(result);
        mockDataObject.Verify(o => o.ContainsText(TextDataFormat.UnicodeText), Times.Once());
    }

    [Theory]
    [EnumData<TextDataFormat>]
    public void DataObject_ContainsText_InvokeTextDataFormat_ReturnsFalse(TextDataFormat format)
    {
        DataObject dataObject = new();
        dataObject.ContainsText(format).Should().BeFalse();
    }

    public static TheoryData<TextDataFormat, string, bool> ContainsText_TextDataFormat_TheoryData()
    {
        TheoryData<TextDataFormat, string, bool> theoryData = [];
        foreach (bool result in new bool[] { true, false })
        {
            theoryData.Add(TextDataFormat.Text, DataFormats.UnicodeText, result);
            theoryData.Add(TextDataFormat.UnicodeText, DataFormats.UnicodeText, result);
            theoryData.Add(TextDataFormat.Rtf, DataFormats.Rtf, result);
            theoryData.Add(TextDataFormat.Html, DataFormats.Html, result);
            theoryData.Add(TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, result);
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(ContainsText_TextDataFormat_TheoryData))]
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
        mockDataObject.Object.ContainsText(format).Should().Be(result);
        mockDataObject.Verify(o => o.GetDataPresent(expectedFormat, false), Times.Once());
    }

    [Theory]
    [InvalidEnumData<TextDataFormat>]
    public void DataObject_ContainsText_InvokeInvalidTextDataFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        DataObject dataObject = new();
        ((Action)(() => dataObject.ContainsText(format))).Should().Throw<InvalidEnumArgumentException>().WithParameterName("format");
    }

    [Fact]
    public void DataObject_GetAudioStream_InvokeDefault_ReturnsNull()
    {
        DataObject dataObject = new();
        dataObject.GetAudioStream().Should().BeNull();
    }

    public static TheoryData<object, Stream> GetAudioStream_TheoryData()
    {
        TheoryData<object, Stream> theoryData = new()
        {
            { null, null },
            { new(), null }
        };
        MemoryStream stream = new();
        theoryData.Add(stream, stream);

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(GetAudioStream_TheoryData))]
    public void DataObject_GetAudioStream_InvokeWithData_ReturnsExpected(object result, Stream expected)
    {
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.WaveAudio, result);
        dataObject.GetAudioStream().Should().BeSameAs(expected);
    }

    [Theory]
    [MemberData(nameof(GetAudioStream_TheoryData))]
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
        mockDataObject.Object.GetAudioStream().Should().BeSameAs(expected);
        mockDataObject.Verify(o => o.GetData(DataFormats.WaveAudio, false), Times.Once());
    }

    public static TheoryData<string> GetData_String_TheoryData() => s_restrictedClipboardFormats.ToTheoryData();

    public static TheoryData<string> GetData_String_UnboundedFormat_TheoryData() => s_unboundedClipboardFormats.ToTheoryData();

    [Theory]
    [MemberData(nameof(GetData_String_TheoryData))]
    [MemberData(nameof(GetData_String_UnboundedFormat_TheoryData))]
    public void DataObject_GetData_InvokeStringDefault_ReturnsNull(string format)
    {
        DataObject dataObject = new();
        dataObject.GetData(format).Should().BeNull();
    }

    public static TheoryData<string, object> GetData_InvokeStringMocked_TheoryData()
    {
        TheoryData<string, object> theoryData = [];
        foreach (object result in new object[] { new(), null })
        {
            theoryData.Add("format", result);
            theoryData.Add("  ", result);
            theoryData.Add(string.Empty, result);
            theoryData.Add(null, result);
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(GetData_InvokeStringMocked_TheoryData))]
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
        mockDataObject.Object.GetData(format).Should().BeSameAs(result);
        mockDataObject.Verify(o => o.GetData(format, true), Times.Once());
    }

    [Theory]
    [MemberData(nameof(GetData_InvokeStringMocked_TheoryData))]
    public void DataObject_GetData_InvokeStringIDataObject_ReturnsExpected(string format, object result)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetData(format, true))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.GetData(format).Should().BeSameAs(result);
        mockDataObject.Verify(o => o.GetData(format, true), Times.Once());
    }

    public static TheoryData<string, bool, object> GetData_StringBoolIDataObject_TheoryData()
    {
        TheoryData<string, bool, object> theoryData = [];
        foreach (bool autoConvert in new bool[] { true, false })
        {
            foreach (object result in new object[] { new(), null })
            {
                theoryData.Add("format", autoConvert, result);
                theoryData.Add("  ", autoConvert, result);
                theoryData.Add(string.Empty, autoConvert, result);
                theoryData.Add(null, autoConvert, result);
            }
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(GetData_StringBoolIDataObject_TheoryData))]
    public void DataObject_GetData_InvokeStringBoolIDataObject_ReturnsExpected(string format, bool autoConvert, object result)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetData(format, autoConvert))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.GetData(format, autoConvert).Should().BeSameAs(result);
        mockDataObject.Verify(o => o.GetData(format, autoConvert), Times.Once());
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(null)]
    public void DataObject_GetData_InvokeTypeDefault_ReturnsNull(Type format)
    {
        DataObject dataObject = new();
        dataObject.GetData(format).Should().BeNull();
    }

    public static TheoryData<Type, object, int, object> GetData_InvokeTypeMocked_TheoryData()
    {
        TheoryData<Type, object, int, object> theoryData = new();
        foreach (object result in new object[] { new(), null })
        {
            theoryData.Add(typeof(int), result, 1, result);
            theoryData.Add(null, result, 0, null);
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(GetData_InvokeTypeMocked_TheoryData))]
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
        mockDataObject.Object.GetData(format).Should().BeSameAs(expectedResult);
        mockDataObject.Verify(o => o.GetData(formatName), Times.Exactly(expectedCallCount));
    }

    #nullable enable
    internal class DataObjectOverridesTryGetDataCore : DataObject
    {
        private readonly string _format;
        private readonly Func<TypeName, Type>? _resolver;
        // true is the default value for general purpose APIs (GetData).
        private readonly bool _autoConvert;

        public DataObjectOverridesTryGetDataCore(string format, Func<TypeName, Type>? resolver, bool autoConvert) : base()
        {
            _format = format;
            _resolver = resolver;
            _autoConvert = autoConvert;
        }

        public int Count { get; private set; }
        public static Type Resolver(TypeName _) => typeof(string);

        protected override bool TryGetDataCore<T>(
            string format,
            Func<TypeName, Type>? resolver,
            bool autoConvert,
            [NotNullWhen(true), MaybeNullWhen(false)] out T data)
        {
            format.Should().Be(_format);
            resolver.Should().BeEquivalentTo(_resolver);
            autoConvert.Should().Be(_autoConvert);
            typeof(T).Should().Be(typeof(string));

            Count++;
            data = default;
            return false;
        }
    }

    [Fact]
    public void DataObject_TryGetData_InvokeString_CallsTryGetDataCore()
    {
        DataObjectOverridesTryGetDataCore dataObject = new(typeof(string).FullName!, resolver: null, autoConvert: true);
        dataObject.Count.Should().Be(0);

        dataObject.TryGetData(out string? data).Should().BeFalse();
        data.Should().BeNull();
        dataObject.Count.Should().Be(1);
    }

    public static TheoryData<string> RestrictedAndUnrestrictedFormat =>
    [
        DataFormats.CommaSeparatedValue,
        "something custom"
    ];

    [Theory]
    [MemberData(nameof(RestrictedAndUnrestrictedFormat))]
    public void TryGetData_InvokeStringString_CallsTryGetDataCore(string format)
    {
        DataObjectOverridesTryGetDataCore dataObject = new(format, null, autoConvert: true);
        dataObject.Count.Should().Be(0);

        dataObject.TryGetData(format, out string? data).Should().BeFalse();
        data.Should().BeNull();
        dataObject.Count.Should().Be(1);
    }

    [Fact]
    public void TryGetData_InvokeStringString_ValidationFails()
    {
        string format = DataFormats.Bitmap;
        DataObjectOverridesTryGetDataCore dataObject = new(format, null, autoConvert: true);
        dataObject.Count.Should().Be(0);

        // Incompatible format and type.
        Action tryGetData = () => dataObject.TryGetData(format, out string? data);
        tryGetData.Should().Throw<NotSupportedException>();
        dataObject.Count.Should().Be(0);
    }

    public static TheoryData<string, bool> FormatAndAutoConvert => new()
    {
        { DataFormats.CommaSeparatedValue, true },
        { "something custom", true },
        { DataFormats.CommaSeparatedValue, false },
        { "something custom", false }
    };

    [Theory]
    [MemberData(nameof(FormatAndAutoConvert))]
    public void TryGetData_InvokeStringBoolString_CallsTryGetDataCore(string format, bool autoConvert)
    {
        DataObjectOverridesTryGetDataCore dataObject = new(format, resolver: null, autoConvert);
        dataObject.Count.Should().Be(0);

        dataObject.TryGetData(format, autoConvert, out string? data).Should().BeFalse();
        data.Should().BeNull();
        dataObject.Count.Should().Be(1);
    }

    private static Type NotSupportedResolver(TypeName typeName) => throw new NotSupportedException();

    [Theory]
    [BoolData]
    public void TryGetData_InvokeStringBoolString_ValidationFails(bool autoConvert)
    {
        string format = DataFormats.Bitmap;
        DataObjectOverridesTryGetDataCore dataObject = new(format, NotSupportedResolver, autoConvert);
        dataObject.Count.Should().Be(0);

        Action tryGetData = () => dataObject.TryGetData(format, autoConvert, out string? data);
        tryGetData.Should().Throw<NotSupportedException>();
        dataObject.Count.Should().Be(0);
    }

    [Theory]
    [MemberData(nameof(FormatAndAutoConvert))]
    public void DataObject_TryGetData_InvokeStringFuncBoolString_CallsTryGetDataCore(string format, bool autoConvert)
    {
        DataObjectOverridesTryGetDataCore dataObject = new(format, DataObjectOverridesTryGetDataCore.Resolver, autoConvert);
        dataObject.Count.Should().Be(0);

        dataObject.TryGetData(format, DataObjectOverridesTryGetDataCore.Resolver, autoConvert, out string? data).Should().BeFalse();
        data.Should().BeNull();
        dataObject.Count.Should().Be(1);
    }

    [Theory]
    [BoolData]
    public void TryGetData_InvokeStringFuncBoolString_ValidationFails(bool autoConvert)
    {
        string format = DataFormats.Bitmap;
        DataObjectOverridesTryGetDataCore dataObject = new(format, DataObjectOverridesTryGetDataCore.Resolver, autoConvert);
        dataObject.Count.Should().Be(0);

        Action tryGetData = () => dataObject.TryGetData(format, DataObjectOverridesTryGetDataCore.Resolver, autoConvert, out string? data);
        tryGetData.Should().Throw<NotSupportedException>();
        dataObject.Count.Should().Be(0);
    }
    #nullable disable

    [Theory]
    [MemberData(nameof(GetData_String_TheoryData))]
    [MemberData(nameof(GetData_String_UnboundedFormat_TheoryData))]
    public void DataObject_GetDataPresent_InvokeStringDefault_ReturnsFalse(string format)
    {
        DataObject dataObject = new();
        dataObject.GetDataPresent(format).Should().BeFalse();
    }

    public static TheoryData<string, bool> GetDataPresent_StringMocked_TheoryData()
    {
        TheoryData<string, bool> theoryData = [];
        foreach (bool result in new bool[] { true, false })
        {
            theoryData.Add("format", result);
            theoryData.Add("  ", result);
            theoryData.Add(string.Empty, result);
            theoryData.Add(null, result);
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(GetDataPresent_StringMocked_TheoryData))]
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
        mockDataObject.Object.GetDataPresent(format).Should().Be(result);
        mockDataObject.Verify(o => o.GetDataPresent(format, true), Times.Once());
    }

    [Theory]
    [MemberData(nameof(GetDataPresent_StringMocked_TheoryData))]
    public void DataObject_GetDataPresent_InvokeStringIDataObject_ReturnsExpected(string format, bool result)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetDataPresent(format, true))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.GetDataPresent(format).Should().Be(result);
        mockDataObject.Verify(o => o.GetDataPresent(format, true), Times.Once());
    }

    public static TheoryData<string, bool, bool> GetDataPresent_StringBoolIDataObject_TheoryData()
    {
        TheoryData<string, bool, bool> theoryData = [];
        foreach (bool autoConvert in new bool[] { true, false })
        {
            foreach (bool result in new bool[] { true, false })
            {
                theoryData.Add("format", autoConvert, result);
                theoryData.Add("  ", autoConvert, result);
                theoryData.Add(string.Empty, autoConvert, result);
                theoryData.Add(null, autoConvert, result);
            }
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(GetDataPresent_StringBoolIDataObject_TheoryData))]
    public void DataObject_GetDataPresent_InvokeStringBoolIDataObject_ReturnsExpected(string format, bool autoConvert, bool result)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetDataPresent(format, autoConvert))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.GetDataPresent(format, autoConvert).Should().Be(result);
        mockDataObject.Verify(o => o.GetDataPresent(format, autoConvert), Times.Once());
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(null)]
    public void DataObject_GetDataPresentPresent_InvokeTypeDefault_ReturnsFalse(Type format)
    {
        DataObject dataObject = new();
        dataObject.GetDataPresent(format).Should().BeFalse();
    }

    public static TheoryData<Type, bool, int, bool, string> GetDataPresent_InvokeTypeMocked_TheoryData()
    {
        TheoryData<Type, bool, int, bool, string> theoryData = [];
        foreach (bool result in new bool[] { true, false })
        {
            theoryData.Add(typeof(int), result, 1, result, typeof(int).FullName);
            theoryData.Add(null, result, 0, false, "(null)");
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(GetDataPresent_InvokeTypeMocked_TheoryData))]
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
        mockDataObject.Object.GetDataPresent(format).Should().Be(expectedResult);
        mockDataObject.Verify(o => o.GetDataPresent(expectedFormatName), Times.Exactly(expectedCallCount));
    }

    [Fact]
    public void DataObject_GetFileDropList_InvokeDefault_ReturnsEmpty()
    {
        DataObject dataObject = new();
        dataObject.GetFileDropList().Cast<string>().Should().BeEmpty();
    }

    public static TheoryData<object, string[]> GetFileDropList_TheoryData()
    {
        TheoryData<object, string[]> theoryData = [];
        theoryData.Add(null, Array.Empty<string>());
        theoryData.Add(new(), Array.Empty<string>());
        string[] list = ["a", "  ", string.Empty, null];
        theoryData.Add(list, list);

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(GetFileDropList_TheoryData))]
    public void DataObject_GetFileDropList_InvokeWithData_ReturnsExpected(object result, string[] expected)
    {
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.FileDrop, result);
        dataObject.GetFileDropList().Cast<string>().Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(GetFileDropList_TheoryData))]
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
        mockDataObject.Object.GetFileDropList().Cast<string>().Should().BeEquivalentTo(expected);
        mockDataObject.Verify(o => o.GetData(DataFormats.FileDrop, true), Times.Once());
    }

    [Fact]
    public void DataObject_GetFormats_InvokeDefault_ReturnsEmpty()
    {
        DataObject dataObject = new();
        dataObject.GetFormats().Cast<string>().Should().BeEmpty();
    }

    [WinFormsFact]
    public void DataObject_GetFormats_InvokeWithValues_ReturnsExpected()
    {
        DataObject dataObject = new();
        dataObject.SetData("format1", "data1");
        dataObject.GetFormats().Should().Equal(["format1"]);

        dataObject.SetText("data2");
        dataObject.GetFormats().Should().Equal(["format1", "UnicodeText"]);

        using Bitmap bitmap1 = new(10, 10);
        dataObject.SetData("format2", bitmap1);
        dataObject.GetFormats().OrderBy(s => s).Should().Equal(["format1", "format2", "UnicodeText"]);

        using Bitmap bitmap2 = new(10, 10);
        dataObject.SetData(bitmap2);
        dataObject.GetFormats().OrderBy(s => s).Should().Equal(["Bitmap", "format1", "format2", "System.Drawing.Bitmap", "UnicodeText", "WindowsForms10PersistentObject"]);
    }

    public static TheoryData<string[]> GetFormats_Mocked_TheoryData() => new()
    {
        { null },
        { Array.Empty<string>() },
        { new string[] { "a", "  ", string.Empty, null } }
    };

    [Theory]
    [MemberData(nameof(GetFormats_Mocked_TheoryData))]
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
        mockDataObject.Object.GetFormats().Should().BeSameAs(result);
        mockDataObject.Verify(o => o.GetFormats(true), Times.Once());
    }

    public static TheoryData<string[]> GetFormats_IDataObject_TheoryData() => new()
    {
        { null },
        { Array.Empty<string>() },
        { new string[] { "a", string.Empty, null } },
        { new string[] { "a", "  ", string.Empty, null } }
    };

    [Theory]
    [MemberData(nameof(GetFormats_IDataObject_TheoryData))]
    public void DataObject_GetFormats_InvokeIDataObject_ReturnsExpected(string[] result)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats(true))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.GetFormats().Should().BeSameAs(result);
        mockDataObject.Verify(o => o.GetFormats(true), Times.Once());
    }

    [Theory]
    [BoolData]
    public void DataObject_GetFormats_InvokeBoolDefault_ReturnsEmpty(bool autoConvert)
    {
        DataObject dataObject = new();
        dataObject.GetFormats(autoConvert).Should().BeEmpty();
    }

    [WinFormsFact]
    public void DataObject_GetFormats_InvokeBoolWithValues_ReturnsExpected()
    {
        DataObject dataObject = new();
        dataObject.SetData("format1", "data1");
        dataObject.GetFormats(autoConvert: true).Should().Equal(["format1"]);
        dataObject.GetFormats(autoConvert: false).Should().Equal(["format1"]);

        dataObject.SetText("data2");
        dataObject.GetFormats(autoConvert: true).Should().Equal(["format1", "UnicodeText"]);
        dataObject.GetFormats(autoConvert: false).Should().Equal(["format1", "UnicodeText"]);

        using Bitmap bitmap1 = new(10, 10);
        dataObject.SetData("format2", bitmap1);
        dataObject.GetFormats(autoConvert: true).OrderBy(s => s).Should().Equal(["format1", "format2", "UnicodeText"]);
        dataObject.GetFormats(autoConvert: false).OrderBy(s => s).Should().Equal(["format1", "format2", "UnicodeText"]);

        using Bitmap bitmap2 = new(10, 10);
        dataObject.SetData(bitmap2);
        dataObject.GetFormats(autoConvert: true).OrderBy(s => s).Should().Equal(["Bitmap", "format1", "format2", "System.Drawing.Bitmap", "UnicodeText", "WindowsForms10PersistentObject"]);
        dataObject.GetFormats(autoConvert: false).OrderBy(s => s).Should().Equal(["format1", "format2", "System.Drawing.Bitmap", "UnicodeText", "WindowsForms10PersistentObject"]);
    }

    public static TheoryData<bool, string[]> GetFormats_BoolIDataObject_TheoryData() => new()
    {
        { true, null },
        { true, Array.Empty<string>() },
        { true, new string[] { "a", "  ", string.Empty, null } },
        { false, null },
        { false, Array.Empty<string>() },
        { false, new string[] { "a", "  ", string.Empty, null } }
    };

    [Theory]
    [MemberData(nameof(GetFormats_BoolIDataObject_TheoryData))]
    public void DataObject_GetFormats_InvokeBoolIDataObject_ReturnsExpected(bool autoConvert, string[] result)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats(autoConvert))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.GetFormats(autoConvert).Should().BeSameAs(result);
        mockDataObject.Verify(o => o.GetFormats(autoConvert), Times.Once());
    }

    [Fact]
    public void DataObject_GetImage_InvokeDefault_ReturnsNull()
    {
        DataObject dataObject = new();
        dataObject.GetImage().Should().BeNull();
    }

    public static TheoryData<object, Image> GetImage_TheoryData()
    {
        TheoryData<object, Image> theoryData = [];
        theoryData.Add(null, null);
        theoryData.Add(new(), null);
        // This bitmap is not backed up by the GDI handle, we don't have to dispose it.
        Bitmap image = new(10, 10);
        theoryData.Add(image, image);

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(GetImage_TheoryData))]
    public void DataObject_GetImage_InvokeWithData_ReturnsExpected(object result, Image expected)
    {
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.Bitmap, result);
        dataObject.GetImage().Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(GetImage_TheoryData))]
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
        mockDataObject.Object.GetImage().Should().BeSameAs(expected);
        mockDataObject.Verify(o => o.GetData(DataFormats.Bitmap, true), Times.Once());
    }

    [Fact]
    public void DataObject_GetText_InvokeDefault_ReturnsEmpty()
    {
        DataObject dataObject = new();
        dataObject.GetText().Should().BeEmpty();
    }

    public static TheoryData<string, string> GetText_TheoryData() => new()
    {
        { null, string.Empty},
        { string.Empty, string.Empty},
        { "  ", "  "},
        { "a", "a"}
    };

    [Theory]
    [MemberData(nameof(GetText_TheoryData))]
    public void DataObject_GetText_InvokeWithData_ReturnsExpected(string result, string expected)
    {
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.UnicodeText, result);
        dataObject.GetText().Should().Be(expected);
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
        mockDataObject.Object.GetText().Should().BeSameAs(result);
        mockDataObject.Verify(o => o.GetText(TextDataFormat.UnicodeText), Times.Once());
    }

    [Theory]
    [EnumData<TextDataFormat>]
    public void DataObject_GetText_InvokeTextDataFormatDefault_ReturnsEmpty(TextDataFormat format)
    {
        DataObject dataObject = new();
        dataObject.GetText(format).Should().BeEmpty();
    }

    public static TheoryData<TextDataFormat, string, object, string> GetText_TextDataFormat_TheoryData() => new()
    {
        { TextDataFormat.Text, DataFormats.UnicodeText, null, string.Empty },
        { TextDataFormat.Text, DataFormats.UnicodeText, new(), string.Empty },
        { TextDataFormat.Text, DataFormats.UnicodeText, string.Empty, string.Empty },
        { TextDataFormat.Text, DataFormats.UnicodeText, "  ", "  " },
        { TextDataFormat.Text, DataFormats.UnicodeText, "a", "a" },
        //
        { TextDataFormat.UnicodeText, DataFormats.UnicodeText, null, string.Empty },
        { TextDataFormat.UnicodeText, DataFormats.UnicodeText, new(), string.Empty },
        { TextDataFormat.UnicodeText, DataFormats.UnicodeText, string.Empty, string.Empty },
        { TextDataFormat.UnicodeText, DataFormats.UnicodeText, "  ", "  " },
        { TextDataFormat.UnicodeText, DataFormats.UnicodeText, "a", "a" },
        //
        { TextDataFormat.Rtf, DataFormats.Rtf, null, string.Empty },
        { TextDataFormat.Rtf, DataFormats.Rtf, new(), string.Empty },
        { TextDataFormat.Rtf, DataFormats.Rtf, string.Empty, string.Empty },
        { TextDataFormat.Rtf, DataFormats.Rtf, "  ", "  " },
        { TextDataFormat.Rtf, DataFormats.Rtf, "a", "a" },
        //
        { TextDataFormat.Html, DataFormats.Html, null, string.Empty },
        { TextDataFormat.Html, DataFormats.Html, new(), string.Empty },
        { TextDataFormat.Html, DataFormats.Html, string.Empty, string.Empty },
        { TextDataFormat.Html, DataFormats.Html, "  ", "  " },
        { TextDataFormat.Html, DataFormats.Html, "a", "a" },
        //
        { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, null, string.Empty },
        { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, new(), string.Empty },
        { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, string.Empty, string.Empty },
        { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, "  ", "  " },
        { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, "a", "a" },
    };

    [Theory]
    [MemberData(nameof(GetText_TextDataFormat_TheoryData))]
    public void DataObject_GetText_InvokeTextDataFormatWithData_ReturnsExpected(TextDataFormat format, string expectedFormat, object result, string expected)
    {
        DataObject dataObject = new();
        dataObject.SetData(expectedFormat, result);
        dataObject.GetText(format).Should().BeSameAs(expected);
    }

    [Theory]
    [MemberData(nameof(GetText_TextDataFormat_TheoryData))]
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
        mockDataObject.Object.GetText(format).Should().BeSameAs(expected);
        mockDataObject.Verify(o => o.GetData(expectedFormat, false), Times.Once());
    }

    [Theory]
    [InvalidEnumData<TextDataFormat>]
    public void DataObject_GetText_InvokeInvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        DataObject dataObject = new();
        Action action = () => dataObject.GetText(format);
        action.Should().Throw<InvalidEnumArgumentException>().WithParameterName("format");
    }

    public static TheoryData<byte[]> SetAudio_ByteArray_TheoryData() => new()
    {
        { Array.Empty<byte>() },
        { new byte[] { 1, 2, 3 } }
    };

    [Theory]
    [MemberData(nameof(SetAudio_ByteArray_TheoryData))]
    public void DataObject_SetAudio_InvokeByteArray_GetReturnsExpected(byte[] audioBytes)
    {
        DataObject dataObject = new();
        dataObject.SetAudio(audioBytes);

        dataObject.GetAudioStream().Should().BeOfType<MemoryStream>().Subject.ToArray().Should().BeEquivalentTo(audioBytes);
        dataObject.GetData(DataFormats.WaveAudio, autoConvert: true).Should().BeOfType<MemoryStream>().Subject.ToArray().Should().Equal(audioBytes);
        dataObject.GetData(DataFormats.WaveAudio, autoConvert: false).Should().BeOfType<MemoryStream>().Subject.ToArray().Should().Equal(audioBytes);

        dataObject.ContainsAudio().Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.WaveAudio, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.WaveAudio, autoConvert: false).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(SetAudio_ByteArray_TheoryData))]
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
    [MemberData(nameof(SetAudio_ByteArray_TheoryData))]
    public void DataObject_SetAudio_InvokeByteArrayIDataObject_CallsSetData(byte[] audioBytes)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(DataFormats.WaveAudio, false, It.IsAny<MemoryStream>()))
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.SetAudio(audioBytes);
        mockDataObject.Verify(o => o.SetData(DataFormats.WaveAudio, false, It.IsAny<MemoryStream>()), Times.Once());
    }

    [Fact]
    public void DataObject_SetAudio_NullAudioBytes_ThrowsArgumentNullException()
    {
        DataObject dataObject = new();
        Action action = () => dataObject.SetAudio((byte[])null);
        action.Should().Throw<ArgumentNullException>().WithParameterName("audioBytes");
    }

    public static TheoryData<MemoryStream> SetAudio_Stream_TheoryData() => new()
    {
        { new MemoryStream(Array.Empty<byte>()) },
        { new MemoryStream([1, 2, 3]) }
    };

    [Theory]
    [MemberData(nameof(SetAudio_Stream_TheoryData))]
    public void DataObject_SetAudio_InvokeStream_GetReturnsExpected(MemoryStream audioStream)
    {
        DataObject dataObject = new();
        dataObject.SetAudio(audioStream);

        dataObject.GetAudioStream().Should().BeSameAs(audioStream);
        dataObject.GetData(DataFormats.WaveAudio, autoConvert: true).Should().BeSameAs(audioStream);
        dataObject.GetData(DataFormats.WaveAudio, autoConvert: false).Should().BeSameAs(audioStream);

        dataObject.ContainsAudio().Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.WaveAudio, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.WaveAudio, autoConvert: false).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(SetAudio_Stream_TheoryData))]
    public void DataObject_SetAudio_InvokeStreamMocked_CallsSetAudio(MemoryStream audioStream)
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
    [MemberData(nameof(SetAudio_Stream_TheoryData))]
    public void DataObject_SetAudio_InvokeStreamIDataObject_CallsSetData(MemoryStream audioStream)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(DataFormats.WaveAudio, false, audioStream))
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.SetAudio(audioStream);
        mockDataObject.Verify(o => o.SetData(DataFormats.WaveAudio, false, audioStream), Times.Once());
    }

    [Fact]
    public void DataObject_SetAudio_NullAudioStream_ThrowsArgumentNullException()
    {
        DataObject dataObject = new();
        Action action = () => dataObject.SetAudio((Stream)null);
        action.Should().Throw<ArgumentNullException>().WithParameterName("audioStream");
    }

    public static TheoryData<object, string> SetData_Object_TheoryData() => new()
    {
        { new(), typeof(object).FullName },
        { new Bitmap(10, 10), typeof(Bitmap).FullName },
        { new Mock<ISerializable>(MockBehavior.Strict).Object, DataFormats.Serializable }
    };

    [Theory]
    [MemberData(nameof(SetData_Object_TheoryData))]
    public void DataObject_SetData_Object_GetDataReturnsExpected(object data, string format)
    {
        DataObject dataObject = new();
        dataObject.SetData(data);

        dataObject.GetData(format, autoConvert: false).Should().BeSameAs(data);
        dataObject.GetData(format, autoConvert: true).Should().BeSameAs(data);
        dataObject.GetDataPresent(format, autoConvert: false).Should().BeTrue();
        dataObject.GetDataPresent(format, autoConvert: true).Should().BeTrue();
    }

    [Fact]
    public void DataObject_SetData_MultipleNonSerializable_GetDataReturnsExpected()
    {
        object data1 = new();
        object data2 = new();
        DataObject dataObject = new();
        dataObject.SetData(data1);
        dataObject.SetData(data2);
        string format = data1.GetType().FullName;

        dataObject.GetData(format, autoConvert: false).Should().Be(data2);
        dataObject.GetData(format, autoConvert: true).Should().Be(data2);

        dataObject.GetDataPresent(format, autoConvert: false).Should().BeTrue();
        dataObject.GetDataPresent(format, autoConvert: true).Should().BeTrue();
    }

    [Fact]
    public void DataObject_SetData_MultipleSerializable_GetDataReturnsExpected()
    {
        var data1 = new Mock<ISerializable>(MockBehavior.Strict).Object;
        var data2 = new Mock<ISerializable>(MockBehavior.Strict).Object;
        DataObject dataObject = new();
        dataObject.SetData(data1);
        dataObject.SetData(data2);

        dataObject.GetData(DataFormats.Serializable, autoConvert: false).Should().Be(data1);
        dataObject.GetData(DataFormats.Serializable, autoConvert: true).Should().Be(data1);
        dataObject.GetData(data2.GetType().FullName, autoConvert: false).Should().Be(data2);
        dataObject.GetData(data2.GetType().FullName, autoConvert: true).Should().Be(data2);

        dataObject.GetDataPresent(DataFormats.Serializable, autoConvert: false).Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.Serializable, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(data2.GetType().FullName, autoConvert: false).Should().BeTrue();
        dataObject.GetDataPresent(data2.GetType().FullName, autoConvert: true).Should().BeTrue();
    }

    public static TheoryData<object> SetData_ObjectIDataObject_TheoryData() => new()
    {
        { new() },
        { null }
    };

    [Theory]
    [MemberData(nameof(SetData_ObjectIDataObject_TheoryData))]
    public void DataObject_SetData_InvokeObjectIDataObject_CallsSetData(object data)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(data))
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.SetData(data);
        mockDataObject.Verify(o => o.SetData(data), Times.Once());
    }

    [Fact]
    public void DataObject_SetData_NullData_ThrowsArgumentNullException()
    {
        DataObject dataObject = new();
        ((Action)(() => dataObject.SetData(null))).Should().Throw<ArgumentNullException>().WithParameterName("data");
    }

    public static TheoryData<string, string, bool, bool> SetData_StringObject_TheoryData()
    {
        TheoryData<string, string, bool, bool> theoryData = new();
        foreach (string format in s_restrictedClipboardFormats)
        {
            if (string.IsNullOrWhiteSpace(format) || format == typeof(Bitmap).FullName || format.StartsWith("FileName", StringComparison.Ordinal))
            {
                continue;
            }

            theoryData.Add(format, null, format == DataFormats.FileDrop, format == DataFormats.Bitmap);
            theoryData.Add(format, "input", format == DataFormats.FileDrop, format == DataFormats.Bitmap);
        }

        theoryData.Add(typeof(Bitmap).FullName, null, false, true);
        theoryData.Add(typeof(Bitmap).FullName, "input", false, true);

        theoryData.Add("FileName", null, true, false);
        theoryData.Add("FileName", "input", true, false);

        theoryData.Add("FileNameW", null, true, false);
        theoryData.Add("FileNameW", "input", true, false);

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(SetData_StringObject_TheoryData))]
    private void DataObject_SetData_InvokeStringObject_GetReturnsExpected(string format, string input, bool expectedContainsFileDropList, bool expectedContainsImage)
    {
        DataObject dataObject = new();
        dataObject.SetData(format, input);

        dataObject.GetDataPresent(format).Should().BeTrue();
        dataObject.GetDataPresent(format, autoConvert: false).Should().BeTrue();
        dataObject.GetDataPresent(format, autoConvert: true).Should().BeTrue();

        dataObject.GetData(format).Should().Be(input);
        dataObject.GetData(format, autoConvert: false).Should().Be(input);
        dataObject.GetData(format, autoConvert: true).Should().Be(input);

        dataObject.ContainsAudio().Should().Be(format == DataFormats.WaveAudio);
        dataObject.ContainsFileDropList().Should().Be(expectedContainsFileDropList);
        dataObject.ContainsImage().Should().Be(expectedContainsImage);
        dataObject.ContainsText().Should().Be(format == DataFormats.UnicodeText);
        dataObject.ContainsText(TextDataFormat.Text).Should().Be(format == DataFormats.UnicodeText);
        dataObject.ContainsText(TextDataFormat.UnicodeText).Should().Be(format == DataFormats.UnicodeText);
        dataObject.ContainsText(TextDataFormat.Rtf).Should().Be(format == DataFormats.Rtf);
        dataObject.ContainsText(TextDataFormat.Html).Should().Be(format == DataFormats.Html);
        dataObject.ContainsText(TextDataFormat.CommaSeparatedValue).Should().Be(format == DataFormats.CommaSeparatedValue);
    }

    [Theory]
    [InlineData(DataFormatNames.Serializable, null)]
    [InlineData(DataFormatNames.Serializable, "input")]
    [InlineData("something custom", null)]
    [InlineData("something custom", "input")]
    private void DataObject_SetData_InvokeStringObject_Unbounded_GetReturnsExpected(string format, string input)
    {
        DataObject dataObject = new();
        dataObject.SetData(format, input);

        dataObject.GetDataPresent(format).Should().BeTrue();
        dataObject.GetDataPresent(format, autoConvert: false).Should().BeTrue();
        dataObject.GetDataPresent(format, autoConvert: true).Should().BeTrue();

        dataObject.GetData(format).Should().Be(input);
        dataObject.GetData(format, autoConvert: false).Should().Be(input);
        dataObject.GetData(format, autoConvert: true).Should().Be(input);

        _ = dataObject.TryGetData(format, out object _).Should().Be(input is not null);
        _ = dataObject.TryGetData(format, autoConvert: false, out object _).Should().Be(input is not null);
        _ = dataObject.TryGetData(format, autoConvert: true, out object _).Should().Be(input is not null);
        _ = dataObject.TryGetData(format, NotSupportedResolver, autoConvert: true, out object _).Should().Be(input is not null);
        _ = dataObject.TryGetData(format, NotSupportedResolver, autoConvert: false, out object _).Should().Be(input is not null);

        dataObject.TryGetData(format, NotSupportedResolver, autoConvert: false, out string data).Should().Be(input is not null);
        data.Should().Be(input);
        dataObject.TryGetData(format, NotSupportedResolver, autoConvert: true, out data).Should().Be(input is not null);
        data.Should().Be(input);

        dataObject.ContainsAudio().Should().BeFalse();
        dataObject.ContainsFileDropList().Should().BeFalse();
        dataObject.ContainsImage().Should().BeFalse();
        dataObject.ContainsText().Should().BeFalse();
        dataObject.ContainsText(TextDataFormat.Text).Should().BeFalse();
        dataObject.ContainsText(TextDataFormat.UnicodeText).Should().BeFalse();
        dataObject.ContainsText(TextDataFormat.Rtf).Should().BeFalse();
        dataObject.ContainsText(TextDataFormat.Html).Should().BeFalse();
        dataObject.ContainsText(TextDataFormat.CommaSeparatedValue).Should().BeFalse();
    }

    [Fact]
    public void DataObject_SetData_InvokeStringObjectDibBitmapAutoConvert_GetDataReturnsExpected()
    {
        using Bitmap image = new(10, 10);
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.Dib, autoConvert: true, image);

        dataObject.GetImage().Should().BeSameAs(image);
        dataObject.GetData(DataFormats.Bitmap, autoConvert: true).Should().BeSameAs(image);
        dataObject.GetData(DataFormats.Bitmap, autoConvert: false).Should().BeSameAs(image);
        dataObject.GetData(typeof(Bitmap).FullName, autoConvert: true).Should().BeSameAs(image);
        dataObject.GetData(typeof(Bitmap).FullName, autoConvert: false).Should().BeNull();
        dataObject.GetData(DataFormats.Dib, autoConvert: true).Should().BeNull();
        dataObject.GetData(DataFormats.Dib, autoConvert: false).Should().BeNull();

        dataObject.ContainsImage().Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: false).Should().BeTrue();
        dataObject.GetDataPresent(typeof(Bitmap).FullName, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(typeof(Bitmap).FullName, autoConvert: false).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Dib, autoConvert: true).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Dib, autoConvert: false).Should().BeFalse();
    }

    public static TheoryData<string, object> SetData_StringObjectIDataObject_TheoryData()
    {
        TheoryData<string, object> theoryData = [];
        foreach (string format in new string[] { "format", "  ", string.Empty, null })
        {
            theoryData.Add(format, null);
            theoryData.Add(format, new());
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(SetData_StringObjectIDataObject_TheoryData))]
    public void DataObject_SetData_InvokeStringObjectIDataObject_CallsSetData(string format, object data)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(format, data))
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.SetData(format, data);
        mockDataObject.Verify(o => o.SetData(format, data), Times.Once());
    }

    public static TheoryData<string, bool, string, bool, bool> SetData_StringBoolObject_TheoryData()
    {
        TheoryData<string, bool, string, bool, bool> theoryData = new();

        foreach (string format in s_restrictedClipboardFormats)
        {
            if (string.IsNullOrWhiteSpace(format) || format == typeof(Bitmap).FullName || format.StartsWith("FileName", StringComparison.Ordinal))
            {
                continue;
            }

            foreach (bool autoConvert in new bool[] { true, false })
            {
                theoryData.Add(format, autoConvert, null, format == DataFormats.FileDrop, format == DataFormats.Bitmap);
                theoryData.Add(format, autoConvert, "input", format == DataFormats.FileDrop, format == DataFormats.Bitmap);
            }
        }

        theoryData.Add(typeof(Bitmap).FullName, false, null, false, false);
        theoryData.Add(typeof(Bitmap).FullName, false, "input", false, false);
        theoryData.Add(typeof(Bitmap).FullName, true, null, false, true);
        theoryData.Add(typeof(Bitmap).FullName, true, "input", false, true);

        theoryData.Add("FileName", false, null, false, false);
        theoryData.Add("FileName", false, "input", false, false);
        theoryData.Add("FileName", true, null, true, false);
        theoryData.Add("FileName", true, "input", true, false);

        theoryData.Add("FileNameW", false, null, false, false);
        theoryData.Add("FileNameW", false, "input", false, false);
        theoryData.Add("FileNameW", true, null, true, false);
        theoryData.Add("FileNameW", true, "input", true, false);

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(SetData_StringBoolObject_TheoryData))]
    private void DataObject_SetData_InvokeStringBoolObject_GetReturnsExpected(string format, bool autoConvert, string input, bool expectedContainsFileDropList, bool expectedContainsImage)
    {
        DataObject dataObject = new();
        dataObject.SetData(format, autoConvert, input);

        dataObject.GetData(format, autoConvert: false).Should().Be(input);
        dataObject.GetData(format, autoConvert: true).Should().Be(input);

        dataObject.GetDataPresent(format, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(format, autoConvert: false).Should().BeTrue();

        dataObject.ContainsAudio().Should().Be(format == DataFormats.WaveAudio);
        dataObject.ContainsFileDropList().Should().Be(expectedContainsFileDropList);
        dataObject.ContainsImage().Should().Be(expectedContainsImage);
        dataObject.ContainsText().Should().Be(format == DataFormats.UnicodeText);
        dataObject.ContainsText(TextDataFormat.Text).Should().Be(format == DataFormats.UnicodeText);
        dataObject.ContainsText(TextDataFormat.UnicodeText).Should().Be(format == DataFormats.UnicodeText);
        dataObject.ContainsText(TextDataFormat.Rtf).Should().Be(format == DataFormats.Rtf);
        dataObject.ContainsText(TextDataFormat.Html).Should().Be(format == DataFormats.Html);
        dataObject.ContainsText(TextDataFormat.CommaSeparatedValue).Should().Be(format == DataFormats.CommaSeparatedValue);
    }

    [Theory]
    [InlineData("something custom", false, "input")]
    [InlineData("something custom", false, null)]
    [InlineData("something custom", true, "input")]
    [InlineData("something custom", true, null)]
    [InlineData(DataFormatNames.Serializable, false, "input")]
    [InlineData(DataFormatNames.Serializable, false, null)]
    [InlineData(DataFormatNames.Serializable, true, "input")]
    [InlineData(DataFormatNames.Serializable, true, null)]
    private void DataObject_SetData_InvokeStringBoolObject_Unbounded(string format, bool autoConvert, string input)
    {
        DataObject dataObject = new();
        dataObject.SetData(format, autoConvert, input);

        dataObject.GetData(format, autoConvert: false).Should().Be(input);
        dataObject.GetData(format, autoConvert: true).Should().Be(input);

        dataObject.TryGetData(format, out string data).Should().Be(input is not null);
        data.Should().Be(input);

        dataObject.TryGetData(format, autoConvert: false, out data).Should().Be(input is not null);
        data.Should().Be(input);
        dataObject.TryGetData(format, autoConvert: true, out data).Should().Be(input is not null);
        data.Should().Be(input);

        dataObject.TryGetData(format, NotSupportedResolver, autoConvert: false, out data).Should().Be(input is not null);
        data.Should().Be(input);
        dataObject.TryGetData(format, NotSupportedResolver, autoConvert: true, out data).Should().Be(input is not null);
        data.Should().Be(input);

        dataObject.GetDataPresent(format, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(format, autoConvert: false).Should().BeTrue();

        dataObject.ContainsAudio().Should().Be(format == DataFormats.WaveAudio);
        dataObject.ContainsFileDropList().Should().BeFalse();
        dataObject.ContainsImage().Should().BeFalse();
        dataObject.ContainsText().Should().BeFalse();
        dataObject.ContainsText(TextDataFormat.Text).Should().BeFalse();
        dataObject.ContainsText(TextDataFormat.UnicodeText).Should().BeFalse();
        dataObject.ContainsText(TextDataFormat.Rtf).Should().BeFalse();
        dataObject.ContainsText(TextDataFormat.Html).Should().BeFalse();
        dataObject.ContainsText(TextDataFormat.CommaSeparatedValue).Should().BeFalse();
    }

    [Fact]
    public void DataObject_SetData_InvokeStringBoolObjectDibBitmapAutoConvert_GetDataReturnsExpected()
    {
        using Bitmap image = new(10, 10);
        DataObject dataObject = new();
        dataObject.SetData(DataFormats.Dib, autoConvert: true, image);

        dataObject.GetImage().Should().BeSameAs(image);
        dataObject.GetData(DataFormats.Bitmap, autoConvert: true).Should().BeSameAs(image);
        dataObject.GetData(DataFormats.Bitmap, autoConvert: false).Should().BeSameAs(image);
        dataObject.GetData(typeof(Bitmap).FullName, autoConvert: true).Should().BeSameAs(image);
        dataObject.GetData(typeof(Bitmap).FullName, autoConvert: false).Should().BeNull();
        dataObject.GetData(DataFormats.Dib, autoConvert: true).Should().BeNull();
        dataObject.GetData(DataFormats.Dib, autoConvert: false).Should().BeNull();

        dataObject.ContainsImage().Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: false).Should().BeTrue();
        dataObject.GetDataPresent(typeof(Bitmap).FullName, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(typeof(Bitmap).FullName, autoConvert: false).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Dib, autoConvert: true).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Dib, autoConvert: false).Should().BeFalse();
    }

    public static TheoryData<string, bool, object> SetData_StringBoolObjectIDataObject_TheoryData()
    {
        TheoryData<string, bool, object> theoryData = [];
        foreach (string format in new string[] { "format", "  ", string.Empty, null })
        {
            foreach (bool autoConvert in new bool[] { true, false })
            {
                theoryData.Add(format, autoConvert, null);
                theoryData.Add(format, autoConvert, new());
            }
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(SetData_StringBoolObjectIDataObject_TheoryData))]
    public void DataObject_SetData_InvokeStringBoolObjectIDataObject_CallsSetData(string format, bool autoConvert, object data)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(format, autoConvert, data))
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.SetData(format, autoConvert, data);
        mockDataObject.Verify(o => o.SetData(format, autoConvert, data), Times.Once());
    }

    public static TheoryData<Type, object> SetData_TypeObjectIDataObject_TheoryData()
    {
        TheoryData<Type, object> theoryData = [];
        foreach (Type format in new Type[] { typeof(int), null })
        {
            theoryData.Add(format, null);
            theoryData.Add(format, new());
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(SetData_TypeObjectIDataObject_TheoryData))]
    public void DataObject_SetData_InvokeTypeObjectIDataObject_CallsSetData(Type format, object data)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(format, data))
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.SetData(format, data);
        mockDataObject.Verify(o => o.SetData(format, data), Times.Once());
    }

    [Fact]
    public void DataObject_SetData_NullFormat_ThrowsArgumentNullException()
    {
        DataObject dataObject = new();
        ((Action)(() => dataObject.SetData((string)null, new object()))).Should()
            .Throw<ArgumentNullException>().WithParameterName("format");
        ((Action)(() => dataObject.SetData(null, true, new object()))).Should()
            .Throw<ArgumentNullException>().WithParameterName("format");
        ((Action)(() => dataObject.SetData(null, false, new object()))).Should()
            .Throw<ArgumentNullException>().WithParameterName("format");
        ((Action)(() => dataObject.SetData((Type)null, new object()))).Should()
            .Throw<ArgumentNullException>().WithParameterName("format");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void DataObject_SetData_WhitespaceOrEmptyFormat_ThrowsArgumentException(string format)
    {
        DataObject dataObject = new();
        Action action = () => dataObject.SetData(format, new object());
        action.Should().Throw<ArgumentException>().WithParameterName("format");

        action = () => dataObject.SetData(format, true, new object());
        action.Should().Throw<ArgumentException>().WithParameterName("format");

        action = () => dataObject.SetData(format, false, new object());
        action.Should().Throw<ArgumentException>().WithParameterName("format");
    }

    [Fact]
    public void DataObject_SetData_DibBitmapNoAutoConvert_ThrowsNotSupportedException()
    {
        DataObject dataObject = new();
        ((Action)(() => dataObject.SetData(DataFormats.Dib, false, new Bitmap(10, 10))))
            .Should().Throw<NotSupportedException>();
    }

    public static TheoryData<StringCollection> SetFileDropList_TheoryData() => new()
    {
        { new StringCollection() },
        { new StringCollection { "file", "  ", string.Empty, null } }
    };

    [Theory]
    [MemberData(nameof(SetFileDropList_TheoryData))]
    public void DataObject_SetFileDropList_Invoke_GetReturnsExpected(StringCollection filePaths)
    {
        DataObject dataObject = new();
        dataObject.SetFileDropList(filePaths);

        dataObject.GetFileDropList().Should().BeEquivalentTo(filePaths);
        dataObject.GetData(DataFormats.FileDrop, autoConvert: true).Should().BeEquivalentTo(filePaths.Cast<string>());
        dataObject.GetData(DataFormats.FileDrop, autoConvert: false).Should().BeEquivalentTo(filePaths.Cast<string>());
        dataObject.GetData("FileName", autoConvert: true).Should().BeEquivalentTo(filePaths.Cast<string>());
        dataObject.GetData("FileName", autoConvert: false).Should().BeNull();
        dataObject.GetData("FileNameW", autoConvert: true).Should().BeEquivalentTo(filePaths.Cast<string>());
        dataObject.GetData("FileNameW", autoConvert: false).Should().BeNull();

        dataObject.ContainsFileDropList().Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.FileDrop, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.FileDrop, autoConvert: false).Should().BeTrue();
        dataObject.GetDataPresent("FileName", autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent("FileName", autoConvert: false).Should().BeFalse();
        dataObject.GetDataPresent("FileNameW", autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent("FileNameW", autoConvert: false).Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(SetFileDropList_TheoryData))]
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
    [MemberData(nameof(SetFileDropList_TheoryData))]
    public void DataObject_SetFileDropList_InvokeIDataObject_CallsSetData(StringCollection filePaths)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(DataFormats.FileDrop, true, It.IsAny<string[]>()))
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.SetFileDropList(filePaths);
        mockDataObject.Verify(o => o.SetData(DataFormats.FileDrop, true, It.IsAny<string[]>()), Times.Once());
    }

    [Fact]
    public void DataObject_SetFileDropList_NullFilePaths_ThrowsArgumentNullException()
    {
        DataObject dataObject = new();
        Action action = () => dataObject.SetFileDropList(null);
        action.Should().Throw<ArgumentNullException>().WithParameterName("filePaths");
    }

    [Fact]
    public void DataObject_SetImage_Invoke_GetReturnsExpected()
    {
        using Bitmap image = new(10, 10);
        DataObject dataObject = new();
        dataObject.SetImage(image);

        dataObject.GetImage().Should().BeSameAs(image);
        dataObject.GetData(DataFormats.Bitmap, autoConvert: true).Should().BeSameAs(image);
        dataObject.GetData(DataFormats.Bitmap, autoConvert: false).Should().BeSameAs(image);
        dataObject.GetData(typeof(Bitmap).FullName, autoConvert: true).Should().BeSameAs(image);
        dataObject.GetData(typeof(Bitmap).FullName, autoConvert: false).Should().BeNull();
        dataObject.GetData(DataFormats.Dib, autoConvert: true).Should().BeNull();
        dataObject.GetData(DataFormats.Dib, autoConvert: false).Should().BeNull();

        dataObject.ContainsImage().Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.Bitmap, autoConvert: false).Should().BeTrue();
        dataObject.GetDataPresent(typeof(Bitmap).FullName, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(typeof(Bitmap).FullName, autoConvert: false).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Dib, autoConvert: true).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Dib, autoConvert: false).Should().BeFalse();
    }

    [Fact]
    public void DataObject_SetImage_InvokeMocked_CallsSetImage()
    {
        using Bitmap image = new(10, 10);

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

    [Fact]
    public void DataObject_SetImage_InvokeIDataObject_CallsSetData()
    {
        using Bitmap image = new(10, 10);

        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(DataFormats.Bitmap, true, image))
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.SetImage(image);
        mockDataObject.Verify(o => o.SetData(DataFormats.Bitmap, true, image), Times.Once());
    }

    [Fact]
    public void DataObject_SetImage_NullImage_ThrowsArgumentNullException()
    {
        DataObject dataObject = new();
        Action action = () => dataObject.SetImage(null);
        action.Should().Throw<ArgumentNullException>().WithParameterName("image");
    }

    public static TheoryData<string> SetText_String_TheoryData() => new()
    {
        { "  " },
        { "textData" }
    };

    [Theory]
    [MemberData(nameof(SetText_String_TheoryData))]
    public void DataObject_SetText_InvokeString_GetReturnsExpected(string textData)
    {
        DataObject dataObject = new();
        dataObject.SetText(textData);

        dataObject.GetText().Should().BeSameAs(textData);
        dataObject.GetData(DataFormats.UnicodeText, autoConvert: true).Should().BeSameAs(textData);
        dataObject.GetData(DataFormats.UnicodeText, autoConvert: false).Should().BeSameAs(textData);
        dataObject.GetData(DataFormats.Text, autoConvert: true).Should().BeSameAs(textData);
        dataObject.GetData(DataFormats.Text, autoConvert: false).Should().BeNull();
        dataObject.GetData(DataFormats.StringFormat, autoConvert: true).Should().BeSameAs(textData);
        dataObject.GetData(DataFormats.StringFormat, autoConvert: false).Should().BeNull();
        dataObject.GetData(DataFormats.Rtf, autoConvert: true).Should().BeNull();
        dataObject.GetData(DataFormats.Rtf, autoConvert: false).Should().BeNull();
        dataObject.GetData(DataFormats.Html, autoConvert: true).Should().BeNull();
        dataObject.GetData(DataFormats.Html, autoConvert: false).Should().BeNull();
        dataObject.GetData(DataFormats.CommaSeparatedValue, autoConvert: true).Should().BeNull();
        dataObject.GetData(DataFormats.CommaSeparatedValue, autoConvert: false).Should().BeNull();

        dataObject.ContainsText().Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: true).Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: false).Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.Text, autoConvert: true).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Text, autoConvert: false).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.StringFormat, autoConvert: true).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.StringFormat, autoConvert: false).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Rtf, autoConvert: true).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Rtf, autoConvert: false).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Html, autoConvert: true).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Html, autoConvert: false).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.CommaSeparatedValue, autoConvert: true).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.CommaSeparatedValue, autoConvert: false).Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(SetText_String_TheoryData))]
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
    [MemberData(nameof(SetText_String_TheoryData))]
    public void DataObject_SetText_InvokeStringIDataObject_CallsSetData(string textData)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(DataFormats.UnicodeText, false, textData))
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.SetText(textData);
        mockDataObject.Verify(o => o.SetData(DataFormats.UnicodeText, false, textData), Times.Once());
    }

    public static TheoryData<string, TextDataFormat, string, string, string, string> SetText_StringTextDataFormat_TheoryData()
    {
        TheoryData<string, TextDataFormat, string, string, string, string> theoryData = [];
        foreach (string textData in new string[] { "textData", "  " })
        {
            theoryData.Add(textData, TextDataFormat.Text, textData, null, null, null);
            theoryData.Add(textData, TextDataFormat.UnicodeText, textData, null, null, null);
            theoryData.Add(textData, TextDataFormat.Rtf, null, textData, null, null);
            theoryData.Add(textData, TextDataFormat.Html, null, null, textData, null);
            theoryData.Add(textData, TextDataFormat.CommaSeparatedValue, null, null, null, textData);
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(SetText_StringTextDataFormat_TheoryData))]
    public void DataObject_SetText_InvokeStringTextDataFormat_GetReturnsExpected(string textData, TextDataFormat format, string expectedUnicodeText, string expectedRtfText, string expectedHtmlText, string expectedCsvText)
    {
        DataObject dataObject = new();
        dataObject.SetText(textData, format);

        dataObject.GetText(format).Should().Be(textData);
        dataObject.GetData(DataFormats.UnicodeText, autoConvert: true).Should().Be(expectedUnicodeText);
        dataObject.GetData(DataFormats.UnicodeText, autoConvert: false).Should().Be(expectedUnicodeText);
        dataObject.GetData(DataFormats.Text, autoConvert: true).Should().Be(expectedUnicodeText);
        dataObject.GetData(DataFormats.Text, autoConvert: false).Should().BeNull();
        dataObject.GetData(DataFormats.StringFormat, autoConvert: true).Should().Be(expectedUnicodeText);
        dataObject.GetData(DataFormats.StringFormat, autoConvert: false).Should().BeNull();
        dataObject.GetData(DataFormats.Rtf, autoConvert: true).Should().Be(expectedRtfText);
        dataObject.GetData(DataFormats.Rtf, autoConvert: false).Should().Be(expectedRtfText);
        dataObject.GetData(DataFormats.Html, autoConvert: true).Should().Be(expectedHtmlText);
        dataObject.GetData(DataFormats.Html, autoConvert: false).Should().Be(expectedHtmlText);
        dataObject.GetData(DataFormats.CommaSeparatedValue, autoConvert: true).Should().Be(expectedCsvText);
        dataObject.GetData(DataFormats.CommaSeparatedValue, autoConvert: false).Should().Be(expectedCsvText);

        dataObject.ContainsText(format).Should().BeTrue();
        dataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: true).Should().Be(expectedUnicodeText is not null);
        dataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: false).Should().Be(expectedUnicodeText is not null);
        dataObject.GetDataPresent(DataFormats.Text, autoConvert: true).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Text, autoConvert: false).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.StringFormat, autoConvert: true).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.StringFormat, autoConvert: false).Should().BeFalse();
        dataObject.GetDataPresent(DataFormats.Rtf, autoConvert: true).Should().Be(expectedRtfText is not null);
        dataObject.GetDataPresent(DataFormats.Rtf, autoConvert: false).Should().Be(expectedRtfText is not null);
        dataObject.GetDataPresent(DataFormats.Html, autoConvert: true).Should().Be(expectedHtmlText is not null);
        dataObject.GetDataPresent(DataFormats.Html, autoConvert: false).Should().Be(expectedHtmlText is not null);
        dataObject.GetDataPresent(DataFormats.CommaSeparatedValue, autoConvert: true).Should().Be(expectedCsvText is not null);
        dataObject.GetDataPresent(DataFormats.CommaSeparatedValue, autoConvert: false).Should().Be(expectedCsvText is not null);
    }

    public static TheoryData<string, TextDataFormat, string> SetText_StringTextDataFormatMocked_TheoryData()
    {
        TheoryData<string, TextDataFormat, string> theoryData = [];
        foreach (string textData in new string[] { "textData", "  " })
        {
            theoryData.Add(textData, TextDataFormat.Text, DataFormats.UnicodeText);
            theoryData.Add(textData, TextDataFormat.UnicodeText, DataFormats.UnicodeText);
            theoryData.Add(textData, TextDataFormat.Rtf, DataFormats.Rtf);
            theoryData.Add(textData, TextDataFormat.Html, DataFormats.Html);
            theoryData.Add(textData, TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue);
        }

        return theoryData;
    }

    [Theory]
    [MemberData(nameof(SetText_StringTextDataFormatMocked_TheoryData))]
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
    [MemberData(nameof(SetText_StringTextDataFormatMocked_TheoryData))]
    public void DataObject_SetText_InvokeStringTextDataFormatIDataObject_CallsSetData(string textData, TextDataFormat format, string expectedFormat)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.SetData(expectedFormat, false, textData))
            .Verifiable();
        DataObject dataObject = new(mockDataObject.Object);
        dataObject.SetText(textData, format);
        mockDataObject.Verify(o => o.SetData(expectedFormat, false, textData), Times.Once());
    }

    [Theory]
    [NullAndEmptyStringData]
    public void DataObject_SetText_NullOrEmptyTextData_ThrowsArgumentNullException(string textData)
    {
        DataObject dataObject = new();
        ((Action)(() => dataObject.SetText(textData))).Should()
            .Throw<ArgumentNullException>().WithParameterName("textData");
        ((Action)(() => dataObject.SetText(textData, TextDataFormat.Text))).Should()
            .Throw<ArgumentNullException>().WithParameterName("textData");
    }

    [Theory]
    [InvalidEnumData<TextDataFormat>]
    public void DataObject_SetText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
    {
        DataObject dataObject = new();
        ((Action)(() => dataObject.SetText("text", format))).Should()
            .Throw<InvalidEnumArgumentException>().WithParameterName("format");
    }

    [WinFormsFact]
    public void DataObject_GetData_EnhancedMetafile_DoesNotTerminateProcess()
    {
        DataObject data = new(new DataObjectIgnoringStorageMediumForEnhancedMetafile());

        // Office ignores the storage medium in GetData(EnhancedMetafile) and always returns a handle,
        // even when asked for a stream. This used to crash the process when DataObject interpreted the
        // handle as a pointer to a COM IStream without checking the storage medium it retrieved.

        data.GetData(DataFormats.EnhancedMetafile).Should().BeNull();
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
            // do not check the requested storage medium, we always return a metafile handle, that's what Office does

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

    public static TheoryData<ADVF, IAdviseSink> DAdvise_TheoryData()
    {
        TheoryData<ADVF, IAdviseSink> theoryData = new()
        {
            { ADVF.ADVF_DATAONSTOP, null }
        };

        Mock<IAdviseSink> mockAdviseSink = new(MockBehavior.Strict);
        theoryData.Add(ADVF.ADVF_DATAONSTOP, mockAdviseSink.Object);

        return theoryData;
    }

    [WinFormsTheory]
    [MemberData(nameof(DAdvise_TheoryData))]
    public void DataObject_DAdvise_InvokeDefault_Success(ADVF advf, IAdviseSink adviseSink)
    {
        DataObject dataObject = new();
        IComDataObject comDataObject = dataObject;
        FORMATETC formatetc = default;
        ((HRESULT)comDataObject.DAdvise(ref formatetc, advf, adviseSink, out int connection)).Should().Be(HRESULT.E_NOTIMPL);
        connection.Should().Be(0);
    }

    private delegate void DAdviseCallback(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection);

    [WinFormsTheory]
    [MemberData(nameof(DAdvise_TheoryData))]
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
        FORMATETC formatetc = default;
        comDataObject.DAdvise(ref formatetc, advf, adviseSink, out int connection).Should().Be(1);
        connection.Should().Be(2);
        formatetc.cfFormat.Should().Be(3);
        mockComDataObject.Verify(o => o.DAdvise(ref It.Ref<FORMATETC>.IsAny, advf, adviseSink, out It.Ref<int>.IsAny), Times.Once());
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    public void DataObject_DUnadvise_InvokeDefault_Success(int connection)
    {
        DataObject dataObject = new();
        IComDataObject comDataObject = dataObject;
        Action action = () => comDataObject.DUnadvise(connection);
        action.Should().Throw<NotImplementedException>();
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
        ((HRESULT)comDataObject.EnumDAdvise(out IEnumSTATDATA enumAdvise)).Should().Be(HRESULT.OLE_E_ADVISENOTSUPPORTED);
        enumAdvise.Should().BeNull();
    }

    private delegate void EnumDAdviseCallback(out IEnumSTATDATA enumAdvise);

    public static TheoryData<IEnumSTATDATA> EnumDAdvise_CustomComDataObject_TheoryData()
    {
        TheoryData<IEnumSTATDATA> theoryData = [null];

        Mock<IEnumSTATDATA> mockEnumStatData = new(MockBehavior.Strict);
        theoryData.Add(mockEnumStatData.Object);

        return theoryData;
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumDAdvise_CustomComDataObject_TheoryData))]
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
        comDataObject.EnumDAdvise(out IEnumSTATDATA enumStatData).Should().Be(1);
        enumStatData.Should().BeSameAs(result);
        mockComDataObject.Verify(o => o.EnumDAdvise(out It.Ref<IEnumSTATDATA>.IsAny), Times.Once());
    }

    public static TheoryData<int> EnumFormatEtc_Default_TheoryData() => new()
    {
        { -1 },
        { 0 },
        { 1 },
        { 2 }
    };

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_Default_TheoryData))]
    public void DataObject_EnumFormatEtc_InvokeDefault_Success(int celt)
    {
        DataObject dataObject = new();
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC enumerator = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        enumerator.Should().NotBeNull();

        var result = new FORMATETC[1];
        int[] fetched = new int[1];

        for (int i = 0; i < 2; i++)
        {
            enumerator.Next(celt, result, fetched).Should().Be((int)HRESULT.S_FALSE);
            result[0].cfFormat.Should().Be(0);
            fetched[0].Should().Be(0);

            enumerator.Next(celt, null, null).Should().Be((int)HRESULT.S_FALSE);

            enumerator.Reset().Should().Be((int)HRESULT.S_OK);
        }
    }

    public static TheoryData<string, TYMED> EnumFormatEtc_TheoryData() => new()
    {
        { DataFormats.Bitmap, TYMED.TYMED_GDI },
        { DataFormats.CommaSeparatedValue, TYMED.TYMED_HGLOBAL },
        { DataFormats.Dib, TYMED.TYMED_HGLOBAL },
        { DataFormats.Dif, TYMED.TYMED_HGLOBAL },
        { DataFormats.EnhancedMetafile, TYMED.TYMED_HGLOBAL },
        { DataFormats.FileDrop, TYMED.TYMED_HGLOBAL },
        { "FileName", TYMED.TYMED_HGLOBAL },
        { "FileNameW", TYMED.TYMED_HGLOBAL },
        { DataFormats.Html, TYMED.TYMED_HGLOBAL },
        { DataFormats.Locale, TYMED.TYMED_HGLOBAL },
        { DataFormats.MetafilePict, TYMED.TYMED_HGLOBAL },
        { DataFormats.OemText, TYMED.TYMED_HGLOBAL },
        { DataFormats.Palette, TYMED.TYMED_HGLOBAL },
        { DataFormats.PenData, TYMED.TYMED_HGLOBAL },
        { DataFormats.Riff, TYMED.TYMED_HGLOBAL },
        { DataFormats.Rtf, TYMED.TYMED_HGLOBAL },
        { DataFormats.Serializable, TYMED.TYMED_HGLOBAL },
        { DataFormats.StringFormat, TYMED.TYMED_HGLOBAL },
        { DataFormats.SymbolicLink, TYMED.TYMED_HGLOBAL },
        { DataFormats.Text, TYMED.TYMED_HGLOBAL },
        { DataFormats.Tiff, TYMED.TYMED_HGLOBAL },
        { DataFormats.UnicodeText, TYMED.TYMED_HGLOBAL },
        { DataFormats.WaveAudio, TYMED.TYMED_HGLOBAL }
    };

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_TheoryData))]
    public void DataObject_EnumFormatEtc_InvokeWithValues_Success(string format1, TYMED expectedTymed)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats())
            .Returns([format1, "Format2"]);
        DataObject dataObject = new(mockDataObject.Object);
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC enumerator = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        enumerator.Should().NotBeNull();

        var result = new FORMATETC[2];
        int[] fetched = new int[2];

        for (int i = 0; i < 1; i++)
        {
            // Fetch nothing.
            ((HRESULT)enumerator.Next(0, result, fetched)).Should().Be(HRESULT.S_FALSE);
            fetched[0].Should().Be(0);

            ((HRESULT)enumerator.Next(0, null, null)).Should().Be(HRESULT.S_FALSE);

            // Fetch negative.
            ((HRESULT)enumerator.Next(-1, result, fetched)).Should().Be(HRESULT.S_FALSE);
            fetched[0].Should().Be(0);

            ((HRESULT)enumerator.Next(-1, null, null)).Should().Be(HRESULT.S_FALSE);

            // Null.
            ((Action)(() => enumerator.Next(1, null, fetched))).Should().Throw<NullReferenceException>();

            // Fetch first.
            ((HRESULT)enumerator.Next(i + 1, result, fetched)).Should().Be(HRESULT.S_OK);
            result[0].cfFormat.Should().Be(unchecked((short)(ushort)(DataFormats.GetFormat(format1).Id)));
            result[0].dwAspect.Should().Be(DVASPECT.DVASPECT_CONTENT);
            result[0].lindex.Should().Be(-1);
            result[0].ptd.Should().Be(IntPtr.Zero);
            result[0].tymed.Should().Be(expectedTymed);
            result[1].cfFormat.Should().Be(0);
            fetched[0].Should().Be(1);

            // Fetch second.
            ((HRESULT)enumerator.Next(i + 1, result, fetched)).Should().Be(HRESULT.S_OK);
            result[0].cfFormat.Should().NotBe(0);
            result[0].dwAspect.Should().Be(DVASPECT.DVASPECT_CONTENT);
            result[0].lindex.Should().Be(-1);
            result[0].ptd.Should().Be(IntPtr.Zero);
            result[0].tymed.Should().Be(TYMED.TYMED_HGLOBAL);
            result[1].cfFormat.Should().Be(0);
            fetched[0].Should().Be(1);

            // Fetch another.
            ((HRESULT)enumerator.Next(1, null, null)).Should().Be(HRESULT.S_FALSE);
            ((HRESULT)enumerator.Next(1, null, null)).Should().Be(HRESULT.S_FALSE);

            // Reset.
            ((HRESULT)enumerator.Reset()).Should().Be(HRESULT.S_OK);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_Default_TheoryData))]
    public void DataObject_EnumFormatEtc_InvokeNullFormats_Success(int celt)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats())
            .Returns((string[])null);
        DataObject dataObject = new(mockDataObject.Object);
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC enumerator = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        enumerator.Should().NotBeNull();

        var result = new FORMATETC[1];
        int[] fetched = new int[1];

        for (int i = 0; i < 2; i++)
        {
            ((HRESULT)enumerator.Next(celt, result, fetched)).Should().Be(HRESULT.S_FALSE);
            result[0].cfFormat.Should().Be(0);
            fetched[0].Should().Be(0);

            ((HRESULT)enumerator.Next(celt, null, null)).Should().Be(HRESULT.S_FALSE);

            ((HRESULT)enumerator.Reset()).Should().Be(HRESULT.S_OK);
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
        enumerator.Should().NotBeNull();

        var result = new FORMATETC[1];
        int[] fetched = new int[1];
        ((HRESULT)enumerator.Skip(celt)).Should().Be(HRESULT.S_FALSE);
        ((HRESULT)enumerator.Next(1, result, fetched)).Should().Be(HRESULT.S_FALSE);

        // Negative.
        ((HRESULT)enumerator.Skip(-1)).Should().Be(HRESULT.S_OK);
        Action action = () => enumerator.Next(1, result, fetched);
        action.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("index");
    }

    [WinFormsFact]
    public void DataObject_EnumFormatEtc_SkipCustom_Success()
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats())
            .Returns(["Format1", DataFormats.Bitmap, "Format2"]);
        DataObject dataObject = new(mockDataObject.Object);
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC enumerator = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        enumerator.Should().NotBeNull();

        var result = new FORMATETC[1];
        int[] fetched = new int[1];

        ((HRESULT)enumerator.Skip(1)).Should().Be(HRESULT.S_OK);
        ((HRESULT)enumerator.Next(1, result, fetched)).Should().Be(HRESULT.S_OK);
        result[0].cfFormat.Should().Be(2);
        result[0].dwAspect.Should().Be(DVASPECT.DVASPECT_CONTENT);
        result[0].lindex.Should().Be(-1);
        result[0].ptd.Should().Be(IntPtr.Zero);
        result[0].tymed.Should().Be(TYMED.TYMED_GDI);
        fetched[0].Should().Be(1);

        // Skip negative.
        ((HRESULT)enumerator.Skip(-2)).Should().Be(HRESULT.S_OK);
        ((HRESULT)enumerator.Next(1, result, fetched)).Should().Be(HRESULT.S_OK);
        result[0].cfFormat.Should().Be(unchecked((short)(ushort)(DataFormats.GetFormat("Format1").Id)));
        result[0].dwAspect.Should().Be(DVASPECT.DVASPECT_CONTENT);
        result[0].lindex.Should().Be(-1);
        result[0].ptd.Should().Be(IntPtr.Zero);
        result[0].tymed.Should().Be(TYMED.TYMED_HGLOBAL);
        fetched[0].Should().Be(1);

        // Skip end.
        ((HRESULT)enumerator.Skip(1)).Should().Be(HRESULT.S_OK);
        ((HRESULT)enumerator.Next(1, result, fetched)).Should().Be(HRESULT.S_OK);
        result[0].cfFormat.Should().Be(unchecked((short)(ushort)(DataFormats.GetFormat("Format2").Id)));
        result[0].dwAspect.Should().Be(DVASPECT.DVASPECT_CONTENT);
        result[0].lindex.Should().Be(-1);
        result[0].ptd.Should().Be(IntPtr.Zero);
        result[0].tymed.Should().Be(TYMED.TYMED_HGLOBAL);
        fetched[0].Should().Be(1);

        ((HRESULT)enumerator.Skip(0)).Should().Be(HRESULT.S_FALSE);
        ((HRESULT)enumerator.Skip(1)).Should().Be(HRESULT.S_FALSE);

        // Negative.
        ((HRESULT)enumerator.Skip(-4)).Should().Be(HRESULT.S_OK);
        Action action = () => enumerator.Next(1, result, fetched);
        action.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("index");
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_Default_TheoryData))]
    public void DataObject_EnumFormatEtc_CloneDefault_Success(int celt)
    {
        DataObject dataObject = new();
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC source = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        source.Clone(out IEnumFORMATETC enumerator);
        enumerator.Should().NotBeNull();

        var result = new FORMATETC[1];
        int[] fetched = new int[1];

        for (int i = 0; i < 2; i++)
        {
            ((HRESULT)enumerator.Next(celt, result, fetched)).Should().Be(HRESULT.S_FALSE);
            result[0].cfFormat.Should().Be(0);
            fetched[0].Should().Be(0);

            ((HRESULT)enumerator.Next(celt, null, null)).Should().Be(HRESULT.S_FALSE);

            ((HRESULT)enumerator.Reset()).Should().Be(HRESULT.S_OK);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_TheoryData))]
    public void DataObject_EnumFormatEtc_CloneWithValues_Success(string format1, TYMED expectedTymed)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats())
            .Returns([format1, "Format2"]);
        DataObject dataObject = new(mockDataObject.Object);
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC source = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        source.Clone(out IEnumFORMATETC enumerator);
        enumerator.Should().NotBeNull();

        var result = new FORMATETC[2];
        int[] fetched = new int[2];

        for (int i = 0; i < 1; i++)
        {
            // Fetch nothing.
            ((HRESULT)enumerator.Next(0, result, fetched)).Should().Be(HRESULT.S_FALSE);
            fetched[0].Should().Be(0);

            ((HRESULT)enumerator.Next(0, null, null)).Should().Be(HRESULT.S_FALSE);

            // Fetch negative.
            ((HRESULT)enumerator.Next(-1, result, fetched)).Should().Be(HRESULT.S_FALSE);
            fetched[0].Should().Be(0);

            ((HRESULT)enumerator.Next(-1, null, null)).Should().Be(HRESULT.S_FALSE);

            // Null.
            ((Action)(() => enumerator.Next(1, null, fetched))).Should().Throw<NullReferenceException>();

            // Fetch first.
            ((HRESULT)enumerator.Next(i + 1, result, fetched)).Should().Be(HRESULT.S_OK);
            result[0].cfFormat.Should().Be(unchecked((short)(ushort)(DataFormats.GetFormat(format1).Id)));
            result[0].dwAspect.Should().Be(DVASPECT.DVASPECT_CONTENT);
            result[0].lindex.Should().Be(-1);
            result[0].ptd.Should().Be(IntPtr.Zero);
            result[0].tymed.Should().Be(expectedTymed);
            result[1].cfFormat.Should().Be(0);
            fetched[0].Should().Be(1);

            // Fetch second.
            ((HRESULT)enumerator.Next(i + 1, result, fetched)).Should().Be(HRESULT.S_OK);
            result[0].cfFormat.Should().NotBe(0);
            result[0].dwAspect.Should().Be(DVASPECT.DVASPECT_CONTENT);
            result[0].lindex.Should().Be(-1);
            result[0].ptd.Should().Be(IntPtr.Zero);
            result[0].tymed.Should().Be(TYMED.TYMED_HGLOBAL);
            result[1].cfFormat.Should().Be(0);
            fetched[0].Should().Be(1);

            // Fetch another.
            ((HRESULT)enumerator.Next(1, null, null)).Should().Be(HRESULT.S_FALSE);
            ((HRESULT)enumerator.Next(1, null, null)).Should().Be(HRESULT.S_FALSE);

            // Reset.
            ((HRESULT)enumerator.Reset()).Should().Be(HRESULT.S_OK);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_Default_TheoryData))]
    public void DataObject_EnumFormatEtc_CloneNullFormats_Success(int celt)
    {
        Mock<IDataObject> mockDataObject = new(MockBehavior.Strict);
        mockDataObject
            .Setup(o => o.GetFormats())
            .Returns((string[])null);
        DataObject dataObject = new(mockDataObject.Object);
        IComDataObject comDataObject = dataObject;
        IEnumFORMATETC source = comDataObject.EnumFormatEtc(DATADIR.DATADIR_GET);
        source.Clone(out IEnumFORMATETC enumerator);
        enumerator.Should().NotBeNull();

        var result = new FORMATETC[1];
        int[] fetched = new int[1];

        for (int i = 0; i < 2; i++)
        {
            ((HRESULT)enumerator.Next(celt, result, fetched)).Should().Be(HRESULT.S_FALSE);
            result[0].cfFormat.Should().Be(0);
            fetched[0].Should().Be(0);

            ((HRESULT)enumerator.Next(celt, null, null)).Should().Be(HRESULT.S_FALSE);

            ((HRESULT)enumerator.Reset()).Should().Be(HRESULT.S_OK);
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
        ((Action)(() => comDataObject.EnumFormatEtc(dwDirection))).Should().Throw<ExternalException>();
    }

    public static TheoryData<DATADIR, IEnumFORMATETC> EnumFormatEtc_CustomComDataObject_TheoryData()
    {
        TheoryData<DATADIR, IEnumFORMATETC> theoryData = [];
        Mock<IEnumFORMATETC> mockEnumFormatEtc = new(MockBehavior.Strict);

        theoryData.Add(DATADIR.DATADIR_GET, mockEnumFormatEtc.Object);
        theoryData.Add(DATADIR.DATADIR_SET, mockEnumFormatEtc.Object);
        theoryData.Add(DATADIR.DATADIR_GET - 1, mockEnumFormatEtc.Object);
        theoryData.Add(DATADIR.DATADIR_SET + 1, mockEnumFormatEtc.Object);

        return theoryData;
    }

    [WinFormsTheory]
    [MemberData(nameof(EnumFormatEtc_CustomComDataObject_TheoryData))]
    public void DataObject_EnumFormatEtc_InvokeCustomComDataObject_Success(DATADIR dwDirection, IEnumFORMATETC result)
    {
        Mock<IComDataObject> mockComDataObject = new(MockBehavior.Strict);
        mockComDataObject
            .Setup(o => o.EnumFormatEtc(dwDirection))
            .Returns(result)
            .Verifiable();
        DataObject dataObject = new(mockComDataObject.Object);
        IComDataObject comDataObject = dataObject;
        comDataObject.EnumFormatEtc(dwDirection).Should().BeSameAs(result);
        mockComDataObject.Verify(o => o.EnumFormatEtc(dwDirection), Times.Once());
    }

    public static TheoryData<TextDataFormat, short> GetDataHere_Text_TheoryData() => new()
    {
        { TextDataFormat.Rtf, (short)DataFormats.GetFormat(DataFormats.Rtf).Id  },
        { TextDataFormat.Html, (short)DataFormats.GetFormat(DataFormats.Html).Id }
    };

    [WinFormsTheory]
    [MemberData(nameof(GetDataHere_Text_TheoryData))]
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
            new string(pChar).Should().Be("text");
        }
        finally
        {
            PInvokeCore.GlobalFree(handle);
        }
    }

    public static TheoryData<TextDataFormat, short> GetDataHere_UnicodeText_TheoryData() => new()
    {
        { TextDataFormat.Text, (short)CLIPBOARD_FORMAT.CF_UNICODETEXT },
        { TextDataFormat.UnicodeText, (short)CLIPBOARD_FORMAT.CF_UNICODETEXT }
    };

    [WinFormsTheory]
    [MemberData(nameof(GetDataHere_UnicodeText_TheoryData))]
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
            new string(pChar).Should().Be("text");
        }
        finally
        {
            PInvokeCore.GlobalFree(handle);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(GetDataHere_Text_TheoryData))]
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

        ((Action)(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium))).Should().Throw<ArgumentException>();
    }

    [WinFormsTheory]
    [MemberData(nameof(GetDataHere_UnicodeText_TheoryData))]
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

        ((Action)(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium))).Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public unsafe void IComDataObjectGetDataHere_FileNames_Success()
    {
        DataObject dataObject = new();
        dataObject.SetFileDropList(["Path1", "Path2"]);
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
            pDropFiles->pFiles.Should().Be(20u);
            pDropFiles->pt.Should().Be(Point.Empty);
            pDropFiles->fNC.Should().Be(BOOL.FALSE);
            pDropFiles->fWide.Should().Be(BOOL.TRUE);
            char* text = (char*)IntPtr.Add((IntPtr)pDropFiles, (int)pDropFiles->pFiles);
            new string(text, 0, "Path1".Length + 1 + "Path2".Length + 1 + 1).Should().Be("Path1\0Path2\0\0");
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
        dataObject.SetFileDropList([]);
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
            pDropFiles->pFiles.Should().Be(0u);
            pDropFiles->pt.Should().Be(Point.Empty);
            pDropFiles->fNC.Should().Be(BOOL.FALSE);
            pDropFiles->fWide.Should().Be(BOOL.FALSE);
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
        dataObject.SetFileDropList(["Path1", "Path2"]);
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
        ((Action)(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium))).Should().Throw<ArgumentException>();
    }

    [WinFormsFact]
    public unsafe void IComDataObjectGetDataHere_EmptyFileNamesNoData_Success()
    {
        DataObject dataObject = new();
        dataObject.SetFileDropList([]);
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

        ((Action)(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium))).Should().Throw<COMException>()
            .Where(e => e.HResult == HRESULT.DV_E_TYMED);
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

        ((Action)(() => iComDataObject.GetDataHere(ref formatetc, ref stgMedium))).Should().Throw<COMException>()
            .Where(e => e.HResult == HRESULT.DV_E_FORMATETC);
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
    public unsafe void DataObject_MockRoundTrip_OutData_IsSame(object data)
    {
        dynamic controlAccessor = typeof(Control).TestAccessor().Dynamic;
        var dropTargetAccessor = typeof(DropTarget).TestAccessor();

        DataObject inData = controlAccessor.CreateRuntimeDataObjectForDrag(data);
        if (data is CustomDataObject)
        {
            inData.Should().NotBeSameAs(data);
        }
        else
        {
            inData.Should().BeSameAs(data);
        }

        using var inDataPtr = ComHelpers.GetComScope<Com.IDataObject>(inData);
        IDataObject outData = dropTargetAccessor.CreateDelegate<CreateWinFormsDataObjectForOutgoingDropData>()(inDataPtr);
        outData.Should().BeSameAs(data);
    }

    public static IEnumerable<object[]> DataObjectWithJsonMockRoundTripData()
    {
        yield return new object[] { new DataObject() };
        yield return new object[] { new DerivedDataObject() };
    }

    [WinFormsTheory]
    [MemberData(nameof(DataObjectWithJsonMockRoundTripData))]
    public unsafe void DataObject_WithJson_MockRoundTrip_OutData_IsSame(DataObject data)
    {
        dynamic controlAccessor = typeof(Control).TestAccessor().Dynamic;
        var dropTargetAccessor = typeof(DropTarget).TestAccessor();

        Point point = new() { X = 1, Y = 1 };
        data.SetDataAsJson("point", point);
        DataObject inData = controlAccessor.CreateRuntimeDataObjectForDrag(data);
        inData.Should().BeSameAs(data);

        using var inDataPtr = ComHelpers.GetComScope<Com.IDataObject>(inData);
        IDataObject outData = dropTargetAccessor.CreateDelegate<CreateWinFormsDataObjectForOutgoingDropData>()(inDataPtr);
        outData.Should().BeSameAs(data);
        ITypedDataObject typedOutData = outData.Should().BeAssignableTo<ITypedDataObject>().Subject;
        typedOutData.GetDataPresent("point").Should().BeTrue();
        typedOutData.TryGetData("point", out Point deserialized).Should().BeTrue();
        deserialized.Should().BeEquivalentTo(point);
    }

    [WinFormsFact]
    public unsafe void DataObject_StringData_MockRoundTrip_IsWrapped()
    {
        string testString = "Test";
        dynamic accessor = typeof(Control).TestAccessor().Dynamic;
        var dropTargetAccessor = typeof(DropTarget).TestAccessor();

        DataObject inData = accessor.CreateRuntimeDataObjectForDrag(testString);
        inData.Should().BeAssignableTo<DataObject>();

        using var inDataPtr = ComHelpers.GetComScope<Com.IDataObject>(inData);
        IDataObject outData = dropTargetAccessor.CreateDelegate<CreateWinFormsDataObjectForOutgoingDropData>()(inDataPtr);
        outData.Should().BeSameAs(inData);
        outData.GetData(typeof(string)).Should().Be(testString);
    }

    [WinFormsFact]
    public unsafe void DataObject_IDataObject_MockRoundTrip_IsWrapped()
    {
        CustomIDataObject data = new();
        dynamic accessor = typeof(Control).TestAccessor().Dynamic;
        var dropTargetAccessor = typeof(DropTarget).TestAccessor();

        DataObject inData = accessor.CreateRuntimeDataObjectForDrag(data);
        inData.Should().BeAssignableTo<DataObject>();
        inData.Should().NotBeSameAs(data);

        using var inDataPtr = ComHelpers.GetComScope<Com.IDataObject>(inData);
        IDataObject outData = dropTargetAccessor.CreateDelegate<CreateWinFormsDataObjectForOutgoingDropData>()(inDataPtr);
        outData.Should().BeSameAs(data);
    }

    [WinFormsFact]
    public unsafe void DataObject_ComTypesIDataObject_MockRoundTrip_IsWrapped()
    {
        CustomComTypesDataObject data = new();
        dynamic accessor = typeof(Control).TestAccessor().Dynamic;
        var dropTargetAccessor = typeof(DropTarget).TestAccessor();

        DataObject inData = accessor.CreateRuntimeDataObjectForDrag(data);
        inData.Should().NotBeSameAs(data);
        inData.Should().BeAssignableTo<DataObject>();

        using var inDataPtr = ComHelpers.GetComScope<Com.IDataObject>(inData);
        IDataObject outData = dropTargetAccessor.CreateDelegate<CreateWinFormsDataObjectForOutgoingDropData>()(inDataPtr);
        outData.Should().BeSameAs(inData);
    }

    [WinFormsFact]
    public unsafe void DataObject_Native_GetFormats_ReturnsExpected()
    {
        DataObject native = new();
        using Bitmap bitmap = new(10, 10);
        native.SetImage(bitmap);
        string customFormat = "customFormat";
        native.SetData(customFormat, "custom");
        // Simulate receiving DataObject from native.
        DataObject data = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        data.GetDataPresent(typeof(Bitmap)).Should().BeTrue();
        data.GetDataPresent(customFormat).Should().BeTrue();
        data.GetDataPresent("notExist").Should().BeFalse();
        data.GetFormats().Should().BeEquivalentTo([typeof(Bitmap).FullName, nameof(Bitmap), customFormat]);
    }

    [WinFormsFact]
    public unsafe void DataObject_Native_GetData_SerializationFailure()
    {
        using Font value = new("Arial", 10);
        using BinaryFormatterScope scope = new(enable: true);
        // We are blocking managed font from being serialized as a Locale format.
        DataObject native = new(DataFormats.Locale, value);

        // Simulate receiving DataObject from native.
        // Clipboard.SetDataObject(native, copy: true);
        var comDataObject = ComHelpers.GetComPointer<Com.IDataObject>(native);
        Com.FORMATETC formatetc = new()
        {
            tymed = (uint)TYMED.TYMED_HGLOBAL,
            cfFormat = (ushort)CLIPBOARD_FORMAT.CF_LOCALE
        };
        comDataObject->GetData(formatetc, out Com.STGMEDIUM medium);

        // Validate that HGLOBAL had been freed when handling an error.
        medium.hGlobal.IsNull.Should().BeTrue();
    }

    [WinFormsFact]
    public void DataObject_SetDataAsJson_DataObject_Throws()
    {
        string format = "format";
        DataObject dataObject = new();
        Action action = () => dataObject.SetDataAsJson(format, new DataObject());
        action.Should().Throw<InvalidOperationException>();

        Action dataObjectSet2 = () => dataObject.SetDataAsJson(format, new DerivedDataObject());
        dataObjectSet2.Should().NotThrow();
    }

    [WinFormsFact]
    public void DataObject_SetDataAsJson_ReturnsExpected()
    {
        SimpleTestData testData = new() { X = 1, Y = 1 };
        DataObject dataObject = new();
        string format = "testData";
        dataObject.SetDataAsJson(format, testData);
        dataObject.GetDataPresent(format).Should().BeTrue();
        dataObject.TryGetData(format, out SimpleTestData deserialized).Should().BeTrue();
        deserialized.Should().BeEquivalentTo(testData);
    }

    [WinFormsFact]
    public void DataObject_SetDataAsJson_Wrapped_ReturnsExpected()
    {
        SimpleTestData testData = new() { X = 1, Y = 1 };
        DataObject dataObject = new();
        string format = "testData";
        dataObject.SetDataAsJson(format, testData);
        DataObject wrapped = new(dataObject);
        wrapped.GetDataPresent(format).Should().BeTrue();
        wrapped.TryGetData(format, out SimpleTestData deserialized).Should().BeTrue();
        deserialized.Should().BeEquivalentTo(testData);
    }

    [WinFormsFact]
    public void DataObject_SetDataAsJson_MultipleData_ReturnsExpected()
    {
        SimpleTestData testData1 = new() { X = 1, Y = 1 };
        SimpleTestData testData2 = new() { Y = 2, X = 2 };
        DataObject data = new();
        data.SetDataAsJson("testData1", testData1);
        data.SetDataAsJson("testData2", testData2);
        data.SetData("Mystring", "test");

        data.TryGetData("testData1", out SimpleTestData deserializedTestData1).Should().BeTrue();
        deserializedTestData1.Should().BeEquivalentTo(testData1);
        data.TryGetData("testData2", out SimpleTestData deserializedTestData2).Should().BeTrue();
        deserializedTestData2.Should().BeEquivalentTo(testData2);
        data.TryGetData("Mystring", out string deserializedString).Should().BeTrue();
        deserializedString.Should().Be("test");
    }

    [WinFormsFact]
    public void DataObject_SetDataAsJson_CustomJsonConverter_ReturnsExpected()
    {
        // This test demonstrates one way users can achieve custom JSON serialization behavior if the
        // default JSON serialization behavior that is used in SetDataAsJson APIs is not enough for their scenario.
        Font font = new("Consolas", emSize: 10);
        WeatherForecast forecast = new()
        {
            Date = DateTimeOffset.Now,
            TemperatureCelsius = 25,
            Summary = "Hot",
            Font = font
        };

        DataObject dataObject = new();
        dataObject.SetDataAsJson("custom", forecast);
        dataObject.TryGetData("custom", out WeatherForecast deserialized).Should().BeTrue();
        string offsetFormat = "MM/dd/yyyy";
        deserialized.Date.ToString(offsetFormat).Should().Be(forecast.Date.ToString(offsetFormat));
        deserialized.TemperatureCelsius.Should().Be(forecast.TemperatureCelsius);
        deserialized.Summary.Should().Be($"{forecast.Summary} custom!");
        deserialized.Font.Should().Be(font);
    }

    [JsonConverter(typeof(WeatherForecastJsonConverter))]
    private class WeatherForecast
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string Summary { get; set; }
        public Font Font { get; set; }
    }

    private class WeatherForecastJsonConverter : JsonConverter<WeatherForecast>
    {
        public override WeatherForecast Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            WeatherForecast result = new();
            string fontFamily = null;
            int size = -1;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    if (fontFamily is null || size == -1)
                    {
                        throw new JsonException();
                    }

                    result.Font = new(fontFamily, size);
                    return result;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case nameof(WeatherForecast.Date):
                        result.Date = DateTimeOffset.ParseExact(reader.GetString(), "MM/dd/yyyy", null);
                        break;
                    case nameof(WeatherForecast.TemperatureCelsius):
                        result.TemperatureCelsius = reader.GetInt32();
                        break;
                    case nameof(WeatherForecast.Summary):
                        result.Summary = reader.GetString();
                        break;
                    case nameof(Font.FontFamily):
                        fontFamily = reader.GetString();
                        break;
                    case nameof(Font.Size):
                        size = reader.GetInt32();
                        break;
                    default:
                        throw new JsonException();
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, WeatherForecast value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(WeatherForecast.Date), value.Date.ToString("MM/dd/yyyy"));
            writer.WriteNumber(nameof(WeatherForecast.TemperatureCelsius), value.TemperatureCelsius);
            writer.WriteString(nameof(WeatherForecast.Summary), $"{value.Summary} custom!");
            writer.WriteString(nameof(Font.FontFamily), value.Font.FontFamily.Name);
            writer.WriteNumber(nameof(Font.Size), value.Font.Size);
            writer.WriteEndObject();
        }
    }

    [WinFormsFact]
    public void DataObject_SetDataAsJson_NullData_Throws()
    {
        DataObject dataObject = new();
        Action dataObjectSet = () => dataObject.SetDataAsJson<string>(null);
        dataObjectSet.Should().Throw<ArgumentNullException>();
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(DataObjectTestHelpers), nameof(DataObjectTestHelpers.StringFormat))]
    [CommonMemberData(typeof(DataObjectTestHelpers), nameof(DataObjectTestHelpers.BitmapFormat))]
    [CommonMemberData(typeof(DataObjectTestHelpers), nameof(DataObjectTestHelpers.UndefinedRestrictedFormat))]
    public void DataObject_SetDataAsJson_RestrictedFormats_NotJsonSerialized(string format)
    {
        DataObject dataObject = new();
        dataObject.SetDataAsJson(format, 1);
        object storedData = dataObject.TestAccessor().Dynamic._innerData.GetData(format);
        storedData.Should().NotBeAssignableTo<IJsonData>();
        dataObject.GetData(format).Should().Be(1);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(DataObjectTestHelpers), nameof(DataObjectTestHelpers.UnboundedFormat))]
    public void DataObject_SetDataAsJson_NonRestrictedFormat_NotJsonSerialized(string format)
    {
        DataObject data = new();
        data.SetDataAsJson(format, 1);
        object storedData = data.TestAccessor().Dynamic._innerData.GetData(format);
        storedData.Should().NotBeAssignableTo<IJsonData>();
        data.GetData(format).Should().Be(1);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(DataObjectTestHelpers), nameof(DataObjectTestHelpers.UnboundedFormat))]
    public void DataObject_SetDataAsJson_NonRestrictedFormat_JsonSerialized(string format)
    {
        DataObject data = new();
        SimpleTestData testData = new() { X = 1, Y = 1 };
        data.SetDataAsJson(format, testData);
        object storedData = data.TestAccessor().Dynamic._innerData.GetData(format);
        storedData.Should().BeOfType<JsonData<SimpleTestData>>();

        // We don't expose JsonData<T> in public legacy API
        data.GetData(format).Should().BeNull();
    }

    [WinFormsFact]
    public void DataObject_SetDataAsJson_WrongType_ReturnsNull()
    {
        DataObject dataObject = new();
        dataObject.SetDataAsJson("test", new SimpleTestData() { X = 1, Y = 1 });
        dataObject.TryGetData("test", out Bitmap data).Should().BeFalse();
        data.Should().BeNull();
    }

    [Fact]
    public void DataObject_CreateFromDataObject_DoesNotUnwrapDataStore()
    {
        // The inner data should not have it's data store unwrapped.
        DataObject dataObject = new();
        DataObject wrapped = new(dataObject);
        DataObject.Composition composition = wrapped.TestAccessor().Dynamic._innerData;
        IDataObject original = composition.TestAccessor().Dynamic._winFormsDataObject;
        original.Should().BeSameAs(dataObject);
    }

    [Fact]
    public void DataObject_CreateFromDataObject_VirtualsAreCalled()
    {
        Mock<DataObject> mock = new(MockBehavior.Loose);
        DataObject wrapped = new(mock.Object);

        wrapped.GetData("Foo", false);
        mock.Verify(o => o.GetData("Foo", false), Times.Once());
        mock.VerifyNoOtherCalls();
        mock.Reset();

        wrapped.GetData("Foo");
        mock.Verify(o => o.GetData("Foo", true), Times.Once());
        mock.VerifyNoOtherCalls();
        mock.Reset();

        wrapped.GetData(typeof(string));
        mock.Verify(o => o.GetData("System.String", true), Times.Once());
        mock.VerifyNoOtherCalls();
        mock.Reset();

        wrapped.GetDataPresent("Foo", false);
        mock.Verify(o => o.GetDataPresent("Foo", false), Times.Once());
        mock.VerifyNoOtherCalls();
        mock.Reset();

        wrapped.GetDataPresent("Foo");
        mock.Verify(o => o.GetDataPresent("Foo", true), Times.Once());
        mock.VerifyNoOtherCalls();
        mock.Reset();

        wrapped.GetDataPresent(typeof(string));
        mock.Verify(o => o.GetDataPresent("System.String", true), Times.Once());
        mock.VerifyNoOtherCalls();
        mock.Reset();

        wrapped.GetFormats(false);
        mock.Verify(o => o.GetFormats(false), Times.Once());
        mock.VerifyNoOtherCalls();
        mock.Reset();

        wrapped.GetFormats();
        mock.Verify(o => o.GetFormats(true), Times.Once());
        mock.VerifyNoOtherCalls();
        mock.Reset();

        wrapped.SetData("Foo", "Bar");
        mock.Verify(o => o.SetData("Foo", "Bar"), Times.Once());
        mock.VerifyNoOtherCalls();
        mock.Reset();

        wrapped.SetData(typeof(string), "Bar");
        mock.Verify(o => o.SetData(typeof(string), "Bar"), Times.Once());
        mock.VerifyNoOtherCalls();
        mock.Reset();

        wrapped.SetData("Bar");
        mock.Verify(o => o.SetData("Bar"), Times.Once());
        mock.VerifyNoOtherCalls();
        mock.Reset();
    }
}
