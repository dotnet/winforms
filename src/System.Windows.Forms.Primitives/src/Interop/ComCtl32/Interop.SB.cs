// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum SB : uint
        {
            SETPARTS = WM.USER + 4,
            GETPARTS = WM.USER + 6,
            GETBORDERS = WM.USER + 7,
            SETMINHEIGHT = WM.USER + 8,
            SIMPLE = WM.USER + 9,
            GETRECT = WM.USER + 10,
            SETTEXT = WM.USER + 11,
            GETTEXTLENGTH = WM.USER + 12,
            GETTEXT = WM.USER + 13,
            ISSIMPLE = WM.USER + 14,
            SETICON = WM.USER + 15,
            SETTIPTEXT = WM.USER + 17,
            GETTIPTEXT = WM.USER + 19,
            GETICON = WM.USER + 20
        }
    }
}
