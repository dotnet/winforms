// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms;

public sealed unsafe partial class HtmlWindow
{
    internal static readonly object s_eventError = new();
    internal static readonly object s_eventGotFocus = new();
    internal static readonly object s_eventLoad = new();
    internal static readonly object s_eventLostFocus = new();
    internal static readonly object s_eventResize = new();
    internal static readonly object s_eventScroll = new();
    internal static readonly object s_eventUnload = new();

    private readonly HtmlShimManager _shimManager;
    private readonly AgileComPointer<IHTMLWindow2> _htmlWindow2;
    private readonly int _hashCode;

    internal HtmlWindow(HtmlShimManager shimManager, IHTMLWindow2* window)
    {
#if DEBUG
        _htmlWindow2 = new(window, takeOwnership: true, trackDisposal: false);
#else
        _htmlWindow2 = new(window, takeOwnership: true);
#endif
        Debug.Assert(NativeHtmlWindow is not null, "The window object should implement IHTMLWindow2");
        using var scope = _htmlWindow2.GetInterface<IUnknown>();
        _hashCode = HashCode.Combine((nint)scope.Value);

        _shimManager = shimManager;
    }

    internal AgileComPointer<IHTMLWindow2> NativeHtmlWindow => _htmlWindow2;

    /// <summary>
    ///  Helper method to get IHTMLWindowX interface of interest. Throws if failure occurs.
    /// </summary>
    private ComScope<T> GetHtmlWindow<T>() where T : unmanaged, IComIID
    {
        using var htmlWindow2 = NativeHtmlWindow.GetInterface();
        var scope = htmlWindow2.TryQuery<T>(out HRESULT hr);
        hr.ThrowOnFailure();
        return scope;
    }

    private HtmlShimManager ShimManager => _shimManager;

    private HtmlWindowShim WindowShim
    {
        get
        {
            HtmlWindowShim? shim = ShimManager.GetWindowShim(this);
            if (shim is null)
            {
                _shimManager.AddWindowShim(this);
                shim = ShimManager.GetWindowShim(this);
            }

            return shim!;
        }
    }

    public HtmlDocument? Document
    {
        get
        {
            using var htmlWindow = NativeHtmlWindow.GetInterface();
            using ComScope<IHTMLDocument2> htmlDoc2 = new(null);
            htmlWindow.Value->get_document(htmlDoc2).ThrowOnFailure();
            if (htmlDoc2.IsNull)
            {
                return null;
            }

            using var htmlDoc = htmlDoc2.TryQuery<IHTMLDocument>(out HRESULT hr);
            return hr.Succeeded ? new HtmlDocument(ShimManager, htmlDoc) : null;
        }
    }

    public object DomWindow => NativeHtmlWindow.GetManagedObject();

    public HtmlWindowCollection? Frames
    {
        get
        {
            using var htmlWindow = NativeHtmlWindow.GetInterface();
            IHTMLFramesCollection2* iHTMLFramesCollection2;
            htmlWindow.Value->get_frames(&iHTMLFramesCollection2).ThrowOnFailure();
            return iHTMLFramesCollection2 is not null ? new HtmlWindowCollection(ShimManager, iHTMLFramesCollection2) : null;
        }
    }

    public HtmlHistory? History
    {
        get
        {
            using var htmlWindow = NativeHtmlWindow.GetInterface();
            IOmHistory* iOmHistory;
            htmlWindow.Value->get_history(&iOmHistory).ThrowOnFailure();
            return iOmHistory is not null ? new HtmlHistory(iOmHistory) : null;
        }
    }

    public bool IsClosed
    {
        get
        {
            using var htmlWindow = NativeHtmlWindow.GetInterface();
            VARIANT_BOOL closed;
            htmlWindow.Value->get_closed(&closed).ThrowOnFailure();
            return closed;
        }
    }

    /// <summary>
    ///  Name of the NativeHtmlWindow
    /// </summary>
    public string Name
    {
        get
        {
            using var htmlWindow = NativeHtmlWindow.GetInterface();
            using BSTR name = default;
            htmlWindow.Value->get_name(&name).ThrowOnFailure();
            return name.ToString();
        }
        set
        {
            using var htmlWindow = NativeHtmlWindow.GetInterface();
            using BSTR newValue = new(value);
            htmlWindow.Value->put_name(newValue).ThrowOnFailure();
        }
    }

    public HtmlWindow? Opener
    {
        get
        {
            using var htmlWindow = NativeHtmlWindow.GetInterface();
            VARIANT variantDispatch = default;
            htmlWindow.Value->get_opener(&variantDispatch).ThrowOnFailure();
            using ComScope<IDispatch> dispatch = new(variantDispatch.data.pdispVal);
            IHTMLWindow2* htmlWindow2;
            return !dispatch.IsNull && dispatch.Value->QueryInterface(IID.Get<IHTMLWindow2>(), (void**)&htmlWindow2).Succeeded
                ? new HtmlWindow(ShimManager, htmlWindow2)
                : null;
        }
    }

    public HtmlWindow? Parent
    {
        get
        {
            using var htmlWindow = NativeHtmlWindow.GetInterface();
            IHTMLWindow2* iHTMLWindow2;
            htmlWindow.Value->get_parent(&iHTMLWindow2).ThrowOnFailure();
            return (iHTMLWindow2 is not null) ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
        }
    }

    public Point Position
    {
        get
        {
            using var htmlWindow3 = GetHtmlWindow<IHTMLWindow3>();
            int x;
            int y;
            htmlWindow3.Value->get_screenLeft(&x).ThrowOnFailure();
            htmlWindow3.Value->get_screenTop(&y).ThrowOnFailure();
            return new(x, y);
        }
    }

    /// <summary>
    ///  Gets or sets size for the window
    /// </summary>
    public Size Size
    {
        get
        {
            using var htmlWindow = NativeHtmlWindow.GetInterface();
            using ComScope<IHTMLDocument2> htmlDoc2 = new(null);
            htmlWindow.Value->get_document(htmlDoc2).ThrowOnFailure();
            using ComScope<IHTMLElement> bodyElement = new(null);
            htmlDoc2.Value->get_body(bodyElement).ThrowOnFailure();

            int offsetWidth;
            int offsetHeight;
            bodyElement.Value->get_offsetWidth(&offsetWidth).ThrowOnFailure();
            bodyElement.Value->get_offsetHeight(&offsetHeight).ThrowOnFailure();
            return new(offsetWidth, offsetHeight);
        }
        set
        {
            ResizeTo(value.Width, value.Height);
        }
    }

    public string StatusBarText
    {
        get
        {
            using var htmlWindow = NativeHtmlWindow.GetInterface();
            using BSTR status = default;
            htmlWindow.Value->get_status(&status).ThrowOnFailure();
            return status.ToString();
        }
        set
        {
            using var htmlWindow = NativeHtmlWindow.GetInterface();
            using BSTR newValue = new(value);
            htmlWindow.Value->put_status(newValue).ThrowOnFailure();
        }
    }

    public Uri? Url
    {
        get
        {
            using var htmlWindow = NativeHtmlWindow.GetInterface();
            using ComScope<IHTMLLocation> location = new(null);
            htmlWindow.Value->get_location(location).ThrowOnFailure();
            if (location.IsNull)
            {
                return null;
            }

            using BSTR href = default;
            location.Value->get_href(&href).ThrowOnFailure();
            string hrefString = href.ToString();
            return string.IsNullOrEmpty(hrefString) ? null : new(hrefString);
        }
    }

    public HtmlElement? WindowFrameElement
    {
        get
        {
            using var htmlWindow4 = GetHtmlWindow<IHTMLWindow4>();
            using ComScope<IHTMLFrameBase> htmlFrameBase = new(null);
            htmlWindow4.Value->get_frameElement(htmlFrameBase).ThrowOnFailure();
            IHTMLElement* htmlElement;
            return !htmlFrameBase.IsNull && htmlFrameBase.Value->QueryInterface(IID.Get<IHTMLElement>(), (void**)&htmlElement).Succeeded
                ? new HtmlElement(ShimManager, htmlElement)
                : null;
        }
    }

    public void Alert(string message)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        using BSTR bstrMessage = new(message);
        htmlWindow.Value->alert(bstrMessage).ThrowOnFailure();
    }

    public void AttachEventHandler(string eventName, EventHandler eventHandler) =>
        WindowShim.AttachEventHandler(eventName, eventHandler);

    public void Close()
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        htmlWindow.Value->close().ThrowOnFailure();
    }

    public bool Confirm(string message)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        using BSTR bstrMessage = new(message);
        VARIANT_BOOL confirmed;
        htmlWindow.Value->confirm(bstrMessage, &confirmed).ThrowOnFailure();
        return confirmed;
    }

    public void DetachEventHandler(string eventName, EventHandler eventHandler) =>
        WindowShim.DetachEventHandler(eventName, eventHandler);

    public void Focus()
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        htmlWindow.Value->focus().ThrowOnFailure();
    }

    /// <summary>
    ///  Moves the Window to the position requested
    /// </summary>
    public void MoveTo(int x, int y)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        htmlWindow.Value->moveTo(x, y).ThrowOnFailure();
    }

    /// <summary>
    ///  Moves the Window to the point requested
    /// </summary>
    public void MoveTo(Point point)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        htmlWindow.Value->moveTo(point.X, point.Y).ThrowOnFailure();
    }

    public void Navigate(Uri url)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        using BSTR bstrUrl = new(url.ToString());
        htmlWindow.Value->navigate(bstrUrl).ThrowOnFailure();
    }

    ///  Note: We intentionally have a string overload (apparently Mort wants one). We don't have
    ///  string overloads call Uri overloads because that breaks Uris that aren't fully qualified
    ///  (things like "www.Microsoft.com") that the underlying objects support and we don't want to
    ///  break.
    public void Navigate(string urlString)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        using BSTR url = new(urlString);
        htmlWindow.Value->navigate(url).ThrowOnFailure();
    }

    ///  Note: We intentionally have a string overload (apparently Mort wants one). We don't have
    ///  string overloads call Uri overloads because that breaks Uris that aren't fully qualified
    ///  (things like "www.Microsoft.com") that the underlying objects support and we don't want to
    ///  break.
    public HtmlWindow? Open(string urlString, string target, string windowOptions, bool replaceEntry)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        using BSTR url = new(urlString);
        using BSTR bstrTarget = new(target);
        using BSTR options = new(windowOptions);
        IHTMLWindow2* htmlWindow2;
        htmlWindow.Value->open(url, bstrTarget, options, replaceEntry, &htmlWindow2).ThrowOnFailure();
        return htmlWindow2 is not null ? new HtmlWindow(ShimManager, htmlWindow2) : null;
    }

    public HtmlWindow? Open(Uri url, string target, string windowOptions, bool replaceEntry) =>
        Open(url.ToString(), target, windowOptions, replaceEntry);

    ///  Note: We intentionally have a string overload (apparently Mort wants one). We don't have
    ///  string overloads call Uri overloads because that breaks Uris that aren't fully qualified
    ///  (things like "www.Microsoft.com") that the underlying objects support and we don't want to
    ///  break.
    public HtmlWindow? OpenNew(string urlString, string windowOptions)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        using BSTR url = new(urlString);
        using BSTR target = new("_blank");
        using BSTR options = new(windowOptions);
        IHTMLWindow2* iHTMLWindow2;
        htmlWindow.Value->open(url, target, options, VARIANT_BOOL.VARIANT_TRUE, &iHTMLWindow2).ThrowOnFailure();
        return iHTMLWindow2 is not null ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
    }

    public HtmlWindow? OpenNew(Uri url, string windowOptions) =>
        OpenNew(url.ToString(), windowOptions);

    public string? Prompt(string message, string defaultInputValue)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        using BSTR bstrMessage = new(message);
        using BSTR input = new(defaultInputValue);
        using VARIANT result = default;
        htmlWindow.Value->prompt(bstrMessage, input, &result).ThrowOnFailure();
        return (string?)result.ToObject();
    }

    public void RemoveFocus()
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        htmlWindow.Value->blur().ThrowOnFailure();
    }

    /// <summary>
    ///  Resize the window to the width/height requested
    /// </summary>
    public void ResizeTo(int width, int height)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        htmlWindow.Value->resizeTo(width, height).ThrowOnFailure();
    }

    /// <summary>
    ///  Resize the window to the Size requested
    /// </summary>
    public void ResizeTo(Size size)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        htmlWindow.Value->resizeTo(size.Width, size.Height).ThrowOnFailure();
    }

    /// <summary>
    ///  Scroll the window to the position requested
    /// </summary>
    public void ScrollTo(int x, int y)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        htmlWindow.Value->scrollTo(x, y).ThrowOnFailure();
    }

    /// <summary>
    ///  Scroll the window to the point requested
    /// </summary>
    public void ScrollTo(Point point)
    {
        using var htmlWindow = NativeHtmlWindow.GetInterface();
        htmlWindow.Value->scrollTo(point.X, point.Y).ThrowOnFailure();
    }

    //
    // Events
    //

    public event HtmlElementErrorEventHandler? Error
    {
        add => WindowShim.AddHandler(s_eventError, value);
        remove => WindowShim.RemoveHandler(s_eventError, value);
    }

    public event HtmlElementEventHandler? GotFocus
    {
        add => WindowShim.AddHandler(s_eventGotFocus, value);
        remove => WindowShim.RemoveHandler(s_eventGotFocus, value);
    }

    public event HtmlElementEventHandler? Load
    {
        add => WindowShim.AddHandler(s_eventLoad, value);
        remove => WindowShim.RemoveHandler(s_eventLoad, value);
    }

    public event HtmlElementEventHandler? LostFocus
    {
        add => WindowShim.AddHandler(s_eventLostFocus, value);
        remove => WindowShim.RemoveHandler(s_eventLostFocus, value);
    }

    public event HtmlElementEventHandler? Resize
    {
        add => WindowShim.AddHandler(s_eventResize, value);
        remove => WindowShim.RemoveHandler(s_eventResize, value);
    }

    public event HtmlElementEventHandler? Scroll
    {
        add => WindowShim.AddHandler(s_eventScroll, value);
        remove => WindowShim.RemoveHandler(s_eventScroll, value);
    }

    public event HtmlElementEventHandler? Unload
    {
        add => WindowShim.AddHandler(s_eventUnload, value);
        remove => WindowShim.RemoveHandler(s_eventUnload, value);
    }

    public static unsafe bool operator ==(HtmlWindow? left, HtmlWindow? right)
    {
        if (left is null)
        {
            return right is null;
        }

        if (right is null)
        {
            return false;
        }

        // Neither are null. Compare their native pointers.
        return left.NativeHtmlWindow.IsSameNativeObject(right.NativeHtmlWindow);
    }

    public static bool operator !=(HtmlWindow? left, HtmlWindow? right) => !(left == right);

    public override int GetHashCode() => _hashCode;

    public override bool Equals(object? obj) => this == (HtmlWindow?)obj;
}
