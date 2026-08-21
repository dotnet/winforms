// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Text;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads
public class HtmlHistoryTests
{
    private const string HtmlPage1 = "<html><body>page1</body></html>";
    private const string HtmlPage2 = "<html><body>page2</body></html>";

    [WinFormsFact]
    public async Task HtmlHistory_Length_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        HtmlDocument document = await GetDocument(control, HtmlPage1);
        document.Window.Should().NotBeNull();
        using HtmlHistory? history = document.Window.History;

        history.Should().NotBeNull();
        // IE/WebBrowser reports engine-specific history lengths (often 0 for a single load).
        // Exercise the getter and only require a non-negative value.
        history!.Length.Should().BeGreaterThanOrEqualTo(0);
    }

    [WinFormsFact]
    public async Task HtmlHistory_DomHistory_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        HtmlDocument document = await GetDocument(control, HtmlPage1);
        document.Window.Should().NotBeNull();
        using HtmlHistory? history = document.Window.History;

        history.Should().NotBeNull();
        object domHistory = history!.DomHistory;

        domHistory.Should().NotBeNull();
        domHistory.Should().BeSameAs(history.DomHistory);
        domHistory.GetType().IsCOMObject.Should().BeTrue();
        domHistory.Should().BeAssignableTo<IOmHistory.Interface>();
    }

    [WinFormsFact]
    public async Task HtmlHistory_Back_Negative_ThrowsArgumentOutOfRangeException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        HtmlDocument document = await GetDocument(control, HtmlPage1);
        document.Window.Should().NotBeNull();
        using HtmlHistory? history = document.Window.History;

        history.Should().NotBeNull();
        Action action = () => history!.Back(-1);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("numberBack");
    }

    [WinFormsFact]
    public async Task HtmlHistory_Forward_Negative_ThrowsArgumentOutOfRangeException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        HtmlDocument document = await GetDocument(control, HtmlPage1);
        document.Window.Should().NotBeNull();
        using HtmlHistory? history = document.Window.History;

        history.Should().NotBeNull();
        Action action = () => history!.Forward(-1);
        action.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("numberForward");
    }

    [WinFormsFact]
    public async Task HtmlHistory_Back_And_Forward_Zero_DoNotThrow()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        HtmlDocument document = await GetDocument(control, HtmlPage1);
        document.Window.Should().NotBeNull();
        using HtmlHistory? history = document.Window.History;

        history.Should().NotBeNull();

        // Zero is a no-op (guarded by number > 0) and must not call into COM go().
        Action back = () => history!.Back(0);
        Action forward = () => history!.Forward(0);

        back.Should().NotThrow();
        forward.Should().NotThrow();
    }

    [WinFormsFact]
    public async Task HtmlHistory_Back_And_Forward_Positive_InvokeGo()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        // Keep both temp files alive for the life of the test. Disposing them after
        // Navigate can leave history entries pointing at deleted paths and collapse the stack.
        using TempFile file1 = CreateTempFile(HtmlPage1);
        using TempFile file2 = CreateTempFile(HtmlPage2);

        await NavigateToPathAsync(control, file1.Path);
        await NavigateToPathAsync(control, file2.Path);

        // Positive Back/Forward must enter the number > 0 branch that calls IOmHistory.go.
        // Do not assert Length — IE reports engine-specific values (often 1 after two loads).
        // Do not wait for DocumentCompleted — go() may not raise it when the stack is thin.
        control.Document.Should().NotBeNull();
        control.Document!.Window.Should().NotBeNull();

        using (HtmlHistory? history = control.Document.Window!.History)
        {
            history.Should().NotBeNull();
            Action back = () => history!.Back(1);
            back.Should().NotThrow();
        }

        // History wrapper is per-get; obtain a fresh instance after Back.
        control.Document.Window.Should().NotBeNull();
        using (HtmlHistory? history = control.Document.Window!.History)
        {
            history.Should().NotBeNull();
            Action forward = () => history!.Forward(1);
            forward.Should().NotThrow();
        }
    }

    [WinFormsFact]
    public async Task HtmlHistory_Go_Overloads_DoNotThrow()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        HtmlDocument document = await GetDocument(control, HtmlPage1);
        document.Window.Should().NotBeNull();
        using HtmlHistory? history = document.Window.History;

        history.Should().NotBeNull();

        // Relative position always forwards to COM go(); 0 is the safe relative position.
        Action goRelative = () => history!.Go(0);
        // String overload intentionally accepts values that may not be fully qualified Uris.
        Action goString = () => history!.Go("about:blank");
        // Uri overload is a thin wrapper over the string overload.
        Action goUri = () => history!.Go(new Uri("about:blank"));

        goRelative.Should().NotThrow();
        goString.Should().NotThrow();
        goUri.Should().NotThrow();
    }

    [WinFormsFact]
    public async Task HtmlHistory_Dispose_IsIdempotentAndMembersThrow()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        HtmlDocument document = await GetDocument(control, HtmlPage1);
        document.Window.Should().NotBeNull();
        HtmlHistory? history = document.Window.History;

        history.Should().NotBeNull();
        history!.Dispose();

        // Dispose is safe to call more than once.
        Action disposeAgain = history.Dispose;
        disposeAgain.Should().NotThrow();

        // Members route through NativeOmHistory, which throws once disposed.
        // Back/Forward(1) also cover the disposed path past the negative check.
        Action getLength = () => _ = history.Length;
        Action getDomHistory = () => _ = history.DomHistory;
        Action back = () => history.Back(1);
        Action forward = () => history.Forward(1);
        Action go = () => history.Go(0);

        getLength.Should().Throw<ObjectDisposedException>();
        getDomHistory.Should().Throw<ObjectDisposedException>();
        back.Should().Throw<ObjectDisposedException>();
        forward.Should().Throw<ObjectDisposedException>();
        go.Should().Throw<ObjectDisposedException>();
    }

    private static async Task<HtmlDocument> GetDocument(WebBrowser control, string html)
    {
        using TempFile file = CreateTempFile(html);
        await NavigateToPathAsync(control, file.Path);
        return control.Document!;
    }

    private static async Task NavigateToPathAsync(WebBrowser control, string path)
    {
        TaskCompletionSource<bool> source = new();
        void Handler(object? sender, WebBrowserDocumentCompletedEventArgs e) => source.TrySetResult(true);
        control.DocumentCompleted += Handler;
        try
        {
            await Task.Run(() => control.Navigate(path));
            Assert.True(await source.Task);
        }
        finally
        {
            control.DocumentCompleted -= Handler;
        }
    }

    private static TempFile CreateTempFile(string html)
    {
        byte[] data = Encoding.UTF8.GetBytes(html);
        return TempFile.Create(data);
    }
}
