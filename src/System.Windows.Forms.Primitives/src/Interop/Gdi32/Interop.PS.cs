// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [Flags]
        public enum PS : uint
        {
            SOLID = 0x00000000,
            DASH = 0x00000001,
            DOT = 0x00000002,
            DASHDOT = 0x00000003,
            DASHDOTDOT = 0x00000004,
            NULL = 0x00000005,
            INSIDEFRAME = 0x00000006,
            USERSTYLE = 0x00000007,
            ALTERNATE = 0x00000008,
            STYLE_MASK = 0x0000000f,

            ENDCAP_ROUND = 0x00000000,
            ENDCAP_SQUARE = 0x00000100,
            ENDCAP_FLAT = 0x00000200,
            ENDCAP_MASK = 0x00000f00,

            JOIN_ROUND = 0x00000000,
            JOIN_BEVEL = 0x00001000,
            JOIN_MITER = 0x00002000,
            JOIN_MASK = 0x0000f000,

            COSMETIC = 0x00000000,
            GEOMETRIC = 0x00010000,
            TYPE_MASK = 0x000f0000,
        }
    }
}
