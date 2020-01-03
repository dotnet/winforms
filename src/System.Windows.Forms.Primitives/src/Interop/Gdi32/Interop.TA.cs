// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [Flags]
        public enum TA : uint
        {
            NOUPDATECP = 0x00,
            UPDATECP = 0x01,
            LEFT = 0x00,
            RIGHT = 0x02,
            CENTER = 0x06,
            TOP = 0x00,
            BOTTOM = 0x08,
            BASELINE = 0x18,
            RTLREADING = 0x100,
            MASK = BASELINE + CENTER + UPDATECP + RTLREADING
        }
    }
}
