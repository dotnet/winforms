﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public enum DeviceCapability : int
        {
            BITSPIXEL = 12,
            PLANES = 14,
            LOGPIXELSX = 88,
            LOGPIXELSY = 90
        }

        [SuppressGCTransition]
        [LibraryImport(Libraries.Gdi32, SetLastError = true)]
        public static partial int GetDeviceCaps(HDC hDC, DeviceCapability nIndex);
    }
}
