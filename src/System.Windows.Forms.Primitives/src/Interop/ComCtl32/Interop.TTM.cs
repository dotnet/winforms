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
            ACTIVATE = WM_USER + 1,
            SETDELAYTIME = WM_USER + 3,
            RELAYEVENT = WM_USER + 7,
            GETTOOLCOUNT = WM_USER + 13,
            WINDOWFROMPOINT = WM_USER + 16,
            TRACKACTIVATE = WM_USER + 17,
            TRACKPOSITION = WM_USER + 18,
            SETTIPBKCOLOR = WM_USER + 19,
            SETTIPTEXTCOLOR = WM_USER + 20,
            GETDELAYTIME = WM_USER + 21,
            GETTIPBKCOLOR = WM_USER + 22,
            GETTIPTEXTCOLOR = WM_USER + 23,
            SETMAXTIPWIDTH = WM_USER + 24,
            GETMAXTIPWIDTH = WM_USER + 25,
            SETMARGIN = WM_USER + 26,
            GETMARGIN = WM_USER + 27,
            POP = WM_USER + 28,
            UPDATE = WM_USER + 29,
            GETBUBBLESIZE = WM_USER + 30,
            ADJUSTRECT = WM_USER + 31,
            SETTITLEW = WM_USER + 33,
            POPUP = WM_USER + 34,
            GETTITLE = WM_USER + 35,
            ADDTOOLW = WM_USER + 50,
            DELTOOLW = WM_USER + 51,
            NEWTOOLRECTW = WM_USER + 52,
            GETTOOLINFOW = WM_USER + 53,
            SETTOOLINFOW = WM_USER + 54,
            HITTESTW = WM_USER + 55,
            GETTEXTW = WM_USER + 56,
            UPDATETIPTEXTW = WM_USER + 57,
            ENUMTOOLSW = WM_USER + 58,
            GETCURRENTTOOLW = WM_USER + 59,
            GETWINDOWTHEME = CCM.SETWINDOWTHEME,
        }
    }
}
