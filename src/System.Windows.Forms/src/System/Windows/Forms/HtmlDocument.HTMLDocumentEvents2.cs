// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop.Mshtml;

namespace System.Windows.Forms
{
    public sealed partial class HtmlDocument
    {
        //
        // Private classes:
        //
        [ClassInterface(ClassInterfaceType.None)]
        private class HTMLDocumentEvents2 : StandardOleMarshalObject, /*Enforce calling back on the same thread*/
                                            DHTMLDocumentEvents2
        {
            private readonly HtmlDocument parent;

            public HTMLDocumentEvents2(HtmlDocument htmlDocument)
            {
                parent = htmlDocument;
            }

            private void FireEvent(object key, EventArgs e)
            {
                if (parent != null)
                {
                    parent.DocumentShim.FireEvent(key, e);
                }
            }

            public bool onclick(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventClick, e);

                return e.ReturnValue;
            }

            public bool oncontextmenu(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventContextMenuShowing, e);
                return e.ReturnValue;
            }

            public void onfocusin(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventFocusing, e);
            }

            public void onfocusout(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventLosingFocus, e);
            }

            public void onmousemove(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseMove, e);
            }

            public void onmousedown(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseDown, e);
            }

            public void onmouseout(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseLeave, e);
            }

            public void onmouseover(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseOver, e);
            }

            public void onmouseup(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseUp, e);
            }

            public bool onstop(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventStop, e);
                return e.ReturnValue;
            }

            public bool onhelp(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool ondblclick(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onkeydown(IHTMLEventObj evtObj) { }

            public void onkeyup(IHTMLEventObj evtObj) { }

            public bool onkeypress(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onreadystatechange(IHTMLEventObj evtObj) { }

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

            public bool ondragstart(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onselectstart(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onerrorupdate(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onrowsdelete(IHTMLEventObj evtObj) { }

            public void onrowsinserted(IHTMLEventObj evtObj) { }

            public void oncellchange(IHTMLEventObj evtObj) { }

            public void onpropertychange(IHTMLEventObj evtObj) { }

            public void ondatasetchanged(IHTMLEventObj evtObj) { }

            public void ondataavailable(IHTMLEventObj evtObj) { }

            public void ondatasetcomplete(IHTMLEventObj evtObj) { }

            public void onbeforeeditfocus(IHTMLEventObj evtObj) { }

            public void onselectionchange(IHTMLEventObj evtObj) { }

            public bool oncontrolselect(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onmousewheel(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onactivate(IHTMLEventObj evtObj) { }

            public void ondeactivate(IHTMLEventObj evtObj) { }

            public bool onbeforeactivate(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onbeforedeactivate(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }
        }
    }
}
