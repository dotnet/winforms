// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum TBM : uint
        {
            GETPOS = WM.USER,
            GETRANGEMIN = WM.USER + 1,
            GETRANGEMAX = WM.USER + 2,
            GETTIC = WM.USER + 3,
            SETTIC = WM.USER + 4,
            SETPOS = WM.USER + 5,
            SETRANGE = WM.USER + 6,
            SETRANGEMIN = WM.USER + 7,
            SETRANGEMAX = WM.USER + 8,
            CLEARTICS = WM.USER + 9,
            SETSEL = WM.USER + 10,
            SETSELSTART = WM.USER + 11,
            SETSELEND = WM.USER + 12,
            GETPTICS = WM.USER + 14,
            GETTICPOS = WM.USER + 15,
            GETNUMTICS = WM.USER + 16,
            GETSELSTART = WM.USER + 17,
            GETSELEND = WM.USER + 18,
            CLEARSEL = WM.USER + 19,
            SETTICFREQ = WM.USER + 20,
            SETPAGESIZE = WM.USER + 21,
            GETPAGESIZE = WM.USER + 22,
            SETLINESIZE = WM.USER + 23,
            GETLINESIZE = WM.USER + 24,
            GETTHUMBRECT = WM.USER + 25,
            GETCHANNELRECT = WM.USER + 26,
            SETTHUMBLENGTH = WM.USER + 27,
            GETTHUMBLENGTH = WM.USER + 28,
            SETTOOLTIPS = WM.USER + 29,
            GETTOOLTIPS = WM.USER + 30,
            SETTIPSIDE = WM.USER + 31,
            SETBUDDY = WM.USER + 32,
            GETBUDDY = WM.USER + 33,
            SETPOSNOTIFY = WM.USER + 34,
            SETUNICODEFORMAT = CCM.SETUNICODEFORMAT,
            GETUNICODEFORMAT = CCM.GETUNICODEFORMAT,
        }
    }
}
