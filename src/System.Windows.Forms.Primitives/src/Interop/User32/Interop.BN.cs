// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum BN : uint
        {
            CLICKED = 0,
            PAINT = 1,
            HILITE = 2,
            UNHILITE = 3,
            DISABLE = 4,
            DOUBLECLICKED = 5,
            PUSHED = HILITE,
            UNPUSHED = UNHILITE,
            DBLCLK = DOUBLECLICKED,
            SETFOCUS = 6,
            KILLFOCUS = 7,
        }
    }
}
