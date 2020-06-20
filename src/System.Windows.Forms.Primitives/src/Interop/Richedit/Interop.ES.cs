// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [Flags]
        public enum ES
        {
            NOOLEDRAGDROP = 0x00000008,
            DISABLENOSCROLL = 0x00002000,
            SUNKEN = 0x00004000,
            SAVESEL = 0x00008000,
            SELFIME = 0x00040000,
            NOIME = 0x00080000,
            VERTICAL = 0x00400000,
            SELECTIONBAR = 0x01000000
        }
    }
}
