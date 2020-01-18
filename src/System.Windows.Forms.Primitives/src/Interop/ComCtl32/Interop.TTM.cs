// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum TTM : uint
        {
            ACTIVATE = WM.USER + 1,
            SETDELAYTIME = WM.USER + 3,
            RELAYEVENT = WM.USER + 7,
            GETTOOLCOUNT = WM.USER + 13,
            WINDOWFROMPOINT = WM.USER + 16,
            TRACKACTIVATE = WM.USER + 17,
            TRACKPOSITION = WM.USER + 18,
            SETTIPBKCOLOR = WM.USER + 19,
            SETTIPTEXTCOLOR = WM.USER + 20,
            GETDELAYTIME = WM.USER + 21,
            GETTIPBKCOLOR = WM.USER + 22,
            GETTIPTEXTCOLOR = WM.USER + 23,
            SETMAXTIPWIDTH = WM.USER + 24,
            GETMAXTIPWIDTH = WM.USER + 25,
            SETMARGIN = WM.USER + 26,
            GETMARGIN = WM.USER + 27,
            POP = WM.USER + 28,
            UPDATE = WM.USER + 29,
            GETBUBBLESIZE = WM.USER + 30,
            ADJUSTRECT = WM.USER + 31,
            SETTITLEW = WM.USER + 33,
            POPUP = WM.USER + 34,
            GETTITLE = WM.USER + 35,
            ADDTOOLW = WM.USER + 50,
            DELTOOLW = WM.USER + 51,
            NEWTOOLRECTW = WM.USER + 52,
            GETTOOLINFOW = WM.USER + 53,
            SETTOOLINFOW = WM.USER + 54,
            HITTESTW = WM.USER + 55,
            GETTEXTW = WM.USER + 56,
            UPDATETIPTEXTW = WM.USER + 57,
            ENUMTOOLSW = WM.USER + 58,
            GETCURRENTTOOLW = WM.USER + 59,
            GETWINDOWTHEME = CCM.SETWINDOWTHEME,
        }
    }
}
