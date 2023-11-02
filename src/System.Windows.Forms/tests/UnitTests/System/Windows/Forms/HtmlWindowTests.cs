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
        using var parent = new Control();
        using var control = new WebBrowser
        {
            Parent = parent
        };

        const string Html = "<html><body>test</body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlWindow window = document.Window;
        Assert.NotSame(window, document.Window);
        Assert.Null(window.Opener);
    }

    [WinFormsFact]
    public async Task HtmlWindow_WindowFrameElement_NoneReturnsNull()
    {
        using var parent = new Control();
        using var control = new WebBrowser
        {
            Parent = parent
        };

        const string Html = "<html><body>test</body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlWindow window = document.Window;
        Assert.NotSame(window, document.Window);
        Assert.Null(window.WindowFrameElement);
    }

    [WinFormsFact]
    public async Task HtmlWindow_DomWindow_Get_ReturnsExpected()
    {
        using var parent = new Control();
        using var control = new WebBrowser
        {
            Parent = parent
        };

        const string Html = "<html><body>test</body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlWindow window = document.Window;
        object domWindow = window.DomWindow;

        Assert.Same(domWindow, window.DomWindow);
        Assert.True(domWindow.GetType().IsCOMObject);
        Assert.True(domWindow is IHTMLWindow2.Interface);
        Assert.True(domWindow is IHTMLWindow3.Interface);
        Assert.True(domWindow is IHTMLWindow4.Interface);
    }

    private static async Task<HtmlDocument> GetDocument(WebBrowser control, string html)
    {
        var source = new TaskCompletionSource<bool>();
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
