// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public enum DIB : uint
        {
            RGB_COLORS = 0,
            PAL_COLORS = 1,
            PAL_INDICES = 2
        }
    }
}
