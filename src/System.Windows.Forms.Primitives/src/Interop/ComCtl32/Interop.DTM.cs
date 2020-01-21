// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum DTM : uint
        {
            FIRST = 0x1000,
            GETSYSTEMTIME = FIRST + 1,
            SETSYSTEMTIME = FIRST + 2,
            GETRANGE = FIRST + 3,
            SETRANGE = FIRST + 4,
            SETMCCOLOR = FIRST + 6,
            GETMCCOLOR = FIRST + 7,
            GETMONTHCAL = FIRST + 8,
            SETMCFONT = FIRST + 9,
            GETMCFONT = FIRST + 10,
            SETMCSTYLE = FIRST + 11,
            GETMCSTYLE = FIRST + 12,
            CLOSEMONTHCAL = FIRST + 13,
            GETDATETIMEPICKERINFO = FIRST + 14,
            GETIDEALSIZE = FIRST + 15,
            SETFORMATW = FIRST + 50,
        }
    }
}
