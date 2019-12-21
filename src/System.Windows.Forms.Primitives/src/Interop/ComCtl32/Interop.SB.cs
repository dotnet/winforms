// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using static Interop.User32;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum SB : uint
        {
            SETPARTS = WM_USER + 4,
            GETPARTS = WM_USER + 6,
            GETBORDERS = WM_USER + 7,
            SETMINHEIGHT = WM_USER + 8,
            SIMPLE = WM_USER + 9,
            GETRECT = WM_USER + 10,
            SETTEXT = WM_USER + 11,
            GETTEXTLENGTH = WM_USER + 12,
            GETTEXT = WM_USER + 13,
            ISSIMPLE = WM_USER + 14,
            SETICON = WM_USER + 15,
            SETTIPTEXT = WM_USER + 17,
            GETTIPTEXT = WM_USER + 19,
            GETICON = WM_USER + 20
        }
    }
}
