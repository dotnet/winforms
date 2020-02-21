// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LVGS : uint
        {
            NORMAL = 0x00000000,
            COLLAPSED = 0x00000001,
            HIDDEN = 0x00000002,
            NOHEADER = 0x00000004,
            COLLAPSIBLE = 0x00000008,
            FOCUSED = 0x00000010,
            SELECTED = 0x00000020,
            SUBSETED = 0x00000040,
            SUBSETLINKFOCUSED = 0x00000080,
        }
    }
}
