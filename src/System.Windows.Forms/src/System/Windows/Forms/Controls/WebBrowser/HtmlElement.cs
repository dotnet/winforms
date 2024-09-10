// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms;

public sealed unsafe partial class HtmlElement
{
    internal static readonly object s_eventClick = new();
    internal static readonly object s_eventDoubleClick = new();
    internal static readonly object s_eventDrag = new();
    internal static readonly object s_eventDragEnd = new();
    internal static readonly object s_eventDragLeave = new();
    internal static readonly object s_eventDragOver = new();
    internal static readonly object s_eventFocusing = new();
    internal static readonly object s_eventGotFocus = new();
    internal static readonly object s_eventLosingFocus = new();
    internal static readonly object s_eventLostFocus = new();
    internal static readonly object s_eventKeyDown = new();
    internal static readonly object s_eventKeyPress = new();
    internal static readonly object s_eventKeyUp = new();
    internal static readonly object s_eventMouseDown = new();
    internal static readonly object s_eventMouseEnter = new();
    internal static readonly object s_eventMouseLeave = new();
    internal static readonly object s_eventMouseMove = new();
    internal static readonly object s_eventMouseOver = new();
    internal static readonly object s_eventMouseUp = new();

    private readonly AgileComPointer<IHTMLElement> _htmlElement;
    private readonly HtmlShimManager _shimManager;
    private readonly int _hashCode;

    internal HtmlElement(HtmlShimManager shimManager, IHTMLElement* element)
    {
#if DEBUG
        _htmlElement = new(element, takeOwnership: true, trackDisposal: false);
#else
        _htmlElement = new(element, takeOwnership: true);
#endif
        Debug.Assert(NativeHtmlElement is not null, "The element object should implement IHTMLElement");
        using var scope = _htmlElement.GetInterface<IUnknown>();
        _hashCode = HashCode.Combine((nint)scope.Value);

        _shimManager = shimManager;
    }

    public HtmlElementCollection All
    {
        get
        {
            using var htmlElement = NativeHtmlElement.GetInterface();
            using ComScope<IDispatch> dispatch = new(null);
            htmlElement.Value->get_all(dispatch).ThrowOnFailure();
            IHTMLElementCollection* htmlElementCollection;
            return !dispatch.IsNull && dispatch.Value->QueryInterface(IID.Get<IHTMLElementCollection>(), (void**)&htmlElementCollection).Succeeded
                ? new(_shimManager, htmlElementCollection)
                : new(_shimManager);
        }
    }

    public HtmlElementCollection Children
    {
        get
        {
            using var htmlElement = NativeHtmlElement.GetInterface();
            using ComScope<IDispatch> dispatch = new(null);
            htmlElement.Value->get_children(dispatch).ThrowOnFailure();
            IHTMLElementCollection* htmlElementCollection;
            return !dispatch.IsNull && dispatch.Value->QueryInterface(IID.Get<IHTMLElementCollection>(), (void**)&htmlElementCollection).Succeeded
                ? new(_shimManager, htmlElementCollection)
                : new(_shimManager);
        }
    }

    public bool CanHaveChildren
    {
        get
        {
            using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
            VARIANT_BOOL canHaveChildren = default;
            htmlElement2.Value->get_canHaveChildren(&canHaveChildren).ThrowOnFailure();
            return canHaveChildren;
        }
    }

    public Rectangle ClientRectangle
    {
        get
        {
            using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
            int clientLeft;
            int clientTop;
            int clientWidth;
            int clientHeight;
            htmlElement2.Value->get_clientLeft(&clientLeft).ThrowOnFailure();
            htmlElement2.Value->get_clientTop(&clientTop).ThrowOnFailure();
            htmlElement2.Value->get_clientWidth(&clientWidth).ThrowOnFailure();
            htmlElement2.Value->get_clientHeight(&clientHeight).ThrowOnFailure();

            return new(clientLeft, clientTop, clientWidth, clientHeight);
        }
    }

    public HtmlDocument? Document
    {
        get
        {
            using var nativeHtmlElement = NativeHtmlElement.GetInterface();
            using ComScope<IDispatch> dispatch = new(null);
            nativeHtmlElement.Value->get_document(dispatch).ThrowOnFailure();
            if (dispatch.IsNull)
            {
                return null;
            }

            using var htmlDocument = dispatch.TryQuery<IHTMLDocument>(out HRESULT hr);
            return hr.Succeeded ? new HtmlDocument(_shimManager, htmlDocument) : null;
        }
    }

    public bool Enabled
    {
        get
        {
            using var htmlElement3 = GetHtmlElement<IHTMLElement3>();
            VARIANT_BOOL disabled = default;
            htmlElement3.Value->get_disabled(&disabled).ThrowOnFailure();
            return !disabled;
        }
        set
        {
            using var htmlElement3 = GetHtmlElement<IHTMLElement3>();
            htmlElement3.Value->put_disabled(!value).ThrowOnFailure();
        }
    }

    private HtmlElementShim ElementShim
    {
        get
        {
            HtmlElementShim? shim = ShimManager.GetElementShim(this);
            if (shim is null)
            {
                _shimManager.AddElementShim(this);
                shim = ShimManager.GetElementShim(this);
            }

            return shim!;
        }
    }

    public HtmlElement? FirstChild
    {
        get
        {
            IHTMLElement* iHtmlElement = null;
            using var htmlElement = NativeHtmlElement.GetInterface();
            using var node = htmlElement.TryQuery<IHTMLDOMNode>(out HRESULT hr);
            if (hr.Succeeded)
            {
                using ComScope<IHTMLDOMNode> child = new(null);
                node.Value->get_firstChild(child).ThrowOnFailure();
                if (!child.IsNull)
                {
                    hr = child.Value->QueryInterface(IID.Get<IHTMLElement>(), (void**)&iHtmlElement);
                    hr.AssertSuccess();
                }
            }

            return iHtmlElement is not null ? new HtmlElement(_shimManager, iHtmlElement) : null;
        }
    }

    public string Id
    {
        get
        {
            using var nativeHtmlElement = NativeHtmlElement.GetInterface();
            using BSTR id = default;
            nativeHtmlElement.Value->get_id(&id).ThrowOnFailure();
            return id.ToString();
        }
        set
        {
            using var nativeHtmlElement = NativeHtmlElement.GetInterface();
            using BSTR newValue = new(value);
            nativeHtmlElement.Value->put_id(newValue).ThrowOnFailure();
        }
    }

    public string InnerHtml
    {
        get
        {
            using var nativeHtmlElement = NativeHtmlElement.GetInterface();
            using BSTR innerHtml = default;
            nativeHtmlElement.Value->get_innerHTML(&innerHtml).ThrowOnFailure();
            return innerHtml.ToString();
        }
        set
        {
            try
            {
                using var nativeHtmlElement = NativeHtmlElement.GetInterface();
                using BSTR newValue = new(value);
                nativeHtmlElement.Value->put_innerHTML(newValue).ThrowOnFailure();
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == unchecked((int)0x800a0258))
                {
                    throw new NotSupportedException(SR.HtmlElementPropertyNotSupported);
                }

                throw;
            }
        }
    }

    public string InnerText
    {
        get
        {
            using var nativeHtmlElement = NativeHtmlElement.GetInterface();
            using BSTR innerText = default;
            nativeHtmlElement.Value->get_innerText(&innerText).ThrowOnFailure();
            return innerText.ToString();
        }
        set
        {
            try
            {
                using var nativeHtmlElement = NativeHtmlElement.GetInterface();
                using BSTR newValue = new(value);
                nativeHtmlElement.Value->put_innerText(newValue).ThrowOnFailure();
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == unchecked((int)0x800a0258))
                {
                    throw new NotSupportedException(SR.HtmlElementPropertyNotSupported);
                }

                throw;
            }
        }
    }

    public string Name
    {
        get => GetAttribute("Name");
        set => SetAttribute("Name", value);
    }

    internal AgileComPointer<IHTMLElement> NativeHtmlElement => _htmlElement;

    /// <summary>
    ///  Helper method to get IHTMLElementX interface of interest. Throws if failure occurs.
    /// </summary>
    private ComScope<T> GetHtmlElement<T>() where T : unmanaged, IComIID
    {
        using var htmlElement = NativeHtmlElement.GetInterface();
        var scope = htmlElement.TryQuery<T>(out HRESULT hr);
        hr.ThrowOnFailure();
        return scope;
    }

    public HtmlElement? NextSibling
    {
        get
        {
            IHTMLElement* iHtmlElement = null;
            using var htmlElement = NativeHtmlElement.GetInterface();
            using var node = htmlElement.TryQuery<IHTMLDOMNode>(out HRESULT hr);
            if (hr.Succeeded)
            {
                using ComScope<IHTMLDOMNode> sibling = new(null);
                node.Value->get_nextSibling(sibling).ThrowOnFailure();
                if (!sibling.IsNull)
                {
                    hr = sibling.Value->QueryInterface(IID.Get<IHTMLElement>(), (void**)&iHtmlElement);
                    hr.AssertSuccess();
                }
            }

            return iHtmlElement is not null ? new HtmlElement(_shimManager, iHtmlElement) : null;
        }
    }

    public Rectangle OffsetRectangle
    {
        get
        {
            using var nativeHtmlElement = NativeHtmlElement.GetInterface();
            int offsetLeft;
            int offsetTop;
            int offsetWidth;
            int offsetHeight;

            nativeHtmlElement.Value->get_offsetLeft(&offsetLeft).ThrowOnFailure();
            nativeHtmlElement.Value->get_offsetTop(&offsetTop).ThrowOnFailure();
            nativeHtmlElement.Value->get_offsetWidth(&offsetWidth).ThrowOnFailure();
            nativeHtmlElement.Value->get_offsetHeight(&offsetHeight).ThrowOnFailure();

            return new(offsetLeft, offsetTop, offsetWidth, offsetHeight);
        }
    }

    public HtmlElement? OffsetParent
    {
        get
        {
            using var nativeHtmlElement = NativeHtmlElement.GetInterface();
            IHTMLElement* iHtmlElement;
            nativeHtmlElement.Value->get_offsetParent(&iHtmlElement).ThrowOnFailure();
            return iHtmlElement is not null ? new HtmlElement(_shimManager, iHtmlElement) : null;
        }
    }

    public string OuterHtml
    {
        get
        {
            using var nativeHtmlElement = NativeHtmlElement.GetInterface();
            using BSTR outerHtml = default;
            nativeHtmlElement.Value->get_outerHTML(&outerHtml).ThrowOnFailure();
            return outerHtml.ToString();
        }
        set
        {
            try
            {
                using var nativeHtmlElement = NativeHtmlElement.GetInterface();
                using BSTR newValue = new(value);
                nativeHtmlElement.Value->put_outerHTML(newValue).ThrowOnFailure();
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == unchecked((int)0x800a0258))
                {
                    throw new NotSupportedException(SR.HtmlElementPropertyNotSupported);
                }

                throw;
            }
        }
    }

    public string OuterText
    {
        get
        {
            using var htmlElement = NativeHtmlElement.GetInterface();
            using BSTR outerText = default;
            htmlElement.Value->get_outerText(&outerText).ThrowOnFailure();
            return outerText.ToString();
        }
        set
        {
            try
            {
                using var htmlElement = NativeHtmlElement.GetInterface();
                using BSTR newValue = new(value);
                htmlElement.Value->put_outerText(newValue).ThrowOnFailure();
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == unchecked((int)0x800a0258))
                {
                    throw new NotSupportedException(SR.HtmlElementPropertyNotSupported);
                }

                throw;
            }
        }
    }

    public HtmlElement? Parent
    {
        get
        {
            using var htmlElement = NativeHtmlElement.GetInterface();
            IHTMLElement* iHtmlElement;
            htmlElement.Value->get_parentElement(&iHtmlElement).ThrowOnFailure();
            return iHtmlElement is not null ? new HtmlElement(_shimManager, iHtmlElement) : null;
        }
    }

    public Rectangle ScrollRectangle
    {
        get
        {
            using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
            int scrollLeft;
            int scrollTop;
            int scrollWidth;
            int scrollHeight;

            htmlElement2.Value->get_scrollLeft(&scrollLeft).ThrowOnFailure();
            htmlElement2.Value->get_scrollTop(&scrollTop).ThrowOnFailure();
            htmlElement2.Value->get_scrollWidth(&scrollWidth).ThrowOnFailure();
            htmlElement2.Value->get_scrollHeight(&scrollHeight).ThrowOnFailure();

            return new(scrollLeft, scrollTop, scrollWidth, scrollHeight);
        }
    }

    public int ScrollLeft
    {
        get
        {
            using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
            int scrollLeft;
            htmlElement2.Value->get_scrollLeft(&scrollLeft).ThrowOnFailure();
            return scrollLeft;
        }
        set
        {
            using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
            htmlElement2.Value->put_scrollLeft(value).ThrowOnFailure();
        }
    }

    public int ScrollTop
    {
        get
        {
            using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
            int scrollTop;
            htmlElement2.Value->get_scrollTop(&scrollTop).ThrowOnFailure();
            return scrollTop;
        }
        set
        {
            using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
            htmlElement2.Value->put_scrollTop(value).ThrowOnFailure();
        }
    }

    private HtmlShimManager ShimManager => _shimManager;

    public string Style
    {
        get
        {
            using var htmlElement = NativeHtmlElement.GetInterface();
            using ComScope<IHTMLStyle> style = new(null);
            htmlElement.Value->get_style(style).ThrowOnFailure();
            using BSTR cssText = default;
            style.Value->get_cssText(&cssText).ThrowOnFailure();
            return cssText.ToString();
        }
        set
        {
            using var htmlElement = NativeHtmlElement.GetInterface();
            using ComScope<IHTMLStyle> style = new(null);
            htmlElement.Value->get_style(style).ThrowOnFailure();
            using BSTR newValue = new(value);
            style.Value->put_cssText(newValue).ThrowOnFailure();
        }
    }

    public string TagName
    {
        get
        {
            using var htmlElement = NativeHtmlElement.GetInterface();
            using BSTR tagName = default;
            htmlElement.Value->get_tagName(&tagName).ThrowOnFailure();
            return tagName.ToString();
        }
    }

    public short TabIndex
    {
        get
        {
            using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
            short tabIndex;
            htmlElement2.Value->get_tabIndex(&tabIndex).ThrowOnFailure();
            return tabIndex;
        }
        set
        {
            using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
            htmlElement2.Value->put_tabIndex(value).ThrowOnFailure();
        }
    }

    public object DomElement => NativeHtmlElement.GetManagedObject();

    public HtmlElement? AppendChild(HtmlElement newElement)
    {
        return InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, newElement);
    }

    public void AttachEventHandler(string eventName, EventHandler eventHandler)
    {
        ElementShim.AttachEventHandler(eventName, eventHandler);
    }

    public void DetachEventHandler(string eventName, EventHandler eventHandler)
    {
        ElementShim.DetachEventHandler(eventName, eventHandler);
    }

    public void Focus()
    {
        try
        {
            using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
            htmlElement2.Value->focus().ThrowOnFailure();
        }
        catch (COMException ex)
        {
            if (ex.ErrorCode == unchecked((int)0x800a083e))
            {
                throw new NotSupportedException(SR.HtmlElementMethodNotSupported);
            }

            throw;
        }
    }

    public string GetAttribute(string attributeName)
    {
        using var htmlElement = NativeHtmlElement.GetInterface();
        using BSTR name = new(attributeName);
        using VARIANT attributeValue = default;
        htmlElement.Value->getAttribute(name, 0, &attributeValue).ThrowOnFailure();
        return attributeValue.Type == VARENUM.VT_NULL || attributeValue.ToObject() is not string validString
            ? string.Empty
            : validString;
    }

    public HtmlElementCollection GetElementsByTagName(string tagName)
    {
        using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
        using BSTR name = new(tagName);
        IHTMLElementCollection* iHTMLElementCollection;
        htmlElement2.Value->getElementsByTagName(name, &iHTMLElementCollection).ThrowOnFailure();
        return iHTMLElementCollection is not null ? new HtmlElementCollection(_shimManager, iHTMLElementCollection) : new HtmlElementCollection(_shimManager);
    }

    public HtmlElement? InsertAdjacentElement(HtmlElementInsertionOrientation orientation, HtmlElement newElement)
    {
        using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
        using BSTR where = new(orientation.ToString());
        using var htmlElement = NativeHtmlElement.GetInterface();
        using var insertedElement = ComHelpers.GetComScope<IHTMLElement>(newElement.DomElement);
        IHTMLElement* adjElement;
        htmlElement2.Value->insertAdjacentElement(where, insertedElement, &adjElement).ThrowOnFailure();
        return adjElement is not null ? new HtmlElement(_shimManager, adjElement) : null;
    }

    public object? InvokeMember(string methodName) => InvokeMember(methodName, parameter: null);

    public unsafe object? InvokeMember(string methodName, params object[]? parameter)
    {
        try
        {
            using var htmlElement = NativeHtmlElement.GetInterface();
            using var scriptDispatch = htmlElement.TryQuery<IDispatch>(out HRESULT hr);
            if (hr.Failed)
            {
                return null;
            }

            int dispid = PInvokeCore.DISPID_UNKNOWN;

            fixed (char* n = methodName)
            {
                hr = scriptDispatch.Value->GetIDsOfNames(IID.NULL(), (PWSTR*)&n, 1, PInvokeCore.GetThreadLocale(), &dispid);
                if (!hr.Succeeded || dispid == PInvokeCore.DISPID_UNKNOWN)
                {
                    return null;
                }
            }

            if (parameter is not null)
            {
                // Reverse the parameter order so that they read naturally after IDispatch.
                Array.Reverse(parameter);
            }

            using VARIANTVector vectorArgs = new(parameter);
            fixed (VARIANT* pVariant = vectorArgs.Variants)
            {
                DISPPARAMS dispParams = new()
                {
                    rgvarg = pVariant,
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

    public void RemoveFocus()
    {
        using var htmlElement2 = GetHtmlElement<IHTMLElement2>();
        htmlElement2.Value->blur().ThrowOnFailure();
    }

    public void RaiseEvent(string eventName)
    {
        using var htmlElement3 = GetHtmlElement<IHTMLElement3>();
        using BSTR name = new(eventName);
        VARIANT eventObj = default;
        VARIANT_BOOL canceled = default;
        htmlElement3.Value->fireEvent(name, &eventObj, &canceled).ThrowOnFailure();
    }

    public void ScrollIntoView(bool alignWithTop)
    {
        using var htmlElement = NativeHtmlElement.GetInterface();
        using var variantAlignWithTop = (VARIANT)alignWithTop;
        htmlElement.Value->scrollIntoView(variantAlignWithTop).ThrowOnFailure();
    }

    public void SetAttribute(string attributeName, string value)
    {
        try
        {
            using var htmlElement = NativeHtmlElement.GetInterface();
            using BSTR name = new(attributeName);
            using var variantValue = (VARIANT)value;
            htmlElement.Value->setAttribute(name, variantValue, 0).ThrowOnFailure();
        }
        catch (COMException comException)
        {
            if (comException.ErrorCode == unchecked((int)0x80020009))
            {
                throw new NotSupportedException(SR.HtmlElementAttributeNotSupported);
            }

            throw;
        }
    }

    //
    // Events:
    //
    public event HtmlElementEventHandler? Click
    {
        add => ElementShim.AddHandler(s_eventClick, value);
        remove => ElementShim.RemoveHandler(s_eventClick, value);
    }

    public event HtmlElementEventHandler? DoubleClick
    {
        add => ElementShim.AddHandler(s_eventDoubleClick, value);
        remove => ElementShim.RemoveHandler(s_eventDoubleClick, value);
    }

    public event HtmlElementEventHandler? Drag
    {
        add => ElementShim.AddHandler(s_eventDrag, value);
        remove => ElementShim.RemoveHandler(s_eventDrag, value);
    }

    public event HtmlElementEventHandler? DragEnd
    {
        add => ElementShim.AddHandler(s_eventDragEnd, value);
        remove => ElementShim.RemoveHandler(s_eventDragEnd, value);
    }

    public event HtmlElementEventHandler? DragLeave
    {
        add => ElementShim.AddHandler(s_eventDragLeave, value);
        remove => ElementShim.RemoveHandler(s_eventDragLeave, value);
    }

    public event HtmlElementEventHandler? DragOver
    {
        add => ElementShim.AddHandler(s_eventDragOver, value);
        remove => ElementShim.RemoveHandler(s_eventDragOver, value);
    }

    public event HtmlElementEventHandler? Focusing
    {
        add => ElementShim.AddHandler(s_eventFocusing, value);
        remove => ElementShim.RemoveHandler(s_eventFocusing, value);
    }

    public event HtmlElementEventHandler? GotFocus
    {
        add => ElementShim.AddHandler(s_eventGotFocus, value);
        remove => ElementShim.RemoveHandler(s_eventGotFocus, value);
    }

    public event HtmlElementEventHandler? LosingFocus
    {
        add => ElementShim.AddHandler(s_eventLosingFocus, value);
        remove => ElementShim.RemoveHandler(s_eventLosingFocus, value);
    }

    public event HtmlElementEventHandler? LostFocus
    {
        add => ElementShim.AddHandler(s_eventLostFocus, value);
        remove => ElementShim.RemoveHandler(s_eventLostFocus, value);
    }

    public event HtmlElementEventHandler? KeyDown
    {
        add => ElementShim.AddHandler(s_eventKeyDown, value);
        remove => ElementShim.RemoveHandler(s_eventKeyDown, value);
    }

    public event HtmlElementEventHandler? KeyPress
    {
        add => ElementShim.AddHandler(s_eventKeyPress, value);
        remove => ElementShim.RemoveHandler(s_eventKeyPress, value);
    }

    public event HtmlElementEventHandler? KeyUp
    {
        add => ElementShim.AddHandler(s_eventKeyUp, value);
        remove => ElementShim.RemoveHandler(s_eventKeyUp, value);
    }

    public event HtmlElementEventHandler? MouseMove
    {
        add => ElementShim.AddHandler(s_eventMouseMove, value);
        remove => ElementShim.RemoveHandler(s_eventMouseMove, value);
    }

    public event HtmlElementEventHandler? MouseDown
    {
        add => ElementShim.AddHandler(s_eventMouseDown, value);
        remove => ElementShim.RemoveHandler(s_eventMouseDown, value);
    }

    public event HtmlElementEventHandler? MouseOver
    {
        add => ElementShim.AddHandler(s_eventMouseOver, value);
        remove => ElementShim.RemoveHandler(s_eventMouseOver, value);
    }

    public event HtmlElementEventHandler? MouseUp
    {
        add => ElementShim.AddHandler(s_eventMouseUp, value);
        remove => ElementShim.RemoveHandler(s_eventMouseUp, value);
    }

    /// <summary>
    ///  Fires when the mouse enters the element
    /// </summary>
    public event HtmlElementEventHandler? MouseEnter
    {
        add => ElementShim.AddHandler(s_eventMouseEnter, value);
        remove => ElementShim.RemoveHandler(s_eventMouseEnter, value);
    }

    /// <summary>
    ///  Fires when the mouse leaves the element
    /// </summary>
    public event HtmlElementEventHandler? MouseLeave
    {
        add => ElementShim.AddHandler(s_eventMouseLeave, value);
        remove => ElementShim.RemoveHandler(s_eventMouseLeave, value);
    }

    public static unsafe bool operator ==(HtmlElement? left, HtmlElement? right)
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
        return left.NativeHtmlElement.IsSameNativeObject(right.NativeHtmlElement);
    }

    public static bool operator !=(HtmlElement? left, HtmlElement? right) => !(left == right);

    public override int GetHashCode() => _hashCode;

    public override bool Equals(object? obj) => this == (obj as HtmlElement);
}
