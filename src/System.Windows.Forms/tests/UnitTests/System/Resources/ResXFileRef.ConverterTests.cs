// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Text;

namespace System.Resources.Tests;

// NB: doesn't require thread affinity
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
        object value = new();
        ResXFileRef.Converter converter = new();

        object result = converter.ConvertFrom(null, null, value);

        Assert.Null(result);
    }

    [Fact]
    public void ConvertFrom_ReadsFileAsString()
    {
        string resxFileRefString = @"TestResources\Files\text.ansi.txt;System.String";
        string expected = "Text";
        ResXFileRef.Converter converter = new();

        string result = (string)converter.ConvertFrom(null, null, resxFileRefString);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertFrom_ReadsFileAsStringUsingEncodingFromRefString()
    {
        string resxFileRefString = @"TestResources\Files\text.utf8.txt;System.String;utf-8";
        string expected = "Привет";
        ResXFileRef.Converter converter = new();

        string result = (string)converter.ConvertFrom(null, null, resxFileRefString);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertFrom_ReadsFileAsByteArray()
    {
        string resxFileRefString = @"TestResources\Files\text.ansi.txt;System.Byte[]";
        string expected = "Text";
        ResXFileRef.Converter converter = new();

        byte[] result = (byte[])converter.ConvertFrom(null, null, resxFileRefString);

        Assert.Equal(expected, Encoding.Default.GetString(result));
    }

    [Fact]
    public void ConvertFrom_ReadsFileAsMemoryStream()
    {
        string resxFileRefString = @"TestResources\Files\text.ansi.txt;System.IO.MemoryStream";
        string expected = "Text";
        ResXFileRef.Converter converter = new();

        var result = (MemoryStream)converter.ConvertFrom(null, null, resxFileRefString);

        Assert.Equal(expected, Encoding.Default.GetString(result.ToArray()));
    }

    [Fact]
    public void ConvertFrom_ReadsFileAsIcon()
    {
        string resxFileRefString = @"TestResources\Files\Error.ico;System.Drawing.Icon, System.Drawing.Common";
        ResXFileRef.Converter converter = new();

        var result = (Icon)converter.ConvertFrom(null, null, resxFileRefString);

        Assert.NotNull(result);
        Assert.False(result.Size.IsEmpty);
    }

    [Fact]
    public void ConvertFrom_ReadsFileAsIconWhenTypeIsBitmap()
    {
        string bitmapIconRefString = @"TestResources\Files\Error.ico;System.Drawing.Bitmap, System.Drawing.Common";
        string iconRefString = @"TestResources\Files\Error.ico;System.Drawing.Icon, System.Drawing.Common";
        ResXFileRef.Converter converter = new();

        var iconResult = (Icon)converter.ConvertFrom(null, null, iconRefString);
        var bitmapResult = (Bitmap)converter.ConvertFrom(null, null, bitmapIconRefString);

        Assert.Equal(iconResult.Size, bitmapResult.Size);
    }

    [Fact]
    public void ConvertFrom_ReadsFileAsBitmap()
    {
        string resxFileRefString = @"TestResources\Files\ErrorControl.bmp;System.Drawing.Bitmap, System.Drawing.Common";
        ResXFileRef.Converter converter = new();

        var result = (Bitmap)converter.ConvertFrom(null, null, resxFileRefString);

        Assert.NotNull(result);
        Assert.False(result.Size.IsEmpty);
    }
}
