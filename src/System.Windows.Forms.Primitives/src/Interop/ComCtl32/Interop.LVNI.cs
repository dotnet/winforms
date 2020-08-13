// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVNI : uint
        {
            ALL = 0x0000,

            FOCUSED = 0x0001,
            SELECTED = 0x0002,
            CUT = 0x0004,
            DROPHILITED = 0x0008,
            STATEMASK = FOCUSED | SELECTED | CUT | DROPHILITED,

            VISIBLEORDER = 0x0010,
            PREVIOUS = 0x0020,
            VISIBLEONLY = 0x0040,
            SAMEGROUPONLY = 0x0080,

            ABOVE = 0x0100,
            BELOW = 0x0200,
            TOLEFT = 0x0400,
            TORIGHT = 0x0800,
            DIRECTIONMASK = ABOVE | BELOW | TOLEFT | TORIGHT,
        }
    }
}
