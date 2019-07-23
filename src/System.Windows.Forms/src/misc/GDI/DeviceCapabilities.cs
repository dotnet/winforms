// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Device capability indexes - See Win32 GetDeviceCaps().
    /// </summary>
    internal enum DeviceCapabilities
    {
        DriverVersion = 0,          // device driver version
        Technology = 2,             // device classification
        HorizontalSize = 4,         // horizontal size in millimeters
        VerticalSize = 6,           // vertical size in millimeters
        HorizontalResolution = 8,   // horizontal width in pixels
        VerticalResolution = 10,    // vertical height in pixels
        BitsPerPixel = 12,          // number of bits per pixel
        LogicalPixelsX = 88,        // Logical pixels/inch in X
        LogicalPixelsY = 90,        // Logical pixels/inch in Y

        // printing related devicecaps. these replace the appropriate escapes

        PhysicalWidth = 110,        // physical width in device units
        PhysicalHeight = 111,       // physical height in device units
        PhysicalOffsetX = 112,      // physical printable area x margin
        PhysicalOffsetY = 113,      // physical printable area y margin
        ScalingFactorX = 114,       // scaling factor x
        ScalingFactorY = 115,       // scaling factor y
    }
}
