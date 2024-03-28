// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using static Interop.Mshtml;

namespace System.Windows.Forms;

public sealed partial class HtmlElement
{
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
        private readonly HtmlElement _parent;

        public HTMLElementEvents2(HtmlElement htmlElement)
        {
            _parent = htmlElement;
        }

        public void onabort(IHTMLEventObj evtObj) { }

        public void onactivate(IHTMLEventObj evtObj) { }

        public void onafterupdate(IHTMLEventObj evtObj) { }

        public bool onbeforeactivate(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public bool onbeforecopy(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public bool onbeforecut(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public bool onbeforedeactivate(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public bool onbeforepaste(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public bool onbeforeupdate(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onblur(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventLostFocus, e);
        }

        public void onbounce(IHTMLEventObj evtObj) { }

        public void oncellchange(IHTMLEventObj evtObj) { }

        public bool onchange(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onchange_void(IHTMLEventObj evtObj) { }

        public bool onclick(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventClick, e);
            return e.ReturnValue;
        }

        public bool oncontextmenu(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public bool oncontrolselect(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public bool oncopy(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public bool oncut(IHTMLEventObj evtObj)
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
            FireEvent(s_eventDoubleClick, e);
            return e.ReturnValue;
        }

        public void ondeactivate(IHTMLEventObj evtObj) { }

        public bool ondrag(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventDrag, e);
            return e.ReturnValue;
        }

        public void ondragend(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventDragEnd, e);
        }

        public bool ondragenter(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void ondragleave(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventDragLeave, e);
        }

        public bool ondragover(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventDragOver, e);
            return e.ReturnValue;
        }

        public bool ondragstart(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public bool ondrop(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onerror(IHTMLEventObj evtObj) { }

        public bool onerrorupdate(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onfilterchange(IHTMLEventObj evtObj) { }

        public void onfinish(IHTMLEventObj evtObj) { }

        public void onfocus(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventGotFocus, e);
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

        public void onkeydown(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventKeyDown, e);
        }

        public bool onkeypress(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventKeyPress, e);
            return e.ReturnValue;
        }

        public void onkeyup(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventKeyUp, e);
        }

        public void onlayoutcomplete(IHTMLEventObj evtObj) { }

        public void onload(IHTMLEventObj evtObj) { }

        public void onlosecapture(IHTMLEventObj evtObj) { }

        public void onmousedown(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventMouseDown, e);
        }

        public void onmouseenter(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventMouseEnter, e);
        }

        public void onmouseleave(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventMouseLeave, e);
        }

        public void onmousemove(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            FireEvent(s_eventMouseMove, e);
        }

        public void onmouseout(IHTMLEventObj evtObj) { }

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

        public void onmove(IHTMLEventObj evtObj) { }

        public void onmoveend(IHTMLEventObj evtObj) { }

        public bool onmovestart(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onpage(IHTMLEventObj evtObj) { }

        public bool onpaste(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onpropertychange(IHTMLEventObj evtObj) { }

        public void onreadystatechange(IHTMLEventObj evtObj) { }

        public bool onreset(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onresize(IHTMLEventObj evtObj) { }

        public void onresizeend(IHTMLEventObj evtObj) { }

        public bool onresizestart(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onrowenter(IHTMLEventObj evtObj) { }

        public bool onrowexit(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onrowsdelete(IHTMLEventObj evtObj) { }

        public void onrowsinserted(IHTMLEventObj evtObj) { }

        public void onscroll(IHTMLEventObj evtObj) { }

        public void onselect(IHTMLEventObj evtObj) { }

        public bool onselectstart(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        public void onstart(IHTMLEventObj evtObj) { }

        public bool onsubmit(IHTMLEventObj evtObj)
        {
            HtmlElementEventArgs e = new(_parent.ShimManager, evtObj);
            return e.ReturnValue;
        }

        private void FireEvent(object key, EventArgs e)
        {
            _parent?.ElementShim.FireEvent(key, e);
        }
    }
}
