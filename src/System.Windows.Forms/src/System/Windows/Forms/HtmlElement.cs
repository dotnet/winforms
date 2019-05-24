// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;

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

        private UnsafeNativeMethods.IHTMLElement htmlElement;
        private HtmlShimManager shimManager;

        internal HtmlElement(HtmlShimManager shimManager, UnsafeNativeMethods.IHTMLElement element)
        {
            htmlElement = element;
            Debug.Assert(NativeHtmlElement != null, "The element object should implement IHTMLElement");

            this.shimManager = shimManager;

        }

        public HtmlElementCollection All
        {
            get
            {
                UnsafeNativeMethods.IHTMLElementCollection iHTMLElementCollection = NativeHtmlElement.GetAll() as UnsafeNativeMethods.IHTMLElementCollection;
                return iHTMLElementCollection != null ? new HtmlElementCollection(shimManager, iHTMLElementCollection) : new HtmlElementCollection(shimManager);
            }
        }

        public HtmlElementCollection Children
        {
            get
            {
                UnsafeNativeMethods.IHTMLElementCollection iHTMLElementCollection = NativeHtmlElement.GetChildren() as UnsafeNativeMethods.IHTMLElementCollection;
                return iHTMLElementCollection != null ? new HtmlElementCollection(shimManager, iHTMLElementCollection) : new HtmlElementCollection(shimManager);
            }
        }

        public bool CanHaveChildren
        {
            get
            {
                return ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).CanHaveChildren();
            }
        }

        public Rectangle ClientRectangle
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement2 htmlElement2 = (UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement;
                return new Rectangle(htmlElement2.ClientLeft(), htmlElement2.ClientTop(),
                    htmlElement2.ClientWidth(), htmlElement2.ClientHeight());
            }
        }


        public HtmlDocument Document
        {
            get
            {
                UnsafeNativeMethods.IHTMLDocument iHTMLDocument = NativeHtmlElement.GetDocument() as UnsafeNativeMethods.IHTMLDocument;
                return iHTMLDocument != null ? new HtmlDocument(shimManager, iHTMLDocument) : null;
            }
        }

        public bool Enabled
        {
            get
            {
                return !(((UnsafeNativeMethods.IHTMLElement3)NativeHtmlElement).GetDisabled());
            }
            set
            {
                ((UnsafeNativeMethods.IHTMLElement3)NativeHtmlElement).SetDisabled(!value);
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
                UnsafeNativeMethods.IHTMLElement iHtmlElement = null;
                UnsafeNativeMethods.IHTMLDOMNode iHtmlDomNode = NativeHtmlElement as UnsafeNativeMethods.IHTMLDOMNode;

                if (iHtmlDomNode != null)
                {
                    iHtmlElement = iHtmlDomNode.FirstChild() as UnsafeNativeMethods.IHTMLElement;
                }
                return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
            }
        }

        public string Id
        {
            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
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


        private UnsafeNativeMethods.IHTMLElement NativeHtmlElement
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
                UnsafeNativeMethods.IHTMLElement iHtmlElement = null;
                UnsafeNativeMethods.IHTMLDOMNode iHtmlDomNode = NativeHtmlElement as UnsafeNativeMethods.IHTMLDOMNode;

                if (iHtmlDomNode != null)
                {
                    iHtmlElement = iHtmlDomNode.NextSibling() as UnsafeNativeMethods.IHTMLElement;
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
                UnsafeNativeMethods.IHTMLElement iHtmlElement = NativeHtmlElement.GetOffsetParent();
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
                UnsafeNativeMethods.IHTMLElement iHtmlElement = NativeHtmlElement.GetParentElement();
                return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
            }
        }



        public Rectangle ScrollRectangle
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement2 htmlElement2 = (UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement;
                return new Rectangle(htmlElement2.GetScrollLeft(), htmlElement2.GetScrollTop(),
                    htmlElement2.GetScrollWidth(), htmlElement2.GetScrollHeight());
            }
        }

        public int ScrollLeft
        {
            get
            {
                return ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).GetScrollLeft();
            }
            set
            {
                ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).SetScrollLeft(value);
            }
        }

        public int ScrollTop
        {
            get
            {
                return ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).GetScrollTop();
            }
            set
            {
                ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).SetScrollTop(value);
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
                return ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).GetTabIndex();
            }
            set
            {
                ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).SetTabIndex(value);
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
                ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).Focus();
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

        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public string GetAttribute(string attributeName)
        {
            object oAttributeValue = NativeHtmlElement.GetAttribute(attributeName, 0);
            return oAttributeValue == null ? "" : oAttributeValue.ToString();
        }

        public HtmlElementCollection GetElementsByTagName(string tagName)
        {
            UnsafeNativeMethods.IHTMLElementCollection iHTMLElementCollection = ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).GetElementsByTagName(tagName);
            return iHTMLElementCollection != null ? new HtmlElementCollection(shimManager, iHTMLElementCollection) : new HtmlElementCollection(shimManager);
        }
        public HtmlElement InsertAdjacentElement(HtmlElementInsertionOrientation orient, HtmlElement newElement)
        {
            UnsafeNativeMethods.IHTMLElement iHtmlElement = ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).InsertAdjacentElement(orient.ToString(),
                (UnsafeNativeMethods.IHTMLElement)newElement.DomElement);
            return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
        }

        public object InvokeMember(string methodName)
        {
            return InvokeMember(methodName, null);
        }

        public object InvokeMember(string methodName, params object[] parameter)
        {
            object retVal = null;
            NativeMethods.tagDISPPARAMS dp = new NativeMethods.tagDISPPARAMS();
            dp.rgvarg = IntPtr.Zero;
            try
            {
                UnsafeNativeMethods.IDispatch scriptObject = NativeHtmlElement as UnsafeNativeMethods.IDispatch;
                if (scriptObject != null)
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
            ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).Blur();
        }

        // PM review done
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void RaiseEvent(string eventName)
        {
            ((UnsafeNativeMethods.IHTMLElement3)NativeHtmlElement).FireEvent(eventName, IntPtr.Zero);
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
        ///    <para>Fires when the mouse enters the element</para>
        /// </summary>
        public event HtmlElementEventHandler MouseEnter
        {
            add => ElementShim.AddHandler(EventMouseEnter, value);
            remove => ElementShim.RemoveHandler(EventMouseEnter, value);
        }

        /// <summary>
        ///    <para>Fires when the mouse leaves the element</para>
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
                                           UnsafeNativeMethods.DHTMLElementEvents2,
                                           UnsafeNativeMethods.DHTMLAnchorEvents2,
                                           UnsafeNativeMethods.DHTMLAreaEvents2,
                                           UnsafeNativeMethods.DHTMLButtonElementEvents2,
                                           UnsafeNativeMethods.DHTMLControlElementEvents2,
                                           UnsafeNativeMethods.DHTMLFormElementEvents2,
                                           UnsafeNativeMethods.DHTMLFrameSiteEvents2,
                                           UnsafeNativeMethods.DHTMLImgEvents2,
                                           UnsafeNativeMethods.DHTMLInputFileElementEvents2,
                                           UnsafeNativeMethods.DHTMLInputImageEvents2,
                                           UnsafeNativeMethods.DHTMLInputTextElementEvents2,
                                           UnsafeNativeMethods.DHTMLLabelEvents2,
                                           UnsafeNativeMethods.DHTMLLinkElementEvents2,
                                           UnsafeNativeMethods.DHTMLMapEvents2,
                                           UnsafeNativeMethods.DHTMLMarqueeElementEvents2,
                                           UnsafeNativeMethods.DHTMLOptionButtonElementEvents2,
                                           UnsafeNativeMethods.DHTMLSelectElementEvents2,
                                           UnsafeNativeMethods.DHTMLStyleElementEvents2,
                                           UnsafeNativeMethods.DHTMLTableEvents2,
                                           UnsafeNativeMethods.DHTMLTextContainerEvents2,
                                           UnsafeNativeMethods.DHTMLScriptEvents2
        {

            private HtmlElement parent;

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


            public bool onclick(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventClick, e);
                return e.ReturnValue;
            }

            public bool ondblclick(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventDoubleClick, e);
                return e.ReturnValue;
            }

            public bool onkeypress(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventKeyPress, e);
                return e.ReturnValue;
            }

            public void onkeydown(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventKeyDown, e);
            }

            public void onkeyup(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventKeyUp, e);
            }

            public void onmouseover(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventMouseOver, e);
            }

            public void onmousemove(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventMouseMove, e);
            }

            public void onmousedown(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventMouseDown, e);
            }

            public void onmouseup(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventMouseUp, e);
            }

            public void onmouseenter(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventMouseEnter, e);
            }

            public void onmouseleave(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventMouseLeave, e);
            }

            public bool onerrorupdate(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onfocus(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventGotFocus, e);
            }

            public bool ondrag(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventDrag, e);
                return e.ReturnValue;
            }

            public void ondragend(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventDragEnd, e);
            }

            public void ondragleave(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventDragLeave, e);
            }

            public bool ondragover(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventDragOver, e);
                return e.ReturnValue;
            }

            public void onfocusin(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventFocusing, e);
            }

            public void onfocusout(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventLosingFocus, e);
            }

            public void onblur(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlElement.EventLostFocus, e);
            }

            public void onresizeend(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public bool onresizestart(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onhelp(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onmouseout(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public bool onselectstart(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onfilterchange(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public bool ondragstart(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onbeforeupdate(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onafterupdate(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public bool onrowexit(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onrowenter(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void ondatasetchanged(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void ondataavailable(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void ondatasetcomplete(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onlosecapture(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onpropertychange(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onscroll(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onresize(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public bool ondragenter(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool ondrop(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onbeforecut(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool oncut(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onbeforecopy(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool oncopy(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onbeforepaste(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onpaste(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool oncontextmenu(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onrowsdelete(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onrowsinserted(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void oncellchange(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onreadystatechange(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onlayoutcomplete(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onpage(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onactivate(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void ondeactivate(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public bool onbeforedeactivate(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onbeforeactivate(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onmove(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public bool oncontrolselect(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onmovestart(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onmoveend(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public bool onmousewheel(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onchange(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onselect(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onload(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onerror(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onabort(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public bool onsubmit(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onreset(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onchange_void(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onbounce(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onfinish(UnsafeNativeMethods.IHTMLEventObj evtObj) { }

            public void onstart(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
        }


        ///<summary>
        /// HtmlElementShim - this is the glue between the DOM eventing mechanisms
        ///                    and our CLR callbacks.  
        ///             
        ///     HTMLElementEvents2: we create an IConnectionPoint (via ConnectionPointCookie) between us and MSHTML and it calls back
        ///                        on our an instance of HTMLElementEvents2.  The HTMLElementEvents2 class then fires the event.
        ///
        ///</summary>  
        internal class HtmlElementShim : HtmlShim
        {

            private static Type[] dispInterfaceTypes = {typeof(UnsafeNativeMethods.DHTMLElementEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLAnchorEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLAreaEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLButtonElementEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLControlElementEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLFormElementEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLFrameSiteEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLImgEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLInputFileElementEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLInputImageEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLInputTextElementEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLLabelEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLLinkElementEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLMapEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLMarqueeElementEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLOptionButtonElementEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLSelectElementEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLStyleElementEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLTableEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLTextContainerEvents2),
                                                    typeof(UnsafeNativeMethods.DHTMLScriptEvents2)};

            private AxHost.ConnectionPointCookie cookie;   // To hook up events from the native HtmlElement
            private HtmlElement htmlElement;
            private UnsafeNativeMethods.IHTMLWindow2 associatedWindow = null;

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

            public UnsafeNativeMethods.IHTMLElement NativeHtmlElement
            {
                get { return htmlElement.NativeHtmlElement; }
            }

            internal HtmlElement Element
            {
                get { return htmlElement; }
            }

            public override UnsafeNativeMethods.IHTMLWindow2 AssociatedWindow
            {
                get { return associatedWindow; }
            }

            /// Support IHTMLElement2.AttachEventHandler
            public override void AttachEventHandler(string eventName, System.EventHandler eventHandler)
            {

                // IE likes to call back on an IDispatch of DISPID=0 when it has an event, 
                // the HtmlToClrEventProxy helps us fake out the CLR so that we can call back on 
                // our EventHandler properly.

                HtmlToClrEventProxy proxy = AddEventProxy(eventName, eventHandler);
                bool success = ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).AttachEvent(eventName, proxy);
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

            /// Support IHTMLElement2.DetachHandler
            public override void DetachEventHandler(string eventName, System.EventHandler eventHandler)
            {
                HtmlToClrEventProxy proxy = RemoveEventProxy(eventHandler);
                if (proxy != null)
                {
                    ((UnsafeNativeMethods.IHTMLElement2)NativeHtmlElement).DetachEvent(eventName, proxy);
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
        [SuppressMessage("Microsoft.Design", "CA1046:DoNotOverrideOperatorEqualsOnReferenceTypes")]
        public static bool operator ==(HtmlElement left, HtmlElement right)
        {
            //Not equal if only one's null.
            if (object.ReferenceEquals(left, null) != object.ReferenceEquals(right, null))
            {
                return false;
            }

            //Equal if both are null.
            if (object.ReferenceEquals(left, null))
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

        public override int GetHashCode()
        {
            return htmlElement == null ? 0 : htmlElement.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            //If obj isn't an HtmlElement, we want Equals to return false.  this will
            //never be null, so now it will return false as expected (not throw).
            return this == (obj as HtmlElement);
        }
        #endregion



    }
}

