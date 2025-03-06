// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms;

public sealed unsafe partial class HtmlDocument
{
    internal static object s_eventClick = new();
    internal static object s_eventContextMenuShowing = new();
    internal static object s_eventFocusing = new();
    internal static object s_eventLosingFocus = new();
    internal static object s_eventMouseDown = new();
    internal static object s_eventMouseLeave = new();
    internal static object s_eventMouseMove = new();
    internal static object s_eventMouseOver = new();
    internal static object s_eventMouseUp = new();
    internal static object s_eventStop = new();

    private readonly AgileComPointer<IHTMLDocument2> _htmlDocument2;
    private readonly HtmlShimManager _shimManager;
    private readonly int _hashCode;

    internal HtmlDocument(HtmlShimManager shimManager, IHTMLDocument* doc)
    {
        IHTMLDocument2* htmlDoc2;
        doc->QueryInterface(IID.Get<IHTMLDocument2>(), (void**)&htmlDoc2).ThrowOnFailure();
#if DEBUG
        _htmlDocument2 = new(htmlDoc2, takeOwnership: true, trackDisposal: false);
#else
        _htmlDocument2 = new(htmlDoc2, takeOwnership: true);
#endif
        Debug.Assert(NativeHtmlDocument2 is not null, "The document should implement IHtmlDocument2");
        using var scope = _htmlDocument2.GetInterface<IUnknown>();
        _hashCode = HashCode.Combine((nint)scope.Value);

        _shimManager = shimManager;
    }

    internal AgileComPointer<IHTMLDocument2> NativeHtmlDocument2 => _htmlDocument2;

    /// <summary>
    ///  Helper method to get IHTMLDocumentX interface of interest. Throws if failure occurs.
    /// </summary>
    private ComScope<T> GetHtmlDocument<T>() where T : unmanaged, IComIID
    {
        using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
        var scope = htmlDoc2.TryQuery<T>(out HRESULT hr);
        hr.ThrowOnFailure();
        return scope;
    }

    private HtmlDocumentShim DocumentShim
    {
        get
        {
            HtmlDocumentShim? shim = ShimManager.GetDocumentShim(this);
            if (shim is null)
            {
                _shimManager.AddDocumentShim(this);
                shim = ShimManager.GetDocumentShim(this);
            }

            return shim!;
        }
    }

    private HtmlShimManager ShimManager => _shimManager;

    public HtmlElement? ActiveElement
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            IHTMLElement* iHtmlElement;
            htmlDoc2.Value->get_activeElement(&iHtmlElement).ThrowOnFailure();
            return iHtmlElement is not null ? new HtmlElement(ShimManager, iHtmlElement) : null;
        }
    }

    public HtmlElement? Body
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            IHTMLElement* iHtmlElement;
            htmlDoc2.Value->get_body(&iHtmlElement).ThrowOnFailure();
            return iHtmlElement is not null ? new HtmlElement(ShimManager, iHtmlElement) : null;
        }
    }

    public string Domain
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            using BSTR domain = default;
            htmlDoc2.Value->get_domain(&domain).ThrowOnFailure();
            return domain.ToString();
        }
        set
        {
            try
            {
                using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
                using BSTR newValue = new(value);
                htmlDoc2.Value->put_domain(newValue).ThrowOnFailure();
            }
            catch (ArgumentException)
            {
                // Give a better message describing the error
                throw new ArgumentException(SR.HtmlDocumentInvalidDomain);
            }
        }
    }

    public string Title
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            using BSTR title = default;
            htmlDoc2.Value->get_title(&title).ThrowOnFailure();
            return title.ToString();
        }
        set
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            using BSTR newValue = new(value);
            htmlDoc2.Value->put_title(newValue).ThrowOnFailure();
        }
    }

    public Uri? Url
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            using ComScope<IHTMLLocation> htmlLocation = new(null);
            htmlDoc2.Value->get_location(htmlLocation).ThrowOnFailure();
            if (htmlLocation.IsNull)
            {
                return null;
            }

            using BSTR href = default;
            htmlLocation.Value->get_href(&href).ThrowOnFailure();
            string hrefString = href.ToString();
            return string.IsNullOrEmpty(hrefString) ? null : new Uri(hrefString);
        }
    }

    public HtmlWindow? Window
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            IHTMLWindow2* iHTMLWindow2;
            htmlDoc2.Value->get_parentWindow(&iHTMLWindow2).ThrowOnFailure();
            return iHTMLWindow2 is not null ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
        }
    }

    public Color BackColor
    {
        get
        {
            try
            {
                using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
                using VARIANT color = default;
                htmlDoc2.Value->get_bgColor(&color).ThrowOnFailure();
                return ColorFromVARIANT(color);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                return Color.Empty;
            }
        }
        set
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            int color = value.R << 16 | value.G << 8 | value.B;
            var variantColor = (VARIANT)color;
            htmlDoc2.Value->put_bgColor(variantColor).ThrowOnFailure();
        }
    }

    public Color ForeColor
    {
        get
        {
            try
            {
                using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
                using VARIANT color = default;
                htmlDoc2.Value->get_fgColor(&color).ThrowOnFailure();
                return ColorFromVARIANT(color);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                return Color.Empty;
            }
        }
        set
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            int color = value.R << 16 | value.G << 8 | value.B;
            var variantColor = (VARIANT)color;
            htmlDoc2.Value->put_fgColor(variantColor).ThrowOnFailure();
        }
    }

    public Color LinkColor
    {
        get
        {
            try
            {
                using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
                using VARIANT color = default;
                htmlDoc2.Value->get_linkColor(&color).ThrowOnFailure();
                return ColorFromVARIANT(color);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                return Color.Empty;
            }
        }
        set
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            int color = value.R << 16 | value.G << 8 | value.B;
            var variantColor = (VARIANT)color;
            htmlDoc2.Value->put_linkColor(variantColor).ThrowOnFailure();
        }
    }

    public Color ActiveLinkColor
    {
        get
        {
            try
            {
                using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
                using VARIANT color = default;
                htmlDoc2.Value->get_alinkColor(&color).ThrowOnFailure();
                return ColorFromVARIANT(color);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                return Color.Empty;
            }
        }
        set
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            int color = value.R << 16 | value.G << 8 | value.B;
            var variantColor = (VARIANT)color;
            htmlDoc2.Value->put_alinkColor(variantColor).ThrowOnFailure();
        }
    }

    public Color VisitedLinkColor
    {
        get
        {
            try
            {
                using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
                using VARIANT color = default;
                htmlDoc2.Value->get_vlinkColor(&color).ThrowOnFailure();
                return ColorFromVARIANT(color);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                return Color.Empty;
            }
        }
        set
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            int color = value.R << 16 | value.G << 8 | value.B;
            var variantColor = (VARIANT)color;
            htmlDoc2.Value->put_vlinkColor(variantColor).ThrowOnFailure();
        }
    }

    public bool Focused
    {
        get
        {
            using var htmlDoc4 = GetHtmlDocument<IHTMLDocument4>();
            VARIANT_BOOL focused = default;
            htmlDoc4.Value->hasFocus(&focused).ThrowOnFailure();
            return focused;
        }
    }

    public object DomDocument => NativeHtmlDocument2.GetManagedObject();

    public string Cookie
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            using BSTR cookie = default;
            htmlDoc2.Value->get_cookie(&cookie).ThrowOnFailure();
            return cookie.ToString();
        }
        set
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            using BSTR newValue = new(value);
            htmlDoc2.Value->put_cookie(newValue).ThrowOnFailure();
        }
    }

    public bool RightToLeft
    {
        get
        {
            using var htmlDoc3 = GetHtmlDocument<IHTMLDocument3>();
            using BSTR dir = default;
            htmlDoc3.Value->get_dir(&dir).ThrowOnFailure();
            return dir.ToString() == "rtl";
        }
        set
        {
            using var htmlDoc3 = GetHtmlDocument<IHTMLDocument3>();
            using BSTR newValue = new(value ? "rtl" : "ltr");
            htmlDoc3.Value->put_dir(newValue).ThrowOnFailure();
        }
    }

    public string Encoding
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            using BSTR charset = default;
            htmlDoc2.Value->get_charset(&charset).ThrowOnFailure();
            return charset.ToString();
        }
        set
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            using BSTR newValue = new(value);
            htmlDoc2.Value->put_charset(newValue).ThrowOnFailure();
        }
    }

    public string DefaultEncoding
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            using BSTR charset = default;
            htmlDoc2.Value->get_defaultCharset(&charset).ThrowOnFailure();
            return charset.ToString();
        }
    }

    public HtmlElementCollection All
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            IHTMLElementCollection* iHTMLElementCollection;
            htmlDoc2.Value->get_all(&iHTMLElementCollection).ThrowOnFailure();
            return iHTMLElementCollection is not null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
        }
    }

    public HtmlElementCollection Links
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            IHTMLElementCollection* iHTMLElementCollection;
            htmlDoc2.Value->get_links(&iHTMLElementCollection).ThrowOnFailure();
            return iHTMLElementCollection is not null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
        }
    }

    public HtmlElementCollection Images
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            IHTMLElementCollection* iHTMLElementCollection;
            htmlDoc2.Value->get_images(&iHTMLElementCollection).ThrowOnFailure();
            return iHTMLElementCollection is not null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
        }
    }

    public HtmlElementCollection Forms
    {
        get
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            IHTMLElementCollection* iHTMLElementCollection;
            htmlDoc2.Value->get_forms(&iHTMLElementCollection).ThrowOnFailure();
            return iHTMLElementCollection is not null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
        }
    }

    public void Write(string text)
    {
        using SafeArrayScope<object> scope = new(1);
        if (scope.IsNull)
        {
            return;
        }

        scope[0] = text;

        using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
        htmlDoc2.Value->write(scope);
    }

    /// <summary>
    ///  Executes a command on the document
    /// </summary>
    public void ExecCommand(string command, bool showUI, object value)
    {
        using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
        using BSTR bstrCommand = new(command);
        using var variantValue = VARIANT.FromObject(value);
        VARIANT_BOOL varBool = default;
        htmlDoc2.Value->execCommand(bstrCommand, showUI, variantValue, &varBool).ThrowOnFailure();
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void Focus()
    {
        using var htmlDoc4 = GetHtmlDocument<IHTMLDocument4>();
        htmlDoc4.Value->focus().ThrowOnFailure();
        // Seems to have a problem in really setting focus the first time
        htmlDoc4.Value->focus().ThrowOnFailure();
    }

    public HtmlElement? GetElementById(string id)
    {
        using var htmlDoc3 = GetHtmlDocument<IHTMLDocument3>();
        using BSTR bstrId = new(id);
        IHTMLElement* iHTMLElement;
        htmlDoc3.Value->getElementById(bstrId, &iHTMLElement).ThrowOnFailure();
        return iHTMLElement is not null ? new HtmlElement(ShimManager, iHTMLElement) : null;
    }

    public HtmlElement? GetElementFromPoint(Point point)
    {
        using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
        IHTMLElement* iHTMLElement;
        htmlDoc2.Value->elementFromPoint(point.X, point.Y, &iHTMLElement).ThrowOnFailure();
        return iHTMLElement is not null ? new HtmlElement(ShimManager, iHTMLElement) : null;
    }

    public HtmlElementCollection GetElementsByTagName(string tagName)
    {
        using var htmlDoc3 = GetHtmlDocument<IHTMLDocument3>();
        using BSTR bstrTagName = new(tagName);
        IHTMLElementCollection* iHTMLElementCollection;
        htmlDoc3.Value->getElementsByTagName(bstrTagName, &iHTMLElementCollection).ThrowOnFailure();
        return iHTMLElementCollection is not null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
    }

    public HtmlDocument? OpenNew(bool replaceInHistory)
    {
        using var name = (VARIANT)(replaceInHistory ? "replace" : string.Empty);
        using BSTR url = new("text/html");

        using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
        using ComScope<IDispatch> dispatch = new(null);
        htmlDoc2.Value->open(url, name, VARIANT.Empty, VARIANT.Empty, dispatch).ThrowOnFailure();
        if (dispatch.IsNull)
        {
            return null;
        }

        using var htmlDoc = dispatch.TryQuery<IHTMLDocument>(out HRESULT hr);
        return hr.Succeeded ? new HtmlDocument(ShimManager, htmlDoc) : null;
    }

    public HtmlElement? CreateElement(string elementTag)
    {
        using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
        using BSTR bstrElementTag = new(elementTag);
        IHTMLElement* iHTMLElement;
        htmlDoc2.Value->createElement(bstrElementTag, &iHTMLElement).ThrowOnFailure();
        return iHTMLElement is not null ? new HtmlElement(ShimManager, iHTMLElement) : null;
    }

    public unsafe object? InvokeScript(string scriptName, object[]? args)
    {
        try
        {
            using var htmlDoc2 = NativeHtmlDocument2.GetInterface();
            using ComScope<IDispatch> scriptDispatch = new(null);
            HRESULT hr = htmlDoc2.Value->get_Script(scriptDispatch);
            if (hr.Failed || scriptDispatch.IsNull)
            {
                return null;
            }

            int dispid = PInvokeCore.DISPID_UNKNOWN;
            fixed (char* n = scriptName)
            {
                hr = scriptDispatch.Value->GetIDsOfNames(IID.NULL(), (PWSTR*)&n, 1, PInvokeCore.GetThreadLocale(), &dispid);
                if (!hr.Succeeded || dispid == PInvokeCore.DISPID_UNKNOWN)
                {
                    return null;
                }
            }

            if (args is not null)
            {
                // Reverse the arg order so that they read naturally after IDispatch.
                Array.Reverse(args);
            }

            using VARIANTVector vectorArgs = new(args);
            fixed (VARIANT* pVariants = vectorArgs.Variants)
            {
                DISPPARAMS dispParams = new()
                {
                    rgvarg = pVariants,
                    cArgs = (uint)vectorArgs.Variants.Length,
                    rgdispidNamedArgs = null,
                    cNamedArgs = 0
                };

                VARIANT result = default;
                EXCEPINFO excepInfo = default;
                hr = scriptDispatch.Value->Invoke(
                    dispid,
                    IID.NULL(),
                    PInvokeCore.GetThreadLocale(),
                    DISPATCH_FLAGS.DISPATCH_METHOD,
                    &dispParams,
                    &result,
                    &excepInfo,
                    null);

                if (hr == HRESULT.S_OK)
                {
                    return result.ToObject();
                }
            }

            return null;
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
        }

        return null;
    }

    public object? InvokeScript(string scriptName) => InvokeScript(scriptName, null);

    public void AttachEventHandler(string eventName, EventHandler eventHandler)
    {
        HtmlDocumentShim? shim = DocumentShim;
        shim?.AttachEventHandler(eventName, eventHandler);
    }

    public void DetachEventHandler(string eventName, EventHandler eventHandler)
    {
        HtmlDocumentShim? shim = DocumentShim;
        shim?.DetachEventHandler(eventName, eventHandler);
    }

    public event HtmlElementEventHandler? Click
    {
        add => DocumentShim.AddHandler(s_eventClick, value);
        remove => DocumentShim.RemoveHandler(s_eventClick, value);
    }

    public event HtmlElementEventHandler? ContextMenuShowing
    {
        add => DocumentShim.AddHandler(s_eventContextMenuShowing, value);
        remove => DocumentShim.RemoveHandler(s_eventContextMenuShowing, value);
    }

    public event HtmlElementEventHandler? Focusing
    {
        add => DocumentShim.AddHandler(s_eventFocusing, value);
        remove => DocumentShim.RemoveHandler(s_eventFocusing, value);
    }

    public event HtmlElementEventHandler? LosingFocus
    {
        add => DocumentShim.AddHandler(s_eventLosingFocus, value);
        remove => DocumentShim.RemoveHandler(s_eventLosingFocus, value);
    }

    public event HtmlElementEventHandler? MouseDown
    {
        add => DocumentShim.AddHandler(s_eventMouseDown, value);
        remove => DocumentShim.RemoveHandler(s_eventMouseDown, value);
    }

    /// <summary>
    ///  Occurs when the mouse leaves the document
    /// </summary>
    public event HtmlElementEventHandler? MouseLeave
    {
        add => DocumentShim.AddHandler(s_eventMouseLeave, value);
        remove => DocumentShim.RemoveHandler(s_eventMouseLeave, value);
    }

    public event HtmlElementEventHandler? MouseMove
    {
        add => DocumentShim.AddHandler(s_eventMouseMove, value);
        remove => DocumentShim.RemoveHandler(s_eventMouseMove, value);
    }

    public event HtmlElementEventHandler? MouseOver
    {
        add => DocumentShim.AddHandler(s_eventMouseOver, value);
        remove => DocumentShim.RemoveHandler(s_eventMouseOver, value);
    }

    public event HtmlElementEventHandler? MouseUp
    {
        add => DocumentShim.AddHandler(s_eventMouseUp, value);
        remove => DocumentShim.RemoveHandler(s_eventMouseUp, value);
    }

    public event HtmlElementEventHandler? Stop
    {
        add => DocumentShim.AddHandler(s_eventStop, value);
        remove => DocumentShim.RemoveHandler(s_eventStop, value);
    }

    private static Color ColorFromVARIANT(VARIANT vColor)
    {
        try
        {
            if (vColor.Type == VARENUM.VT_BSTR)
            {
                string strColor = (string)vColor.ToObject()!;
                int index = strColor.IndexOf('#');
                if (index >= 0)
                {
                    // The string is of the form: #ff00a0. Skip past the #
                    string hexColor = strColor[(index + 1)..];

                    // The actual color is non-transparent. So set alpha = 255.
                    return Color.FromArgb(255, Color.FromArgb(int.Parse(hexColor, NumberStyles.HexNumber, CultureInfo.InvariantCulture)));
                }
                else
                {
                    return Color.FromName(strColor);
                }
            }
            else if (vColor.Type is VARENUM.VT_I4 or VARENUM.VT_INT)
            {
                // The actual color is non-transparent. So set alpha = 255.
                return Color.FromArgb(255, Color.FromArgb(vColor.data.intVal));
            }
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
        }

        return Color.Empty;
    }

    public static unsafe bool operator ==(HtmlDocument? left, HtmlDocument? right)
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
        return left.NativeHtmlDocument2.IsSameNativeObject(right.NativeHtmlDocument2);
    }

    public static bool operator !=(HtmlDocument? left, HtmlDocument? right) => !(left == right);

    public override int GetHashCode() => _hashCode;

    public override bool Equals(object? obj) => this == (HtmlDocument?)obj;
}
