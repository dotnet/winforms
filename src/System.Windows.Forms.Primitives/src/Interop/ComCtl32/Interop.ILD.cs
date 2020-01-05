// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum ILD : uint
        {
            NORMAL = 0x00000000,
            TRANSPARENT = 0x00000001,
            MASK = 0x00000010,
            IMAGE = 0x00000020,
            ROP = 0x00000040,
            BLEND25 = 0x00000002,
            BLEND50 = 0x00000004,
            OVERLAYMASK = 0x00000F00,
            PRESERVEALPHA = 0x00001000,
            SCALE = 0x00002000,
            DPISCALE = 0x00004000,
            ASYNC = 0x00008000,
            SELECTED = BLEND50,
            FOCUS = BLEND25,
            BLEND = BLEND50,
        }
    }
}
