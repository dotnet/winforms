// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TBS : uint
        {
            AUTOTICKS = 0x0001,
            VERT = 0x0002,
            HORZ = 0x0000,
            TOP = 0x0004,
            BOTTOM = 0x0000,
            LEFT = 0x0004,
            RIGHT = 0x0000,
            BOTH = 0x0008,
            NOTICKS = 0x0010,
            ENABLESELRANGE = 0x0020,
            FIXEDLENGTH = 0x0040,
            NOTHUMB = 0x0080,
            TOOLTIPS = 0x0100,
            REVERSED = 0x0200,
            DOWNISLEFT = 0x0400,
            NOTIFYBEFOREMOVE = 0x0800,
            TRANSPARENTBKGND = 0x1000,
        }
    }
}
