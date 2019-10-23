// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public enum OUT_PRECIS : byte
        {
            DEFAULT = 0,
            STRING = 1,
            CHARACTER = 2,
            STROKE = 3,
            TT = 4,
            DEVICE = 5,
            RASTER = 6,
            TT_ONLY = 7,
            OUTLINE = 8,
            SCREEN_OUTLINE = 9,
            PS_ONLY = 10
        }
    }
}
