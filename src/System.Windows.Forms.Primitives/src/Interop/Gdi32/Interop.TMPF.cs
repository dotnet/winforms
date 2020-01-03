// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [Flags]
        public enum TMPF : byte
        {
            FIXED_PITCH = 0x01,
            VECTOR = 0x02,
            TRUETYPE = 0x04,
            DEVICE = 0x08,
        }
    }
}
