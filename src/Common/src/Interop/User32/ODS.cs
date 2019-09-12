// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum ODS : uint
        {
            SELECTED = 0x0001,
            GRAYED = 0x0002,
            DISABLED = 0x0004,
            CHECKED = 0x0008,
            FOCUS = 0x0010,
            DEFAULT = 0x0020,
            HOTLIGHT = 0x0040,
            INACTIVE = 0x0080,
            NOACCEL = 0x0100,
            NOFOCUSRECT = 0x0200,
            COMBOBOXEDIT = 0x1000,
        }
    }
}
