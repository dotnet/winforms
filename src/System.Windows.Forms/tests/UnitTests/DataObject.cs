// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Collections.Generic;
using System.Drawing;

namespace System.Windows.Forms.Tests
{
    public class DataObjectTests
    {
        [Fact]
        public void Defaults()
        {
            var dataObject = new DataObject();

            var formats = dataObject.GetFormats(true);
            Assert.NotNull(formats);
            Assert.Empty(formats);

            formats = dataObject.GetFormats(false);
            Assert.NotNull(formats);
            Assert.Empty(formats);
        }

        private static readonly string[] s_formats =
        {
            DataFormats.CommaSeparatedValue,
            DataFormats.Dib,
            DataFormats.Dif,
            DataFormats.PenData,
            DataFormats.Riff,
            DataFormats.Serializable,
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
            "something custom",
            typeof(Bitmap).FullName,
            "FileName",
            "FileNameW",
        };

        public static IEnumerable<object[]> ClipboardFormats
        {
            get
            {
                foreach (string format in s_formats)
                {
                    yield return new object[] { format };
                }
            }
        }

        [Theory]
        [MemberData(nameof(ClipboardFormats))]
        private void SetGetDataRoundTrip(string format)
        {
            string input = "payload";
            DataObject dataObject = new DataObject();

            dataObject.SetData(format, autoConvert: false, input);

            Assert.True(dataObject.GetDataPresent(format, false));
            Assert.Equal(input, dataObject.GetData(format, autoConvert: false));
        }
    }
}
