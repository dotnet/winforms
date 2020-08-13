// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum HDN
        {
            FIRST = 0 - 300,
            LAST = 0 - 399,
            BEGINDRAG = FIRST - 10,
            ENDDRAG = FIRST - 11,
            FILTERCHANGE = FIRST - 12,
            FILTERBTNCLICK = FIRST - 13,
            BEGINFILTEREDIT = FIRST - 14,
            ENDFILTEREDIT = FIRST - 15,
            ITEMSTATEICONCLICK = FIRST - 16,
            ITEMKEYDOWN = FIRST - 17,
            DROPDOWN = FIRST - 18,
            OVERFLOWCLICK = FIRST - 19,
            ITEMCHANGINGW = FIRST - 20,
            ITEMCHANGEDW = FIRST - 21,
            ITEMCLICKW = FIRST - 22,
            ITEMDBLCLICKW = FIRST - 23,
            DIVIDERDBLCLICKW = FIRST - 25,
            BEGINTRACKW = FIRST - 26,
            ENDTRACKW = FIRST - 27,
            TRACKW = FIRST - 28,
            GETDISPINFOW = FIRST - 29
        }
    }
}
