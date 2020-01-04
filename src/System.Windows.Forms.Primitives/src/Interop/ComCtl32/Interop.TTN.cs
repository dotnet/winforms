// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum TTN : int
        {
            FIRST = 0 - 520,
            GETDISPINFOA = FIRST - 0,
            SHOW = FIRST - 1,
            POP = FIRST - 2,
            LINKCLICK = FIRST - 3,
            GETDISPINFOW = FIRST - 10,
        }
    }
}
