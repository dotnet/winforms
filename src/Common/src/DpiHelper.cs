// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

using CAPS = System.Windows.Forms.NativeMethods;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Helper class for scaling coordinates and images according to current DPI scaling set in Windows for the primary screen.
    /// </summary>
    internal static partial class DpiHelper
    {
        internal const double LogicalDpi = 96.0;
        private static bool s_isInitialized = false;
        private static bool s_isInitializeDpiHelperForWinforms = false;

        /// <summary>
        ///  The primary screen's (device) current DPI
        /// </summary>
        private static double s_deviceDpi = LogicalDpi;
        private static double s_logicalToDeviceUnitsScalingFactor = 0.0;
        private static InterpolationMode s_interpolationMode = InterpolationMode.Invalid;

        // Backing field, indicating that we will need to send a PerMonitorV2 query in due course.
        private static bool s_doesNeedQueryForPerMonitorV2Awareness = false;

        // Backing field, indicating that either DPI is <> 96 or we are in some PerMonitor HighDpi mode.
        private static bool s_isScalingRequirementMet = false;

        private static void Initialize()
        {
            if (s_isInitialized)
                return;

            using ScreenDC dc = ScreenDC.Create();
            s_deviceDpi = Interop.Gdi32.GetDeviceCaps(dc, Interop.Gdi32.DeviceCapability.LOGPIXELSX);
            s_isInitialized = true;
        }

        internal static void InitializeDpiHelperForWinforms()
        {
            if (s_isInitializeDpiHelperForWinforms)
            {
                return;
            }

            // initialize shared fields
            Initialize();

            if (OsVersion.IsWindows10_1607OrGreater)
            {
                // We are on Windows 10/1603 or greater, but we could still be DpiUnaware or SystemAware, so let's find that out...
                var currentProcessId = SafeNativeMethods.GetCurrentProcessId();
                IntPtr hProcess = SafeNativeMethods.OpenProcess(SafeNativeMethods.PROCESS_QUERY_INFORMATION, false, currentProcessId);
                SafeNativeMethods.GetProcessDpiAwareness(hProcess, out CAPS.PROCESS_DPI_AWARENESS processDpiAwareness);

                // Only if we're not, it makes sense to query for PerMonitorV2 awareness from now on, if needed.
                if (!(processDpiAwareness == CAPS.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE ||
                      processDpiAwareness == CAPS.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE))
                {
                    s_doesNeedQueryForPerMonitorV2Awareness = true;
                }
            }

            if (IsScalingRequired || s_doesNeedQueryForPerMonitorV2Awareness)
            {
                s_isScalingRequirementMet = true;
            }

            s_isInitializeDpiHelperForWinforms = true;
        }

        internal static bool DoesCurrentContextRequireScaling
            => true;

        /// <summary>
        ///  Returns a boolean to specify if we should enable processing of WM_DPICHANGED and related messages
        /// </summary>
        internal static bool IsPerMonitorV2Awareness
        {
            get
            {
                InitializeDpiHelperForWinforms();
                if (s_doesNeedQueryForPerMonitorV2Awareness)
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
        ///  Indicates, if rescaling becomes necessary, either because we are not 96 DPI or we're PerMonitorV2Aware.
        /// </summary>
        internal static bool IsScalingRequirementMet
        {
            get
            {
                InitializeDpiHelperForWinforms();
                return s_isScalingRequirementMet;
            }
        }

        internal static int DeviceDpi
        {
            get
            {
                Initialize();
                return (int)s_deviceDpi;
            }
        }

        private static double LogicalToDeviceUnitsScalingFactor
        {
            get
            {
                if (s_logicalToDeviceUnitsScalingFactor == 0.0)
                {
                    Initialize();
                    s_logicalToDeviceUnitsScalingFactor = s_deviceDpi / LogicalDpi;
                }
                return s_logicalToDeviceUnitsScalingFactor;
            }
        }

        private static InterpolationMode InterpolationMode
        {
            get
            {
                if (s_interpolationMode == InterpolationMode.Invalid)
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
                        s_interpolationMode = InterpolationMode.NearestNeighbor;
                    }
                    else if (dpiScalePercent < 100)
                    {
                        s_interpolationMode = InterpolationMode.HighQualityBilinear;
                    }
                    else
                    {
                        s_interpolationMode = InterpolationMode.HighQualityBicubic;
                    }
                }
                return s_interpolationMode;
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
            Size deviceImageSize = LogicalToDeviceUnits(logicalImage.Size, deviceDpi);
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
                return s_deviceDpi != LogicalDpi;
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
        ///  Create and return a new bitmap scaled to the specified size.
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
        ///  Creating bitmap from Icon resource
        /// </summary>
        public static Bitmap GetBitmapFromIcon(Type t, string name)
        {
            Icon b = new Icon(t, name);
            Bitmap bitmap = b.ToBitmap();
            b.Dispose();
            return bitmap;
        }

        /// <summary>
        ///  Create a new bitmap scaled for the device units.
        ///  When displayed on the device, the scaled image will have same size as the original image would have when displayed at 96dpi.
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

        /// <summary>
        ///  Indicates whether the first (Parking)Window has been created. From that moment on,
        ///  we will not be able nor allowed to change the Process' DpiMode.
        /// </summary>
        internal static bool FirstParkingWindowCreated { get; set; }

        /// <summary>
        ///  Gets the DPI awareness.
        /// </summary>
        /// <returns>The thread's/process' current HighDpi mode</returns>
        internal static HighDpiMode GetWinformsApplicationDpiAwareness()
        {
            // For Windows 10 RS2 and above
            if (OsVersion.IsWindows10_1607OrGreater)
            {
                DpiAwarenessContext dpiAwareness = CommonUnsafeNativeMethods.GetThreadDpiAwarenessContext();

                if (CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwareness, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
                {
                    return HighDpiMode.SystemAware;
                }

                if (CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwareness, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNAWARE))
                {
                    return HighDpiMode.DpiUnaware;
                }

                if (CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwareness, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2))
                {
                    return HighDpiMode.PerMonitorV2;
                }

                if (CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwareness, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE))
                {
                    return HighDpiMode.PerMonitor;
                }

                if (CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwareness, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED))
                {
                    return HighDpiMode.DpiUnawareGdiScaled;
                }
            }
            else if (OsVersion.IsWindows8_1OrGreater)
            {
                SafeNativeMethods.GetProcessDpiAwareness(IntPtr.Zero, out CAPS.PROCESS_DPI_AWARENESS processDpiAwareness);
                switch (processDpiAwareness)
                {
                    case CAPS.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE:
                        return HighDpiMode.DpiUnaware;
                    case CAPS.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE:
                        return HighDpiMode.SystemAware;
                    case CAPS.PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE:
                        return HighDpiMode.PerMonitor;
                }
            }
            else
            {
                // Available on Vista and higher
                return SafeNativeMethods.IsProcessDPIAware() ?
                       HighDpiMode.SystemAware :
                       HighDpiMode.DpiUnaware;
            }

            // We should never get here, except someone ported this with force to < Windows 7.
            return HighDpiMode.DpiUnaware;
        }

        /// <summary>
        ///  Sets the DPI awareness. If not available on the current OS, it falls back to the next possible.
        /// </summary>
        /// <returns>true/false - If the process DPI awareness is successfully set, returns true. Otherwise false.</returns>
        internal static bool SetWinformsApplicationDpiAwareness(HighDpiMode highDpiMode)
        {
            CAPS.PROCESS_DPI_AWARENESS dpiFlag = CAPS.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNINITIALIZED;

            if (OsVersion.IsWindows10_1703OrGreater)
            {
                // SetProcessDpiAwarenessContext needs Windows 10 RS2 and above

                int rs2AndAboveDpiFlag;
                switch (highDpiMode)
                {
                    case HighDpiMode.SystemAware:
                        rs2AndAboveDpiFlag = CAPS.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE;
                        break;
                    case HighDpiMode.PerMonitor:
                        rs2AndAboveDpiFlag = CAPS.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE;
                        break;
                    case HighDpiMode.PerMonitorV2:
                        // Necessary for RS1, since this SetProcessDpiAwarenessContext IS available here.
                        rs2AndAboveDpiFlag = SafeNativeMethods.IsValidDpiAwarenessContext(CAPS.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2) ?
                                             CAPS.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 :
                                             CAPS.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE;
                        break;
                    case HighDpiMode.DpiUnawareGdiScaled:
                        // Let's make sure, we do not try to set a value which has been introduced in later Windows releases.
                        rs2AndAboveDpiFlag = SafeNativeMethods.IsValidDpiAwarenessContext(CAPS.DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED) ?
                                             CAPS.DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED :
                                             CAPS.DPI_AWARENESS_CONTEXT_UNAWARE;
                        break;
                    default:
                        rs2AndAboveDpiFlag = CAPS.DPI_AWARENESS_CONTEXT_UNAWARE;
                        break;
                }
                return SafeNativeMethods.SetProcessDpiAwarenessContext(rs2AndAboveDpiFlag);
            }
            else if (OsVersion.IsWindows8_1OrGreater)
            {
                // 8.1 introduced SetProcessDpiAwareness

                switch (highDpiMode)
                {
                    case HighDpiMode.DpiUnaware:
                    case HighDpiMode.DpiUnawareGdiScaled:
                        dpiFlag = CAPS.PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE;
                        break;
                    case HighDpiMode.SystemAware:
                        dpiFlag = CAPS.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE;
                        break;
                    case HighDpiMode.PerMonitor:
                    case HighDpiMode.PerMonitorV2:
                        dpiFlag = CAPS.PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE;
                        break;
                    default:
                        dpiFlag = CAPS.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE;
                        break;
                }

                return SafeNativeMethods.SetProcessDpiAwareness(dpiFlag) == CAPS.S_OK;
            }
            else
            {
                // Vista or higher has SetProcessDPIAware

                switch (highDpiMode)
                {
                    case HighDpiMode.DpiUnaware:
                    case HighDpiMode.DpiUnawareGdiScaled:
                        // We can return, there is nothing to set if we assume we're already in DpiUnaware.
                        return true;
                    case HighDpiMode.SystemAware:
                    case HighDpiMode.PerMonitor:
                    case HighDpiMode.PerMonitorV2:
                        dpiFlag = CAPS.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE;
                        break;
                }

                if (dpiFlag == CAPS.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE)
                {
                    return SafeNativeMethods.SetProcessDPIAware();
                }
            }
            return false;
        }
    }

    internal enum DpiAwarenessContext
    {
        DPI_AWARENESS_CONTEXT_UNSPECIFIED = 0,
        DPI_AWARENESS_CONTEXT_UNAWARE = -1,
        DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = -2,
        DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = -3,
        DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4,
        DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED = -5
    }
}
