// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.IO;
using System.Text;
using Xunit;

namespace System.Resources.Tests
{
    public class ResXFileRef_Converter
    {
        [Theory]
        [InlineData("\"File Name.txt\";", new[] { "File Name.txt", "" })]
        [InlineData("\"File Name.txt\";System.String", new[] { "File Name.txt", "System.String" })]
        [InlineData("\"File Name.txt\";System.String;utf-8", new[] { "File Name.txt", "System.String", "utf-8" })]
        [InlineData("File.txt;", new[] { "File.txt", "" })]
        [InlineData("File.txt;System.String", new[] { "File.txt", "System.String" })]
        [InlineData("File.txt;System.String;utf-8", new[] { "File.txt", "System.String", "utf-8" })]
        [InlineData("File.txt; System.String", new[] { "File.txt", " System.String" })]
        [InlineData("File.txt; System.String; utf-8 ", new[] { "File.txt", " System.String", " utf-8" })]
        [InlineData("File.txt; System.String   ", new[] { "File.txt", " System.String" })]
        [InlineData("File.txt; System.String ; utf-8    ", new[] { "File.txt", " System.String ", " utf-8" })]
        public void ParseResxFileRefString_ReturnsCorrectParts(string resxFileRefString, string[] result)
        {
            string[] parts = ResXFileRef.Converter.ParseResxFileRefString(resxFileRefString);

            Assert.Equal(result, parts);
        }

        [Theory]
        [InlineData("\"File.txt")]
        [InlineData("File.txt")]
        public void ParseResxFileRefString_ThrowsArgumentException(string resxFileRefString)
        {
            Assert.Throws<ArgumentException>(() => ResXFileRef.Converter.ParseResxFileRefString(resxFileRefString));
        }

        [Fact]
        public void ConvertFrom_ReturnNullWhenValueIsNotAString()
        {
            var value = new object();
            var converter = new ResXFileRef.Converter();

            var result = converter.ConvertFrom(null, null, value);

            Assert.Null(result);
        }

        [Fact]
        public void ConvertFrom_ReadsFileAsString()
        {
            var resxFileRefString = @"TestResources\Files\text.ansi.txt;System.String";
            var expected = "Text";
            var converter = new ResXFileRef.Converter();

            var result = (string)converter.ConvertFrom(null, null, resxFileRefString);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertFrom_ReadsFileAsStringUsingEncodingFromRefString()
        {
            var resxFileRefString = @"TestResources\Files\text.utf7.txt;System.String;utf-7";
            var expected = "Привет";
            var converter = new ResXFileRef.Converter();

            var result = (string)converter.ConvertFrom(null, null, resxFileRefString);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertFrom_ReadsFileAsByteArray()
        {
            var resxFileRefString = @"TestResources\Files\text.ansi.txt;System.Byte[]";
            var expected = "Text";
            var converter = new ResXFileRef.Converter();

            var result = (byte[])converter.ConvertFrom(null, null, resxFileRefString);

            Assert.Equal(expected, Encoding.Default.GetString(result));
        }

        [Fact]
        public void ConvertFrom_ReadsFileAsMemoryStream()
        {
            var resxFileRefString = @"TestResources\Files\text.ansi.txt;System.IO.MemoryStream";
            var expected = "Text";
            var converter = new ResXFileRef.Converter();

            var result = (MemoryStream)converter.ConvertFrom(null, null, resxFileRefString);

            Assert.Equal(expected, Encoding.Default.GetString(result.ToArray()));
        }

        [Fact]
        public void ConvertFrom_ReadsFileAsIcon()
        {
            var resxFileRefString = @"TestResources\Files\Error.ico;System.Drawing.Icon, System.Drawing.Common";
            var converter = new ResXFileRef.Converter();

            var result = (Icon)converter.ConvertFrom(null, null, resxFileRefString);

            Assert.NotNull(result);
            Assert.False(result.Size.IsEmpty);
        }

        [Fact]
        public void ConvertFrom_ReadsFileAsIconWhenTypeIsBitmap()
        {
            var bitmapIconRefString = @"TestResources\Files\Error.ico;System.Drawing.Bitmap, System.Drawing.Common";
            var iconRefString = @"TestResources\Files\Error.ico;System.Drawing.Icon, System.Drawing.Common";
            var converter = new ResXFileRef.Converter();

            var iconResult = (Icon)converter.ConvertFrom(null, null, iconRefString);
            var bitmapResult = (Bitmap)converter.ConvertFrom(null, null, bitmapIconRefString);

            Assert.Equal(iconResult.Size, bitmapResult.Size);
        }

        [Fact]
        public void ConvertFrom_ReadsFileAsBitmap()
        {
            var resxFileRefString = @"TestResources\Files\ErrorControl.bmp;System.Drawing.Bitmap, System.Drawing.Common";
            var converter = new ResXFileRef.Converter();

            var result = (Bitmap)converter.ConvertFrom(null, null, resxFileRefString);

            Assert.NotNull(result);
            Assert.False(result.Size.IsEmpty);
        }
    }
}
