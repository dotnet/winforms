// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Text;
using Windows.Win32.System.Variant;
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

    [WinFormsFact]
    public async Task HtmlWindow_Unload_NavigateToLocalPage_Ownership_DomEvents_Navigation()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string PageA = "<html><head><title>C6 Navigation A</title></head>"
            + "<body><div id=\"eventTarget\">Page A</div></body></html>";
        HtmlDocument oldDocument = await GetDocument(control, PageA, TimeSpan.FromSeconds(15));
        HtmlWindow oldWindow = oldDocument.Window;
        HtmlElement oldElement = oldDocument.GetElementById("eventTarget");
        Assert.Equal("C6 Navigation A", oldDocument.Title);
        Assert.Equal("Page A", oldElement.InnerText);

        int documentClickCount = 0;
        int elementClickCount = 0;
        int unloadCount = 0;
        object unloadSender = null;
        EventArgs unloadEventArgs = null;
        List<string> callbackOrder = [];
        TaskCompletionSource<bool> unloadSource = new();
        HtmlElementEventHandler documentClickHandler = (sender, eventArgs) => documentClickCount++;
        HtmlElementEventHandler elementClickHandler = (sender, eventArgs) => elementClickCount++;
        HtmlElementEventHandler unloadHandler = (sender, eventArgs) =>
        {
            unloadCount++;
            unloadSender = sender;
            unloadEventArgs = eventArgs;
            callbackOrder.Add("Unload");
            unloadSource.TrySetResult(true);
        };

        oldWindow.Unload += unloadHandler;
        oldDocument.Click += documentClickHandler;
        oldElement.Click += elementClickHandler;

        HtmlWindow.HtmlWindowShim oldWindowShim = oldWindow.TestAccessor.Dynamic.WindowShim;
        HtmlDocument.HtmlDocumentShim oldDocumentShim = oldDocument.TestAccessor.Dynamic.DocumentShim;
        HtmlElement.HtmlElementShim oldElementShim = oldElement.TestAccessor.Dynamic.ElementShim;
        AgileComPointer<IHTMLWindow2> documentAssociatedWindow = oldDocumentShim.TestAccessor.Dynamic._associatedWindow;
        AgileComPointer<IHTMLWindow2> elementAssociatedWindow = oldElementShim.TestAccessor.Dynamic._associatedWindow;
        Assert.NotNull(documentAssociatedWindow);
        Assert.NotNull(elementAssociatedWindow);
        Assert.NotSame(documentAssociatedWindow, elementAssociatedWindow);
        Assert.NotSame(oldWindow.NativeHtmlWindow, documentAssociatedWindow);
        Assert.NotSame(oldWindow.NativeHtmlWindow, elementAssociatedWindow);
        Assert.NotEqual(0u, (uint)documentAssociatedWindow.TestAccessor.Dynamic._cookie);
        Assert.NotEqual(0u, (uint)elementAssociatedWindow.TestAccessor.Dynamic._cookie);
        AxHost.ConnectionPointCookie oldWindowCookie = oldWindowShim.TestAccessor.Dynamic._cookie;
        AxHost.ConnectionPointCookie oldDocumentCookie = oldDocumentShim.TestAccessor.Dynamic._cookie;
        AxHost.ConnectionPointCookie oldElementCookie = oldElementShim.TestAccessor.Dynamic._cookie;
        HtmlShimManager shimManager = oldDocument.TestAccessor.Dynamic._shimManager;
        Dictionary<HtmlWindow, HtmlWindow.HtmlWindowShim> windowShims =
            shimManager.TestAccessor.Dynamic._htmlWindowShims;
        Dictionary<HtmlDocument, HtmlDocument.HtmlDocumentShim> documentShims =
            shimManager.TestAccessor.Dynamic._htmlDocumentShims;
        Dictionary<HtmlElement, HtmlElement.HtmlElementShim> elementShims =
            shimManager.TestAccessor.Dynamic._htmlElementShims;
        Assert.NotNull(oldWindowCookie);
        Assert.NotNull(oldDocumentCookie);
        Assert.NotNull(oldElementCookie);
        Assert.True(oldWindowCookie.Connected);
        Assert.True(oldDocumentCookie.Connected);
        Assert.True(oldElementCookie.Connected);
        Assert.True(windowShims.ContainsValue(oldWindowShim));
        Assert.True(documentShims.ContainsValue(oldDocumentShim));
        Assert.True(elementShims.ContainsValue(oldElementShim));

        const string PageB = "<html><head><title>C6 Navigation B</title></head>"
            + "<body><p>Page B</p></body></html>";
        using var pageBFile = CreateTempFile(PageB);
        Uri pageBUri = new(pageBFile.Path);
        int pageBCompletedCount = 0;
        TaskCompletionSource<bool> pageBCompletedSource = new();
        WebBrowserDocumentCompletedEventHandler pageBCompletedHandler = (sender, eventArgs) =>
        {
            if (eventArgs.Url == pageBUri)
            {
                pageBCompletedCount++;
                callbackOrder.Add("DocumentCompleted");
                pageBCompletedSource.TrySetResult(true);
            }
        };

        control.DocumentCompleted += pageBCompletedHandler;
        try
        {
            await Task.Run(() => control.Navigate(pageBFile.Path));
            await Task.WhenAll(
                unloadSource.Task.WaitAsync(TimeSpan.FromSeconds(15)),
                pageBCompletedSource.Task.WaitAsync(TimeSpan.FromSeconds(15)));
        }
        finally
        {
            control.DocumentCompleted -= pageBCompletedHandler;
        }

        Assert.Equal(1, unloadCount);
        Assert.Same(oldWindow, unloadSender);
        Assert.IsType<HtmlElementEventArgs>(unloadEventArgs);
        Assert.Equal(1, pageBCompletedCount);
        Assert.Equal(["Unload", "DocumentCompleted"], callbackOrder);
        Assert.Equal(0, documentClickCount);
        Assert.Equal(0, elementClickCount);

        HtmlDocument newDocument = control.Document;
        Assert.Equal("C6 Navigation B", newDocument.Title);

        int newDocumentClickCount = 0;
        object newDocumentClickSender = null;
        EventArgs newDocumentClickEventArgs = null;
        HtmlElementEventHandler newDocumentClickHandler = (sender, eventArgs) =>
        {
            newDocumentClickCount++;
            newDocumentClickSender = sender;
            newDocumentClickEventArgs = eventArgs;
        };

        newDocument.Click += newDocumentClickHandler;
        HtmlDocument.HtmlDocumentShim newDocumentShim = newDocument.TestAccessor.Dynamic.DocumentShim;
        AxHost.ConnectionPointCookie newDocumentCookie = newDocumentShim.TestAccessor.Dynamic._cookie;
        Assert.NotNull(newDocumentCookie);
        Assert.True(newDocumentCookie.Connected);
        FireDocumentClick(newDocument);
        Assert.Equal(1, newDocumentClickCount);
        Assert.Equal(newDocument, newDocumentClickSender);
        Assert.IsType<HtmlElementEventArgs>(newDocumentClickEventArgs);

        newDocument.Click -= newDocumentClickHandler;
        newDocumentClickSender = null;
        newDocumentClickEventArgs = null;
        FireDocumentClick(newDocument);
        Assert.Equal(1, newDocumentClickCount);
        Assert.Null(newDocumentClickSender);
        Assert.Null(newDocumentClickEventArgs);

        bool oldWindowRetained = windowShims.ContainsValue(oldWindowShim);
        bool oldDocumentRetained = documentShims.ContainsValue(oldDocumentShim);
        bool oldElementRetained = elementShims.ContainsValue(oldElementShim);
        bool oldWindowCookieConnected = oldWindowCookie.Connected;
        bool oldDocumentCookieConnected = oldDocumentCookie.Connected;
        bool oldElementCookieConnected = oldElementCookie.Connected;
        bool oldWindowShimCookieRetained = oldWindowShim.TestAccessor.Dynamic._cookie is not null;
        bool oldDocumentShimCookieRetained = oldDocumentShim.TestAccessor.Dynamic._cookie is not null;
        bool oldElementShimCookieRetained = oldElementShim.TestAccessor.Dynamic._cookie is not null;
        bool oldWindowWrapperRetained = oldWindowShim.TestAccessor.Dynamic._htmlWindow is not null;
        bool oldDocumentWrapperRetained = oldDocumentShim.TestAccessor.Dynamic._htmlDocument is not null;
        bool oldElementWrapperRetained = oldElementShim.TestAccessor.Dynamic._htmlElement is not null;
        string cleanupState = $"manager(window={oldWindowRetained}, document={oldDocumentRetained}, "
            + $"element={oldElementRetained}); cookies(window={oldWindowCookieConnected}, "
            + $"document={oldDocumentCookieConnected}, element={oldElementCookieConnected}); "
            + $"shim-cookies(window={oldWindowShimCookieRetained}, document={oldDocumentShimCookieRetained}, "
            + $"element={oldElementShimCookieRetained}); wrappers(window={oldWindowWrapperRetained}, "
            + $"document={oldDocumentWrapperRetained}, element={oldElementWrapperRetained})";
        Assert.False(
            oldWindowRetained
                || oldDocumentRetained
                || oldElementRetained
                || oldWindowCookieConnected
                || oldDocumentCookieConnected
                || oldElementCookieConnected
                || oldWindowShimCookieRetained
                || oldDocumentShimCookieRetained
                || oldElementShimCookieRetained
                || oldWindowWrapperRetained
                || oldDocumentWrapperRetained
                || oldElementWrapperRetained,
            cleanupState);

        uint[] remainingWindowRegistrations =
        [
            (uint)documentAssociatedWindow.TestAccessor.Dynamic._cookie,
            (uint)elementAssociatedWindow.TestAccessor.Dynamic._cookie
        ];
        Assert.Equal([0u, 0u], remainingWindowRegistrations);
        Assert.Null(oldDocumentShim.TestAccessor.Dynamic._associatedWindow);
        Assert.Null(oldElementShim.TestAccessor.Dynamic._associatedWindow);

        GC.KeepAlive(oldWindowCookie);
        GC.KeepAlive(oldDocumentCookie);
        GC.KeepAlive(oldElementCookie);
        GC.KeepAlive(oldWindowShim);
        GC.KeepAlive(oldDocumentShim);
        GC.KeepAlive(oldElementShim);
        GC.KeepAlive(oldWindow);
        GC.KeepAlive(oldDocument);
        GC.KeepAlive(oldElement);

        static unsafe void FireDocumentClick(HtmlDocument document)
        {
            using var document2 = document.NativeHtmlDocument2.GetInterface();
            using var document4 = document2.Query<IHTMLDocument4>();
            using BSTR onClick = new("onclick");
            VARIANT eventObject = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(document4.Value->fireEvent(onClick, &eventObject, &cancelled).Succeeded);
        }
    }

    [WinFormsFact]
    public async Task HtmlWindow_Unload_NavigateFrame_Ownership_DomEvents_PreservesSibling()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        using var departingPage = CreateTempFile(
            "<html><head><title>Departing</title></head><body><div id='target'>Departing</div></body></html>");
        using var siblingPage = CreateTempFile(
            "<html><head><title>Sibling</title></head><body><div id='target'>Sibling</div></body></html>");
        using var replacementPage = CreateTempFile(
            "<html><head><title>Replacement</title></head><body>Replacement</body></html>");
        Uri departingUri = new(departingPage.Path);
        Uri siblingUri = new(siblingPage.Path);
        Uri replacementUri = new(replacementPage.Path);
        using var parentPage = CreateTempFile($"""
            <html><frameset cols="50%,50%">
                <frame name="departing" src="{departingUri.AbsoluteUri}">
                <frame name="sibling" src="{siblingUri.AbsoluteUri}">
            </frameset></html>
            """);
        Uri parentUri = new(parentPage.Path);
        await NavigateAndWait(control, parentUri, () => control.Navigate(parentUri));

        HtmlWindow parentWindow = control.Document.Window;
        HtmlWindow departingWindow = parentWindow.Frames[0];
        HtmlWindow siblingWindow = parentWindow.Frames[1];
        HtmlDocument departingDocument = departingWindow.Document;
        HtmlDocument siblingDocument = siblingWindow.Document;
        HtmlElement departingElement = departingDocument.GetElementById("target");
        HtmlElement siblingElement = siblingDocument.GetElementById("target");
        Assert.Equal("Departing", departingDocument.Title);
        Assert.Equal("Sibling", siblingDocument.Title);

        int departingUnloadCount = 0;
        int siblingUnloadCount = 0;
        int parentUnloadCount = 0;
        int siblingDocumentClickCount = 0;
        int siblingElementClickCount = 0;
        TaskCompletionSource<bool> unloaded = new();
        departingWindow.Unload += (sender, eventArgs) =>
        {
            departingUnloadCount++;
            unloaded.TrySetResult(true);
        };
        siblingWindow.Unload += (sender, eventArgs) => siblingUnloadCount++;
        parentWindow.Unload += (sender, eventArgs) => parentUnloadCount++;
        departingDocument.Click += (sender, eventArgs) => { };
        departingElement.Click += (sender, eventArgs) => { };
        siblingDocument.Click += (sender, eventArgs) => siblingDocumentClickCount++;
        siblingElement.Click += (sender, eventArgs) => siblingElementClickCount++;

        HtmlShimManager manager = departingDocument.TestAccessor.Dynamic._shimManager;
        HtmlDocument.HtmlDocumentShim departingDocumentShim = departingDocument.TestAccessor.Dynamic.DocumentShim;
        HtmlElement.HtmlElementShim departingElementShim = departingElement.TestAccessor.Dynamic.ElementShim;
        HtmlDocument.HtmlDocumentShim siblingDocumentShim = siblingDocument.TestAccessor.Dynamic.DocumentShim;
        HtmlElement.HtmlElementShim siblingElementShim = siblingElement.TestAccessor.Dynamic.ElementShim;
        AxHost.ConnectionPointCookie departingDocumentCookie = departingDocumentShim.TestAccessor.Dynamic._cookie;
        AxHost.ConnectionPointCookie departingElementCookie = departingElementShim.TestAccessor.Dynamic._cookie;
        AxHost.ConnectionPointCookie siblingDocumentCookie = siblingDocumentShim.TestAccessor.Dynamic._cookie;
        AxHost.ConnectionPointCookie siblingElementCookie = siblingElementShim.TestAccessor.Dynamic._cookie;
        Dictionary<HtmlDocument, HtmlDocument.HtmlDocumentShim> documentShims = manager.TestAccessor.Dynamic._htmlDocumentShims;
        Dictionary<HtmlElement, HtmlElement.HtmlElementShim> elementShims = manager.TestAccessor.Dynamic._htmlElementShims;
        Assert.True(departingDocumentCookie.Connected);
        Assert.True(departingElementCookie.Connected);
        Assert.True(siblingDocumentCookie.Connected);
        Assert.True(siblingElementCookie.Connected);
        Assert.True(documentShims.ContainsValue(departingDocumentShim));
        Assert.True(elementShims.ContainsValue(departingElementShim));
        Assert.True(documentShims.ContainsValue(siblingDocumentShim));
        Assert.True(elementShims.ContainsValue(siblingElementShim));

        await NavigateAndWait(control, replacementUri, () => departingWindow.Navigate(replacementUri));
        await unloaded.Task.WaitAsync(TimeSpan.FromSeconds(15));

        Assert.Equal(1, departingUnloadCount);
        Assert.Equal(0, siblingUnloadCount);
        Assert.Equal(0, parentUnloadCount);
        Assert.False(documentShims.ContainsValue(departingDocumentShim));
        Assert.False(elementShims.ContainsValue(departingElementShim));
        Assert.False(departingDocumentCookie.Connected);
        Assert.False(departingElementCookie.Connected);
        Assert.True(documentShims.ContainsValue(siblingDocumentShim));
        Assert.True(elementShims.ContainsValue(siblingElementShim));
        Assert.True(siblingDocumentCookie.Connected);
        Assert.True(siblingElementCookie.Connected);
        Assert.Equal("Replacement", parentWindow.Frames[0].Document.Title);
        Assert.Equal("Sibling", siblingDocument.Title);
        Assert.Equal("Sibling", siblingElement.InnerText);

        FireSiblingClicks(siblingDocument, siblingElement);
        Assert.True(siblingDocumentClickCount > 0);
        Assert.Equal(1, siblingElementClickCount);

        GC.KeepAlive(departingDocumentCookie);
        GC.KeepAlive(departingElementCookie);
        GC.KeepAlive(departingWindow);
        GC.KeepAlive(departingDocument);
        GC.KeepAlive(departingElement);

        static async Task NavigateAndWait(WebBrowser browser, Uri target, Action navigate)
        {
            TaskCompletionSource<bool> completed = new();
            WebBrowserDocumentCompletedEventHandler handler = (sender, eventArgs) =>
            {
                if (eventArgs.Url == target)
                {
                    completed.TrySetResult(true);
                }
            };
            browser.DocumentCompleted += handler;
            try
            {
                navigate();
                await completed.Task.WaitAsync(TimeSpan.FromSeconds(15));
            }
            finally
            {
                browser.DocumentCompleted -= handler;
            }
        }

        static unsafe void FireSiblingClicks(HtmlDocument document, HtmlElement element)
        {
            using var nativeDocument = document.NativeHtmlDocument2.GetInterface<IHTMLDocument4>();
            using var nativeElement = element.NativeHtmlElement.GetInterface<IHTMLElement3>();
            using BSTR onClick = new("onclick");
            VARIANT eventObject = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(nativeDocument.Value->fireEvent(onClick, &eventObject, &cancelled).Succeeded);
            Assert.True(nativeElement.Value->fireEvent(onClick, &eventObject, &cancelled).Succeeded);
        }
    }

    [WinFormsFact]
    public async Task HtmlWindow_Unload_BrowserDispose_Ownership_DomEvents_Unload()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><head><title>C6 Dispose Control</title></head>"
            + "<body><div id=\"eventTarget\">Dispose Control</div></body></html>";
        HtmlDocument document = await GetDocument(control, Html, TimeSpan.FromSeconds(15));
        HtmlWindow window = document.Window;
        HtmlElement element = document.GetElementById("eventTarget");
        int callbackCount = 0;
        HtmlElementEventHandler handler = (sender, eventArgs) => callbackCount++;

        window.Unload += handler;
        document.Click += handler;
        element.Click += handler;

        HtmlWindow.HtmlWindowShim windowShim = window.TestAccessor.Dynamic.WindowShim;
        HtmlDocument.HtmlDocumentShim documentShim = document.TestAccessor.Dynamic.DocumentShim;
        HtmlElement.HtmlElementShim elementShim = element.TestAccessor.Dynamic.ElementShim;
        AgileComPointer<IHTMLWindow2> documentAssociatedWindow = documentShim.TestAccessor.Dynamic._associatedWindow;
        AgileComPointer<IHTMLWindow2> elementAssociatedWindow = elementShim.TestAccessor.Dynamic._associatedWindow;
        Assert.NotNull(documentAssociatedWindow);
        Assert.NotNull(elementAssociatedWindow);
        Assert.NotSame(documentAssociatedWindow, elementAssociatedWindow);
        Assert.NotSame(window.NativeHtmlWindow, documentAssociatedWindow);
        Assert.NotSame(window.NativeHtmlWindow, elementAssociatedWindow);
        Assert.NotEqual(0u, (uint)documentAssociatedWindow.TestAccessor.Dynamic._cookie);
        Assert.NotEqual(0u, (uint)elementAssociatedWindow.TestAccessor.Dynamic._cookie);
        AxHost.ConnectionPointCookie windowCookie = windowShim.TestAccessor.Dynamic._cookie;
        AxHost.ConnectionPointCookie documentCookie = documentShim.TestAccessor.Dynamic._cookie;
        AxHost.ConnectionPointCookie elementCookie = elementShim.TestAccessor.Dynamic._cookie;
        Assert.NotNull(windowCookie);
        Assert.NotNull(documentCookie);
        Assert.NotNull(elementCookie);
        Assert.True(windowCookie.Connected);
        Assert.True(documentCookie.Connected);
        Assert.True(elementCookie.Connected);

        control.Dispose();

        bool windowCookieConnected = windowCookie.Connected;
        bool documentCookieConnected = documentCookie.Connected;
        bool elementCookieConnected = elementCookie.Connected;
        bool windowShimCookieRetained = windowShim.TestAccessor.Dynamic._cookie is not null;
        bool documentShimCookieRetained = documentShim.TestAccessor.Dynamic._cookie is not null;
        bool elementShimCookieRetained = elementShim.TestAccessor.Dynamic._cookie is not null;
        bool windowWrapperRetained = windowShim.TestAccessor.Dynamic._htmlWindow is not null;
        bool documentWrapperRetained = documentShim.TestAccessor.Dynamic._htmlDocument is not null;
        bool elementWrapperRetained = elementShim.TestAccessor.Dynamic._htmlElement is not null;
        string cleanupState = $"cookies(window={windowCookieConnected}, document={documentCookieConnected}, "
            + $"element={elementCookieConnected}); shim-cookies(window={windowShimCookieRetained}, "
            + $"document={documentShimCookieRetained}, element={elementShimCookieRetained}); "
            + $"wrappers(window={windowWrapperRetained}, document={documentWrapperRetained}, "
            + $"element={elementWrapperRetained})";
        Assert.False(
            windowCookieConnected
                || documentCookieConnected
                || elementCookieConnected
                || windowShimCookieRetained
                || documentShimCookieRetained
                || elementShimCookieRetained
                || windowWrapperRetained
                || documentWrapperRetained
                || elementWrapperRetained,
            cleanupState);
        Assert.Equal(0, callbackCount);

        uint[] remainingWindowRegistrations =
        [
            (uint)documentAssociatedWindow.TestAccessor.Dynamic._cookie,
            (uint)elementAssociatedWindow.TestAccessor.Dynamic._cookie
        ];
        Assert.Equal([0u, 0u], remainingWindowRegistrations);
        Assert.Null(documentShim.TestAccessor.Dynamic._associatedWindow);
        Assert.Null(elementShim.TestAccessor.Dynamic._associatedWindow);

        documentShim.Dispose();
        elementShim.Dispose();
        Assert.Null(documentShim.AssociatedWindow);
        Assert.Null(elementShim.AssociatedWindow);

        GC.KeepAlive(windowCookie);
        GC.KeepAlive(documentCookie);
        GC.KeepAlive(elementCookie);
        GC.KeepAlive(windowShim);
        GC.KeepAlive(documentShim);
        GC.KeepAlive(elementShim);
        GC.KeepAlive(window);
        GC.KeepAlive(document);
        GC.KeepAlive(element);
    }

    private static async Task<HtmlDocument> GetDocument(WebBrowser control, string html, TimeSpan? timeout = null)
    {
        TaskCompletionSource<bool> source = new();
        WebBrowserDocumentCompletedEventHandler handler = (sender, eventArgs) => source.TrySetResult(true);
        control.DocumentCompleted += handler;

        try
        {
            using var file = CreateTempFile(html);
            await Task.Run(() => control.Navigate(file.Path));
            Assert.True(await (timeout.HasValue ? source.Task.WaitAsync(timeout.Value) : source.Task));

            return control.Document;
        }
        finally
        {
            control.DocumentCompleted -= handler;
        }
    }

    private static TempFile CreateTempFile(string html)
    {
        byte[] data = Encoding.UTF8.GetBytes(html);
        return TempFile.Create(data);
    }
}
