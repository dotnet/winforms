// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop.Mshtml;

namespace System.Windows.Forms
{
    public sealed class HtmlElement
    {
        internal static readonly object EventClick = new object();
        internal static readonly object EventDoubleClick = new object();
        internal static readonly object EventDrag = new object();
        internal static readonly object EventDragEnd = new object();
        internal static readonly object EventDragLeave = new object();
        internal static readonly object EventDragOver = new object();
        internal static readonly object EventFocusing = new object();
        internal static readonly object EventGotFocus = new object();
        internal static readonly object EventLosingFocus = new object();
        internal static readonly object EventLostFocus = new object();
        internal static readonly object EventKeyDown = new object();
        internal static readonly object EventKeyPress = new object();
        internal static readonly object EventKeyUp = new object();
        internal static readonly object EventMouseDown = new object();
        internal static readonly object EventMouseEnter = new object();
        internal static readonly object EventMouseLeave = new object();
        internal static readonly object EventMouseMove = new object();
        internal static readonly object EventMouseOver = new object();
        internal static readonly object EventMouseUp = new object();

        private readonly IHTMLElement htmlElement;
        private readonly HtmlShimManager shimManager;

        internal HtmlElement(HtmlShimManager shimManager, IHTMLElement element)
        {
            htmlElement = element;
            Debug.Assert(NativeHtmlElement != null, "The element object should implement IHTMLElement");

            this.shimManager = shimManager;

        }

        public HtmlElementCollection All
        {
            get
            {
                return NativeHtmlElement.GetAll() is IHTMLElementCollection iHTMLElementCollection ? new HtmlElementCollection(shimManager, iHTMLElementCollection) : new HtmlElementCollection(shimManager);
            }
        }

        public HtmlElementCollection Children
        {
            get
            {
                return NativeHtmlElement.GetChildren() is IHTMLElementCollection iHTMLElementCollection ? new HtmlElementCollection(shimManager, iHTMLElementCollection) : new HtmlElementCollection(shimManager);
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
                return NativeHtmlElement.GetDocument() is IHTMLDocument iHTMLDocument ? new HtmlDocument(shimManager, iHTMLDocument) : null;
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
                if (ShimManager != null)
                {
                    HtmlElementShim shim = ShimManager.GetElementShim(this);
                    if (shim == null)
                    {
                        shimManager.AddElementShim(this);
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
                return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
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
                return htmlElement;
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
                return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
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
                return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
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
                return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
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
                return shimManager;
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
            return oAttributeValue == null ? "" : oAttributeValue.ToString();
        }

        public HtmlElementCollection GetElementsByTagName(string tagName)
        {
            IHTMLElementCollection iHTMLElementCollection = ((IHTMLElement2)NativeHtmlElement).GetElementsByTagName(tagName);
            return iHTMLElementCollection != null ? new HtmlElementCollection(shimManager, iHTMLElementCollection) : new HtmlElementCollection(shimManager);
        }
        public HtmlElement InsertAdjacentElement(HtmlElementInsertionOrientation orient, HtmlElement newElement)
        {
            IHTMLElement iHtmlElement = ((IHTMLElement2)NativeHtmlElement).InsertAdjacentElement(orient.ToString(),
                (IHTMLElement)newElement.DomElement);
            return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
        }

        public object InvokeMember(string methodName)
        {
            return InvokeMember(methodName, null);
        }

        public object InvokeMember(string methodName, params object[] parameter)
        {
            object retVal = null;
            NativeMethods.tagDISPPARAMS dp = new NativeMethods.tagDISPPARAMS
            {
                rgvarg = IntPtr.Zero
            };
            try
            {
                if (NativeHtmlElement is UnsafeNativeMethods.IDispatch scriptObject)
                {
                    Guid g = Guid.Empty;
                    string[] names = new string[] { methodName };
                    int[] dispids = new int[] { NativeMethods.ActiveX.DISPID_UNKNOWN };
                    int hr = scriptObject.GetIDsOfNames(ref g, names, 1,
                                                   SafeNativeMethods.GetThreadLCID(), dispids);
                    if (NativeMethods.Succeeded(hr) && (dispids[0] != NativeMethods.ActiveX.DISPID_UNKNOWN))
                    {
                        // Reverse the arg order below so that parms are read properly thru IDispatch. (
                        if (parameter != null)
                        {
                            // Reverse the parm order so that they read naturally after IDispatch. (
                            Array.Reverse(parameter);
                        }
                        dp.rgvarg = (parameter == null) ? IntPtr.Zero : HtmlDocument.ArrayToVARIANTVector(parameter);
                        dp.cArgs = (parameter == null) ? 0 : parameter.Length;
                        dp.rgdispidNamedArgs = IntPtr.Zero;
                        dp.cNamedArgs = 0;

                        object[] retVals = new object[1];

                        hr = scriptObject.Invoke(dispids[0], ref g, SafeNativeMethods.GetThreadLCID(),
                                NativeMethods.DISPATCH_METHOD, dp,
                                retVals, new NativeMethods.tagEXCEPINFO(), null);
                        if (hr == NativeMethods.S_OK)
                        {
                            retVal = retVals[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }
            finally
            {
                if (dp.rgvarg != IntPtr.Zero)
                {
                    HtmlDocument.FreeVARIANTVector(dp.rgvarg, parameter.Length);
                }
            }
            return retVal;
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
            add => ElementShim.AddHandler(EventClick, value);
            remove => ElementShim.RemoveHandler(EventClick, value);

        }

        public event HtmlElementEventHandler DoubleClick
        {
            add => ElementShim.AddHandler(EventDoubleClick, value);
            remove => ElementShim.RemoveHandler(EventDoubleClick, value);
        }

        public event HtmlElementEventHandler Drag
        {
            add => ElementShim.AddHandler(EventDrag, value);
            remove => ElementShim.RemoveHandler(EventDrag, value);
        }

        public event HtmlElementEventHandler DragEnd
        {
            add => ElementShim.AddHandler(EventDragEnd, value);
            remove => ElementShim.RemoveHandler(EventDragEnd, value);
        }

        public event HtmlElementEventHandler DragLeave
        {
            add => ElementShim.AddHandler(EventDragLeave, value);
            remove => ElementShim.RemoveHandler(EventDragLeave, value);

        }

        public event HtmlElementEventHandler DragOver
        {
            add => ElementShim.AddHandler(EventDragOver, value);
            remove => ElementShim.RemoveHandler(EventDragOver, value);
        }

        public event HtmlElementEventHandler Focusing
        {
            add => ElementShim.AddHandler(EventFocusing, value);
            remove => ElementShim.RemoveHandler(EventFocusing, value);
        }

        public event HtmlElementEventHandler GotFocus
        {
            add => ElementShim.AddHandler(EventGotFocus, value);
            remove => ElementShim.RemoveHandler(EventGotFocus, value);

        }

        public event HtmlElementEventHandler LosingFocus
        {
            add => ElementShim.AddHandler(EventLosingFocus, value);
            remove => ElementShim.RemoveHandler(EventLosingFocus, value);
        }

        public event HtmlElementEventHandler LostFocus
        {
            add => ElementShim.AddHandler(EventLostFocus, value);
            remove => ElementShim.RemoveHandler(EventLostFocus, value);
        }

        public event HtmlElementEventHandler KeyDown
        {
            add => ElementShim.AddHandler(EventKeyDown, value);
            remove => ElementShim.RemoveHandler(EventKeyDown, value);
        }
        public event HtmlElementEventHandler KeyPress
        {
            add => ElementShim.AddHandler(EventKeyPress, value);
            remove => ElementShim.RemoveHandler(EventKeyPress, value);

        }
        public event HtmlElementEventHandler KeyUp
        {
            add => ElementShim.AddHandler(EventKeyUp, value);
            remove => ElementShim.RemoveHandler(EventKeyUp, value);
        }

        public event HtmlElementEventHandler MouseMove
        {
            add => ElementShim.AddHandler(EventMouseMove, value);
            remove => ElementShim.RemoveHandler(EventMouseMove, value);
        }
        public event HtmlElementEventHandler MouseDown
        {
            add => ElementShim.AddHandler(EventMouseDown, value);
            remove => ElementShim.RemoveHandler(EventMouseDown, value);
        }
        public event HtmlElementEventHandler MouseOver
        {
            add => ElementShim.AddHandler(EventMouseOver, value);
            remove => ElementShim.RemoveHandler(EventMouseOver, value);
        }

        public event HtmlElementEventHandler MouseUp
        {
            add => ElementShim.AddHandler(EventMouseUp, value);
            remove => ElementShim.RemoveHandler(EventMouseUp, value);
        }

        /// <summary>
        ///  Fires when the mouse enters the element
        /// </summary>
        public event HtmlElementEventHandler MouseEnter
        {
            add => ElementShim.AddHandler(EventMouseEnter, value);
            remove => ElementShim.RemoveHandler(EventMouseEnter, value);
        }

        /// <summary>
        ///  Fires when the mouse leaves the element
        /// </summary>
        public event HtmlElementEventHandler MouseLeave
        {
            add => ElementShim.AddHandler(EventMouseLeave, value);
            remove => ElementShim.RemoveHandler(EventMouseLeave, value);
        }

        //
        // Private classes:
        //
        [ClassInterface(ClassInterfaceType.None)]
        private class HTMLElementEvents2 : StandardOleMarshalObject, /*Enforce calling back on the same thread*/
                                           DHTMLElementEvents2,
                                           DHTMLAnchorEvents2,
                                           DHTMLAreaEvents2,
                                           DHTMLButtonElementEvents2,
                                           DHTMLControlElementEvents2,
                                           DHTMLFormElementEvents2,
                                           DHTMLFrameSiteEvents2,
                                           DHTMLImgEvents2,
                                           DHTMLInputFileElementEvents2,
                                           DHTMLInputImageEvents2,
                                           DHTMLInputTextElementEvents2,
                                           DHTMLLabelEvents2,
                                           DHTMLLinkElementEvents2,
                                           DHTMLMapEvents2,
                                           DHTMLMarqueeElementEvents2,
                                           DHTMLOptionButtonElementEvents2,
                                           DHTMLSelectElementEvents2,
                                           DHTMLStyleElementEvents2,
                                           DHTMLTableEvents2,
                                           DHTMLTextContainerEvents2,
                                           DHTMLScriptEvents2
        {
            private readonly HtmlElement parent;

            public HTMLElementEvents2(HtmlElement htmlElement)
            {
                parent = htmlElement;
            }
            private void FireEvent(object key, EventArgs e)
            {
                if (parent != null)
                {
                    parent.ElementShim.FireEvent(key, e);
                }
            }

            public bool onclick(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventClick, e);
                return e.ReturnValue;
            }

            public bool ondblclick(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventDoubleClick, e);
                return e.ReturnValue;
            }

            public bool onkeypress(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventKeyPress, e);
                return e.ReturnValue;
            }

            public void onkeydown(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventKeyDown, e);
            }

            public void onkeyup(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventKeyUp, e);
            }

            public void onmouseover(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventMouseOver, e);
            }

            public void onmousemove(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventMouseMove, e);
            }

            public void onmousedown(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventMouseDown, e);
            }

            public void onmouseup(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventMouseUp, e);
            }

            public void onmouseenter(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventMouseEnter, e);
            }

            public void onmouseleave(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventMouseLeave, e);
            }

            public bool onerrorupdate(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onfocus(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventGotFocus, e);
            }

            public bool ondrag(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventDrag, e);
                return e.ReturnValue;
            }

            public void ondragend(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventDragEnd, e);
            }

            public void ondragleave(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventDragLeave, e);
            }

            public bool ondragover(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventDragOver, e);
                return e.ReturnValue;
            }

            public void onfocusin(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventFocusing, e);
            }

            public void onfocusout(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventLosingFocus, e);
            }

            public void onblur(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventLostFocus, e);
            }

            public void onresizeend(IHTMLEventObj evtObj) { }

            public bool onresizestart(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onhelp(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onmouseout(IHTMLEventObj evtObj) { }

            public bool onselectstart(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onfilterchange(IHTMLEventObj evtObj) { }

            public bool ondragstart(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onbeforeupdate(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onafterupdate(IHTMLEventObj evtObj) { }

            public bool onrowexit(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onrowenter(IHTMLEventObj evtObj) { }

            public void ondatasetchanged(IHTMLEventObj evtObj) { }

            public void ondataavailable(IHTMLEventObj evtObj) { }

            public void ondatasetcomplete(IHTMLEventObj evtObj) { }

            public void onlosecapture(IHTMLEventObj evtObj) { }

            public void onpropertychange(IHTMLEventObj evtObj) { }

            public void onscroll(IHTMLEventObj evtObj) { }

            public void onresize(IHTMLEventObj evtObj) { }

            public bool ondragenter(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool ondrop(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onbeforecut(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool oncut(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onbeforecopy(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool oncopy(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onbeforepaste(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onpaste(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool oncontextmenu(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onrowsdelete(IHTMLEventObj evtObj) { }

            public void onrowsinserted(IHTMLEventObj evtObj) { }

            public void oncellchange(IHTMLEventObj evtObj) { }

            public void onreadystatechange(IHTMLEventObj evtObj) { }

            public void onlayoutcomplete(IHTMLEventObj evtObj) { }

            public void onpage(IHTMLEventObj evtObj) { }

            public void onactivate(IHTMLEventObj evtObj) { }

            public void ondeactivate(IHTMLEventObj evtObj) { }

            public bool onbeforedeactivate(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onbeforeactivate(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onmove(IHTMLEventObj evtObj) { }

            public bool oncontrolselect(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onmovestart(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onmoveend(IHTMLEventObj evtObj) { }

            public bool onmousewheel(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onchange(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onselect(IHTMLEventObj evtObj) { }

            public void onload(IHTMLEventObj evtObj) { }

            public void onerror(IHTMLEventObj evtObj) { }

            public void onabort(IHTMLEventObj evtObj) { }

            public bool onsubmit(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onreset(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onchange_void(IHTMLEventObj evtObj) { }

            public void onbounce(IHTMLEventObj evtObj) { }

            public void onfinish(IHTMLEventObj evtObj) { }

            public void onstart(IHTMLEventObj evtObj) { }
        }

        ///<summary>
        ///  HtmlElementShim - this is the glue between the DOM eventing mechanisms
        ///          and our CLR callbacks.
        ///
        ///  HTMLElementEvents2: we create an IConnectionPoint (via ConnectionPointCookie) between us and MSHTML and it calls back
        ///              on our an instance of HTMLElementEvents2.  The HTMLElementEvents2 class then fires the event.
        ///
        ///</summary>
        internal class HtmlElementShim : HtmlShim
        {
            private static readonly Type[] dispInterfaceTypes = {typeof(DHTMLElementEvents2),
                                                    typeof(DHTMLAnchorEvents2),
                                                    typeof(DHTMLAreaEvents2),
                                                    typeof(DHTMLButtonElementEvents2),
                                                    typeof(DHTMLControlElementEvents2),
                                                    typeof(DHTMLFormElementEvents2),
                                                    typeof(DHTMLFrameSiteEvents2),
                                                    typeof(DHTMLImgEvents2),
                                                    typeof(DHTMLInputFileElementEvents2),
                                                    typeof(DHTMLInputImageEvents2),
                                                    typeof(DHTMLInputTextElementEvents2),
                                                    typeof(DHTMLLabelEvents2),
                                                    typeof(DHTMLLinkElementEvents2),
                                                    typeof(DHTMLMapEvents2),
                                                    typeof(DHTMLMarqueeElementEvents2),
                                                    typeof(DHTMLOptionButtonElementEvents2),
                                                    typeof(DHTMLSelectElementEvents2),
                                                    typeof(DHTMLStyleElementEvents2),
                                                    typeof(DHTMLTableEvents2),
                                                    typeof(DHTMLTextContainerEvents2),
                                                    typeof(DHTMLScriptEvents2)};

            private AxHost.ConnectionPointCookie cookie;   // To hook up events from the native HtmlElement
            private HtmlElement htmlElement;
            private readonly IHTMLWindow2 associatedWindow = null;

            public HtmlElementShim(HtmlElement element)
            {
                htmlElement = element;

                // snap our associated window so we know when to disconnect.
                if (htmlElement != null)
                {
                    HtmlDocument doc = htmlElement.Document;
                    if (doc != null)
                    {
                        HtmlWindow window = doc.Window;
                        if (window != null)
                        {
                            associatedWindow = window.NativeHtmlWindow;
                        }
                    }
                }
            }

            public IHTMLElement NativeHtmlElement
            {
                get { return htmlElement.NativeHtmlElement; }
            }

            internal HtmlElement Element
            {
                get { return htmlElement; }
            }

            public override IHTMLWindow2 AssociatedWindow
            {
                get { return associatedWindow; }
            }

            ///  Support IHTMLElement2.AttachEventHandler
            public override void AttachEventHandler(string eventName, EventHandler eventHandler)
            {

                // IE likes to call back on an IDispatch of DISPID=0 when it has an event,
                // the HtmlToClrEventProxy helps us fake out the CLR so that we can call back on
                // our EventHandler properly.

                HtmlToClrEventProxy proxy = AddEventProxy(eventName, eventHandler);
                bool success = ((IHTMLElement2)NativeHtmlElement).AttachEvent(eventName, proxy);
                Debug.Assert(success, "failed to add event");
            }

            public override void ConnectToEvents()
            {
                if (cookie == null || !cookie.Connected)
                {
                    for (int i = 0; i < dispInterfaceTypes.Length && cookie == null; i++)
                    {
                        cookie = new AxHost.ConnectionPointCookie(NativeHtmlElement,
                                                                                  new HTMLElementEvents2(htmlElement),
                                                                                  dispInterfaceTypes[i],
                                                                                  /*throwException*/ false);
                        if (!cookie.Connected)
                        {
                            cookie = null;
                        }
                    }
                }
            }

            ///  Support IHTMLElement2.DetachHandler
            public override void DetachEventHandler(string eventName, EventHandler eventHandler)
            {
                HtmlToClrEventProxy proxy = RemoveEventProxy(eventHandler);
                if (proxy != null)
                {
                    ((IHTMLElement2)NativeHtmlElement).DetachEvent(eventName, proxy);
                }
            }

            public override void DisconnectFromEvents()
            {
                if (cookie != null)
                {
                    cookie.Disconnect();
                    cookie = null;
                }

            }
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (htmlElement != null)
                {
                    Marshal.FinalReleaseComObject(htmlElement.NativeHtmlElement);
                }
                htmlElement = null;
            }

            protected override object GetEventSender()
            {
                return htmlElement;
            }

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

        public override int GetHashCode() => htmlElement?.GetHashCode() ?? 0;

        public override bool Equals(object obj)
        {
            //If obj isn't an HtmlElement, we want Equals to return false.  this will
            //never be null, so now it will return false as expected (not throw).
            return this == (obj as HtmlElement);
        }
        #endregion

    }
}

