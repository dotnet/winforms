// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Shell32
    {
        [Flags]
        public enum NIIF : uint
        {
            NONE = 0x00000000,
            INFO = 0x00000001,
            WARNING = 0x00000002,
            ERROR = 0x00000003,
            USER = 0x00000004,
            ICON_MASK = 0x0000000F,
            NOSOUND = 0x00000010,
            LARGE_ICON = 0x00000020,
            RESPECT_QUIET_TIME = 0x00000080
        }
    }
}
