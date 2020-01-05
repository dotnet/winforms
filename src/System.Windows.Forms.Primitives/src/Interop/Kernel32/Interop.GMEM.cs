// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Kernel32
    {
        [Flags]
        public enum GMEM : uint
        {
            FIXED = 0x0000,
            MOVEABLE = 0x0002,
            NOCOMPACT = 0x0010,
            NODISCARD = 0x0020,
            ZEROINIT = 0x0040,
            MODIFY = 0x0080,
            DISCARDABLE = 0x0100,
            NOT_BANKED = 0x1000,
            SHARE = 0x2000,
            DDESHARE = 0x2000,
            NOTIFY = 0x4000,
            LOWER = NOT_BANKED,
            DISCARDED = 0x4000,
            LOCKCOUNT = 0x00ff,
            INVALID_HANDLE = 0x8000,

            GHND = MOVEABLE | ZEROINIT,
            GPTR = FIXED | ZEROINIT,
        }
    }
}
