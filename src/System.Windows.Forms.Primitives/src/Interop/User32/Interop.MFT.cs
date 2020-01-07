// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum MFT : uint
        {
            STRING = MF.STRING,
            BITMAP = MF.BITMAP,
            MENUBARBREAK = MF.MENUBARBREAK,
            MENUBREAK = MF.MENUBREAK,
            OWNERDRAW = MF.OWNERDRAW,
            RADIOCHECK = 0x00000200,
            SEPARATOR = MF.SEPARATOR,
            RIGHTORDER = 0x00002000,
            RIGHTJUSTIFY = MF.RIGHTJUSTIFY,
        }
    }
}
