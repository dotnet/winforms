// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using static Interop.Mshtml;

namespace System.Windows.Forms;

public sealed partial class HtmlDocument
{
    //
    // Private classes:
    //
    [ClassInterface(ClassInterfaceType.None)]
    private class HTMLDocumentEvents2 : StandardOleMarshalObject, /*Enforce calling back on the same thread*/
                                        DHTMLDocumentEvents2
    {
        private readonly HtmlDocument _parent;

        public HTMLDocumentEvents2(HtmlDocument htmlDocument)
        {
            _parent = htmlDocument;
        }

        public void onactivate(IHTMLEventObj evtObj) { }

        public void onafterupdate(IHTMLEventObj evtObj) { }

        public bool onbeforeactivate(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public bool onbeforedeactivate(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onbeforeeditfocus(IHTMLEventObj evtObj) { }

        public bool onbeforeupdate(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void oncellchange(IHTMLEventObj evtObj) { }

        public bool onclick(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventClick, e);

            return e.ReturnValue;
        }

        public bool oncontextmenu(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventContextMenuShowing, e);
            return e.ReturnValue;
        }

        public bool oncontrolselect(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void ondataavailable(IHTMLEventObj evtObj) { }

        public void ondatasetchanged(IHTMLEventObj evtObj) { }

        public void ondatasetcomplete(IHTMLEventObj evtObj) { }

        public bool ondblclick(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void ondeactivate(IHTMLEventObj evtObj) { }

        public bool ondragstart(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public bool onerrorupdate(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onfocusin(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventFocusing, e);
        }

        public void onfocusout(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventLosingFocus, e);
        }

        public bool onhelp(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onkeydown(IHTMLEventObj evtObj) { }

        public bool onkeypress(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onkeyup(IHTMLEventObj evtObj) { }

        public void onmousedown(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventMouseDown, e);
        }

        public void onmousemove(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventMouseMove, e);
        }

        public void onmouseout(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventMouseLeave, e);
        }

        public void onmouseover(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventMouseOver, e);
        }

        public void onmouseup(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventMouseUp, e);
        }

        public bool onmousewheel(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onpropertychange(IHTMLEventObj evtObj) { }

        public void onreadystatechange(IHTMLEventObj evtObj) { }

        public void onrowenter(IHTMLEventObj evtObj) { }

        public bool onrowexit(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onrowsdelete(IHTMLEventObj evtObj) { }

        public void onrowsinserted(IHTMLEventObj evtObj) { }

        public void onselectionchange(IHTMLEventObj evtObj) { }

        public bool onselectstart(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public bool onstop(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventStop, e);
            return e.ReturnValue;
        }

        private void FireEvent(object key, EventArgs e)
        {
            _parent?.DocumentShim!.FireEvent(key, e);
        }
    }
}
