// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum GR : uint
        {
            GDIOBJECTS          = 0,
            USEROBJECTS         = 1,
            GDIOBJECTS_PEAK     = 2,
            USEROBJECTS_PEAK    = 4,
        }
    }
}
