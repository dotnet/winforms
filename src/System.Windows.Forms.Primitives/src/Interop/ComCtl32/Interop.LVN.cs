// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum LVN
        {
            FIRST = 0 - 100,
            LAST = 0 - 199,
            ITEMCHANGING = FIRST - 0,
            ITEMCHANGED = FIRST - 1,
            INSERTITEM = FIRST - 2,
            DELETEITEM = FIRST - 3,
            DELETEALLITEMS = FIRST - 4,
            BEGINLABELEDITW = FIRST - 75,
            ENDLABELEDITW = FIRST - 76,
            COLUMNCLICK = FIRST - 8,
            BEGINDRAG = FIRST - 9,
            BEGINRDRAG = FIRST - 11,
            ODCACHEHINT = FIRST - 13,
            ODFINDITEMW = FIRST - 79,
            ITEMACTIVATE = FIRST - 14,
            ODSTATECHANGED = FIRST - 15,
            HOTTRACK = FIRST - 21,
            GETDISPINFOW = FIRST - 77,
            SETDISPINFOW = FIRST - 78,
            KEYDOWN = FIRST - 55,
            MARQUEEBEGIN = FIRST - 56,
            GETINFOTIPW = FIRST - 58,
            INCREMENTALSEARCHW = FIRST - 63,
            COLUMNDROPDOWN = FIRST - 64,
            COLUMNOVERFLOWCLICK = FIRST - 66,
            BEGINSCROLL = FIRST - 80,
            ENDSCROLL = FIRST - 81,
            LINKCLICK = FIRST - 84,
            GETEMPTYMARKUP = FIRST - 87
        }
    }
}
