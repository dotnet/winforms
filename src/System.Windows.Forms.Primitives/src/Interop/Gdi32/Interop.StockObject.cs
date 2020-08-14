// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        internal enum StockObject : uint
        {
            WHITE_BRUSH             = 0x80000000,   // 0x00FFFFFF
            LTGRAY_BRUSH            = 0x80000001,   // 0x00C0C0C0
            GRAY_BRUSH              = 0x80000002,   // 0x00808080
            DKGRAY_BRUSH            = 0x80000003,   // 0x00404040
            BLACK_BRUSH             = 0x80000004,   // 0x00404040
            NULL_BRUSH              = 0x80000005,   // Same as HOLLOW_BRUSH
            WHITE_PEN               = 0x80000006,   // 0x00FFFFFF
            BLACK_PEN               = 0x80000007,   // 0x00000000
            NULL_PEN                = 0x80000008,
            OEM_FIXED_FONT          = 0x8000000A,
            ANSI_FIXED_FONT         = 0x8000000B,
            ANSI_VAR_FONT           = 0x8000000C,
            SYSTEM_FONT             = 0x8000000D,
            DEVICE_DEFAULT_FONT     = 0x8000000E,
            DEFAULT_PALETTE         = 0x8000000F,
            SYSTEM_FIXED_FONT       = 0x80000010,
            DEFAULT_GUI_FONT        = 0x80000011,
            DC_BRUSH                = 0x80000012,
            DC_PEN                  = 0x80000013
        }
    }
}
