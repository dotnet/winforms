// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public enum MM : int
        {
            TEXT = 1,
            LOMETRIC = 2,
            HIMETRIC = 3,
            LOENGLISH = 4,
            HIENGLISH = 5,
            TWIPS = 6,
            ISOTROPIC = 7,
            ANISOTROPIC = 8,
        }
    }
}
