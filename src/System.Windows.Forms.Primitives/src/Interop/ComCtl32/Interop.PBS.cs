﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum PBS : uint
        {
            SMOOTH = 0x01,
            VERTICAL = 0x04,
            MARQUEE = 0x08,
            SMOOTHREVERSE = 0x10,
        }
    }
}
