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
            GETPOS = WM_USER,
            GETRANGEMIN = WM_USER + 1,
            GETRANGEMAX = WM_USER + 2,
            GETTIC = WM_USER + 3,
            SETTIC = WM_USER + 4,
            SETPOS = WM_USER + 5,
            SETRANGE = WM_USER + 6,
            SETRANGEMIN = WM_USER + 7,
            SETRANGEMAX = WM_USER + 8,
            CLEARTICS = WM_USER + 9,
            SETSEL = WM_USER + 10,
            SETSELSTART = WM_USER + 11,
            SETSELEND = WM_USER + 12,
            GETPTICS = WM_USER + 14,
            GETTICPOS = WM_USER + 15,
            GETNUMTICS = WM_USER + 16,
            GETSELSTART = WM_USER + 17,
            GETSELEND = WM_USER + 18,
            CLEARSEL = WM_USER + 19,
            SETTICFREQ = WM_USER + 20,
            SETPAGESIZE = WM_USER + 21,
            GETPAGESIZE = WM_USER + 22,
            SETLINESIZE = WM_USER + 23,
            GETLINESIZE = WM_USER + 24,
            GETTHUMBRECT = WM_USER + 25,
            GETCHANNELRECT = WM_USER + 26,
            SETTHUMBLENGTH = WM_USER + 27,
            GETTHUMBLENGTH = WM_USER + 28,
            SETTOOLTIPS = WM_USER + 29,
            GETTOOLTIPS = WM_USER + 30,
            SETTIPSIDE = WM_USER + 31,
            SETBUDDY = WM_USER + 32,
            GETBUDDY = WM_USER + 33,
            SETPOSNOTIFY = WM_USER + 34,
            SETUNICODEFORMAT = CCM.SETUNICODEFORMAT,
            GETUNICODEFORMAT = CCM.GETUNICODEFORMAT,
        }
    }
}
