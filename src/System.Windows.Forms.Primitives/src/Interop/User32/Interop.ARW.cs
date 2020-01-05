// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum ARW : int
        {
            BOTTOMLEFT = 0x0000,
            BOTTOMRIGHT = 0x0001,
            TOPLEFT = 0x0002,
            TOPRIGHT = 0x0003,
            LEFT = 0x0000,
            RIGHT = 0x0000,
            UP = 0x0004,
            DOWN = 0x0004,
            HIDE = 0x0008
        }
    }
}
