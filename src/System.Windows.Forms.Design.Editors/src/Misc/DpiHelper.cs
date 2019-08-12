// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using CAPS = System.Windows.Forms.Design.NativeMethods;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Helper class for scaling coordinates and images according to current DPI scaling set in Windows for the primary screen.
    /// </summary>
    internal static partial class DpiHelper
    {
        internal const double LogicalDpi = 96.0;
        private static bool isInitialized = false;

        /// <summary>
        ///  The primary screen's (device) current DPI
        /// </summary>
        private static double deviceDpi = LogicalDpi;
        private static double logicalToDeviceUnitsScalingFactor = 0.0;
        private static InterpolationMode interpolationMode = InterpolationMode.Invalid;

        private static void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            using ScreenDC dc = ScreenDC.Create();
            deviceDpi = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSX);
            isInitialized = true;
        }

        private static double LogicalToDeviceUnitsScalingFactor
        {
            get
            {
                if (logicalToDeviceUnitsScalingFactor == 0.0)
                {
                    Initialize();
                    logicalToDeviceUnitsScalingFactor = deviceDpi / LogicalDpi;
                }
                return logicalToDeviceUnitsScalingFactor;
            }
        }

        private static InterpolationMode InterpolationMode
        {
            get
            {
                if (interpolationMode == InterpolationMode.Invalid)
                {
                    int dpiScalePercent = (int)Math.Round(LogicalToDeviceUnitsScalingFactor * 100);

                    // We will prefer NearestNeighbor algorithm for 200, 300, 400, etc zoom factors, in which each pixel become a 2x2, 3x3, 4x4, etc rectangle.
                    // This produces sharp edges in the scaled image and doesn't cause distorsions of the original image.
                    // For any other scale factors we will prefer a high quality resizing algorith. While that introduces fuzziness in the resulting image,
                    // it will not distort the original (which is extremely important for small zoom factors like 125%, 150%).
                    // We'll use Bicubic in those cases, except on reducing (zoom < 100, which we shouldn't have anyway), in which case Linear produces better
                    // results because it uses less neighboring pixels.
                    if ((dpiScalePercent % 100) == 0)
                    {
                        interpolationMode = InterpolationMode.NearestNeighbor;
                    }
                    else if (dpiScalePercent < 100)
                    {
                        interpolationMode = InterpolationMode.HighQualityBilinear;
                    }
                    else
                    {
                        interpolationMode = InterpolationMode.HighQualityBicubic;
                    }
                }
                return interpolationMode;
            }
        }

        private static Bitmap ScaleBitmapToSize(Bitmap logicalImage, Size deviceImageSize)
        {
            Bitmap deviceImage;
            deviceImage = new Bitmap(deviceImageSize.Width, deviceImageSize.Height, logicalImage.PixelFormat);

            using (Graphics graphics = Graphics.FromImage(deviceImage))
            {
                graphics.InterpolationMode = InterpolationMode;

                RectangleF sourceRect = new RectangleF(0, 0, logicalImage.Size.Width, logicalImage.Size.Height);
                RectangleF destRect = new RectangleF(0, 0, deviceImageSize.Width, deviceImageSize.Height);

                // Specify a source rectangle shifted by half of pixel to account for GDI+ considering the source origin the center of top-left pixel
                // Failing to do so will result in the right and bottom of the bitmap lines being interpolated with the graphics' background color,
                // and will appear black even if we cleared the background with transparent color.
                // The apparition of these artifacts depends on the interpolation mode, on the dpi scaling factor, etc.
                // E.g. at 150% DPI, Bicubic produces them and NearestNeighbor is fine, but at 200% DPI NearestNeighbor also shows them.
                sourceRect.Offset(-0.5f, -0.5f);

                graphics.DrawImage(logicalImage, destRect, sourceRect, GraphicsUnit.Pixel);
            }

            return deviceImage;
        }

        private static Bitmap CreateScaledBitmap(Bitmap logicalImage, int deviceDpi = 0)
        {
            Size deviceImageSize = DpiHelper.LogicalToDeviceUnits(logicalImage.Size, deviceDpi);
            return ScaleBitmapToSize(logicalImage, deviceImageSize);
        }

        /// <summary>
        ///  Returns whether scaling is required when converting between logical-device units,
        ///  if the application opted in the automatic scaling in the .config file.
        /// </summary>
        public static bool IsScalingRequired
        {
            get
            {
                Initialize();
                return deviceDpi != LogicalDpi;
            }
        }

        /// <summary>
        ///  Transforms a horizontal or vertical integer coordinate from logical to device units
        ///  by scaling it up  for current DPI and rounding to nearest integer value
        /// </summary>
        /// <param name="value">value in logical units</param>
        /// <returns>value in device units</returns>
        public static int LogicalToDeviceUnits(int value, int devicePixels = 0)
        {
            if (devicePixels == 0)
            {
                return (int)Math.Round(LogicalToDeviceUnitsScalingFactor * (double)value);
            }
            double scalingFactor = devicePixels / LogicalDpi;
            return (int)Math.Round(scalingFactor * (double)value);
        }

        /// <summary>
        ///  Transforms a horizontal integer coordinate from logical to device units
        ///  by scaling it up  for current DPI and rounding to nearest integer value
        /// </summary>
        /// <param name="value">The horizontal value in logical units</param>
        /// <returns>The horizontal value in device units</returns>
        public static int LogicalToDeviceUnitsX(int value)
        {
            return LogicalToDeviceUnits(value, 0);
        }

        /// <summary>
        ///  Transforms a vertical integer coordinate from logical to device units
        ///  by scaling it up  for current DPI and rounding to nearest integer value
        /// </summary>
        /// <param name="value">The vertical value in logical units</param>
        /// <returns>The vertical value in device units</returns>
        public static int LogicalToDeviceUnitsY(int value)
        {
            return LogicalToDeviceUnits(value, 0);
        }

        /// <summary>
        ///  Returns a new Size with the input's
        ///  dimensions converted from logical units to device units.
        /// </summary>
        /// <param name="logicalSize">Size in logical units</param>
        /// <returns>Size in device units</returns>
        public static Size LogicalToDeviceUnits(Size logicalSize, int deviceDpi = 0)
        {
            return new Size(LogicalToDeviceUnits(logicalSize.Width, deviceDpi),
                            LogicalToDeviceUnits(logicalSize.Height, deviceDpi));
        }

        /// <summary>
        ///  Create a new button bitmap scaled for the device units.
        ///  Note: original image might be disposed.
        /// </summary>
        /// <param name="button">button with an image, image size is defined in logical units</param>
        public static void ScaleButtonImageLogicalToDevice(Button button)
        {
            if (button == null)
            {
                return;
            }
            if (!(button.Image is Bitmap buttonBitmap))
            {
                return;
            }
            Bitmap deviceBitmap = CreateScaledBitmap(buttonBitmap);
            button.Image.Dispose();
            button.Image = deviceBitmap;
        }
    }

    internal enum DpiAwarenessContext
    {
        DPI_AWARENESS_CONTEXT_UNSPECIFIED = 0,
        DPI_AWARENESS_CONTEXT_UNAWARE = -1,
        DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = -2,
        DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = -3,
        DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4
    }
}
