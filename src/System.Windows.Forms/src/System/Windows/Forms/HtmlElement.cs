// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.Mshtml;

namespace System.Windows.Forms
{
    public sealed partial class HtmlElement
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

        private readonly IHTMLElement _htmlElement;
        private readonly HtmlShimManager _shimManager;

        internal HtmlElement(HtmlShimManager shimManager, IHTMLElement element)
        {
            _htmlElement = element;
            Debug.Assert(NativeHtmlElement is not null, "The element object should implement IHTMLElement");

            _shimManager = shimManager;
        }

        public HtmlElementCollection All
        {
            get
            {
                return NativeHtmlElement.GetAll() is IHTMLElementCollection iHTMLElementCollection ? new HtmlElementCollection(_shimManager, iHTMLElementCollection) : new HtmlElementCollection(_shimManager);
            }
        }

        public HtmlElementCollection Children
        {
            get
            {
                return NativeHtmlElement.GetChildren() is IHTMLElementCollection iHTMLElementCollection ? new HtmlElementCollection(_shimManager, iHTMLElementCollection) : new HtmlElementCollection(_shimManager);
            }
        }

        public bool CanHaveChildren
        {
            get
            {
                return ((IHTMLElement2)NativeHtmlElement).CanHaveChildren();
            }
        }

        public Rectangle ClientRectangle
        {
            get
            {
                IHTMLElement2 htmlElement2 = (IHTMLElement2)NativeHtmlElement;
                return new Rectangle(htmlElement2.ClientLeft(), htmlElement2.ClientTop(),
                    htmlElement2.ClientWidth(), htmlElement2.ClientHeight());
            }
        }

        public HtmlDocument Document
        {
            get
            {
                return NativeHtmlElement.GetDocument() is IHTMLDocument iHTMLDocument ? new HtmlDocument(_shimManager, iHTMLDocument) : null;
            }
        }

        public bool Enabled
        {
            get
            {
                return !(((IHTMLElement3)NativeHtmlElement).GetDisabled());
            }
            set
            {
                ((IHTMLElement3)NativeHtmlElement).SetDisabled(!value);
            }
        }

        private HtmlElementShim ElementShim
        {
            get
            {
                if (ShimManager is not null)
                {
                    HtmlElementShim shim = ShimManager.GetElementShim(this);
                    if (shim is null)
                    {
                        _shimManager.AddElementShim(this);
                        shim = ShimManager.GetElementShim(this);
                    }

                    return shim;
                }

                return null;
            }
        }

        public HtmlElement FirstChild
        {
            get
            {
                IHTMLElement iHtmlElement = null;

                if (NativeHtmlElement is IHTMLDOMNode iHtmlDomNode)
                {
                    iHtmlElement = iHtmlDomNode.FirstChild() as IHTMLElement;
                }

                return iHtmlElement is not null ? new HtmlElement(_shimManager, iHtmlElement) : null;
            }
        }

        public string Id
        {
            get
            {
                return NativeHtmlElement.GetId();
            }
            set
            {
                NativeHtmlElement.SetId(value);
            }
        }

        public string InnerHtml
        {
            get
            {
                return NativeHtmlElement.GetInnerHTML();
            }
            set
            {
                try
                {
                    NativeHtmlElement.SetInnerHTML(value);
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
                return NativeHtmlElement.GetInnerText();
            }
            set
            {
                try
                {
                    NativeHtmlElement.SetInnerText(value);
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
            get
            {
                return GetAttribute("Name");
            }
            set
            {
                SetAttribute("Name", value);
            }
        }

        private IHTMLElement NativeHtmlElement
        {
            get
            {
                return _htmlElement;
            }
        }

        public HtmlElement NextSibling
        {
            get
            {
                IHTMLElement iHtmlElement = null;

                if (NativeHtmlElement is IHTMLDOMNode iHtmlDomNode)
                {
                    iHtmlElement = iHtmlDomNode.NextSibling() as IHTMLElement;
                }

                return iHtmlElement is not null ? new HtmlElement(_shimManager, iHtmlElement) : null;
            }
        }

        public Rectangle OffsetRectangle
        {
            get
            {
                return new Rectangle(NativeHtmlElement.GetOffsetLeft(), NativeHtmlElement.GetOffsetTop(),
                    NativeHtmlElement.GetOffsetWidth(), NativeHtmlElement.GetOffsetHeight());
            }
        }

        public HtmlElement OffsetParent
        {
            get
            {
                IHTMLElement iHtmlElement = NativeHtmlElement.GetOffsetParent();
                return iHtmlElement is not null ? new HtmlElement(_shimManager, iHtmlElement) : null;
            }
        }

        public string OuterHtml
        {
            get
            {
                return NativeHtmlElement.GetOuterHTML();
            }
            set
            {
                try
                {
                    NativeHtmlElement.SetOuterHTML(value);
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
                return NativeHtmlElement.GetOuterText();
            }
            set
            {
                try
                {
                    NativeHtmlElement.SetOuterText(value);
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

        public HtmlElement Parent
        {
            get
            {
                IHTMLElement iHtmlElement = NativeHtmlElement.GetParentElement();
                return iHtmlElement is not null ? new HtmlElement(_shimManager, iHtmlElement) : null;
            }
        }

        public Rectangle ScrollRectangle
        {
            get
            {
                IHTMLElement2 htmlElement2 = (IHTMLElement2)NativeHtmlElement;
                return new Rectangle(htmlElement2.GetScrollLeft(), htmlElement2.GetScrollTop(),
                    htmlElement2.GetScrollWidth(), htmlElement2.GetScrollHeight());
            }
        }

        public int ScrollLeft
        {
            get
            {
                return ((IHTMLElement2)NativeHtmlElement).GetScrollLeft();
            }
            set
            {
                ((IHTMLElement2)NativeHtmlElement).SetScrollLeft(value);
            }
        }

        public int ScrollTop
        {
            get
            {
                return ((IHTMLElement2)NativeHtmlElement).GetScrollTop();
            }
            set
            {
                ((IHTMLElement2)NativeHtmlElement).SetScrollTop(value);
            }
        }

        private HtmlShimManager ShimManager
        {
            get
            {
                return _shimManager;
            }
        }

        public string Style
        {
            get
            {
                return NativeHtmlElement.GetStyle().GetCssText();
            }
            set
            {
                NativeHtmlElement.GetStyle().SetCssText(value);
            }
        }

        public string TagName
        {
            get
            {
                return NativeHtmlElement.GetTagName();
            }
        }

        public short TabIndex
        {
            get
            {
                return ((IHTMLElement2)NativeHtmlElement).GetTabIndex();
            }
            set
            {
                ((IHTMLElement2)NativeHtmlElement).SetTabIndex(value);
            }
        }

        public object DomElement
        {
            get
            {
                return NativeHtmlElement;
            }
        }

        public HtmlElement AppendChild(HtmlElement newElement)
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
                ((IHTMLElement2)NativeHtmlElement).Focus();
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
            object oAttributeValue = NativeHtmlElement.GetAttribute(attributeName, 0);
            return oAttributeValue is null ? "" : oAttributeValue.ToString();
        }

        public HtmlElementCollection GetElementsByTagName(string tagName)
        {
            IHTMLElementCollection iHTMLElementCollection = ((IHTMLElement2)NativeHtmlElement).GetElementsByTagName(tagName);
            return iHTMLElementCollection is not null ? new HtmlElementCollection(_shimManager, iHTMLElementCollection) : new HtmlElementCollection(_shimManager);
        }

        public HtmlElement InsertAdjacentElement(HtmlElementInsertionOrientation orient, HtmlElement newElement)
        {
            IHTMLElement iHtmlElement = ((IHTMLElement2)NativeHtmlElement).InsertAdjacentElement(orient.ToString(),
                (IHTMLElement)newElement.DomElement);
            return iHtmlElement is not null ? new HtmlElement(_shimManager, iHtmlElement) : null;
        }

        public object InvokeMember(string methodName)
        {
            return InvokeMember(methodName, null);
        }

        public unsafe object InvokeMember(string methodName, params object[] parameter)
        {
            try
            {
                if (NativeHtmlElement is Oleaut32.IDispatch scriptObject)
                {
                    Guid g = Guid.Empty;
                    var names = new string[] { methodName };
                    Ole32.DispatchID dispid = Ole32.DispatchID.UNKNOWN;
                    HRESULT hr = scriptObject.GetIDsOfNames(&g, names, 1, PInvoke.GetThreadLocale(), &dispid);
                    if (!hr.Succeeded || dispid == Ole32.DispatchID.UNKNOWN)
                    {
                        return null;
                    }

                    if (parameter is not null)
                    {
                        // Reverse the parameter order so that they read naturally after IDispatch.
                        Array.Reverse(parameter);
                    }

                    using var vectorArgs = new Oleaut32.VARIANTVector(parameter);
                    fixed (Oleaut32.VARIANT* pVariant = vectorArgs.Variants)
                    {
                        var dispParams = new Oleaut32.DISPPARAMS();
                        dispParams.rgvarg = pVariant;
                        dispParams.cArgs = (uint)vectorArgs.Variants.Length;
                        dispParams.rgdispidNamedArgs = null;
                        dispParams.cNamedArgs = 0;

                        var retVals = new object[1];
                        var excepInfo = new Oleaut32.EXCEPINFO();
                        hr = scriptObject.Invoke(
                            dispid,
                            &g,
                            PInvoke.GetThreadLocale(),
                            Oleaut32.DISPATCH.METHOD,
                            &dispParams,
                            retVals,
                            &excepInfo,
                            null);
                        if (hr == HRESULT.Values.S_OK)
                        {
                            return retVals[0];
                        }
                    }
                }
            }
            catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
            {
            }

            return null;
        }

        public void RemoveFocus()
        {
            ((IHTMLElement2)NativeHtmlElement).Blur();
        }

        // PM review done
        public void RaiseEvent(string eventName)
        {
            ((IHTMLElement3)NativeHtmlElement).FireEvent(eventName, IntPtr.Zero);
        }

        public void ScrollIntoView(bool alignWithTop)
        {
            NativeHtmlElement.ScrollIntoView((object)alignWithTop);
        }

        public void SetAttribute(string attributeName, string value)
        {
            try
            {
                NativeHtmlElement.SetAttribute(attributeName, (object)value, 0);
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
        public event HtmlElementEventHandler Click
        {
            add => ElementShim.AddHandler(s_eventClick, value);
            remove => ElementShim.RemoveHandler(s_eventClick, value);
        }

        public event HtmlElementEventHandler DoubleClick
        {
            add => ElementShim.AddHandler(s_eventDoubleClick, value);
            remove => ElementShim.RemoveHandler(s_eventDoubleClick, value);
        }

        public event HtmlElementEventHandler Drag
        {
            add => ElementShim.AddHandler(s_eventDrag, value);
            remove => ElementShim.RemoveHandler(s_eventDrag, value);
        }

        public event HtmlElementEventHandler DragEnd
        {
            add => ElementShim.AddHandler(s_eventDragEnd, value);
            remove => ElementShim.RemoveHandler(s_eventDragEnd, value);
        }

        public event HtmlElementEventHandler DragLeave
        {
            add => ElementShim.AddHandler(s_eventDragLeave, value);
            remove => ElementShim.RemoveHandler(s_eventDragLeave, value);
        }

        public event HtmlElementEventHandler DragOver
        {
            add => ElementShim.AddHandler(s_eventDragOver, value);
            remove => ElementShim.RemoveHandler(s_eventDragOver, value);
        }

        public event HtmlElementEventHandler Focusing
        {
            add => ElementShim.AddHandler(s_eventFocusing, value);
            remove => ElementShim.RemoveHandler(s_eventFocusing, value);
        }

        public event HtmlElementEventHandler GotFocus
        {
            add => ElementShim.AddHandler(s_eventGotFocus, value);
            remove => ElementShim.RemoveHandler(s_eventGotFocus, value);
        }

        public event HtmlElementEventHandler LosingFocus
        {
            add => ElementShim.AddHandler(s_eventLosingFocus, value);
            remove => ElementShim.RemoveHandler(s_eventLosingFocus, value);
        }

        public event HtmlElementEventHandler LostFocus
        {
            add => ElementShim.AddHandler(s_eventLostFocus, value);
            remove => ElementShim.RemoveHandler(s_eventLostFocus, value);
        }

        public event HtmlElementEventHandler KeyDown
        {
            add => ElementShim.AddHandler(s_eventKeyDown, value);
            remove => ElementShim.RemoveHandler(s_eventKeyDown, value);
        }

        public event HtmlElementEventHandler KeyPress
        {
            add => ElementShim.AddHandler(s_eventKeyPress, value);
            remove => ElementShim.RemoveHandler(s_eventKeyPress, value);
        }

        public event HtmlElementEventHandler KeyUp
        {
            add => ElementShim.AddHandler(s_eventKeyUp, value);
            remove => ElementShim.RemoveHandler(s_eventKeyUp, value);
        }

        public event HtmlElementEventHandler MouseMove
        {
            add => ElementShim.AddHandler(s_eventMouseMove, value);
            remove => ElementShim.RemoveHandler(s_eventMouseMove, value);
        }

        public event HtmlElementEventHandler MouseDown
        {
            add => ElementShim.AddHandler(s_eventMouseDown, value);
            remove => ElementShim.RemoveHandler(s_eventMouseDown, value);
        }

        public event HtmlElementEventHandler MouseOver
        {
            add => ElementShim.AddHandler(s_eventMouseOver, value);
            remove => ElementShim.RemoveHandler(s_eventMouseOver, value);
        }

        public event HtmlElementEventHandler MouseUp
        {
            add => ElementShim.AddHandler(s_eventMouseUp, value);
            remove => ElementShim.RemoveHandler(s_eventMouseUp, value);
        }

        /// <summary>
        ///  Fires when the mouse enters the element
        /// </summary>
        public event HtmlElementEventHandler MouseEnter
        {
            add => ElementShim.AddHandler(s_eventMouseEnter, value);
            remove => ElementShim.RemoveHandler(s_eventMouseEnter, value);
        }

        /// <summary>
        ///  Fires when the mouse leaves the element
        /// </summary>
        public event HtmlElementEventHandler MouseLeave
        {
            add => ElementShim.AddHandler(s_eventMouseLeave, value);
            remove => ElementShim.RemoveHandler(s_eventMouseLeave, value);
        }

        #region operators
        public static bool operator ==(HtmlElement left, HtmlElement right)
        {
            //Not equal if only one's null.
            if (left is null != right is null)
            {
                return false;
            }

            //Equal if both are null.
            if (left is null)
            {
                return true;
            }

            //Neither are null.  Get the IUnknowns and compare them.
            IntPtr leftPtr = IntPtr.Zero;
            IntPtr rightPtr = IntPtr.Zero;
            try
            {
                leftPtr = Marshal.GetIUnknownForObject(left.NativeHtmlElement);
                rightPtr = Marshal.GetIUnknownForObject(right.NativeHtmlElement);
                return leftPtr == rightPtr;
            }
            finally
            {
                if (leftPtr != IntPtr.Zero)
                {
                    Marshal.Release(leftPtr);
                }

                if (rightPtr != IntPtr.Zero)
                {
                    Marshal.Release(rightPtr);
                }
            }
        }

        public static bool operator !=(HtmlElement left, HtmlElement right)
        {
            return !(left == right);
        }

        public override int GetHashCode() => _htmlElement?.GetHashCode() ?? 0;

        public override bool Equals(object obj)
        {
            //If obj isn't an HtmlElement, we want Equals to return false.  this will
            //never be null, so now it will return false as expected (not throw).
            return this == (obj as HtmlElement);
        }
        #endregion

    }
}
