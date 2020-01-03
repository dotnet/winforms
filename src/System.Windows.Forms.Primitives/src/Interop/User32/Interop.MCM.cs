// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum MCM : int
        {
            FIRST = 0x1000,
            SETTODAY = FIRST + 12,
            GETTODAY = FIRST + 13,
        }
    }
}
