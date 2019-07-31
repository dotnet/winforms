// Licensed to the .NET Foundation under one or more agreements.	
// The .NET Foundation licenses this file to you under the MIT license.	
// See the LICENSE file in the project root for more information.	

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
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
            var dataObject = new DataObject();
            Assert.False(dataObject.ContainsAudio());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataObject_ContainsAudio_InvokeMocked_CallsGetDataPresent(bool result)
        {
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var dataObject = new DataObject();
            Assert.False(dataObject.ContainsFileDropList());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataObject_ContainsFileDropList_InvokeMocked_CallsGetDataPresent(bool result)
        {
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var dataObject = new DataObject();
            Assert.False(dataObject.ContainsImage());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataObject_ContainsImage_InvokeMocked_CallsGetDataPresent(bool result)
        {
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var dataObject = new DataObject();
            Assert.False(dataObject.ContainsText());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataObject_ContainsText_InvokeMocked_CallsGetDataPresent(bool result)
        {
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextDataFormat))]
        public void DataObject_ContainsText_InvokeTextDataFormat_ReturnsFalse(TextDataFormat format)
        {
            var dataObject = new DataObject();
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
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextDataFormat))]
        public void DataObject_ContainsText_InvokeInvalidTextDataFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
        {
            var dataObject = new DataObject();
            Assert.Throws<InvalidEnumArgumentException>("format", () => dataObject.ContainsText(format));
        }

        [Fact]
        public void DataObject_GetAudioStream_InvokeDefault_ReturnsNull()
        {
            var dataObject = new DataObject();
            Assert.Null(dataObject.GetAudioStream());
        }
        
        public static IEnumerable<object[]> GetAudioStream_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new object(), null };

            var stream = new MemoryStream();
            yield return new object[] { stream, stream };
        }

        [Theory]
        [MemberData(nameof(GetAudioStream_TestData))]
        public void DataObject_GetAudioStream_InvokeWithData_ReturnsExpected(object result, Stream expected)
        {
            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.WaveAudio, result);
            Assert.Same(expected, dataObject.GetAudioStream());
        }

        [Theory]
        [MemberData(nameof(GetAudioStream_TestData))]
        public void DataObject_GetAudioStream_InvokeMocked_ReturnsExpected(object result, Stream expected)
        {
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var dataObject = new DataObject();
            Assert.Null(dataObject.GetData(format));
        }

        public static IEnumerable<object[]> GetData_InvokeStringMocked_TestData()
        {
            foreach (object result in new object[] { new object(), null })
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
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            foreach (object result in new object[] { new object(), null })
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
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.GetData(format, true))
                .Returns(result)
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
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
            var dataObject = new DataObject();
            Assert.Null(dataObject.GetData(format, autoConvert));
        }

        public static IEnumerable<object[]> GetData_StringBoolIDataObject_TestData()
        {
            foreach (bool autoConvert in new bool[] { true, false })
            {
                foreach (object result in new object[] { new object(), null })
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
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.GetData(format, autoConvert))
                .Returns(result)
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
            Assert.Same(result, dataObject.GetData(format, autoConvert));
            mockDataObject.Verify(o => o.GetData(format, autoConvert), Times.Once());
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(null)]
        public void DataObject_GetData_InvokeTypeDefault_ReturnsNull(Type format)
        {
            var dataObject = new DataObject();
            Assert.Null(dataObject.GetData(format));
        }

        public static IEnumerable<object[]> GetData_InvokeTypeMocked_TestData()
        {
            foreach (object result in new object[] { new object(), null })
            {
                yield return new object[] { typeof(int), result, 1, result };
                yield return new object[] { null, result, 0, null };
            }
        }

        [Theory]
        [MemberData(nameof(GetData_InvokeTypeMocked_TestData))]
        public void DataObject_GetData_InvokeTypeMocked_CallsGetData(Type format, object result, int expectedCallCount, object expectedResult)
        {
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var dataObject = new DataObject();
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
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.GetDataPresent(format, true))
                .Returns(result)
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
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
            var dataObject = new DataObject();
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
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.GetDataPresent(format, autoConvert))
                .Returns(result)
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
            Assert.Equal(result, dataObject.GetDataPresent(format, autoConvert));
            mockDataObject.Verify(o => o.GetDataPresent(format, autoConvert), Times.Once());
        }


        [Theory]
        [InlineData(typeof(int))]
        [InlineData(null)]
        public void DataObject_GetDataPresentPresent_InvokeTypeDefault_ReturnsFalse(Type format)
        {
            var dataObject = new DataObject();
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
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var dataObject = new DataObject();
            Assert.Empty(dataObject.GetFileDropList());
        }
        
        public static IEnumerable<object[]> GetFileDropList_TestData()
        {
            yield return new object[] { null, Array.Empty<string>() };
            yield return new object[] { new object(), Array.Empty<string>() };

            var list = new string[] { "a", "  ", string.Empty, null };
            yield return new object[] { list, list };
        }

        [Theory]
        [MemberData(nameof(GetFileDropList_TestData))]
        public void DataObject_GetFileDropList_InvokeWithData_ReturnsExpected(object result, string[] expected)
        {
            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.FileDrop, result);
            Assert.Equal(expected, dataObject.GetFileDropList().Cast<string>());
        }

        [Theory]
        [MemberData(nameof(GetFileDropList_TestData))]
        public void DataObject_GetFileDropList_InvokeMocked_ReturnsExpected(object result, string[] expected)
        {
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var dataObject = new DataObject();
            Assert.Empty(dataObject.GetFormats());
        }

        public static IEnumerable<object[]> GetFormats_Mocked_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new string[0] };
            yield return new object[] { new string[] { "a", "  ", string.Empty, null } };
        }

        [Theory]
        [MemberData(nameof(GetFormats_Mocked_TestData))]
        public void DataObject_GetFormats_InvokeMocked_ReturnsExpected(string[] result)
        {
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            yield return new object[] { new string[0] };
            yield return new object[] { new string[] { "a", string.Empty, null } };
            yield return new object[] { new string[] { "a", "  ", string.Empty, null } };
        }

        [Theory]
        [MemberData(nameof(GetFormats_IDataObject_TestData))]
        public void DataObject_GetFormats_InvokeIDataObject_ReturnsExpected(string[] result)
        {
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.GetFormats(true))
                .Returns(result)
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
            Assert.Same(result, dataObject.GetFormats());
            mockDataObject.Verify(o => o.GetFormats(true), Times.Once());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataObject_GetFormats_InvokeBoolDefault_ReturnsEmpty(bool autoConvert)
        {
            var dataObject = new DataObject();
            Assert.Empty(dataObject.GetFormats(autoConvert));
        }

        public static IEnumerable<object[]> GetFormats_BoolIDataObject_TestData()
        {
            foreach (bool autoConvert in new bool[] { true, false })
            {
                yield return new object[] { autoConvert, null };
                yield return new object[] { autoConvert, new string[0] };
                yield return new object[] { autoConvert, new string[] { "a", "  ", string.Empty, null } };
            }
        }

        [Theory]
        [MemberData(nameof(GetFormats_BoolIDataObject_TestData))]
        public void DataObject_GetFormats_InvokeBoolIDataObject_ReturnsExpected(bool autoConvert, string[] result)
        {
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.GetFormats(autoConvert))
                .Returns(result)
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
            Assert.Same(result, dataObject.GetFormats(autoConvert));
            mockDataObject.Verify(o => o.GetFormats(autoConvert), Times.Once());
        }

        [Fact]
        public void DataObject_GetImage_InvokeDefault_ReturnsNull()
        {
            var dataObject = new DataObject();
            Assert.Null(dataObject.GetImage());
        }
        
        public static IEnumerable<object[]> GetImage_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new object(), null };

            var image = new Bitmap(10, 10);
            yield return new object[] { image, image };
        }

        [Theory]
        [MemberData(nameof(GetImage_TestData))]
        public void DataObject_GetImage_InvokeWithData_ReturnsExpected(object result, Image expected)
        {
            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.Bitmap, result);
            Assert.Same(expected, dataObject.GetImage());
        }

        [Theory]
        [MemberData(nameof(GetImage_TestData))]
        public void DataObject_GetImage_InvokeMocked_ReturnsExpected(object result, Image expected)
        {
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var dataObject = new DataObject();
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
            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.UnicodeText, result);
            Assert.Equal(expected, dataObject.GetText());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataObject_GetText_InvokeMocked_ReturnsExpected(string result)
        {
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextDataFormat))]
        public void DataObject_GetText_InvokeTextDataFormatDefault_ReturnsEmpty(TextDataFormat format)
        {
            var dataObject = new DataObject();
            Assert.Empty(dataObject.GetText(format));
        }
        
        public static IEnumerable<object[]> GetText_TextDataFormat_TestData()
        {
            yield return new object[] { TextDataFormat.Text, DataFormats.UnicodeText, null, string.Empty };
            yield return new object[] { TextDataFormat.Text, DataFormats.UnicodeText, new object(), string.Empty };
            yield return new object[] { TextDataFormat.Text, DataFormats.UnicodeText, string.Empty, string.Empty };
            yield return new object[] { TextDataFormat.Text, DataFormats.UnicodeText, "  ", "  " };
            yield return new object[] { TextDataFormat.Text, DataFormats.UnicodeText, "a", "a" };

            yield return new object[] { TextDataFormat.UnicodeText, DataFormats.UnicodeText, null, string.Empty };
            yield return new object[] { TextDataFormat.UnicodeText, DataFormats.UnicodeText, new object(), string.Empty };
            yield return new object[] { TextDataFormat.UnicodeText, DataFormats.UnicodeText, string.Empty, string.Empty };
            yield return new object[] { TextDataFormat.UnicodeText, DataFormats.UnicodeText, "  ", "  " };
            yield return new object[] { TextDataFormat.UnicodeText, DataFormats.UnicodeText, "a", "a" };

            yield return new object[] { TextDataFormat.Rtf, DataFormats.Rtf, null, string.Empty };
            yield return new object[] { TextDataFormat.Rtf, DataFormats.Rtf, new object(), string.Empty };
            yield return new object[] { TextDataFormat.Rtf, DataFormats.Rtf, string.Empty, string.Empty };
            yield return new object[] { TextDataFormat.Rtf, DataFormats.Rtf, "  ", "  " };
            yield return new object[] { TextDataFormat.Rtf, DataFormats.Rtf, "a", "a" };

            yield return new object[] { TextDataFormat.Html, DataFormats.Html, null, string.Empty };
            yield return new object[] { TextDataFormat.Html, DataFormats.Html, new object(), string.Empty };
            yield return new object[] { TextDataFormat.Html, DataFormats.Html, string.Empty, string.Empty };
            yield return new object[] { TextDataFormat.Html, DataFormats.Html, "  ", "  " };
            yield return new object[] { TextDataFormat.Html, DataFormats.Html, "a", "a" };

            yield return new object[] { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, null, string.Empty };
            yield return new object[] { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, new object(), string.Empty };
            yield return new object[] { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, string.Empty, string.Empty };
            yield return new object[] { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, "  ", "  " };
            yield return new object[] { TextDataFormat.CommaSeparatedValue, DataFormats.CommaSeparatedValue, "a", "a" };
        }

        [Theory]
        [MemberData(nameof(GetText_TextDataFormat_TestData))]
        public void DataObject_GetText_InvokeTextDataFormatWithData_ReturnsExpected(TextDataFormat format, string expectedFormat, object result, string expected)
        {
            var dataObject = new DataObject();
            dataObject.SetData(expectedFormat, result);
            Assert.Same(expected, dataObject.GetText(format));
        }

        [Theory]
        [MemberData(nameof(GetText_TextDataFormat_TestData))]
        public void DataObject_GetText_InvokeTextDataFormatMocked_ReturnsExpected(TextDataFormat format, string expectedFormat, object result, string expected)
        {
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextDataFormat))]
        public void DataObject_GetText_InvokeInvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
        {
            var dataObject = new DataObject();
            Assert.Throws<InvalidEnumArgumentException>("format", () => dataObject.GetText(format));
        }

        public static IEnumerable<object[]> SetAudio_ByteArray_TestData()
        {
            yield return new object[] { new byte[0] };
            yield return new object[] { new byte[] { 1, 2, 3 } };
        }

        [Theory]
        [MemberData(nameof(SetAudio_ByteArray_TestData))]
        public void DataObject_SetAudio_InvokeByteArray_GetReturnsExpected(byte[] audioBytes)
        {
            var dataObject = new DataObject();
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
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.SetData(DataFormats.WaveAudio, false, It.IsAny<MemoryStream>()))
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
            dataObject.SetAudio(audioBytes);
            mockDataObject.Verify(o => o.SetData(DataFormats.WaveAudio, false, It.IsAny<MemoryStream>()), Times.Once());
        }

        [Fact]
        public void DataObject_SetAudio_NullAudioBytes_ThrowsArgumentNullException()
        {
            var dataObject = new DataObject();
            Assert.Throws<ArgumentNullException>("audioBytes", () => dataObject.SetAudio((byte[])null));
        }

        public static IEnumerable<object[]> SetAudio_Stream_TestData()
        {
            yield return new object[] { new MemoryStream(new byte[0]) };
            yield return new object[] { new MemoryStream(new byte[] { 1, 2, 3 }) };
        }

        [Theory]
        [MemberData(nameof(SetAudio_Stream_TestData))]
        public void DataObject_SetAudio_InvokeStream_GetReturnsExpected(Stream audioStream)
        {
            var dataObject = new DataObject();
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
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.SetData(DataFormats.WaveAudio, false, audioStream))
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
            dataObject.SetAudio(audioStream);
            mockDataObject.Verify(o => o.SetData(DataFormats.WaveAudio, false, audioStream), Times.Once());
        }

        [Fact]
        public void DataObject_SetAudio_NullAudioStream_ThrowsArgumentNullException()
        {
            var dataObject = new DataObject();
            Assert.Throws<ArgumentNullException>("audioStream", () => dataObject.SetAudio((Stream)null));
        }

        public static IEnumerable<object[]> SetData_Object_TestData()
        {
            yield return new object[] { new object(), typeof(object).FullName };
            yield return new object[] { new Bitmap(10, 10), typeof(Bitmap).FullName };
            yield return new object[] { new Mock<ISerializable>(MockBehavior.Strict).Object, DataFormats.Serializable };
        }

        [Theory]
        [MemberData(nameof(SetData_Object_TestData))]
        public void SetData_Object_GetDataReturnsExpected(object data, string expectedFormat)
        {
            var dataObject = new DataObject();
            dataObject.SetData(data);
            Assert.Same(data, dataObject.GetData(expectedFormat, autoConvert: false));
            Assert.Same(data, dataObject.GetData(expectedFormat, autoConvert: true));
            Assert.True(dataObject.GetDataPresent(expectedFormat, autoConvert: true));
            Assert.True(dataObject.GetDataPresent(expectedFormat, autoConvert: false));
        }

        [Fact]
        public void DataObject_SetData_MultipleNonSerializable_GetDataReturnsExpected()
        {
            var data1 = new object();
            var data2 = new object();

            var dataObject = new DataObject();
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

            var dataObject = new DataObject();
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
            yield return new object[] { new object() };
        }

        [Theory]
        [MemberData(nameof(SetData_ObjectIDataObject_TestData))]
        public void DataObject_SetData_InvokeObjectIDataObject_CallsSetData(object data)
        {
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.SetData(data))
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
            dataObject.SetData(data);
            mockDataObject.Verify(o => o.SetData(data), Times.Once());
        }

        [Fact]
        public void DataObject_SetData_NullData_ThrowsArgumentNullException()
        {
            var dataObject = new DataObject();
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
            var dataObject = new DataObject();
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
            var image = new Bitmap(10, 10);
            var dataObject = new DataObject();
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
                yield return new object[] { format, new object() };
            }
        }

        [Theory]
        [MemberData(nameof(SetData_StringObjectIDataObject_TestData))]
        public void DataObject_SetData_InvokeStringObjectIDataObject_CallsSetData(string format, object data)
        {
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.SetData(format, data))
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
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
            var dataObject = new DataObject();
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
            var image = new Bitmap(10, 10);
            var dataObject = new DataObject();
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
                    yield return new object[] { format, autoConvert, new object() };
                }
            }
        }

        [Theory]
        [MemberData(nameof(SetData_StringBoolObjectIDataObject_TestData))]
        public void DataObject_SetData_InvokeStringBoolObjectIDataObject_CallsSetData(string format, bool autoConvert, object data)
        {
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.SetData(format, autoConvert, data))
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
            dataObject.SetData(format, autoConvert, data);
            mockDataObject.Verify(o => o.SetData(format, autoConvert, data), Times.Once());
        }

        public static IEnumerable<object[]> SetData_TypeObjectIDataObject_TestData()
        {
            foreach (Type format in new Type[] { typeof(int), null })
            {
                yield return new object[] { format, null };
                yield return new object[] { format, new object() };
            }
        }

        [Theory]
        [MemberData(nameof(SetData_TypeObjectIDataObject_TestData))]
        public void DataObject_SetData_InvokeTypeObjectIDataObject_CallsSetData(Type format, object data)
        {
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.SetData(format, data))
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
            dataObject.SetData(format, data);
            mockDataObject.Verify(o => o.SetData(format, data), Times.Once());
        }

        [Fact]
        public void DataObject_SetData_NullFormat_ThrowsArgumentNullException()
        {
            var dataObject = new DataObject();
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
            var dataObject = new DataObject();
            Assert.Throws<ArgumentException>("format", () => dataObject.SetData(format, new object()));
            Assert.Throws<ArgumentException>("format", () => dataObject.SetData(format, true, new object()));
            Assert.Throws<ArgumentException>("format", () => dataObject.SetData(format, false, new object()));
        }

        [Fact]
        public void DataObject_SetData_DibBitmapNoAutoConvert_ThrowsNotSupportedException()
        {
            var dataObject = new DataObject();
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
            var dataObject = new DataObject();
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
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.SetData(DataFormats.FileDrop, true, It.IsAny<string[]>()))
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
            dataObject.SetFileDropList(filePaths);
            mockDataObject.Verify(o => o.SetData(DataFormats.FileDrop, true, It.IsAny<string[]>()), Times.Once());
        }

        [Fact]
        public void DataObject_SetFileDropList_NullFilePaths_ThrowsArgumentNullException()
        {
            var dataObject = new DataObject();
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
            var dataObject = new DataObject();
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
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.SetData(DataFormats.Bitmap, true, image))
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
            dataObject.SetImage(image);
            mockDataObject.Verify(o => o.SetData(DataFormats.Bitmap, true, image), Times.Once());
        }

        [Fact]
        public void DataObject_SetImage_NullImage_ThrowsArgumentNullException()
        {
            var dataObject = new DataObject();
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
            var dataObject = new DataObject();
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
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.SetData(DataFormats.UnicodeText, false, textData))
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
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
            var dataObject = new DataObject();
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
            Assert.Equal(expectedUnicodeText != null, dataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: true));
            Assert.Equal(expectedUnicodeText != null, dataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: false));
            Assert.False(dataObject.GetDataPresent(DataFormats.Text, autoConvert: true));
            Assert.False(dataObject.GetDataPresent(DataFormats.Text, autoConvert: false));
            Assert.False(dataObject.GetDataPresent(DataFormats.StringFormat, autoConvert: true));
            Assert.False(dataObject.GetDataPresent(DataFormats.StringFormat, autoConvert: false));
            Assert.Equal(expectedRtfText != null, dataObject.GetDataPresent(DataFormats.Rtf, autoConvert: true));
            Assert.Equal(expectedRtfText != null, dataObject.GetDataPresent(DataFormats.Rtf, autoConvert: false));
            Assert.Equal(expectedHtmlText != null, dataObject.GetDataPresent(DataFormats.Html, autoConvert: true));
            Assert.Equal(expectedHtmlText != null, dataObject.GetDataPresent(DataFormats.Html, autoConvert: false));
            Assert.Equal(expectedCsvText != null, dataObject.GetDataPresent(DataFormats.CommaSeparatedValue, autoConvert: true));
            Assert.Equal(expectedCsvText != null, dataObject.GetDataPresent(DataFormats.CommaSeparatedValue, autoConvert: false));
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
            var mockDataObject = new Mock<DataObject>(MockBehavior.Strict);
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
            var mockDataObject = new Mock<IDataObject>(MockBehavior.Strict);
            mockDataObject
                .Setup(o => o.SetData(expectedFormat, false, textData))
                .Verifiable();
            var dataObject = new DataObject((object)mockDataObject.Object);
            dataObject.SetText(textData, format);
            mockDataObject.Verify(o => o.SetData(expectedFormat, false, textData), Times.Once());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void DataObject_SetText_NullOrEmptyTextData_ThrowsArgumentNullException(string textData)
        {
            var dataObject = new DataObject();
            Assert.Throws<ArgumentNullException>("textData", () => dataObject.SetText(textData));
            Assert.Throws<ArgumentNullException>("textData", () => dataObject.SetText(textData, TextDataFormat.Text));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextDataFormat))]
        public void DataObject_SetText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
        {
            var dataObject = new DataObject();
            Assert.Throws<InvalidEnumArgumentException>("format", () => dataObject.SetText("text",  format));
        }
    }
}
