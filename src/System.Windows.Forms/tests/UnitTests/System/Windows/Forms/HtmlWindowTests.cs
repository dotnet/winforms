// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads
public class HtmlWindowTests
{
    [WinFormsFact]
    public async Task HtmlWindow_Opener_NoneReturnsNull()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body>test</body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlWindow window = document.Window;
        window.Should().NotBeSameAs(document.Window);
        window.Opener.Should().BeNull();
    }

    [WinFormsFact]
    public async Task HtmlWindow_WindowFrameElement_NoneReturnsNull()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body>test</body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlWindow window = document.Window;
        window.Should().NotBeSameAs(document.Window);
        window.WindowFrameElement.Should().BeNull();
    }

    [WinFormsFact]
    public async Task HtmlWindow_DomWindow_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body>test</body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlWindow window = document.Window;
        object domWindow = window.DomWindow;

        domWindow.Should().BeSameAs(window.DomWindow);
        domWindow.GetType().IsCOMObject.Should().BeTrue();
        domWindow.Should().BeAssignableTo<IHTMLWindow2.Interface>();
        domWindow.Should().BeAssignableTo<IHTMLWindow3.Interface>();
        domWindow.Should().BeAssignableTo<IHTMLWindow4.Interface>();
    }

    [WinFormsFact]
    public async Task HtmlWindow_NavigateAround_MaintainsEquality()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent,
        };

        string Html =
            $"""
            <html>
                <frameset rows="1,1,1" cols="1">
                    <frame id="1" name="1">
                    <frame id="2" name="2">
                </frameset>
            </html>
            """;

        HtmlDocument document = await GetDocument(control, Html);
        HtmlWindow window = document.Window;
        window.Should().Be(document.Window);
        window.Should().Be(document.GetElementById("1").Parent.Document.Window);
        window.Should().Be(window.Frames[0].Parent.Document.Window);
        window.Should().Be(window.Frames[1].Parent.Document.Window);
    }

    private static async Task<HtmlDocument> GetDocument(WebBrowser control, string html)
    {
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        using var file = CreateTempFile(html);
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);

        return control.Document;
    }

    private static TempFile CreateTempFile(string html)
    {
        byte[] data = Encoding.UTF8.GetBytes(html);
        return TempFile.Create(data);
    }
}
