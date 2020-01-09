// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum LBS : uint
        {
            NOTIFY = 0x0001,
            SORT = 0x0002,
            NOREDRAW = 0x0004,
            MULTIPLESEL = 0x0008,
            OWNERDRAWFIXED = 0x0010,
            OWNERDRAWVARIABLE = 0x0020,
            HASSTRINGS = 0x0040,
            USETABSTOPS = 0x0080,
            NOINTEGRALHEIGHT = 0x0100,
            MULTICOLUMN = 0x0200,
            WANTKEYBOARDINPUT = 0x0400,
            EXTENDEDSEL = 0x0800,
            DISABLENOSCROLL = 0x1000,
            NODATA = 0x2000,
            COMBOBOX = 0x8000,
            NOSEL = 0x4000,
            STANDARD = NOTIFY | SORT | WS.VSCROLL | WS.BORDER,
        }
    }
}
