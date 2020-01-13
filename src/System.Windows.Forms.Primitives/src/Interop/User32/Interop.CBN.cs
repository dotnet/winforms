// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum CBN : int
        {
            ERRSPACE = -1,
            SELCHANGE = 1,
            DBLCLK = 2,
            SETFOCUS = 3,
            KILLFOCUS = 4,
            EDITCHANGE = 5,
            EDITUPDATE = 6,
            DROPDOWN = 7,
            CLOSEUP = 8,
            SELENDOK = 9,
            SELENDCANCEL = 10,
        }
    }
}
