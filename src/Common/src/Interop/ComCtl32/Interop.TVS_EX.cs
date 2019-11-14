// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TVS_EX : uint
        {
            NOSINGLECOLLAPSE = 0x0001,
            MULTISELECT = 0x0002,
            DOUBLEBUFFER = 0x0004,
            NOINDENTSTATE = 0x0008,
            RICHTOOLTIP = 0x0010,
            AUTOHSCROLL = 0x0020,
            FADEINOUTEXPANDOS = 0x0040,
            PARTIALCHECKBOXES = 0x0080,
            EXCLUSIONCHECKBOXES = 0x0100,
            DIMMEDCHECKBOXES = 0x0200,
            DRAWIMAGEASYNC = 0x0400,
        }
    }
}
