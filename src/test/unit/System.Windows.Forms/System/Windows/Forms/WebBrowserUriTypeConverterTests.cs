// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms.Tests;

public class WebBrowserUriTypeConverterTests
{
    private readonly WebBrowserUriTypeConverter _converter = new();

    [Theory]
    [InlineData("ftp.microsoft.com", "http://ftp.microsoft.com/")]
    [InlineData("  www.example.com  ", "http://www.example.com/")]
    [InlineData("HTTPS://secure.com", "https://secure.com/")]
    [InlineData("localhost", "http://localhost/")]
    [InlineData("sub.domain.com/path", "http://sub.domain.com/path")]
    public void WebBrowserUri_ConvertFrom_RelativeUri_PrependsHttp(string input, string expected)
    {
        Uri result = (Uri)_converter.ConvertFrom(null, CultureInfo.InvariantCulture, input)!;

        result.Should().NotBeNull();
        result.Should().BeOfType<Uri>();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("http://valid.com")]
    [InlineData("https://secure.com")]
    [InlineData("file:///C:/path/to/file.txt")]
    public void WebBrowserUri_ConvertFrom_AbsoluteUri_ReturnsSame(string input)
    {
        Uri inputUri = new(input);
        Uri result = (Uri)_converter.ConvertFrom(null, CultureInfo.InvariantCulture, inputUri)!;

        result.Should().Be(inputUri);
    }

    [Fact]
    public void WebBrowserUri_ConvertFrom_EmptyString_ReturnsNull()
    {
        object? result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, string.Empty);

        result.Should().BeNull();
    }

    [Fact]
    public void WebBrowserUri_ConvertFrom_InvalidUri_ThrowsUriFormatException()
    {
        Action action = () => _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "http://inv@lid uri");

        action.Should().Throw<UriFormatException>();
    }

    [Fact]
    public void WebBrowserUri_ConvertFrom_AlreadyUri_ReturnsSameUri()
    {
        Uri uri = new("http://www.microsoft.com/");
        Uri result = (Uri)_converter.ConvertFrom(null, CultureInfo.InvariantCulture, uri)!;

        result.Should().Be(uri);
    }
}
