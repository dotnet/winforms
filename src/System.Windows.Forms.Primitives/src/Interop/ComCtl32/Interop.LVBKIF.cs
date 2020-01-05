// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVBKIF : uint
        {
            SOURCE_NONE = 0x00000000,
            SOURCE_HBITMAP = 0x00000001,
            SOURCE_URL = 0x00000002,
            SOURCE_MASK = 0x00000003,
            STYLE_NORMAL = 0x00000000,
            STYLE_TILE = 0x00000010,
            STYLE_MASK = 0x00000010,
            FLAG_TILEOFFSET = 0x00000100,
            TYPE_WATERMARK = 0x10000000,
            FLAG_ALPHABLEND = 0x20000000,
        }
    }
}
