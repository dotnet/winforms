// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;

namespace System.Windows.Forms {

    /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs"]/*' />
    public sealed class HtmlElementEventArgs : EventArgs {
        private UnsafeNativeMethods.IHTMLEventObj htmlEventObj;
        private HtmlShimManager shimManager;
        
        internal HtmlElementEventArgs(HtmlShimManager shimManager, UnsafeNativeMethods.IHTMLEventObj eventObj) {
            this.htmlEventObj = eventObj;
            Debug.Assert(this.NativeHTMLEventObj != null, "The event object should implement IHTMLEventObj");
            
            this.shimManager = shimManager;
        }
        
        private UnsafeNativeMethods.IHTMLEventObj NativeHTMLEventObj {
            get {
                return this.htmlEventObj;
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.MouseButtonsPressed"]/*' />
        public MouseButtons MouseButtonsPressed {
            get {
                MouseButtons buttons = MouseButtons.None;
                int nButtons = this.NativeHTMLEventObj.GetButton();
                if ((nButtons & 1) != 0) {
                    buttons |= MouseButtons.Left;
                }
                if ((nButtons & 2) != 0) {
                    buttons |= MouseButtons.Right;
                }
                if ((nButtons & 4) != 0) {
                    buttons |= MouseButtons.Middle;
                }
                return buttons;
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.ClientMousePosition"]/*' />
        public Point ClientMousePosition {
            get {
                return new Point(this.NativeHTMLEventObj.GetClientX(), this.NativeHTMLEventObj.GetClientY());
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.OffsetMousePosition"]/*' />
        public Point OffsetMousePosition {
            get {
                return new Point(this.NativeHTMLEventObj.GetOffsetX(), this.NativeHTMLEventObj.GetOffsetY());
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.MousePosition"]/*' />
        public Point MousePosition {
            get {
                return new Point(this.NativeHTMLEventObj.GetX(), this.NativeHTMLEventObj.GetY());
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.BubbleEvent"]/*' />
        public bool BubbleEvent {
            get {
                return !this.NativeHTMLEventObj.GetCancelBubble();
            }
            set {
                this.NativeHTMLEventObj.SetCancelBubble(!value);
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.KeyPressedCode"]/*' />
        public int KeyPressedCode {
            get {
                return this.NativeHTMLEventObj.GetKeyCode();
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.AltKeyPressed"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the Alt key was pressed, if this information is 
        ///     provided to the IHtmlEventObj </para>
        /// </devdoc>
        public bool AltKeyPressed
        {
            get
            {
                return this.NativeHTMLEventObj.GetAltKey();
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.CtrlKeyPressed"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the Ctrl key was pressed, if this information is 
        ///     provided to the IHtmlEventObj </para>
        /// </devdoc>
        public bool CtrlKeyPressed
        {
            get
            {
                return this.NativeHTMLEventObj.GetCtrlKey();
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.ShiftKeyPressed"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the Shift key was pressed, if this information is 
        ///     provided to the IHtmlEventObj </para>
        /// </devdoc>
        public bool ShiftKeyPressed
        {
            get
            {
                return this.NativeHTMLEventObj.GetShiftKey();
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.EventType"]/*' />
        public string EventType {
            get {
                return this.NativeHTMLEventObj.GetEventType();
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.ReturnValue"]/*' />
        public bool ReturnValue {
            get {
                object obj = this.NativeHTMLEventObj.GetReturnValue();
                return obj == null ? true : (bool)obj;
            }
            set {
                object objValue = value;
                this.NativeHTMLEventObj.SetReturnValue(objValue);
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.FromElement"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public HtmlElement FromElement {
            get {
                UnsafeNativeMethods.IHTMLElement htmlElement = this.NativeHTMLEventObj.GetFromElement();
                return htmlElement == null ? null : new HtmlElement(shimManager, htmlElement);
            }
        }

        /// <include file='doc\HtmlElementEventArgs.uex' path='docs/doc[@for="HtmlElementEventArgs.ToElement"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public HtmlElement ToElement {
            get {
                UnsafeNativeMethods.IHTMLElement htmlElement = this.NativeHTMLEventObj.GetToElement();
                return htmlElement == null ? null : new HtmlElement(shimManager, htmlElement);
            }
        }
    }
}

