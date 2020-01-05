// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [Flags]
        public enum ETO : uint
        {
            OPAQUE = 0x0002,
            CLIPPED = 0x0004,
            GLYPH_INDEX = 0x0010,
            RTLREADING = 0x0080,
            NUMERICSLOCAL = 0x0400,
            NUMERICSLATIN = 0x0800,
            IGNORELANGUAGE = 0x1000,
            PDY = 0x2000,
            REVERSE_INDEX_MAP = 0x10000,
        }
    }
}
