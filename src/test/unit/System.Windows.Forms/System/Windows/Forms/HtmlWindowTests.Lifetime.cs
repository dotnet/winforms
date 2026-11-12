// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Moq;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms.Tests;

/// <summary>
///  Covers the manager-owned lifetime of HTML subscriptions using local MSHTML documents.
/// </summary>
public partial class HtmlWindowTests
{
    private const string LifetimeHtml = "<html><body><button id='button'>Click</button></body></html>";

    [StaFact]
    public unsafe void HtmlWindow_Lifetime_UnadvisableSource_DoesNotThrowOnSubscription()
    {
        using HtmlShimManager manager = new();
        Mock<IHTMLWindow2.Interface> source = new();
        HtmlWindow window = new(manager, ComHelpers.GetComPointer<IHTMLWindow2>(source.Object));
        try
        {
            HtmlElementEventHandler handler = (_, _) => { };
            window.Resize += handler;
            window.Resize -= handler;
            HtmlWindow.HtmlWindowShim shim = Assert.IsType<HtmlWindow.HtmlWindowShim>(manager.GetWindowShim(window));

            Assert.Null(shim.TestAccessor.Dynamic._cookie);
            Assert.False(shim.IsDisposed);
            manager.Dispose();
            Assert.True(shim.IsDisposed);
        }
        finally
        {
            window.NativeHtmlWindow.Dispose();
        }
    }

    [StaFact]
    public unsafe void HtmlWindow_Lifetime_UnsubscribeDisposedWrapper_DoesNotQueryItOrRemoveReplacementHandler()
    {
        using HtmlShimManager manager = new();
        Mock<IHTMLWindow2.Interface> source = new();
        HtmlWindow oldWindow = new(manager, ComHelpers.GetComPointer<IHTMLWindow2>(source.Object));
        HtmlElementEventHandler handler = (_, _) => { };
        oldWindow.Resize += handler;
        manager.OnWindowUnloaded(oldWindow);

        // MSHTML can reuse a window's COM identity across navigation. The disposed wrapper has the
        // same dictionary hash as the replacement, but its revoked registration must not be queried.
        HtmlWindow replacement = new(manager, ComHelpers.GetComPointer<IHTMLWindow2>(source.Object));
        replacement.Resize += handler;
        oldWindow.Resize -= handler;
        oldWindow.DetachEventHandler("onresize", (_, _) => { });

        HtmlWindow.HtmlWindowShim shim = manager.GetWindowShim(replacement)!;
        EventHandlerList handlers = shim.TestAccessor.Dynamic._events;
        Assert.NotNull(handlers[HtmlWindow.s_eventResize]);
        Assert.False(shim.IsDisposed);
    }

    [WinFormsTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task HtmlWindow_Lifetime_UnsubscribeAfterTeardown_DoesNotCreateShims(bool navigate, bool subscribe)
    {
        using Control parent = new();
        using WebBrowser browser = new() { Parent = parent };
        HtmlDocument document = await NavigateLifetimeDocument(browser, LifetimeHtml);
        HtmlElement button = document.GetElementById("button")!;
        HtmlWindow window = document.Window!;
        HtmlShimManager manager = document.TestAccessor.Dynamic._shimManager;
        HtmlElementEventHandler standard = (_, _) => { };
        EventHandler attached = (_, _) => { };

        Unsubscribe();
        Assert.Null(manager.TestAccessor.Dynamic._htmlDocumentShims);
        Assert.Null(manager.TestAccessor.Dynamic._htmlElementShims);
        Assert.Null(manager.TestAccessor.Dynamic._htmlWindowShims);

        if (subscribe)
        {
            document.Click += standard;
            button.Click += standard;
            window.Resize += standard;
            document.AttachEventHandler("onclick", attached);
            button.AttachEventHandler("onclick", attached);
            window.AttachEventHandler("onresize", attached);
        }

        if (navigate)
        {
            await NavigateLifetimeDocument(browser, LifetimeHtml);
        }
        else
        {
            browser.Disposed += (_, _) => Unsubscribe();
            browser.Dispose();
        }

        Unsubscribe();
        Unsubscribe();
        Assert.Null(manager.GetDocumentShim(document));
        Assert.Null(manager.GetElementShim(button));
        Assert.Null(manager.GetWindowShim(window));

        void Unsubscribe()
        {
            document.Click -= standard;
            button.Click -= standard;
            window.Resize -= standard;
            document.DetachEventHandler("onclick", attached);
            button.DetachEventHandler("onclick", attached);
            window.DetachEventHandler("onresize", attached);
        }
    }

    [WinFormsTheory]
    [InlineData(false, false, false)]
    [InlineData(false, false, true)]
    [InlineData(false, true, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(true, true, true)]
    public async Task HtmlWindow_Lifetime_NavigationWithoutUnloadHandler_PrunesSubscriptions(
        bool elementEvents,
        bool attachedEvents,
        bool windowFirst)
    {
        using Control parent = new();
        using WebBrowser browser = new() { Parent = parent };
        HtmlDocument document = await NavigateLifetimeDocument(browser, LifetimeHtml);

        for (int navigation = 0; navigation < 3; navigation++)
        {
            HtmlElement button = document.GetElementById("button")!;
            HtmlWindow window = document.Window!;
            HtmlShimManager manager = document.TestAccessor.Dynamic._shimManager;
            HtmlElementEventHandler standardHandler = (_, _) => { };
            EventHandler attachedHandler = (_, _) => { };

            if (windowFirst)
            {
                window.Resize += standardHandler;
                window.Resize -= standardHandler;
            }

            if (elementEvents)
            {
                if (attachedEvents)
                {
                    button.AttachEventHandler("onclick", attachedHandler);
                }
                else
                {
                    button.Click += standardHandler;
                }
            }
            else if (attachedEvents)
            {
                document.AttachEventHandler("onclick", attachedHandler);
            }
            else
            {
                document.Click += standardHandler;
            }

            HtmlShim shim = elementEvents
                ? manager.GetElementShim(button)!
                : manager.GetDocumentShim(document)!;
            HtmlWindow.HtmlWindowShim windowShim = manager.GetWindowShim(window)!;
            AxHost.ConnectionPointCookie cookie = windowShim.TestAccessor.Dynamic._cookie;
            AgileComPointer<IHTMLWindow2> associatedWindow = shim.TestAccessor.Dynamic._associatedWindow;
            Assert.True(cookie.Connected);
            Assert.NotEqual(0u, (uint)associatedWindow.TestAccessor.Dynamic._cookie);

            HtmlDocument nextDocument = await NavigateLifetimeDocument(browser, LifetimeHtml);

            Assert.True(shim.IsDisposed);
            Assert.True(windowShim.IsDisposed);
            Assert.False(cookie.Connected);
            Assert.Null(shim.TestAccessor.Dynamic._associatedWindow);
            Assert.Equal(0u, (uint)associatedWindow.TestAccessor.Dynamic._cookie);
            Assert.Null(manager.GetDocumentShim(document));
            Assert.Null(manager.GetElementShim(button));
            Assert.Null(manager.GetWindowShim(window));
            Assert.NotNull(nextDocument.GetElementById("button"));
            GC.KeepAlive(associatedWindow);
            document = nextDocument;
        }
    }

    [WinFormsFact]
    public async Task HtmlWindow_Lifetime_LastApplicationHandlerRemoved_PreservesUnloadObservation()
    {
        using Control parent = new();
        using WebBrowser browser = new() { Parent = parent };
        HtmlDocument document = await NavigateLifetimeDocument(browser, LifetimeHtml);
        HtmlWindow window = document.Window!;
        HtmlShimManager manager = document.TestAccessor.Dynamic._shimManager;
        HtmlElementEventHandler handler = (_, _) => { };
        document.Click += handler;
        HtmlDocument.HtmlDocumentShim documentShim = manager.GetDocumentShim(document)!;
        window.Resize += handler;
        window.Resize -= handler;

        await NavigateLifetimeDocument(browser, LifetimeHtml);

        Assert.True(documentShim.IsDisposed);
        Assert.Null(manager.GetDocumentShim(document));
    }

    [WinFormsFact]
    public async Task HtmlWindow_Lifetime_Dispose_ClearsAllOwnersAndRegistrations()
    {
        using Control parent = new();
        using WebBrowser browser = new() { Parent = parent };
        HtmlDocument document = await NavigateLifetimeDocument(browser, LifetimeHtml);
        HtmlElement button = document.GetElementById("button")!;
        HtmlWindow window = document.Window!;
        HtmlShimManager manager = document.TestAccessor.Dynamic._shimManager;
        EventHandler attached = (_, _) => { };
        document.AttachEventHandler("onclick", attached);
        button.AttachEventHandler("onclick", attached);
        window.AttachEventHandler("onresize", attached);
        HtmlDocument.HtmlDocumentShim documentShim = manager.GetDocumentShim(document)!;
        HtmlElement.HtmlElementShim elementShim = manager.GetElementShim(button)!;
        HtmlWindow.HtmlWindowShim windowShim = manager.GetWindowShim(window)!;
        AgileComPointer<IHTMLWindow2> documentWindow = documentShim.TestAccessor.Dynamic._associatedWindow;
        AgileComPointer<IHTMLWindow2> elementWindow = elementShim.TestAccessor.Dynamic._associatedWindow;

        browser.Dispose();
        manager.Dispose();
        documentShim.Dispose();
        elementShim.Dispose();
        windowShim.Dispose();

        Assert.Null(manager.TestAccessor.Dynamic._htmlDocumentShims);
        Assert.Null(manager.TestAccessor.Dynamic._htmlElementShims);
        Assert.Null(manager.TestAccessor.Dynamic._htmlWindowShims);
        Assert.Null(documentShim.TestAccessor.Dynamic._attachedEventList);
        Assert.Null(elementShim.TestAccessor.Dynamic._attachedEventList);
        Assert.Null(windowShim.TestAccessor.Dynamic._attachedEventList);
        Assert.Equal(0u, (uint)documentWindow.TestAccessor.Dynamic._cookie);
        Assert.Equal(0u, (uint)elementWindow.TestAccessor.Dynamic._cookie);
        Assert.True(documentShim.IsDisposed);
        Assert.True(elementShim.IsDisposed);
        Assert.True(windowShim.IsDisposed);
        GC.KeepAlive(documentWindow);
        GC.KeepAlive(elementWindow);
    }

    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task HtmlWindow_Lifetime_AttachedDuplicatesAndNames_DetachIndependently(bool documentEvents)
    {
        using Control parent = new();
        using WebBrowser browser = new() { Parent = parent };
        HtmlDocument document = await NavigateLifetimeDocument(browser, LifetimeHtml);
        HtmlElement button = document.GetElementById("button")!;
        int calls = 0;
        EventHandler handler = (_, _) => calls++;
        Action<string, EventHandler> attach = documentEvents ? document.AttachEventHandler : button.AttachEventHandler;
        Action<string, EventHandler> detach = documentEvents ? document.DetachEventHandler : button.DetachEventHandler;

        attach("onclick", handler);
        attach("onclick", handler);
        attach("onmouseover", handler);
        button.InvokeMember("click");
        Assert.Equal(2, calls);

        detach("onclick", handler);
        button.InvokeMember("click");
        Assert.Equal(3, calls);

        detach("onmouseover", handler);
        button.InvokeMember("click");
        Assert.Equal(4, calls);

        detach("onclick", handler);
        button.InvokeMember("click");
        Assert.Equal(4, calls);
    }

    [WinFormsFact]
    public async Task HtmlWindow_Lifetime_MixedEventApis_RemovingStandardHandlerKeepsAttachedHandler()
    {
        using Control parent = new();
        using WebBrowser browser = new() { Parent = parent };
        HtmlDocument document = await NavigateLifetimeDocument(browser, LifetimeHtml);
        HtmlElement button = document.GetElementById("button")!;
        int calls = 0;
        HtmlElementEventHandler standard = (_, _) => { };
        EventHandler attached = (_, _) => calls++;
        button.Click += standard;
        button.AttachEventHandler("onclick", attached);

        button.Click -= standard;
        button.InvokeMember("click");

        Assert.Equal(1, calls);
        button.DetachEventHandler("onclick", attached);
    }

    [WinFormsFact]
    public async Task HtmlWindow_Lifetime_UnloadHandlerDisposesBrowser_DoesNotRecreateShims()
    {
        using Control parent = new();
        using WebBrowser browser = new() { Parent = parent };
        HtmlDocument document = await NavigateLifetimeDocument(browser, LifetimeHtml);
        HtmlWindow window = document.Window!;
        HtmlShimManager manager = document.TestAccessor.Dynamic._shimManager;
        TaskCompletionSource completed = new(TaskCreationOptions.RunContinuationsAsynchronously);
        window.Unload += (_, _) =>
        {
            browser.Dispose();
            completed.TrySetResult();
        };
        HtmlWindow.HtmlWindowShim shim = manager.GetWindowShim(window)!;
        using TempFile replacement = TempFile.Create(Encoding.UTF8.GetBytes(LifetimeHtml));

        browser.Navigate(replacement.Path);
        await completed.Task.WaitAsync(TimeSpan.FromSeconds(30));

        Assert.True(browser.IsDisposed);
        Assert.True(shim.IsDisposed);
        Assert.Null(manager.TestAccessor.Dynamic._htmlWindowShims);
        Assert.Null(manager.GetWindowShim(window));
    }

    [WinFormsFact]
    public async Task HtmlWindow_Lifetime_DetachFailure_DoesNotSkipBrowserDisposal()
    {
        using Control parent = new();
        using WebBrowser browser = new() { Parent = parent };
        HtmlDocument document = await NavigateLifetimeDocument(browser, LifetimeHtml);
        HtmlShimManager manager = document.TestAccessor.Dynamic._shimManager;
        ThrowingDocumentShim shim = new(document);
        shim.AttachEventHandler("onclick", (_, _) => { });
        Assert.Null(manager.TestAccessor.Dynamic._htmlDocumentShims);
        manager.TestAccessor.Dynamic._htmlDocumentShims =
            new Dictionary<HtmlDocument, HtmlDocument.HtmlDocumentShim> { [document] = shim };
        IntPtr handle = browser.Handle;

        COMException exception = Assert.Throws<COMException>(browser.Dispose);

        Assert.Equal((int)HRESULT.E_FAIL, exception.HResult);
        Assert.True(browser.IsDisposed);
        Assert.False(PInvoke.IsWindow((HWND)handle));
        Assert.True(shim.IsDisposed);
        Assert.Null(shim.AssociatedWindow);
        Assert.Null(manager.TestAccessor.Dynamic._htmlDocumentShims);
    }

    [WinFormsFact]
    public async Task HtmlWindow_Lifetime_NavigatingFrame_PreservesSiblingSubscriptions()
    {
        using Control parent = new();
        using WebBrowser browser = new() { Parent = parent };
        using TempFile leftFile = TempFile.Create(Encoding.UTF8.GetBytes(LifetimeHtml));
        using TempFile rightFile = TempFile.Create(Encoding.UTF8.GetBytes(LifetimeHtml));
        HtmlDocument root = await NavigateLifetimeDocument(
            browser,
            $"<html><frameset cols='50%,50%'><frame src='{new Uri(leftFile.Path).AbsoluteUri}'><frame src='{new Uri(rightFile.Path).AbsoluteUri}'></frameset></html>");
        HtmlWindow rootWindow = Assert.IsType<HtmlWindow>(root.Window);
        HtmlWindowCollection frames = Assert.IsType<HtmlWindowCollection>(rootWindow.Frames);
        HtmlWindow left = Assert.IsType<HtmlWindow>(frames[0]);
        HtmlWindow right = Assert.IsType<HtmlWindow>(frames[1]);
        HtmlDocument leftDocument = left.Document!;
        HtmlDocument rightDocument = right.Document!;
        int rightCalls = 0;
        leftDocument.Click += (_, _) => { };
        rightDocument.Click += (_, _) => rightCalls++;
        HtmlShimManager manager = root.TestAccessor.Dynamic._shimManager;
        HtmlDocument.HtmlDocumentShim leftShim = manager.GetDocumentShim(leftDocument)!;
        HtmlDocument.HtmlDocumentShim rightShim = manager.GetDocumentShim(rightDocument)!;
        using TempFile replacement = TempFile.Create(Encoding.UTF8.GetBytes(LifetimeHtml));

        await WaitForLifetimeNavigation(browser, new Uri(replacement.Path), () => left.Navigate(replacement.Path));

        Assert.True(leftShim.IsDisposed);
        Assert.False(rightShim.IsDisposed);
        Assert.Same(rightShim, manager.GetDocumentShim(rightDocument));
        rightDocument.GetElementById("button")!.InvokeMember("click");
        Assert.Equal(1, rightCalls);
    }

    private static async Task<HtmlDocument> NavigateLifetimeDocument(WebBrowser browser, string html)
    {
        using TempFile file = TempFile.Create(Encoding.UTF8.GetBytes(html));
        await WaitForLifetimeNavigation(browser, new Uri(file.Path), () => browser.Navigate(file.Path));
        return browser.Document!;
    }

    private static async Task WaitForLifetimeNavigation(WebBrowser browser, Uri expected, Action navigate)
    {
        TaskCompletionSource completed = new(TaskCreationOptions.RunContinuationsAsynchronously);
        WebBrowserDocumentCompletedEventHandler handler = (_, e) =>
        {
            if (e.Url == expected)
            {
                completed.TrySetResult();
            }
        };

        browser.DocumentCompleted += handler;
        try
        {
            navigate();
            await completed.Task.WaitAsync(TimeSpan.FromSeconds(30));
        }
        finally
        {
            browser.DocumentCompleted -= handler;
        }
    }

    /// <summary>
    ///  Injects a teardown error after detaching the real native registration so the test leaves no sink behind.
    /// </summary>
    private sealed class ThrowingDocumentShim : HtmlDocument.HtmlDocumentShim
    {
        public ThrowingDocumentShim(HtmlDocument document) : base(document)
        {
        }

        protected override void DetachEventProxy(HtmlToClrEventProxy proxy)
        {
            base.DetachEventProxy(proxy);
            HRESULT.E_FAIL.ThrowOnFailure();
        }
    }
}
