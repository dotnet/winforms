// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum SBS : uint
        {
            HORZ = 0x0000,
            VERT = 0x0001,
            TOPALIGN = 0x0002,
            LEFTALIGN = 0x0002,
            SIZEBOXTOPLEFTALIGN = 0x0002,
            BOTTOMALIGN = 0x0004,
            RIGHTALIGN = 0x0004,
            SIZEBOXBOTTOMRIGHTALIGN = 0x0004,
            SIZEBOX = 0x0008,
            SIZEGRIP = 0x0010
        }
    }
}
