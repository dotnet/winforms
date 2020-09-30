// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [Flags]
        public enum SCF : uint
        {
            DEFAULT         = 0x0000,
            SELECTION       = 0x0001,
            WORD            = 0x0002,
            ALL             = 0x0004,
            USEUIRULES      = 0x0008,
            ASSOCIATEFONT   = 0x0010,
            NOKBUPDATE      = 0x0020,
            ASSOCIATEFONT2  = 0x0040,
            SMARTFONT       = 0x0080,
            CHARREPFROMLCID = 0x0100,
        }
    }
}
