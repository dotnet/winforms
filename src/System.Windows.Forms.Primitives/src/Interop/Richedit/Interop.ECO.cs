// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [Flags]
        public enum ECO
        {
            AUTOWORDSELECTION = 0x00000001,
            AUTOVSCROLL = 0x00000040,
            AUTOHSCROLL = 0x00000080,
            NOHIDESEL = 0x00000100,
            READONLY = 0x00000800,
            WANTRETURN = 0x00001000,
            SAVESEL = 0x00008000,
            VERTICAL = 0x00400000,
            SELECTIONBAR = 0x01000000
        }
    }
}
