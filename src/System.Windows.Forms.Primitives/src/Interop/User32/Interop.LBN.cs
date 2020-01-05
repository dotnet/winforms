// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum LBN : int
        {
            ERRSPACE = -2,
            SELCHANGE = 1,
            DBLCLK = 2,
            SELCANCEL = 3,
            SETFOCUS = 4,
            KILLFOCUS = 5,
        }
    }
}
