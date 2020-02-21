// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public enum BS : uint
        {
            SOLID = 0,
            NULL = 1,
            HOLLOW = 1,
            HATCHED = 2,
            PATTERN = 3,
            INDEXED = 4,
            DIBPATTERN = 5,
            DIBPATTERNPT = 6,
            PATTERN8X8 = 7,
            DIBPATTERN8X8 = 8,
            MONOPATTERN = 9,
        }
    }
}
