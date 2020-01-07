// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum SS : uint
        {
            LEFT = 0x00000000,
            CENTER = 0x00000001,
            RIGHT = 0x00000002,
            ICON = 0x00000003,
            BLACKRECT = 0x00000004,
            GRAYRECT = 0x00000005,
            WHITERECT = 0x00000006,
            BLACKFRAME = 0x00000007,
            GRAYFRAME = 0x00000008,
            WHITEFRAME = 0x00000009,
            USERITEM = 0x0000000A,
            SIMPLE = 0x0000000B,
            LEFTNOWORDWRAP = 0x0000000C,
            OWNERDRAW = 0x0000000D,
            BITMAP = 0x0000000E,
            ENHMETAFILE = 0x0000000F,
            ETCHEDHORZ = 0x00000010,
            ETCHEDVERT = 0x00000011,
            ETCHEDFRAME = 0x00000012,
            TYPEMASK = 0x0000001F,
            REALSIZECONTROL = 0x00000040,
            NOPREFIX = 0x00000080,
            NOTIFY = 0x00000100,
            CENTERIMAGE = 0x00000200,
            RIGHTJUST = 0x00000400,
            REALSIZEIMAGE = 0x00000800,
            SUNKEN = 0x00001000,
            EDITCONTROL = 0x00002000,
            ENDELLIPSIS = 0x00004000,
            PATHELLIPSIS = 0x00008000,
            WORDELLIPSIS = 0x0000C000,
            ELLIPSISMASK = 0x0000C000
        }
    }
}
