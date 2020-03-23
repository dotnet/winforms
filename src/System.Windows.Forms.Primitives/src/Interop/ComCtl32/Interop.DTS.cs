// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum DTS
        {
            SHORTDATEFORMAT = 0x0000,
            UPDOWN = 0x0001,
            SHOWNONE = 0x0002,
            LONGDATEFORMAT = 0x0004,
            TIMEFORMAT = 0x0009,
            SHORTDATECENTURYFORMAT = 0x000C,
            APPCANPARSE = 0x0010,
            RIGHTALIGN = 0x0020
        }
    }
}
