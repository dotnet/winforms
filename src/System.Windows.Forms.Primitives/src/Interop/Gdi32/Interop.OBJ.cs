// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public enum OBJ : int
        {
            PEN = 1,
            BRUSH = 2,
            DC = 3,
            METADC = 4,
            PAL = 5,
            FONT = 6,
            BITMAP = 7,
            REGION = 8,
            METAFILE = 9,
            MEMDC = 10,
            EXTPEN = 11,
            ENHMETADC = 12,
            ENHMETAFILE = 13,
            COLORSPACE = 14
        }
    }
}
