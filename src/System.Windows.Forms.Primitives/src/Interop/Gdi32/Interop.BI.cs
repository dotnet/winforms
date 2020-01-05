// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public enum BI : uint
        {
            RGB = 0,
            RLE8 = 1,
            RLE4 = 2,
            BITFIELDS = 3,
            JPEG = 4,
            PNG = 5,
        }
    }
}
