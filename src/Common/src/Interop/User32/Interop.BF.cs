// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum BF : uint
        {
            LEFT = 0x0001,
            TOP = 0x0002,
            RIGHT = 0x0004,
            BOTTOM = 0x0008,
            TOPLEFT = TOP | LEFT,
            TOPRIGHT = TOP | RIGHT,
            BOTTOMLEFT = BOTTOM | LEFT,
            BOTTOMRIGHT = BOTTOM | RIGHT,
            RECT = LEFT | TOP | RIGHT | BOTTOM,
            DIAGONAL = 0x0010,
            DIAGONAL_ENDTOPRIGHT = DIAGONAL | TOP | RIGHT,
            DIAGONAL_ENDTOPLEFT = DIAGONAL | TOP | LEFT,
            DIAGONAL_ENDBOTTOMLEFT = DIAGONAL | BOTTOM | LEFT,
            DIAGONAL_ENDBOTTOMRIGHT = DIAGONAL | BOTTOM | RIGHT,
            MIDDLE = 0x0800,
            SOFT = 0x1000,
            ADJUST = 0x2000,
            FLAT = 0x4000,
            MONO = 0x8000,
        }
    }
}
