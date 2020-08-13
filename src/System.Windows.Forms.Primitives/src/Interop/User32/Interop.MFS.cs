// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum MFS : uint
        {
            GRAYED = 0x00000003,
            DISABLED = GRAYED,
            CHECKED = MF.CHECKED,
            HILITE = MF.HILITE,
            ENABLED = MF.ENABLED,
            UNCHECKED = MF.UNCHECKED,
            UNHILITE = MF.UNHILITE,
            DEFAULT = MF.DEFAULT,
        }
    }
}
