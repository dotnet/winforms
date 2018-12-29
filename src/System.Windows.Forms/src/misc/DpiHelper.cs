// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

#if WINFORMS_NAMESPACE
using CAPS = System.Windows.Forms.NativeMethods;
#elif DRAWING_NAMESPACE
using CAPS = System.Drawing.SafeNativeMethods;
#elif DRAWINGDESIGN_NAMESPACE
using System.Drawing.Design;
using CAPS = System.Drawing.Design.NativeMethods;
#else
using System.Design;
using CAPS = System.Design.NativeMethods;
#endif

namespace System.Windows.Forms
{
    /// <summary>
    /// Helper class for scaling coordinates and images according to current DPI scaling set in Windows for the primary screen.
    /// </summary>
    internal static partial class DpiHelper
    {
        internal const double LogicalDpi = 96.0;
        private static bool isInitialized = false;
        private static bool isInitializeDpiHelperForWinforms = false;

        /// <summary>
        /// The primary screen's (device) current DPI
        /// </summary>
        private static double deviceDpi = LogicalDpi;
        private static double logicalToDeviceUnitsScalingFactor = 0.0;
        private static InterpolationMode interpolationMode = InterpolationMode.Invalid;

        // Indicates that per monitor dpi scaling v1 is enabled and the OS does not support
        // the dpi awareness context apis
        private static bool isPerMonitorV1 = false;

        // Backing field, indicating that we will need to send a PerMonitorV2 query in due course.
        private static bool doesNeedQueryForPerMonitorV2Awareness = false;

        // Backing field, indicating that either DPI is <> 96 or we are in some PerMonitor HighDpi mode.
        private static bool isScalingRequirementMet = false;

        private static void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            IntPtr hDC = UnsafeNativeMethods.GetDC(NativeMethods.NullHandleRef);
            if (hDC != IntPtr.Zero)
            {
                deviceDpi = UnsafeNativeMethods.GetDeviceCaps(new HandleRef(null, hDC), CAPS.LOGPIXELSX);

                UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, hDC));
            }
            isInitialized = true;
        }

        internal static void InitializeDpiHelperForWinforms()
        {
            if (isInitializeDpiHelperForWinforms)
            {
                return;
            }

            // initialize shared fields
            Initialize();

            NativeMethods.PROCESS_DPI_AWARENESS processDpiAwareness = CAPS.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNINITIALIZED;

            // Determine current dpi awareness mode and check for per monitor v1
            if (ApiHelper.IsApiAvailable(ExternDll.ShCore, nameof(SafeNativeMethods.GetProcessDpiAwareness)))
            {
                var currentProcessId = SafeNativeMethods.GetCurrentProcessId();
                IntPtr hProcess = SafeNativeMethods.OpenProcess(SafeNativeMethods.PROCESS_QUERY_INFORMATION, false, currentProcessId);
                var result = SafeNativeMethods.GetProcessDpiAwareness(hProcess, out processDpiAwareness);
                isPerMonitorV1 = processDpiAwareness == CAPS.PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE;
            }

            // We are in Windows 10/1603 or greater when this API is present.
            if (ApiHelper.IsApiAvailable(ExternDll.User32, nameof(CommonUnsafeNativeMethods.GetThreadDpiAwarenessContext)))
            {
                // We are on Windows 10/1603 or greater all right, but we could still be DpiUnaware or SystemAware, so let's find that out...
                // Only if we're not, it makes sense to query for PerMonitorV2 awareness from now on, if needed.
                if (!(processDpiAwareness == CAPS.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE ||
                      processDpiAwareness == CAPS.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE))
                {
                    isPerMonitorV1 = false;
                    doesNeedQueryForPerMonitorV2Awareness = true;
                }
            }

            if (IsScalingRequired || isPerMonitorV1 || doesNeedQueryForPerMonitorV2Awareness)
            {
                isScalingRequirementMet = true;
            }

            isInitializeDpiHelperForWinforms = true;
        }

        /// <summary>
        /// Returns a boolean to specify if the application is set to per monitor dpi v1 awareness
        /// </summary>
        internal static bool IsPerMonitorV1Awareness
        {
            get
            {
                InitializeDpiHelperForWinforms();
                if (doesNeedQueryForPerMonitorV2Awareness)
                {
                    // The OS might be per monitor v2 capable, but the application could still be in per monitor v1 mode
                    DpiAwarenessContext dpiAwareness = CommonUnsafeNativeMethods.GetThreadDpiAwarenessContext();
                    return CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwareness, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE);
                }
                else
                {
                    return isPerMonitorV1;
                }
            }
        }

        /// <summary>
        /// Returns a boolean to specify if we should enable processing of WM_DPICHANGED and related messages
        /// </summary>
        internal static bool IsPerMonitorV2Awareness
        {
            get
            {
                InitializeDpiHelperForWinforms();
                if (doesNeedQueryForPerMonitorV2Awareness)
                {
                    // We can't cache this value because different top level windows can have different DPI awareness context
                    // for mixed mode applications.
                    DpiAwarenessContext dpiAwareness = CommonUnsafeNativeMethods.GetThreadDpiAwarenessContext();
                    return CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwareness, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Indicates, if rescaling becomes necessary, either because we are not 96 DPI or we're PerMonitorV2Aware.
        /// </summary>
        internal static bool IsScalingRequirementMet
        {
            get
            {
                InitializeDpiHelperForWinforms();
                return isScalingRequirementMet;
            }
        }

        internal static int DeviceDpi
        {
            get
            {
                Initialize();
                return (int)deviceDpi;
            }
        }

        /// <summary>
        /// Returns a dpi value for a given handle
        /// </summary>
        internal static int GetDpiValueForHandle(HandleRef handle)
        {
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                int dpi = (int)UnsafeNativeMethods.GetDpiForWindow(handle);
                if (dpi != 0)
                {
                    return dpi;
                }
            }
            else if (DpiHelper.IsPerMonitorV1Awareness)
            {
                // Get monitor the handle belongs to
                IntPtr monitor = SafeNativeMethods.MonitorFromWindow(handle, 0);
                if (monitor != IntPtr.Zero)
                {
                    // Get dpi value for this monitor
                    uint dpiX = 0;
                    uint dpiY = 0;
                    if (SafeNativeMethods.GetDpiForMonitor(monitor, CAPS.MONITOR_DPI_TYPE.MDT_DEFAULT, out dpiX, out dpiY) == NativeMethods.S_OK)
                    {
                        return (int)dpiX;
                    }
                }
            }

            // Return system dpi by default
            return (DpiHelper.DeviceDpi);
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
        /// Returns whether scaling is required when converting between logical-device units,
        /// if the application opted in the automatic scaling in the .config file.
        /// TODO: Remove this property and update dependent code, because it only considers the system dpi
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
        /// Transforms a horizontal or vertical integer coordinate from logical to device units
        /// by scaling it up  for current DPI and rounding to nearest integer value
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

        // <summary>
        /// Transforms a horizontal or vertical integer coordinate from device units to logical
        /// by scaling it up for current DPI and rounding to nearest integer value
        /// </summary>
        /// <param name="value">value in logical units</param>
        /// <returns>value in device units</returns>
        public static int DeviceToLogicalUnits(int value, int devicePixels = 0)
        {
            if (devicePixels == 0)
            {
                return (int)Math.Round((double)value / LogicalToDeviceUnitsScalingFactor);
            }
            double scalingFactor = LogicalDpi / devicePixels;
            return (int)Math.Round(scalingFactor * (double)value);
        }

        /// <summary>
        /// Transforms a horizontal integer coordinate from logical to device units
        /// by scaling it up  for current DPI and rounding to nearest integer value
        /// </summary>
        /// <param name="value">The horizontal value in logical units</param>
        /// <returns>The horizontal value in device units</returns>
        public static int LogicalToDeviceUnitsX(int value)
        {
            return LogicalToDeviceUnits(value, 0);
        }

        /// <summary>
        /// Transforms a vertical integer coordinate from logical to device units
        /// by scaling it up  for current DPI and rounding to nearest integer value
        /// </summary>
        /// <param name="value">The vertical value in logical units</param>
        /// <returns>The vertical value in device units</returns>
        public static int LogicalToDeviceUnitsY(int value)
        {
            return LogicalToDeviceUnits(value, 0);
        }

        /// <summary>
        /// Returns a new Size with the input's
        /// dimensions converted from logical units to device units.
        /// </summary>
        /// <param name="logicalSize">Size in logical units</param>
        /// <returns>Size in device units</returns>
        public static Size LogicalToDeviceUnits(Size logicalSize, int deviceDpi = 0)
        {
            return new Size(LogicalToDeviceUnits(logicalSize.Width, deviceDpi),
                            LogicalToDeviceUnits(logicalSize.Height, deviceDpi));
        }

        /// <summary>
        /// Returns a new Rectangle with the input's
        /// dimensions converted from logical units to device units.
        /// </summary>
        /// <param name="logicalSize">Size in logical units</param>
        /// <param name="extendToPartialPixels">Round down the top left position and round up the bottom right position of the rectangle to the nearest integers</param>
        /// <returns>Size in device units</returns>
        public static Rectangle LogicalToDeviceUnits(Rectangle logicalRect, int deviceDpi = 0, bool extendToPartialPixels = false)
        {
            if(extendToPartialPixels)
            {
                double scalingFactor = deviceDpi == 0 ? LogicalToDeviceUnitsScalingFactor : (double)deviceDpi / LogicalDpi;
                Point bottomRight = logicalRect.Location + logicalRect.Size;
                Point scaledLocation = new Point((int)Math.Floor(scalingFactor * logicalRect.X), (int)Math.Floor(scalingFactor * logicalRect.Y));
                Point scaledBottomRight = new Point((int)Math.Ceiling(scalingFactor * bottomRight.X), (int)Math.Ceiling(scalingFactor * bottomRight.Y));
                Size scaledSize = new Size(scaledBottomRight) - new Size(scaledLocation);
                return new Rectangle(scaledLocation, scaledSize);
            }
            return new Rectangle(LogicalToDeviceUnits(logicalRect.X, deviceDpi),
                            LogicalToDeviceUnits(logicalRect.Y, deviceDpi),
                            LogicalToDeviceUnits(logicalRect.Width, deviceDpi),
                            LogicalToDeviceUnits(logicalRect.Height, deviceDpi));
        }

        /// <summary>
        /// Returns a new Size with the input's
        /// dimensions converted from device to logical units.
        /// </summary>
        /// <param name="logicalSize">Size in logical units</param>
        /// <returns>Size in device units</returns>
        public static Size DeviceToLogicalUnits(Size deviceSize, int deviceDpi = 0)
        {
            return new Size(DeviceToLogicalUnits(deviceSize.Width, deviceDpi),
                            DeviceToLogicalUnits(deviceSize.Height, deviceDpi));
        }

        /// <summary>
        /// Returns a new Rectangle with the input's
        /// dimensions converted from device units to logical units.
        /// </summary>
        /// <param name="logicalSize">Size in logical units</param>
        /// <param name="extendToPartialPixels">Round down the top left position and round up the bottom right position of the rectangle to the nearest integers</param>
        /// <returns>Size in device units</returns>
        public static Rectangle DeviceToLogicalUnits(Rectangle deviceRect, int deviceDpi = 0, bool extendToPartialPixels = false)
        {
            if (extendToPartialPixels)
            {
                double scalingFactor = deviceDpi == 0 ? 1 / LogicalToDeviceUnitsScalingFactor : LogicalDpi / deviceDpi;
                Point bottomRight = deviceRect.Location + deviceRect.Size;
                Point scaledLocation = new Point((int)Math.Floor(scalingFactor * deviceRect.X), (int)Math.Floor(scalingFactor * deviceRect.Y));
                Point scaledBottomRight = new Point((int)Math.Ceiling(scalingFactor * bottomRight.X), (int)Math.Ceiling(scalingFactor * bottomRight.Y));
                Size scaledSize = new Size(scaledBottomRight) - new Size(scaledLocation);
                return new Rectangle(scaledLocation, scaledSize);
            }
            return new Rectangle(DeviceToLogicalUnits(deviceRect.X, deviceDpi),
                            DeviceToLogicalUnits(deviceRect.Y, deviceDpi),
                            DeviceToLogicalUnits(deviceRect.Width, deviceDpi),
                            DeviceToLogicalUnits(deviceRect.Height, deviceDpi));
        }

        /// <summary>
        /// Returns a new Padding with the input's
        /// dimensions converted from device units to logical units.
        /// </summary>
        /// <param name="logicalPadding">Padding in logical units</param>
        /// <returns>Size in device units</returns>
        public static Padding DeviceToLogicalUnits(Padding devicePadding, int deviceDpi = 0)
        {
            return new Padding(DeviceToLogicalUnits(devicePadding.Left, deviceDpi),
                            DeviceToLogicalUnits(devicePadding.Top, deviceDpi),
                            DeviceToLogicalUnits(devicePadding.Right, deviceDpi),
                            DeviceToLogicalUnits(devicePadding.Bottom, deviceDpi));
        }

        /// <summary>
        /// Create and return a new bitmap scaled to the specified size.
        /// </summary>
        /// <param name="logicalImage">The image to scale from logical units to device units</param>
        /// <param name="targetImageSize">The size to scale image to</param>
        public static Bitmap CreateResizedBitmap(Bitmap logicalImage, Size targetImageSize)
        {
            if (logicalImage == null)
            {
                return null;
            }

            return ScaleBitmapToSize(logicalImage, targetImageSize);
        }

        /// <summary>
        /// Create a new bitmap scaled for the device units.
        /// When displayed on the device, the scaled image will have same size as the original image would have when displayed at 96dpi.
        /// </summary>
        /// <param name="logicalBitmap">The image to scale from logical units to device units</param>
        public static void ScaleBitmapLogicalToDevice(ref Bitmap logicalBitmap, int deviceDpi = 0)
        {
            if (logicalBitmap == null)
            {
                return;
            }
            Bitmap deviceBitmap = CreateScaledBitmap(logicalBitmap, deviceDpi);
            if (deviceBitmap != null)
            {
                logicalBitmap.Dispose();
                logicalBitmap = deviceBitmap;
            }
        }

        // This method is used only in System.Design, thus excluding the rest.
        // This is particularly important for System.Drawing, which should not depend 
        // on System.Windows.Forms assembly, where "Button" type is defined. 
#if (!DRAWING_NAMESPACE && !DRAWINGDESIGN_NAMESPACE && !WINFORMS_NAMESPACE)
        /// <summary>
        /// Create a new button bitmap scaled for the device units. 
        /// Note: original image might be disposed.
        /// </summary>
        /// <param name="button">button with an image, image size is defined in logical units</param>
        public static void ScaleButtonImageLogicalToDevice(Button button)
        {
            if (button == null)
            {
                return;
            }
            Bitmap buttonBitmap = button.Image as Bitmap;
            if (buttonBitmap == null)
            {
                return;
            }
            Bitmap deviceBitmap = CreateScaledBitmap(buttonBitmap);
            button.Image.Dispose();
            button.Image = deviceBitmap;
        }
#endif

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
