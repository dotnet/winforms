// Licensed to the .NET Foundation under one or more agreements.	
// The .NET Foundation licenses this file to you under the MIT license.	
// See the LICENSE file in the project root for more information.	

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataObjectTests
    {
        [Fact]
        private void DataObject_Constructor()
        {
            var dataObject = new DataObject();
            var formats = dataObject.GetFormats(true);
            Assert.NotNull(formats);
            Assert.Empty(formats);
            formats = dataObject.GetFormats(false);
            Assert.NotNull(formats);
            Assert.Empty(formats);
        }
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
            typeof(Bitmap).FullName,
            "FileName",
            "FileNameW",
        };
        public static TheoryData<string> GetClipboardFormats()
        {
            var data = new TheoryData<string>();
            foreach (string format in s_clipboardFormats)
            {
                data.Add(format);
            }
            return data;
        }
        [Theory]
        [MemberData(nameof(GetClipboardFormats))]
        private void DataObject_DataGetSet(string format)
        {
            string input = "payload";
            DataObject dataObject = new DataObject();
            dataObject.SetData(format, autoConvert: false, input);
            Assert.True(dataObject.GetDataPresent(format, autoConvert: false));
            Assert.Equal(input, dataObject.GetData(format, autoConvert: false));
        }

        public static TheoryData<Memory<byte>> AudioData => TestHelper.GetMemoryBytes();

        [Theory]
        [MemberData(nameof(AudioData))]
        public void DataObject_SetAudio(Memory<byte> audioBytes)
        {
            var inner = new TestDataObject();
            inner.AutoConvert = true;

            var outer = new DataObject((IDataObject)inner);
            outer.SetAudio(audioBytes.Span);

            Assert.True(audioBytes.Span.SequenceEqual(inner.GetDataAsStream()));
            Assert.Equal(DataFormats.WaveAudio, inner.Format);
            Assert.False(inner.AutoConvert);
        }

        [Fact]
        public void DataObject_SetAudioEmpty()
        {
            var outer = new DataObject();
            Assert.Throws<ArgumentException>(() => outer.SetAudio(Array.Empty<byte>().AsSpan()));
        }

        private class TestDataObject : DataObject
        {
            public object Data { get; set; }
            public string Format { get; set; }
            public bool AutoConvert { get; set; }
            public Type FormatType { get; set; }

            public byte[] GetDataAsStream()
            {
                if (!(Data is System.IO.Stream stream))
                    throw new Exception($"The content of {nameof(Data)} was expected to be a Stream");

                var blob = new byte[stream.Length];
                var read = stream.Read(blob);
                if (blob.Length != read)
                    throw new Exception($"The stream length {stream.Length} does not match the content length {read}");
                return blob;
            }

            public override void SetData(object data) => Data = data;

            public override void SetData(string format, bool autoConvert, object data)
            {
                Format = format;
                AutoConvert = autoConvert;
                Data = data;
            }

            public override void SetData(string format, object data)
            {
                Format = format;
                Data = data;
            }

            public override void SetData(Type format, object data)
            {
                FormatType = format;
                Data = data;
            }
        }
    }
}
