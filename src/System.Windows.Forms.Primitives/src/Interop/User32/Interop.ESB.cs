// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum ESB : uint
        {
            ENABLE_BOTH = 0x0000,
            DISABLE_BOTH = 0x0003,
            DISABLE_LEFT = 0x0001,
            DISABLE_RIGHT = 0x0002,
            DISABLE_UP = 0x0001,
            DISABLE_DOWN = 0x0002,
            DISABLE_LTUP = DISABLE_LEFT,
            DISABLE_RTDN = DISABLE_RIGHT,
        }
    }
}
