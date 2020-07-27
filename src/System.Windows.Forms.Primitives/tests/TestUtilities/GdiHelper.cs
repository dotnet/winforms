// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System
{
    /// <summary>
    ///  Utility class for Gdi related things.
    /// </summary>
    public static class GdiHelper
    {
        static GdiHelper()
        {
            using var dc = User32.GetDcScope.ScreenDC;
            LogicalPixelsX = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSX);
            LogicalPixelsY = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSY);
        }

        /// <summary>
        ///  Logical pixels per inch horizontally.
        /// </summary>
        public static int LogicalPixelsX { get; private set; }

        /// <summary>
        ///  Logical pixels per inch vertically.
        /// </summary>
        public static int LogicalPixelsY { get; private set; }

        // private const int TwipsPerInch = 1440;

        /// <summary>
        ///  Himetric units per inch.
        /// </summary>
        public const int HiMetricPerInch = 2540;

        /// <summary>
        ///  Convert HiMetric units to pixels (width).
        /// </summary>
        public static int HimetricToPixelX(int himetric)
            => (int)Math.Round((double)((long)LogicalPixelsX * himetric) / HiMetricPerInch, 0);

        /// <summary>
        ///  Convert Himetric units to pixels (height).
        /// </summary>
        public static int HimetricToPixelY(int himetric)
            => (int)Math.Round((double)((long)LogicalPixelsY * himetric) / HiMetricPerInch, 0);
    }
}
