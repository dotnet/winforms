// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using static Interop.Mshtml;

namespace System.Windows.Forms;

public sealed partial class HtmlWindow
{
    //
    // Private classes:
    //
    [ClassInterface(ClassInterfaceType.None)]
    private class HTMLWindowEvents2 : StandardOleMarshalObject, /*Enforce calling back on the same thread*/
        DHTMLWindowEvents2
    {
        private readonly HtmlWindow _parent;

        public HTMLWindowEvents2(HtmlWindow htmlWindow)
        {
            _parent = htmlWindow;
        }

        public void onafterprint(IHTMLEventObj evtObj) { }

        public void onbeforeprint(IHTMLEventObj evtObj) { }

        public void onbeforeunload(IHTMLEventObj evtObj) { }

        public void onblur(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventLostFocus, e);
        }

        public bool onerror(string description, string urlString, int line)
        {
            HtmlElementErrorEventArgs e = new(description, urlString, line);
            FireEvent(s_eventError, e);
            return e.Handled;
        }

        public void onfocus(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventGotFocus, e);
        }

        public bool onhelp(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onload(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventLoad, e);
        }

        public void onresize(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventResize, e);
        }

        public void onscroll(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventScroll, e);
        }

        public void onunload(IHTMLEventObj evtObj)
        {
            HtmlWindowShim? shim = _parent.ShimManager.GetWindowShim(_parent);
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            try
            {
                shim?.FireEvent(s_eventUnload, e);
            }
            finally
            {
                // User handlers may dispose the browser or throw. Retain the original shim rather
                // than looking up the lazy WindowShim property and recreating an owner during teardown.
                shim?.OnWindowUnload();
            }
        }

        private void FireEvent(object key, EventArgs e)
        {
            _parent.ShimManager.GetWindowShim(_parent)?.FireEvent(key, e);
        }
    }
}
