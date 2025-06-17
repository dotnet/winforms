// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class HtmlElementErrorEventArgsTests
{
    [Fact]
    public void HtmlElementErrorEventArgs_InitializesProperties()
    {
        string description = "Script error";
        string urlString = "https://example.com/page";
        int lineNumber = 42;

        HtmlElementErrorEventArgs args = new(description, urlString, lineNumber);

        args.Description.Should().Be(description);
        args.LineNumber.Should().Be(lineNumber);
        args.Url.Should().Be(new Uri(urlString));
        args.Handled.Should().BeFalse();
        args.Handled = true;
        args.Handled.Should().BeTrue();
    }

    [Fact]
    public void HtmlElementErrorEventArgs_Url_CachesUriInstance()
    {
        HtmlElementErrorEventArgs args = new("desc", "https://test", 1);

        Uri url1 = args.Url;
        Uri url2 = args.Url;

        url1.Should().BeSameAs(url2);
    }

    [Fact]
    public void HtmlElementErrorEventArgs_Url_ThrowsOnInvalidUri()
    {
        HtmlElementErrorEventArgs args = new("desc", "not a uri", 1);

        Action action = () => _ = args.Url;

        action.Should().Throw<UriFormatException>();
    }
}
