// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms
{

    public sealed class HtmlElementEventArgs : EventArgs
    {
        private HtmlShimManager _shimManager;
        
        internal HtmlElementEventArgs(HtmlShimManager shimManager, UnsafeNativeMethods.IHTMLEventObj eventObj)
        {
            NativeHTMLEventObj = eventObj;
            Debug.Assert(NativeHTMLEventObj != null, "The event object should implement IHTMLEventObj");
            
            _shimManager = shimManager;
        }
        
        private UnsafeNativeMethods.IHTMLEventObj NativeHTMLEventObj { get; }

        public MouseButtons MouseButtonsPressed
        {
            get
            {
                MouseButtons buttons = MouseButtons.None;
                int nButtons = NativeHTMLEventObj.GetButton();
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

        public Point ClientMousePosition
        {
            get => new Point(NativeHTMLEventObj.GetClientX(), NativeHTMLEventObj.GetClientY());
        }

        public Point OffsetMousePosition
        {
            get => new Point(NativeHTMLEventObj.GetOffsetX(), NativeHTMLEventObj.GetOffsetY());
        }

        public Point MousePosition
        {
            get => new Point(NativeHTMLEventObj.GetX(), NativeHTMLEventObj.GetY());
        }

        public bool BubbleEvent {
            get => !NativeHTMLEventObj.GetCancelBubble();
            set => NativeHTMLEventObj.SetCancelBubble(!value);
        }

        public int KeyPressedCode => NativeHTMLEventObj.GetKeyCode();

        /// <devdoc>
        /// Indicates whether the Alt key was pressed, if this information is 
        /// provided to the IHtmlEventObj
        /// </devdoc>
        public bool AltKeyPressed => NativeHTMLEventObj.GetAltKey();

        /// <devdoc>
        /// Indicates whether the Ctrl key was pressed, if this information is 
        /// provided to the IHtmlEventObj
        /// </devdoc>
        public bool CtrlKeyPressed => NativeHTMLEventObj.GetCtrlKey();

        /// <devdoc>
        /// Indicates whether the Shift key was pressed, if this information is 
        /// provided to the IHtmlEventObj
        /// </devdoc>
        public bool ShiftKeyPressed => NativeHTMLEventObj.GetShiftKey();

        public string EventType => NativeHTMLEventObj.GetEventType();

        public bool ReturnValue
        {
            get
            {
                object obj = NativeHTMLEventObj.GetReturnValue();
                return obj == null ? true : (bool)obj;
            }
            set => NativeHTMLEventObj.SetReturnValue(value);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public HtmlElement FromElement
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement htmlElement = NativeHTMLEventObj.GetFromElement();
                return htmlElement == null ? null : new HtmlElement(_shimManager, htmlElement);
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public HtmlElement ToElement
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement htmlElement = NativeHTMLEventObj.GetToElement();
                return htmlElement == null ? null : new HtmlElement(_shimManager, htmlElement);
            }
        }
    }
}
