// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms.Tests;

public class WebBrowserUriTypeConverterTests
{
    private readonly WebBrowserUriTypeConverter _converter = new();

    [Theory]
    [InlineData("http://www.microsoft.com", "http://www.microsoft.com/")]
    [InlineData("www.microsoft.com", "http://www.microsoft.com/")]
    public void WebBrowserUri_ConvertFrom_UriString(string input, string expected)
    {
        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input);

        result.Should().BeOfType<Uri>();
        ((Uri)result!).AbsoluteUri.Should().Be(expected);
    }

    [Fact]
    public void WebBrowserUri_ConvertFrom_EmptyString_ReturnsNull()
    {
        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, "");

        result.Should().BeNull();
    }

    [Fact]
    public void WebBrowserUri_ConvertFrom_InvalidUri_ThrowsUriFormatException()
    {
        string invalid = "http://inv@lid uri";
        Action action = () => _converter.ConvertFrom(null, CultureInfo.InvariantCulture, invalid);

        action.Should().Throw<UriFormatException>();
    }

    [Fact]
    public void WebBrowserUri_ConvertFrom_AlreadyUri_ReturnsSameUri()
    {
        Uri uri = new("http://www.microsoft.com/");
        var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, uri);

        result.Should().Be(uri);
    }
}
