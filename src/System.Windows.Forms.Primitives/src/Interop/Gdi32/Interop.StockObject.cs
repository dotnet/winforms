// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Stock GDI object identifiers.
        /// </summary>
        /// <remarks>
        ///  Note that in metafile records these values are OR'ed with 0x80000000.
        /// </remarks>
        internal enum StockObject : int
        {
            WHITE_BRUSH             = 0x00000000,   // 0x00FFFFFF
            LTGRAY_BRUSH            = 0x00000001,   // 0x00C0C0C0
            GRAY_BRUSH              = 0x00000002,   // 0x00808080
            DKGRAY_BRUSH            = 0x00000003,   // 0x00404040
            BLACK_BRUSH             = 0x00000004,   // 0x00000000
            NULL_BRUSH              = 0x00000005,   // Same as HOLLOW_BRUSH
            WHITE_PEN               = 0x00000006,   // 0x00FFFFFF
            BLACK_PEN               = 0x00000007,   // 0x00000000
            NULL_PEN                = 0x00000008,
            OEM_FIXED_FONT          = 0x0000000A,
            ANSI_FIXED_FONT         = 0x0000000B,
            ANSI_VAR_FONT           = 0x0000000C,
            SYSTEM_FONT             = 0x0000000D,
            DEVICE_DEFAULT_FONT     = 0x0000000E,
            DEFAULT_PALETTE         = 0x0000000F,
            SYSTEM_FIXED_FONT       = 0x00000010,
            DEFAULT_GUI_FONT        = 0x00000011,
            DC_BRUSH                = 0x00000012,
            DC_PEN                  = 0x00000013
        }
    }
}
