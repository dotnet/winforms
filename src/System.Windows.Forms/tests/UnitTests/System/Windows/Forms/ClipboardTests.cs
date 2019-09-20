// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms.Tests
{
    public class ClipboardTests
    {
        [StaFact]
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

        [Fact]
        public void Clipboard_Clear_NotSta_ThrowsThreadStateException()
        {
            Assert.Throws<ThreadStateException>(() => Clipboard.Clear());
        }

        [Fact]
        public void Clipboard_ContainsAudio_InvokeMultipleTimes_Success()
        {
            bool result = Clipboard.ContainsAudio();
            Assert.Equal(result, Clipboard.ContainsAudio());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Clipboard_ContainsData_InvokeMultipleTimes_Success(string format)
        {
            bool result = Clipboard.ContainsData(format);
            Assert.Equal(result, Clipboard.ContainsData(format));
            Assert.False(result);
        }

        [Fact]
        public void Clipboard_ContainsFileDropList_InvokeMultipleTimes_Success()
        {
            bool result = Clipboard.ContainsFileDropList();
            Assert.Equal(result, Clipboard.ContainsFileDropList());
        }

        [Fact]
        public void Clipboard_ContainsImage_InvokeMultipleTimes_Success()
        {
            bool result = Clipboard.ContainsImage();
            Assert.Equal(result, Clipboard.ContainsImage());
        }

        [Fact]
        public void Clipboard_ContainsText_InvokeMultipleTimes_Success()
        {
            bool result = Clipboard.ContainsText();
            Assert.Equal(result, Clipboard.ContainsText());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextDataFormat))]
        public void Clipboard_ContainsText_InvokeTextDataFormatMultipleTimes_Success(TextDataFormat format)
        {
            bool result = Clipboard.ContainsText(format);
            Assert.Equal(result, Clipboard.ContainsText(format));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextDataFormat))]
        public void Clipboard_ContainsText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
        {
            Assert.Throws<InvalidEnumArgumentException>("format", () => Clipboard.ContainsText(format));
        }

        [Fact]
        public void Clipboard_GetAudioStream_InvokeMultipleTimes_Success()
        {
            Stream result = Clipboard.GetAudioStream();
            Assert.Equal(result, Clipboard.GetAudioStream());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Clipboard_GetData_InvokeMultipleTimes_Success(string format)
        {
            object result = Clipboard.GetData(format);
            Assert.Equal(result, Clipboard.GetData(format));
            Assert.Null(result);
        }

        [Fact]
        public void Clipboard_GetDataObject_InvokeMultipleTimes_Success()
        {
            object result = Clipboard.GetDataObject();
            Assert.Equal(result, Clipboard.GetDataObject());
        }

        [Fact]
        public void Clipboard_GetFileDropList_InvokeMultipleTimes_Success()
        {
            bool result = Clipboard.ContainsFileDropList();
            Assert.Equal(result, Clipboard.ContainsFileDropList());
        }

        [Fact]
        public void Clipboard_GetImage_InvokeMultipleTimes_Success()
        {
            bool result = Clipboard.ContainsImage();
            Assert.Equal(result, Clipboard.ContainsImage());
        }

        [Fact]
        public void Clipboard_GetText_InvokeMultipleTimes_Success()
        {
            bool result = Clipboard.ContainsText();
            Assert.Equal(result, Clipboard.ContainsText());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextDataFormat))]
        public void Clipboard_GetText_InvokeTextDataFormatMultipleTimes_Success(TextDataFormat format)
        {
            string result = Clipboard.GetText(format);
            Assert.Equal(result, Clipboard.GetText(format));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextDataFormat))]
        public void Clipboard_GetText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
        {
            Assert.Throws<InvalidEnumArgumentException>("format", () => Clipboard.GetText(format));
        }

        [StaFact]
        public void Clipboard_SetAudio_InvokeByteArray_GetReturnsExpected()
        {
            byte[] audioBytes = new byte[] { 1, 2, 3 };
            Clipboard.SetAudio(audioBytes);
            Assert.Equal(audioBytes, Assert.IsType<MemoryStream>(Clipboard.GetAudioStream()).ToArray());
            Assert.Equal(audioBytes, Assert.IsType<MemoryStream>(Clipboard.GetData(DataFormats.WaveAudio)).ToArray());
            Assert.True(Clipboard.ContainsAudio());
            Assert.True(Clipboard.ContainsData(DataFormats.WaveAudio));
        }

        [StaFact]
        public void Clipboard_SetAudio_InvokeEmptyByteArray_GetReturnsExpected()
        {
            byte[] audioBytes = new byte[0];
            Clipboard.SetAudio(audioBytes);
            Assert.Null(Clipboard.GetAudioStream());
            Assert.Null(Clipboard.GetData(DataFormats.WaveAudio));
            Assert.True(Clipboard.ContainsAudio());
            Assert.True(Clipboard.ContainsData(DataFormats.WaveAudio));
        }

        [Fact]
        public void Clipboard_SetAudio_NullAudioBytes_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("audioBytes", () => Clipboard.SetAudio((byte[])null));
        }

        [StaFact]
        public void Clipboard_SetAudio_InvokeStream_GetReturnsExpected()
        {
            byte[] audioBytes = new byte[] { 1, 2, 3 };
            using (var audioStream = new MemoryStream(audioBytes))
            {
                Clipboard.SetAudio(audioStream);
                Assert.Equal(audioBytes, Assert.IsType<MemoryStream>(Clipboard.GetAudioStream()).ToArray());
                Assert.Equal(audioBytes, Assert.IsType<MemoryStream>(Clipboard.GetData(DataFormats.WaveAudio)).ToArray());
                Assert.True(Clipboard.ContainsAudio());
                Assert.True(Clipboard.ContainsData(DataFormats.WaveAudio));
            }
        }

        [StaFact]
        public void Clipboard_SetAudio_InvokeEmptyStream_GetReturnsExpected()
        {
            var audioStream = new MemoryStream();
            Clipboard.SetAudio(audioStream);
            Assert.Null(Clipboard.GetAudioStream());
            Assert.Null(Clipboard.GetData(DataFormats.WaveAudio));
            Assert.True(Clipboard.ContainsAudio());
            Assert.True(Clipboard.ContainsData(DataFormats.WaveAudio));
        }

        [Fact]
        public void Clipboard_SetAudio_NullAudioStream_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("audioStream", () => Clipboard.SetAudio((Stream)null));
        }

        [Fact]
        public void Clipboard_SetAudio_NotSta_ThrowsThreadStateException()
        {
            Assert.Throws<ThreadStateException>(() => Clipboard.SetAudio(new byte[0]));
            Assert.Throws<ThreadStateException>(() => Clipboard.SetAudio(new MemoryStream()));
        }

        [StaTheory]
        [InlineData("format", null)]
        [InlineData("format", 1)]
        public void Clipboard_SetData_Invoke_GetReturnsExpected(string format, object data)
        {
            Clipboard.SetData(format, data);
            Assert.Equal(data, Clipboard.GetData(format));
            Assert.True(Clipboard.ContainsData(format));
        }

        [Fact]
        public void Clipboard_SetData_NullFormat_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("format", () => Clipboard.SetData(null, null));
        }

        [Fact]
        public void Clipboard_SetData_EmptyFormat_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>("format", () => Clipboard.SetData(string.Empty, null));
        }

        [Fact]
        public void Clipboard_SetData_NotSta_ThrowsThreadStateException()
        {
            Assert.Throws<ThreadStateException>(() => Clipboard.SetData("format", null));
        }

        [StaTheory]
        [InlineData(1)]
        [InlineData("data")]
        public void Clipboard_SetDataObject_InvokeObjectNotIComDataObject_GetReturnsExpected(object data)
        {
            Clipboard.SetDataObject(data);
            Assert.Equal(data, Clipboard.GetDataObject().GetData(data.GetType()));
            Assert.True(Clipboard.ContainsData(data.GetType().FullName));
        }

        [StaTheory]
        [InlineData(1)]
        [InlineData("data")]
        public void Clipboard_SetDataObject_InvokeObjectIComDataObject_GetReturnsExpected(object data)
        {
            var dataObject = new DataObject(data);
            Clipboard.SetDataObject(dataObject);
            Assert.Equal(data, Clipboard.GetDataObject().GetData(data.GetType()));
            Assert.True(Clipboard.ContainsData(data.GetType().FullName));
        }

        [StaTheory]
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

        [StaTheory]
        [InlineData(1, true, 0, 0)]
        [InlineData(1, false, 1, 2)]
        [InlineData("data", true, 0, 0)]
        [InlineData("data", false, 1, 2)]
        public void Clipboard_SetDataObject_InvokeObjectBoolIComDataObject_GetReturnsExpected(object data, bool copy, int retryTimes, int retryDelay)
        {
            var dataObject = new DataObject(data);
            Clipboard.SetDataObject(dataObject, copy, retryTimes, retryDelay);
            Assert.Equal(data, Clipboard.GetDataObject().GetData(data.GetType()));
            Assert.True(Clipboard.ContainsData(data.GetType().FullName));
        }

        [StaTheory]
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

        [StaFact]
        public void Clipboard_SetDataObject_NullData_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("data", () => Clipboard.SetDataObject(null));
            Assert.Throws<ArgumentNullException>("data", () => Clipboard.SetDataObject(null, copy: true));
            Assert.Throws<ArgumentNullException>("data", () => Clipboard.SetDataObject(null, copy: true, retryTimes: 10, retryDelay: 0));
        }

        [StaFact]
        public void Clipboard_SetDataObject_NegativeRetryTimes_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("retryTimes", () => Clipboard.SetDataObject(new object(), copy: true, retryTimes: -1, retryDelay: 0));
        }

        [StaFact]
        public void Clipboard_SetDataObject_NegativeRetryDelay_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>("retryDelay", () => Clipboard.SetDataObject(new object(), copy: true, retryTimes: 10, retryDelay: -1));
        }

        [Fact]
        public void Clipboard_SetDataObject_NotSta_ThrowsThreadStateException()
        {
            Assert.Throws<ThreadStateException>(() => Clipboard.SetDataObject(null));
            Assert.Throws<ThreadStateException>(() => Clipboard.SetDataObject(null, copy: true));
            Assert.Throws<ThreadStateException>(() => Clipboard.SetDataObject(null, copy: true, retryTimes: 10, retryDelay: 0));
        }

        [StaFact]
        public void Clipboard_SetFileDropList_Invoke_GetReturnsExpected()
        {
            var filePaths = new StringCollection
            {
                "filePath"
            };
            Clipboard.SetFileDropList(filePaths);
            Assert.Equal(filePaths, Clipboard.GetFileDropList());
            Assert.True(Clipboard.ContainsFileDropList());
        }

        [Fact]
        public void Clipboard_SetFileDropList_NullFilePaths_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("filePaths", () => Clipboard.SetFileDropList(null));
        }

        [Fact]
        public void Clipboard_SetFileDropList_EmptyFilePaths_ThrowsArgumentException()
        {
            var filePaths = new StringCollection();
            Assert.Throws<ArgumentException>(null, () => Clipboard.SetFileDropList(filePaths));
        }

        [Theory]
        [InlineData("")]
        [InlineData("\0")]
        public void Clipboard_SetFileDropList_InvalidFileInPaths_ThrowsArgumentException(string filePath)
        {
            var filePaths = new StringCollection
            {
                filePath
            };
            Assert.Throws<ArgumentException>(null, () => Clipboard.SetFileDropList(filePaths));
        }

        [Fact]
        public void Clipboard_SetFileDropList_NotSta_ThrowsThreadStateException()
        {
            var filePaths = new StringCollection
            {
                "filePath"
            };
            Assert.Throws<ThreadStateException>(() => Clipboard.SetFileDropList(filePaths));
        }

        [StaFact]
        public void Clipboard_SetImage_InvokeBitmap_GetReturnsExpected()
        {
            using (var bitmap = new Bitmap(10, 10))
            {
                bitmap.SetPixel(1, 2, Color.FromArgb(0x01, 0x02, 0x03, 0x04));
                Clipboard.SetImage(bitmap);
                Bitmap result = Assert.IsType<Bitmap>(Clipboard.GetImage());
                Assert.Equal(bitmap.Size, result.Size);
                Assert.Equal(Color.FromArgb(0xFF, 0xD2, 0xD2, 0xD2), result.GetPixel(1, 2));
                Assert.True(Clipboard.ContainsImage());
            }
        }

        [StaFact]
        public void Clipboard_SetImage_InvokeMetafile_GetReturnsExpected()
        {
            using (var metafile = new Metafile("bitmaps/telescope_01.wmf"))
            {
                Clipboard.SetImage(metafile);
                Assert.Null(Clipboard.GetImage());
                Assert.True(Clipboard.ContainsImage());
            }
        }

        [StaFact]
        public void Clipboard_SetImage_InvokeEnhancedMetafile_GetReturnsExpected()
        {
            using (var metafile = new Metafile("bitmaps/milkmateya01.emf"))
            {
                Clipboard.SetImage(metafile);
                Assert.Null(Clipboard.GetImage());
                Assert.True(Clipboard.ContainsImage());
            }
        }

        [StaFact]
        public void Clipboard_SetImage_NullImage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("image", () => Clipboard.SetImage(null));
        }

        [Fact]
        public void Clipboard_SetImage_NotSta_ThrowsThreadStateException()
        {
            using (var bitmap = new Bitmap(10, 10))
            using (var metafile = new Metafile("bitmaps/telescope_01.wmf"))
            using (var enhancedMetafile = new Metafile("bitmaps/milkmateya01.emf"))
            {
                Assert.Throws<ThreadStateException>(() => Clipboard.SetImage(bitmap));
                Assert.Throws<ThreadStateException>(() => Clipboard.SetImage(metafile));
                Assert.Throws<ThreadStateException>(() => Clipboard.SetImage(enhancedMetafile));
            }
        }

        [StaFact]
        public void Clipboard_SetText_InvokeString_GetReturnsExpected()
        {
            Clipboard.SetText("text");
            Assert.Equal("text", Clipboard.GetText());
            Assert.True(Clipboard.ContainsText());
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextDataFormat))]
        public void Clipboard_SetText_InvokeStringTextDataFormat_GetReturnsExpected(TextDataFormat format)
        {
            Clipboard.SetText("text", format);
            Assert.Equal("text", Clipboard.GetText(format));
            Assert.True(Clipboard.ContainsText(format));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void Clipboard_SetText_NullOrEmptyText_ThrowsArgumentNullException(string text)
        {
            Assert.Throws<ArgumentNullException>("text", () => Clipboard.SetText(text));
            Assert.Throws<ArgumentNullException>("text", () => Clipboard.SetText(text, TextDataFormat.Text));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextDataFormat))]
        public void Clipboard_SetText_InvalidFormat_ThrowsInvalidEnumArgumentException(TextDataFormat format)
        {
            Assert.Throws<InvalidEnumArgumentException>("format", () => Clipboard.SetText("text", format));
        }

        [Fact]
        public void Clipboard_SetText_NotSta_ThrowsThreadStateException()
        {
            Assert.Throws<ThreadStateException>(() => Clipboard.SetText("text"));
            Assert.Throws<ThreadStateException>(() => Clipboard.SetText("text", TextDataFormat.Text));
        }
    }
}
