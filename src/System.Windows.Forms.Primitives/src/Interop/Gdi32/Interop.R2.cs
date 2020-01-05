// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public enum R2 : uint
        {
            BLACK = 1,
            NOTMERGEPEN = 2,
            MASKNOTPEN = 3,
            NOTCOPYPEN = 4,
            MASKPENNOT = 5,
            NOT = 6,
            XORPEN = 7,
            NOTMASKPEN = 8,
            MASKPEN = 9,
            NOTXORPEN = 10,
            NOP = 11,
            MERGENOTPEN = 12,
            COPYPEN = 13,
            MERGEPENNOT = 14,
            MERGEPEN = 15,
            WHITE = 16,
            LAST = 16,
        }
    }
}
