// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public enum FW : int
        {
            DONTCARE = 0,
            THIN = 100,
            EXTRALIGHT = 200,
            ULTRALIGHT = 200,
            LIGHT = 300,
            NORMAL = 400,
            REGULAR = 400,
            MEDIUM = 500,
            SEMIBOLD = 600,
            DEMIBOLD = 600,
            BOLD = 700,
            EXTRABOLD = 800,
            ULTRABOLD = 800,
            HEAVY = 900,
            BLACK = 900
        }
    }
}
