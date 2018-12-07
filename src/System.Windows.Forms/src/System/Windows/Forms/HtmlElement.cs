// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.Windows.Forms
{
    /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement"]/*' />
    [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
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

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        internal HtmlElement(HtmlShimManager shimManager, UnsafeNativeMethods.IHTMLElement element)
        {
            this.htmlElement = element;
            Debug.Assert(this.NativeHtmlElement != null, "The element object should implement IHTMLElement");

            this.shimManager = shimManager;

        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.All"]/*' />
        public HtmlElementCollection All
        {
            get
            {
                UnsafeNativeMethods.IHTMLElementCollection iHTMLElementCollection = this.NativeHtmlElement.GetAll() as UnsafeNativeMethods.IHTMLElementCollection;
                return iHTMLElementCollection != null ? new HtmlElementCollection(shimManager, iHTMLElementCollection) : new HtmlElementCollection(shimManager);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Children"]/*' />
        public HtmlElementCollection Children
        {
            get
            {
                UnsafeNativeMethods.IHTMLElementCollection iHTMLElementCollection = this.NativeHtmlElement.GetChildren() as UnsafeNativeMethods.IHTMLElementCollection;
                return iHTMLElementCollection != null ? new HtmlElementCollection(shimManager, iHTMLElementCollection) : new HtmlElementCollection(shimManager);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.CanHaveChildren"]/*' />
        public bool CanHaveChildren
        {
            get
            {
                return ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).CanHaveChildren();
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.ClientRectangle"]/*' />
        public Rectangle ClientRectangle
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement2 htmlElement2 = (UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement;
                return new Rectangle(htmlElement2.ClientLeft(), htmlElement2.ClientTop(),
                    htmlElement2.ClientWidth(), htmlElement2.ClientHeight());
            }
        }


        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Document"]/*' />
        public HtmlDocument Document
        {
            get
            {
                UnsafeNativeMethods.IHTMLDocument iHTMLDocument = this.NativeHtmlElement.GetDocument() as UnsafeNativeMethods.IHTMLDocument;
                return iHTMLDocument != null ? new HtmlDocument(shimManager, iHTMLDocument) : null;
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Enabled"]/*' />
        public bool Enabled
        {
            get
            {
                return !(((UnsafeNativeMethods.IHTMLElement3)this.NativeHtmlElement).GetDisabled());
            }
            set
            {
                ((UnsafeNativeMethods.IHTMLElement3)this.NativeHtmlElement).SetDisabled(!value);
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

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.FirstChild"]/*' />
        public HtmlElement FirstChild
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement iHtmlElement = null;
                UnsafeNativeMethods.IHTMLDOMNode iHtmlDomNode = this.NativeHtmlElement as UnsafeNativeMethods.IHTMLDOMNode;

                if( iHtmlDomNode != null )
                {
                    iHtmlElement = iHtmlDomNode.FirstChild() as UnsafeNativeMethods.IHTMLElement;               
                }
                return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Id"]/*' />
        public string Id
        {
            [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
            get
            {
                return this.NativeHtmlElement.GetId();
            }
            set
            {
                this.NativeHtmlElement.SetId(value);
            }
        }


        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.InnerHtml"]/*' />
        public string InnerHtml
        {
            get
            {
                return this.NativeHtmlElement.GetInnerHTML();
            }
            set
            {
                try
                {
                    this.NativeHtmlElement.SetInnerHTML(value);
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

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.InnerText"]/*' />
        public string InnerText
        {
            get
            {
                return this.NativeHtmlElement.GetInnerText();
            }
            set
            {
                try
                {
                    this.NativeHtmlElement.SetInnerText(value);
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

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Name"]/*' />
        public string Name
        {
            get
            {
                return this.GetAttribute("Name");
            }
            set
            {
                this.SetAttribute("Name", value);
            }
        }


        private UnsafeNativeMethods.IHTMLElement NativeHtmlElement
        {
            get
            {
                return this.htmlElement;
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.NextSibling"]/*' />
        public HtmlElement NextSibling
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement iHtmlElement = null;
                UnsafeNativeMethods.IHTMLDOMNode iHtmlDomNode = this.NativeHtmlElement as UnsafeNativeMethods.IHTMLDOMNode;

                if( iHtmlDomNode != null )
                {
                    iHtmlElement = iHtmlDomNode.NextSibling() as UnsafeNativeMethods.IHTMLElement;
                }
                return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
            }
        }


        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.OffsetRectangle"]/*' />
        public Rectangle OffsetRectangle
        {
            get
            {
                return new Rectangle(this.NativeHtmlElement.GetOffsetLeft(), this.NativeHtmlElement.GetOffsetTop(),
                    this.NativeHtmlElement.GetOffsetWidth(), this.NativeHtmlElement.GetOffsetHeight());
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.OffsetParent"]/*' />
        public HtmlElement OffsetParent
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement iHtmlElement = this.NativeHtmlElement.GetOffsetParent();
                return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.OuterHtml"]/*' />
        public string OuterHtml
        {
            get
            {
                return this.NativeHtmlElement.GetOuterHTML();
            }
            set
            {
                try
                {
                    this.NativeHtmlElement.SetOuterHTML(value);
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

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.OuterText"]/*' />
        public string OuterText
        {
            get
            {
                return this.NativeHtmlElement.GetOuterText();
            }
            set
            {
                try
                {
                    this.NativeHtmlElement.SetOuterText(value);
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

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Parent"]/*' />
        public HtmlElement Parent
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement iHtmlElement = this.NativeHtmlElement.GetParentElement();
                return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
            }
        }



        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.ScrollRectangle"]/*' />
        public Rectangle ScrollRectangle
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement2 htmlElement2 = (UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement;
                return new Rectangle(htmlElement2.GetScrollLeft(), htmlElement2.GetScrollTop(),
                    htmlElement2.GetScrollWidth(), htmlElement2.GetScrollHeight());
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.ScrollLeft"]/*' />
        public int ScrollLeft
        {
            get
            {
                return ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).GetScrollLeft();
            }
            set
            {
                ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).SetScrollLeft(value);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.ScrollTop"]/*' />
        public int ScrollTop
        {
            get
            {
                return ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).GetScrollTop();
            }
            set
            {
                ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).SetScrollTop(value);
            }
        }

        private HtmlShimManager ShimManager
        {
            get
            {
                return shimManager;
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Style"]/*' />
        public string Style
        {
            get
            {
                return this.NativeHtmlElement.GetStyle().GetCssText();
            }
            set
            {
                this.NativeHtmlElement.GetStyle().SetCssText(value);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.TagName"]/*' />
        public string TagName
        {
            get
            {
                return this.NativeHtmlElement.GetTagName();
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.TabIndex"]/*' />
        public short TabIndex
        {
            get
            {
                return ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).GetTabIndex();
            }
            set
            {
                ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).SetTabIndex(value);
            }
        }


        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.DomElement"]/*' />
        public object DomElement
        {
            get
            {
                return this.NativeHtmlElement;
            }
        }


        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.AppendChild"]/*' />
        public HtmlElement AppendChild(HtmlElement newElement)
        {
            return this.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, newElement);
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.AttachEventHandler"]/*' />
        public void AttachEventHandler(string eventName, EventHandler eventHandler)
        {
            ElementShim.AttachEventHandler(eventName, eventHandler);
        }


        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.DetachEventHandler"]/*' />
        public void DetachEventHandler(string eventName, EventHandler eventHandler)
        {
            ElementShim.DetachEventHandler(eventName, eventHandler);
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Focus"]/*' />
        public void Focus()
        {
            try
            {
                ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).Focus();
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

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.GetAttribute"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public string GetAttribute(string attributeName)
        {
            object oAttributeValue = this.NativeHtmlElement.GetAttribute(attributeName, 0);
            return oAttributeValue == null ? "" : oAttributeValue.ToString();
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.GetElementsByTagName"]/*' />
        public HtmlElementCollection GetElementsByTagName(string tagName)
        {
            UnsafeNativeMethods.IHTMLElementCollection iHTMLElementCollection = ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).GetElementsByTagName(tagName);
            return iHTMLElementCollection != null ? new HtmlElementCollection(shimManager, iHTMLElementCollection) : new HtmlElementCollection(shimManager);
        }
        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.InsertAdjacentElement"]/*' />
        public HtmlElement InsertAdjacentElement(HtmlElementInsertionOrientation orient, HtmlElement newElement)
        {
            UnsafeNativeMethods.IHTMLElement iHtmlElement = ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).InsertAdjacentElement(orient.ToString(),
                (UnsafeNativeMethods.IHTMLElement)newElement.DomElement);
            return iHtmlElement != null ? new HtmlElement(shimManager, iHtmlElement) : null;
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.InvokeMember"]/*' />
        public object InvokeMember(string methodName)
        {
            return InvokeMember(methodName, null);
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.InvokeMember"]/*' />
        public object InvokeMember(string methodName, params object[] parameter)
        {
            object retVal = null;
            NativeMethods.tagDISPPARAMS dp = new NativeMethods.tagDISPPARAMS();
            dp.rgvarg = IntPtr.Zero;
            try
            {
                UnsafeNativeMethods.IDispatch scriptObject = this.NativeHtmlElement as UnsafeNativeMethods.IDispatch;
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


        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.RemoveFocus"]/*' />
        public void RemoveFocus()
        {
            ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).Blur();
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.RaiseEvent"]/*' />
        // PM review done
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void RaiseEvent(string eventName)
        {
            ((UnsafeNativeMethods.IHTMLElement3)this.NativeHtmlElement).FireEvent(eventName, IntPtr.Zero);
        }


        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.ScrollIntoView"]/*' />
        public void ScrollIntoView(bool alignWithTop)
        {
            this.NativeHtmlElement.ScrollIntoView((object)alignWithTop);
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.SetAttribute"]/*' />
        public void SetAttribute(string attributeName, string value)
        {
            try
            {
                this.NativeHtmlElement.SetAttribute(attributeName, (object)value, 0);
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
        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Click"]/*' />
        public event HtmlElementEventHandler Click
        {
            add
            {
                ElementShim.AddHandler(EventClick, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventClick, value);
            }

        }


        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.DoubleClick"]/*' />
        public event HtmlElementEventHandler DoubleClick
        {
            add
            {
                ElementShim.AddHandler(EventDoubleClick, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventDoubleClick, value);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Drag"]/*' />
        public event HtmlElementEventHandler Drag
        {
            add
            {
                ElementShim.AddHandler(EventDrag, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventDrag, value);
            }
        }


        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.DragEnd"]/*' />
        public event HtmlElementEventHandler DragEnd
        {
            add
            {
                ElementShim.AddHandler(EventDragEnd, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventDragEnd, value);
            }
        }


        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Drag"]/*' />
        public event HtmlElementEventHandler DragLeave
        {
            add
            {
                ElementShim.AddHandler(EventDragLeave, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventDragLeave, value);
            }

        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.DragOver"]/*' />
        public event HtmlElementEventHandler DragOver
        {
            add
            {
                ElementShim.AddHandler(EventDragOver, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventDragOver, value);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Focusing"]/*' />
        public event HtmlElementEventHandler Focusing
        {
            add
            {
                ElementShim.AddHandler(EventFocusing, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventFocusing, value);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Focus"]/*' />
        public event HtmlElementEventHandler GotFocus
        {
            add
            {
                ElementShim.AddHandler(EventGotFocus, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventGotFocus, value);
            }

        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.LosingFocus"]/*' />
        public event HtmlElementEventHandler LosingFocus
        {
            add
            {
                ElementShim.AddHandler(EventLosingFocus, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventLosingFocus, value);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.LostFocus"]/*' />
        public event HtmlElementEventHandler LostFocus
        {
            add
            {
                ElementShim.AddHandler(EventLostFocus, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventLostFocus, value);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.KeyDown"]/*' />
        public event HtmlElementEventHandler KeyDown
        {
            add
            {
                ElementShim.AddHandler(EventKeyDown, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventKeyDown, value);
            }
        }
        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.KeyPress"]/*' />
        public event HtmlElementEventHandler KeyPress
        {
            add
            {
                ElementShim.AddHandler(EventKeyPress, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventKeyPress, value);
            }

        }
        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.KeyUp"]/*' />
        public event HtmlElementEventHandler KeyUp
        {
            add
            {
                ElementShim.AddHandler(EventKeyUp, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventKeyUp, value);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.MouseMove"]/*' />
        public event HtmlElementEventHandler MouseMove
        {
            add
            {
                ElementShim.AddHandler(EventMouseMove, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventMouseMove, value);
            }
        }
        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.MouseDown"]/*' />
        public event HtmlElementEventHandler MouseDown
        {
            add
            {
                ElementShim.AddHandler(EventMouseDown, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventMouseDown, value);
            }
        }
        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.MouseOver"]/*' />
        public event HtmlElementEventHandler MouseOver
        {
            add
            {
                ElementShim.AddHandler(EventMouseOver, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventMouseOver, value);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.MouseUp"]/*' />
        public event HtmlElementEventHandler MouseUp
        {
            add
            {
                ElementShim.AddHandler(EventMouseUp, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventMouseUp, value);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.MouseEnter"]/*' />
        /// <devdoc>
        ///    <para>Fires when the mouse enters the element</para>
        /// </devdoc>
        public event HtmlElementEventHandler MouseEnter
        {
            add
            {
                ElementShim.AddHandler(EventMouseEnter, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventMouseEnter, value);
            }
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.MouseLeave"]/*' />
        /// <devdoc>
        ///    <para>Fires when the mouse leaves the element</para>
        /// </devdoc>
        public event HtmlElementEventHandler MouseLeave
        {
            add
            {
                ElementShim.AddHandler(EventMouseLeave, value);
            }
            remove
            {
                ElementShim.RemoveHandler(EventMouseLeave, value);
            }
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
                this.parent = htmlElement;
            }
            private void FireEvent(object key, EventArgs e)
            {
                if (this.parent != null)
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


        ///<devdoc>
        /// HtmlElementShim - this is the glue between the DOM eventing mechanisms
        ///                    and our CLR callbacks.  
        ///             
        ///     HTMLElementEvents2: we create an IConnectionPoint (via ConnectionPointCookie) between us and MSHTML and it calls back
        ///                        on our an instance of HTMLElementEvents2.  The HTMLElementEvents2 class then fires the event.
        ///
        ///</devdoc>  
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
                this.htmlElement = element;

                // snap our associated window so we know when to disconnect.
                if (this.htmlElement != null)
                {
                    HtmlDocument doc = this.htmlElement.Document;
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
                bool success = ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).AttachEvent(eventName, proxy);
                Debug.Assert(success, "failed to add event");
            }

            public override void ConnectToEvents()
            {
                if (cookie == null || !cookie.Connected)
                {
                    for (int i = 0; i < dispInterfaceTypes.Length && this.cookie == null; i++)
                    {
                        this.cookie = new AxHost.ConnectionPointCookie(this.NativeHtmlElement,
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
                    ((UnsafeNativeMethods.IHTMLElement2)this.NativeHtmlElement).DetachEvent(eventName, proxy);
                }
            }


            public override void DisconnectFromEvents()
            {
                if (this.cookie != null)
                {
                    this.cookie.Disconnect();
                    this.cookie = null;
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
        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.operatorEQ"]/*' />
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

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.operatorNE"]/*' />
        public static bool operator !=(HtmlElement left, HtmlElement right)
        {
            return !(left == right);
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.GetHashCode"]/*' />
        public override int GetHashCode()
        {
            return htmlElement == null ? 0 : htmlElement.GetHashCode();
        }

        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElement.Equals"]/*' />
        public override bool Equals(object obj)
        {
            //If obj isn't an HtmlElement, we want Equals to return false.  this will
            //never be null, so now it will return false as expected (not throw).
            return this == (obj as HtmlElement);
        }
            #endregion



    }

    /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElementInsertionOrientation"]/*' />
    public enum HtmlElementInsertionOrientation
    {
        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElementInsertionOrientation.BeforeBegin"]/*' />
        BeforeBegin,
        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElementInsertionOrientation.AfterBegin"]/*' />
        AfterBegin,
        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElementInsertionOrientation.BeforeEnd"]/*' />
        BeforeEnd,
        /// <include file='doc\HtmlElement.uex' path='docs/doc[@for="HtmlElementInsertionOrientation.AfterEnd"]/*' />
        AfterEnd
    }
}

