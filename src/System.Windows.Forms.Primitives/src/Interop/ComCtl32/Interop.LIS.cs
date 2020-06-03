// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum LIS : uint
        {
            ENABLED = 0x00000001,
            FOCUSED = 0x00000002,
            VISITED = 0x00000004,
            HOTTRACK = 0x00000008,
            DEFAULTCOLORS = 0x00000010,
        }
    }
}
