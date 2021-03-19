// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop.Mshtml;

namespace System.Windows.Forms
{
    public sealed partial class HtmlWindow
    {
        //
        // Private classes:
        //
        [ClassInterface(ClassInterfaceType.None)]
        private class HTMLWindowEvents2 : StandardOleMarshalObject, /*Enforce calling back on the same thread*/
            DHTMLWindowEvents2
        {
            private readonly HtmlWindow parent;

            public HTMLWindowEvents2(HtmlWindow htmlWindow)
            {
                parent = htmlWindow;
            }

            private void FireEvent(object key, EventArgs e)
            {
                if (parent != null)
                {
                    parent.WindowShim.FireEvent(key, e);
                }
            }

            public void onfocus(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventGotFocus, e);
            }

            public void onblur(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventLostFocus, e);
            }

            public bool onerror(string description, string urlString, int line)
            {
                HtmlElementErrorEventArgs e = new HtmlElementErrorEventArgs(description, urlString, line);
                FireEvent(HtmlWindow.EventError, e);
                return e.Handled;
            }

            public void onload(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventLoad, e);
            }

            public void onunload(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventUnload, e);
                if (parent != null)
                {
                    parent.WindowShim.OnWindowUnload();
                }
            }

            public void onscroll(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventScroll, e);
            }

            public void onresize(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventResize, e);
            }

            public bool onhelp(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onbeforeunload(IHTMLEventObj evtObj) { }

            public void onbeforeprint(IHTMLEventObj evtObj) { }

            public void onafterprint(IHTMLEventObj evtObj) { }
        }
    }
}
