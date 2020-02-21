// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum CBS : uint
        {
            SIMPLE = 0x0001,
            DROPDOWN = 0x0002,
            DROPDOWNLIST = 0x0003,
            OWNERDRAWFIXED = 0x0010,
            OWNERDRAWVARIABLE = 0x0020,
            AUTOHSCROLL = 0x0040,
            OEMCONVERT = 0x0080,
            SORT = 0x0100,
            HASSTRINGS = 0x0200,
            NOINTEGRALHEIGHT = 0x0400,
            DISABLENOSCROLL = 0x0800,
            UPPERCASE = 0x2000,
            LOWERCASE = 0x4000,
        }
    }
}
