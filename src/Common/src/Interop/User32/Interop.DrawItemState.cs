// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum DrawItemState : uint
        {
            ODS_CHECKED = 0x0008,
            ODS_COMBOBOXEDIT = 0x1000,
            ODS_DEFAULT = 0x0020,
            ODS_DISABLED = 0x0004,
            ODS_FOCUS = 0x0010,
            ODS_GRAYED = 0x0002,
            ODS_HOTLIGHT = 0x0040,
            ODS_INACTIVE = 0x0080,
            ODS_NOACCEL = 0x0100,
            ODS_NOFOCUSRECT = 0x0200,
            ODS_SELECTED = 0x0001,
        }
    }
}
