// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("3050f625-98b5-11cf-bb82-00aa00bdce0b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        [TypeLibType(TypeLibTypeFlags.FHidden)]
        public interface DHTMLWindowEvents2
        {
            [DispId(1003)] void onload(IHTMLEventObj evtObj);
            [DispId(1008)] void onunload(IHTMLEventObj evtObj);
            [DispId(-2147418102)] bool onhelp(IHTMLEventObj evtObj);
            [DispId(-2147418111)] void onfocus(IHTMLEventObj evtObj);
            [DispId(-2147418112)] void onblur(IHTMLEventObj evtObj);
            [DispId(1002)] bool onerror(string description, string url, int line);
            [DispId(1016)] void onresize(IHTMLEventObj evtObj);
            [DispId(1014)] void onscroll(IHTMLEventObj evtObj);
            [DispId(1017)] void onbeforeunload(IHTMLEventObj evtObj);
            [DispId(1024)] void onbeforeprint(IHTMLEventObj evtObj);
            [DispId(1025)] void onafterprint(IHTMLEventObj evtObj);
        }
    }
}
