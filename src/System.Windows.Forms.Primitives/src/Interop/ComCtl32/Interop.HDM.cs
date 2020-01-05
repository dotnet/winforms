// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum HDM : int
        {
            FIRST = 0x1200,
            GETITEMCOUNT = FIRST + 0,
            INSERTITEMA = FIRST + 1,
            DELETEITEM = FIRST + 2,
            GETITEMA = FIRST + 3,
            SETITEMA = FIRST + 4,
            LAYOUT = FIRST + 5,
            HITTEST = FIRST + 6,
            GETITEMRECT = FIRST + 7,
            SETIMAGELIST = FIRST + 8,
            GETIMAGELIST = FIRST + 9,
            INSERTITEMW = FIRST + 10,
            GETITEMW = FIRST + 11,
            SETITEMW = FIRST + 12,
            ORDERTOINDEX = FIRST + 15,
            CREATEDRAGIMAGE = FIRST + 16,
            SETORDERARRAY = FIRST + 18,
            SETHOTDIVIDER = FIRST + 19,
            SETBITMAPMARGIN = FIRST + 20,
            GETBITMAPMARGIN = FIRST + 21,
            SETUNICODEFORMAT = (int)CCM.SETUNICODEFORMAT,
            GETUNICODEFORMAT = (int)CCM.GETUNICODEFORMAT,
            SETFILTERCHANGETIMEOUT = FIRST + 22,
            EDITFILTER = FIRST + 23,
            CLEARFILTER = FIRST + 24,
            GETITEMDROPDOWNRECT = FIRST + 25,
            GETOVERFLOWRECT = FIRST + 26,
            GETFOCUSEDITEM = FIRST + 27,
            SETFOCUSEDITEM = FIRST + 28,
        }
    }
}
