// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("3050f61c-98b5-11cf-bb82-00aa00bdce0b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        [TypeLibType(TypeLibTypeFlags.FHidden)]
        public interface DHTMLLabelEvents2
        {
            [DispId(-2147418102)] bool onhelp(IHTMLEventObj evtObj);
            [DispId(-600)] bool onclick(IHTMLEventObj evtObj);
            [DispId(-601)] bool ondblclick(IHTMLEventObj evtObj);
            [DispId(-603)] bool onkeypress(IHTMLEventObj evtObj);
            [DispId(-602)] void onkeydown(IHTMLEventObj evtObj);
            [DispId(-604)] void onkeyup(IHTMLEventObj evtObj);
            [DispId(-2147418103)] void onmouseout(IHTMLEventObj evtObj);
            [DispId(-2147418104)] void onmouseover(IHTMLEventObj evtObj);
            [DispId(-606)] void onmousemove(IHTMLEventObj evtObj);
            [DispId(-605)] void onmousedown(IHTMLEventObj evtObj);
            [DispId(-607)] void onmouseup(IHTMLEventObj evtObj);
            [DispId(-2147418100)] bool onselectstart(IHTMLEventObj evtObj);
            [DispId(-2147418095)] void onfilterchange(IHTMLEventObj evtObj);
            [DispId(-2147418101)] bool ondragstart(IHTMLEventObj evtObj);
            [DispId(-2147418108)] bool onbeforeupdate(IHTMLEventObj evtObj);
            [DispId(-2147418107)] void onafterupdate(IHTMLEventObj evtObj);
            [DispId(-2147418099)] bool onerrorupdate(IHTMLEventObj evtObj);
            [DispId(-2147418106)] bool onrowexit(IHTMLEventObj evtObj);
            [DispId(-2147418105)] void onrowenter(IHTMLEventObj evtObj);
            [DispId(-2147418098)] void ondatasetchanged(IHTMLEventObj evtObj);
            [DispId(-2147418097)] void ondataavailable(IHTMLEventObj evtObj);
            [DispId(-2147418096)] void ondatasetcomplete(IHTMLEventObj evtObj);
            [DispId(-2147418094)] void onlosecapture(IHTMLEventObj evtObj);
            [DispId(-2147418093)] void onpropertychange(IHTMLEventObj evtObj);
            [DispId(1014)] void onscroll(IHTMLEventObj evtObj);
            [DispId(-2147418111)] void onfocus(IHTMLEventObj evtObj);
            [DispId(-2147418112)] void onblur(IHTMLEventObj evtObj);
            [DispId(1016)] void onresize(IHTMLEventObj evtObj);
            [DispId(-2147418092)] bool ondrag(IHTMLEventObj evtObj);
            [DispId(-2147418091)] void ondragend(IHTMLEventObj evtObj);
            [DispId(-2147418090)] bool ondragenter(IHTMLEventObj evtObj);
            [DispId(-2147418089)] bool ondragover(IHTMLEventObj evtObj);
            [DispId(-2147418088)] void ondragleave(IHTMLEventObj evtObj);
            [DispId(-2147418087)] bool ondrop(IHTMLEventObj evtObj);
            [DispId(-2147418083)] bool onbeforecut(IHTMLEventObj evtObj);
            [DispId(-2147418086)] bool oncut(IHTMLEventObj evtObj);
            [DispId(-2147418082)] bool onbeforecopy(IHTMLEventObj evtObj);
            [DispId(-2147418085)] bool oncopy(IHTMLEventObj evtObj);
            [DispId(-2147418081)] bool onbeforepaste(IHTMLEventObj evtObj);
            [DispId(-2147418084)] bool onpaste(IHTMLEventObj evtObj);
            [DispId(1023)] bool oncontextmenu(IHTMLEventObj evtObj);
            [DispId(-2147418080)] void onrowsdelete(IHTMLEventObj evtObj);
            [DispId(-2147418079)] void onrowsinserted(IHTMLEventObj evtObj);
            [DispId(-2147418078)] void oncellchange(IHTMLEventObj evtObj);
            [DispId(-609)] void onreadystatechange(IHTMLEventObj evtObj);
            [DispId(1030)] void onlayoutcomplete(IHTMLEventObj evtObj);
            [DispId(1031)] void onpage(IHTMLEventObj evtObj);
            [DispId(1042)] void onmouseenter(IHTMLEventObj evtObj);
            [DispId(1043)] void onmouseleave(IHTMLEventObj evtObj);
            [DispId(1044)] void onactivate(IHTMLEventObj evtObj);
            [DispId(1045)] void ondeactivate(IHTMLEventObj evtObj);
            [DispId(1034)] bool onbeforedeactivate(IHTMLEventObj evtObj);
            [DispId(1047)] bool onbeforeactivate(IHTMLEventObj evtObj);
            [DispId(1048)] void onfocusin(IHTMLEventObj evtObj);
            [DispId(1049)] void onfocusout(IHTMLEventObj evtObj);
            [DispId(1035)] void onmove(IHTMLEventObj evtObj);
            [DispId(1036)] bool oncontrolselect(IHTMLEventObj evtObj);
            [DispId(1038)] bool onmovestart(IHTMLEventObj evtObj);
            [DispId(1039)] void onmoveend(IHTMLEventObj evtObj);
            [DispId(1040)] bool onresizestart(IHTMLEventObj evtObj);
            [DispId(1041)] void onresizeend(IHTMLEventObj evtObj);
            [DispId(1033)] bool onmousewheel(IHTMLEventObj evtObj);
        }
    }
}
