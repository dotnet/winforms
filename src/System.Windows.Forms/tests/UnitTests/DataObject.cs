﻿// Licensed to the .NET Foundation under one or more agreements.	
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
            var dataObject = new DataObject();
            dataObject.SetAudio(audioBytes.Span);
            using (var stream = dataObject.GetAudioStream())
            {
                var blob = new byte[stream.Length];
                var read = stream.Read(blob);
                Assert.Equal(blob.Length, read);
                Assert.True(audioBytes.Span.SequenceEqual(blob));
            }
        }

        [Fact]
        public void DataObject_SetAudioEmpty()
        {
            var outer = new DataObject();
            Assert.Throws<ArgumentException>(() => outer.SetAudio(Array.Empty<byte>().AsSpan()));
        }
    }
}