// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TVIS : uint
        {
            FOCUSED = 0x0001,
            SELECTED = 0x0002,
            CUT = 0x0004,
            DROPHILITED = 0x0008,
            BOLD = 0x0010,
            EXPANDED = 0x0020,
            EXPANDEDONCE = 0x0040,
            EXPANDPARTIAL = 0x0080,
            OVERLAYMASK = 0x0F00,
            STATEIMAGEMASK = 0xF000,
            USERMASK = STATEIMAGEMASK
        }
    }
}
